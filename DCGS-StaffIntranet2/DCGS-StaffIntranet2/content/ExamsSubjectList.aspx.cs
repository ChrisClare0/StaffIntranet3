using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet
{
    public partial class StudentEntriesEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Encode en = new Encode();
            ExamsEntries_sql.SelectCommand = CreateQueryString();
            ExamsEntries_sql.ConnectionString = en.GetCervalConnectionString();
            ExamsYears_sql.ConnectionString = en.GetCervalConnectionString();
            GridView1.RowCommand += new GridViewCommandEventHandler(GridView1_RowCommand);
            if (!IsPostBack)
            {
                Utility u = new Utility();
                string exam_season = Request.QueryString["ExamSeason"];
                string exam_year = Request.QueryString["ExamYear"];
                string type = Request.QueryString["Task"];
                //string staff_code = u.GetStaffCodefromContext(Context);
                string staff_code = u.GetsStaffCodefromRequest(Request);
                string qual = "GCSE";
                string item = "C";
                ExamsEntries_Modules_sql.ConnectionString = en.GetCervalConnectionString();
                ExamsEntries_Modules_sql.SelectCommand = "SELECT DISTINCT [OptionCode], [OptionTitle] FROM [qry_Cerval_Exams_Entries] WHERE (ExamYear ='" + exam_year + "') AND (ExamSeason ='" + exam_season + "' ) AND (OptionQualification = '" + qual + "' )  AND (OptionItem = '" + item + "' ) ORDER BY [OptionTitle]";
                Label_ExamYear.Text = exam_year;
                Label_ExamSeason.Text = exam_season;
                Label_staffCode.Text = staff_code;
                DropDownList_ExamYear.DataBind();

                //todo list in reverse
                for( int i = 0; i<DropDownList_ExamYear.Items.Count;i++)
                {
                    if(DropDownList_ExamYear.Items[i].Text==exam_year)
                    {
                        DropDownList_ExamYear.SelectedIndex=i;
                    }
                }
                for (int i = 0; i < DropDownList1.Items.Count;i++ )
                {
                    if (DropDownList1.Items[i].Value.ToString() == exam_season)
                    {
                        DropDownList1.SelectedIndex = i;
                    }
                }
                Label_type.Text = type;
                if (type == "SubjectEntries") visibility(1);
                if (type == "Student")
                {
                    visibility(2);
                    string studentID = Request.QueryString["StudentID"];
                    Label_StudentID.Text = studentID;
                    SimplePupil p1 = new SimplePupil(); p1.Load(studentID);
                    Label_name.Text = p1.m_GivenName + "  " + p1.m_Surname + " (" + p1.m_adno.ToString() + ")";
                    Label_Title.Text = Label_name.Text;
                }
                if (type == "mylist") visibility(1);
                ExamsEntries_sql.SelectCommand = CreateQueryString();
                GridView1.DataBind();
            }
        }

        private void UpdateList2()
        {
            ExamsEntries_Modules_sql.SelectCommand = "SELECT DISTINCT [OptionCode], [OptionTitle] FROM [qry_Cerval_Exams_Entries] WHERE (ExamSeason ='" + Label_ExamSeason.Text + "' ) AND (ExamYear ='" + Label_ExamYear.Text + "' ) AND (OptionQualification = '" + DropDownList3.SelectedValue.ToString() + "' )  AND (OptionItem = '" + DropDownList4.SelectedValue.ToString() + "' )  ORDER BY [OptionTitle]";
            DropDownList2.Items.Clear();
            DropDownList2.DataBind();
        }
 

        private string CreateQueryString()
        {
            string s = "";
            if (Label_type.Text == "Student")
            {
                s = "SELECT  [ExamEntryID] , [PersonGivenName] , [PersonSurname] ,[StudentExamNumber], [OptionCode],[OptionTitle],  [DateEntered], [PredictedGrade]  FROM [qry_Cerval_Exams_Entries]  WHERE (StudentID ='" + Label_StudentID.Text + "' )  AND (ExamSeason ='" + Label_ExamSeason.Text + "' ) AND (ExamYear ='" + Label_ExamYear.Text + "' ) ORDER BY [PersonSurname] ASC";
            }
            if (Label_type.Text == "SubjectEntries")
            {
                s = "SELECT  [ExamEntryID] , [PersonGivenName] , [PersonSurname] ,[StudentExamNumber], [OptionCode],[OptionTitle],  [DateEntered], [PredictedGrade]  FROM [qry_Cerval_Exams_Entries]  WHERE (OptionCode ='" + Label_OptionCode.Text + "' )   AND (ExamSeason ='" + Label_ExamSeason.Text + "' ) AND (ExamYear ='" + Label_ExamYear.Text + "' ) ORDER BY [PersonSurname] ASC";

            }
            if (Label_type.Text == "mylist")
            {
                s = "SELECT  [ExamEntryID] , [PersonGivenName] , [PersonSurname] ,[StudentExamNumber], [OptionCode],[OptionTitle],  [DateEntered], [PredictedGrade]  FROM [qry_Cerval_Exams_Entries_Groups]  WHERE (OptionCode ='" + Label_OptionCode.Text + "' )   AND (ExamSeason ='" + Label_ExamSeason.Text + "' ) AND (ExamYear ='" + Label_ExamYear.Text + "' ) AND (StaffCode ='" + Label_staffCode.Text + "' )  ORDER BY [PersonSurname] ASC";
            }
            return s;
        }



        void visibility(int type)
        {
            //type 1 = normal list by subject...
            //type 2 = one student
            //3 for display timetable
            //4 for predicted grade edit
            Label_ExamSeason.Visible = false;
            Label_ExamYear.Visible = false;
            Label_EntryID_for_edit.Visible = false;
            ListBox_PredictedGrades.Visible = false;
            Button_Save_Prediction.Visible = false;
            Label_Title_for_Edit.Visible = false;
            Button_Edit_Cancel.Visible = false;

            if (type == 2)
            {
                DropDownList_ExamYear.Visible = true;
                DropDownList1.Visible = true;
                Label_Title.Visible = true;
                Label1.Visible = true;
                Label2.Visible = true;
                Button_Excel.Visible = true;
                DropDownList2.Visible = false;
                Label3.Visible = false;
                Label_Title.Text = Label_name.Text;
                Button_ShowTimetable.Visible = true;
                DropDownList3.Visible = false;
                DropDownList4.Visible = false;
                Label4.Visible = false;
                Label5.Visible = false;
            }
            if (type == 1)
            {
                Button_ShowTimetable.Visible = false;
                DropDownList_ExamYear.Visible = true;
                DropDownList1.Visible = true;
                DropDownList2.Visible = true;
                Label_Title.Visible = true;
                Label1.Visible = true;
                Label2.Visible = true;
                Label3.Visible = true;
                Button_Excel.Visible = true;
                GridView1.Visible = true;

            }
            if (type == 3)
            {
                DropDownList2.Visible = false;
                Label3.Visible = false;
                DropDownList_ExamYear.Visible = false;
                DropDownList1.Visible = false;
                Label_Title.Visible = false;
                Label1.Visible = false;
                Label2.Visible = false;
                Button_Excel.Visible = false;
                Button_ShowTimetable.Visible = false;
                DropDownList3.Visible = false;
                DropDownList4.Visible = false;
                Label4.Visible = false;
                Label5.Visible = false;
            }
            if (type == 4)
            {
                DropDownList2.Visible = false;
                Label3.Visible = false;
                DropDownList_ExamYear.Visible = false;
                DropDownList1.Visible = false;
                Label_Title.Visible = false;
                Label1.Visible = false;
                Label2.Visible = false;
                Button_Excel.Visible = false;
                Button_ShowTimetable.Visible = false;
                GridView1.Visible = false;
                ListBox_PredictedGrades.Visible = true;
                Button_Save_Prediction.Visible = true;
                Label_Title_for_Edit.Visible = true;
                Button_Edit_Cancel.Visible = true;
                DropDownList3.Visible = false;
                DropDownList4.Visible = false;
                Label4.Visible = false;
                Label5.Visible = false;
            }
        }

        void GridView1_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            //calls here for any command - including a sort
            if (e.CommandName == "Edit_Grade")
            {
                Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_Forecast_Grade_Edit");
                string s1 = c.Value;
                if (!c.valid)//try revert to config file
                {
                    System.Configuration.AppSettingsReader ar = new AppSettingsReader();
                    s1 = ar.GetValue("Exam Entries Forecast Grade Edit", s1.GetType()).ToString();
                }
                if (s1 == "none")
                {
                    visibility(4);
                    {
                        ListBox_PredictedGrades.Visible = false;
                        Button_Save_Prediction.Visible = false;
                        Label_Title_for_Edit.Text = "Predicted grades can't be edited at present";
                        return;
                    }
                }


                int row = Convert.ToInt32(e.CommandArgument);
                GridViewRow row1 = GridView1.Rows[row];
                string s = Server.HtmlDecode(row1.Cells[0].Text);//is id
                Exam_Entry ex1 = new Exam_Entry(); ex1.Load(s);
                SimplePupil p1 = new SimplePupil();p1.Load(ex1.m_StudentID.ToString());
                ExamOption exo1 = new ExamOption();exo1.Load(ex1.m_OptionID);
                Utility u = new Utility();
                bool valid = false;
                PupilPeriodList PPLlist1 = new PupilPeriodList(); 
                PPLlist1.LoadList("StudentID", ex1.m_StudentID.ToString(), true, DateTime.Now);
                //now I'd like to find the cse... but we dont know these
                CourseList cselist1 = new CourseList(ex1.m_OptionID);
                GroupList_SL grplist1 = new GroupList_SL(Label_staffCode.Text.Trim());

                foreach (Course c1 in cselist1._courses)
                {
                    foreach (ScheduledPeriod scp in PPLlist1.m_pupilTTlist)
                    {
                        if (scp.m_groupcode.Substring(2, 2).ToUpper().Trim() == c1.CourseCode.ToUpper().Trim())
                        {
                            if (scp.m_staffcode.Trim().ToUpper() == Label_staffCode.Text.Trim().ToUpper())
                            {
                                if (s1 == "staff") valid = true;
                            } 
                            foreach (Group g1 in grplist1._groups)
                            {
                                if (g1._GroupCode == scp.m_groupcode) valid = true;//is SL
                            }
                        }

                    }
                }
                if (s1 == "all") valid = true;
                visibility(4);
                if (!valid)
                {
                    ListBox_PredictedGrades.Visible = false;
                    Button_Save_Prediction.Visible = false;
                    Label_Title_for_Edit.Text = "Can't edit this grade as you don't teach the student";
                    return;
                }

                Label_Title_for_Edit.Text = "Predicted Grade for " +p1.m_GivenName + " " + p1.m_Surname +" for "+ exo1.m_OptionCode + ": " + exo1.m_OptionTitle;
                Label_EntryID_for_edit.Text = s;
                ListBox_PredictedGrades.Items.Clear();
                if (exo1.m_OptionQualification == "GCE")
                {
                    ListBox_PredictedGrades.Items.Add("A*");
                    ListBox_PredictedGrades.Items.Add("A");
                    ListBox_PredictedGrades.Items.Add("B");
                    ListBox_PredictedGrades.Items.Add("C");
                    ListBox_PredictedGrades.Items.Add("D");
                    ListBox_PredictedGrades.Items.Add("E");
                    ListBox_PredictedGrades.Items.Add("U");
                }
                if (exo1.m_OptionQualification == "GCSE")
                {
                    ListBox_PredictedGrades.Items.Add("A*");
                    ListBox_PredictedGrades.Items.Add("A");
                    ListBox_PredictedGrades.Items.Add("B");
                    ListBox_PredictedGrades.Items.Add("C");
                    ListBox_PredictedGrades.Items.Add("D");
                    ListBox_PredictedGrades.Items.Add("E");
                    ListBox_PredictedGrades.Items.Add("");
                }
                if (ListBox_PredictedGrades.Items.FindByText(Server.HtmlDecode(row1.Cells[7].Text)) != null)
                {
                    ListBox_PredictedGrades.Items.FindByText(Server.HtmlDecode(row1.Cells[7].Text)).Selected = true;
                    Button_Save_Prediction.Visible = true;
                }
                else
                {
                    //ListBox_PredictedGrades.Visible = false;
                    //Button_Save_Prediction.Visible = false;
                    //Label_Title_for_Edit.Text = "Can't edit this grade";
                }
                Button_Save_Prediction.Visible = true;

            }

            if (e.CommandName == "Edit_Button")
            {
                int row = Convert.ToInt32(e.CommandArgument);
                GridViewRow row1 = GridView1.Rows[row];
                string s = Server.HtmlDecode(row1.Cells[0].Text);//is id
                Exam_Entry ex1 = new Exam_Entry(); ex1.Load(s);
                ExamsEntries_sql.UpdateCommand = "UPDATE  dbo.tbl_Exams_Entries  SET PredictedGrade = '?' WHERE ExamEntryID ='"+s+"' ";

            }
            if (e.CommandName == "Delete_Button")
            {
                int row = Convert.ToInt32(e.CommandArgument);
                GridViewRow row1 = GridView1.Rows[row];
                string s = Server.HtmlDecode(row1.Cells[0].Text);
                //do delete...
            }
        }
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void write_excel()
        {
            int max_cols=0;
            int x = 0;
            int max_rows=0;
            ArrayList a1 = new ArrayList();
            foreach (GridViewRow r1 in GridView1.Rows)
            {
                x = 0;
                foreach (TableCell c1 in r1.Cells)
                {
                    Grid_Element g1 = new Grid_Element();
                    a1.Add(g1); g1.m_value = c1.Text; g1.m_y = max_rows; g1.m_x = x; x++;
                }
                max_rows++; max_cols = x;
            }

            ExcelXmlWriter xl = new ExcelXmlWriter();
            xl.OutputToExcel(Response, a1, max_cols, max_rows, "SetLists");
        }

        protected void Button_Excel_Click(object sender, EventArgs e)
        {
            write_excel();
        }

        protected void DropDownList_ExamYear_SelectedIndexChanged(object sender, EventArgs e)
        {

            Label_ExamYear.Text = DropDownList_ExamYear.SelectedValue;
            UpdateList2();
            ExamsEntries_sql.SelectCommand = CreateQueryString();
        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label_OptionCode.Text = DropDownList2.SelectedValue;
            ExamsEntries_sql.SelectCommand = CreateQueryString();
            Encode en = new Encode();
            Label_Title.Text = "Total number of Entries = "+en.ExecuteScalarSQL("SELECT COUNT(*) FROM [qry_Cerval_Exams_Entries] WHERE (OptionCode ='" + Label_OptionCode.Text + "' )  AND (ExamSeason ='" + Label_ExamSeason.Text + "' ) AND (ExamYear ='" + Label_ExamYear.Text + "' )");
            if (Label_type.Text == "mylist")
            {
                Label_Title.Text += " - only showing students taught by " + Label_staffCode.Text;
            }
        }

        protected void DropDownList1_SelectedIndexChanged1(object sender, EventArgs e)
        {
            Label_ExamSeason.Text = DropDownList1.SelectedValue;
            UpdateList2();
            ExamsEntries_sql.SelectCommand = CreateQueryString();
        }

        protected void Button_ShowTimetable_Click(object sender, EventArgs e)
        {
            GridView1.Visible = false;
            visibility(3);
            string studentID = Label_StudentID.Text;
            PupilDetails p1 = new PupilDetails(studentID);
            {
                Utility u1 = new Utility();
                DateTime t1 = new DateTime();
                DateTime t0 = new DateTime();
                t0 = DateTime.Now;
                t1 = t0.AddMonths(6);
                t0 = t0.AddMonths(-2);
                //string season = u1.ThisSeason(t0);
                ArrayList m_list = new ArrayList();

                string s = "PersonImagePage.aspx?id=" + p1.m_PersonId.ToString();
                s = "\"" + s;
                s += "\" width = \"110\" height=\"140\"";
                Response.Write("<FONT FACE = \"Arial\"><p align=\"center\"> <img src=" + s + "></p>");
                {
                    Response.Write("<br><center><h2>Exam Timetable for " + p1.m_GivenName + "  " + p1.m_Surname + "</h2></center>");
                    StudentSENList sen1 = new StudentSENList(p1.m_StudentId.ToString()); s = "";
                    foreach (StudentSEN s1 in sen1.m_List)
                    {
                        if (s1.m_ExamsExtraTime > 0)
                        {
                            s+="Extra Time ("+s1.m_ExamsExtraTime.ToString()+"%)  ";
                        }
                        if (s1.m_ExamsCanType)
                        {
                            s+="  Can Type ";
                        }
                    }
                    if (s != "")
                    {
                        s="<center><h3>"+s+"</h3></center>";
                        Response.Write(s);
                    }
                    ScheduledComponentList scl1 = new ScheduledComponentList();
                    scl1.LoadList_Student(t0, t1, studentID);
                    SimpleRoom room1 = new SimpleRoom();
                    Response.Write("<BR><p  align=\"center\" ><TABLE BORDER><TR>");
                    s = "Date"; Response.Write("<TD>" + s + "</TD>");
                    s = "Day"; Response.Write("<TD>" + s + "</TD>");
                    s = "Start Time"; Response.Write("<TD>" + s + "</TD>");
                    s = "End Time"; Response.Write("<TD>" + s + "</TD>");
                    s = "Paper Code"; Response.Write("<TD>" + s + "</TD>");
                    s = "Paper Name"; Response.Write("<TD>" + s + "</TD>");
                    s = "Room"; Response.Write("<TD>" + s + "</TD>");
                    s = "Desk"; Response.Write("<TD>" + s + "</TD>");
                    Response.Write("</TR>");
                    foreach (ScheduledComponent sc in scl1.m_List)
                    {
                        Response.Write("<TR>");
                        s = sc.m_Date.ToShortDateString(); Response.Write("<TD>" + s + "</TD>");
                        s = sc.m_Date.DayOfWeek.ToString(); Response.Write("<TD>" + s + "</TD>");
                        t1 = sc.m_Date;
                        s = sc.m_Date.ToShortTimeString(); Response.Write("<TD>" + s + "</TD>");
                        t1 = t1.AddMinutes(sc.m_TimeAllowed);
                        s = t1.ToShortTimeString(); Response.Write("<TD>" + s + "</TD>");
                        s = sc.m_ComponentCode; Response.Write("<TD>" + s + "</TD>");
                        s = sc.m_ComponentTitle.ToLower(); Response.Write("<TD>" + s + "</TD>");
                        if (sc.m_RoomId != Guid.Empty)
                        {
                            room1.Load(sc.m_RoomId.ToString());
                            s = room1.m_roomcode; Response.Write("<TD>" + s + "</TD>");
                        }
                        else
                        {
                            Response.Write("<TD>not yet assigned</TD>");
                        }

                        s = sc.m_Desk; Response.Write("<TD>" + s + "</TD>");
                        Response.Write("</TR>");
                    }
                    Response.Write("</TABLE>");
                }
                Response.Write("<br><center><h3>Candidate Number = " + p1.m_examNo.ToString() + " , UCI=" + p1.m_UCI + "</h3></center></Font>");

            }

        }

        protected void ExamsEntries_Modules_sql_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {

        }

        protected void Button_Save_Prediction_Click(object sender, EventArgs e)
        {
            string s = Label_EntryID_for_edit.Text;
            Exam_Entry ex1 = new Exam_Entry();
            ex1.Load(s);
            s = ListBox_PredictedGrades.SelectedItem.Text;
            ex1.m_PredictedGrade = s;
            ex1.Save();
            GridView1.DataBind();
            visibility(GetDisplayType(Label_type.Text));
        }
        private int GetDisplayType(string type)
        {
            if(type == "SubjectEntries") return 1;
            if (type == "Student") return 2;
            if (type == "mylist") return 1 ;
            return 1;
        }

        protected void Button_Edit_Cancel_Click(object sender, EventArgs e)
        {
            visibility(GetDisplayType(Label_type.Text));
        }

        protected void DropDownList2_DataBound(object sender, EventArgs e)
        {
            Label_OptionCode.Text = DropDownList2.SelectedValue;
            ExamsEntries_sql.SelectCommand = CreateQueryString();
            Encode en = new Encode();
            Label_Title.Text = "Total number of Entries = " + en.ExecuteScalarSQL("SELECT COUNT(*) FROM [qry_Cerval_Exams_Entries] WHERE (OptionCode ='" + Label_OptionCode.Text + "' )  AND (ExamSeason ='" + Label_ExamSeason.Text + "' ) AND (ExamYear ='" + Label_ExamYear.Text + "' )");
            if (Label_type.Text == "mylist")
            {
                Label_Title.Text += " - only showing students taught by " + Label_staffCode.Text;
            }
        }

        protected void ListBox_Qualification_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateList2();
        }

        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateList2();
        }

        protected void DropDownList4_SelectedIndexChanged(object sender, EventArgs e)
        {

            UpdateList2();
        }

    }
}
