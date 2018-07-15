using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content.NextTT
{
    public partial class NextYearTTChoice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_TTD_AllowRoomEdit");
            string s = c.Value;
            if (!c.valid)//try revert to config file
            {
                System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
                s = ar.GetValue("TTD_AllowRoomEdit", typeof(string)).ToString();
            }
            if (s.ToUpper() == "TRUE")
            {
                MenuList.InnerHtml += "<a href =\"EditRooms.aspx\">Edit Rooms</a><br />";
            }

            c = new Cerval_Configuration("StaffIntranet_TTPlan_Title");
            Information.InnerHtml = c.Value + "<br />";

            c = new Cerval_Configuration("StaffIntranet_TTD_TimetablePlanDate");
            DateTime d = new DateTime();
            try
            { 
                d = System.Convert.ToDateTime(c.Value);

                Information.InnerHtml += "Timetable as at " + d.ToShortDateString() + "<br/> Please select a choice from the options below:<br /><br />";
            }
            catch { }
        }
    }
}
