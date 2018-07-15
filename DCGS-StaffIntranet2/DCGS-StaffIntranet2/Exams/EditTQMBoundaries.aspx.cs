using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class EditTQMBoundaries : System.Web.UI.Page
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
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("A*","A*"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("A","A"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("B","B"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("C","C"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("D","D"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("E","E"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("9", "9"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("8", "8"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("7", "7"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("6", "6"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("5", "5"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("4", "4"));
                DropDownList_Grades.Items.Add(new System.Web.UI.WebControls.ListItem("3", "3"));
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
            foreach(ExamOption e1  in exol1.m_list)
            {
                ListItem l1 = new ListItem(e1.m_OptionCode + ":" + e1.m_OptionTitle, e1.m_OptionID.ToString());
                DropDownList_Options.Items.Add(l1);
            }
        }

        protected void DropDownList_Options_SelectedIndexChanged(object sender, EventArgs e)
        {
            //so option selection .. need to get TQM data
            ExamTQMBoundaryList bl1 = new ExamTQMBoundaryList();//will be ordered largest first
            bl1.LoadList(new Guid(DropDownList_Options.SelectedValue), YearCode.ToString(), SeasonCode.ToString());


            Current_Data.InnerHtml = UpdateCurrentData(bl1);
        }

        private string UpdateCurrentData(ExamTQMBoundaryList bl1)
        {
            string s = " <p><h3 align=\"center\" > Current TQM Data held for " + DropDownList_Options.SelectedItem.ToString() + "</h3></p>";
            s += "<TABLE BORDER   class=\"TimetableTable\" style = \"font-size:small\" align=\"center\"  >";
            s += "<tr><th>Grade</th><th>  TQM   </th></tr>";
            foreach (ExamTQMBoundary eb in bl1.m_list)
            {
                s += "<tr><td>" + eb.Grade + "</td><td>" + eb.Mark + "</td></tr>";
            }
            s += "</table>";
            return s;
        }

        protected void Button_Update_Click(object sender, EventArgs e)
        {
            //so first check we have a valid option and value
            int v = 0;
            try
            {
                v = System.Convert.ToInt32(TextBox_Value.Text);
            }
            catch
            {
                //flag error
                return;
            }
            ExamOption ex1 = new ExamOption();
            ex1.Load(new Guid(DropDownList_Options.SelectedValue));
            if(!ex1.m_valid)
            {
                //flag error
                return;
            }

            ExamTQMBoundaryList exTQMlist1 = new ExamTQMBoundaryList();
            exTQMlist1.LoadList(ex1.m_OptionID, YearCode.ToString(), SeasonCode.ToString());
            bool found = false;
            foreach(ExamTQMBoundary eb in exTQMlist1.m_list)
            {
                if(eb.Grade==DropDownList_Grades.SelectedItem.Value)
                {
                    found = true;
                    eb.Mark = v;
                    eb.Save();
                    break;
                }
            }
            if (!found)
            {
                ExamTQMBoundary eb1 = new ExamTQMBoundary();
                eb1.OptionId = ex1.m_OptionID;eb1.Season = SeasonCode.ToString();eb1.Year = YearCode.ToString();
                eb1.Grade = DropDownList_Grades.SelectedItem.Value;eb1.Mark = v;eb1.Version = 1;
                eb1.Save();
            }
            exTQMlist1.LoadList(ex1.m_OptionID, YearCode.ToString(), SeasonCode.ToString());
            Current_Data.InnerHtml = UpdateCurrentData(exTQMlist1);

        }

        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            ExamOption ex1 = new ExamOption();
            ex1.Load(new Guid(DropDownList_Options.SelectedValue));
            if (!ex1.m_valid)
            {
                //flag error
                return;
            }
            ExamTQMBoundaryList exTQMlist1 = new ExamTQMBoundaryList();
            exTQMlist1.LoadList(ex1.m_OptionID, YearCode.ToString(), SeasonCode.ToString());
            foreach (ExamTQMBoundary eb in exTQMlist1.m_list) eb.Delete();
            exTQMlist1.LoadList(ex1.m_OptionID, YearCode.ToString(), SeasonCode.ToString());
            Current_Data.InnerHtml = UpdateCurrentData(exTQMlist1);
        }
    }
}