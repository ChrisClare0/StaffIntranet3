using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class SimpleLists : System.Web.UI.Page
    {
        public string ListType
        {
            get { return (((String)ViewState["Type"] == null) ? String.Empty : (String)ViewState["Type"]); }
            set { ViewState["Type"] = value; }
        }
        public string Year
        {
            get { return (((String)ViewState["Year"] == null) ? String.Empty : (String)ViewState["Year"]); }
            set { ViewState["Year"] = value; }
        }
        public string group1
        {
            get { return (((String)ViewState["group1"] == null) ? String.Empty : (String)ViewState["group1"]); }
            set { ViewState["group1"] = value; }
        }
        public DateTime time1
        {
            get { return (((DateTime)ViewState["time1"] == null) ? DateTime.Now : (DateTime)ViewState["time1"]); }
            set { ViewState["time1"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                foreach (GroupListControl.DisplayItem d in GroupListControl1.DisplayItems)
                {
                    ListItem l = new ListItem(d._name.ToString(), d._name.ToString(), true);
                    l.Selected = d._display;
                    Display_List.Items.Add(l); 
                }
                Display_List.RepeatColumns = 2;
                TextBox_ListDate.Text = DateTime.Now.ToShortDateString();

                if (Request.QueryString.Count >= 1)
                {
                    Year = Request.QueryString["Year"];
                    ListType = Request.QueryString["Type"]; 
                    group1 = Request.QueryString["Group"];
                    string s = Request.QueryString["Time"];
                    time1 = DateTime.Now;
                    if (s != null)
                    {
                        try
                        {
                            time1 = System.Convert.ToDateTime(Request.QueryString["Time"]);
                        }
                        catch
                        {
                            time1 = DateTime.Now;
                        }
                    }
                    TextBox_ListDate.Text = time1.ToShortDateString();
                    Build_GroupList(ListType, Year,group1);
                }
            }
        }

        private void Build_GroupList(string type, string year,string group1)
        {
            GroupListBox.Items.Clear();
            DateTime ListDate = new DateTime(); ListDate = System.Convert.ToDateTime(TextBox_ListDate.Text);
            if (type == "Group")
            {
                Group g = new Group(); g.Load(group1, ListDate);
                ListItem l = new ListItem(g._GroupCode, g._GroupID.ToString());
                GroupListBox.Items.Add(l);
                l.Selected = true;
                Label_Groups.Text = "";
                Display();
                return;
            }
            if (Year == "0")
            {
                //need to do mask... soo nothing...
                TextBox_mask.Visible = true;
                GroupListBox.Visible = false;
                Label_Groups.Text = "Type mask and return";
                Display_List.Visible = false;
                GroupListControl1.Groups.Clear(); return;
            }
            bool add = false;
            string user = ""; PupilPeriodList ppl = new PupilPeriodList();
            if (type == "User")
            {
                Utility u = new Utility();
                //user = u.GetStaffCodefromContext(Context);
                user = u.GetsStaffCodefromRequest(Request);
#if DEBUG  
                user = "CC";
#endif
                ppl.LoadList("StaffCode", user, false, ListDate);
            }

            
            GroupList gl1 = new GroupList();
            gl1.LoadList_NonNewDawnOnly(ListDate, GroupList.GroupListOrder.GroupName);
            foreach(Group g in gl1._groups)
            {
                add = false;
                switch (type)
                {
                    case "Year": if (g._GroupCode.StartsWith(year) || (year == "All")) add = true; break;
                    case "User": foreach (ScheduledPeriod p in ppl.m_pupilTTlist) { if (p.m_groupcode == g._GroupCode) { add = true; break; } }; break;
                    default:       break;
                }
                if (add)
                {
                    ListItem l = new ListItem(g._GroupCode, g._GroupID.ToString());
                    GroupListBox.Items.Add(l);
                }
            }
            switch (type)
            {
                case "Year": Label_Groups.Text = "Year" + Year; break;
                case "User": Label_Groups.Text = "Groups for " + user; break;
            }
        }

        private void Display()
        {
            GroupListControl1.ListDate = System.Convert.ToDateTime(TextBox_ListDate.Text);
            foreach (ListItem l in Display_List.Items)
            {
                foreach (GroupListControl.DisplayItem d in GroupListControl1.DisplayItems)
                {
                    if (l.Text == d._name.ToString()) d._display = l.Selected;
                }
            }
            foreach (int i in  GroupListBox.GetSelectedIndices())
            {
                GroupListControl1.Groups.Add(new Cerval_Library.Listitem(GroupListBox.Items[i].Text,new Guid(GroupListBox.Items[i].Value)));
            }
            if (GroupListBox.SelectedIndex >= 0)
            {
                HyperLinks.InnerHtml = "<a href=\"PlainResponseForm.aspx?Type=PhotoOnly&GroupId=" + GroupListBox.SelectedItem.Value + "&GroupName=" + GroupListBox.SelectedItem.Text + "&ListDate=" + TextBox_ListDate.Text + "\">PhotoOnly</a><br>";
                HyperLinks.InnerHtml += "<a href=\"PlainResponseForm.aspx?Type=FullAddressList&GroupId=" + GroupListBox.SelectedItem.Value + "&GroupName=" + GroupListBox.SelectedItem.Text + "&ListDate=" + TextBox_ListDate.Text + " \">Full Address Grid</a><br>";
                HyperLinks.InnerHtml += "<a href=\"PlainResponseForm.aspx?Type=GroupIncidents&GroupId=" + GroupListBox.SelectedItem.Value + "&GroupName=" + GroupListBox.SelectedItem.Text + "\">Group Incidents</a><br>";
                string s3 = GroupListBox.SelectedItem.Text;
                if (GroupListBox.SelectedItem.Text.Contains("YR") || GroupListBox.SelectedItem.Text.Contains("RG"))
                {
                    HyperLinks.InnerHtml += "<a href=\"PlainResponseForm.aspx?Type=TargetOutput&GroupId=" + GroupListBox.SelectedItem.Value + "&GroupName=" + GroupListBox.SelectedItem.Text + "&ListDate=" + TextBox_ListDate.Text +  "&TimeWarning=true  \">Progress Summary</a><br>";
                }
            }

            GroupListControl1.save(); 
        }

        protected void GroupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Display();
        }

        protected void DisableListItemsExcept(ListItem l0)
        {
            foreach (ListItem l in Display_List.Items)
            {
                l.Enabled = false;
            }
            l0.Selected = false; l0.Enabled = true;
        }

        protected void Display_List_SelectedIndexChanged(object sender, EventArgs e)
        {

            Display();
        }

        protected void TextBox_ListDate_TextChanged(object sender, EventArgs e)
        {
            Build_GroupList(ListType, Year, group1);
        }

        protected void TextBox_mask_TextChanged(object sender, EventArgs e)
        {
            GroupListBox.Items.Clear();
            GroupList gl1 = new GroupList();
            gl1.LoadList_NonNewDawnOnly(DateTime.Now, GroupList.GroupListOrder.GroupName);
            foreach (Group g in gl1._groups)
            {
                if (g._GroupCode.ToUpper().Contains(TextBox_mask.Text.ToUpper()))
                {
                    ListItem l = new ListItem(g._GroupCode, g._GroupID.ToString());
                    GroupListBox.Items.Add(l);
                }
            }
            if (GroupListBox.Items.Count > 0)
            {
                //GroupListBox.Items[0].Selected = true;
                GroupListBox.Visible = true;
                Display_List.Visible = true;
                Display();
            }
        }

        protected void Button_Email_Click(object sender, EventArgs e)
        {

        }
    }
}
