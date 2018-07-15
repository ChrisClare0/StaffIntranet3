using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StudentInformation
{
    public partial class PlainResponseForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string type = Request.QueryString["Type"];
                if (type == "MusicGroupChanges")
                {

                    string Group = Request.QueryString["Group"];
                    string time = Request.QueryString["Time"];
                    string s = "";
                    Cerval_Library.ScheduledPeriodRawList fred = new Cerval_Library.ScheduledPeriodRawList();
                    fred.Load_for_Group(new Guid(Group));
                    Cerval_Library.Group g1 = new Cerval_Library.Group();g1.Load(new Guid(Group));
                    Cerval_Library.Period period1 = new Cerval_Library.Period();
                    s = "<h2><p align=\"center\"> Future Scheduling for " + g1._GroupCode + "</p></h2>";
                    s += "<p  align=\"center\"><TABLE BORDER  class= \"TimetableTable\" > ";
                    s += "<TR><TD>From</TD><TD>To</TD><TD>Period</TD><TD>Time Slot</TD><TD>Staff</TD><TD>Room</TD></TR>";
                    foreach (Cerval_Library.ScheduledPeriodRaw r in fred.m_list)
                    {
                        s += "<TR>";
                        s += "<TD>" + r.ValidityStart.ToShortDateString() + "</TD>";
                        s += "<TD>" + r.ValidityEnd.ToShortDateString() + "</TD>";
                        period1.Load(r.PeriodId);
                        s += "<TD>" + period1.m_periodcode + "</TD>";
                        s += "<TD>" + period1.m_PeriodStart.ToShortTimeString() + "-" + period1.m_PeriodEnd.ToShortTimeString() + "</TD>";
                        Cerval_Library.SimpleStaff staff1 = new Cerval_Library.SimpleStaff(r.StaffId);
                        s += "<TD>" + staff1.m_StaffCode + "</TD>";
                        Cerval_Library.SimpleRoom room1 = new Cerval_Library.SimpleRoom(r.RoomId);
                        s += "<TD>" + room1.m_roomcode + "</TD></TR>";
                    }
                    s += "</TABLE></p>";
                    ServerContent.InnerHtml = s;
                }
            }

        }
    }
}
