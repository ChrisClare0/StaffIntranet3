using System;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content.NextTT
{
    public partial class YearTimetables : System.Web.UI.Page
    {
        static int max_rows = 25;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetupList();
                Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_TTPlan_Title");
                Information.InnerHtml = c.Value + "<br/> <br/>Select the Year for the display and then press Display.";
            }
        }

        protected void SetupList()
        {
                ListBox_year.Items.Clear();
                for(int i = 7; i<14;i++)
                {
                    ListItem l = new ListItem(i.ToString(),i.ToString());
                    ListBox_year.Items.Add(l);
                }
        }

        protected string GenerateTimetable(string[,] data, string[] staff, int no_staff)
        {
            string s = "<p><TABLE BORDER  class= \"TimetableTable\"   ><tr><TD>Room</td>";
            //only going to do mon-fri  period 1,2,3,4,5
            string[] days = new string[5]; days[0] = "MO "; days[1] = "TU "; days[2] = "WE "; days[3] = "TH "; days[4] = "FR ";
            for (int d = 0; d < 5; d++)
            {
                for (int p = 1; p < 6; p++)
                {
                    s += "<td>" + days[d] + p.ToString() + "</td>";
                }
            }
            s += "</tr>";
            for (int r = 0; r < no_staff; r++)
            {
                s += "<tr>";
                s += "<td>" + staff[r] + "</td>";
                for (int d = 0; d < 5; d++)
                {
                    for (int p = 1; p < 6; p++)
                    {
                        s += "<td>" + data[p + d * 5, r] + "</td>";
                    }
                }
                s += "</tr>";
            }
            s += "</table></p>";
            return s;
        }


        protected void ButtonGenerateTT_Click(object sender, EventArgs e)
        {
            string file_name = Server.MapPath(@"~/App_Data/tt_data.ttd"); int p1 = 0;
            string[,] data = new string[30, max_rows]; //has the display data
            for (int i = 0; i < 30; i++) { for (int j = 0; j < max_rows; j++) { data[i, j] = ""; } }
            string[] staff = new string[max_rows];
            int no_staff = 0;
            ListBox_year.Visible = false;
            HeaderDiv.Visible = false;
            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_TTD_TimetablePlanDate");
            DateTime date1 = new DateTime();
            try { date1 = System.Convert.ToDateTime(c.Value); }
            catch { date1 = DateTime.Now; }

            TTData tt1 = new TTData();
            c = new Cerval_Configuration("StaffIntranet_UseOldTTDfile");
            if (c.Value.ToUpper() == "TRUE") tt1.Load(file_name, date1, date1, false, false, ""); else tt1.Load_DB(date1);
            int x = 0;
            foreach (TTData.TT_period t in tt1.periodlist1.m_list)
            {
                foreach (ListItem l in ListBox_year.Items)
                {
                    if (t.SetName.Substring(0, 2).ToUpper().Contains(l.Text.ToUpper().Trim()) && l.Selected)
                    {
                        //so we want this set....
                        try
                        {
                            p1 = System.Convert.ToInt16(t.PeriodCode);
                        }
                        catch
                        {
                            p1 = 0;//reg periods etc
                        }
                        if (p1 > 0)
                        {
                            x = t.DayNo * 5 + p1;
                            for (int i = 0; i < max_rows; i++)
                            {
                                if (data[x, i] == "")
                                {
                                    data[x, i] = t.SetName + "<br>" + t.StaffCode.Trim();
                                    if ((i + 1) > no_staff) no_staff++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            content0.InnerHtml = GenerateTimetable(data, staff, no_staff);
        }

    }
}
