using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace PhysicsBookings.content.Booking
{
    public partial class BookingSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Utility u = new Utility();
                Calendar1.SelectedDate = DateTime.Now;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            string[, ,] grid = new string[6, 30, 2];  int j1;
            for (int i = 0; i < 6; i++) { for (int j = 0; j < 30; j++) { grid[i, j, 0] = ""; } }
            //setup for 5 P labs....
            grid[0, 0, 0] = "P1"; grid[0, 1, 0] = "P2"; grid[0, 2, 0] = "P3"; grid[0, 3, 0] = "P4"; grid[0, 4, 0] = "P5";
            int n_rows = 5;
            string s = "";
            s = "<div><h3>Bookings for " + Calendar1.SelectedDate.ToShortDateString() + "</h3></div>";
            s += "<p  align=\"center\"><TABLE BORDER  class= \"ResultsTbl\" ><TR><TD>Room</TD> ";
            s += "<TD>Period 1</TD><TD>Period 2</TD><TD>Period 3</TD><TD>Period 4</TD><TD>Period 5</TD></tr>";
            PhysicsBookingList pblist1 = new PhysicsBookingList();
            pblist1.LoadList_Date(Calendar1.SelectedDate);
            if (pblist1.m_list.Count > 0)
            {
                foreach (PhysicsBooking p in pblist1.m_list)
                {
                    SimpleRoom room1 = new SimpleRoom(p.RoomId);
                    Period p1 = new Period(p.PeriodId);
                    int p2 = System.Convert.ToInt32(p1.m_periodcode);
                    SimpleStaff s1 = new SimpleStaff(p.StaffId);
                    j1 = -1;
                    for (int j = 0; j < n_rows; j++)
                    {
                        if (grid[0, j, 0] == room1.m_roomcode.Trim())
                        {
                            j1 = j; break;
                        }
                    }
                    if (j1 == -1) { grid[0, n_rows, 0] = room1.m_roomcode.Trim(); j1 = n_rows; n_rows++; }
                    grid[p2, j1, 0] = s1.m_StaffCode+":<br/>";
                    grid[p2, j1, 1] = p.BookingId.ToString();
                    PhysicsEquipmentItemList ff = new PhysicsEquipmentItemList();
                    ff.LoadList(p);
                    foreach (PhysicsEquipmentItem i in ff.m_list)
                    {
                        grid[p2, j1, 0] += i.EquipmentItemCode+",  ";
                    }


                }
                for (int j = 0; j < n_rows; j++)
                {
                    s += "<tr>";
                    for (int i = 0; i < 6; i++)
                    {
                        s += "<td>";
                        if (grid[i, j, 0] == "") { }
                        else
                        {
                            if (i != 0)
                            {
                                s += "<A HREF=\"DisplayBooking.aspx?Id=" + grid[i, j, 1] + " \">Booking</A>";
                                s += "<br/>staff: ";
                            }
                            s += grid[i, j, 0] + "<br/>";
                        }
                        s += "</td>";
                    }
                    s += "</tr>";
                }
                s += "</table><br/>";
            }
            else
            {
                s += "</table><br/>No Bookings on this date<br/>";
            }
            s += "</table>";
            servercontent.InnerHtml = s;
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
    }
}
