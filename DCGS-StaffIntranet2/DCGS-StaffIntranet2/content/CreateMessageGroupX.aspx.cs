using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class CreateMessageGroupX : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GroupList gl1 = new GroupList();
                Utility u = new Utility();
                //string staff_code = u.GetStaffCodefromContext(this.Context);
                string staff_code = u.GetsStaffCodefromRequest(Request);
                gl1.LoadList_StaffPrivateGroups(staff_code.Trim(), GroupList.GroupListOrder.GroupName);
                
                foreach (Group g in gl1._groups)
                {
                    string s2 = g._GroupName;
                    System.Web.UI.WebControls.ListItem l = new ListItem(s2,g._GroupID.ToString());
                    DropDownList_groups.Items.Add(l);
                }
            }
        }

        protected void Button_Edit_Click(object sender, EventArgs e)
        {
            string s = DropDownList_groups.SelectedItem.Text;
            string s2 = DropDownList_groups.SelectedItem.Value;

            s="EditmessageGroupX.aspx?GroupId="+s2+"&GroupName='"+s+"'";
            Server.Transfer(s);
        }

        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            string s = DropDownList_groups.SelectedItem.Text;
            string s2 = DropDownList_groups.SelectedItem.Value;
            Guid g1 = new Guid(s2);
            Group gp1 = new Group(); gp1.Load(g1);
            gp1.Delete();
            GroupList gl1 = new GroupList();
            Utility u = new Utility();
            //string staff_code = u.GetStaffCodefromContext(this.Context);
            string staff_code = u.GetsStaffCodefromRequest(Request);
            gl1.LoadList_StaffPrivateGroups(staff_code.Trim(), GroupList.GroupListOrder.GroupName);
            DropDownList_groups.Items.Clear();
            foreach (Group g in gl1._groups)
            {
                s2 = g._GroupName;
                System.Web.UI.WebControls.ListItem l = new ListItem(s2, g._GroupID.ToString());
                DropDownList_groups.Items.Add(l);
            }

        }

        protected void Button_MakeGroup_Click(object sender, EventArgs e)
        {
            string s = TextBox_GroupName.Text;
            Utility u = new Utility();
            //string s1 = u.GetStaffCodefromContext(this.Context);
            string s1 = u.GetsStaffCodefromRequest(Request);
            Guid g = new Guid(u.Get_StaffID(s1));

            GroupList gl1 = new GroupList();
            gl1.LoadList_StaffPrivateGroups(s1, GroupList.GroupListOrder.GroupName);

            string code = s1 + "_" + (gl1._groups.Count + 1).ToString();

            Group g1 = new Group();
            g1._CourseID = Cerval_Globals.newdawnCse;
            int y = DateTime.Now.Year;
            if (DateTime.Now.Month > 7) y++;
            g1._EndDate = new DateTime(y, 7, 31);
            g1._GroupCode = code;
            g1._GroupManagedBy = g;
            g1._GroupName = s;
            g1._GroupPrimaryAdministrative = false;
            g1._StartDate = DateTime.Now.AddDays(-2);
            g1.Save();

            //now need to insert into New Dawn stuff....

            Group g2 = new Group(); g2.Load("StaffGps", DateTime.Now);
            if (g2._valid)
            {
                GroupLink gll1 = new GroupLink();
                gll1.ParentId = g2._GroupID;
                gll1.ChildId = g1._GroupID;
                gll1.Save();
            }
            gl1._groups.Clear();
            //string staff_code = u.GetStaffCodefromContext(Context);
            string staff_code = u.GetsStaffCodefromRequest(Request);
            gl1.LoadList_StaffPrivateGroups(staff_code.Trim(), GroupList.GroupListOrder.GroupName);
            DropDownList_groups.Items.Clear();
            foreach (Group g3 in gl1._groups)
            {

                System.Web.UI.WebControls.ListItem l = new ListItem(g3._GroupName, g3._GroupID.ToString());
                DropDownList_groups.Items.Add(l);
            }

        }



    }
}
