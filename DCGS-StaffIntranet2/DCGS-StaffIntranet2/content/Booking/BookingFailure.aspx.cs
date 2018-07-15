using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace PhysicsBookings.content.Booking
{
    public partial class BookingFailure : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string s = Request.Params.Get("BookingId"); 
            ViewState.Add("Id", s);
            s = Request.Params.Get("EquipmentCode");
            s = "Not able to book " + s + " as is already booked for this period. ";
            string s1 = Request.Params.Get("ClashedBookingId");
            PhysicsBooking b = new PhysicsBooking(); b.Load(s1);
            Utility u = new Utility();
            s += "Booked by :"+u.Get_StaffCodefromStaffID(b.StaffId);
            ServerContent.InnerHtml = s;
        }
        protected void Button_continue_Click(object sender, EventArgs e)
        {     
            string s = "BookingList.aspx?BookingId="+ViewState["Id"];
            Response.Redirect(s);
        }
    }
}
