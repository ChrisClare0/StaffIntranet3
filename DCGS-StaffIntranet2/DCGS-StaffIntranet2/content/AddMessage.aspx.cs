using System;
using System.Web.UI.WebControls;
using Cerval_Library;


namespace DCGS_Staff_Intranet2
{
    public partial class AddMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Label_MessageType.Text = "Group";
                UpdateMessageList(Label_MessageType.Text);
                DropDownList_Year.SelectedIndex = 0;
                Calendar2.SelectedDate = DateTime.Now;
            }
        }
        protected void UpdateMessageList(string t)
        {
            Cerval_Globals cg1 = new Cerval_Globals();
            ListBox_staff.Items.Clear();
            string s1 = DropDownList_Year.SelectedValue;
            DateTime date1 = new DateTime();
            date1 = DateTime.Now.AddMonths(0);

            if (t == "Group")
            {
                GroupList gl1;
                if (s1 == "My Groups")
                {
                    gl1 = new GroupList();
                    Utility u = new Utility();
                    string staff_code = u.GetsStaffCodefromRequest(Request);
                    gl1.LoadList_StaffPrivateGroups(staff_code.Trim(), GroupList.GroupListOrder.GroupName);
                    foreach (Group g in gl1._groups)
                    {
                        ListItem l1 = new ListItem(g._GroupName, g._GroupID.ToString());
                        ListBox_staff.Items.Add(l1);
                    }
                    return;
                }
                else
                {
                    gl1 = new GroupList("", date1);
                }
                //put the form groups first
                foreach (Group g in gl1._groups)
                {
                    if (g._GroupCode.StartsWith(s1))
                    {
                        if (g._CourseID == Cerval_Globals.RegistrationCse)//registration../
                        {
                            //only for year groups...
                            ListItem l1 = new ListItem(g._GroupCode, g._GroupID.ToString());
                            ListBox_staff.Items.Add(l1);
                        }
                    }
                }
                foreach (Group g in gl1._groups)
                {
                    if(g._GroupCode.StartsWith(s1))
                    {
                        if ((g._CourseID != Cerval_Globals.RegistrationCse)&&(!Cerval_Globals.NewStructureCourses.Contains(g._CourseID)))
                        {
                            ListItem l1 = new ListItem(g._GroupCode, g._GroupID.ToString());
                            ListBox_staff.Items.Add(l1);
                        }
                    }
                    if (s1.ToUpper() == "14")
                    {
                        if (g._GroupCode.StartsWith("1") || g._GroupCode.StartsWith("7") || g._GroupCode.StartsWith("8") || g._GroupCode.StartsWith("9"))
                        {
                        }
                        else
                        {
                            ListItem l1 = new ListItem(g._GroupCode, g._GroupID.ToString());
                            ListBox_staff.Items.Add(l1);
                        }
                    }
                }
            }


            if (t == "Student")
            {
                //StudentYearList yl1 = new StudentYearList(ListBox_staff, s1 + "Year", "{ fn NOW() }");
                StudentYearList yl1 = new StudentYearList(ListBox_staff, s1 + "Year", DateTime.Now);
                ListBox_staff.SelectionMode = System.Web.UI.WebControls.ListSelectionMode.Multiple;
                TextBox_textTo.Visible = true;
            }
        }
        protected void Button_groups_Click(object sender, EventArgs e)
        {
            string s = Label_MessageType.Text;
            switch (s)
            {
                case "Group":
                    Label_MessageType.Text = "Student";
                    Button_groups.Text = "Select Groups";
                    break;
                case "Student":
                    Button_groups.Text = "Select Individuals";
                    Label_MessageType.Text = "Group"; 
                    break;
            }

            UpdateMessageList(Label_MessageType.Text);
        }

        protected void DropDownList_Year_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMessageList(Label_MessageType.Text);
        }

        protected void Button_Add_Click(object sender, EventArgs e)
        {
            string s = Label_MessageType.Text;

            switch (s)
            {
                case "Group":
                    foreach (ListItem i in ListBox_staff.Items)
                    {
                        if (i.Selected)
                        {
                            //need to check to see if staff groups...
                            Group gp1 = new Group(); gp1.Load(new Guid(i.Value));
                            if (gp1._CourseID == Cerval_Globals.newdawnCse)
                            {
                                //oh pain we are going to have to add names...
                                PupilGroupList pgl1 = new PupilGroupList();
                                pgl1.AddToList(gp1._GroupID, DateTime.Now);
                                foreach (SimplePupil p in pgl1.m_pupilllist)
                                {
                                    TextBox_textTo.Text += p.m_GivenName + " " + p.m_Surname + "\n";
                                    TextBox_StudentList.Text += p.m_StudentId.ToString() + ":";
                                }
                            }
                            else
                            {
                                TextBox_textTo.Text += i.Text.ToString() + "\n";
                                TextBox_GroupList.Text += i.Value.ToString() + ":";
                            }
                        }
                    }
                    break;
                case "Student":
                    foreach (ListItem i in ListBox_staff.Items)
                    {
                        if (i.Selected)
                        {
                            TextBox_textTo.Text += i.Text.ToString() + "\n";
                            TextBox_StudentList.Text += i.Value.ToString() + ":";
                        }
                    }
                    break;
            }
        }

        protected void Button_Clear_Click(object sender, EventArgs e)
        {
            TextBox_textTo.Text = ""; TextBox_GroupList.Text = ""; TextBox_StudentList.Text = "";
        }

        protected void Button_SendMessage_Click(object sender, EventArgs e)
        {
            DateTime t1 = Calendar2.SelectedDate;
            //so we send the message to the students indicated......
            //first who is it from...
            string s = "";
            Utility u = new Utility();
            /* 
                        string struser = Context.User.Identity.Name;
                        Guid personID = u.GetPersonID(struser, out s);
            #if DEBUG
                        personID = new Guid("20744211-d0f0-4e69-af84-020c1023dfda");//cc
            #endif
                        if (personID == Guid.Empty)
                        {
                            Server.Transfer("StartForm.aspx");
                        }
                        else
                        {
                            s = u.Get_StaffCode(personID);
                            s = u.Get_StaffID(s);
                        }
                        */

            s = u.GetsStaffIdfromRequest(Request).ToString();
            //first make a message...
            Message m1 = new Message();
            m1.Msg = u.CleanInvertedCommas(TextBox1.Text);
            m1.StaffId = new Guid(s);
            m1.ValidFrom = Calendar2.SelectedDate;
            m1.ValidUntil = DateTime.Now.AddDays(7);
            //m1.DocumentURL = TextBox_URL.Text;
            Guid Id = m1.Save();
            //now loop though the students and the groups....

            s = TextBox_StudentList.Text;
            string s1 = "";
            int i = 0;

            i = s.IndexOf(":");
            while (i > 0)
            {
                s1 = s.Substring(0, i);
                //s1 has student GUID
                MessageStudent mst1 = new MessageStudent();
                mst1.StudentId = new Guid(s1);
                mst1.MessageId = Id;
                mst1.Delivered = false;
                mst1.Save();
                s = s.Substring(i + 1);
                i = s.IndexOf(":");
            }

            s = TextBox_GroupList.Text;
            s1 = "";
            i = s.IndexOf(":");
            while (i > 0)
            {
                s1 = s.Substring(0, i);
                //s1 has Group GUID
                MessageGroup mst2 = new MessageGroup();
                mst2.GroupId = new Guid(s1);
                mst2.MessageId = Id;
                mst2.Delivered = false;
                mst2.Save();
                s = s.Substring(i + 1);
                i = s.IndexOf(":");
            }
            Server.Transfer("../content/StartForm.aspx");
        }

        protected void TextBox_textTo_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
