using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class Admin_Timetables : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StaffList sl1 = new StaffList(); sl1.LoadList(DateTime.Now, true);
            foreach (SimpleStaff s in sl1.m_stafflist)
            {
                ListItem l = new ListItem(s.m_StaffCode+" : "+s.m_PersonGivenName+" "+s.m_PersonSurname,s.m_StaffId.ToString());
                ListBox_staff.Items.Add(l);
            }
        }

        protected void ListBox_staff_SelectedIndexChanged(object sender, EventArgs e)
        {
            TT_writer t1 = new TT_writer();
            List<Guid> a = new List<Guid>();
            foreach (ListItem l in ListBox_staff.Items)
            {
                if (l.Selected)a.Add(new Guid(l.Value));
            }
            fred.InnerHtml = t1.OutputTT_ShowFree(a, DateTime.Now);

        }
    }
}