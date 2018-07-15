using System;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content.NextTT
{
    public partial class FreeStaff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string file_name = Server.MapPath(@"~/App_Data/tt_data.ttd");
            Cerval_Configuration c;

            c = new Cerval_Configuration("StaffIntranet_TTD_TimetablePlanDate");
            DateTime date1 = new DateTime();
            try { date1 = System.Convert.ToDateTime(c.Value); }
            catch { date1 = DateTime.Now; }

            TTData tt1 = new TTData();
            c = new Cerval_Configuration("StaffIntranet_UseOldTTDfile");
            if (c.Value.ToUpper() == "TRUE")
            {
                tt1.Load(file_name, date1, date1, false, false, "");
            }
            else
            {
                tt1.Load_DB(date1); //load from database
            }
            string[] r = new string[255];
            string[] per = new string[5];
            per[0] = "1"; per[1] = "2"; per[2] = "3"; per[3] = "4"; per[4] = "5";
            int n = 0; string s1 = "";
            string s = "<p><TABLE BORDER  class= \"TimetableTable\"   ><tr><th>Day</th> <th>1</th> <th>2</th> <th>3</th> <th>4</th> <th>5</th> </tr>";
            //only going to do mon-fri  period 1,2,3,4,5
            string[] days = new string[5]; days[0] = "MO "; days[1] = "TU "; days[2] = "WE "; days[3] = "TH "; days[4] = "FR ";
            string ExcludeList = ",MAF,";
            for (int d = 0; d < 5; d++)
            {
                s += "<tr><td>" + days[d] + "</td>";
                for (int p = 0; p < 5; p++)
                {
                    s += "<td>";
                    n = tt1.FindFreeStaff(d, per[p], ref r);
                    for (int i = 0; i < n; i++)
                    {
                        if (r[i].Trim().Length > 0)
                        {
                            s1 = "," + r[i].Trim().ToUpper() + ",";
                            if (!ExcludeList.Contains(s1)) s += r[i] + ", ";
                        }
                    }
                    s += "</td>";
                }
                s += "</tr>";
            }
            s += "</table>";
            s += "<br/><p>The following staff are excluded in the search above:<br/>" + ExcludeList + "</p>";
            content.InnerHtml = s;
        }
    }
}