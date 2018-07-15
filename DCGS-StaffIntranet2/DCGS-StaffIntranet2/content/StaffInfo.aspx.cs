using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class StaffInfo : System.Web.UI.Page
    {
        public string Type
        {
            get { return (((String)ViewState["Type"] == null) ? String.Empty : (String)ViewState["Type"]); }
            set { ViewState["Type"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Count >= 1)
                {
                    Type = Request.QueryString["Type"];
                    BuildNamesList(Type);
                }
            }

        }
        private void BuildNamesList(string type)
        {
            NameList.Items.Clear();
            if (type == "staff")
            {
                StaffList stlist1 = new StaffList(); stlist1.LoadList(DateTime.Now,true);
                foreach (SimpleStaff s in stlist1.m_stafflist) { ListItem l = new ListItem(s.m_PersonGivenName + " " + s.m_PersonSurname+"-"+s.m_StaffCode, s.m_StaffId.ToString()); NameList.Items.Add(l);}
                Label_Year.Text = "Type to search on name or code.";
                TextBox_mask.Visible = true;
                Display_List.Items.FindByValue("Details").Enabled = true;
                Display_List.Items.FindByValue("Incidents Authored by").Enabled = true;

            }
            if (type == "room")
            {
                RoomList rmlist1 = new RoomList(); rmlist1.LoadList();
                foreach (SimpleRoom r in rmlist1.m_roomlist) { ListItem l = new ListItem(r.m_roomcode, r.m_RoomID.ToString()); NameList.Items.Add(l); }
                Label_Year.Text = "Rooms";
                RadioButtonList_StaffType.Visible = false;
            }
            
            if (type == "Currentstaff")
            {
                StaffList stlist1 = new StaffList(); stlist1.LoadList(DateTime.Now, false);
                foreach (SimpleStaff s in stlist1.m_stafflist) { ListItem l = new ListItem(s.m_PersonGivenName + " " + s.m_PersonSurname + "-" + s.m_StaffCode, s.m_StaffId.ToString()); NameList.Items.Add(l); }
                Label_Year.Text = "Type to search on name or code.";
                TextBox_mask.Visible = true;
                Display_List.Items.FindByValue("Details").Enabled = true;
                Display_List.Items.FindByValue("Incidents Authored by").Enabled = true;
            }

            if (type == "Allstaff")
            {
                StaffList stlist1 = new StaffList(); stlist1.LoadFullList();
                foreach (SimpleStaff s in stlist1.m_stafflist) { ListItem l = new ListItem(s.m_PersonGivenName + " " + s.m_PersonSurname + "-" + s.m_StaffCode, s.m_StaffId.ToString()); NameList.Items.Add(l); }
                Label_Year.Text = "Type to search on name or code.";
                TextBox_mask.Visible = true;
                Display_List.Items.FindByValue("Details").Enabled = true;
                Display_List.Items.FindByValue("Incidents Authored by").Enabled = true;
            }
        }

        private void Display()
        {
            if (NameList.SelectedItem == null) return;

            string Name = NameList.SelectedItem.Text;
            string Id = NameList.SelectedItem.Value;
            GroupIncidentControl1.GroupCode = "";
            GroupIncidentControl1.StaffCode = Id;
            TimetableControl1.BaseId = new Guid(NameList.SelectedValue);
            if (Type=="staff")TimetableControl1.type = TT_writer.TimetableType.Staff;
            if (Type == "room") TimetableControl1.type = TT_writer.TimetableType.Room;
            TimetableControl1.Visible = Display_List.Items.FindByValue("Timetable").Selected;
            GroupIncidentControl1.Visible= Display_List.Items.FindByValue("Incidents Authored by").Selected;
            if (Display_List.Items.FindByValue("Details").Selected)
            {
                bool See_Private = false;
                Utility u = new Utility();
                if (u.CheckStaffInConfigGroup(Context, "StaffDetailsAccess"))
                {
                    See_Private = true;
                }
                SimpleStaff s1 = new SimpleStaff(TimetableControl1.BaseId);
                string s = s1.m_PersonId.ToString();
                PersonDetails p1 = new PersonDetails(s);
                s = "<H2>" + p1.m_Title + " " + p1.m_GivenName + " " + p1.m_Surname + "</h2>";
                ContactList cl1 = new ContactList();
                cl1.LoadList(s1.m_PersonId.ToString());
                foreach (Contact c in cl1.m_contactlist)
                {
                    if ((!c.m_Contact_Private) || (See_Private))
                    {
                        s += "<h3>" + c.m_ContactType + "      :      " + c.m_Contact_Value + "<br></h3>";
                    }
                }
                if (See_Private)
                {
                    s += "<p>Address : " + p1.m_address + "</p>";
                    s += "<p>doB:   " + p1.m_dob.ToShortDateString() + "</p>";
                }
                content0.InnerHtml = s;
            }
            else content0.InnerHtml = ""; 
        }

        protected void NameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Display();
        }
        protected void Display_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            Display();
        }

        protected void TextBox_mask_TextChanged(object sender, EventArgs e)
        {
            NameList.Items.Clear();

            StaffList stlist1 = new StaffList(); stlist1.LoadList(DateTime.Now, true);
            foreach (SimpleStaff s in stlist1.m_stafflist)
            {
                if ((s.m_PersonSurname.ToUpper().Contains(TextBox_mask.Text.ToUpper()) || (s.m_PersonGivenName.ToUpper().Contains(TextBox_mask.Text.ToUpper()))) || s.m_StaffCode.ToUpper().Contains(TextBox_mask.Text.ToUpper()))
                {
                    ListItem l = new ListItem(s.m_PersonGivenName + " " + s.m_PersonSurname + "-" + s.m_StaffCode, s.m_StaffId.ToString()); NameList.Items.Add(l);
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

        protected void RadioButtonList_StaffType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildNamesList(RadioButtonList_StaffType.SelectedValue);
            Display();
        }
    }
}
