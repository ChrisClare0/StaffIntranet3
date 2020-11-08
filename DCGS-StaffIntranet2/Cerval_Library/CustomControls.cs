using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.IO;

namespace Cerval_Library
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:SimpleC runat=server></{0}:SimpleC>")]
    public class SimpleC : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }



        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write("custom1");
        }

    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:SectionalTimetable runat=server></{0}:SectionalTimetable>")]
    public class SectionalTimetable : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write("SectionalTimetable");
        }

    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ComplexStudentList runat=server></{0}:ComplexStudentList>")]
    public class ComplexStudentList : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        private Label label1 = new Label();
        private DropDownList Param1 = new DropDownList();
        private DropDownList Condition1 = new DropDownList();
        private DropDownList Param2 = new DropDownList();
        private DropDownList Condition3 = new DropDownList();
        private DropDownList Param3 = new DropDownList();
        private DropDownList Condition4 = new DropDownList();
        private DropDownList Param4 = new DropDownList();
        private DropDownList Condition2 = new DropDownList();
        private Button UpdateListButton = new Button();
        private System.Collections.Generic.List<string> parameters = new System.Collections.Generic.List<string>();

        public ComplexStudentList()
        {
            parameters.Add("none");
            parameters.Add("who study");
            parameters.Add("who do not study");
            parameters.Add("In Year Group");
            parameters.Add("Free at period");


            Controls.Add(Param1);
            Controls.Add(Param2);
            Controls.Add(Param3);
            Controls.Add(Param4);
            Controls.Add(Condition1);
            Controls.Add(Condition2);
            Controls.Add(Condition3);
            Controls.Add(Condition4);
            Controls.Add(label1); label1.Text = "Select the criteria and click on Updatelist";


            Param1.AutoPostBack = true;
            foreach (string s in parameters)
            {
                Param1.Items.Add(new ListItem(s));
                Param2.Items.Add(new ListItem(s));
                Param3.Items.Add(new ListItem(s));
                Param4.Items.Add(new ListItem(s));
            }
            Param2.AutoPostBack = true;
            Param3.AutoPostBack = true;
            Param4.AutoPostBack = true;
            Param1.SelectedIndexChanged += new EventHandler(Param1_SelectedIndexChanged);
            Param2.SelectedIndexChanged += new EventHandler(Param2_SelectedIndexChanged);
            Param3.SelectedIndexChanged += new EventHandler(Param3_SelectedIndexChanged);
            Param4.SelectedIndexChanged += new EventHandler(Param4_SelectedIndexChanged);
            Controls.Add(UpdateListButton); UpdateListButton.Text = "UpdateList";
            UpdateListButton.Click += new EventHandler(UpdateListButton_Click);
        }


        void Process_Restriction(string s1, string s2, string s3, ref SimpleStudentList ssl1)
        {
            switch (s1)
            {
                case "In Year Group":
                    int y = System.Convert.ToInt16(s2);
                    ssl1.Restrict_to_year(y);
                    break;
                case "Free at period":
                    int d = System.Convert.ToInt16(s3.Substring(0, 1));
                    ssl1.Restrict_FreeAt(new Period(s3.Substring(2, s3.Length - 2)), d, DateTime.Now);
                    break;

                case "who study":
                    Guid cseId = new Guid(s3);
                    ssl1.Restrict_to_those_studying(cseId, DateTime.Now);
                    break;
                case "who do not study":
                    Guid cseId1 = new Guid(s3);
                    ssl1.Restrict_to_those_NOTstudying(cseId1, DateTime.Now);
                    break;

                default:
                    break;
            }
        }

        void UpdateListButton_Click(object sender, EventArgs e)
        {
            //now update the list....
            SimpleStudentList ssl1 = new SimpleStudentList("");
            if (Param1.SelectedIndex >= 1)
            {
                Process_Restriction(Param1.SelectedItem.Text, Condition1.SelectedItem.Text, Condition1.SelectedItem.Value, ref ssl1);
            }
            if (Param2.SelectedIndex >= 1)
            {
                Process_Restriction(Param2.SelectedItem.Text, Condition2.SelectedItem.Text, Condition2.SelectedItem.Value, ref ssl1);
            }
            if (Param3.SelectedIndex >= 1)
            {
                Process_Restriction(Param3.SelectedItem.Text, Condition3.SelectedItem.Text, Condition3.SelectedItem.Value, ref ssl1);
            }
            if (Param4.SelectedIndex >= 1)
            {
                Process_Restriction(Param4.SelectedItem.Text, Condition4.SelectedItem.Text, Condition4.SelectedItem.Value, ref ssl1);
            }
            ViewState.Add("Studentlist", ssl1);

        }

        void Param1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCondition(Condition1, Param1.SelectedItem.Text);
        }
        void Param4_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCondition(Condition4, Param4.SelectedItem.Text);
        }
        void Param2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCondition(Condition2, Param2.SelectedItem.Text);
        }
        void Param3_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCondition(Condition3, Param3.SelectedItem.Text);
        }
        void UpdateCondition(DropDownList cond, string param)
        {
            cond.Items.Clear();
            if (param == "In Year Group")
            {
                cond.Items.Add(new ListItem("8"));
                cond.Items.Add(new ListItem("9"));
                cond.Items.Add(new ListItem("10"));
                cond.Items.Add(new ListItem("11"));
                cond.Items.Add(new ListItem("12"));
                cond.Items.Add(new ListItem("13"));
            }
            if (param == "Free at period")
            {
                PeriodList ppl1 = new PeriodList();
                DayList dayl1 = new DayList();
                foreach (days d in dayl1.m_DayList)
                {
                    foreach (Period p in ppl1.m_PeriodList)
                    {
                        cond.Items.Add(new ListItem(d.m_dayname + " " + p.m_periodname, d.m_daycode.ToString() + "," + p.m_PeriodId.ToString()));
                    }
                }

            }

            if ((param == "who study") || (param == "who do not study"))
            {
                int ks = 0;
                CourseList cl1 = new CourseList(ks);
                foreach (Course c in cl1._courses)
                {
                    cond.Items.Add(new ListItem(c.CourseName+"(KeyStage: "+c.KeyStage+")", c._CourseID.ToString()));
                }
            }


        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            label1.RenderControl(output); output.Write("<br/>");
            Param1.RenderControl(output);
            Condition1.RenderControl(output); output.Write("  and... <br/>");
            Param2.RenderControl(output);
            Condition2.RenderControl(output); output.Write("  and... <br/>");
            Param3.RenderControl(output);
            Condition3.RenderControl(output); output.Write("  and... <br/>");
            Param4.RenderControl(output);
            Condition4.RenderControl(output); output.Write("<br/><br/>");
            //output.Write("ComplexStudentList");
            UpdateListButton.RenderControl(output); output.Write("<br/>");
            SimpleStudentList ssl1 = (SimpleStudentList)ViewState["Studentlist"];

            if (ssl1 != null)
            {
                output.Write("Students matching these criteria..." + ssl1._studentlist.Count.ToString() + "<br/><br/>");
                output.Write("<table><tr><td>FirstName</td><td>Surname</td><td>Adno</td><td>Form</td></tr>");
                foreach (SimplePupil p in ssl1)
                {
                    output.Write("<td>" + p.m_GivenName + "</td><td> " + p.m_Surname + "</td><td>" + p.m_adno.ToString() + "</td><td>" + p.m_form + "</td></tr>");
                }
            }
            output.Write("</table>");
        }

    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:SetsResults runat=server></{0}:SetsResults>")]
    public class SetsResults : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        public String Group
        {
            get
            {
                String s = (String)ViewState["Group"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Group"] = value;
            }
        }

        public DateTime Date
        {
            get
            {
                DateTime s = (DateTime)ViewState["Date"];
                return ((s == null) ? DateTime.Now : s);
            }

            set
            {
                ViewState["Date"] = value;
            }
        }

        public SetsResults()
        {
            Group = "";
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (Group == "")
            {
                return;
            }
            string[,] grid1 = new string[300, 25];
            grid1[0, 0] = "Surname"; grid1[0, 1] = "GivenName"; grid1[0, 2] = "Adno";
            int n_students = 1; int n_results = 0; int j = 0;
            Group gr1 = new Group();
            gr1.Load(Group, Date);
            //mess about with PH and SC
            if (gr1._GroupCode.Contains("PH")) gr1._CourseID = new Guid("d771e877-f038-442b-9301-0b031c8c6290");
            StudentGroupMembershipList sgml = new StudentGroupMembershipList();
            sgml.LoadList_Group(gr1._GroupID, Date);
            if (sgml.m_list.Count == 0) return;
            ResultsList rl1 = new ResultsList();
            foreach (StudentGroupMembership s in sgml.m_list)
            {
                //load results for this student
                rl1.m_parameters = 3; rl1.m_db_field2 = "dbo.tbl_Core_Results.CourseID"; rl1.m_value2 = gr1._CourseID.ToString();
                rl1.m_db_extraquery = " AND (dbo.tbl_Core_Results.ResultDate > CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ) ";
                rl1.LoadList("dbo.tbl_Core_Results.StudentId", s.m_Studentid.ToString());
                foreach (Result r in rl1._results)
                {
                    grid1[n_students, 0] = r.Surname; grid1[n_students, 1] = r.Givenname; grid1[n_students, 2] = r.Adno.ToString();
                    if (r.External)
                    {
                        string s1 = r.OptionTitle;
                        if (r.Resulttype == 11) s1 = r.OptionCode.ToString();//module
                        j = -1;
                        for (int i = 0; i < n_results; i++)
                        {
                            if (grid1[0, i + 3] == s1) j = i;
                        }
                        if (j == -1) { grid1[0, 3 + n_results] = s1; j = n_results; n_results++; }
                        grid1[n_students, 3 + j] = r.Value;
                    }
                }
                n_students++;
            }
            if (n_results == 0) return;
            //s now oputput...

            output.Write("<p width=\"420\">Results for " + gr1._GroupCode + "<br/><table  border style=\"font-size:smaller;    \">");
            for (int i = 0; i < n_students; i++)
            {
                output.Write("<tr>");
                for (int k = 0; k < 3 + n_results; k++)
                {
                    output.Write("<td>" + grid1[i, k] + "</td>");
                }
                output.Write("</tr>");
            }
            output.Write("</table>");

        }

    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:MedicalDetailsControl runat=server></{0}:MedicalDetailsControl>")]
    public class MedicalDetailsControl : WebControl
    {
        [Category("Data")]
        [DefaultValue("")]
        [Localizable(true)]
        public Guid StudentId
        {
            get
            {
                String s = (String)ViewState["ID"];
                if (s == null)
                    return Guid.Empty;
                else
                {
                    return new Guid(s);
                }
            }
            set
            {
                ViewState["ID"] = value.ToString();
            }
        }
        private string Display()
        {
            StudentMedical s1 = new StudentMedical();
            s1.Load(StudentId.ToString());
            string s = "";
            PupilDetails pupil1 = new PupilDetails(StudentId.ToString());
            string studentFullName = pupil1.m_GivenName + " " + pupil1.m_Surname;
            s = "<p>  <b>" + studentFullName + "<br></b>Adno=<b>" + pupil1.m_adno.ToString() + "<br></b></p>";
            s += "<p> <table  style=\"font-size:smaller;  \">";
            s += "<tr><td>Doctor</td><td>" + s1.m_doctor.m_Title + " " + s1.m_doctor.m_PersonGivenName + " " + s1.m_doctor.m_PersonSurname + "</td></tr>";
            s += "<tr><td>Doctor's Address</td><td>" + s1.m_address + "</td></tr>";
            s += "<tr><td>Medical Notes</td><td>" + s1.m_MedicalNotes + "</td></tr>";
            s += " </table> </p>";
            return s;
        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (StudentId != Guid.Empty)
            {
                output.Write(Display());
            }
            else
            {
                output.Write("Medical Details");
            }
        }

    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:MedicalEditControl runat=server></{0}:MedicalEditControl>")]
    public class MedicalEditControl : WebControl
    {
        [Category("Data")]
        [DefaultValue("")]
        [Localizable(true)]
        public Guid StudentId
        {
            get
            {
                String s = (String)ViewState["ID"];
                if (s == null)
                    return Guid.Empty;
                else
                {
                    return new Guid(s);
                }
            }
            set
            {
                ViewState["ID"] = value.ToString();
            }
        }
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string CallBackPage
        {//url of page to return to
            get
            {
                String s = (String)ViewState["CallBackPage"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["CallBackPage"] = value;
            }
        }

        public MedicalEditControl()
        {
            AddControls();
        }

        TextBox MedicalText = new TextBox();
        Label MedicalName = new Label();

        private void AddControls()
        {
            this.Controls.Add(MedicalText);
            MedicalName.Font.Bold = true;
            this.Controls.Add(MedicalName);
            MedicalText.TextMode = TextBoxMode.MultiLine;
            MedicalText.Width = 600;
            MedicalText.Height = 300;
            MedicalName.Font.Size = FontUnit.Large;
        }


        public void UpdateEdit()
        {
            StudentMedical s1 = new StudentMedical();
            try
            {
                s1.Load(StudentId.ToString());
                s1.SaveMedicalNote(StudentId.ToString(), MedicalText.Text);
            }
            catch
            {

            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (StudentId != Guid.Empty)
            {
                StudentMedical s1 = new StudentMedical();
                s1.Load(StudentId.ToString());
                PupilDetails pupil1 = new PupilDetails(StudentId.ToString());
                string studentFullName = pupil1.m_GivenName + " " + pupil1.m_Surname;
                MedicalName.Text = "Edit Medical detail for " + studentFullName;
                MedicalText.Text = s1.m_MedicalNotes;

                MedicalName.RenderControl(output);
                MedicalText.RenderControl(output);

            }
            else
            {
                output.Write("Medical Details");
            }
        }

    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:EmailStaffControl runat=server></{0}:EmailStaffControl>")]
    public class EmailStaffControl : WebControl
    {
        public System.Web.UI.WebControls.TextBox TextBox1 = new TextBox();
        private System.Web.UI.WebControls.Label label1 = new Label();
        private System.Web.UI.WebControls.Button Button_Send = new Button();
        private System.Web.UI.WebControls.Button Button_Cancel = new Button();
        private System.Web.UI.WebControls.Button Button_Close = new Button();
        public System.Web.UI.WebControls.TextBox TextBox2 = new TextBox();
        public EmailStaffControl()
        {
            this.Controls.Add(TextBox1);
            TextBox1.TextMode = TextBoxMode.MultiLine;
            TextBox1.Width = 500;
            TextBox1.Height = 100;
            Controls.Add(label1);
            label1.Text = "Enter the text of your email message here: ";
            Controls.Add(Button_Send);
            Button_Send.Click += new EventHandler(Button_Send_Click);
            Button_Send.Text = "Send";
            Controls.Add(Button_Cancel); Button_Cancel.Text = "Cancel";
            Button_Cancel.Click += new EventHandler(Button_Cancel_Click);
            Controls.Add(Button_Close); Button_Close.Text = "Close";
            Button_Close.Click += new EventHandler(Button_Close_Click);
            TextBox2.TextMode = TextBoxMode.MultiLine;
            TextBox2.Width = 500;
            TextBox2.Height = 200;
            TextBox2.ReadOnly = true;
            this.Controls.Add(TextBox2);

        }

        void Button_Close_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        void Button_Cancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            TextBox1.Text = "";
        }
        public Guid StudentId
        {
            get
            {
                String s = (String)ViewState["ID"];
                if (s == null)
                    return Guid.Empty;
                else
                {
                    return new Guid(s);
                }
            }
            set
            {
                ViewState["ID"] = value.ToString();
            }
        }

        void Button_Send_Click(object sender, EventArgs e)
        {
            string s2 = ""; string s3 = ""; string s = "";
            Utility u = new Utility();
            //string s1 = u.GetStaffCodefromContext(Context);
            string s1 = u.GetsStaffCodefromRequest(Context.Request);
            string serror = "";

            if (s1 == "")
            {
#if DEBUG
                s1 = "CC";//testing...
#else
                //Server.Transfer("StartForm1.aspx");
#endif
            }
            string staff_Code = s1;
            //s1 = "CC";//testing...
            string from_address = u.Get_StaffEmailAddress(s1);
            PupilPeriodList ppl1 = new PupilPeriodList();
            ppl1.LoadList("StudentId", StudentId.ToString(), true, DateTime.Now.AddDays(-1));
            foreach (ScheduledPeriod p in ppl1.m_pupilTTlist)
            {
                //need to remove duplicates....
                s = p.m_staffcode;
                if (!s1.Contains(s + "-"))
                {
                    s1 += s + "-";
                    s3 = u.Get_StaffEmailAddress(s);

                    System.Configuration.RegexStringValidator rg1 = new System.Configuration.RegexStringValidator(u.GetRegex_email());
                    try
                    {
                        rg1.Validate(s3);
                        if (s3 != "") s2 += s3 + ";";
                    }
                    catch
                    {
                        serror += "Email Address for " + s + " not found, please ticket Cerval Data to update Person_contacts." + Environment.NewLine;
                        //what to do with rubbish emails,..
                    }
                }
            }
            if (s2 != "") s2 = s2.Substring(0, s2.Length - 1);//strip trailing ;
            //now add KS leaders?????
            Cerval_Configurations configs = new Cerval_Configurations(); configs.Load_All();
            Cerval_Configuration c2 = new Cerval_Configuration();
            //System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
            SimplePupil p1 = new SimplePupil(); p1.Load(StudentId.ToString());
            if ((p1.m_form.StartsWith("7")) || (p1.m_form.StartsWith("8")) || (p1.m_form.StartsWith("9")))
            {
                c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SDOEmail");
                if (c2 != null) s2 += ";" + c2.Value;

                c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_KS3LeaderEmail");
                if (c2 != null) s2 += ";" + c2.Value;

            }
            if ((p1.m_form.StartsWith("7")))
            {
                c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_Year7LeaderEmail");
                if (c2 != null) s2 += ";" + c2.Value;
            }

            if ((p1.m_form.StartsWith("10")) || (p1.m_form.StartsWith("11")))
            {
                c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SDOEmail");
                if (c2 != null) s2 += ";" + c2.Value;

                c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_KS4LeaderEmail");
                if (c2 != null) s2 += ";" + c2.Value;
            }
            if ((p1.m_form.StartsWith("12")) || (p1.m_form.StartsWith("13")))
            {
                c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_KS5LeaderEmail");
                if (c2 != null) s2 += ";" + c2.Value;


            }
            if (p1.m_form.StartsWith("12"))
            {
                c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_Year12LeaderEmail");
                if (c2 != null) s2 += ";" + c2.Value;

            }
            if (p1.m_form.StartsWith("13"))
            {
                c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_Year13LeaderEmail");
                if (c2 != null) s2 += ";" + c2.Value;
            }
            string s7 = "";
            try
            {
                s7 = u.Get_StaffEmailAddress(u.GetTutorForStudent(StudentId, DateTime.Now));
                s2 += ";" + s7;
            }
            catch
            {

            }
            //need to remove duplicates in s2 here...

            string[] s4 = new string[100];
            char[] c4 = new char[1]; c4[0] = ';';
            s4 = s2.Split(c4);s2 = "";
            //now re-build with no dublicates
            foreach(string s5 in s4)
            {
                if (!s2.Contains(s5.Trim())) s2 += s5.Trim() + ";";
            }
            //remove trailing ;
            s2 = s2.Substring(0, s2.Length - 1);


            MailHelper m1 = new MailHelper();
            string monitor_email="";
            c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_Monitor_email");
            if (c2 != null) monitor_email = c2.Value;

            m1.SendMail(from_address, s2, "", TextBox1.Text, monitor_email, "From: " + staff_Code + " Re: " + p1.m_GivenName + " " + p1.m_Surname + "(" + p1.m_form.Trim() + ") -sent on behalf of: " + from_address);
            TextBox2.Text = serror + Environment.NewLine + "Email sent to:" + Environment.NewLine + s2;
            if (!m1.sendcomplete)
            {
                TextBox2.Text = "Error sending email" + m1.ErrorMessage;
            }
            //this.Visible = false;

        }

        private string Display()
        {
            string s = "";
            PupilDetails pupil1 = new PupilDetails(StudentId.ToString());
            string studentFullName = pupil1.m_GivenName + " " + pupil1.m_Surname;
            s = "<p>Email Teaching Staff for <b>" + studentFullName + "</b></p><br>";
            return s;
        }

        protected override void RenderContents(HtmlTextWriter output)
        {

            if (StudentId != Guid.Empty)
            {
                output.Write(Display());
                label1.RenderControl(output);
                TextBox1.RenderControl(output);
                output.Write("<br><br>");
                Button_Send.RenderControl(output);
                Button_Cancel.RenderControl(output);
                TextBox2.RenderControl(output);
                Button_Close.RenderControl(output);
            }
            else
            {
                output.Write("EmailStaffControl");
            }


        }

    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:StudentDetailControl runat=server></{0}:StudentDetailControl>")]
    public class StudentDetailControl : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [Localizable(true)]
        public Guid StudentId
        {
            get
            {
                String s = (String)ViewState["ID"];
                return ((s == null) ? Guid.Empty : new Guid(s));
            }
            set
            {
                ViewState["ID"] = value.ToString();
                emailcontrol1.StudentId = value;
            }
        }
        private Button Email_Button = new Button();

        public EmailStaffControl emailcontrol1 = new EmailStaffControl();
        private Button NewSEN_Button = new Button();
        private Label temp = new Label();
        private SENEditControl SENEditControl2 = new SENEditControl();
        public StudentDetailControl()
        {
            Utility u = new Utility();
            Controls.Add(Email_Button);
            Email_Button.Text = "Email student's staff";
            Email_Button.Click += new EventHandler(Email_Button_Click);

            Controls.Add(emailcontrol1);
            emailcontrol1.Visible = false;
            Controls.Add(NewSEN_Button);
            Controls.Add(SENEditControl2);
            NewSEN_Button.Text = "Add SEN";
            Controls.Add(temp);
            //temp.Text = u.GetStaffCodefromContext(Context);
            temp.Text = u.GetsStaffCodefromRequest(Context.Request);
            NewSEN_Button.Visible = u.CheckStaffInConfigGroup(Context, "SEN-MANAGERS");
            SENEditControl2.Visible = false;
            NewSEN_Button.Click += new EventHandler(NewSEN_Button_Click);
        }



        void NewSEN_Button_Click(object sender, EventArgs e)
        {
            SENEditControl2.Visible = true;
            SENEditControl2.StudentId = StudentId;
        }

        void Email_Button_Click(object sender, EventArgs e)
        {
            emailcontrol1.StudentId = StudentId;
            emailcontrol1.Visible = true;
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            string s = "";
            if (StudentId == Guid.Empty)
            {
                output.Write("Student Details"); return;
            }
            if (emailcontrol1.Visible)
            {
                emailcontrol1.RenderControl(output); return;
            }
            if (SENEditControl2.Visible)
            {
                SENEditControl2.RenderControl(output); return;
            }

            {
                output.Write(Display(ref s));
                Email_Button.RenderControl(output);
                NewSEN_Button.RenderControl(output);
                //temp.RenderControl(output);
                output.Write(s);
            }
        }

        private string Display(ref string s)
        {
            if (StudentId == Guid.Empty) return "";
            string s1 = "";
            SimpleStudentList Siblings = new SimpleStudentList();
            Utility u = new Utility();
            SimplePupil p = new SimplePupil(); p.Load(StudentId.ToString());
            PupilDetails pupil1 = new PupilDetails(StudentId.ToString());
            string studentFullName = pupil1.m_GivenName + " ";
            if (pupil1.m_InformalName != "") studentFullName += "(" + pupil1.m_InformalName + ") ";
            studentFullName += pupil1.m_Surname;
            s = "<p  style=\"float:left ;  width:200px; margin-right:10px; height:500px; \"><b>" + studentFullName + "<br></b>Adno=<b>" + pupil1.m_adno.ToString() + "<br></b>";
            s += "Form: " + p.m_form;
            s1 = u.GetTutorForStudent(StudentId, DateTime.Now); if (s1 != "") s += "[" + s1 + "]";
            s1 = "../content/PersonImagePage.aspx?id=" + pupil1.m_PersonId.ToString();
            s1 = "\"" + s1;
            s1 += "\" width = \"110\" height=\"140\"  ";
            s += " <img src=" + s1 + "><br>";
            s += "<b>Student Address:</b> <br>" + pupil1.m_address + "<br> <br>";
            s1 = s; s = "";

            StudentSENList senlist1 = new StudentSENList(StudentId.ToString());
            SENTypeList sentypes1 = new SENTypeList();
            StudentMedical studMed = new StudentMedical(); studMed.Load(StudentId.ToString());
            s += "<p width=\"420\"><table  style=\"font-size:smaller;  \">";
            s += "<tr><td>Admission No.</td><td>" + pupil1.m_adno.ToString() + "</td></tr>";
            s += "<tr><td>UCI</td><td>" + pupil1.m_UCI + "</td></tr>";
            s += "<tr><td>UPN</td><td>" + pupil1.m_upn + "</td></tr>";
            s += "<tr><td>ULN</td><td>" + pupil1.m_uln + "</td></tr>";
            s += "<tr><td>DoB</td><td>" + pupil1.m_dob.ToShortDateString() + "</td></tr>";
            s += "<tr><td>Exam number</td><td>" + pupil1.m_examNo.ToString() + "</td></tr>";
            if (pupil1.m_BTECNumber.Length > 2) { s += "<tr><td>BTEC number</td><td>" + pupil1.m_BTECNumber + "</td></tr>"; }
            s += "<tr><td>Ethnicity </td><td>" + pupil1.m_ethnicity + "</td></tr>";
            s += "<tr><td>Language</td><td>" + pupil1.m_language + "</td></tr>";
            s += "<tr><td>Admission date</td><td>" + pupil1.m_doa.ToShortDateString() + "</td></tr>";
            s += "<tr><td>GoogleAppsLogin:</td><td>" + p.m_GoogleAppsLogin + "</td></tr>";
            if (pupil1.m_dol.Year > 1000)
            {
                s += "<tr><td>Leaving date</td><td>" + pupil1.m_dol.ToShortDateString() + "</td></tr>";
                StudentLeavingDetails stl1 = new StudentLeavingDetails(pupil1.m_adno);
                if (stl1.LeavingDestinationId != Guid.Empty)
                {
                    Organisation o = new Organisation(); o.Load(stl1.LeavingDestinationId);
                    s += "<tr><td>Leaving Destination</td><td>" + o.m_OrganisationName + "</td></tr>";
                }
                s += "<tr><td>Leaving Details</td><td>" + stl1.LeavingDetails + "</td></tr>";

            }


            foreach (Contact c in pupil1.m_contacts.m_contactlist)
            {
                s += "<tr><td>" + c.m_ContactType + "</td><TD>" + c.m_Contact_Value + "</TD></tr>";
            }
            s += "<tr><td>StudentId</td><td>" + pupil1.m_StudentId.ToString() + "</td></tr>";
            s += "</table><hr />";
            s += "<p  style=\"  font-size:small ;\">";


            if (studMed.m_MedicalNotes != null)
            {
                s += "<b>Medical: </b>" + studMed.m_MedicalNotes + "<br><hr />";
            }

            bool sen = false;
            foreach (StudentSEN sen1 in senlist1.m_List)
            {
                foreach (SENType sent1 in sentypes1._List)
                {
                    if (sen1.m_SenType == sent1.id) s += sent1.SENtype + " :   ";
                    sen = true;
                }
                
                if (sen)
                {
                    if (sen1.m_SenDescription.Contains(@"https://docs.google.com"))
                    {
                        int i1 = sen1.m_SenDescription.IndexOf(@"https:");
                        s += sen1.m_SenDescription.Substring(0, i1 - 1);
                        s += " <A HREF=\"" + sen1.m_SenDescription.Substring(i1) + "\">  Details </A> .......";
                    }
                    else
                    {
                        s += sen1.m_SenDescription + "  ...";
                        s += "<A HREF=\"";
                        s += "../content/StudentSENStrategies.aspx";
                        s += "?Type=Display&Id=" + StudentId.ToString() + "&Name=" + studentFullName;
                        s += "&Photo=PersonImagePage.aspx?id=" + pupil1.m_PersonId.ToString();
                        s += "&StudentSENId=" + sen1.m_SENid.ToString();
                        s += "\">Details</A>                ";
                    }
                    s += "<A HREF=\"../content/SENEdit.aspx?SENID=" + sen1.m_SENid.ToString() + "&StudentID=" + StudentId.ToString() + "\">Edit</A>";
                }
                s += "<br>";


                if (sen1.m_ExamsCanType) s += "Can type in exams : ";
                if (sen1.m_ExamsExtraTime > 0) s += "Can use " + sen1.m_ExamsExtraTime.ToString() + "% extra time";

                s += "<br><hr />";
            }
            //add PP eligability
            StudentPPList pplist1 = new StudentPPList(); pplist1.LoadList();
            foreach (SimplePupil p1 in pplist1.m_list)
            {
                if (p1.m_StudentId == pupil1.m_StudentId)
                {
                    s += "In Receipt of Pupil Premium. <br> <hr /> ";
                }
            }
            s += "</p></p>";

            foreach (Relationship r in pupil1.m_relationships.m_Relationshiplist)
            {
                s += "<p  style=\"font-size:small ;\">";
                s += r.m_RelationshipDesc + ": " + r.m_Title + " " + r.m_PersonGivenName + " " + r.m_PersonSurname + "<br> " + r.m_Address + "<br>";
                foreach (Contact c in r.m_contactlist.m_contactlist)
                {
                    s += c.m_ContactType + ": " + c.m_Contact_Value + "<br>";
                }
                s += "<hr /></p>";
            }
            Siblings.LoadSiblings(StudentId);
            foreach (SimplePupil ps in Siblings)
            {
                s += "Sibling: " + "<A HREF=\"";
                s += "../content/PupilChoice.aspx?Type=Student&Id=";
                s += ps.m_StudentId.ToString() + "&Name=" + ps.m_GivenName + " " + ps.m_Surname;
                s += "\">" + ps.m_GivenName + " " + ps.m_Surname + "</A>";
            }
            return s1;
        }
    }

    [ToolboxData("<{0}:TimetableControl runat=server></{0}:TimetableControl>")]
    public class TimetableControl : WebControl
    {
        private PupilPeriodList m_ppl;
        private TextBox datebox = new TextBox();
        private Calendar calendar1 = new Calendar();
        private Button button = new Button();
        private Button button2 = new Button();

        public TimetableControl()
        {
            ViewState.Add("Type", TT_writer.TimetableType.None);
            ViewState.Add("BaseId", Guid.Empty.ToString());
            ViewState.Add("Text", "");
            ViewState.Add("Time", DateTime.Now);
            this.Controls.Add(datebox);
            this.Controls.Add(button);
            this.Controls.Add(button2);
            this.Controls.Add(calendar1);
            calendar1.Visible = false;
            //calendar control used at present only for booking...
            if (type == TT_writer.TimetableType.Booking) { calendar1.Visible = true; datebox.Visible = false; button.Visible = false; button2.Visible = false; }
            button.Text = "ReCalculate";
            button2.Text = "Export";
            button.Click += new EventHandler(button_Click);
            button2.Click += new EventHandler(button2_Click);
            datebox.TextChanged += new EventHandler(datebox_TextChanged);
            datebox.Text = DateTime.Now.ToShortDateString();
            calendar1.SelectionChanged += new EventHandler(Calendar_SelectionChanged);
        }

        private void Reset_To_Monday()
        {
            Time = calendar1.SelectedDate;
            if (type == TT_writer.TimetableType.Booking)
            {
                //going to set back/forward to Monday
                if (Time.DayOfWeek == DayOfWeek.Saturday) Time = Time.AddDays(1);
                if (Time.DayOfWeek == DayOfWeek.Sunday) Time = Time.AddDays(2);
                while (Time.DayOfWeek != DayOfWeek.Monday) Time = Time.AddDays(-1);
                calendar1.SelectedDate = Time;
            }

        }

        void Calendar_SelectionChanged(object sender, EventArgs e)
        {
            Reset_To_Monday();
        }

        void datebox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Time = System.Convert.ToDateTime(datebox.Text);
            }
            catch
            {
            }
        }

        void button_Click(object sender, EventArgs e)
        {
            try { Time = System.Convert.ToDateTime(datebox.Text); }
            catch { }
        }

        void button2_Click(object sender, EventArgs e)
                {

            ViewState.Add("Type", TT_writer.TimetableType.Staff);
            SimpleStaff ss1 = new SimpleStaff(BaseId);
            // m_ppl.LoadList("dbo.tbl_Core_Staff.StaffId", BaseId.ToString(), false, Time);
 


            string staff = "SELECT [ID], [StaffCode], [DATE], [START_TIME], [SUBJECT], [LOCATION], [END_TIME] FROM dbo.Intranet_TT_VBR_DONE  WHERE (StaffCode ='" + ss1.m_StaffCode.ToString() + "' );";
            //string staff = "SELECT [ID], [StaffCode], [DATE], [START_TIME], [SUBJECT], [LOCATION], [END_TIME], [Date_Time] FROM dbo.Intranet_TT_DONE_Test  WHERE (StaffCode ='" + ss1.m_StaffCode.ToString() + "' );";
            


            StringBuilder sb = new StringBuilder();
            //need to conect to database to select each class/periods
            Encode en = new Encode();
            System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(en.GetDbConnection()); // NEED TO GET CN
            cn.Open();
            System.Data.SqlClient.SqlCommand sc = new System.Data.SqlClient.SqlCommand(staff, cn);

            System.Data.SqlClient.SqlDataReader dr = sc.ExecuteReader();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:-//Google Inc//Google Calendar 70.9054//EN");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine("X-WR-CALNAME:DCGS");
            sb.AppendLine("X-WR-TIMEZONE:Europe/London");
            sb.AppendLine("BEGIN:VTIMEZONE");
            sb.AppendLine("TZID:Europe/London");
            sb.AppendLine("X-LIC-LOCATION:Europe/London");
            sb.AppendLine("BEGIN:DAYLIGHT");
            sb.AppendLine("TZOFFSETFROM:+0000");
            sb.AppendLine("TZOFFSETTO:+0100");
            sb.AppendLine("TZNAME:BST");
            sb.AppendLine("DTSTART:19700329T010000");
            sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=-1SU");
            sb.AppendLine("END:DAYLIGHT");
            sb.AppendLine("BEGIN:STANDARD");
            sb.AppendLine("TZOFFSETFROM:+0100");
            sb.AppendLine("TZOFFSETTO:+0000");
            sb.AppendLine("TZNAME:GMT");
            sb.AppendLine("DTSTART:19701025T020000");
            sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=10;BYDAY=-1SU");
            sb.AppendLine("END:STANDARD");
            sb.AppendLine("END:VTIMEZONE");
            // needed to set time zone and day light saving!


            while (dr.Read())
            {
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine(String.Format("DTSTART;TZID=Europe/London:" + dr.GetString(2) + dr.GetString(3))); 
                sb.AppendLine(string.Format("DTEND;TZID=Europe/London:" + dr.GetString(2) + dr.GetString(6)));
                //sb.AppendLine("EXDATE;TZID=Europe/London:" + dr.GetString(7));
                // dates below are for the academi year 2014-2015 - up to may 2015 - when the timetable changes
                sb.AppendLine("EXDATE;TZID=Europe/London:20141027T085000,20141027T115000,20141027T090500,20141027T101000,20141027T113000,20141027T133500,20141027T134000,20141027T144500,20141028T085000,20141028T090500,20141028T115000,20141028T101000,20141028T113000,20141028T133500,20141028T134000,20141028T144500,20141029T085000,20141029T115000,20141029T090500,20141029T101000,20141029T113000,20141029T133500,20141029T134000,20141029T144500,20141030T085000,20141030T115000,20141030T090500,20141030T101000,20141030T113000,20141030T133500,20141030T134000,20141030T144500,20141030T085000,20141030T115000,20141030T090500,20141030T101000,20141030T113000,20141030T133500,20141030T134000,20141030T144500,20141031T085000,20141031T115000,20141031T090500,20141031T101000,20141031T113000,20141031T133500,20141031T134000,20141031T144500");
                sb.AppendLine("EXDATE;TZID=Europe/London:20141219T085000,20141219T115000,20141219T090500,20141219T101000,20141219T113000,20141219T133500,20141219T134000,20141219T144500,20141222T085000,20141222T115000,20141222T090500,20141222T101000,20141222T113000,20141222T133500,20141222T134000,20141222T144500,20141223T085000,20141223T115000,20141223T090500,20141223T101000,20141223T113000,20141223T133500,20141223T134000,20141223T144500,20141224T085000,20141224T115000,20141224T090500,20141224T101000,20141224T113000,20141224T133500,20141224T134000,20141224T144500,20141225T085000,20141225T115000,20141225T090500,20141225T101000,20141225T113000,20141225T133500,20141225T134000,20141225T144500,20141226T085000,20141226T115000,20141226T090500,20141226T101000,20141226T113000,20141226T133500,20141226T134000,20141226T144500,20141229T085000,20141229T115000,20141229T090500,20141229T101000,20141229T113000,20141229T133500,20141229T134000,20141229T144500,20141230T085000,20141230T115000,20141230T090500,20141230T101000,20141230T113000,20141230T133500,20141230T134000,20141230T144500,20141231T085000,20141231T115000,20141231T090500,20141231T101000,20141231T113000,20141231T133500,20141231T134000,20141231T144500,20150101T085000,20150101T115000,20150101T090500,20150101T101000,20150101T113000,20150101T133500,20150101T134000,20150101T144500");
                sb.AppendLine("EXDATE;TZID=Europe/London:20150216T085000,20150216T115000,20150216T090500,20150216T101000,20150216T113000,20150216T133500,20150216T134000,20150216T144500,20150217T085000,20150217T115000,20150217T090500,20150217T101000,20150217T113000,20150217T133500,20150217T134000,20150217T144500,20150218T085000,20150218T115000,20150218T090500,20150218T101000,20150218T113000,20150218T133500,20150218T134000,20150218T144500,20150219T085000,20150219T115000,20150219T090500,20150219T101000,20150219T113000,20150219T133500,20150219T134000,20150219T144500,20150220T085000,20150220T115000,20150220T090500,20150220T101000,20150220T113000,20150220T133500,20150220T134000,20150220T144500");
                sb.AppendLine("EXDATE;TZID=Europe/London:20150330T084500,20150330T085000,20150330T090500,20150330T101000,20150330T113000,20150330T133500,20150330T134000,20150330T144500,20150331T084500,20150331T085000,20150331T090500,20150331T101000,20150331T113000,20150331T133500,20150331T134000,20150331T144500,20150401T084500,20150401T085000,20150401T090500,20150401T101000,20150401T113000,20150401T133500,20150401T134000,20150401T144500,20150402T084500,20150402T085000,20150402T090500,20150402T101000,20150402T113000,20150402T133500,20150402T134000,20150402T144500,20150403T084500,20150403T085000,20150403T090500,20150403T101000,20150403T113000,20150403T133500,20150403T134000,20150403T144500,20150406T084500,20150406T085000,20150406T090500,20150406T101000,20150406T113000,20150406T133500,20150406T134000,20150406T144500,20150407T084500,20150407T085000,20150407T090500,20150407T101000,20150407T113000,20150407T133500,20150407T134000,20150407T144500,20150408T084500,20150408T085000,20150408T090500,20150408T101000,20150408T113000,20150408T133500,20150408T134000,20150408T144500,20150410T084500,20150410T085000,20150410T090500,20150410T113000,20150410T133500,20150410T134000,20150410T144500,20150401T101000");
                
                //when timetable changes the four rows above need to be removed and the line below can  be un- commented!
                //sb.AppendLine("EXDATE;TZID=Europe/London:20150525T084500,20150525T085000,20150525T090500,20150525T101000,20150525T113000,20150525T133500,20150525T134000,20150525T144500,20150526T084500,20150526T085000,20150526T090500,20150526T101000,20150526T113000,20150526T133500,20150526T134000,20150526T144500,20150527T084500,20150527T085000,20150527T090500,20150527T101000,20150527T113000,20150527T133500,20150527T134000,20150527T144500,20150528T084500,20150528T085000,20150528T090500,20150528T101000,20150528T113000,20150528T133500,20150528T134000,20150528T144500,20150529T084500,20150529T085000,20150529T090500,20150529T101000,20150529T113000,20150529T133500,20150529T134000,20150529T144500");
                string date_string = "";
                DateTime t1 = new DateTime(); t1 = DateTime.Now;
                if (t1.Month < 8) date_string = t1.Year.ToString(); else date_string = t1.AddYears(1).Year.ToString();
                date_string += "0731T080000Z";
                sb.AppendLine("RRULE:FREQ=WEEKLY;UNTIL=" + date_string);
                sb.AppendLine(string.Format("UID:" + dr.GetSqlGuid(0)));
                date_string = DateTime.Now.Year.ToString(); date_string += DateTime.Now.Month.ToString();
                date_string += DateTime.Now.Date.ToString(); date_string += "T100400";
                sb.AppendLine(string.Format("CREATED:" + date_string)); // dated for today 
                sb.AppendLine("DESCRIPTION:" + dr.GetString(4) + dr.GetString(5));
                sb.AppendLine("LAST-MODIFIED:" + date_string);
                sb.AppendLine("RRULE:FREQ=WEEKLY;UNTIL=20150522T080000Z");
                sb.AppendLine(string.Format("UID:" + dr.GetSqlGuid(0)));
                sb.AppendLine(string.Format("CREATED:20140707T100400")); // dated for today 
                sb.AppendLine("DESCRIPTION:" + dr.GetString(4) + dr.GetString(5));
                sb.AppendLine("LAST-MODIFIED:20140707T100400");
                sb.AppendLine("LOCATION:" + dr.GetString(5));
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + dr.GetString(4) + ' ' + dr.GetString(5));
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("CATEGORIES:http://schemas.google.com/g/2005#event");
                sb.AppendLine("END:VEVENT");

                //dr.GetString(0);
            }
                sb.AppendLine("END:VCALENDAR");

            dr.Close();


            //new event


            UTF8Encoding enc = new UTF8Encoding();
            byte[] arrBytData = enc.GetBytes(sb.ToString());



            Page.Response.Clear();
            Page.Response.ContentType = "text/plain";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=vCalendar.ics");
            Page.Response.AppendHeader("Content-Length", arrBytData.Length.ToString());
            Page.Response.ContentType = "application/octet-stream";
            Page.Response.BinaryWrite(arrBytData);
            Page.Response.Flush();
            Page.Response.End();


        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public TT_writer.TimetableType type
        {
            //the type of the object (student, staff, room etc) we are displaying)
            get
            {
                TT_writer.TimetableType s = (TT_writer.TimetableType)ViewState["Type"];
                return s;
            }

            set
            {
                ViewState["Type"] = value;
                if (value == TT_writer.TimetableType.Booking) { calendar1.Visible = true; datebox.Visible = false; button.Visible = false; button2.Visible = false; calendar1.SelectedDate = DateTime.Now; Reset_To_Monday(); }
            }
        }
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get { return ((ViewState["Text"] == null) ? string.Empty : (string)ViewState["Text"]); }
            set { ViewState["Text"] = value; }
        }

        public DateTime Time
        {
            get
            {
                DateTime d = (DateTime)ViewState["Time"];
                return ((d == null) ? DateTime.Now : d);
            }

            set
            {
                ViewState["Time"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public Guid BaseId
        {
            get
            {
                String s = (String)ViewState["BaseId"];
                if (s == null)
                    return Guid.Empty;
                else
                {
                    return new Guid(s);
                }
            }
            set
            {
                ViewState["BaseId"] = value.ToString();
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            m_ppl = new PupilPeriodList();
            TT_writer ttw1 = new TT_writer(); string s = "";

            if (BaseId != Guid.Empty)
            {
                switch (type)
                {
                    case TT_writer.TimetableType.None:
                        break;
                    case TT_writer.TimetableType.Student:
                        {
                            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(BaseId.ToString());
                            m_ppl.LoadList("StudentId", BaseId.ToString(), true, Time);
                            s = "<p >Current Timetable for <b>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</b></p>";
                            s += ttw1.OutputTT_string("style = \"font-size:small \";  class=\"TimetableTable\"; ", true, type, ref m_ppl, Time);
                        }
                        break;
                    case TT_writer.TimetableType.Staff:
                        {
                            SimpleStaff ss1 = new SimpleStaff(BaseId);
                            m_ppl.LoadList("dbo.tbl_Core_Staff.StaffId", BaseId.ToString(), false, Time);
                            s = "<p >Current Timetable for <b>" + ss1.m_PersonGivenName + " " + ss1.m_PersonSurname + "</b></p>";
                            s += ttw1.OutputTT_string("style = \"font-size:small ;\"", true, type, ref m_ppl, Time);
                        }
                        break;
                    case TT_writer.TimetableType.Room:
                        {
                            SimpleRoom sr1 = new SimpleRoom(BaseId);
                            m_ppl.LoadList("dbo.tbl_Core_Rooms.RoomId", BaseId.ToString(), false, Time);
                            s = "<p >Current Timetable for <b>" + sr1.m_roomcode + "</b></p>";
                            s += ttw1.OutputTT_string("style = \"font-size:small ;\"", true, type, ref m_ppl, Time);
                        }
                        break;
                    case TT_writer.TimetableType.Booking:
                        {
                            SimpleStaff ss1 = new SimpleStaff(BaseId);
                            m_ppl.LoadList("dbo.tbl_Core_Staff.StaffId", BaseId.ToString(), false, Time);
                            s = "<p >Booking Timetable for <b>" + ss1.m_PersonGivenName + " " + ss1.m_PersonSurname + "</b> for week beginning:  <b>" + Time.ToLongDateString() + "</b></p>";
                            s += ttw1.OutputTT_string("style = \"font-size:small ;\"", false, type, ref m_ppl, Time);
                        }
                        break;
                    default:
                        break;
                }
                output.Write(s);
                if (type != TT_writer.TimetableType.Booking)
                    output.Write("<br>For date:  ");
                else
                    output.Write("<br>For Week beginning:  " + Time.ToLongDateString());
                datebox.RenderControl(output);
                button.RenderControl(output);
                button2.RenderControl(output);
                calendar1.RenderControl(output);

            }
        }
    }


    /// //////////////////////////////////////////////////////////// testing
  
             
    
    [ToolboxData("<{0}:StudentDevelopmentControl runat=server></{0}:StudentDevelopmentControl>")]
    public class StudentDevelopmentControl : WebControl, INamingContainer, System.Web.UI.IPostBackEventHandler
    {
        private System.Web.UI.WebControls.SqlDataSource data1 = new SqlDataSource();
        private System.Web.UI.WebControls.GridView GridView1 = new GridView();
        private System.Web.UI.WebControls.BoundField ColumnID = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnDate = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnText = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnType = new BoundField();
        // private System.Web.UI.WebControls.BoundField ColumnSanction = new BoundField();
        //private System.Web.UI.WebControls.BoundField ColumnComplete = new BoundField();
        private System.Web.UI.WebControls.BoundField StaffReporting = new BoundField();
        private System.Web.UI.WebControls.ButtonColumn EditColumn = new ButtonColumn();
        private System.Web.UI.WebControls.ButtonField EditButton = new ButtonField();
        private System.Web.UI.WebControls.ButtonField DeleteButton = new ButtonField();

        [Bindable(true)]
        [Category("Data")]
        [Localizable(true)]
        public Guid StudentId
        {
            get { return ((ViewState["StudentId"] == null) ? Guid.Empty : (Guid)ViewState["StudentId"]); }
            set { ViewState["StudentId"] = value; page_No = 0; }
        }
        public int page_No
        {
            get { return (int)ViewState["page_No"]; }
            set { ViewState["page_No"] = value; }
        }
        public StudentDevelopmentControl()
        {
            Encode en = new Encode();
            data1.ProviderName = System.Configuration.ConfigurationManager.ConnectionStrings["CervalConnectionString"].ProviderName;
            data1.ConnectionString = en.GetCervalConnectionString();
            Controls.Add(data1);
            data1.ID = "data2";
            Controls.Add(GridView1);
            SetupControls();
            page_No = GridView1.PageIndex;
            GridView1.PageIndexChanged += new EventHandler(GridView1_PageIndexChanged);
        }
        public void RaisePostBackEvent(string eventArgument)
        {
            string s = eventArgument;
            if (s == "ReEnable")
            {

            }
        }
        private void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            page_No = GridView1.PageIndex;//needed to preserve the page no between calls.

        }
        private void SetupControls()
        {
            GridView1.EnableViewState = true;
            GridView1.ID = "GridView2"; GridView1.AllowSorting = true; GridView1.AllowPaging = true;
            GridView1.HeaderStyle.Font.Size = FontUnit.XSmall; GridView1.AutoGenerateColumns = false;
            GridView1.BackColor = System.Drawing.Color.White; GridView1.BorderStyle = BorderStyle.None;
            GridView1.BorderColor = System.Drawing.Color.FromArgb(003399); GridView1.BorderWidth = 1;
            GridView1.CellPadding = 4; //GridView1.DataKeyNames = "StudentDevelopmentID";
            GridView1.DataSourceID = "data2"; GridView1.Width = 610;
            GridView1.PageSize = 8;

            GridView1.Columns.Add(ColumnID);
            ColumnID.DataField = "StudentDevelopmentID"; ColumnID.HeaderText = "Id"; ColumnID.ReadOnly = true; ColumnID.SortExpression = "StudentDevelopmentID";
            ColumnID.ItemStyle.Font.Size = FontUnit.XXSmall; ColumnID.ItemStyle.ForeColor = System.Drawing.Color.LightGray;
            ColumnID.ItemStyle.Width = 20; ColumnID.ItemStyle.Wrap = true;
            ColumnID.Visible = false;

            GridView1.Columns.Add(ColumnDate);
            ColumnDate.DataField = "StudentDevelopmentDate"; ColumnDate.HeaderText = "Date"; ColumnDate.ReadOnly = true; ColumnDate.SortExpression = "StudentDevelopmentDate";
            ColumnDate.ItemStyle.Font.Size = FontUnit.XSmall; ColumnDate.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnDate.ItemStyle.Width = 40; ColumnDate.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnText);
            ColumnText.DataField = "StudentDevelopmentText"; ColumnText.HeaderText = "Description"; ColumnText.ReadOnly = true; ColumnText.SortExpression = "StudentDevelopmentText";
            ColumnText.ItemStyle.Font.Size = FontUnit.XSmall; ColumnText.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnText.ItemStyle.Width = 400; ColumnText.ItemStyle.Wrap = true;
            
            //GridView1.Columns.Add(ColumnType);
            //ColumnType.DataField = "StudentDevelopment";
            //ColumnType.HeaderText = "Type";
            //ColumnType.ReadOnly = true;
            //ColumnType.SortExpression = "StudentDevelopment";
            //ColumnType.ItemStyle.Font.Size = FontUnit.XSmall;
            //ColumnType.ItemStyle.ForeColor = System.Drawing.Color.Black;
            //ColumnType.ItemStyle.Width = 50;
            //ColumnType.ItemStyle.Wrap = true;
            ////
            GridView1.Columns.Add(StaffReporting);
            StaffReporting.DataField = "StaffCode";
            StaffReporting.HeaderText = "Staff";
            StaffReporting.ReadOnly = true;
            StaffReporting.SortExpression = "StaffCode";
            StaffReporting.ItemStyle.Font.Size = FontUnit.XSmall;
            StaffReporting.ItemStyle.ForeColor = System.Drawing.Color.Black;
            //StaffReporting.ItemStyle.Width = 30; ColumnComplete.ItemStyle.Wrap = true;



        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (StudentId != Guid.Empty)
            {
                SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
                GridView1.Visible = true;
                data1.SelectCommand = "SELECT [StudentDevelopmentID], [StudentDevelopmentDate], [StudentDevelopmentText], [StaffCode], [StudentDevelopmentPairs] FROM [qry_Cerval_Core_student_StudentDevelopment2]  WHERE (StudentAdmissionNumber ='" + pupil1.m_adno.ToString() + "' ) ORDER BY [StudentDevelopmentDate] DESC";
                GridView1.PageIndex = page_No;
                output.Write("<p >Student Development Log for <b>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</b></p>");
                GridView1.RenderControl(output);
            }
            else
            {
                output.Write("StudentDevelopment");
            }
        }
    }

    [SupportsEventValidation]
    [Serializable]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [DefaultEvent("Submit"), DefaultProperty("ButtonText")]
    [ToolboxData("<{0}:Register runat=\"server\"> </{0}:Register>")]
    [ViewStateModeById]
    public class EditStudentDevelopmentControl : WebControl, INamingContainer, IPostBackEventHandler
    {
        #region controls definition
        //controls for the grid view...
        private System.Web.UI.WebControls.SqlDataSource EditData1 = new SqlDataSource();
        private System.Web.UI.WebControls.GridView EditGridView1 = new GridView();
        private System.Web.UI.WebControls.BoundField ColumnID = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnDate = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnText = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnType = new BoundField();
        //private System.Web.UI.WebControls.BoundField ColumnSanction = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnComplete = new BoundField();
        private System.Web.UI.WebControls.ButtonColumn EditColumn = new ButtonColumn();
        private System.Web.UI.WebControls.ButtonField EditButton = new ButtonField();
        private System.Web.UI.WebControls.ButtonField DeleteButton = new ButtonField();


        private TextBox TextBoxStudentDevelopmentText = new TextBox();
        private TextBox TextBoxStudentDevelopmentDate = new TextBox();
        private DropDownList DropDownListType = new DropDownList();
        private TextBox TextBoxStudentDevelopmentPairs = new TextBox();
        private DropDownList DropDownStudentDevelopmentStaff = new DropDownList();
        private Button UpdateButton = new Button();
        private Button SaveButton = new Button();
        private Button CancelButton = new Button();

        #endregion

        #region variables
        [Bindable(true)]
        [Category("Data")]
        [Localizable(true)]
        public Guid StudentId
        {
            get { return ((ViewState["StudentId"] == null) ? Guid.Empty : (Guid)ViewState["StudentId"]); }
            set { ViewState["StudentId"] = value; UpdateGrid(); page_No = 0; }
        }
        public Guid StudentDevelopmentId
        {
            get { return ((ViewState["StudentDevelopmentId"] == null) ? Guid.Empty : (Guid)ViewState["StudentDevelopmentId"]); }
            set { ViewState["StudentDevelopmentId"] = value; page_No = 0; }
        }
        public int page_No
        {
            get { return (int)ViewState["page_No"]; }
            set { ViewState["page_No"] = value; }
        }
        public bool IsEdit
        {
            get { return (bool)ViewState["IsEdit"]; }
            set { ViewState["IsEdit"] = value; }
        }
        public bool IsNew
        {
            get { return (bool)ViewState["IsNew"]; }
            set
            {
                ViewState["IsNew"] = value;
                if (value)
                {
                    TextBoxStudentDevelopmentDate.Text = DateTime.Now.ToShortDateString();
                    TextBoxStudentDevelopmentPairs.Text = "";
                    TextBoxStudentDevelopmentText.Text = "";
                }
            }
        }

        public ArrayList m_ControlList = new ArrayList();
        public event EventHandler Finished;
        System.Collections.Generic.Dictionary<string, System.Delegate> eventTable;


        private static readonly object EventSubmitKey = new object();

        private Hashtable fred = new Hashtable();
        [Category("Action"), Description("Raised when the user clicks the button.")]
        public event EventHandler Submit
        {
            add
            {
                eventTable = new System.Collections.Generic.Dictionary<string, System.Delegate>();
                eventTable.Add("Event1", value);
                eventTable.Add("Event2", value);
                Events.AddHandler(EventSubmitKey, value);
                //fred.Add(EventSubmitKey, Events);
                //ViewState.Add("fred", value); //this doesnt...  not serialisable
                //ViewState.Add("fred", eventTable);
                EventHandler SubmitHandler = (EventHandler)Events[EventSubmitKey];
                if (SubmitHandler != null)
                {
                    //SubmitHandler(this, new EventArgs());  this work
                }
            }
            remove
            {
                Events.RemoveHandler(EventSubmitKey, value);
            }
        }

        protected virtual void OnSubmit(EventArgs e)
        {
            //fred = (Hashtable)ViewState["fred"];

            //EventHandler SubmitHandler = (EventHandler)((EventHandlerList)fred[EventSubmitKey])[EventSubmitKey]; ;


        }

        #endregion

        #region methods

        public EditStudentDevelopmentControl()
        {
            IsEdit = false; IsNew = false;
            Encode en = new Encode();
            EditData1.ProviderName = System.Configuration.ConfigurationManager.ConnectionStrings["CervalConnectionString"].ProviderName;
            EditData1.ConnectionString = en.GetCervalConnectionString();


            Controls.Add(EditData1);
            EditData1.ID = "EditData1";
            Controls.Add(EditGridView1);
            SetupGrid();
            page_No = EditGridView1.PageIndex;
            EditGridView1.PageIndexChanged += new EventHandler(GridView1_PageIndexChanged);
            SetUpEditControls();
            StudentDevelopmentId = Guid.Empty;
        }
        private void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            page_No = EditGridView1.PageIndex;//needed to preserve the page no between calls.

        }
        private void SetupGrid()
        {
            EditGridView1.EnableViewState = true;
            EditGridView1.ID = "EditGridView1"; EditGridView1.AllowSorting = true; EditGridView1.AllowPaging = false;
            EditGridView1.HeaderStyle.Font.Size = FontUnit.XSmall; EditGridView1.AutoGenerateColumns = false;
            EditGridView1.BackColor = System.Drawing.Color.White; EditGridView1.BorderStyle = BorderStyle.Groove;
            EditGridView1.BorderColor = System.Drawing.Color.FromArgb(003399); EditGridView1.BorderWidth = 1;
            EditGridView1.SelectedRowStyle.BackColor = System.Drawing.Color.LightSalmon;
            EditGridView1.CellPadding = 4; //GridView1.DataKeyNames = "StudentDevelopmentID";
            EditGridView1.DataSourceID = "EditData1"; EditGridView1.Width = 610;
            EditGridView1.PageSize = 8;

            EditGridView1.Columns.Add(ColumnID);
            ColumnID.DataField = "StudentDevelopmentID"; ColumnID.HeaderText = "Id"; ColumnID.ReadOnly = true; ColumnID.SortExpression = "StudentDevelopmentID";
            ColumnID.ItemStyle.Font.Size = FontUnit.XXSmall; ColumnID.ItemStyle.ForeColor = System.Drawing.Color.LightGray;
            ColumnID.ItemStyle.Width = 20; ColumnID.ItemStyle.Wrap = true;
            ColumnID.Visible = true;

            EditGridView1.Columns.Add(ColumnDate);
            ColumnDate.DataField = "StudentDevelopmentDate"; ColumnDate.HeaderText = "Date"; ColumnDate.ReadOnly = true; ColumnDate.SortExpression = "StudentDevelopmentDate";
            ColumnDate.ItemStyle.Font.Size = FontUnit.XSmall; ColumnDate.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnDate.ItemStyle.Width = 40; ColumnDate.ItemStyle.Wrap = true;

            EditGridView1.Columns.Add(ColumnText);
            ColumnText.DataField = "StudentDevelopmentText"; ColumnText.HeaderText = "Description"; ColumnText.ReadOnly = true; ColumnText.SortExpression = "StudentDevelopmentText";
            ColumnText.ItemStyle.Font.Size = FontUnit.XSmall; ColumnText.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnText.ItemStyle.Width = 400; ColumnText.ItemStyle.Wrap = true;

            //EditGridView1.Columns.Add(ColumnType);
            //ColumnType.DataField = "StudentDevelopment";
            //ColumnType.HeaderText = "Type";
            //ColumnType.ReadOnly = true;
            //ColumnType.SortExpression = "StudentDevelopment";
            //ColumnType.ItemStyle.Font.Size = FontUnit.XSmall;
            //ColumnType.ItemStyle.ForeColor = System.Drawing.Color.Black;
            //ColumnType.ItemStyle.Width = 50;
            //ColumnType.ItemStyle.Wrap = true;


            EditGridView1.Columns.Add(EditButton);
            EditButton.ButtonType = ButtonType.Button; EditButton.CommandName = "Edit_Button"; EditButton.Text = "Edit";
            EditButton.ItemStyle.Width = 20; EditButton.ItemStyle.Font.Size = FontUnit.XSmall;
            EditGridView1.RowCommand += new GridViewCommandEventHandler(GridView1_RowCommand);

            EditGridView1.Columns.Add(DeleteButton);
            DeleteButton.ButtonType = ButtonType.Button; DeleteButton.CommandName = "Delete_Button"; DeleteButton.Text = "Del";
            DeleteButton.ItemStyle.Width = 20; DeleteButton.ItemStyle.Font.Size = FontUnit.XSmall;

        }
        protected void GridView1_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            //calls here for any command- including a sort
            int row = Convert.ToInt32(e.CommandArgument);
            GridViewRow row1 = EditGridView1.Rows[row];
            string s = row1.Cells[0].Text;
            if (e.CommandName == "Edit_Button")
            {
                IsEdit = true;
                StudentDevelopmentId = new Guid(s);
                //setup controls
                //populate controls
                StudentDevelopment StuDe1 = new StudentDevelopment(); StuDe1.Load(StudentDevelopmentId);
                TextBoxStudentDevelopmentDate.Text = StuDe1.Date.ToShortDateString();
                try { DropDownListType.SelectedIndex = DropDownListType.Items.IndexOf(DropDownListType.Items.FindByValue(StuDe1.Type.ToString())); }
                catch { }
                TextBoxStudentDevelopmentPairs.Text = StuDe1.StudentDevelopmentPairs;
                s = StuDe1.StaffID.ToString();
                try { DropDownStudentDevelopmentStaff.SelectedIndex = DropDownStudentDevelopmentStaff.Items.IndexOf(DropDownStudentDevelopmentStaff.Items.FindByValue(s)); }
                catch { }
                TextBoxStudentDevelopmentText.Text = StuDe1.Text;
                //StudentSanction StuSanct1 = new StudentSanction();
               // StuSanct1.Load_forStudentDevelopment(StudentDevelopmentId);
                // if (StuSanct1.Id != Guid.Empty)// if there isn't one don't show it!
                //{
                //  DropDownListSanctionType.SelectedIndex = StuSanct1.SanctionType;
                // TextBoxSanctionCount.Text = StuSanct1.count.ToString();
                //CheckBoxSanctionCompleted.Checked = StuSanct1.completed;
                //TextBoxSanctionDate.Text = StuSanct1.SanctionDate.ToShortDateString();
                //TextBoxSanctionWorkSet.Text = StuSanct1.workset;
                //}
                //else
                // {
                //   PreviousSanctionList = "";
                //  DropDownListSanctionType.SelectedIndex = 0;
                // TextBoxSanctionCount.Text = "";
                //CheckBoxSanctionCompleted.Checked = false;
                // TextBoxSanctionDate.Text = "";
                //  TextBoxSanctionWorkSet.Text = "";
                //}
            }
            if (e.CommandName == "Delete_Button")
            {
                StudentDevelopmentId = new Guid(s);
                StudentDevelopment st1 = new StudentDevelopment();
                st1.Load(StudentDevelopmentId);
                st1.Delete();
               // StudentSanction sts1 = new StudentSanction();
               //sts1.Load_forStudentDevelopment(StudentDevelopmentId);
               //sts1.Delete();
                IsEdit = false;
                Exit();
            }

        }
        private void SetUpEditControls()
        {
            Controls.Add(TextBoxStudentDevelopmentText);
            TextBoxStudentDevelopmentText.TextMode = TextBoxMode.MultiLine; TextBoxStudentDevelopmentText.Height = 100; TextBoxStudentDevelopmentText.Width = 600;
            Controls.Add(TextBoxStudentDevelopmentDate);
            TextBoxStudentDevelopmentDate.Text = DateTime.Now.ToShortDateString(); TextBoxStudentDevelopmentDate.ReadOnly = true;
            TextBoxStudentDevelopmentDate.Width = 80;

      
            Controls.Add(TextBoxStudentDevelopmentPairs); TextBoxStudentDevelopmentPairs.Width = 230;
            Controls.Add(DropDownStudentDevelopmentStaff);
            Controls.Add(UpdateButton);
            Controls.Add(SaveButton);
            UpdateButton.Text = "Update";
            UpdateButton.Click += new EventHandler(UpdateButton_Click);
            SaveButton.Text = "Save";
            SaveButton.Click += new EventHandler(SaveButton_Click);
            SaveButton.ID = "SaveButton1";
            Controls.Add(CancelButton);
            CancelButton.Text = "Cancel";
            CancelButton.Click += new EventHandler(CancelButton_Click);
            ViewState.Add("Clist", m_ControlList);
            StaffList sl = new StaffList(); sl.LoadList(DateTime.Now, false);
            foreach (SimpleStaff s in sl.m_stafflist)
            {
                ListItem l = new ListItem(s.m_Title + " " + s.m_PersonGivenName + " " + s.m_PersonSurname, s.m_StaffId.ToString());
                DropDownStudentDevelopmentStaff.Items.Add(l);
            }
        }

        
        public void ClearControls(string staffId)
       {

         TextBoxStudentDevelopmentText.Text = "";
          if (staffId != "")
        {
           DropDownStudentDevelopmentStaff.SelectedIndex = DropDownStudentDevelopmentStaff.Items.IndexOf(DropDownStudentDevelopmentStaff.Items.FindByValue(staffId));
        }
         ViewState.Add("Clist", m_ControlList);
         }


        private bool GetDatefromText(TextBox b, out DateTime d)
        {
            DateTime d1 = new DateTime(); bool f = true;
            try
            {
                b.BackColor = System.Drawing.Color.White;
                d1 = System.Convert.ToDateTime(b.Text);
            }
            catch
            {
                b.BackColor = System.Drawing.Color.Red;
                f = false;
            }
            d = d1;
            return f;
        }
        private bool GetIntfromText(TextBox b, out int n)
        {
            bool f = true;
            n = 0;
            try
            {
                b.BackColor = System.Drawing.Color.White;
                n = System.Convert.ToInt32(b.Text);
            }
            catch
            {
                b.BackColor = System.Drawing.Color.Red;
                f = false;
            }
            return f;
        }
        private string CleanInvertedCommas(string s)
        {
            int i = 0; i = s.IndexOf("'", i);
            while (i > 0) { i++; s = s.Substring(0, i) + "'" + s.Substring(i); i++; i = s.IndexOf("'", i); }
            return s;

        }
        private void Exit()
        {
            this.Visible = false; IsEdit = false; IsNew = false;
           
            m_ControlList = (ArrayList)ViewState["Clist"];
            foreach (string s in m_ControlList)
            {
                Control c1 = (Control)Parent.FindControl(s); c1.Visible = true;
            }
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Exit();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            StudentDevelopment StuDe1 = new StudentDevelopment();

            // testing taken out changed StuIn to StuDe - StudentDevelopment StuIn1 = new StudentDevelopment();
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
            StuDe1.StudentID = StudentId; StuDe1.AdmissionNumber = pupil1.m_adno;
            StuDe1.ID = new Guid(); StuDe1.ID = Guid.Empty;
            if (UpdateStudentDevelopment(StuDe1)) Exit();
            StuDe1.Save();
        }
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            StudentDevelopment StuDe1 = new StudentDevelopment(); StuDe1.Load(StudentDevelopmentId);
            if (UpdateStudentDevelopment(StuDe1)) Exit();
        }


        // test taking out

        private bool UpdateStudentDevelopment(StudentDevelopment StuDe1)
        {
            bool IsNew = false;
            if (StuDe1.ID == Guid.Empty) IsNew = true;
            if (!GetDatefromText(TextBoxStudentDevelopmentDate, out StuDe1.Date)) return false;
            //was already taken out // StuDe1.Type = System.Convert.ToInt32(DropDownListType.SelectedValue);
            StuDe1.StudentDevelopmentPairs = TextBoxStudentDevelopmentPairs.Text;
            StuDe1.Text = CleanInvertedCommas(TextBoxStudentDevelopmentText.Text);
            StuDe1.StaffID = new Guid(DropDownStudentDevelopmentStaff.SelectedValue);
            StuDe1.Save();
            try { StuDe1.Save(); }
            catch { return false; }


            


            if (IsNew)
            {
                return true;
            }
            return true;
            //return true;
        }

            //and this 2
        //here

      



                    
      


        private string GetQueryStringFormList(string form)
        {
            string s = "SELECT dbo.qry_Cerval_Core_Student.PersonGivenName, dbo.qry_Cerval_Core_Student.PersonSurname, dbo.qry_Cerval_Core_StudentWithFormGroup.GroupCode, ";
            s += "dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentDate, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentText, ";
            s += "dbo.qry_Cerval_Core_StudentWithFormGroup.GroupRegistrationYear, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentID, dbo.tbl_Core_Students_StudentDevelopment.StudentID,  ";
            s += "dbo.tbl_Core_Students_StudentDevelopment.StudentAdmissionNumber, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentReportingStaffID,  ";
            s += "dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentPairs ";
            s += "FROM  dbo.tbl_Core_Students_StudentDevelopment INNER JOIN ";
            s += "dbo.qry_Cerval_Core_StudentWithFormGroup ON  ";
            s += "dbo.tbl_Core_Students_StudentDevelopment.StudentID = dbo.qry_Cerval_Core_StudentWithFormGroup.StudentId INNER JOIN ";
            s += "dbo.qry_Cerval_Core_Student ON dbo.qry_Cerval_Core_StudentWithFormGroup.StudentId = dbo.qry_Cerval_Core_Student.StudentId ";
            s += "WHERE (dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentDate > CONVERT(DATETIME, '2009-01-01 00:00:00', 102)) AND  ";
            s += "(dbo.qry_Cerval_Core_StudentWithFormGroup.GroupCode = '" + form + "') ";
            return s;
        }
        private string GetQueryStringGroupList(string group)
        {
            string s = " SELECT dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentID, dbo.tbl_Core_Students_StudentDevelopment.StudentID,  ";
            s += " dbo.tbl_Core_Students_StudentDevelopment.StudentAdmissionNumber, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentDate,   ";
            s += " dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentText, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentReportingStaffID, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentPairs,   ";
            s += " dbo.qry_Cerval_Core_Student.PersonTitle, dbo.qry_Cerval_Core_Student.PersonGivenName, dbo.qry_Cerval_Core_Student.PersonSurname,   ";
            s += " dbo.tbl_Core_Groups.GroupCode  ";
            s += " FROM  dbo.tbl_Core_Groups INNER JOIN  ";
            s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN  ";
            s += " dbo.tbl_Core_Students_StudentDevelopment INNER JOIN  ";
            s += " dbo.qry_Cerval_Core_Student ON dbo.tbl_Core_Students_StudentDevelopment.StudentID = dbo.qry_Cerval_Core_Student.StudentId ON   ";
            s += " dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students_StudentDevelopment.StudentID  ";
            s += " WHERE (dbo.tbl_Core_Student_Groups.MemberFrom < { fn NOW() }) AND (dbo.tbl_Core_Student_Groups.MemberUntil > { fn NOW() }) AND   ";
            s += " (dbo.tbl_Core_Groups.GroupValidFrom < { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupValidUntil > { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupCode = '" + group + "')  ";

            return s;
        }
        protected void UpdateGrid()
        {
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
           EditData1.SelectCommand = "SELECT [StudentDevelopmentID], [StudentDevelopmentDate], [StudentDevelopmentText], [StaffCode], [StudentDevelopmentPairs] FROM [qry_Cerval_Core_student_StudentDevelopment2]  WHERE (StudentAdmissionNumber ='" + pupil1.m_adno.ToString() + "' ) ORDER BY [StudentDevelopmentDate] DESC ";
            ///testing
           //EditData1.SelectCommand = "SELECT [StudentDevelopmentID], [StudentDevelopmentDate], [StudentDevelopmentText], [StaffCode], [StudentDevelopmentPairs], [StudentDevelopment] FROM [tbl_Core_Students_StudentDevelopment]  WHERE (StudentAdmissionNumber ='" + pupil1.m_adno.ToString() + "' ) ORDER BY [StudentDevelopmentDate] DESC ";
           
            
            EditGridView1.DataBind();
        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (StudentId != Guid.Empty)
            {
                SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
                if (((!IsNew) && (!IsEdit)))
                {
                    EditGridView1.Visible = true;
                    EditData1.SelectCommand = "SELECT [StudentDevelopmentID], [StudentDevelopmentDate], [StudentDevelopmentText], [StaffCode], [StudentDevelopmentPairs] FROM [qry_Cerval_Core_student_StudentDevelopment2]  WHERE (StudentAdmissionNumber ='" + pupil1.m_adno.ToString() + "' ) ORDER BY [StudentDevelopmentDate] DESC";
                    EditGridView1.PageIndex = page_No;
                    output.Write("<p >Student Development Log for <b>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</b></p><br>");
                    CancelButton.RenderControl(output);
                    EditGridView1.RenderControl(output);
                }
                if (IsEdit)
                {
                    RenderEditControls("Edit Student Development for ", output, pupil1);
                }
                if (IsNew)
                {
                    RenderEditControls("Create New Student Development report for ", output, pupil1);
                }
            }
            else
            {
                output.Write("StudentDevelopment");
            }
        }
        protected void RenderEditControls(string title, HtmlTextWriter output, SimplePupil pupil1)
        {
            EditGridView1.Visible = false;
            UpdateButton.Visible = IsEdit; SaveButton.Visible = IsNew;
            output.Write("<p > " + title + " <b>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</b></p>");
            output.Write("Student Development Date: "); TextBoxStudentDevelopmentDate.RenderControl(output);
            output.Write("<br>Student Development Pairs: "); TextBoxStudentDevelopmentPairs.RenderControl(output);
            output.Write("<br>Student Development Staff: "); DropDownStudentDevelopmentStaff.RenderControl(output);
            output.Write("<br>Student Development Text <br>"); TextBoxStudentDevelopmentText.RenderControl(output);

            SaveButton.RenderControl(output); UpdateButton.RenderControl(output);
            CancelButton.RenderControl(output);


        }
        #endregion

        #region PostBackEventHandler
        public virtual void RaisePostBackEvent(string eventArgument)
        {
            string s = eventArgument;
        }
        #endregion
    }
    /// //////////////////////////////////////////////////////////// testing

    public class GroupStudentDevelopmentControl : WebControl
    {

        private System.Web.UI.WebControls.SqlDataSource data1 = new SqlDataSource();
        private System.Web.UI.WebControls.GridView GridView1 = new GridView();
        private System.Web.UI.WebControls.BoundField ColumnID = new BoundField();
        private System.Web.UI.WebControls.BoundField StudentFirstName = new BoundField();
        private System.Web.UI.WebControls.BoundField StudentSurname = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnDate = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnText = new BoundField();


        [Bindable(true)]
        [Category("Data")]
        [Localizable(true)]
        public string GroupCode
        {
            get { return ((ViewState["GroupCode"] == null) ? "" : (string)ViewState["GroupCode"]); }
            set { ViewState["GroupCode"] = value; page_No = 0; }
        }

        public int page_No
        {
            get { return (int)ViewState["page_No"]; }
            set { ViewState["page_No"] = value; }
        }


        public GroupStudentDevelopmentControl()
        {
            Encode en = new Encode();
            data1.ConnectionString = en.GetCervalConnectionString();
            data1.ProviderName = System.Configuration.ConfigurationManager.ConnectionStrings["CervalConnectionString"].ProviderName;
            

            Controls.Add(data1);
            data1.ID = "data2";
            Controls.Add(GridView1);
            SetupGrid();
            page_No = GridView1.PageIndex;
            GridView1.PageIndexChanged += new EventHandler(GridView1_PageIndexChanged);

        }
        private void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            page_No = GridView1.PageIndex;//needed to preserve the page no between calls.
        }
        private void SetupGrid()
        {
            GridView1.ID = "GridView2"; GridView1.AllowSorting = true; GridView1.AllowPaging = true; ;
            GridView1.HeaderStyle.Font.Size = FontUnit.XSmall; GridView1.AutoGenerateColumns = false;
            GridView1.BackColor = System.Drawing.Color.White; GridView1.BorderStyle = BorderStyle.None;
            GridView1.BorderColor = System.Drawing.Color.FromArgb(003399); GridView1.BorderWidth = 1;
            GridView1.CellPadding = 4; //GridView1.DataKeyNames = "StudentDevelopmentID";
            GridView1.DataSourceID = "data2"; GridView1.Width = 610;
            GridView1.PageSize = 20;

            GridView1.Columns.Add(ColumnID);
            ColumnID.DataField = "dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentID"; ColumnID.HeaderText = "Id"; ColumnID.ReadOnly = true; ColumnID.SortExpression = "dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentID";
            ColumnID.ItemStyle.Font.Size = FontUnit.XSmall; ColumnID.ItemStyle.ForeColor = System.Drawing.Color.Gray;
            ColumnID.ItemStyle.Width = 20; ColumnID.ItemStyle.Wrap = true;
            ColumnID.Visible = false;



            GridView1.Columns.Add(StudentFirstName);
            StudentFirstName.DataField = "_GivenName"; StudentFirstName.HeaderText = "FirstName"; StudentFirstName.ReadOnly = true; StudentFirstName.SortExpression = "_GivenName";
            StudentFirstName.ItemStyle.Font.Size = FontUnit.XSmall; StudentFirstName.ItemStyle.ForeColor = System.Drawing.Color.Black;
            StudentFirstName.ItemStyle.Width = 50; StudentFirstName.ItemStyle.Wrap = true;

            GridView1.Columns.Add(StudentSurname);
            StudentSurname.DataField = "_Surname"; StudentSurname.HeaderText = "Surname"; StudentSurname.ReadOnly = true; StudentSurname.SortExpression = "_Surname";
            StudentSurname.ItemStyle.Font.Size = FontUnit.XSmall; StudentSurname.ItemStyle.ForeColor = System.Drawing.Color.Black;
            StudentSurname.ItemStyle.Width = 30; StudentSurname.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnDate);
            ColumnDate.DataField = "_date"; ColumnDate.HeaderText = "Date"; ColumnDate.ReadOnly = true; ColumnDate.SortExpression = "_date";
            ColumnDate.ItemStyle.Font.Size = FontUnit.XSmall; ColumnDate.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnDate.ItemStyle.Width = 40; ColumnDate.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnText);
            ColumnText.DataField = "_text"; ColumnText.HeaderText = "Description"; ColumnText.ReadOnly = true; ColumnText.SortExpression = "_text";
            ColumnText.ItemStyle.Font.Size = FontUnit.XSmall; ColumnText.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnText.ItemStyle.Width = 400; ColumnText.ItemStyle.Wrap = true;


        }


        //void DropDownListSanctionType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //  throw new NotImplementedException();
        // }

        private string GetQueryStringFormList(string form)
        {
            string s = "SELECT dbo.qry_Cerval_Core_Student.PersonGivenName, dbo.qry_Cerval_Core_Student.PersonSurname, dbo.qry_Cerval_Core_StudentWithFormGroup.GroupCode, ";
            s += "dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentDate, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentText, ";
            s += "dbo.qry_Cerval_Core_StudentWithFormGroup.GroupRegistrationYear, dbo.tbl_Core_Students_a.StudentDevelopmentID, dbo.tbl_Core_Students_StudentDevelopment.StudentID,  ";
            s += "dbo.tbl_Core_Students_StudentDevelopment.StudentAdmissionNumber, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentReportingStaffID,  ";
            s += "dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentPairs ";
            s += "FROM  dbo.tbl_Core_Students_StudentDevelopment INNER JOIN ";
            s += "dbo.qry_Cerval_Core_StudentWithFormGroup ON  ";
            s += "dbo.tbl_Core_Students_StudentDevelopment.StudentID = dbo.qry_Cerval_Core_StudentWithFormGroup.StudentId INNER JOIN ";
            s += "dbo.qry_Cerval_Core_Student ON dbo.qry_Cerval_Core_StudentWithFormGroup.StudentId = dbo.qry_Cerval_Core_Student.StudentId ";
            s += "WHERE (dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentDate > CONVERT(DATETIME, '2009-01-01 00:00:00', 102)) AND  ";
            s += "(dbo.qry_Cerval_Core_StudentWithFormGroup.GroupCode = '" + form + "') ";
            return s;
        }

        private string GetQueryStringGroupList(string group, DateTime FirstStudentDevelopmentDate)
        {
            string date_s = "CONVERT(DATETIME, '" + FirstStudentDevelopmentDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT  dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentID, dbo.tbl_Core_Students_StudentDevelopment.StudentID,  ";
            s += " dbo.tbl_Core_Students_StudentDevelopment.StudentAdmissionNumber, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentDate AS _date,   ";
            s += " dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentText AS _text, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentReportingStaffID, dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentPairs,   ";
            s += " dbo.qry_Cerval_Core_Student.PersonTitle, dbo.qry_Cerval_Core_Student.PersonGivenName AS _GivenName, dbo.qry_Cerval_Core_Student.PersonSurname AS _Surname,   ";
            s += " dbo.tbl_Core_Groups.GroupCode  ";
            s += " FROM  dbo.tbl_Core_Groups INNER JOIN  ";
            s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN  ";
            s += " dbo.tbl_Core_Students_StudentDevelopment INNER JOIN  ";
            s += " dbo.qry_Cerval_Core_Student ON dbo.tbl_Core_Students_StudentDevelopment.StudentID = dbo.qry_Cerval_Core_Student.StudentId ON   ";
            s += " dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students_StudentDevelopment.StudentID  ";
            s += " WHERE (dbo.tbl_Core_Student_Groups.MemberFrom < { fn NOW() }) AND (dbo.tbl_Core_Student_Groups.MemberUntil > { fn NOW() }) AND   ";
            s += " (dbo.tbl_Core_Groups.GroupValidFrom < { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupValidUntil > { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupCode = '" + group + "')  ";
            s += " AND ( dbo.tbl_Core_Students_StudentDevelopment.StudentDevelopmentDate >" + date_s + " ) ";
            return s;
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (GroupCode != "")
            {
                GridView1.Visible = true;
                data1.SelectCommand = GetQueryStringGroupList(GroupCode, DateTime.Now.AddYears(-1));
                GridView1.PageIndex = page_No;
                output.Write("<p >Student Development Log for <b>" + GroupCode + "  </b> for previous year.</p>");
                GridView1.RenderControl(output);
            }
            else
            {
                output.Write("Student Development");
            }
        }
    }





    [ToolboxData("<{0}:IncidentConrol runat=server></{0}:IncidentConrol>")]
    public class IncidentControl : WebControl, INamingContainer, System.Web.UI.IPostBackEventHandler
    {
        private System.Web.UI.WebControls.SqlDataSource data1 = new SqlDataSource();
        private System.Web.UI.WebControls.GridView GridView1 = new GridView();
        private System.Web.UI.WebControls.BoundField ColumnID = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnDate = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnText = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnType = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnSanction = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnComplete = new BoundField();
        private System.Web.UI.WebControls.BoundField StaffReporting = new BoundField();
        private System.Web.UI.WebControls.ButtonColumn EditColumn = new ButtonColumn();
        private System.Web.UI.WebControls.ButtonField EditButton = new ButtonField();
        private System.Web.UI.WebControls.ButtonField DeleteButton = new ButtonField();

        [Bindable(true)]
        [Category("Data")]
        [Localizable(true)]
        public Guid StudentId
        {
            get { return ((ViewState["StudentId"] == null) ? Guid.Empty : (Guid)ViewState["StudentId"]); }
            set { ViewState["StudentId"] = value; page_No = 0; }
        }
        public int page_No
        {
            get { return (int)ViewState["page_No"]; }
            set { ViewState["page_No"] = value; }
        }
        public IncidentControl()
        {

            data1.ProviderName = System.Configuration.ConfigurationManager.ConnectionStrings["CervalConnectionString"].ProviderName;
            Encode en = new Encode();
            data1.ConnectionString = en.GetCervalConnectionString();

            Controls.Add(data1);
            data1.ID = "data1";
            Controls.Add(GridView1);
            SetupControls();
            page_No = GridView1.PageIndex;
            GridView1.PageIndexChanged += new EventHandler(GridView1_PageIndexChanged);
        }
        public void RaisePostBackEvent(string eventArgument)
        {
            string s = eventArgument;
            if (s == "ReEnable")
            {

            }
        }
        private void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            page_No = GridView1.PageIndex;//needed to preserve the page no between calls.

        }
        private void SetupControls()
        {
            GridView1.EnableViewState = true;
            GridView1.ID = "GridView1"; GridView1.AllowSorting = true; GridView1.AllowPaging = true;
            GridView1.HeaderStyle.Font.Size = FontUnit.XSmall; GridView1.AutoGenerateColumns = false;
            GridView1.BackColor = System.Drawing.Color.White; GridView1.BorderStyle = BorderStyle.None;
            GridView1.BorderColor = System.Drawing.Color.FromArgb(003399); GridView1.BorderWidth = 1;
            GridView1.CellPadding = 4; //GridView1.DataKeyNames = "IncidentID";
            GridView1.DataSourceID = "data1"; GridView1.Width = 610;
            GridView1.PageSize = 8;

            GridView1.Columns.Add(ColumnID);
            ColumnID.DataField = "IncidentID"; ColumnID.HeaderText = "Id"; ColumnID.ReadOnly = true; ColumnID.SortExpression = "IncidentID";
            ColumnID.ItemStyle.Font.Size = FontUnit.XXSmall; ColumnID.ItemStyle.ForeColor = System.Drawing.Color.LightGray;
            ColumnID.ItemStyle.Width = 20; ColumnID.ItemStyle.Wrap = true;
            ColumnID.Visible = false;

            GridView1.Columns.Add(ColumnDate);
            ColumnDate.DataField = "IncidentDate"; ColumnDate.HeaderText = "Date"; ColumnDate.ReadOnly = true; ColumnDate.SortExpression = "IncidentDate";
            ColumnDate.ItemStyle.Font.Size = FontUnit.XSmall; ColumnDate.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnDate.ItemStyle.Width = 40; ColumnDate.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnText);
            ColumnText.DataField = "IncidentText"; ColumnText.HeaderText = "Description"; ColumnText.ReadOnly = true; ColumnText.SortExpression = "IncidentText";
            ColumnText.ItemStyle.Font.Size = FontUnit.XSmall; ColumnText.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnText.ItemStyle.Width = 400; ColumnText.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnType);
            ColumnType.DataField = "Incident";
            ColumnType.HeaderText = "Type";
            ColumnType.ReadOnly = true;
            ColumnType.SortExpression = "Incident";
            ColumnType.ItemStyle.Font.Size = FontUnit.XSmall;
            ColumnType.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnType.ItemStyle.Width = 50;
            ColumnType.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnSanction);
            ColumnSanction.DataField = "SanctionName"; ColumnSanction.HeaderText = "Sanction"; ColumnSanction.ReadOnly = true; ColumnSanction.SortExpression = "SanctionName";
            ColumnSanction.ItemStyle.Font.Size = FontUnit.XSmall; ColumnSanction.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnSanction.ItemStyle.Width = 50; ColumnSanction.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnComplete);
            ColumnComplete.DataField = "SanctionCompleted"; ColumnComplete.HeaderText = "Complete"; ColumnComplete.ReadOnly = true; ColumnComplete.SortExpression = "SanctionCompleted";
            ColumnComplete.ItemStyle.Font.Size = FontUnit.XSmall; ColumnComplete.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnComplete.ItemStyle.Width = 30; ColumnComplete.ItemStyle.Wrap = true;


            GridView1.Columns.Add(StaffReporting);
            StaffReporting.DataField = "StaffCode";
            StaffReporting.HeaderText = "Staff";
            StaffReporting.ReadOnly = true;
            StaffReporting.SortExpression = "StaffCode";
            StaffReporting.ItemStyle.Font.Size = FontUnit.XSmall;
            StaffReporting.ItemStyle.ForeColor = System.Drawing.Color.Black;
            StaffReporting.ItemStyle.Width = 30; ColumnComplete.ItemStyle.Wrap = true;



        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (StudentId != Guid.Empty)
            {
                SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
                GridView1.Visible = true;
                data1.SelectCommand = "SELECT [IncidentID], [IncidentDate], [IncidentText], [StaffCode], [IncidentPairs], [Incident], [SanctionName], [SanctionDate], [SanctionCompleted] FROM [qry_Cerval_Core_student_incident2]  WHERE (StudentAdmissionNumber ='" + pupil1.m_adno.ToString() + "' ) ORDER BY [IncidentDate] DESC";
                GridView1.PageIndex = page_No;
                output.Write("<p >Incident Log for <b>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</b></p>");
                GridView1.RenderControl(output);
            }
            else
            {
                output.Write("Incidents");
            }
        }
    }

    [SupportsEventValidation]
    [Serializable]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [DefaultEvent("Submit"), DefaultProperty("ButtonText")]
    [ToolboxData("<{0}:Register runat=\"server\"> </{0}:Register>")]
    [ViewStateModeById]
    public class EditIncidentControl : WebControl, INamingContainer, IPostBackEventHandler
    {
        #region controls definition
        //controls for the grid view...
        private System.Web.UI.WebControls.SqlDataSource EditData1 = new SqlDataSource();
        private System.Web.UI.WebControls.GridView EditGridView1 = new GridView();
        private System.Web.UI.WebControls.BoundField ColumnID = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnDate = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnText = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnType = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnSanction = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnComplete = new BoundField();
        //private System.Web.UI.WebControls.ButtonColumn EditColumn = new ButtonColumn();
        private System.Web.UI.WebControls.ButtonField EditButton = new ButtonField();
        private System.Web.UI.WebControls.ButtonField DeleteButton = new ButtonField();

        private TextBox TextBoxIncidentText = new TextBox();
        private TextBox TextBoxIncidentDate = new TextBox();
        private DropDownList DropDownListType = new DropDownList();
        private SqlDataSource SqlDataSourceIncidentsType = new SqlDataSource();
        private TextBox TextBoxIncidentPairs = new TextBox();
        private DropDownList DropDownIncidentStaff = new DropDownList();
        private TextBox TextBoxSanctionWorkSet = new TextBox();
        private DropDownList DropDownListSanctionType = new DropDownList();
        private SqlDataSource SqlDataSourceSanctionType = new SqlDataSource();
        private TextBox TextBoxSanctionCount = new TextBox();
        private TextBox TextBoxSanctionDate = new TextBox();
        private CheckBox CheckBoxSanctionCompleted = new CheckBox();
        private Button UpdateButton = new Button();
        private Button SaveButton = new Button();
        private Button CancelButton = new Button();

        #endregion

        #region variables
        [Bindable(true)]
        [Category("Data")]
        [Localizable(true)]
        public Guid StudentId
        {
            get { return ((ViewState["StudentId"] == null) ? Guid.Empty : (Guid)ViewState["StudentId"]); }
            set { ViewState["StudentId"] = value; UpdateGrid(); page_No = 0; }
        }
        public Guid IncidentId
        {
            get { return ((ViewState["IncidentId"] == null) ? Guid.Empty : (Guid)ViewState["IncidentId"]); }
            set { ViewState["IncidentId"] = value; page_No = 0; }
        }
        public int page_No
        {
            get { return (int)ViewState["page_No"]; }
            set { ViewState["page_No"] = value; }
        }
        public bool IsEdit
        {
            get { return (bool)ViewState["IsEdit"]; }
            set { ViewState["IsEdit"] = value; }
        }
        public bool IsNew
        {
            get { return (bool)ViewState["IsNew"]; }
            set
            {
                ViewState["IsNew"] = value;
                if (value)
                {
                    TextBoxIncidentDate.Text = DateTime.Now.ToShortDateString();
                    TextBoxIncidentPairs.Text = "";
                    TextBoxIncidentText.Text = "";
                }
            }
        }
        public string PreviousSanctionList
        {
            get { return ((ViewState["PreviousSanctionList"] == null) ? "" : (string)ViewState["PreviousSanctionList"]); }
            set { ViewState["PreviousSanctionList"] = value; }
        }
        public ArrayList m_ControlList = new ArrayList();
        public event EventHandler Finished;
        System.Collections.Generic.Dictionary<string, System.Delegate> eventTable;


        private static readonly object EventSubmitKey = new object();

        private Hashtable fred = new Hashtable();
        [Category("Action"), Description("Raised when the user clicks the button.")]
        public event EventHandler Submit
        {
            add
            {
                eventTable = new System.Collections.Generic.Dictionary<string, System.Delegate>();
                eventTable.Add("Event1", value);
                eventTable.Add("Event2", value);
                Events.AddHandler(EventSubmitKey, value);
                //fred.Add(EventSubmitKey, Events);
                //ViewState.Add("fred", value); //this doesnt...  not serialisable
                //ViewState.Add("fred", eventTable);
                EventHandler SubmitHandler = (EventHandler)Events[EventSubmitKey];
                if (SubmitHandler != null)
                {
                    //SubmitHandler(this, new EventArgs());  this work
                }
            }
            remove
            {
                Events.RemoveHandler(EventSubmitKey, value);
            }
        }


        protected virtual void OnSubmit(EventArgs e)
        {
            //fred = (Hashtable)ViewState["fred"];

            //EventHandler SubmitHandler = (EventHandler)((EventHandlerList)fred[EventSubmitKey])[EventSubmitKey]; ;


        }

        #endregion

        #region methods

        public EditIncidentControl()
        {
            IsEdit = false; IsNew = false; PreviousSanctionList = "";

            EditData1.ProviderName = System.Configuration.ConfigurationManager.ConnectionStrings["CervalConnectionString"].ProviderName;
            Encode en = new Encode();
            EditData1.ConnectionString = en.GetCervalConnectionString();


            Controls.Add(EditData1);
            EditData1.ID = "EditData1";
            Controls.Add(EditGridView1);
            SetupGrid();
            page_No = EditGridView1.PageIndex;
            EditGridView1.PageIndexChanged += new EventHandler(GridView1_PageIndexChanged);
            SetUpEditControls();
            IncidentId = Guid.Empty;
        }
        private void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            page_No = EditGridView1.PageIndex;//needed to preserve the page no between calls.

        }
        private void SetupGrid()
        {
            EditGridView1.EnableViewState = true;
            EditGridView1.ID = "EditGridView1"; EditGridView1.AllowSorting = true; EditGridView1.AllowPaging = false;
            EditGridView1.HeaderStyle.Font.Size = FontUnit.XSmall; EditGridView1.AutoGenerateColumns = false;
            EditGridView1.BackColor = System.Drawing.Color.White; EditGridView1.BorderStyle = BorderStyle.Groove;
            EditGridView1.BorderColor = System.Drawing.Color.FromArgb(003399); EditGridView1.BorderWidth = 1;
            EditGridView1.SelectedRowStyle.BackColor = System.Drawing.Color.LightSalmon;
            EditGridView1.CellPadding = 4; //GridView1.DataKeyNames = "IncidentID";
            EditGridView1.DataSourceID = "EditData1"; EditGridView1.Width = 610;
            EditGridView1.PageSize = 8;

            EditGridView1.Columns.Add(ColumnID);
            ColumnID.DataField = "IncidentID"; ColumnID.HeaderText = "Id"; ColumnID.ReadOnly = true; ColumnID.SortExpression = "IncidentID";
            ColumnID.ItemStyle.Font.Size = FontUnit.XXSmall; ColumnID.ItemStyle.ForeColor = System.Drawing.Color.LightGray;
            ColumnID.ItemStyle.Width = 20; ColumnID.ItemStyle.Wrap = true;
            ColumnID.Visible = true;

            EditGridView1.Columns.Add(ColumnDate);
            ColumnDate.DataField = "IncidentDate"; ColumnDate.HeaderText = "Date"; ColumnDate.ReadOnly = true; ColumnDate.SortExpression = "IncidentDate";
            ColumnDate.ItemStyle.Font.Size = FontUnit.XSmall; ColumnDate.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnDate.ItemStyle.Width = 40; ColumnDate.ItemStyle.Wrap = true;

            EditGridView1.Columns.Add(ColumnText);
            ColumnText.DataField = "IncidentText"; ColumnText.HeaderText = "Description"; ColumnText.ReadOnly = true; ColumnText.SortExpression = "IncidentText";
            ColumnText.ItemStyle.Font.Size = FontUnit.XSmall; ColumnText.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnText.ItemStyle.Width = 400; ColumnText.ItemStyle.Wrap = true;

            EditGridView1.Columns.Add(ColumnType);
            ColumnType.DataField = "Incident";
            ColumnType.HeaderText = "Type";
            ColumnType.ReadOnly = true;
            ColumnType.SortExpression = "Incident";
            ColumnType.ItemStyle.Font.Size = FontUnit.XSmall;
            ColumnType.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnType.ItemStyle.Width = 50;
            ColumnType.ItemStyle.Wrap = true;

            EditGridView1.Columns.Add(ColumnSanction);
            ColumnSanction.DataField = "SanctionName"; ColumnSanction.HeaderText = "Sanction"; ColumnSanction.ReadOnly = true; ColumnSanction.SortExpression = "SanctionName";
            ColumnSanction.ItemStyle.Font.Size = FontUnit.XSmall; ColumnSanction.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnSanction.ItemStyle.Width = 50; ColumnSanction.ItemStyle.Wrap = true;

            EditGridView1.Columns.Add(ColumnComplete);
            ColumnComplete.DataField = "SanctionCompleted"; ColumnComplete.HeaderText = "Complete"; ColumnComplete.ReadOnly = true; ColumnComplete.SortExpression = "SanctionCompleted";
            ColumnComplete.ItemStyle.Font.Size = FontUnit.XSmall; ColumnComplete.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnComplete.ItemStyle.Width = 30; ColumnComplete.ItemStyle.Wrap = true;

            EditGridView1.Columns.Add(EditButton);
            EditButton.ButtonType = ButtonType.Button; EditButton.CommandName = "Edit_Button"; EditButton.Text = "Edit";
            EditButton.ItemStyle.Width = 20; EditButton.ItemStyle.Font.Size = FontUnit.XSmall;
            EditGridView1.RowCommand += new GridViewCommandEventHandler(GridView1_RowCommand);

            EditGridView1.Columns.Add(DeleteButton);
            DeleteButton.ButtonType = ButtonType.Button; DeleteButton.CommandName = "Delete_Button"; DeleteButton.Text = "Del";
            DeleteButton.ItemStyle.Width = 20; DeleteButton.ItemStyle.Font.Size = FontUnit.XSmall;

        }
        protected void GridView1_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            //calls here for any command- including a sort
            int row = Convert.ToInt32(e.CommandArgument);
            GridViewRow row1 = EditGridView1.Rows[row];
            string s = row1.Cells[0].Text;
            if (e.CommandName == "Edit_Button")
            {
                IsEdit = true;
                IncidentId = new Guid(s);
                //setup controls
                //populate controls
                StudentIncident StuIn1 = new StudentIncident(); StuIn1.Load(IncidentId);
                TextBoxIncidentDate.Text = StuIn1.Date.ToShortDateString();
                try { DropDownListType.SelectedIndex = DropDownListType.Items.IndexOf(DropDownListType.Items.FindByValue(StuIn1.Type.ToString())); }
                catch { }
                TextBoxIncidentPairs.Text = StuIn1.IncidentPairs;
                s = StuIn1.StaffID.ToString();
                try { DropDownIncidentStaff.SelectedIndex = DropDownIncidentStaff.Items.IndexOf(DropDownIncidentStaff.Items.FindByValue(s)); }
                catch { }
                TextBoxIncidentText.Text = StuIn1.Text;
                StudentSanction StuSanct1 = new StudentSanction();
                StuSanct1.Load_forIncident(IncidentId);
                if (StuSanct1.Id != Guid.Empty)// if there isn't one don't show it!
                {
                    DropDownListSanctionType.SelectedIndex = StuSanct1.SanctionType;
                    TextBoxSanctionCount.Text = StuSanct1.count.ToString();
                    CheckBoxSanctionCompleted.Checked = StuSanct1.completed;
                    TextBoxSanctionDate.Text = StuSanct1.SanctionDate.ToShortDateString();
                    TextBoxSanctionWorkSet.Text = StuSanct1.workset;
                }
                else
                {
                    PreviousSanctionList = "";
                    DropDownListSanctionType.SelectedIndex = 0;
                    TextBoxSanctionCount.Text = "";
                    CheckBoxSanctionCompleted.Checked = false;
                    TextBoxSanctionDate.Text = "";
                    TextBoxSanctionWorkSet.Text = "";
                }
            }
            if (e.CommandName == "Delete_Button")
            {
                IncidentId = new Guid(s);
                StudentIncident st1 = new StudentIncident();
                st1.Load(IncidentId);
                st1.Delete();
                StudentSanction sts1 = new StudentSanction();
                sts1.Load_forIncident(IncidentId);
                sts1.Delete();
                IsEdit = false;
                Exit();
            }

        }
        private void SetUpEditControls()
        {
            Controls.Add(TextBoxIncidentText);
            TextBoxIncidentText.TextMode = TextBoxMode.MultiLine; TextBoxIncidentText.Height = 100; TextBoxIncidentText.Width = 600;
            Controls.Add(TextBoxIncidentDate);
            TextBoxIncidentDate.Text = DateTime.Now.ToShortDateString(); TextBoxIncidentDate.ReadOnly = true;
            TextBoxIncidentDate.Width = 80;

            Controls.Add(DropDownListType);
            Controls.Add(SqlDataSourceIncidentsType);
            DropDownListType.DataSourceID = "SqlDataSourceIncidentsType";
            DropDownListType.ID = "IncidentTypeList1";
            DropDownListType.DataTextField = "Incident"; DropDownListType.DataValueField = "Id"; DropDownListType.Width = 180;
            Encode en = new Encode();
            SqlDataSourceIncidentsType.ConnectionString = en.GetDbConnection(); SqlDataSourceIncidentsType.ID = "SqlDataSourceIncidentsType";
            SqlDataSourceIncidentsType.SelectCommand = "SELECT Incident, Id FROM tbl_List_IncidentTypes ORDER BY Incident";

            Controls.Add(TextBoxIncidentPairs); TextBoxIncidentPairs.Width = 230;
            Controls.Add(DropDownIncidentStaff);
            StaffList sl = new StaffList(); sl.LoadList(DateTime.Now, false);
            foreach (SimpleStaff s in sl.m_stafflist)
            {
                ListItem l = new ListItem(s.m_Title + " " + s.m_PersonGivenName + " " + s.m_PersonSurname, s.m_StaffId.ToString());
                DropDownIncidentStaff.Items.Add(l);
            }

            Controls.Add(TextBoxSanctionWorkSet);
            TextBoxSanctionWorkSet.TextMode = TextBoxMode.MultiLine; TextBoxSanctionWorkSet.Height = 40; TextBoxSanctionWorkSet.Width = 600;

            Controls.Add(DropDownListSanctionType);
            DropDownListSanctionType.ID = "SanctionList1";
            DropDownListSanctionType.DataSourceID = "SqlDataSourceSanctionType";
            DropDownListSanctionType.DataTextField = "SanctionName";
            DropDownListSanctionType.DataValueField = "Id";
            DropDownListSanctionType.Width = 160;
            DropDownListSanctionType.SelectedIndexChanged += new EventHandler(DropDownListSanctionType_SelectedIndexChanged);
            DropDownListSanctionType.AutoPostBack = true;

            Controls.Add(SqlDataSourceSanctionType);
            SqlDataSourceSanctionType.ConnectionString = en.GetDbConnection(); SqlDataSourceSanctionType.ID = "SqlDataSourceSanctionType";
            SqlDataSourceSanctionType.SelectCommand = "SELECT [Id], [SanctionName] FROM [tbl_List_Sanctions]";

            DropDownListSanctionType.SelectedIndex = 0;
            Controls.Add(TextBoxSanctionCount);
            TextBoxSanctionCount.Width = 30;
            Controls.Add(TextBoxSanctionDate);
            TextBoxSanctionDate.Width = 80;
            Controls.Add(CheckBoxSanctionCompleted);
            Controls.Add(UpdateButton);
            Controls.Add(SaveButton);
            UpdateButton.Text = "Update Incident Log";
            UpdateButton.Click += new EventHandler(UpdateButton_Click);
            SaveButton.Text = "Save";
            SaveButton.Click += new EventHandler(SaveButton_Click);
            SaveButton.ID = "SaveButton1";
            Controls.Add(CancelButton);
            CancelButton.Text = "Cancel";
            CancelButton.Click += new EventHandler(CancelButton_Click);
        }
       // public void ClearControls2(string staffId)
        //{
          //  DropDownListSanctionType.SelectedIndex = -1;
           // TextBoxSanctionWorkSet.Text = "";
            //CheckBoxSanctionCompleted.Checked = false;
            //TextBoxSanctionDate.Text = "";
            //TextBoxStudentDevelopmentText.Text = "";
            //if (staffId != "")
           // {
            //    DropDownStudentDevelopmentStaff.SelectedIndex = DropDownStudentvelopmentStaff.Items.IndexOf(DropDownStudentDevelopmentStaff.Items.FindByValue(staffId));
            //}
            //ViewState.Add("Clist", m_ControlList);
        //}

        public void ClearControls(string staffId)
        {
            DropDownListSanctionType.SelectedIndex = -1;
            TextBoxSanctionWorkSet.Text = "";
            CheckBoxSanctionCompleted.Checked = false;
            TextBoxSanctionDate.Text = "";
            TextBoxIncidentText.Text = "";
            if (staffId != "")
            {
                DropDownIncidentStaff.SelectedIndex = DropDownIncidentStaff.Items.IndexOf(DropDownIncidentStaff.Items.FindByValue(staffId));
            }
            ViewState.Add("Clist", m_ControlList);
        }
        private bool GetDatefromText(TextBox b, out DateTime d)
        {
            DateTime d1 = new DateTime(); bool f = true;
            try
            {
                b.BackColor = System.Drawing.Color.White;
                d1 = System.Convert.ToDateTime(b.Text);
            }
            catch
            {
                b.BackColor = System.Drawing.Color.Red;
                f = false;
            }
            d = d1;
            return f;
        }
        private bool GetIntfromText(TextBox b, out int n)
        {
            bool f = true;
            n = 0;
            try
            {
                b.BackColor = System.Drawing.Color.White;
                n = System.Convert.ToInt32(b.Text);
            }
            catch
            {
                b.BackColor = System.Drawing.Color.Red;
                f = false;
            }
            return f;
        }
        private string CleanInvertedCommas(string s)
        {
            int i = 0; i = s.IndexOf("'", i);
            while (i > 0) { i++; s = s.Substring(0, i) + "'" + s.Substring(i); i++; i = s.IndexOf("'", i); }
            return s;

        }
        private void Exit()
        {
            this.Visible = false; IsEdit = false; IsNew = false;
            /*
            //OnSubmit(EventArgs.Empty);trying to get evnets to work....
            EventHandler handler1;
            //eventTable = (System.Collections.Generic.Dictionary<string, System.Delegate>)ViewState["fred"];
            if (null != (handler1 = (EventHandler)eventTable["Event1"]))
            {
                handler1(this, new EventArgs());
            }

            if (Finished != null)
            {
                Finished(this, new EventArgs());
            }
             * */
            m_ControlList = (ArrayList)ViewState["Clist"];
            foreach (string s in m_ControlList)
            {
                Control c1 = (Control)Parent.FindControl(s); c1.Visible = true;
            }
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Exit();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            StudentIncident StuIn1 = new StudentIncident();
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
            StuIn1.StudentID = StudentId; StuIn1.AdmissionNumber = pupil1.m_adno;
            StuIn1.ID = new Guid(); StuIn1.ID = Guid.Empty;
            if (UpdateIncident(StuIn1)) Exit();
        }
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            StudentIncident StuIn1 = new StudentIncident(); StuIn1.Load(IncidentId);
            if (UpdateIncident(StuIn1)) Exit();
        }
        private bool UpdateIncident(StudentIncident StuIn1)
        {
            bool IsNew = false;
            if (StuIn1.ID == Guid.Empty) IsNew = true;
            if (!GetDatefromText(TextBoxIncidentDate, out StuIn1.Date)) return false;
            StuIn1.Type = System.Convert.ToInt32(DropDownListType.SelectedValue);
            StuIn1.IncidentPairs = TextBoxIncidentPairs.Text;
            StuIn1.Text = CleanInvertedCommas(TextBoxIncidentText.Text);
            StuIn1.StaffID = new Guid(DropDownIncidentStaff.SelectedValue);
            StudentSanction StuSanct1 = new StudentSanction();
            if (DropDownListSanctionType.SelectedIndex > 0)//option 0 = none....
            {
                StuSanct1.Load_forIncident(StuIn1.ID);// sanction ID will be empty if none...
                StuSanct1.SanctionType = DropDownListSanctionType.SelectedIndex;
                if (!GetIntfromText(TextBoxSanctionCount, out StuSanct1.count)) return false;
                StuSanct1.completed = CheckBoxSanctionCompleted.Checked;
                if (!GetDatefromText(TextBoxSanctionDate, out StuSanct1.SanctionDate)) return false;
                StuSanct1.workset = CleanInvertedCommas(TextBoxSanctionWorkSet.Text);
            }
            //so all ready.. try saves...
            StuIn1.Save();
            //try { StuIn1.Save(); }
            //catch { return false; }

            if (DropDownListSanctionType.SelectedIndex > 0)
            {
                StuSanct1.IncidentId = StuIn1.ID;
                StuSanct1.StaffId = StuIn1.StaffID;
                StuSanct1.Save();
                //try { StuSanct1.Save(); }
                //catch { return false; }
            }
            if (IsNew)
            {
                SendNotification(StuIn1, StuSanct1);
            }
            return true;
        }

        protected void SendNotification(StudentIncident Incident, StudentSanction sanction)
        {
            MailHelper m1 = new MailHelper();
            Utility u = new Utility();
            Sanction sanct1 = new Sanction(); sanct1.Load(sanction.SanctionType);
            string s = u.GetTutorForStudent(StudentId, DateTime.Now);
            SimpleStaff staff1 = new SimpleStaff(Incident.StaffID);
            string s1 = u.Get_StaffEmailAddress(staff1.m_StaffCode);
            SimplePupil p1 = new SimplePupil(); p1.Load(StudentId);


            try
            {
                if (s != "")
                {
                    string s2 = u.Get_StaffEmailAddress(s);
                    Cerval_Configurations configs = new Cerval_Configurations(); configs.Load_All();
                    Cerval_Configuration c2 = new Cerval_Configuration();
                    if ((p1.m_form.StartsWith("7")) || (p1.m_form.StartsWith("8")) || (p1.m_form.StartsWith("9")))
                    {
                        c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SDOEmail");
                        if (c2 != null) s2 += ";" + c2.Value;

                        c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_KS3LeaderEmail");
                        if (c2 != null) s2 += ";" + c2.Value;

                    }
                    if ((p1.m_form.StartsWith("7")))
                    {
                        c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_Year7LeaderEmail");
                        if (c2 != null) s2 += ";" + c2.Value;
                    }

                    if ((p1.m_form.StartsWith("10")) || (p1.m_form.StartsWith("11")))
                    {
                        c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SDOEmail");
                        if (c2 != null) s2 += ";" + c2.Value;

                        c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_KS4LeaderEmail");
                        if (c2 != null) s2 += ";" + c2.Value;
                    }
                    if ((p1.m_form.StartsWith("12")) || (p1.m_form.StartsWith("13")))
                    {
                        c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_KS5LeaderEmail");
                        if (c2 != null) s2 += ";" + c2.Value;


                    }
                    if (p1.m_form.StartsWith("12"))
                    {
                        c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_Year12LeaderEmail");
                        if (c2 != null) s2 += ";" + c2.Value;

                    }
                    if (p1.m_form.StartsWith("13"))
                    {
                        c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_Year13LeaderEmail");
                        if (c2 != null) s2 += ";" + c2.Value;
                    }
/*

                    if (p1.m_form.StartsWith("7"))
                    {
                        s2 += ";" + (string)ar.GetValue("SDOEmail", s.GetType());
                        s2 += ";" + (string)ar.GetValue("Year7LeaderEmail", s.GetType());
                    }
                    if ((p1.m_form.StartsWith("8")) || (p1.m_form.StartsWith("9")))
                    {
                        s2 += ";" + (string)ar.GetValue("SDOEmail", s.GetType());
                        s2 += ";" + (string)ar.GetValue("KS3LeaderEmail", s.GetType());
                    }
                    if ((p1.m_form.StartsWith("10")) || (p1.m_form.StartsWith("11")))
                    {
                        s2 += ";" + (string)ar.GetValue("SDOEmail", s.GetType());
                        s2 += ";" + (string)ar.GetValue("KS4LeaderEmail", s.GetType());
                    }
                    if ((p1.m_form.StartsWith("12")) || (p1.m_form.StartsWith("13")))
                    {
                        s2 += ";" + (string)ar.GetValue("KS5LeaderEmail", s.GetType());
                    }
                    if (p1.m_form.StartsWith("12"))
                    {
                        s2 += ";" + (string)ar.GetValue("Year12LeaderEmail", s.GetType());
                    }
                    if (p1.m_form.StartsWith("13"))
                    {
                        s2 += ";" + (string)ar.GetValue("Year13LeaderEmail", s.GetType());
                    }
 */
                    s = "This auto-generated email is to inform you that " + staff1.m_PersonGivenName + " " + staff1.m_PersonSurname + " recorded a new incident for ";
                    s += p1.m_GivenName + " " + p1.m_Surname + "(" + p1.m_adno.ToString() + ")" + " on " + DateTime.Now.ToShortDateString();
                    s += "        ";
                    s += " The text for the incident is:    " + Incident.Text + " .           ";
                    if (sanction.Id != Guid.Empty)
                    {
                        s += " A sanction was applied:    ";
                        s += sanct1.Description + " .";
                        staff1 = new SimpleStaff(sanction.StaffId);
                        s += "  Sanction issued by: " + staff1.m_StaffCode;
                    }
                    string monitor_email = "";
                    c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_Monitor_email");
                    if (c2 != null) monitor_email = c2.Value;
                    m1.SendMail(s1, s2, "", s, monitor_email, "New Incident Recorded - sent on behalf of " + s1);
                }
            }
            catch (Exception e)
            {
                string s2 = e.Message;
            }

        }


        private void DropDownListSanctionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //call back to update display and add the sanction stuff
            //and to update count....
            PreviousSanctionList = "";
            if (DropDownListSanctionType.SelectedIndex > 0)
            {
                TextBoxSanctionCount.Text = (SetupPreviousSanctionList() + 1).ToString();
            }
        }
        private int SetupPreviousSanctionList()
        {
            StudentSanctionList ssl1 = new StudentSanctionList();
            int year = DateTime.Now.Year; if (DateTime.Now.Month < 9) year--;
            DateTime d1 = new DateTime(year, 9, 1);
            ssl1.Load(StudentId, d1, DropDownListSanctionType.SelectedIndex);
            PreviousSanctionList = "<br><br>Previous Sanctions since " + d1.ToLongDateString() + "of type: " + DropDownListSanctionType.SelectedItem.Text + "<br>";
            foreach (StudentSanction s in ssl1._sanctionlist)
            {
                PreviousSanctionList += "<br>Date:  " + s.SanctionDate.ToShortDateString() + ".... count :" + s.count.ToString();
            }
            return ssl1.count;
        }
        private string GetQueryStringFormList(string form)
        {
            string s = "SELECT dbo.qry_Cerval_Core_Student.PersonGivenName, dbo.qry_Cerval_Core_Student.PersonSurname, dbo.qry_Cerval_Core_StudentWithFormGroup.GroupCode, ";
            s += "dbo.tbl_Core_Students_Incidents.IncidentDate, dbo.tbl_Core_Students_Incidents.IncidentType, dbo.tbl_Core_Students_Incidents.IncidentText, ";
            s += "dbo.qry_Cerval_Core_StudentWithFormGroup.GroupRegistrationYear, dbo.tbl_Core_Students_Incidents.IncidentID, dbo.tbl_Core_Students_Incidents.StudentID,  ";
            s += "dbo.tbl_Core_Students_Incidents.StudentAdmissionNumber, dbo.tbl_Core_Students_Incidents.IncidentReportingStaffID,  ";
            s += "dbo.tbl_Core_Students_Incidents.IncidentPairs ";
            s += "FROM  dbo.tbl_Core_Students_Incidents INNER JOIN ";
            s += "dbo.qry_Cerval_Core_StudentWithFormGroup ON  ";
            s += "dbo.tbl_Core_Students_Incidents.StudentID = dbo.qry_Cerval_Core_StudentWithFormGroup.StudentId INNER JOIN ";
            s += "dbo.qry_Cerval_Core_Student ON dbo.qry_Cerval_Core_StudentWithFormGroup.StudentId = dbo.qry_Cerval_Core_Student.StudentId ";
            s += "WHERE (dbo.tbl_Core_Students_Incidents.IncidentDate > CONVERT(DATETIME, '2009-01-01 00:00:00', 102)) AND  ";
            s += "(dbo.qry_Cerval_Core_StudentWithFormGroup.GroupCode = '" + form + "') ";
            return s;
        }
        private string GetQueryStringGroupList(string group)
        {
            string s = " SELECT  dbo.tbl_Core_Students_Incidents.IncidentID, dbo.tbl_Core_Students_Incidents.StudentID,  ";
            s += " dbo.tbl_Core_Students_Incidents.StudentAdmissionNumber, dbo.tbl_Core_Students_Incidents.IncidentDate, dbo.tbl_Core_Students_Incidents.IncidentType,   ";
            s += " dbo.tbl_Core_Students_Incidents.IncidentText, dbo.tbl_Core_Students_Incidents.IncidentReportingStaffID, dbo.tbl_Core_Students_Incidents.IncidentPairs,   ";
            s += " dbo.qry_Cerval_Core_Student.PersonTitle, dbo.qry_Cerval_Core_Student.PersonGivenName, dbo.qry_Cerval_Core_Student.PersonSurname,   ";
            s += " dbo.tbl_Core_Groups.GroupCode  ";
            s += " FROM  dbo.tbl_Core_Groups INNER JOIN  ";
            s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN  ";
            s += " dbo.tbl_Core_Students_Incidents INNER JOIN  ";
            s += " dbo.qry_Cerval_Core_Student ON dbo.tbl_Core_Students_Incidents.StudentID = dbo.qry_Cerval_Core_Student.StudentId ON   ";
            s += " dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students_Incidents.StudentID  ";
            s += " WHERE (dbo.tbl_Core_Student_Groups.MemberFrom < { fn NOW() }) AND (dbo.tbl_Core_Student_Groups.MemberUntil > { fn NOW() }) AND   ";
            s += " (dbo.tbl_Core_Groups.GroupValidFrom < { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupValidUntil > { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupCode = '" + group + "')  ";

            return s;
        }
        protected void UpdateGrid()
        {
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
            EditData1.SelectCommand = "SELECT [IncidentID], [IncidentDate], [IncidentText], [StaffCode], [IncidentPairs], [Incident], [SanctionName], [SanctionDate], [SanctionCompleted] FROM [qry_Cerval_Core_student_incident2]  WHERE (StudentAdmissionNumber ='" + pupil1.m_adno.ToString() + "' ) ORDER BY [IncidentDate] DESC ";
            EditGridView1.DataBind();
        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (StudentId != Guid.Empty)
            {
                SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
                if (((!IsNew) && (!IsEdit)))
                {
                    EditGridView1.Visible = true;
                    EditData1.SelectCommand = "SELECT [IncidentID], [IncidentDate], [IncidentText], [StaffCode], [IncidentPairs], [Incident], [SanctionName], [SanctionDate], [SanctionCompleted] FROM [qry_Cerval_Core_student_incident2]  WHERE (StudentAdmissionNumber ='" + pupil1.m_adno.ToString() + "' ) ORDER BY [IncidentDate] DESC";
                    EditGridView1.PageIndex = page_No;
                    output.Write("<p >Incident Log for <b>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</b></p><br>");
                    CancelButton.RenderControl(output);
                    EditGridView1.RenderControl(output);
                }
                if (IsEdit)
                {
                    RenderEditControls("Edit Incident for ", output, pupil1);
                }
                if (IsNew)
                {
                    RenderEditControls("Create New Incident report for ", output, pupil1);
                }
            }
            else
            {
                output.Write("Incidents");
            }
        }
        protected void RenderEditControls(string title, HtmlTextWriter output, SimplePupil pupil1)
        {
            EditGridView1.Visible = false;
            UpdateButton.Visible = IsEdit; SaveButton.Visible = IsNew;
            output.Write("<p > " + title + " <b>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</b></p>");
            output.Write("Incident Date: "); TextBoxIncidentDate.RenderControl(output);
            output.Write("Incident Type: "); DropDownListType.RenderControl(output);
            output.Write("<br>Incident Pairs: "); TextBoxIncidentPairs.RenderControl(output);
            output.Write("<br>Incident Staff: "); DropDownIncidentStaff.RenderControl(output);
            output.Write("<br>Incident Text <br>"); TextBoxIncidentText.RenderControl(output);

            output.Write("<br><br>Sanction: "); DropDownListSanctionType.RenderControl(output);
            if (DropDownListSanctionType.SelectedIndex != 0)//ie not none
            {
                output.Write("Sanction Count: "); TextBoxSanctionCount.RenderControl(output);
                if (IsEdit) { output.Write("Sanction Complete: "); CheckBoxSanctionCompleted.RenderControl(output); }
                output.Write("<br>Sanction Date: "); TextBoxSanctionDate.RenderControl(output);
                output.Write("<br>Sanction Work Set: <br>"); TextBoxSanctionWorkSet.RenderControl(output);
                output.Write("<br>");
            }
            SaveButton.RenderControl(output); UpdateButton.RenderControl(output);
            CancelButton.RenderControl(output);
            if (DropDownListSanctionType.SelectedIndex != 0)
            {
                output.Write(PreviousSanctionList);
            }
        }
        #endregion

        #region PostBackEventHandler
        public virtual void RaisePostBackEvent(string eventArgument)
        {
            string s = eventArgument;
        }
        #endregion
    }

    public class GroupIncidentControl : WebControl
    {

        private System.Web.UI.WebControls.SqlDataSource data1 = new SqlDataSource();
        private System.Web.UI.WebControls.GridView GridView1 = new GridView();
        private System.Web.UI.WebControls.BoundField ColumnID = new BoundField();
        private System.Web.UI.WebControls.BoundField StudentFirstName = new BoundField();
        private System.Web.UI.WebControls.BoundField StudentSurname = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnDate = new BoundField();
        private System.Web.UI.WebControls.BoundField ColumnText = new BoundField();


        [Bindable(true)]
        [Category("Data")]
        [Localizable(true)]
        public string GroupCode
        {
            get { return ((ViewState["GroupCode"] == null) ? "" : (string)ViewState["GroupCode"]); }
            set { ViewState["GroupCode"] = value; page_No = 0; }
        }
        [Bindable(true)]
        [Category("Data")]
        [Localizable(true)]
        public string StaffCode
        {
            get { return ((ViewState["StaffCode"] == null) ? "" : (string)ViewState["StaffCode"]); }
            set { ViewState["StaffCode"] = value; page_No = 0; }
        }

        public int page_No
        {
            get { return (int)ViewState["page_No"]; }
            set { ViewState["page_No"] = value; }
        }


        public GroupIncidentControl()
        {

            data1.ProviderName = System.Configuration.ConfigurationManager.ConnectionStrings["CervalConnectionString"].ProviderName;
            Encode en = new Encode();
            data1.ConnectionString = en.GetCervalConnectionString();


            Controls.Add(data1);
            data1.ID = "data1";
            Controls.Add(GridView1);
            SetupGrid();
            page_No = GridView1.PageIndex;
            GridView1.PageIndexChanged += new EventHandler(GridView1_PageIndexChanged);

        }
        private void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            page_No = GridView1.PageIndex;//needed to preserve the page no between calls.
        }
        private void SetupGrid()
        {
            GridView1.ID = "GridView1"; GridView1.AllowSorting = true; GridView1.AllowPaging = true; ;
            GridView1.HeaderStyle.Font.Size = FontUnit.XSmall; GridView1.AutoGenerateColumns = false;
            GridView1.BackColor = System.Drawing.Color.White; GridView1.BorderStyle = BorderStyle.None;
            GridView1.BorderColor = System.Drawing.Color.FromArgb(003399); GridView1.BorderWidth = 1;
            GridView1.CellPadding = 4; //GridView1.DataKeyNames = "IncidentID";
            GridView1.DataSourceID = "data1"; GridView1.Width = 610;
            GridView1.PageSize = 20;

            GridView1.Columns.Add(ColumnID);
            ColumnID.DataField = "dbo.tbl_Core_Students_Incidents.IncidentID"; ColumnID.HeaderText = "Id"; ColumnID.ReadOnly = true; ColumnID.SortExpression = "dbo.tbl_Core_Students_Incidents.IncidentID";
            ColumnID.ItemStyle.Font.Size = FontUnit.XSmall; ColumnID.ItemStyle.ForeColor = System.Drawing.Color.Gray;
            ColumnID.ItemStyle.Width = 20; ColumnID.ItemStyle.Wrap = true;
            ColumnID.Visible = false;



            GridView1.Columns.Add(StudentFirstName);
            StudentFirstName.DataField = "_GivenName"; StudentFirstName.HeaderText = "FirstName"; StudentFirstName.ReadOnly = true; StudentFirstName.SortExpression = "_GivenName";
            StudentFirstName.ItemStyle.Font.Size = FontUnit.XSmall; StudentFirstName.ItemStyle.ForeColor = System.Drawing.Color.Black;
            StudentFirstName.ItemStyle.Width = 50; StudentFirstName.ItemStyle.Wrap = true;

            GridView1.Columns.Add(StudentSurname);
            StudentSurname.DataField = "_Surname"; StudentSurname.HeaderText = "Surname"; StudentSurname.ReadOnly = true; StudentSurname.SortExpression = "_Surname";
            StudentSurname.ItemStyle.Font.Size = FontUnit.XSmall; StudentSurname.ItemStyle.ForeColor = System.Drawing.Color.Black;
            StudentSurname.ItemStyle.Width = 30; StudentSurname.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnDate);
            ColumnDate.DataField = "_date"; ColumnDate.HeaderText = "Date"; ColumnDate.ReadOnly = true; ColumnDate.SortExpression = "_date";
            ColumnDate.ItemStyle.Font.Size = FontUnit.XSmall; ColumnDate.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnDate.ItemStyle.Width = 40; ColumnDate.ItemStyle.Wrap = true;

            GridView1.Columns.Add(ColumnText);
            ColumnText.DataField = "_text"; ColumnText.HeaderText = "Description"; ColumnText.ReadOnly = true; ColumnText.SortExpression = "_text";
            ColumnText.ItemStyle.Font.Size = FontUnit.XSmall; ColumnText.ItemStyle.ForeColor = System.Drawing.Color.Black;
            ColumnText.ItemStyle.Width = 400; ColumnText.ItemStyle.Wrap = true;


        }


        void DropDownListSanctionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private string GetQueryStringFormList(string form)
        {
            string s = "SELECT dbo.qry_Cerval_Core_Student.PersonGivenName, dbo.qry_Cerval_Core_Student.PersonSurname, dbo.qry_Cerval_Core_StudentWithFormGroup.GroupCode, ";
            s += "dbo.tbl_Core_Students_Incidents.IncidentDate, dbo.tbl_Core_Students_Incidents.IncidentType, dbo.tbl_Core_Students_Incidents.IncidentText, ";
            s += "dbo.qry_Cerval_Core_StudentWithFormGroup.GroupRegistrationYear, dbo.tbl_Core_Students_Incidents.IncidentID, dbo.tbl_Core_Students_Incidents.StudentID,  ";
            s += "dbo.tbl_Core_Students_Incidents.StudentAdmissionNumber, dbo.tbl_Core_Students_Incidents.IncidentReportingStaffID,  ";
            s += "dbo.tbl_Core_Students_Incidents.IncidentPairs ";
            s += "FROM  dbo.tbl_Core_Students_Incidents INNER JOIN ";
            s += "dbo.qry_Cerval_Core_StudentWithFormGroup ON  ";
            s += "dbo.tbl_Core_Students_Incidents.StudentID = dbo.qry_Cerval_Core_StudentWithFormGroup.StudentId INNER JOIN ";
            s += "dbo.qry_Cerval_Core_Student ON dbo.qry_Cerval_Core_StudentWithFormGroup.StudentId = dbo.qry_Cerval_Core_Student.StudentId ";
            s += "WHERE (dbo.tbl_Core_Students_Incidents.IncidentDate > CONVERT(DATETIME, '2009-01-01 00:00:00', 102)) AND  ";
            s += "(dbo.qry_Cerval_Core_StudentWithFormGroup.GroupCode = '" + form + "') ";
            return s;
        }

        private string GetQueryStringGroupList(string group, DateTime FirstIncidentDate)
        {
            string date_s = "CONVERT(DATETIME, '" + FirstIncidentDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT  dbo.tbl_Core_Students_Incidents.IncidentID, dbo.tbl_Core_Students_Incidents.StudentID,  ";
            s += " dbo.tbl_Core_Students_Incidents.StudentAdmissionNumber, dbo.tbl_Core_Students_Incidents.IncidentDate AS _date, dbo.tbl_Core_Students_Incidents.IncidentType,   ";
            s += " dbo.tbl_Core_Students_Incidents.IncidentText AS _text, dbo.tbl_Core_Students_Incidents.IncidentReportingStaffID, dbo.tbl_Core_Students_Incidents.IncidentPairs,   ";
            s += " dbo.qry_Cerval_Core_Student.PersonTitle, dbo.qry_Cerval_Core_Student.PersonGivenName AS _GivenName, dbo.qry_Cerval_Core_Student.PersonSurname AS _Surname,   ";
            s += " dbo.tbl_Core_Groups.GroupCode  ";
            s += " FROM  dbo.tbl_Core_Groups INNER JOIN  ";
            s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN  ";
            s += " dbo.tbl_Core_Students_Incidents INNER JOIN  ";
            s += " dbo.qry_Cerval_Core_Student ON dbo.tbl_Core_Students_Incidents.StudentID = dbo.qry_Cerval_Core_Student.StudentId ON   ";
            s += " dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students_Incidents.StudentID  ";
            s += " WHERE (dbo.tbl_Core_Student_Groups.MemberFrom < { fn NOW() }) AND (dbo.tbl_Core_Student_Groups.MemberUntil > { fn NOW() }) AND   ";
            s += " (dbo.tbl_Core_Groups.GroupValidFrom < { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupValidUntil > { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupCode = '" + group + "')  ";
            s += " AND ( dbo.tbl_Core_Students_Incidents.IncidentDate >" + date_s + " ) ";
            return s;
        }

        private string GetQueryStringStaff(string staffid, DateTime FirstIncidentDate)
        {
            string date_s = "CONVERT(DATETIME, '" + FirstIncidentDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT  dbo.tbl_Core_Students_Incidents.IncidentDate AS _date, dbo.tbl_Core_Students_Incidents.IncidentType,   ";
            s += " dbo.tbl_Core_Students_Incidents.IncidentText AS _text, dbo.tbl_Core_Students_Incidents.IncidentReportingStaffID,   ";
            s += "  dbo.qry_Cerval_Core_Student.PersonGivenName AS _GivenName, dbo.qry_Cerval_Core_Student.PersonSurname AS _Surname   ";

            s += " FROM  dbo.qry_Cerval_Core_Student INNER JOIN  ";
            s += " dbo.tbl_Core_Students_Incidents ";
            s += " ON dbo.tbl_Core_Students_Incidents.StudentID = dbo.qry_Cerval_Core_Student.StudentId ";
            s += " WHERE (dbo.tbl_Core_Students_Incidents.IncidentReportingStaffID = '" + staffid+ "')  ";
            s += " AND ( dbo.tbl_Core_Students_Incidents.IncidentDate >" + date_s + " ) ";
            return s;
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (GroupCode != "")
            {
                GridView1.Visible = true;
                data1.SelectCommand = GetQueryStringGroupList(GroupCode, DateTime.Now.AddYears(-1));
                GridView1.PageIndex = page_No;
                output.Write("<p >Incident Log for <b>" + GroupCode + "  </b> for previous year.</p>");
                GridView1.RenderControl(output);
            }
            else
            {
                if (StaffCode != "")
                {
                    GridView1.Visible = true;
                    data1.SelectCommand = GetQueryStringStaff(StaffCode, DateTime.Now.AddYears(-1));
                    GridView1.PageIndex = page_No;
                    SimpleStaff s1 = new SimpleStaff(new Guid(StaffCode));
                    output.Write("<p >Incident Log Entries authored by <b>" + s1.m_Title+" "+s1.m_PersonGivenName+" "+s1.m_PersonSurname + "  </b> for past year.</p>");
                    GridView1.RenderControl(output);
                }
                else
                {
                    output.Write("Incidents");
                }
            }
        }
    }

    [ToolboxData("<{0}:GroupListControl runat=server></{0}:GroupListControl>")]
    public class GroupListControl : WebControl
    {
        Button CSV_Button = new Button();
        public ArrayList array1 = new ArrayList();
        int max_x = 0; int max_y = 0;
        public Group Current_Group = new Group();
        public DateTime ListDate = new DateTime();
        public bool PhotoOnly
        {
            get { return (bool)ViewState["PhotoOnly"]; }
            set { ViewState["PhotoOnly"] = value; }
        }
        public bool FullAddressList
        {
            get { return (bool)ViewState["FullAddressList"]; }
            set { ViewState["FullAddressList"] = value; }
        }
        public bool IncidentOnly
        {
            get { return (bool)ViewState["IncidentOnly"]; }
            set { ViewState["IncidentOnly"] = value; }
        }
        public System.Collections.Generic.List<Cerval_Library.Listitem> Groups = new System.Collections.Generic.List<Cerval_Library.Listitem>();
        [Serializable]
        public class DisplayItem
        {
            public enum Fields
            {
                none, Adno, Surname, GivenName, DoB, Form, Year, MiddleName,
                Upn, ExamNo, Details, SEN, Photo, Address, UCI,
                Language, Ethnicity, Email, PhoneNo, Mobile,
                Medical, Doctor, AllContacts, CanType, XtraTime,
                ParentEmail, Target, InformalName, PP, GoogleAppsLogin, FullName, FullNameR,Gender
            }
            //needed for target grade;;;
            public Guid CourseId;
            public ValueAddedEquation ValueAddedEqn;
            public ValueAddedMethod ValueAddedMth;
            public ResultsList rl1 = new ResultsList();
            public bool _display;
            public Fields _name;
            public bool _simple;//true if no more db access required       
            public DisplayItem(bool display, Fields name)
            { _display = display; _name = name; if (name.CompareTo(Fields.Address) < 0) { _simple = true; } }
            public string GetValue(SimplePupil p, PupilDetails pupil1)
            {
                StudentMedical sdm = new StudentMedical(); string s1 = "";
                bool can_type = false; int extra_time = 0;
                if ((_name == Fields.Doctor) || (_name == Fields.Medical)) sdm.Load(p.m_StudentId.ToString());
                if ((!_simple) && (pupil1 == null)) pupil1.Load(p.m_StudentId.ToString());
                if (_name == Fields.GivenName) return p.m_GivenName;
                if (_name == Fields.MiddleName) return p.m_MiddleName;
                if (_name == Fields.InformalName) return p.m_InformalName;
                if (_name == Fields.PP) return p.m_InReceiptPupilPremium.ToString();
                if (_name == Fields.GoogleAppsLogin) return p.m_GoogleAppsLogin;
                if (_name == Fields.Surname) return p.m_Surname;
                if (_name == Fields.Adno) return p.m_adno.ToString();
                if (_name == Fields.DoB) return p.m_dob.ToShortDateString();
                if (_name == Fields.Form) return p.m_form;
                if (_name == Fields.Year) return p.m_year.ToString();
                if (_name == Fields.Upn) return p.m_upn;
                if (_name == Fields.FullName)
                {
                    if (String.IsNullOrEmpty(p.m_InformalName) || String.IsNullOrEmpty(p.m_InformalName.Trim()))
                        return p.m_GivenName + " " + p.m_Surname; else return p.m_InformalName + " " + p.m_Surname;
                };
                


                if (_name == Fields.FullNameR)
                {
                    if (String.IsNullOrEmpty(p.m_InformalName) || String.IsNullOrEmpty(p.m_InformalName.Trim()))
                        return p.m_Surname + ", " + p.m_GivenName; else return p.m_Surname + ", " + p.m_InformalName;
                };
                if (_name == Fields.Gender) return pupil1.m_Gender;
                if (_name == Fields.ExamNo) return p.m_exam_no.ToString();
                if (_name == Fields.Details) return "<A HREF=\"PupilChoice.aspx?Type=Student&Id=" + p.m_StudentId.ToString() + "&Name=" + p.m_GivenName + "  " + p.m_Surname + "\">Details</A>";

                if (_name == Fields.Photo) return "<img src=\"" + "PersonImagePage.aspx?id=" + p.m_PersonId.ToString() + "&w=110&h=140\" width=\"100\" height=\"130\">";
                if (_name == Fields.Address) return pupil1.m_address;
                if (_name == Fields.UCI) return pupil1.m_UCI;
                if (_name == Fields.Language) return pupil1.m_language;
                if (_name == Fields.Ethnicity) return pupil1.m_ethnicity;
                if (_name == Fields.PhoneNo) { foreach (Contact c in pupil1.m_contacts.m_contactlist) { if (c.m_ContactType == "Home Phone")return (c.m_Contact_Value); };return ""; }
                if (_name == Fields.Email) { foreach (Contact c in pupil1.m_contacts.m_contactlist) { if (c.m_ContactType == "Email")return (c.m_Contact_Value); };return ""; }
                if (_name == Fields.Mobile) { foreach (Contact c in pupil1.m_contacts.m_contactlist) { if (c.m_ContactType == "Mobile Phone")return (c.m_Contact_Value); };return ""; }
                if (_name == Fields.Medical) return sdm.m_MedicalNotes;
                if (_name == Fields.Doctor) return sdm.m_doctor.m_Title + " " + sdm.m_doctor.m_PersonGivenName + " " + sdm.m_doctor.m_PersonSurname + " : " + sdm.m_doctorPhoneNo;
                if (_name == Fields.AllContacts) { s1 = ""; foreach (Relationship r in pupil1.m_relationships.m_Relationshiplist) { s1 += "<b>" + r.m_RelationshipDesc + "</b>"; foreach (Contact c in r.m_contactlist.m_contactlist) { s1 += c.m_ContactType + ": " + c.m_Contact_Value + " "; } s1 += "<br>"; } return s1; }

                if (_name == Fields.SEN) return GetSEN(p.m_StudentId, out can_type, out extra_time);
                if (_name == Fields.InformalName) return p.m_InformalName;
                if (_name == Fields.CanType)
                
                {
                    GetSEN(p.m_StudentId, out can_type, out extra_time);
                    if (can_type) return ("Can Type"); else return ("");
                }
                if (_name == Fields.XtraTime)
                {
                    GetSEN(p.m_StudentId, out can_type, out extra_time);
                    if (extra_time > 0) return (extra_time.ToString() + "%"); else return ("");
                }
                if (_name == Fields.ParentEmail)
                {
                    foreach (Relationship r in pupil1.m_relationships.m_Relationshiplist)
                    {
                        //these are ordered by relationship priority ... so take first one...
                        foreach (Contact c in r.m_contactlist.m_contactlist)
                        {
                            if (c.m_ContactType == "Email")
                            {
                                return (c.m_Contact_Value);
                            }
                        }
                    }
                }
                if (_name == Fields.Target)
                {
                    Double tg = 0; Double vaBase = 0; Double tgNew = 0;
                    StudentSubjectProfile ssp1 = new StudentSubjectProfile();
                    if (ValueAddedEqn == null) return "NA";
                    //need base value...
                    //rl1.m_parameters = 1;rl1.LoadList("dbo.tbl_Core_Students.StudentId", p.m_StudentId.ToString());
                    rl1.LoadListSimple(@"WHERE (StudentId = '" + p.m_StudentId.ToString() + "')  AND  (ResultType = '" + ValueAddedMth.m_ValueAddedBaseResultType.ToString() + "' ) ");
                    foreach (Result r in rl1._results)
                    {
                        if (r.Resulttype == ValueAddedMth.m_ValueAddedBaseResultType)
                        {
                            vaBase = System.Convert.ToDouble(r.Value);
                            tg = Round1(ValueAddedEqn.m_coef0 + ValueAddedEqn.m_coef1 * vaBase + ValueAddedEqn.m_coef2 * vaBase * vaBase);
                            ssp1.VA_ResultType = ValueAddedMth.m_ValueAddedOutputResultType;
                            ssp1.Score_to_String(tg);
                        }
                    }
                    if (tg == 0) return "NA";
                    return tg.ToString() + "(" + ssp1.Score_to_String(tg) + ")";
                }
                return "";
            }
            public double Round1(double x)
            {
                int i = (int)(x * 10);
                return (double)i / 10;
            }
            private string GetSEN(Guid StudentId, out bool Can_Type, out int Extra_Time)
            {
                string s1 = ""; Can_Type = false; Extra_Time = 0;
                StudentSENList ss1 = new StudentSENList(StudentId.ToString());
                foreach (StudentSEN s in ss1.m_List)
                {
                    
                    if (s.m_SenDescription.Contains(@"https://docs.google.com"))
                    {
                        int i1 = s.m_SenDescription.IndexOf(@"https:");
                        s1 += s.m_SenDescription.Substring(0, i1 - 1);
                        s1 += " <A HREF=\"" + s.m_SenDescription.Substring(i1) + "\">  Details </A> .......:    ";
                    }
                    else
                    {
                        s1 += s.m_SenDescription + ": ";
                    }


                    if (s.m_ExamsCanType) Can_Type = true;
                    if (s.m_ExamsExtraTime > Extra_Time) Extra_Time = s.m_ExamsExtraTime;
                }
                return s1;
            }
        }
        public System.Collections.Generic.List<DisplayItem> DisplayItems = new System.Collections.Generic.List<DisplayItem>();
        public GroupListControl()
        {
            PhotoOnly = false; FullAddressList = false; IncidentOnly = false;
            DisplayItem d0 = new DisplayItem(true, DisplayItem.Fields.Adno); DisplayItems.Add(d0);
            DisplayItem d1 = new DisplayItem(true, DisplayItem.Fields.GivenName); DisplayItems.Add(d1);
            DisplayItem d2 = new DisplayItem(false, DisplayItem.Fields.MiddleName); DisplayItems.Add(d2);
            DisplayItem d3 = new DisplayItem(true, DisplayItem.Fields.Surname); DisplayItems.Add(d3);
            //DisplayItem d6 = new DisplayItem(false, DisplayItem.Fields.Year); DisplayItems.Add(d6);
            DisplayItem d12 = new DisplayItem(false, DisplayItem.Fields.Address); DisplayItems.Add(d12);
            DisplayItem d24 = new DisplayItem(false, DisplayItem.Fields.ParentEmail); DisplayItems.Add(d24);
            DisplayItem d14 = new DisplayItem(false, DisplayItem.Fields.Language); DisplayItems.Add(d14);
            DisplayItem d15 = new DisplayItem(false, DisplayItem.Fields.Ethnicity); DisplayItems.Add(d15);
            DisplayItem d16 = new DisplayItem(false, DisplayItem.Fields.PhoneNo); DisplayItems.Add(d16);
            DisplayItem d17 = new DisplayItem(false, DisplayItem.Fields.Mobile); DisplayItems.Add(d17);
            DisplayItem d9 = new DisplayItem(false, DisplayItem.Fields.Details); DisplayItems.Add(d9);
            DisplayItem d4 = new DisplayItem(false, DisplayItem.Fields.DoB); DisplayItems.Add(d4);
            DisplayItem d5 = new DisplayItem(false, DisplayItem.Fields.Form); DisplayItems.Add(d5);
            DisplayItem d7 = new DisplayItem(false, DisplayItem.Fields.Upn); DisplayItems.Add(d7);
            DisplayItem d8 = new DisplayItem(false, DisplayItem.Fields.ExamNo); DisplayItems.Add(d8);
            DisplayItem d10 = new DisplayItem(false, DisplayItem.Fields.SEN); DisplayItems.Add(d10);
            DisplayItem d11 = new DisplayItem(false, DisplayItem.Fields.Photo); DisplayItems.Add(d11);
            DisplayItem d13 = new DisplayItem(false, DisplayItem.Fields.UCI); DisplayItems.Add(d13);
            DisplayItem d18 = new DisplayItem(false, DisplayItem.Fields.Email); DisplayItems.Add(d18);
            DisplayItem d19 = new DisplayItem(false, DisplayItem.Fields.Medical); DisplayItems.Add(d19);
            DisplayItem d20 = new DisplayItem(false, DisplayItem.Fields.Doctor); DisplayItems.Add(d20);
            DisplayItem d21 = new DisplayItem(false, DisplayItem.Fields.AllContacts); DisplayItems.Add(d21);
            DisplayItem d22 = new DisplayItem(false, DisplayItem.Fields.CanType); DisplayItems.Add(d22);
            DisplayItem d23 = new DisplayItem(false, DisplayItem.Fields.XtraTime); DisplayItems.Add(d23);
            DisplayItem d25 = new DisplayItem(false, DisplayItem.Fields.Target); DisplayItems.Add(d25);
            DisplayItem d27 = new DisplayItem(false, DisplayItem.Fields.InformalName); DisplayItems.Add(d27);
            DisplayItem d28 = new DisplayItem(false, DisplayItem.Fields.PP); DisplayItems.Add(d28);
            DisplayItem d29 = new DisplayItem(false, DisplayItem.Fields.GoogleAppsLogin); DisplayItems.Add(d29);
            DisplayItem d30 = new DisplayItem(false, DisplayItem.Fields.FullName); DisplayItems.Add(d30);
            DisplayItem d31 = new DisplayItem(false, DisplayItem.Fields.FullNameR); DisplayItems.Add(d31);
            DisplayItem d32 = new DisplayItem(false, DisplayItem.Fields.Gender); DisplayItems.Add(d32);

            ListDate = DateTime.Now;
            save();
            Controls.Add(CSV_Button); CSV_Button.Text = "Download as Spreadsheet (csv)";
            CSV_Button.Visible = true;
            CSV_Button.Click += new EventHandler(CSV_Button_Click);
        }
        void CSV_Button_Click(object sender, EventArgs e)
        {
            GenerateArray();
            CSVWriter exl1 = new CSVWriter();
            string s = "";
            foreach (Cerval_Library.Listitem l in Groups)
            {
                s += l.m_text.Trim() + "_";
            }
                exl1.OutputToCSV(Page.Response, array1, max_x, max_y, s+ListDate.ToShortDateString());
        }
        public void save()
        {
            ViewState.Add("Groups", Groups);
            ViewState.Add("DisplayList", DisplayItems);
            ViewState.Add("ListDate", ListDate);
        }
        public void GenerateArray()
        {
            DisplayItems = (System.Collections.Generic.List<DisplayItem>)ViewState["DisplayList"];
            Groups = (System.Collections.Generic.List<Cerval_Library.Listitem>)ViewState["Groups"];
            int total = 0; array1.Clear(); max_x = 0; max_y = 0;
            int x = 0; int y = 0;
            ListDate = (DateTime)ViewState["ListDate"];
            bool display_contain_detail = false;
            PupilDetails pupil1 = new PupilDetails();
            foreach (Cerval_Library.Listitem l in Groups)
            {
                PupilGroupList pgl1 = new PupilGroupList();
                pgl1.AddToList(l.m_value, ListDate);
                Grid_Element g0 = new Grid_Element(x, y, l.m_text); y++; array1.Add(g0);
                foreach (DisplayItem d in DisplayItems)
                {
                    if (d._name == DisplayItem.Fields.Photo) d._display = false;//don't output photos...
                    if (d._display) { Grid_Element g1 = new Grid_Element(x, y, d._name.ToString()); array1.Add(g1); x++; }
                    if (!d._simple) display_contain_detail = true;
                    if (max_x < x) max_x = x;
                }
                y++; total = 0;
                foreach (SimplePupil p in pgl1.m_pupilllist)
                {
                    x = 0;
                    foreach (DisplayItem d in DisplayItems)
                    {
                        if (d._display)
                        {
                            if (display_contain_detail) pupil1.Load(p.m_StudentId.ToString()); else pupil1 = null;
                            Grid_Element gn = new Grid_Element(x, y, d.GetValue(p, pupil1)); array1.Add(gn); x++;
                        }
                    }
                    y++; total++;
                }
                x = 0;
                Grid_Element g2 = new Grid_Element(x, y, "Total for " + l.m_text + " : " + total.ToString());
                array1.Add(g2); y++;
                max_y = y;
            }
        }
        public string GenerateJson()
        {
            string s = "";
            DisplayItems = (System.Collections.Generic.List<DisplayItem>)ViewState["DisplayList"];
            Groups = (System.Collections.Generic.List<Cerval_Library.Listitem>)ViewState["Groups"];
            int total = 0; array1.Clear(); max_x = 0; max_y = 0;
            int x = 0; int y = 0;
            ListDate = (DateTime)ViewState["ListDate"];
            bool display_contain_detail = false;
            PupilDetails pupil1 = new PupilDetails();

            //these needed for value added stuff...
            string VA_grades = "";
            ValueAddedMethodList vaml1 = new ValueAddedMethodList();
            ValueAddedMethod vam = new ValueAddedMethod();
            ValueAddedConversionList vacl1 = new ValueAddedConversionList();
            ValueAddedEquation va1 = new ValueAddedEquation();
            bool DisplayVA = false;


            DateTime CourseStartDate;
            s = "{" + Environment.NewLine + "\"List\" :";
            foreach (Cerval_Library.Listitem l in Groups)
            {
                PupilGroupList pgl1 = new PupilGroupList();
                pgl1.AddToList(l.m_value, ListDate);

                foreach (DisplayItem d in DisplayItems)
                {
                    if (d._name == DisplayItem.Fields.Target) DisplayVA = d._display;
                }
                if (DisplayVA)
                {
                    Current_Group.Load(l.m_value);
                    CourseStartDate = Current_Group._StartDate;
                    SimplePupil p1 = (SimplePupil)pgl1.m_pupilllist[0];

                    CourseStartDate = new DateTime(DateTime.Now.Year, 9, 2);
                    if (DateTime.Now.Month < 9) CourseStartDate = CourseStartDate.AddYears(-1);
                    if (p1.m_year == 11) CourseStartDate = CourseStartDate.AddYears(-1);
                    if (p1.m_year == 13) CourseStartDate = CourseStartDate.AddYears(-1);

                    vam = vaml1.FindVAMethod(p1.m_StudentId, Current_Group._CourseID, CourseStartDate );

                    va1.Load1("WHERE (ValueAddedMethodID='" + vam.m_ValueAddedMethodID.ToString() + "') AND ( CourseID='" + Current_Group._CourseID.ToString() + "' ) ");
                    //now have the va equation.....
                    //going to form the text conversion string for values....
                    foreach (ValueAddedConversion va in vacl1.m_ValueAddedConversionList)
                    {
                        if (va.m_ResultType == vam.m_ValueAddedOutputResultType)
                        {
                            if ((va.m_ResultValue.Trim() != "+") && (va.m_ResultValue.Trim() != "*"))
                            {
                                VA_grades += va.m_ResultValue.Trim() + "=" + va.m_ResultNumericValue.ToString() + " : ";
                            }
                        }
                    }
                    VA_grades += "<br/> Alis/Yellis gives grade predictions based on a range, so A prediction at A-level from scores 110 - 130, and A prediction at GCSE from 49-55.";

                }





                s += "{ " + Environment.NewLine + "\"Group\": \"" + l.m_text + "\"," + Environment.NewLine;
                s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + Environment.NewLine;
                s+="\"Students\": ["+Environment.NewLine;


                foreach (DisplayItem d in DisplayItems)
                {
                    if (d._name == DisplayItem.Fields.Photo) d._display = false;//don't output photos...
                    if (d._display) { x++; }
                    if (!d._simple) display_contain_detail = true;
                    if (max_x < x) max_x = x;
                }
                y++; total = 0;
                foreach (SimplePupil p in pgl1.m_pupilllist)
                {
                    x = 0;
                    s += " {" + Environment.NewLine;


                    foreach (DisplayItem d in DisplayItems)
                    {
                        if (d._display)
                        {
                            if (display_contain_detail) pupil1.Load(p.m_StudentId.ToString()); else pupil1 = null;
                            d.CourseId = Current_Group._CourseID;//need for predicted grade
                            d.ValueAddedEqn = va1;
                            d.ValueAddedMth = vam;
                            s += "\"" + d._name + "\": \"" + d.GetValue(p, pupil1).Trim() + "\"," + Environment.NewLine;
                        }
                    }
                    s += "}," + Environment.NewLine;
                }
                s += "] " + Environment.NewLine + "} " + Environment.NewLine;
            }
            s+="}" + Environment.NewLine;
            return s;
        }
        public string GenerateJson2()
        {
            if (PhotoOnly) { return""; }
            if (FullAddressList) {  return""; }
            Groups = (System.Collections.Generic.List<Cerval_Library.Listitem>)ViewState["Groups"];
            ListDate = (DateTime)ViewState["ListDate"];
            int total = 0; string s = "";
            bool display_contain_detail = false;
            string VA_grades = "";
            PupilDetails pupil1 = new PupilDetails();

            //these needed for value added stuff...
            ValueAddedMethodList vaml1 = new ValueAddedMethodList();
            ValueAddedMethod vam = new ValueAddedMethod();
            ValueAddedConversionList vacl1 = new ValueAddedConversionList();
            ValueAddedEquation va1 = new ValueAddedEquation();
            bool DisplayVA = false;
            DateTime CourseStartDate;
            s = "{" + Environment.NewLine + "\"List\" :";
            foreach (Cerval_Library.Listitem l in Groups)
            {
                
                PupilGroupList pgl1 = new PupilGroupList();
                pgl1.AddToList(l.m_value, ListDate); DisplayVA = false;
                foreach (DisplayItem d in DisplayItems)
                {
                    if (d._name == DisplayItem.Fields.Target) DisplayVA = d._display;
                }

                if (DisplayVA)
                {

                    Current_Group.Load(l.m_value);
                    CourseStartDate = Current_Group._StartDate;

                    SimplePupil p1 = (SimplePupil)pgl1.m_pupilllist[0];

                    CourseStartDate = new DateTime(DateTime.Now.Year, 9, 2);
                    if (DateTime.Now.Month < 9) CourseStartDate = CourseStartDate.AddYears(-1);
                    if (p1.m_year == 11) CourseStartDate = CourseStartDate.AddYears(-1);
                    if (p1.m_year == 13) CourseStartDate = CourseStartDate.AddYears(-1);

                    vam = vaml1.FindVAMethod(p1.m_StudentId, Current_Group._CourseID, CourseStartDate);
                    va1.Load1("WHERE (ValueAddedMethodID='" + vam.m_ValueAddedMethodID.ToString() + "') AND ( CourseID='" + Current_Group._CourseID.ToString() + "' ) ");
                    //now have the va equation.....
                    //going to form the text conversion string for values....
                    foreach (ValueAddedConversion va in vacl1.m_ValueAddedConversionList)
                    {
                        if (va.m_ResultType == vam.m_ValueAddedOutputResultType)
                        {
                            if ((va.m_ResultValue.Trim() != "+") && (va.m_ResultValue.Trim() != "*"))
                            {
                                VA_grades += va.m_ResultValue.Trim() + "=" + va.m_ResultNumericValue.ToString() + " : ";
                            }
                        }
                    }
                    VA_grades += "<br/> Alis/Yellis gives grade predictions based on a range, so A prediction at A-level from scores 110 - 130, and A prediction at GCSE from 49-55.";

                }

                s += "{ " + Environment.NewLine + "\"Group\": \"" + l.m_text + "\"," + Environment.NewLine;
                s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + Environment.NewLine;
                s += "\"Students\": [" + Environment.NewLine;


                foreach (DisplayItem d in DisplayItems)
                {

                    if (!d._simple) display_contain_detail = true;
                }



                foreach (SimplePupil p in pgl1.m_pupilllist)
                {
                    s += " {" + Environment.NewLine;

                    if (display_contain_detail) pupil1.Load(p.m_StudentId.ToString()); else pupil1 = null;

                    foreach (DisplayItem d in DisplayItems)
                    {
                        if (d._display)
                        {
                            d.CourseId = Current_Group._CourseID;//need for predicted grade
                            d.ValueAddedEqn = va1;
                            d.ValueAddedMth = vam;
                            s += "\"" + d._name + "\": \"" + d.GetValue(p, pupil1).Trim() + "\"," + Environment.NewLine;

                        }
                    }
                    s += "}," + Environment.NewLine;
                }
                s += "] " + Environment.NewLine + "} " + Environment.NewLine;

            }
            s += "}" + Environment.NewLine;
            //s += VA_grades;
            return s;
        }
        private void RenderAddressList(HtmlTextWriter output)
        {
            Groups = (System.Collections.Generic.List<Cerval_Library.Listitem>)ViewState["Groups"];
            ListDate = (DateTime)ViewState["ListDate"];
            foreach (Cerval_Library.Listitem l in Groups)
            {
                output.Write("<b>" + l.m_text + "</b>");
                PupilGroupList pgl1 = new PupilGroupList();
                pgl1.AddToList(l.m_value, ListDate);
                PupilDetails pupil1 = new PupilDetails();
                output.Write("<p  align=\"center\"><TABLE BORDER>");
                string[] slist = new string[20];
                foreach (SimplePupil sp in pgl1.m_pupilllist)
                {
                    pupil1.Load(sp.m_StudentId.ToString());
                    output.Write("<tr><td>" + sp.m_GivenName + "</td><td>" + sp.m_Surname + "</td><td>" + sp.m_adno.ToString() + "</td>");
                    output.Write("<td>" + sp.m_form + "</td>");
                    for (int i = 2; i < 10; i++)
                    {
                        output.Write("<td>" + pupil1.m_address_elements[i] + "</td>");
                    }
                    output.Write("</tr>");
                }

                output.Write("</TABLE>");
            }

        }
        private void RenderPhotos(HtmlTextWriter output)
        {
            Groups = (System.Collections.Generic.List<Cerval_Library.Listitem>)ViewState["Groups"];
            ListDate = (DateTime)ViewState["ListDate"];
            foreach (Cerval_Library.Listitem l in Groups)
            {
                output.Write("<b>" + l.m_text + "</b>");
                PupilGroupList pgl1 = new PupilGroupList();
                pgl1.AddToList(l.m_value, ListDate);
                //layout with 5 across page..?
                //max is about 600 across and 800 down....
                int x = 0; string s = "";
                foreach (SimplePupil sp in pgl1.m_pupilllist) x++;
                int width = 100;
                int height = 130;
                int z_max = 6;
                //is we exceed 36.. problems.. so
                //if (x > 36)
                //{
                //   x = (int)Math.Sqrt(480000 / (1.2 * x));
                //  width = x; height = (int)((double)x * 1.3); z_max = (int)(600 / (double)width);
                // }
                if (z_max > 19) z_max = 19;
                output.Write("<p  align=\"center\"><TABLE BORDER>");
                x = 0;

                string[] slist = new string[20];
                int z = 0;
                foreach (SimplePupil sp in pgl1.m_pupilllist)
                {
                    slist[z] = sp.m_GivenName + " " + sp.m_Surname;
                    s = "PersonImagePage.aspx?id=" + sp.m_PersonId.ToString() + "&w=110&h=140";
                    output.Write("<TD><img src=\"" + s + "\" width=\"" + width.ToString() + "\" height=\"" + height.ToString() + "\"></TD>");
                    z++;
                    if (z == z_max)
                    {
                        output.Write("</TR><TR>");
                        for (int j = 0; j < z_max; j++)
                        {
                            output.Write("<TD>" + slist[j] + "</TD>");
                        }
                        output.Write("</TR>"); z = 0;
                    }
                }
                output.Write("</TR>");
                if (z != 0)
                {
                    output.Write("<TR>");
                    for (int j = 0; j < z; j++)
                    {
                        output.Write("<TD>" + slist[j] + "</TD>");
                    }
                    output.Write("</TR>"); z = 0;
                }
                output.Write("</TABLE>");
            }

        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (PhotoOnly) { RenderPhotos(output); return; }
            if (FullAddressList) { RenderAddressList(output); return; }
            Groups = (System.Collections.Generic.List<Cerval_Library.Listitem>)ViewState["Groups"];
            ListDate = (DateTime)ViewState["ListDate"];
            int total = 0; string s = "";
            bool display_contain_detail = false;
            string VA_grades = "";
            PupilDetails pupil1 = new PupilDetails();

            //these needed for value added stuff...
            ValueAddedMethodList vaml1 = new ValueAddedMethodList();
            ValueAddedMethod vam = new ValueAddedMethod();
            ValueAddedConversionList vacl1 = new ValueAddedConversionList();
            ValueAddedEquation va1 = new ValueAddedEquation();
            bool DisplayVA = false;
            DateTime CourseStartDate;
            foreach (Cerval_Library.Listitem l in Groups)
            {
                output.Write("<b>" + l.m_text + "</b>               ");
                PupilGroupList pgl1 = new PupilGroupList();
                pgl1.AddToList(l.m_value, ListDate); DisplayVA = false;
                foreach (DisplayItem d in DisplayItems)
                {
                    if (d._name == DisplayItem.Fields.Target) DisplayVA = d._display;
                }
                if (DisplayVA)
                {
                    Current_Group.Load(l.m_value);
                    CourseStartDate = Current_Group._StartDate;
                    SimplePupil p1 = (SimplePupil)pgl1.m_pupilllist[0];

                    CourseStartDate = new DateTime(DateTime.Now.Year, 9, 2);
                    if (DateTime.Now.Month < 9) CourseStartDate = CourseStartDate.AddYears(-1);
                    if (p1.m_year == 11) CourseStartDate = CourseStartDate.AddYears(-1);
                    if (p1.m_year == 13) CourseStartDate = CourseStartDate.AddYears(-1);

                    vam = vaml1.FindVAMethod(p1.m_StudentId, Current_Group._CourseID,CourseStartDate);

                    va1.Load1("WHERE (ValueAddedMethodID='" + vam.m_ValueAddedMethodID.ToString() + "') AND ( CourseID='" + Current_Group._CourseID.ToString() + "' ) ");
                    //now have the va equation.....
                    //going to form the text conversion string for values....
                    foreach (ValueAddedConversion va in vacl1.m_ValueAddedConversionList)
                    {
                        if (va.m_ResultType == vam.m_ValueAddedOutputResultType)
                        {
                            if ((va.m_ResultValue.Trim() != "+") && (va.m_ResultValue.Trim() != "*"))
                            {
                                VA_grades += va.m_ResultValue.Trim() + "=" + va.m_ResultNumericValue.ToString() + " : ";
                            }
                        }
                    }
                    VA_grades += "<br/> Alis/Yellis gives grade predictions based on a range, so A prediction at A-level from scores 110 - 130, and A prediction at GCSE from 49-55.";

                }
                output.Write("<table border style = \"font-size:small ;\">");
                output.Write("<tr>");
                foreach (DisplayItem d in DisplayItems)
                {
                    if (d._display) output.Write("<td>" + d._name.ToString() + "</td>");
                    if (!d._simple) display_contain_detail = true;
                }
                output.Write("</tr>"); total = 0;
                foreach (SimplePupil p in pgl1.m_pupilllist)
                {
                    if (display_contain_detail) pupil1.Load(p.m_StudentId.ToString()); else pupil1 = null;
                    output.Write("<tr>");
                    foreach (DisplayItem d in DisplayItems)
                    {
                        if (d._display)
                        {
                            d.CourseId = Current_Group._CourseID;//need for predicted grade
                            d.ValueAddedEqn = va1;
                            d.ValueAddedMth = vam;
                            s = d.GetValue(p, pupil1);
                            output.Write("<td>" + s + "</td>");
                        }
                    }
                    output.Write("</tr>"); total++;
                }
                output.Write("</table><br>Total for " + l.m_text + " : " + total.ToString() + "<br>");
                foreach (DisplayItem d in DisplayItems)
                {
                    if (d._display)
                    {
                        if (d._name == DisplayItem.Fields.Target)
                        {
                            s = "Target based on " + vam.m_ValeAddedDescription;
                            s += "<br />(" + VA_grades + ")";
                            output.Write("<P>" + s + "</p>");
                        }
                    }
                }
            }
            CSV_Button.RenderControl(output);
        }
        public int GetKeyStage_fromYear(string GroupName)
        {
            int KeyStage = 0; string s = "";
            if (GroupName.StartsWith("1")) { s = GroupName.Substring(0, 2); } else { s = GroupName.Substring(0, 1); }
            switch (s)
            {
                case "7":
                case "8":
                case "9":
                    KeyStage = 3;
                    break;
                case "10":
                case "11":
                    KeyStage = 4;
                    break;
                case "12":
                case "13":
                    KeyStage = 5;
                    break;
            }
            return KeyStage;
     }
    }

    [ToolboxData("<{0}:SENEditControl runat=server></{0}:SENEditControl>")]
    public class SENEditControl : WebControl
    {
        [Category("Data")]
        [Localizable(true)]
        public Guid StudentId
        {
            get
            {
                String s = (String)ViewState["ID"];
                return ((s == null) ? Guid.Empty : new Guid(s));
            }
            set
            {
                ViewState["ID"] = value.ToString();
                SimplePupil p = new SimplePupil();
                p.Load(value.ToString());
                Name_Label.Text = "Edit SEN detail for " + p.m_GivenName + " " + p.m_Surname + " (" + p.m_form + ") :Adno =" + p.m_adno.ToString();
            }
        }
        [Category("Data")]
        [Localizable(true)]
        public Guid SenId //mark as null or empty if new...
        {
            get { return ((ViewState["SenId"] == null) ? Guid.Empty : (Guid)ViewState["SenId"]); }
            set { ViewState["SenId"] = value; }
        }
        private TextBox TextBox_desc = new TextBox();
        private TextBox TextBox_Start = new TextBox();
        private TextBox TextBox_EndDate = new TextBox();
        private CheckBox CheckBox_CanType = new CheckBox();
        private TextBox TextBox_ExtraTime = new TextBox();
        private DropDownList DropDownList_SENTypes = new DropDownList();
        private DropDownList DropDownList_SENStatuses = new DropDownList();
        private Label Name_Label = new Label();
        private Button Button_Save = new Button();
        private Button Button_Cancel = new Button();


        protected override void RenderContents(HtmlTextWriter output)
        {
            if (StudentId == Guid.Empty) { output.Write("SENControl"); return; }
            output.Write("");
            Name_Label.RenderControl(output);
            output.Write("<br><br>");
            output.Write("<br>SEN Type   :"); DropDownList_SENTypes.RenderControl(output);
            output.Write("    SEN Status :"); DropDownList_SENStatuses.RenderControl(output);
            output.Write("<br><br>Start Date (dd/mm/yyyy) :"); TextBox_Start.RenderControl(output);
            output.Write("    End Date (blank = none) :"); TextBox_EndDate.RenderControl(output);
            output.Write("<br><br>Can Type :"); CheckBox_CanType.RenderControl(output);
            output.Write("Extra Time (%)          :"); TextBox_ExtraTime.RenderControl(output);
            output.Write("<br><br>SEN Description<br>"); TextBox_desc.RenderControl(output);
            output.Write("<br><br>"); Button_Save.RenderControl(output);
            Button_Cancel.RenderControl(output);
            output.Write("<br><hr>");
        }
        public SENEditControl()
        {
            Setup_Controls();
        }
        public void  Setup()
        {
            Setup_Controls();
        }

        protected void Setup_Controls()
        {
            Button_Save.Click += new EventHandler(Button_Save_Click);
            Button_Save.Text = "Save"; Button_Cancel.Text = "Cancel";
            Button_Cancel.Click += new EventHandler(Button_Cancel_Click);
            Controls.Add(Button_Cancel); Controls.Add(Button_Save);
            Controls.Add(TextBox_desc); Controls.Add(TextBox_EndDate);
            Controls.Add(TextBox_Start); Controls.Add(TextBox_ExtraTime);
            Controls.Add(DropDownList_SENStatuses); Controls.Add(DropDownList_SENTypes);
            Controls.Add(CheckBox_CanType); Controls.Add(Name_Label);
            TextBox_desc.Width = 400; TextBox_desc.Height = 100;
            TextBox_Start.Width = 80; TextBox_EndDate.Width = 80; TextBox_ExtraTime.Width = 40;
            TextBox_desc.TextMode = TextBoxMode.MultiLine;


            SENTypeList sen_TypeList = new SENTypeList();
            int i = 0;
            foreach (SENType t1 in sen_TypeList._List)
            {
                ListItem i1 = new ListItem(t1.SENtype, t1.id.ToString());
                DropDownList_SENTypes.Items.Add(i1);
                i++;
            }
            i = 0;
            SENStatusList sen_StatusList = new SENStatusList();
            foreach (SENStatus s1 in sen_StatusList._List)
            {
                ListItem i1 = new ListItem(s1.Status, s1.Id.ToString());
                DropDownList_SENStatuses.Items.Add(i1);
                i++;
            }
            if (SenId != Guid.Empty)
            {
                UpdateControls();
            }
            SimplePupil p1 = new SimplePupil(); p1.Load(StudentId.ToString());
            Name_Label.Text = "Edit SEN detail for " + p1.m_GivenName + " " + p1.m_Surname + " (" + p1.m_form + ") :Adno =" + p1.m_adno.ToString();
            //Image1.ImageUrl = "../PersonImagePage.aspx?id=" + p1.m_PersonId.ToString();

        }
        public void UpdateControls()
        {
            StudentSEN sen1 = new StudentSEN();
            string s = (string)ViewState["ID"];
            SimplePupil p1 = new SimplePupil(); p1.Load(s);
            Name_Label.Text = "Edit SEN detail for " + p1.m_GivenName + " " + p1.m_Surname + " (" + p1.m_form + ") :Adno =" + p1.m_adno.ToString();
            sen1.Load(SenId.ToString());
            TextBox_desc.Text = sen1.m_SenDescription;
            TextBox_Start.Text = sen1.m_StartDate.ToShortDateString();
            if (sen1.m_EndDate.Year > 2000) TextBox_EndDate.Text = sen1.m_EndDate.ToShortDateString();
            CheckBox_CanType.Checked = false;
            if (sen1.m_ExamsCanType) CheckBox_CanType.Checked = true;
            if (sen1.m_ExamsExtraTime > 0) TextBox_ExtraTime.Text = sen1.m_ExamsExtraTime.ToString();
            DropDownList_SENTypes.SelectedIndex = -1; DropDownList_SENStatuses.SelectedIndex = -1;
            foreach (ListItem l in DropDownList_SENTypes.Items)
            {
                if (sen1.m_SenType.ToString() == l.Value) l.Selected = true;
            }
            foreach (ListItem l in DropDownList_SENStatuses.Items)
            {
                if (sen1.m_SenStatus.ToString() == l.Value) l.Selected = true;
            }
        }
        private bool GetDatefromText(TextBox b, out DateTime d)
        {
            DateTime d1 = new DateTime(); bool f = true;
            try
            {
                b.BackColor = System.Drawing.Color.White;
                d1 = System.Convert.ToDateTime(b.Text);
            }
            catch
            {
                b.BackColor = System.Drawing.Color.Red;
                f = false;
            }
            d = d1;
            return f;
        }
        private string CleanInvertedCommas(string s)
        {
            int i = 0; i = s.IndexOf("'", i);
            while (i > 0) { i++; s = s.Substring(0, i) + "'" + s.Substring(i); i++; i = s.IndexOf("'", i); }
            return s;

        }
        private bool GetIntfromText(TextBox b, out int n)
        {
            bool f = true;
            n = 0;
            try
            {
                b.BackColor = System.Drawing.Color.White;
                n = System.Convert.ToInt32(b.Text);
            }
            catch
            {
                b.BackColor = System.Drawing.Color.Red;
                f = false;
            }
            return f;
        }
        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            //need to validate dates....????
            StudentSEN st1 = new StudentSEN();
            st1.Load(SenId.ToString());
            if (!GetDatefromText(TextBox_Start, out st1.m_StartDate)) return;
            if (TextBox_EndDate.Text == "") st1.m_EndDateValid = false;
            else
            {
                if (!GetDatefromText(TextBox_EndDate, out st1.m_EndDate)) return;
                st1.m_EndDateValid = true;
            }
            st1.m_SenDescription = CleanInvertedCommas(TextBox_desc.Text);
            if (!GetIntfromText(TextBox_ExtraTime, out st1.m_ExamsExtraTime)) return;
            st1.m_SenType = System.Convert.ToInt32(DropDownList_SENTypes.SelectedItem.Value);
            st1.m_SenStatus = System.Convert.ToInt32(DropDownList_SENStatuses.SelectedItem.Value);
            st1.m_ExamsCanType = CheckBox_CanType.Checked;
            st1.m_StudentId = StudentId;
            st1.Save();
            this.Visible = false;        
        }
    }

    [ToolboxData("<{0}:SENListControl runat=server></{0}:SENListControl>")]
    public class SENListControl : WebControl
    {
        private GridView GridView1 = new GridView();
        private SqlDataSource data1 = new SqlDataSource();
        private BoundField FieldId = new BoundField();
        private BoundField FieldPersonSurname = new BoundField();
        private BoundField FieldPersonGivenName = new BoundField();
        private BoundField FieldAdmissionNumber = new BoundField();
        private BoundField FieldStartDate = new BoundField();
        private BoundField FieldSenStatusType = new BoundField();
        private BoundField FieldSenType = new BoundField();
        private BoundField FieldDescription = new BoundField();
        private BoundField FieldExamsExtraTime = new BoundField();
        private BoundField FieldExamsCanType = new BoundField();
        private BoundField FieldGroupCode = new BoundField();
        private ButtonField Button_Edit = new ButtonField();
        private ButtonField Button_Delete = new ButtonField();
        private SENEditControl SENEditControl1 = new SENEditControl();
        private Button ExcelExport = new Button();
        private DataGrid datagrid = new DataGrid();

        public string SurnameMask //mark as null or empty if new...
        {
            get { return ((ViewState["Mask"] == null) ? "" : (string)ViewState["Mask"]); }
            set { ViewState["Mask"] = value; }
        }

        public SENListControl()
        {
            SetupControls();
        }
        private void AddColumn(BoundField b, int width, string fieldname, string title)
        {
            GridView1.Columns.Add(b);
            b.DataField = fieldname;
            b.HeaderText = title;
            b.ReadOnly = true;
            b.SortExpression = fieldname;
            b.ItemStyle.Font.Size = FontUnit.XSmall;
            b.ItemStyle.ForeColor = System.Drawing.Color.Black;
            b.ItemStyle.Width = width;
            b.ItemStyle.Wrap = true;
        }
        private void SetupControls()
        {
            Controls.Add(GridView1);
            Controls.Add(data1);
            Controls.Add(SENEditControl1);
            Controls.Add(ExcelExport); ExcelExport.Text = "Export to Excel";
            Controls.Add(datagrid);
            ExcelExport.Click += new EventHandler(ExcelExport_Click);
            SurnameMask = "";
            datagrid.DataSourceID = "data1";
            datagrid.AutoGenerateColumns = true;

            SENEditControl1.Visible = false;
            GridView1.ID = "GridView1"; GridView1.AllowSorting = true; GridView1.AllowPaging = true; ;
            GridView1.HeaderStyle.Font.Size = FontUnit.XSmall; GridView1.AutoGenerateColumns = false;
            GridView1.BackColor = System.Drawing.Color.White; GridView1.BorderStyle = BorderStyle.None;
            GridView1.BorderColor = System.Drawing.Color.FromArgb(003399); GridView1.BorderWidth = 1;
            GridView1.CellPadding = 4; //GridView1.DataKeyNames = "IncidentID";
            GridView1.DataSourceID = "data1"; GridView1.Width = 610;
            GridView1.PageSize = 5;
            GridView1.RowCommand += new GridViewCommandEventHandler(GridView1_RowCommand);

            AddColumn(FieldId, 50, "Id", "ID");
            FieldId.ItemStyle.ForeColor = System.Drawing.Color.Gray;
            FieldId.ItemStyle.Width = 10; FieldId.ItemStyle.Wrap = true;

            AddColumn(FieldPersonGivenName, 50, "PersonGivenName", "Given");
            AddColumn(FieldPersonSurname, 50, "PersonSurname", "Surname");
            AddColumn(FieldAdmissionNumber, 50, "AdmissionNumber", "Adno");
            AddColumn(FieldStartDate, 50, "StartDate", "Start");
            AddColumn(FieldSenStatusType, 50, "SenStatusType", "Status");
            AddColumn(FieldSenType, 50, "SenType", "Type");
            AddColumn(FieldDescription, 50, "Description", "Description");
            AddColumn(FieldExamsExtraTime, 50, "ExamsExtraTime", "Xtime");
            AddColumn(FieldExamsCanType, 50, "ExamsCanType", "Type");
            AddColumn(FieldGroupCode, 50, "GroupCode", "GroupCode");
            GridView1.Columns.Add(Button_Edit); Button_Edit.ButtonType = ButtonType.Button; Button_Edit.CommandName = "Edit"; Button_Edit.HeaderText = "Edit"; Button_Edit.ShowHeader = true; Button_Edit.Text = "Edit"; Button_Edit.ItemStyle.Font.Size = FontUnit.XSmall; Button_Edit.Visible = false;
            GridView1.Columns.Add(Button_Delete); Button_Delete.ButtonType = ButtonType.Button; Button_Delete.CommandName = "Delete"; Button_Delete.HeaderText = "Delete"; Button_Delete.ShowHeader = true; Button_Delete.Text = "Delete"; Button_Delete.ItemStyle.Font.Size = FontUnit.XSmall;

            Encode en = new Encode();
            data1.ConnectionString = en.GetDbConnection();
            data1.SelectCommand = GetQuery(SurnameMask);
            data1.ProviderName = "System.Data.SqlClient";
            data1.ID = "data1";
        }

        void ExcelExport_Click(object sender, EventArgs e)
        {
            //export to excel

            Page.Response.Clear();
            datagrid.DataBind();
            Page.Response.Buffer = true;
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.Charset = "";
            this.EnableViewState = false;
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
            HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            datagrid.RenderControl(oHtmlTextWriter);
            Page.Response.Write(oStringWriter.ToString());
            Page.Response.End();

        }
        private string GetQuery(string like_clause)
        {
            string s = "SELECT [Id], [PersonSurname], [PersonGivenName], [AdmissionNumber], [StartDate], [SenStatusType], [SenType], [Description], [ExamsExtraTime], [ExamsCanType], [EndDate], [GroupCode] FROM [qry_Cerval_Core_Student_SEN2]  WHERE ( (EndDate > { fn NOW() })  OR (EndDate IS NULL) ) ";
            if (like_clause != "")
            {
                s += " AND (PersonSurname LIKE '" + like_clause + "%') ";

            }
            s += "ORDER BY [PersonSurname]";
            return s;
        }
        void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                int row = Convert.ToInt32(e.CommandArgument);
                GridViewRow row1 = GridView1.Rows[row];
                string s = Page.Server.HtmlDecode(row1.Cells[0].Text);
                SENEditControl1.Visible = true;
                SENEditControl1.SenId = new Guid(s);
                string adno = Page.Server.HtmlDecode(row1.Cells[3].Text);
                SimplePupil p1 = new SimplePupil();
                int a = System.Convert.ToInt32(adno);
                p1.Load_StudentIdOnly(System.Convert.ToInt32(adno));
                SENEditControl1.StudentId = p1.m_StudentId;
            }

            if (e.CommandName == "Delete")
            {
                int row = Convert.ToInt32(e.CommandArgument);
                GridViewRow row1 = GridView1.Rows[row];
                string s = Page.Server.HtmlDecode(row1.Cells[0].Text);
                StudentSEN stsen1 = new StudentSEN();
                stsen1.Load(s);
                stsen1.Delete();
            }
        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (SENEditControl1.Visible)
            {
                SENEditControl1.UpdateControls();
                SENEditControl1.RenderControl(output);

            }
            else
            {
                output.Write("<h3>Full Current SEN List</h3><br/>"); ExcelExport.RenderControl(output);
                data1.SelectCommand = GetQuery(SurnameMask);
                GridView1.DataBind();
                GridView1.RenderControl(output);

            }
        }
    }

    [ToolboxData("<{0}:StatsControl runat=server></{0}:StatsControl>")]
    public class StatsControl : WebControl
    {
        private string[] forms = new string[100];
        private int no_forms = 0;
        private int[] form_count = new int[100];
        private string[] years = new string[10];
        private int no_years = 7;
        private int[] year_count = new int[10];

        public enum StatsType { none, School, Year, Form, NotOnRole }
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public StatsType type
        {
            get { return ((ViewState["Type"] == null) ? StatsType.none : (StatsType)ViewState["Type"]); }
            set { ViewState["Type"] = value; }
        }

        public StatsControl()
        {
            Calculate();
        }

        private string GetHeaderString()
        {
            return ("<p style=\"font-size:small \"><div align=center><h4>School Population Statistics " + DateTime.Now.ToShortDateString() + "</h4></div><br>");
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            string s1 = ""; string s = ""; int i = 0;
            switch (type)
            {
                case StatsType.none: output.Write("StatsControl");
                    break;
                case StatsType.School:
                    s1 = GetHeaderString();
                    for (int j = 0; j < no_years; j++)
                    {
                        i += year_count[j];
                    }
                    s1 += "<div align=center> Total:   " + i.ToString();
                    s1 += "</div><br></p>";
                    output.Write(s1);
                    break;
                case StatsType.Year:
                    s1 = GetHeaderString();
                    s1 += "<table align =center  border><tr>";
                    s1 += "<td>Year</td><td>Number</td></tr>";
                    for (int j = 0; j < no_years; j++)
                    {
                        s = "SimpleLists.aspx?Type=Group&Group=" + years[j] + "Year";
                        s = "<A HREF=\"" + s + "\">" + years[j] + "</A>";
                        s1 += "<tr><td>" + s + "</td>";
                        s1 += "<td>" + year_count[j].ToString() + "</td></tr>";
                        i += year_count[j];
                    }
                    s1 += "<tr><td> Total</td><td>" + i.ToString() + "</td></tr>";
                    s1 += "</table><br></p>";
                    output.Write(s1);
                    break;
                case StatsType.Form:
                    s1 = GetHeaderString();
                    s1 += "<table align =center  border><tr>";
                    s1 += "<td>Form</td><td>Number</td></tr>";
                    for (int j = 0; j < no_forms; j++)
                    {
                        s = "SimpleLists.aspx?Type=Group&Group=" + forms[j];
                        s = "<A HREF=\"" + s + "\">" + forms[j] + "</A>";
                        s1 += "<tr><td>" + s + "</td>";
                        s1 += "<td>" + form_count[j].ToString() + "</td></tr>";
                    }
                    s1 += "</table><br><br>";
                    output.Write(s1);
                    break;
                case StatsType.NotOnRole:
                    s1 = GetHeaderString();
                    StudentsOnRoleNotInRegGroup nr = new StudentsOnRoleNotInRegGroup();
                    s1 += "Students on Role but NOT in a registration Group and so not included in list above:<br>";
                    s1 += "<table align =center  border><tr>";
                    s1 += "<td>GivenName</td><td>Surname</td><td>Adno</td><td>Details</td></tr>";
                    i = 0;
                    foreach (SimplePupil p in nr._studentlist)
                    {
                        s1 += "<tr><td>" + p.m_GivenName + "</td>";
                        s1 += "<td>" + p.m_Surname + "</td>";
                        s1 += "<td>" + p.m_adno.ToString() + "</td>";
                        s1 += "<td><A HREF=\"PupilChoice.aspx?Type=Student&Id=" + p.m_StudentId.ToString() + "&Name=" + p.m_GivenName + " " + p.m_Surname + "\">Details</A></td>";
                        s1 += "</tr>";
                        i++;
                    }
                    s1 += "<tr><td> Total</td><td>" + i.ToString() + "</td></tr>";
                    s1 += "</table><br><br>";
                    output.Write(s1);
                    break;

                default:
                    break;
            }
        }

        private void Calculate()
        {
            SimpleStudentList st1 = new SimpleStudentList("");
            //SimpleStudentList st1 = new SimpleStudentList();
            //DateTime time1 = new DateTime(2010, 11, 11, 0, 0, 0);
            //st1.LoadList_atDate(time1,false);
            years[0] = "7"; years[1] = "8"; years[2] = "9"; years[3] = "10"; years[4] = "11";
            years[5] = "12"; years[6] = "13";
            string s = ""; int i = 0;
            foreach (SimplePupil p in st1)
            {
                s = p.m_form; i = 0;
                while ((i < no_forms) && (s != forms[i])) i++;
                if (i == no_forms) { no_forms++; forms[i] = s; }
                form_count[i]++;
                s = p.m_year.ToString(); i = 0;
                while ((i < no_years) && (s != years[i])) i++;
                if (i == no_years) { no_years++; years[i] = s; }
                year_count[i]++;
            }
            //order the forms
            for (int k = 0; k < no_forms - 1; k++)
            {
                for (int j = 0; j < no_forms - 1 - k; j++)
                {
                    if (CompareFormName(forms[j], forms[j + 1]))
                    {
                        s = forms[j]; i = form_count[j];
                        forms[j] = forms[j + 1]; form_count[j] = form_count[j + 1];
                        forms[j + 1] = s; form_count[j + 1] = i;
                    }
                }
            }
        }

        protected bool CompareFormName(string s1, string s2)
        {
            //true if s1>s2
            int i1, i2;
            if (s1.StartsWith("1")) i1 = System.Convert.ToInt32(s1.Substring(0, 2)); else i1 = System.Convert.ToInt32(s1.Substring(0, 1));
            if (s2.StartsWith("1")) i2 = System.Convert.ToInt32(s2.Substring(0, 2)); else i2 = System.Convert.ToInt32(s2.Substring(0, 1));
            if (i1 < i2) return false;
            if (i1 > i2) return true;
            //ok so same year so we can compare strings
            if (string.Compare(s1, s2) > 0) return true;
            return false;
        }
    }

    [ToolboxData("<{0}:ReportCommentControl runat=server></{0}:ReportCommentControl>")]
    public class ReportCommentControl : WebControl
    {

        [Category("Data")]
        [Localizable(true)]
        public Guid StudentId
        {
            get { return ((ViewState["StudentId"] == null) ? Guid.Empty : (Guid)ViewState["StudentId"]); }
            set { ViewState["StudentId"] = value; }
        }
        public enum ReportCommentType { none = 0, ImprovementPoints = 1, Comments = 2, }
        public ReportCommentType CommentType
        {
            get { return ((ViewState["CommentType"] == null) ? ReportCommentType.none : (ReportCommentType)ViewState["CommentType"]); }
            set { ViewState["CommentType"] = value; }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (StudentId == Guid.Empty) { output.Write("custom1"); return; }
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId.ToString());
            ReportCommentList rplcoml1 = new ReportCommentList(StudentId.ToString());
            output.Write("<b>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</b>  Report " + CommentType.ToString() + "<br>");

            output.Write("<table border style = \"font-size:small ;\">");
            string s = CommentType.ToString();
            output.Write("<tr><td>Date</td><td>Staff</td><td>Course</td><td>" + s + "</td></tr>");
            foreach (ReportComment r in rplcoml1.m_list)
            {
                if ((CommentType == ReportCommentType.none) || (r.m_commentType == (int)CommentType))
                {
                    output.Write("<tr><td>" + r.m_PolicyName + "</td>");
                    //output.Write("<tr><td>" + r.m_dateModified.ToShortDateString() + "</td>");
                    output.Write("<td>" + r.m_staffCode + "</td>");
                    output.Write("<td>" + r.m_CourseName + "</td>");
                    output.Write("<td>" + r.m_content + "</td>");
                    output.Write("</tr>");
                }
            }
            output.Write("</table>");

        }

    }

    [ToolboxData("<{0}:ResultGrid runat=server></{0}:ResultGrid>")]
    public class ResultGrid : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Result Grid")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        [Bindable(true)]
        [Category("Custom")]
        [DefaultValue("")]
        [Localizable(true)]
        public string StudentId
        {
            get
            {
                String s = (String)ViewState["StudentId"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["StudentId"] = value;
            }
        }


        [Bindable(true)]
        [Category("Custom")]
        [DefaultValue("")]
        [Localizable(true)]
        public GridDisplayType DisplayType
        {
            get
            {
                return (GridDisplayType)ViewState["DisplayType"];
            }

            set
            {
                ViewState["DisplayType"] = value;
            }
        }

        [Bindable(true)]
        [Category("Custom")]
        [DefaultValue("")]
        [Localizable(true)]
        public GridDisplayFlags DisplayFlags
        {
            get
            {
                return (GridDisplayFlags)ViewState["DisplayType"];
            }

            set
            {
                ViewState["DisplayType"] = value;
            }
        }

        private int max_rows;
        private int max_cols;
        private ArrayList a1;// holds the tabel grid....

        public ResultGrid()
        {
            DisplayType = GridDisplayType.none;
            DisplayFlags = GridDisplayFlags.Name | GridDisplayFlags.MaximumMark;
        }

        [Flags]
        public enum GridDisplayType { none = 0x00, External = 0x01, Internal = 0x02, Module = 0x04, Predicted = 0x08, Internal_Scaled };
        [Flags]
        public enum GridDisplayFlags { none = 0x00, Name = 0x01, ExamBoard = 0x02, MaximumMark = 0x04, Date = 0x08 };

        protected override void RenderContents(HtmlTextWriter output)
        {
            bool display_external_message = false;
            if (StudentId == "") { output.Write("ResultGrid"); return; }
            try
            {
                Guid g1 = new Guid(StudentId);
            }
            catch (Exception e)
            {
                output.Write("ResultGrid"); return;
            }
            ResultsList rl = new ResultsList();
            rl.LoadList_OrderByDate("dbo.tbl_Core_Students.StudentId", StudentId);
            bool AnyBreakdowns = false;

            //so we have results... output as table.....
            int x = 0;
            max_rows = 0;
            max_cols = 1;
            int y = 0;
            double d1 = 0;
            string s = ""; string css_class = "";
            a1 = new ArrayList();

            switch (DisplayType)
            {
                case GridDisplayType.External:
                    s = "Date"; OutputElement(s, a1, x, y); x++;
                    s = "Description"; OutputElement(s, a1, x, y); x++;
                    s = "Syllabus Code"; OutputElement(s, a1, x, y); x++;
                    s = "Syllabus Title"; OutputElement(s, a1, x, y); x++;
                    s = "Result"; OutputElement(s, a1, x, y); x++;
                    s = "Add Info"; OutputElement(s, a1, x, y); x++;
                    s = "Board"; OutputElement(s, a1, x, y); x++;
                    y++; max_cols = x;
                    break;
                case GridDisplayType.Module:
                    s = "Date"; OutputElement(s, a1, x, y); x++;
                    s = "Subject"; OutputElement(s, a1, x, y); x++;
                    s = "Option Title"; OutputElement(s, a1, x, y); x++;
                    s = "Option Code"; OutputElement(s, a1, x, y); x++;
                    s = "Result"; OutputElement(s, a1, x, y); x++;
                    s = "Max Mark"; OutputElement(s, a1, x, y); x++;
                    s = "Grade"; OutputElement(s, a1, x, y); x++;
                    y++; max_cols = x;
                    break;
                case GridDisplayType.Internal:
                    ReportList repl = new ReportList(StudentId.ToString());
                    //try to compact down to one attainment grade /date....
                    //they are ordered by date (normalised) and then course, so should be easy
                    repl.ReduceToAttainment();

                    //going to try to add the report grades in to the results...

                    foreach (ReportValue v in repl.m_list)
                    {
                        Result res1 = new Result();
                        rl._results.Add(res1);
                        res1.External = false;
                        res1.Date = v.m_date;
                        res1.Shortname = "Report";
                        res1.Code = v.m_course;
                        if (v.m_IsCommitment) res1.Shortname = "Commitment";
                        x = (int)(v.m_value * 10);
                        d1 = (float)x / 10;
                        res1.Value = d1.ToString();
                        switch (v.m_courseType)
                        {
                            case 1://KS 3
                            case 2://KS 4
                                if (v.m_MaxLevel == 8)
                                {
                                    res1.Value = "";
                                    if (v.m_value > 0.0) res1.Value = "3";
                                    if (v.m_value > 1) res1.Value = "4";
                                    if (v.m_value > 2) res1.Value = "5";
                                    if (v.m_value > 3) res1.Value = "6";
                                    if (v.m_value > 4) res1.Value = "7";
                                    if (v.m_value > 5) res1.Value = "8";
                                    if (v.m_value > 6) res1.Value = "9";
                                    if (v.m_value > 6.5) res1.Value = "10";

                                }
                                else
                                {
                                    res1.Value = "";
                                    if (v.m_value > 0.0) res1.Value = "D";
                                    if (v.m_value > 1) res1.Value = "C";
                                    if (v.m_value > 2) res1.Value = "B";
                                    if (v.m_value > 3) res1.Value = "A";
                                    if (v.m_value > 4) res1.Value = "*";
                                    if (v.m_value > 4.5) res1.Value = "**";
                                }

                                break;
                            case 3://KS 5
                                //so here we need to map values from db for save points to grades...
                                res1.Value = "";
                                if (v.m_value > 0.0) res1.Value = "D";
                                if (v.m_value > 1) res1.Value = "C";
                                if (v.m_value > 2) res1.Value = "B";
                                if (v.m_value > 3) res1.Value = "A";
                                if (v.m_value > 4) res1.Value = "*";
                                if (v.m_value > 4.5) res1.Value = "**";//!! not going to happen
                                break;
                        }
                    }
                    Result r0 = new Result();
                    Result r2 = new Result();
                    //would be nice to sort the list by date.....  cos at the mo we have report stuf at end
                    for (int j = 0; j < rl._results.Count; j++)
                    {
                        for (int k = 0; k < (rl._results.Count - 1 - j); k++)
                        {
                            r0 = (Result)rl._results[k]; r2 = (Result)rl._results[k + 1];
                            if (r0.Date > r2.Date)
                            {
                                rl._results[k] = r2; rl._results[k + 1] = r0;
                            }
                        }
                    }
                    break;
                case GridDisplayType.Predicted:
                    s = "Date"; OutputElement(s, a1, x, y); x++;
                    s = "Subject"; OutputElement(s, a1, x, y); x++;
                    s = "Predicted Grade"; OutputElement(s, a1, x, y); x++;
                    y++; max_cols = x;
                    this.Visible = false;//hide if no predictions
                    break;

            }

            string temps = ""; int max_d = 1;
            string[,] grades = new string[50, 100];// [date, subject
            grades[0, 0] = "Date";
            //ExamOption exo1 = new ExamOption();
            //Exam_Board exb1 = new Exam_Board();
            bool External_has_components = false;
            foreach (Result r1 in rl._results)
            {
                switch (DisplayType)
                {
                    case GridDisplayType.External:
                        if (r1.External)//only external
                        {
                            if (r1.Resulttype == 34)
                            {
                                //expq
                            }

                            if (r1.Resulttype == 25)
                            {
                                //STEP - no basedate/options data
                                x = 0;
                                OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                                OutputElement(r1.Description, a1, x, y); x++;
                                OutputElement("", a1, x, y); x++;
                                OutputElement(r1.Coursename, a1, x, y); x++;
                                OutputElement(r1.Value, a1, x, y); x++;
                                OutputElement(r1.Text, a1, x, y); x++;
                                y++;
                            }
                            if ((r1.OptionItem == "C") || ((r1.OptionItem == "") && (r1.Resulttype != 27) && (r1.Resulttype != 11)))//new options or not module
                            {
                                //exo1.Load(r1.OptionId);
                                x = 0;
                                //check to see if any component results..
                                ExamComponentResultList ecrl1 = new ExamComponentResultList();

                                OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                                OutputElement(r1.Shortname, a1, x, y); x++;
                                OutputElement(r1.SyllabusCode, a1, x, y); x++;
                                try
                                {
                                    if (ecrl1.Load_OptionStudent(r1.OptionId, r1.StudentID) > 0)
                                    {
                                        string s_extra = "";
                                        s_extra = "<a href = \"../StudentInformation/ComponentResults.aspx?StudentId="+r1.StudentID.ToString()+"&OptionId="+r1.OptionId.ToString()+"\" > "+ r1.OptionTitle + " </ a >";
                                        External_has_components = true;
                                             OutputElement(s_extra, a1, x, y); x++;
                                    }
                                    else
                                    {
                                        OutputElement(r1.OptionTitle, a1, x, y); x++;
                                    }
                                }
                                catch
                                {
                                    OutputElement(r1.OptionTitle, a1, x, y); x++;
                                }
                                //OutputElement(r1.OptionTitle, a1, x, y); x++;
                                OutputElement(r1.Value, a1, x, y); x++;
                                OutputElement(r1.Text, a1, x, y); x++;
                                ExamOption exo1 = new ExamOption(); exo1.Load(r1.OptionId);
                                Exam_Board eb1 = new Exam_Board(exo1.m_ExamBoardID);
                                OutputElement(eb1.m_OrganisationFriendlyName, a1, x, y); x++;

                                y++;
                            }
                            else
                            {
                                if ((r1.Resulttype == 10) || (r1.Resulttype == 35))
                                {
                                    x = 0;
                                    OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                                    OutputElement(r1.Shortname+"external", a1, x, y); x++;
                                    OutputElement("Unkown", a1, x, y); x++;
                                    OutputElement(r1.Coursename, a1, x, y); x++;
                                    OutputElement(r1.Value, a1, x, y); x++;
                                    OutputElement(r1.Text, a1, x, y); x++;
                                    //OutputElement("Unkown", a1, x, y); x++;
                                    y++;
                                    display_external_message = true;
                                }
                            }
                        }
                        break;
                    case GridDisplayType.Module:
                        if (r1.OptionItem == "U")
                        {//we have an exam result
                            x = 0;
                            OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                            OutputElement(r1.Coursename, a1, x, y); x++;
                            OutputElement(r1.OptionTitle, a1, x, y); x++;
                            OutputElement(r1.OptionCode, a1, x, y); x++;
                            OutputElement(r1.Value, a1, x, y); x++;
                            OutputElement(r1.OptionMaximumMark, a1, x, y); x++;
                            s = "";

                            if (r1.Resulttype == 11)//GCE modules
                            {
                                try
                                {
                                    double v1 = System.Convert.ToDouble(r1.Value);
                                    double v2 = System.Convert.ToDouble(r1.OptionMaximumMark);
                                    v1 = v1 / v2;
                                    if (v1 >= 0.8) s = "a";
                                    if ((v1 < 0.8) && (v1 >= 0.7)) s = "b";
                                    if ((v1 < 0.7) && (v1 >= 0.6)) s = "c";
                                    if ((v1 < 0.6) && (v1 >= 0.5)) s = "d";
                                    if ((v1 < 0.5) && (v1 >= 0.4)) s = "e";
                                    if ((v1 < 0.4)) s = "u";
                                }
                                catch (Exception exc1)
                                {
                                    string sx = exc1.Message;
                                }
                            }
                            if (r1.Resulttype == 27)//GCSE modules
                            {
                                if (r1.Text != "")
                                {
                                    s = r1.Text;
                                }
                                else
                                {

                                    try
                                    {
                                        double v1 = System.Convert.ToDouble(r1.Value);
                                        double v2 = System.Convert.ToDouble(r1.OptionMaximumMark);
                                        v1 = v1 / v2;
                                        if (v1 >= 0.9) s = "a*";
                                        if ((v1 < 0.9) && (v1 >= 0.8)) s = "a";
                                        if ((v1 < 0.8) && (v1 >= 0.7)) s = "b";
                                        if ((v1 < 0.7) && (v1 >= 0.6)) s = "c";
                                        if ((v1 < 0.6) && (v1 >= 0.5)) s = "d";
                                        if ((v1 < 0.5) && (v1 >= 0.4)) s = "e";
                                        if ((v1 < 0.4)) s = "u";
                                    }
                                    catch (Exception exc1)
                                    {
                                        string sx = exc1.Message;
                                    }
                                }

                            }
                            OutputElement(s, a1, x, y); x++;
                            y++;
                        }
                        break;
                    case GridDisplayType.Internal:
                        if (!r1.External)
                        {
                            temps = r1.Date.ToShortDateString() + "  " + r1.Shortname.Trim(); y = -1;
                            for (int i = 0; i < max_d; i++)
                            {
                                if (temps == grades[0, i]) y = i;
                            }
                            if (y < 0)
                            {
                                y = max_d;
                                grades[0, y] = temps;
                                max_d++;
                            }
                            //need to find subject it or add it...
                            x = -1;
                            for (int i = 1; i < max_cols; i++)
                            {
                                if (grades[i, 0] == r1.Code.Substring(0, 2)) x = i;
                            }
                            if (x < 0)
                            {
                                x = max_cols; grades[x, 0] = r1.Code.Substring(0, 2); max_cols++;
                            }
                            grades[x, y] = r1.Value;
                        }
                        break;
                    case GridDisplayType.Predicted:
                        if (r1.Resulttype == 6)
                        {//we have a predicted result
                            x = 0; this.Visible = true;
                            OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                            OutputElement(r1.Coursename, a1, x, y); x++;
                            OutputElement(r1.Value, a1, x, y); x++;
                        }
                        break;

                }
            }
            //going to copy header row....
            y = max_d; for (int i = 0; i < max_cols; i++)
            {
                grades[i, y] = grades[i, 0];
            }
            max_d++;



            if (DisplayType == GridDisplayType.Internal)
            {
                //now would be quite nice to sort in x....
                for (int i = 0; i < (max_cols); i++)
                {
                    for (int j = 1; j < (max_cols - 1 - i); j++)//don't move col 0 -- date
                    {
                        if (grades[j, 0].CompareTo(grades[j + 1, 0]) > 0)
                        {
                            //swop...
                            for (int k = 0; k < max_d; k++)
                            {
                                s = grades[j, k]; grades[j, k] = grades[j + 1, k]; grades[j + 1, k] = s;
                            }
                        }
                    }
                }
                for (y = 0; y < max_d; y++)
                {
                    for (x = 0; x < max_cols; x++)
                    {
                        OutputElement(grades[x, y], a1, x, y);
                    }
                }
                y = max_d;
            }

            ViewState["Data"] = a1;
            ViewState["mx"] = max_cols;
            ViewState["my"] = y;

            a1 = (ArrayList)ViewState["Data"];
            int max_x = (int)ViewState["mx"];
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId);
            s = "<p ><h3 align=\"center\">";
            switch (DisplayType)
            {
                case GridDisplayType.External: s += "Certified Results for "; css_class = "ResultsTbl";
                    break;
                case GridDisplayType.Module: s += "Module Results for "; css_class = "ResultsTbl";
                    break;
                case GridDisplayType.Internal: s += "Internal Results for "; css_class = "ResultsTbl";
                    break;
                case GridDisplayType.Predicted: s += "Predicted Grades for "; css_class = "ResultsTbl";
                    break;
            }
            s += pupil1.m_GivenName + " " + pupil1.m_Surname + "</h3></p><br/>";
            switch (DisplayType)
            {
                case GridDisplayType.Module: if (AnyBreakdowns) s += "<h4>(Mark Breakdowns are available for modules with hyperlinks shown.)</h4><br/>";
                    break;
                case GridDisplayType.External:
                  s += "Please note that the 'Add Info', where present, is the total mark for that Qualification. ";
                    s+= "For <b><i>modular</i></b> specifications this is the sum of the UMS marks for all modules.";
                    if (External_has_components)
                    {
                        s += "For new <b><i>linear</i></b> Qualifications this is the TQM (<b>not</b> UMS!!!) and further details, including component marks and grade boundaries if available, can be accessed via the hyper-link.";
                    }
                        break;

            }
            s += "<br/><br /><TABLE BORDER   class=\"" + css_class + "\" style = \"font-size:small ;  \">";
            foreach (Grid_Element g in a1)
            {
                if (g.m_x == 0) s += "<TR>";

                if (g.m_y == 0) s += "<Th>" + g.m_value + "</Th>";
                else s += "<TD>" + g.m_value + "</TD>";

                if (g.m_x == max_x) s += "</TR>";
            }
            s += "</table></p>";
            output.Write(s);
            if (display_external_message)
            {
                output.Write("</br>For external results: 'External' in the Add info column means we have seen some collaborating information (results slip etc) and 'External-verified' means we have seen the actual certificates.");
            }



        }

        private ArrayList OutputElement(string s, ArrayList a, int x, int y)
        {
            //Response.Write("<TD>"+s+"</TD>");
            Grid_Element g = new Grid_Element();
            a.Add(g); g.m_x = x; g.m_y = y; g.m_value = s;
            return a;
        }

    }

}
