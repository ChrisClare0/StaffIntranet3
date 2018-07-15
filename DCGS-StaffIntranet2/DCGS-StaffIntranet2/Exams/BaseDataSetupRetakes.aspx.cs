using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class BaseDataSetupRetakes : System.Web.UI.Page
    {
        #region variables

        public int Year
        {   //this is the year in 2016 format
            get { try { return (int)ViewState["Year"]; } catch { return 0; } }
            set { ViewState["Year"] = value; }
        }
        public int YearCode
        {   //this is the year in 16 format
            get { try { return (int)ViewState["YearCode"]; } catch { return 0; } }
            set { ViewState["YearCode"] = value; }
        }
        public int SeasonCode
        {
            get { try { return (int)ViewState["SeasonCode"]; } catch { return 0; } }
            set { ViewState["SeasonCode"] = value; }
        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetMasterValues();
                string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
                ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
            }
        }
        private void GetMasterValues()
        {
            SeasonCode = System.Convert.ToInt32((string)(Session["Season"]));
            Year = System.Convert.ToInt32((string)(Session["Year"]));
            YearCode = Year % 100;
            Label Label_Banner = (Label)Master.FindControl("Label_Banner");
            if (Label_Banner != null)
            {
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "    Setting up for Student Retakes.";
            }
        }

        private void SetUpForReatkes()
        {
            List<ExamOption> list1 = new List<ExamOption>();
            int no_new = 0; int no_found = 0;string Errors = "The following Options seem not to be offered this summer:  ";
            ExamOption_List exoL1 = new ExamOption_List();
            ExamFiles ef1 = new ExamFiles();
            ExamOption exo1 = new ExamOption();
            //load results for last JUNE sitting now.

            exoL1.Load((YearCode - 1).ToString(), SeasonCode.ToString());

            foreach (ExamOption exo in exoL1.m_list)
            {
                Exam_Board eb1 = new Exam_Board(exo.m_ExamBoardID);
                exo1.Load(exo.m_OptionCode, SeasonCode.ToString(), YearCode.ToString(), eb1.m_LegacyExamBdId);
                if (exo1.m_valid)  no_found++;
                else
                {
                    if (!exo.NeedsTeacherMarks())
                    {
                        ExamOption exo2 = ef1.Find_Option(exo.m_OptionCode, eb1, SeasonCode.ToString(), YearCode.ToString());
                        if (exo2 != null) no_new++;
                        else Errors +=  exo.m_OptionCode + ",  ";
                    }
                    else  list1.Add(exo);//list has those needing teacher marks
                }

            }
            Label_Result.Text = "We made  " + no_new.ToString() + " new options and found already " + no_found.ToString();
            Label_Error.Text = Errors;
            //now need to offer those with coursework...
            if (list1.Count > 0)
            {
                DropDownList1.Visible = true;Button_Upload.Visible = true;
                foreach (ExamOption e in list1)
                {
                    ListItem l = new ListItem(e.m_OptionCode + e.m_OptionTitle, e.m_OptionID.ToString());
                    DropDownList1.Items.Add(l);
                }
                LabelDropDown.Text = "The list below are options that require Teacher marks... select and add any you want students to see.";
            }
        }

        protected void Button_Run_Click(object sender, EventArgs e)
        {
            SetUpForReatkes();
        }

        protected void Button_Upload_Click(object sender, EventArgs e)
        {
            Guid id = new Guid(DropDownList1.SelectedValue);
            ExamOption exo = new ExamOption();exo.Load(id);
            Exam_Board eb1 = new Exam_Board(exo.m_ExamBoardID);
            ExamOption exo1 = new ExamOption();
            ExamFiles ef1 = new ExamFiles();
            exo1.Load(exo.m_OptionCode, SeasonCode.ToString(), YearCode.ToString(), eb1.m_LegacyExamBdId);
            if (!exo1.m_valid)
            {
                ExamOption exo2 = ef1.Find_Option(exo.m_OptionCode, eb1, SeasonCode.ToString(), YearCode.ToString());
                if (exo2 != null)
                {
                    Label_Result.Text = "Successfully uploaded " + exo.m_OptionCode + " : " + exo.m_OptionTitle + " to Cerval";
                    DropDownList1.SelectedIndex = -1;
                    ListItem l = DropDownList1.SelectedItem;
                    DropDownList1.Items.Remove(l);
                }
                else
                {
                    Label_Result.Text = "Failed to upload " + exo.m_OptionCode + " : " + exo.m_OptionTitle + " to Cerval";
                }
            }
        }
    }
}