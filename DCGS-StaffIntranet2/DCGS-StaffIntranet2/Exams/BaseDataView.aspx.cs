using System;
using Cerval_Library;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class BaseDataView : System.Web.UI.Page
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
                ExamBoardList eb1 = new ExamBoardList();
                DropDownList_ExamBoards.Items.Clear();
                foreach (Exam_Board eb in eb1._ExamBoardList)
                {
                    ListItem l1 = new ListItem(eb.ToString(), eb.m_ExamBoardId.ToString());
                    DropDownList_ExamBoards.Items.Add(l1);
                }
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Viewing ExamBoard BaseData";
            }
        }

        protected void DropDownList_ExamBoards_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList_ExamBoards.SelectedIndex>-1)
            {
                string id = DropDownList_ExamBoards.SelectedValue;
                Exam_Board eb = new Exam_Board(new Guid(id));
                ExamFiles ef = new ExamFiles();
                DropDownList_data.Items.Clear();
                foreach (ExamBaseOption ebo in ef.ExamsOptionsFromBaseData(eb, SeasonCode.ToString(), YearCode.ToString()))
                {
                    ListItem l = new ListItem(ebo.ToString(), ebo.m_OptionEntryCode);
                    DropDownList_data.Items.Add(l);
                }
            }
        }

        protected void DropDownList_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList_data.SelectedIndex > -1)
            {
                string id = DropDownList_ExamBoards.SelectedValue; string s = "";
                Exam_Board eb = new Exam_Board(new Guid(id));
                ExamFiles ef = new ExamFiles();
                foreach (ExamBaseOption ebo in ef.ExamsOptionsFromBaseData(eb, SeasonCode.ToString(), YearCode.ToString()))
                {
                    if (ebo.m_OptionEntryCode == DropDownList_data.SelectedValue)
                    {
                        s = "Option Code:           " + ebo.m_OptionEntryCode + Environment.NewLine;
                        s += "Option Title:          " + ebo.m_Title + Environment.NewLine;
                        s += "Qualification/Level:   " + ebo.m_Qualification + ebo.m_Level + Environment.NewLine;
                        s += "Cost:                  " + ebo.m_Fee + Environment.NewLine;
                        s += "SyllabusCode:          " + ebo.m_SyllabusCode + Environment.NewLine;
                        s += "Item (C=Cert.):        " + ebo.m_Item + Environment.NewLine;
                        s += "QCA Number:             " + ebo.m_QCANumber + Environment.NewLine;
                        ExamOption eo1 = new ExamOption();
                        if (eo1.Load(DropDownList_data.SelectedValue, SeasonCode.ToString(), YearCode.ToString(), eb.m_ExamBoardId))
                        {
                            s += Environment.NewLine + Environment.NewLine + "Option is known to Cerval";
                        }
                        else
                        {
                            s += Environment.NewLine + Environment.NewLine + "Option is NOT known to Cerval";
                        }
                        

                        TextBox1.Text = s;

                    }
                }

            }
        }

        protected void Button_Upload_Click(object sender, EventArgs e)
        {
            string id = DropDownList_ExamBoards.SelectedValue;
            Exam_Board eb1 = new Exam_Board(new Guid(id));
            string s1=DropDownList_data.SelectedValue;
            ExamOption exo1 = new ExamOption();
            ExamFiles ef1 = new ExamFiles();
            exo1.Load(s1, SeasonCode.ToString(), YearCode.ToString(), eb1.m_LegacyExamBdId);
            if (!exo1.m_valid)
            {
                ExamOption exo2 = ef1.Find_Option(s1, eb1, SeasonCode.ToString(), YearCode.ToString());
                if (exo2 != null)
                {
                    //Label_Result.Text = "Successfully uploaded " + exo.m_OptionCode + " : " + exo.m_OptionTitle + " to Cerval";
                    //DropDownList1.SelectedIndex = -1;
                    //ListItem l = DropDownList1.SelectedItem;
                    //DropDownList1.Items.Remove(l);
                }
                else
                {
                    //Label_Result.Text = "Failed to upload " + exo.m_OptionCode + " : " + exo.m_OptionTitle + " to Cerval";
                }
            }
        }
    }
}