using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class UploadComponentMaxScores : System.Web.UI.Page
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + " Edit TQM Boundary Data";
            }
        }

        protected void DropDownList_ExamBoards_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExamOption_List exol1 = new ExamOption_List();
            exol1.Load(DropDownList_ExamBoards.SelectedValue, YearCode.ToString(), SeasonCode.ToString());
            DropDownList_Options.Items.Clear();
            foreach (ExamOption e1 in exol1.m_list)//only add if we HAVE components!!
            {
                ExamCompononent_List ecl1 = new ExamCompononent_List();ecl1.Load(e1.m_OptionID);
                if (ecl1.m_list.Count > 0)
                {
                    ListItem l1 = new ListItem(e1.m_OptionCode + ":" + e1.m_OptionTitle, e1.m_OptionID.ToString());
                    DropDownList_Options.Items.Add(l1);
                }
            }
        }

        protected void DropDownList_Options_SelectedIndexChanged(object sender, EventArgs e)
        {
            //so option selection .. need to get TQM data
            DropDownList_Component.Items.Clear();
            ExamCompononent_List ecl1 = new ExamCompononent_List();
            ecl1.Load(new Guid(DropDownList_Options.SelectedItem.Value));
            if (ecl1.m_list.Count > 0)
            { 
            foreach (ExamComponent ec1 in ecl1.m_list)
            {
                DropDownList_Component.Items.Add(new System.Web.UI.WebControls.ListItem(ec1.m_ComponentCode + " : " + ec1.m_ComponentTitle, ec1.m_ComponentID.ToString()));
            }
            DropDownList_Components_SelectedIndexChanged(sender, e);
            }
        }


        protected void DropDownList_Components_SelectedIndexChanged(object sender, EventArgs e)
        {
            //so a component is selected
            ExamComponent ec1 = new ExamComponent();ec1.Load(new Guid(DropDownList_Component.SelectedValue.ToString()));
            TextBoxValue.Text = ec1.m_MaximumMark;
            Label1.Text = "Current Max Mark for " + ec1.m_ComponentCode + " is:";
        }

        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            //so do it?
            ExamComponent ec1 = new ExamComponent(); ec1.Load(new Guid(DropDownList_Component.SelectedValue.ToString()));
            ec1.m_MaximumMark = TextBoxValue.Text;
            ec1.Save();

        }
    }
}