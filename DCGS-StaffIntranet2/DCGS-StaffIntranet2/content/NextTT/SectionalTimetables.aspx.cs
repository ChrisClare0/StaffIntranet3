using System;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class SectionalTimetables : System.Web.UI.Page
    {
        static int max_rows = 250;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RadioButtonList1.SelectedIndex = 0;
                SetupList();
            }
        }

        protected void SetupList()
        {
            if (RadioButtonList1.SelectedValue == "Staff")
            {
                StaffList stl1 = new StaffList();stl1.LoadList(DateTime.Now, true);
                ListBox_staff.Items.Clear();
                foreach (SimpleStaff s1 in stl1.m_stafflist)
                {
                    ListItem l = new ListItem(s1.m_StaffCode, s1.m_StaffId.ToString());
                    ListBox_staff.Items.Add(l);
                }

            }
            if(RadioButtonList1.SelectedValue == "Subjects")
            {
                ListBox_staff.Items.Clear();
                CourseList cl1 = new CourseList(5);
                foreach (Course c in cl1._courses)
                {
                    ListItem l = new ListItem(c.CourseCode,c._CourseID.ToString());
                    ListBox_staff.Items.Add(l);
                }
            }
            if (RadioButtonList1.SelectedValue == "All")
            {
                StaffList stl1 = new StaffList(); stl1.LoadList(DateTime.Now, true);
                ListBox_staff.Items.Clear();
                foreach (SimpleStaff s1 in stl1.m_stafflist)
                {
                    ListItem l = new ListItem(s1.m_StaffCode, s1.m_StaffId.ToString());
                    ListBox_staff.Items.Add(l); l.Selected = true;
                }
            }
        }

        protected void ButtonGenerateTT_Click(object sender, EventArgs e)
        {
            string s = "";
            string file_name = Server.MapPath(@"~/App_Data/tt_data.ttd"); int p1 = 0;
            string[,] data = new string[30, max_rows]; //has the display data
            string[] staff = new string[max_rows];
            int no_staff = 0;
            ListBox_staff.Visible = false;
            RadioButtonList1.Visible = false;
            HeaderDiv.Visible = false;
            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_TTD_TimetablePlanDate");
            DateTime date1 = new DateTime();
            try { date1 = System.Convert.ToDateTime(c.Value); }
            catch { date1 = DateTime.Now; }

            TTData tt1 = new TTData();
            c = new Cerval_Configuration("StaffIntranet_UseOldTTDfile");
            if (c.Value.ToUpper() == "TRUE")tt1.Load(file_name, date1, date1, false, false, "");else tt1.Load_DB(date1); 

            if (RadioButtonList1.SelectedValue == "Staff")
            {
                foreach (ListItem l in ListBox_staff.Items)
                {
                    if (l.Selected)
                    {
                        s = l.Text;
                        staff[no_staff] = l.Text.Trim().ToUpper(); no_staff++;
                    }
                }
            }

            if (RadioButtonList1.SelectedValue == "Subjects")
            {
                bool found = false;
                foreach (TTData.TT_period t in tt1.periodlist1.m_list)
                {
                    foreach (ListItem l in ListBox_staff.Items)
                    {
                        if (t.SetName.ToUpper().Contains(l.Text.ToUpper().Trim()) && l.Selected)
                        {
                            found = false;
                            s = t.StaffCode.ToUpper().Trim();
                            for (int r = 0; r < no_staff; r++)
                            {
                                if (staff[r] == s)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                staff[no_staff] = s; no_staff++;
                            }
                        }
                    }
                }

            }
            if (RadioButtonList1.SelectedValue == "All")
            {
                foreach (ListItem l in ListBox_staff.Items)
                {
                    if (l.Selected)
                    {
                        s = l.Text;
                        staff[no_staff] = l.Text.Trim().ToUpper(); no_staff++;
                    }
                }
            }

            foreach (TTData.TT_period t in tt1.periodlist1.m_list)
            {
                for (int r = 0; r < no_staff; r++)
                {
                    if (t.StaffCode.Trim().ToUpper() == staff[r])
                    {
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
                            data[t.DayNo * 5 + p1, r] = t.SetName + "<br>" + t.RoomCode;
                        }
                    }
                }
            }
            content0.InnerHtml = GenerateTimetable(data, staff, no_staff);
        }

        protected string GenerateTimetable(string[,] data, string[] staff, int no_staff)
        {
            string s = "<p><table BORDER  class= \"TimetableTable\"   ><tr><td>Staff</td>";
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

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupList();
        }
    }
}
