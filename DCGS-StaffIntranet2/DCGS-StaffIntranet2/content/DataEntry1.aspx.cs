using System;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class DataEntry1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DDList_Sets.Items.Clear();
            //get sets for this user..
            Utility u = new Utility();
            string staff_code = u.GetsStaffCodefromRequest(Request);
#if DEBUG
            staff_code = "CC";
#endif

            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_Exam_Results_Years");
            string s = c.Value;
            //s contains the years we want to process as comma sep list
            string[] s1 = new string[1]; s1[0] = ",";
            string[] s2 = new string[10];
            s2 = s.Split(s1, StringSplitOptions.None);
            GroupList gl1 = new GroupList();
            gl1.LoadStaff(staff_code, DateTime.Now, GroupList.GroupListOrder.GroupName);
            foreach (Group g in gl1._groups)
            {
                s = g._GroupID.ToString();
                foreach (string s3 in s2)
                {
                    if (g._GroupCode.StartsWith(s3))DDList_Sets.Items.Add(new ListItem(g._GroupCode, s));
                }
            }
            if (DDList_Sets.Items.Count > 0) DDList_Sets.SelectedIndex = 0;
        }

        protected void ButtonWriteSS_Click(object sender, EventArgs e)
        {
            ListItem l = DDList_Sets.SelectedItem;
            
        }
    }
}