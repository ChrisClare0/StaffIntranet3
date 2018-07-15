using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2
{
    
    public partial class PupilChoice : System.Web.UI.Page, IPostBackEventHandler
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Utility u = new Utility();
                Display_List.Items.FindByValue("Medical").Enabled = u.CheckStaffInConfigGroup(Context, "Medical");
                Display_List.Items.FindByValue("StudentDevelopment Log").Enabled = u.CheckStaffInConfigGroup(Context, "StudentDevelopment");
                StudentDetail1.Visible = false;
                TimetableControl1.Visible = false;
                e1.Finished += new EventHandler(e1_Finished);
                e1.Submit += new EventHandler(e1_Finished);
                ResultGrid1.Visible = false;
                Label_Year.Text = "";

                if (Request.QueryString.Count >= 1)
                {
                    string type = Request.QueryString["Type"];
                    ViewState.Add("ListType", type);// type = left for past students...
                    if (type == "Student")
                    {
                        string id1 = Request.QueryString["Id"];
                        string name = Request.QueryString["Name"];
                        TextBox_mask.Visible = false;
                        NameList.Visible = true;
                        Label_Year.Text = "Single Student";
                        Display_List.Visible = true;
                        ListItem Item = new ListItem(name, id1);
                        NameList.Items.Add(Item); Item.Selected = true; Display(); return;
                    }
                    if(type=="StudentByAdno")
                    {
                        string adno = Request.QueryString["Adno"];
                        try {
                            int Adno = Convert.ToInt32(adno);
                            SimplePupil p1 = new SimplePupil();p1.Load_Left(Adno);
                            string id1 = p1.m_StudentId.ToString();
                            string name = p1.m_GivenName + " " + p1.m_Surname;
                            TextBox_mask.Visible = false;
                            NameList.Visible = true;
                            Label_Year.Text = "Single Student";
                            Display_List.Visible = true;
                            ListItem Item = new ListItem(name, id1);
                            NameList.Items.Add(Item); Item.Selected = true; Display(); return;
                        }
                        catch { }
                    }
                    if ((Request.QueryString["Year"] == "0")||(Request.QueryString["Year"] == ""))
                    {
                        //do mask...
                        TextBox_mask.Visible = true;
                        NameList.Visible = false;
                        Label_Year.Text = "Type mask and return";
                        Display_List.Visible = false;
                    }
                    else
                    {
                        Build_NameList(Request.QueryString["Year"]);
                        if (type == "Left") TextBox_mask.Visible = true;
                        Label_Year.Text = "Type mask and return";
                        Display();
                    }
                }
                RadioButtonList rbl1 = Display_List;
                try
                {
                    //going to read display keys
                    //StaffIntranet_StudentDetail_Disable
                    Cerval_Configuration c1 = new Cerval_Configuration("StaffIntranet_StudentDetail_Disable");
                    string[] v1 = c1.Value.Split(',');
                    for (int i = 0; i < 12; i++) rbl1.Items[i].Enabled = (v1[i] == "1") ? true : false;
                }
                catch{}
            }
        }
        
        public void e1_Finished(object sender, EventArgs e)
        {
            string s = "dd";
        }

        private void Build_NameList(string year)
        {
            NameList.Items.Clear();
            if ((year == "14") || (year == ""))
            {
                Label_Year.Text = ((year == "14") ? "Past(Leaving year)" : "Current (Entry year)");
                //special case .. need leavers who are not therefore in a year group
                PastStudentList stlist = new PastStudentList(( (year == "") ?"Expr13 = 1" :"Expr13 = 0"));//not on role
                foreach (SimplePupil sp in stlist._studentlist)
                {
                        ListItem Item = new ListItem(sp.m_GivenName + " " + sp.m_Surname + " (" + ((year == "14") ? sp.m_dol.Year.ToString() : sp.m_doa.Year.ToString()) + ")", sp.m_StudentId.ToString());
                        NameList.Items.Add(Item);
                }
            }
            else
            {
                DateTime d = new DateTime(); d = DateTime.Now;
                Label_Year.Text = "Year " + year;
                StudentYearList yl1 = new StudentYearList(NameList, year + "Year", d);
                if (NameList.Items.Count == 0)
                {
                    Label_Year.Text = "Year " + year +" (Next Year)";
                    
                    yl1.StudentYearList_Load(NameList, year + "Year", d);
                }
            }
            if (NameList.Items.Count > 0) NameList.Items[0].Selected = true;
        }

        private void Display()
        {
            if (NameList.Items.Count == 0) return;
            string studentFullName = NameList.SelectedItem.Text;
            string studentID = NameList.SelectedItem.Value;
            StudentDetail1.StudentId = new Guid(NameList.SelectedValue);
            ReportCommentControl1.StudentId = StudentDetail1.StudentId;
            MedicalDetailsControl1.StudentId = StudentDetail1.StudentId;
            StudentDevelopmentControl1.StudentId = StudentDetail1.StudentId;
            TimetableControl1.BaseId = StudentDetail1.StudentId;
            TimetableControl1.type = TT_writer.TimetableType.Student;
            StudentDetail1.Visible = Display_List.Items.FindByValue("Details").Selected;
            StudentDetail1.emailcontrol1.Visible = false;
            TextBox_Registrations.Visible = false;
            TimetableControl1.Visible = Display_List.Items.FindByValue("Timetable").Selected;
            ReportCommentControl1.Visible = Display_List.Items.FindByValue("ReportComments").Selected;
            RadioButtonList_reports.Visible = Display_List.Items.FindByValue("ReportComments").Selected;
            string s1=RadioButtonList_reports.SelectedItem.Value;
            switch (s1)
            {
                case ("1"): ReportCommentControl1.CommentType = ReportCommentControl.ReportCommentType.ImprovementPoints; break;
                case ("2"): ReportCommentControl1.CommentType = ReportCommentControl.ReportCommentType.Comments; break;
            }

            ResultGrid1.Visible = false;
            IncidentConrol1.Visible = false;
            StudentDevelopmentControl1.Visible = false;
 
            

            TextBox_EditMedical.Visible= false;
            e1.Visible = false;
            e2.Visible = false;
            e1.StudentId= StudentDetail1.StudentId;
            e2.StudentId= StudentDetail1.StudentId;
            IncidentConrol1.StudentId = StudentDetail1.StudentId;
            StudentDevelopmentControl1.StudentId = StudentDetail1.StudentId;
            ResultGrid1.StudentId = studentID;
            TextBox_EditMedical.Visible = Display_List.Items.FindByValue("Medical").Selected;
            Button_SaveMedical.Visible  = Display_List.Items.FindByValue("Medical").Selected;
            MedicalDetailsControl1.Visible = Display_List.Items.FindByValue("Medical").Selected;
            Button_CreateNewIncident.Visible = Display_List.Items.FindByValue("Incident Log").Selected;
            Button_EditIncident.Visible = Display_List.Items.FindByValue("Incident Log").Selected;

            Button_EditStudentDevelopment.Visible = Display_List.Items.FindByValue("StudentDevelopment Log").Selected;
            Button_CreateNewStudentDevelopment.Visible = Display_List.Items.FindByValue("StudentDevelopment Log").Selected;


            if(Display_List.Items.FindByValue("Recent Registrations").Selected)
            {
                TextBox_Registrations.Visible = true;
                Utility u = new Utility(); int adno=u.GetAdmissionNumber(studentID);
                RegistrationsList rl1 = new RegistrationsList();rl1.Get_Recent_Registrations(adno.ToString());
                string s = "Date/Time" + (char)(0x09) + (char)(0x09) + "Staff" + (char)(0x09) + "Period" + (char)(0x09) + "Status" + Environment.NewLine;
                foreach (Registrations r in rl1.Registrations)
                {
                    s += r.m_date.ToString() + (char)(0x09) + r.m_staff + (char)(0x09) + r.m_period + (char)(0x09) + r.status + Environment.NewLine;
                }
                TextBox_Registrations.Text = s;TextBox_Registrations.Height = 200;
            }

            if (Display_List.Items.FindByValue("Medical").Selected)
            {
                StudentMedical sm1 = new StudentMedical(); 
                sm1.Load(NameList.SelectedValue);
                TextBox_EditMedical.Text = sm1.m_MedicalNotes;
            }
            if (Display_List.Items.FindByValue("External Results").Selected)
            {
                ResultGrid1.Visible = true;
                ResultGrid1.DisplayType = ResultGrid.GridDisplayType.External;
            }
            if (Display_List.Items.FindByValue("Module Results").Selected)
            {
                ResultGrid1.Visible = true;
                ResultGrid1.DisplayType = ResultGrid.GridDisplayType.Module;
            }
            if (Display_List.Items.FindByValue("Internal Results").Selected)
            {
                ResultGrid1.Visible = true;
                ResultGrid1.DisplayType = ResultGrid.GridDisplayType.Internal;
            }
            if(Display_List.Items.FindByValue("Incident Log").Selected)
            {     
                IncidentConrol1.Visible = true;
                Utility u = new Utility();
                Button_EditIncident.Visible=u.CheckStaffInConfigGroup(Context, "EditIncidents");          
            }


            if (Display_List.Items.FindByValue("StudentDevelopment Log").Selected)
            {
                Utility u = new Utility();
                StudentDevelopmentControl1.Visible = u.CheckStaffInConfigGroup(Context, "EditStudentDevelopment");
                
                Button_EditStudentDevelopment.Visible = u.CheckStaffInConfigGroup(Context, "EditStudentDevelopment");
            }


            ///
            if (Display_List.Items.FindByValue("Exams").Selected)
            {
                string season = "6";
                int year = DateTime.Now.Year;
                if (DateTime.Now.Month > 9) {season = "1";year++; }
                if (DateTime.Now.Month == 1) season = "1";
                string s = "ExamsSubjectList.aspx?ExamYear=" + year.ToString() + "&ExamSeason=" + season + "&Task=Student&StudentID=" + studentID;
                Server.Transfer(s);
            }
            if (Display_List.Items.FindByValue("Academic Profile").Selected)
            {
                string s = "PupilAcademicProfile.aspx?Id=" + StudentDetail1.StudentId.ToString();
                Server.Transfer(s);
            }

        }

        private bool CheckStaffInConfigGroup(HttpContext Context, string p)
        {
            throw new NotImplementedException();
        }

        protected void NameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Display();
        }
        protected void TextBox_mask_TextChanged(object sender, EventArgs e)
        {
            NameList.Items.Clear();

            string type = (string)ViewState["ListType"];

            if (type == "Left")
            {
                PastStudentList stlist = new PastStudentList("Expr13 = 0");//not on role
                foreach (SimplePupil sp in stlist._studentlist)
                {
                    if ((sp.m_Surname.ToUpper().Contains(TextBox_mask.Text.ToUpper()) || (sp.m_GivenName.ToUpper().Contains(TextBox_mask.Text.ToUpper()))) || sp.m_adno.ToString().Contains(TextBox_mask.Text))
                    {
                        ListItem Item = new ListItem(sp.m_GivenName + " " + sp.m_Surname + "(" + sp.m_dol.Year.ToString() + ")", sp.m_StudentId.ToString());
                        NameList.Items.Add(Item);
                    }
                }
            }
            else
            {
                SimpleStudentList stlist = new SimpleStudentList("");
                foreach (SimplePupil sp in stlist)
                {
                    if ((sp.m_Surname.ToUpper().Contains(TextBox_mask.Text.ToUpper()) || (sp.m_GivenName.ToUpper().Contains(TextBox_mask.Text.ToUpper()))) || sp.m_adno.ToString().Contains(TextBox_mask.Text))
                    {
                        ListItem Item = new ListItem(sp.m_GivenName + " " + sp.m_Surname + " (" + sp.m_form + ")", sp.m_StudentId.ToString());
                        NameList.Items.Add(Item);
                    }
                }
            }

            if (NameList.Items.Count > 0)
            {
                NameList.Items[0].Selected = true;
                NameList.Visible = true;
                Display_List.Visible = true;
                Display();
            }
        }
        protected void Display_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            Display();
        }
        protected void Button_SaveMedical_Click(object sender, EventArgs e)
        {
            StudentMedical sm1 = new StudentMedical();
            sm1.SaveMedicalNote(NameList.SelectedValue, TextBox_EditMedical.Text);
        }
        protected void Button_NewIncident_Click(object sender, EventArgs e)
        {
            e1.StudentId = StudentDetail1.StudentId;
            e1.IsNew = true; e1.IsEdit = false;
            // now set up list of controls to re-enable
            SetUpExitList(e1);
        }
        void EditIncidentControl1_Finished(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        protected void Button_EditIncident_Click(object sender, EventArgs e)
        {
            e1.IsNew = false; e1.IsEdit = false;
            e1.StudentId = StudentDetail1.StudentId;
            SetUpExitList(e1);
        }
        protected void SetUpExitList(EditIncidentControl e)
        {
            Utility u = new Utility();
            Button_CreateNewIncident.Visible = false;
            Button_EditIncident.Visible = false;
            IncidentConrol1.Visible = false;
            e.Visible = true;
            e.m_ControlList.Clear();
            e.m_ControlList.Add(Button_CreateNewIncident.ID);
            e.m_ControlList.Add(Button_EditIncident.ID);
            e.m_ControlList.Add(IncidentConrol1.ID);
            //e.ClearControls(u.Get_StaffID(u.GetStaffCodefromContext(Context)));
            e.ClearControls(u.GetsStaffIdfromRequest(Request).ToString());

        }


 //////// Student Development
        protected void Button_NewStudentDevelopment_Click(object sender, EventArgs e)
        {
            e2.StudentId = StudentDetail1.StudentId;
            e2.IsNew = true; e2.IsEdit = false;
            // now set up list of controls to re-enable
            SetUpExitList(e2);
        }
        void EditStudentDevelopmentControl1_Finished(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        protected void Button_EditStudentDevelopment_Click(object sender, EventArgs e)
        {
            e2.IsNew = false; e2.IsEdit = false;
            e2.StudentId = StudentDetail1.StudentId;
            SetUpExitList(e2);
        }
        protected void SetUpExitList(EditStudentDevelopmentControl e2)
        {
            Utility u = new Utility();
            Button_CreateNewStudentDevelopment.Visible = false;
            Button_EditStudentDevelopment.Visible = false;
            StudentDevelopmentControl1.Visible = false;
            e2.Visible = true;
            e2.m_ControlList.Clear();
            e2.m_ControlList.Add(Button_CreateNewStudentDevelopment.ID);
            e2.m_ControlList.Add(Button_EditStudentDevelopment.ID);
            e2.m_ControlList.Add(StudentDevelopmentControl1.ID);
            //e2.ClearControls(u.Get_StaffID(u.GetStaffCodefromContext(Context)));
            e2.ClearControls(u.GetsStaffIdfromRequest(Request).ToString());
        }

        protected void RadioButtonList_reports_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s1 = RadioButtonList_reports.SelectedItem.Value;
            switch (s1)
            {
                case ("1"): ReportCommentControl1.CommentType = ReportCommentControl.ReportCommentType.ImprovementPoints; break;
                case ("2"): ReportCommentControl1.CommentType = ReportCommentControl.ReportCommentType.Comments; break;
            }
        }

        #region PostBackEventHandler
        public virtual void RaisePostBackEvent(string eventArgument)
        {
            string s = eventArgument;
        }
        #endregion

    }
}
