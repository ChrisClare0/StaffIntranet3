using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class EditMessageGroupX : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string GroupIDS = Request.Params.Get("GroupID");
                ViewState.Add("group", GroupIDS);
                Group g = new Group(); g.Load(new Guid(GroupIDS));
                string s = "Group Members for " + g._GroupName;
                content1.InnerHtml = s;
                SetUpGroupNames(GroupIDS);
                SetUpYearList("7");
            }
        }

        public void SetUpYearList(string year)
        {
            //SimpleStudentList sl1 = new SimpleStudentList("GroupCode LIKE '"+year+"%'");
            StudentYearList yl1 = new StudentYearList(ListBox_Names, year+"Year", DateTime.Now);
            //ListBox_Names.Items.Clear();
            //foreach (SimplePupil p in sl1._studentlist)
            //{
             //   ListItem l = new ListItem(p.m_GivenName + " " + p.m_Surname, p.m_StudentId.ToString());
            //    ListBox_Names.Items.Add(l);
           // }
        }

        public void SetUpGroupNames(string gp)
        {
            PupilGroupList pgl1 = new PupilGroupList();
            pgl1.AddToList(new Guid(gp), DateTime.Now);
            ListBox_SelectedNames.Items.Clear();
            foreach (SimplePupil p in pgl1.m_pupilllist)
            {
                System.Web.UI.WebControls.ListItem l = new ListItem(p.m_GivenName + " " + p.m_Surname, p.m_StudentId.ToString());
                ListBox_SelectedNames.Items.Add(l);
            }
        }

        protected void Button_Clear_Click(object sender, EventArgs e)
        {
            ListBox_SelectedNames.Items.Clear();
        }

        protected void Button_RemoveSelected_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListBox_SelectedNames.Items.Count; i++)
            {
                if (ListBox_SelectedNames.Items[i].Selected)
                {
                    ListBox_SelectedNames.Items.RemoveAt(i); i--;

                }
            }
        }

        protected void Button_Add_Click(object sender, EventArgs e)
        {
            foreach (ListItem l in ListBox_Names.Items)
            {
                if (l.Selected)
                {
                    ListItem l1 = new ListItem(l.Text, l.Value);
                    ListBox_SelectedNames.Items.Add(l1);
                }
                l.Selected = false;
            }
        }

        protected void Button_Save_Click(object sender, EventArgs e)
        {
            //get current group members
            StudentGroupMembershipList sgl1 = new StudentGroupMembershipList();
            string GroupIDS = (string)ViewState["group"];
            Guid g = new Guid(GroupIDS);
            sgl1.LoadList_Group(g, DateTime.Now);
            //now if not in list delete sgm or if in list delete from list;
            bool found = false;
            foreach (StudentGroupMembership sg in sgl1.m_list)
            {
                found = false;
                foreach (ListItem l in ListBox_SelectedNames.Items)
                {
                    if (sg.m_Studentid.ToString() == l.Value)
                    {
                        found = true;
                        ListBox_SelectedNames.Items.Remove(l); break;
                    }
                }
                if (!found)
                {
                    sg.Delete();
                }
            }
            //now make new ones....
            int y = DateTime.Now.Year;
            if (DateTime.Now.Month > 7) y++;



            foreach (ListItem l in ListBox_SelectedNames.Items)
            {
                if (l.Value != "")
                {
                    StudentGroupMembership sg = new StudentGroupMembership();
                    sg.m_Groupid = new Guid(GroupIDS);
                    sg.m_Studentid = new Guid(l.Value);
                    sg.m_ValidTo = new DateTime(y, 7, 31);
                    sg.m_ValidFrom = DateTime.Now.AddDays(-1);
                    sg.Save();
                    //remove duplicates....
                    for (int i = 0; i < ListBox_SelectedNames.Items.Count; i++)
                    {
                        if (l.Value == ListBox_SelectedNames.Items[i].Value)
                        {
                            ListBox_SelectedNames.Items[i].Value = "";
                        }
                    }
                }
            }
            ListBox_Names.Items.Clear();
            SetUpYearList(DropDownList_Years.SelectedValue);
            ListBox_SelectedNames.Items.Clear();
            SetUpGroupNames(GroupIDS);
        }

        protected void DropDownList_Years_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetUpYearList(DropDownList_Years.SelectedValue);
        }
    }
}
