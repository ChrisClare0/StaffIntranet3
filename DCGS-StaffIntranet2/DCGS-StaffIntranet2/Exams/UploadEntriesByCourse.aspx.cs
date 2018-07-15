using System;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class UploadEntriesByCourse : System.Web.UI.Page
    {

        #region variables

        public string DisplayType
        {
            get { return (((String)ViewState["Type"] == null) ? String.Empty : (String)ViewState["Type"]); }
            set { ViewState["Type"] = value; }
        }

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
                TextBox_Date.Text = DateTime.Now.ToString("dd/MM/yyyy");
                //TextBox_Date.Text ="01/02/2014";
                GetMasterValues();
                Encode en = new Encode();
                SqlDataSource_cses.SelectCommand = GetSQLCseString(DropDownList_Year.SelectedValue);
                SqlDataSource_cses.ConnectionString = en.GetDbConnection();
                SqlDataSource_cses.DataBind();
                DropDownList_Course.DataTextField = "CSE";
                DropDownList_Course.DataValueField = "ID";

                SqlDataSource_opts.SelectCommand = GetSQLOptString();
                SqlDataSource_opts.ConnectionString = en.GetDbConnection();
                SqlDataSource_opts.DataBind();
                ListBox_Options.DataTextField = "Expr1";
                ListBox_Options.DataValueField = "ID";

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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) +" : Enter Students by Course";
            }
        }

        protected string GetSQLCseString(string year)
        {
            string KS = "3";
            switch (year)
            {
                case "13":
                case "12":
                    KS = "3";
                    break;
                case "11":
                case "10":
                    KS = "2";
                    break;          
                default:
                    KS = "1";
                    break;
            }
            DateTime t1 = new DateTime();t1 = System.Convert.ToDateTime(TextBox_Date.Text);
            string ds = "CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";
            string s = "";
            s = "SELECT DISTINCT dbo.tbl_Core_Courses.CourseName  AS CSE, dbo.tbl_Core_Groups.CourseId AS ID  ";
            s += "FROM dbo.tbl_Core_ScheduledPeriodValidity INNER JOIN  ";
            s += "dbo.tbl_Core_ScheduledPeriods ON dbo.tbl_Core_ScheduledPeriodValidity.ScheduledPeriodId = dbo.tbl_Core_ScheduledPeriods.ScheduledPeriodId ";
            s += " INNER JOIN dbo.tbl_Core_Groups ON dbo.tbl_Core_ScheduledPeriods.GroupId = dbo.tbl_Core_Groups.GroupId INNER JOIN ";
            s += " dbo.tbl_Core_Courses ON dbo.tbl_Core_Groups.CourseId = dbo.tbl_Core_Courses.CourseId ";
            s += " WHERE(dbo.tbl_Core_Courses.CourseType = '"+KS+"') ";
            s+=" AND(dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart < "+ds+") ";
            s += " AND(dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd > "+ds+" )  ";
            return s;
        }
        protected string GetSQLOptString()
        {
            string s = "SELECT TOP (100) PERCENT { fn CONCAT({ fn CONCAT(OptionCode, '   :   ') }, OptionTitle) }    AS Expr1 , ";
            s += " OptionID AS ID  ";
            s += " FROM dbo.tbl_Exams_Options ";
            s += " WHERE        (YearCode = '"+YearCode.ToString()+"') AND(SeasonCode = '"+SeasonCode.ToString()+"')  ORDER BY OptionCode  ";
            return s;
        }

        protected void DropDownList_Year_SelectedIndexChanged(object sender, EventArgs e)
        {
            //new year....7
            Encode en = new Encode();
            SqlDataSource_cses.SelectCommand = GetSQLCseString(DropDownList_Year.SelectedValue);
            SqlDataSource_cses.ConnectionString = en.GetDbConnection();
            SqlDataSource_cses.DataBind();
            DropDownList_Course.DataTextField = "CSE";
            DropDownList_Course.DataValueField = "ID";
        }

        protected void DropDownList_Course_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if we have a selection in options then enable test button
            foreach (ListItem item in ListBox_Options.Items)
            {
                if (item.Selected) ButtonTest.Visible = true;
            }

        }

        protected void ListBox_Options_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if we have a selection in courses then enable test button
            if(DropDownList_Course.SelectedIndex>0) ButtonTest.Visible = true;
        }

        protected void TextBox_Date_TextChanged(object sender, EventArgs e)
        {
            //test to see if date valid?
        }

        protected void ButtonTest_Click(object sender, EventArgs e)
        {
            //so going to test this....

            List<Guid> students = new List<Guid>();
            DateTime t1 = new DateTime(); t1 = System.Convert.ToDateTime(TextBox_Date.Text);

            string ds = "CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";
            string s = "SELECT dbo.tbl_Core_Student_Groups.StudentId, dbo.tbl_Core_Students.StudentAdmissionNumber, ";
            s += "dbo.tbl_Core_Groups.GroupCode, ";
            s += " dbo.tbl_Core_People.PersonGivenName, dbo.tbl_Core_People.PersonSurname ";
            s += " FROM dbo.tbl_Core_Groups INNER JOIN  ";
            s += "dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN  ";
            s += "dbo.tbl_Core_Students ON dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students.StudentId INNER JOIN  ";
            s += "dbo.tbl_Core_People ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId  ";
            s += "WHERE(dbo.tbl_Core_Groups.CourseId = '" + DropDownList_Course.SelectedValue + "') ";
            s += " AND(dbo.tbl_Core_Student_Groups.MemberFrom < " + ds+" ) ";
            s += " AND(dbo.tbl_Core_Student_Groups.MemberUntil > " + ds + ")";

            Encode en = new Encode();int n = 0;
            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            s = dr.GetString(2);
                            if (s.StartsWith(DropDownList_Year.SelectedValue))
                            {
                                TextBox2.Text += dr.GetInt32(1).ToString() + " : " + dr.GetString(3) + "  " + dr.GetString(4);
                                TextBox2.Text += "("+dr.GetString(2)+")"+Environment.NewLine;n++;
                                students.Add(dr.GetGuid(0));
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            TextBox1.Text = "Found the following " + n.ToString() + " students to enter for:";
            foreach (ListItem item in ListBox_Options.Items)
            {
                if (item.Selected) TextBox1.Text += item.Text + ", ";
            }
            TextBox1.Visible = true;
            TextBox2.Visible = true;
            ViewState["studentlist"] = students;
            Button_Enter.Visible = true;
        }

        protected void Button_Enter_Click(object sender, EventArgs e)
        {
            //do the entry....
            List<Guid> students = new List<Guid>();
            students = (List<Guid>)ViewState["studentlist"];
            List<Guid> Options = new List<Guid>();
            foreach (ListItem item in ListBox_Options.Items)
            {
                if (item.Selected) Options.Add(new Guid(item.Value));
            }
            ExamsUtility u = new ExamsUtility();
            string Errors = "";Guid g2 = new Guid();
            int n = 0;
            foreach (Guid g in Options)
            {
                ExamOption eo1 = new ExamOption();eo1.Load(g);
                Exam_Board eb1 = new Exam_Board(eo1.m_ExamBoardID);
                foreach(Guid g1 in students)
                {
                    //try { 
                    u.AddEntry(g1, eb1, Year, YearCode, SeasonCode, eo1.m_OptionCode, 2, false, ref Errors, ref g2);
                        n++;
                    //}
                    //catch
                    //{
                    //    n = n;
                   // }
                }
            }
            TextBox2.Text = "";TextBox2.Visible = false;
            TextBox1.Text = " Made " + n.ToString() + "  Entries";
            ButtonTest.Visible = false;
            Button_Enter.Visible = false;
            DropDownList_Course.SelectedIndex = -1;
            foreach (ListItem item in ListBox_Options.Items)
            {
                if (item.Selected) item.Selected = false;
            }


        }

        protected void Button_Opt_Find_Click(object sender, EventArgs e)
        {
            //find code...
            string s = TextBox_FindOption.Text.ToUpper().Trim();
            bool found = false;
            foreach (ListItem item in ListBox_Options.Items)
            {
                if (item.Text.Substring(0, 6).ToUpper().Trim() == s)
                {
                    item.Selected = true;found = true; TextBox_FindOption.Text = "";
                }          
            }
            if (found) return;
            found = false; ExamOption eo1 = new ExamOption();
             ExamBoardList ebl1 = new ExamBoardList();ExamFiles ef1 = new ExamFiles();
            foreach(Exam_Board eb in ebl1._ExamBoardList)
            {
                eo1 = ef1.Find_Option(s, eb, SeasonCode.ToString(), YearCode.ToString());
                if (eo1 != null)
                {
                    found = true;break;
                }
            }
            if (!found)
            {
                TextBox1.Text = "NOT FOUND!!!!";TextBox1.Visible = true;return;
            }

            ListItem i1 = new ListItem(eo1.m_OptionCode + ":" + eo1.m_OptionTitle, eo1.m_OptionID.ToString());
            ListBox_Options.Items.Add(i1);
            foreach (ListItem item in ListBox_Options.Items)
            {
                if (item.Text.Substring(0, 6).ToUpper().Trim() == s)
                {
                    item.Selected = true; found = true; TextBox_FindOption.Text = "";
                }
            }

        }
    }
}