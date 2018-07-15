using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace PhysicsBookings.content.Booking
{
    public partial class DisplayBooking : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PhysicsBooking pb1 = new PhysicsBooking();
                string s = Request.UrlReferrer.OriginalString;
                ViewState.Add("booking", pb1);
                ViewState.Add("ref", s);
                pb1.Load(Request.Params.Get("Id"));
                SimpleStaff staff1 = new SimpleStaff(pb1.StaffId);
                SimpleRoom room1 = new SimpleRoom(pb1.RoomId);
                Period p1 = new Period(pb1.PeriodId);
                s = "<h3>Booking for " + pb1.Date.ToShortDateString() + "   " + p1.m_periodname;
                s += "<br/>Staff:  " + staff1.m_StaffCode + "       Room: " + room1.m_roomcode + "</br>";
                s += "<p  align=\"center\"><TABLE BORDER  class= \"ResultsTbl\" ><TR><TD>Item</TD><TD>Location</TD></tr> ";
                PhysicsEquipmentItemList list1 = new PhysicsEquipmentItemList();list1.LoadList(pb1);
                foreach (PhysicsEquipmentItem i in list1.m_list)
                {
                    s += "<tr><td>" + i.EquipmentItemCode + "</td><td>" + i.EquipmentItemLocation + "</td></tr>";
                }
                s += "</table>";
                
                servercontent.InnerHtml = s+"<br/>Notes:<br/>"+pb1.Notes+"<br/>";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            PhysicsBooking pb1 = (PhysicsBooking)ViewState["booking"];
            pb1.Delete();
            string s = (string)ViewState["ref"];
            Response.Redirect(s);
        }
    }
}
