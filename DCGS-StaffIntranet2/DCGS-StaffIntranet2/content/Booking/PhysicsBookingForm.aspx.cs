using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace PhysicsBookings.content.Booking
{
    public partial class PhysicsBookingForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Utility u = new Utility();
                StaffList sl1 = new StaffList(); sl1.LoadList(DateTime.Now, true);
                foreach (SimpleStaff s in sl1.m_stafflist)
                {
                    DropDownList_staff.Items.Add(s.m_StaffCode);
                }
                TimetableControl1.type = TT_writer.TimetableType.Booking;
#if DEBUG
                TimetableControl1.BaseId = new Guid(u.Get_StaffID("CC"));
                if (u.CheckStaffInConfigGroup("CC", "PhysicsExptSupervisor"))
                {
                    Label_ActAs.Visible = true; DropDownList_staff.Visible = true;
                }
#else
                TimetableControl1.BaseId = u.GetsStaffIdfromRequest(Request);
            //TimetableControl1.BaseId = u.GetStaffIDfromContext(Context);
            if(u.CheckStaffInConfigGroup(Context, "PhysicsExptSupervisor"))
            {
                Label_ActAs.Visible = true; DropDownList_staff.Visible = true;
            }

#endif
            }
        }

        protected void DropDownList_staff_SelectedIndexChanged(object sender, EventArgs e)
        {
            Utility u = new Utility();
            TimetableControl1.BaseId = new Guid(u.Get_StaffID(DropDownList_staff.SelectedValue));
        }
    }
}
