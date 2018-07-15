using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.Data.SqlClient;


namespace DCGS_Staff_Intranet2.Exams
{
    public partial class TimetableRooms : System.Web.UI.Page
    {
        #region variables
        public int Year
        {   //this is the year in 2016 format
            get { try { return (int)ViewState["Year"]; } catch { return 0; } }
            set { ViewState["Year"] = value; }
        }
        public int YearCode
        {   //this is the year in 16 format
            get { try { return (int)ViewState["YearCode"]; } catch { return 0; } }
            set { ViewState["YearCode"] = value; }
        }
        public int SeasonCode
        {
            get { try { return (int)ViewState["SeasonCode"]; } catch { return 0; } }
            set { ViewState["SeasonCode"] = value; }
        }

        #endregion
        private void GetMasterValues()
        {
            SeasonCode = System.Convert.ToInt32((string)(Session["Season"]));
            Year = System.Convert.ToInt32((string)(Session["Year"]));
            YearCode = Year % 100;
            Label Label_Banner = (Label)Master.FindControl("Label_Banner");
            if (Label_Banner != null)
            {
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "  Rooms Dialog";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetMasterValues();
                string dateS = Request.QueryString["Date"];
                DateTime t0 = new DateTime();
                string sess = Request.QueryString["Session"];
                bool Is_AM = true;
                try
                {
                    t0 = Convert.ToDateTime(Request.QueryString["Date"]);
                    if (sess.Contains("PM")) Is_AM = false;
                    ViewState["Session_is_AM"] = Is_AM;
                    ViewState["EditDate"] = t0;
                }
                catch
                {
                    //try to get from session state?
                    Is_AM = (bool)ViewState["Session_is_AM"];
                    t0 = (DateTime)ViewState["EditDate"];
                }
                DateTime t1 = new DateTime();
                DateTime t2 = new DateTime();
                t1 = (Is_AM) ? t0.AddHours(8) : t0.AddHours(13);
                t2 = (Is_AM) ? t0.AddHours(13) : t0.AddHours(18);
                Label_Date.Text = t0.ToLongDateString();
                SetUp(t1, t2, Is_AM);
                ExamsUtility u = new ExamsUtility();
                bool b = u.AreRoomsMissing(t1, t2);
                if (!b) Label_message.Text = "All Rooms are Allocated.. you can run again if you wish";
                Encode en = new Encode();
                SqlDataSource3.SelectCommand = GetQueryStringRoomSummary(t1,t2, !b);
                SqlDataSource3.ConnectionString = en.GetDbConnection();
                SqlDataSource3.DataBind();
            }
        }

        protected void SetUp(DateTime t1,DateTime t2,bool Is_AM)
        {
            DateTime t3 = new DateTime();
            ScheduledComponentList scl1 = new ScheduledComponentList();
            scl1.LoadList_Date(t1, t2);//order by  DateTime
            t3 = new DateTime(2000, 1, 1); int nmax = 0; int n = 0;
            Guid st1 = new Guid(); st1 = Guid.Empty;
            foreach (ScheduledComponent sc in scl1.m_List)
            {
                if (t3 != sc.m_Date)
                {
                    t3 = sc.m_Date;
                    nmax = (n > nmax) ? n : nmax;
                    n = 0;
                }
                n++;
            }
            nmax = (n > nmax) ? n : nmax;
            //so max kids nmax //offer rooms

            Label_RoomsAllcation.Text = "Allocated: 0    Needed: " + nmax.ToString();
            ViewState["MaxDesksRequired"] = nmax;
            //setup list box
            ListBox_Rooms.Items.Clear();
            string s = "SELECT DISTINCT  dbo.tbl_Core_Rooms.RoomId AS Id, dbo.tbl_Core_Rooms.RoomCode AS Name ";
            s += " FROM  dbo.tbl_Core_Rooms INNER JOIN dbo.tbl_Exams_RoomLayouts ON dbo.tbl_Core_Rooms.RoomId = dbo.tbl_Exams_RoomLayouts.RoomId ";
            s += " ORDER BY dbo.tbl_Core_Rooms.RoomCode ";
            Encode en = new Encode();
            SqlDataSource2.SelectCommand = s;
            SqlDataSource2.ConnectionString = en.GetDbConnection();
            SqlDataSource2.DataBind();

            foreach(ListItem l in ListBox_Rooms.Items)
            {
                if(l.Text=="HAL")
                {
                    s = s;
                }
            }

            SetupRulesBoxesSources(t1,t2,Is_AM);

        }

        protected void ListBox_Rooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = 0;
            ListBox lb = (ListBox)sender;
            int nmax = (int)ViewState["MaxDesksRequired"];
            foreach (ListItem l in lb.Items)
            {
                if (l.Selected)
                {
                    ExamRoom er = new ExamRoom(); er.Load(new Guid(l.Value));
                    n += er.m_capacity;
                }
            }
            Label_RoomsAllcation.Text = "Allocated : " + n.ToString() + "  Needed: " + nmax.ToString();
            SetupRulesBoxesRooms();

        }

        protected void SetupRulesBoxesRooms()
        {
            DropDownList_Rule1Room.Items.Clear();
            DropDownList_Rule2Room.Items.Clear();
            DropDownList_Rule3Room.Items.Clear();
            DropDownList_Rule4Room.Items.Clear();
            DropDownList_Rule5Room.Items.Clear();
            ListBox lb = ListBox_Rooms;
            foreach (ListItem l in lb.Items)
            {
                if (l.Selected)
                {
                    ListItem l1 = new ListItem(l.Text, l.Value);
                    DropDownList_Rule1Room.Items.Add(l1);
                    DropDownList_Rule2Room.Items.Add(l1);
                    DropDownList_Rule3Room.Items.Add(l1);
                    DropDownList_Rule4Room.Items.Add(l1);
                    DropDownList_Rule5Room.Items.Add(l1);
                    if (l.Text.Trim() == "ST12")
                    {
                        DropDownList_Rule1Source.SelectedIndex = 1;//always typists
                        DropDownList_Rule1Room.SelectedValue = l1.Value;
                }
                }

            }
        }
        protected void SetupRulesBoxesSources(DateTime t1,DateTime t2, bool Is_AM)
        {
            DropDownList_Rule1Source.Items.Clear();
            DropDownList_Rule1Source.Items.Add("None");
            DropDownList_Rule1Source.Items.Add("Typists");
            DropDownList_Rule1Source.Items.Add("ExtraTime");
            DropDownList_Rule1Source.SelectedIndex = 0;

            DropDownList_Rule2Source.Items.Clear();
            DropDownList_Rule2Source.Items.Add("None");
            DropDownList_Rule2Source.Items.Add("Typists");
            DropDownList_Rule2Source.Items.Add("ExtraTime");
            DropDownList_Rule2Source.SelectedIndex = 0;

            DropDownList_Rule3Source.Items.Clear();
            DropDownList_Rule3Source.Items.Add("None");
            DropDownList_Rule3Source.Items.Add("Typists");
            DropDownList_Rule3Source.Items.Add("ExtraTime");
            DropDownList_Rule3Source.SelectedIndex = 0;

            DropDownList_Rule4Source.Items.Clear();
            DropDownList_Rule4Source.Items.Add("None");
            DropDownList_Rule4Source.Items.Add("Typists");
            DropDownList_Rule4Source.Items.Add("ExtraTime");
            DropDownList_Rule4Source.SelectedIndex = 0;

            DropDownList_Rule5Source.Items.Clear();
            DropDownList_Rule5Source.Items.Add("None");
            DropDownList_Rule5Source.Items.Add("Typists");
            DropDownList_Rule5Source.Items.Add("ExtraTime");
            DropDownList_Rule5Source.SelectedIndex = 0;



            string s = "SELECT DISTINCT  dbo.tbl_Exams_Components.ComponentCode, dbo.tbl_Exams_Components.ComponentID  ";
            s += " FROM dbo.tbl_Exams_ScheduledComponents INNER JOIN ";
            s += " dbo.tbl_Exams_Components ON dbo.tbl_Exams_ScheduledComponents.ComponentId = dbo.tbl_Exams_Components.ComponentID  ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += "ORDER BY ComponentCode ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ListItem l = new ListItem(dr.GetString(0), dr.GetGuid(1).ToString());
                            DropDownList_Rule1Source.Items.Add(l);
                            DropDownList_Rule2Source.Items.Add(l);
                            DropDownList_Rule3Source.Items.Add(l);
                            DropDownList_Rule4Source.Items.Add(l);
                            DropDownList_Rule5Source.Items.Add(l);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        protected void Button_Assign_Click(object sender, EventArgs e)
        {
            //now we are actually going to do it???
   
            string ErrorS = "Success!";
            bool Is_AM = (bool)ViewState["Session_is_AM"];
            DateTime t0 = new DateTime();
            t0 = (DateTime)ViewState["EditDate"];
            DateTime t1 = new DateTime();
            DateTime t2 = new DateTime();
            t1 = (Is_AM) ? t0.AddHours(8) : t0.AddHours(13);
            t2 = (Is_AM) ? t0.AddHours(13) : t0.AddHours(18);
            DateTime time_last = new DateTime();




            List<Guid> Rooms = new List<Guid>();
            ListBox lb = ListBox_Rooms;
            foreach (ListItem l in lb.Items)
            {
                if (l.Selected)
                {
                    Rooms.Add(new Guid(l.Value));
                }
            }

            //if no rooms allocated flag error
            if(Rooms.Count==0)
            {
                ErrorS = "...you haven't allocated Any rooms";
                goto ErrorExit;
            }


            ExamsUtility eu = new ExamsUtility();
            eu.ClearDeskAssignments(t1, t2);

            //ought to clear all room allocations first...
            //ought to do this by query
            int n = 0;
            ExamsUtility u = new ExamsUtility();
            u.ClearRoomAssignments(t1, t2);

            ExamRoom er = new ExamRoom();
            n = 0;
            List<int> Room_Capacity = new List<int>();
            foreach (Guid d in Rooms)
            {
                er.Load(d);
                Room_Capacity.Add(er.m_capacity);
                n++;
            }
            int nmax = n - 1;

            ScheduledComponentList scl1 = new ScheduledComponentList();
            scl1.LoadList_Date(t1, t2);//order by  DateTime
            time_last = ((ScheduledComponent)scl1.m_List[0]).m_Date;
            string s = GetQueryStringDay(t0.Year, t0.Month, t0.Day);// gets from sc ordered by datetime,NUMBER

            List<Guid> Components = new List<Guid>();
            List<int> number = new List<int>();
            List<DateTime> Start_Time = new List<DateTime>();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Components.Add(dr.GetGuid(1));
                            number.Add(dr.GetInt32(4));
                            Start_Time.Add(dr.GetDateTime(3));
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

            //first apply rules... hopefully these will fit ... if not flag error back to ui

            int CurrentRoomCapacity = 0;

            string s_rule = DropDownList_Rule1Source.SelectedItem.Text;
            Guid rule_roomId = new Guid(DropDownList_Rule1Room.SelectedValue);
            n = 0; foreach (Guid d in Rooms){if (d == rule_roomId){CurrentRoomCapacity = Room_Capacity[n];break;}}
            if (!Apply_Rule(s_rule, rule_roomId, scl1, ref ErrorS, Components, number, ref CurrentRoomCapacity))
            {
                goto ErrorExit;
            }
            Room_Capacity[n] = CurrentRoomCapacity;
            if ((s_rule != "None")&&(CheckBox1.Checked)) { Rooms.Remove(rule_roomId); nmax--; }


            s_rule = DropDownList_Rule2Source.SelectedItem.Text;
            rule_roomId = new Guid(DropDownList_Rule2Room.SelectedValue);
            n = 0; foreach (Guid d in Rooms) { if (d == rule_roomId) { CurrentRoomCapacity = Room_Capacity[n]; break; } }
            if (!Apply_Rule(s_rule, rule_roomId, scl1, ref ErrorS, Components, number,ref CurrentRoomCapacity))
            {
                goto ErrorExit;
            }
            if ((s_rule != "None") && (CheckBox2.Checked)) { Rooms.Remove(rule_roomId); nmax--; }
            Room_Capacity[n] = CurrentRoomCapacity;


            s_rule = DropDownList_Rule3Source.SelectedItem.Text;
            rule_roomId = new Guid(DropDownList_Rule3Room.SelectedValue);
            n = 0; foreach (Guid d in Rooms) { if (d == rule_roomId) { CurrentRoomCapacity = Room_Capacity[n]; break; } }
            if (!Apply_Rule(s_rule, rule_roomId, scl1, ref ErrorS, Components, number, ref CurrentRoomCapacity))
            {
                goto ErrorExit;
            }
            if ((s_rule != "None") && (CheckBox3.Checked)) { Rooms.Remove(rule_roomId); nmax--; }
            Room_Capacity[n] = CurrentRoomCapacity;



            s_rule = DropDownList_Rule4Source.SelectedItem.Text;
            rule_roomId = new Guid(DropDownList_Rule4Room.SelectedValue);
            n = 0; foreach (Guid d in Rooms) { if (d == rule_roomId) { CurrentRoomCapacity = Room_Capacity[n]; break; } }
            if (!Apply_Rule(s_rule, rule_roomId, scl1, ref ErrorS, Components, number, ref CurrentRoomCapacity))
            {
                goto ErrorExit;
            }
            if ((s_rule != "None") && (CheckBox4.Checked)) { Rooms.Remove(rule_roomId); nmax--; }
            Room_Capacity[n] = CurrentRoomCapacity;



            s_rule = DropDownList_Rule5Source.SelectedItem.Text;
            rule_roomId = new Guid(DropDownList_Rule5Room.SelectedValue);
            n = 0; foreach (Guid d in Rooms) { if (d == rule_roomId) { CurrentRoomCapacity = Room_Capacity[n]; break; } }
            if (!Apply_Rule(s_rule, rule_roomId, scl1, ref ErrorS, Components, number, ref CurrentRoomCapacity))
            {
                goto ErrorExit;
            }
            if ((s_rule != "None") && (CheckBox5.Checked)) { Rooms.Remove(rule_roomId); nmax--; }
            Room_Capacity[n] = CurrentRoomCapacity;


            ScheduledComponentList scl2 = new ScheduledComponentList();
            foreach (ScheduledComponent c in scl1.m_List)
            {
                if (c.m_RoomId == Guid.Empty)
                    scl2.m_List.Add(c);
            }

            //now we apply the rest 
            //idea is we find the sc with most students.... and put into biggest room first
            //first correct room capacity?
            n = 0;
            // well i think we have all the information....
            //going to assign biggest exam to biggest room first

            int index = 0;
            foreach (Guid d in Components)
            {
                //so new component....  
                //if there is a complete gap before we start this component we can clear room allocations....
                //find the first (earliest) scheduled component
                foreach (ScheduledComponent c in scl2.m_List)
                {
                    if (c.m_ComponentId == d)
                    {
                        if (c.m_RoomId == Guid.Empty)
                        {
                            if (c.m_Date > time_last.AddMinutes(10))
                            {
                                //so this one starts at least 10 minutes after all sofar have finished....
                                time_last = c.m_Date.AddMinutes(c.m_TimeAllowed);
                                //so can reset rooms..... ie back to full capacity?
                                Room_Capacity.Clear();n = 0;
                                foreach (Guid d1 in Rooms)
                                {
                                    er.Load(d1);
                                    Room_Capacity.Add(er.m_capacity);
                                    n++;
                                }
                                nmax = n - 1;
                            }
                        }
                    }
                }

                n = 0;
                while ((Room_Capacity[n] == 0) && (n <= nmax)) n++;
                if (n > nmax)
                {
                    ErrorS = "Ran out of room capacity !!"; goto ErrorExit;
                }
                //going to check if next one will fit in any room we have available... if so use it!  search smallest first?
                for (int i = nmax; i >= 0; i--)
                {
                    if (Room_Capacity[i] >= number[index])
                    {
                        n = i; break;
                    }
                }
                foreach (ScheduledComponent c in scl2.m_List)
                {
                    if (c.m_ComponentId == d)
                    {
                        if (c.m_RoomId == Guid.Empty)
                        {
                            if (c.m_Date.AddMinutes(c.m_TimeAllowed) > time_last)
                            {
                                time_last = c.m_Date.AddMinutes(c.m_TimeAllowed);
                            }

                            //really ought to check to see if he has any other componets in this session and assign the same room/...

                            foreach (ScheduledComponent c1 in scl1.m_List)
                            {
                                if (c1.m_StudentId == c.m_StudentId)
                                {
                                    c.m_RoomId = c1.m_RoomId;
                                }
                            }
                            if(c.m_RoomId==Guid.Empty)
                            {
                                c.m_RoomId = Rooms[n];
                                Room_Capacity[n]--;
                            }

                            c.Save();

 

                            foreach (ScheduledComponent c1 in scl2.m_List)
                            {

                                if ((c1.m_StudentId == c.m_StudentId)&&(c1.m_RoomId== Guid.Empty))
                                {
                                    c.m_RoomId = Rooms[n];
                                    c.Save();
                                    //Room_Capacity[n]--;   should already have cleared clashes 
                                }


                            }
                        }
                    }
                    while ((Room_Capacity[n] == 0) && (n < nmax)) n++;
                    if (n > nmax)
                    {
                        ErrorS = "Ran out of room capacity !!"; goto ErrorExit;
                    }
                }
                index++;
            }
            //check??
            //success;  open the summary panel
 
            Panel_left1.Visible = false; Panel_right1.Visible = false;
            Label_message.Text = "Rooms Allocated Successfully.";
            Label_message.Visible = true;
            SqlDataSource3.SelectCommand = GetQueryStringRoomSummary(t1,t2,true);
            SqlDataSource3.ConnectionString = en.GetDbConnection();
            SqlDataSource3.DataBind();
            return;

            ErrorExit: Label_message.Text = ErrorS;
            Label_message.BackColor = System.Drawing.Color.Red;
            return;

        }

        protected bool Apply_Rule(string rule, Guid rule_roomId, ScheduledComponentList scl1, ref string ErrorS, List<Guid> Components, List<int> number,ref int RoomCapacity)
        {
            int n = 0;
            ExamRoom er = new ExamRoom();
            switch (rule)
            {
                case "None": break;
                case "ExtraTime":
                    n = 0;
                    ExamComponent ec1 = new ExamComponent();int time1 = 0;
                    foreach (ScheduledComponent c in scl1.m_List)
                    {
                        ec1.Load(c.m_ComponentId);time1 = Convert.ToInt32(ec1.m_Time);
                        if ((c.m_TimeAllowed>time1) && (c.m_RoomId == Guid.Empty)) n++;
                    }
                    er.Load(rule_roomId);
                    if (n > er.m_capacity) { ErrorS = "Rule" + rule + " Exceeds capacity in" + DropDownList_Rule1Room.Text; return false; }
                    foreach (ScheduledComponent c in scl1.m_List)
                    {
                        ec1.Load(c.m_ComponentId); time1 = Convert.ToInt32(ec1.m_Time);
                        if ((c.m_TimeAllowed > time1) && (c.m_RoomId == Guid.Empty))
                        {
                            c.m_RoomId = rule_roomId;
                            c.Save(); n = 0; RoomCapacity--;
                            foreach (Guid d in Components)
                            {
                                if (d == c.m_ComponentId)
                                {
                                    number[n]--;
                                }
                                n++;
                            }
                        }
                    }
                    break;
                case "Typists":
                    n = 0;
                    foreach (ScheduledComponent c in scl1.m_List) { if ((c.m_Will_Type) && (c.m_RoomId == Guid.Empty)) n++; }
                    er.Load(rule_roomId);
                    if (n > er.m_capacity) { ErrorS = "Rule" + rule + " Exceeds capacity in" + DropDownList_Rule1Room.Text; return false; }
                    foreach (ScheduledComponent c in scl1.m_List)
                    {
                        if ((c.m_Will_Type)&&(c.m_RoomId==Guid.Empty))
                        {
                            c.m_RoomId = rule_roomId;
                            c.Save(); n = 0; RoomCapacity--;
                            foreach (Guid d in Components)
                            {
                                if (d == c.m_ComponentId)
                                {
                                    number[n]--;
                                }
                                n++;
                            }
                        }
                    }
                    break;
                default:
                    //it is a rule based on a component
                    foreach (ScheduledComponent c in scl1.m_List) { if ((c.m_ComponentCode == rule)&& (c.m_RoomId == Guid.Empty)) n++; }
                    er.Load(rule_roomId);
                    if (n > er.m_capacity) { ErrorS = "Rule" + rule + " Exceeds capacity in" + DropDownList_Rule1Room.Text; return false; }
                    foreach (ScheduledComponent c in scl1.m_List)
                    {
                        if ((c.m_ComponentCode == rule)&& (c.m_RoomId == Guid.Empty))
                        {
                            c.m_RoomId = rule_roomId;
                            c.Save(); n = 0; RoomCapacity--; 
                            foreach (Guid d in Components)
                            {
                                if (d == c.m_ComponentId)
                                {
                                    number[n]--;
                                }
                                n++;
                            }
                        }
                    }
                    break;
            }
            ErrorS += "Success_Rule1";
            return true;

        }


        protected string GetQueryStringDay(int year, int month, int day)
        {
            DateTime t0 = new DateTime(year, month, day, 8, 0, 0);
            DateTime t1 = new DateTime(year, month, day, 23, 0, 0);
            string s = " SELECT ComponentCode, ComponentID, ";
            s += " ComponentTitle, DateTime AS StartTime, ";
            s += "COUNT(ScheduledComponentID) AS Number ";
            s += " FROM  qry_Cerval_Exams_ScheduledComponents ";
            s += " GROUP BY DateTime, ComponentId,ComponentCode, ";
            s += " ComponentTitle, ComponentID ";
            s += " HAVING (DateTime > CONVERT(DATETIME, '" + t0.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))";
            s += " AND (DateTime < CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))";
            s += " ORDER BY DateTime, Number  DESC";
            return s;
        }

        protected string GetQueryStringRoomSummary(DateTime t1,DateTime t2,bool IncludeRoomCode)
        {

            string s = "SELECT dbo.tbl_Exams_ScheduledComponents.DateTime, dbo.tbl_Exams_Components.ComponentCode AS Code, ";
            s += " ComponentTitle, ";
            if (IncludeRoomCode)s+=" dbo.tbl_Core_Rooms.RoomCode AS Room, ";
            s += " COUNT(ScheduledComponentID) AS Number  ";
            s += " FROM dbo.tbl_Exams_ScheduledComponents INNER JOIN ";
            if(IncludeRoomCode)s += "  dbo.tbl_Core_Rooms ON dbo.tbl_Exams_ScheduledComponents.RoomId = dbo.tbl_Core_Rooms.RoomId INNER JOIN ";
            s += "  dbo.tbl_Exams_Components ON dbo.tbl_Exams_ScheduledComponents.ComponentId = dbo.tbl_Exams_Components.ComponentID ";
            s += " GROUP BY dbo.tbl_Exams_ScheduledComponents.Season, dbo.tbl_Exams_ScheduledComponents.Year, dbo.tbl_Exams_ScheduledComponents.ComponentId, ";
            if(IncludeRoomCode)s+=" dbo.tbl_Core_Rooms.RoomCode,  ";
            s += " dbo.tbl_Exams_ScheduledComponents.DateTime, dbo.tbl_Exams_Components.ComponentCode, dbo.tbl_Exams_Components.ComponentTitle ";
            s += " HAVING (dbo.tbl_Exams_ScheduledComponents.Season = '"+SeasonCode.ToString()+"') AND(dbo.tbl_Exams_ScheduledComponents.Year = '"+Year.ToString()+"') ";
            s += " AND (DateTime > CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            return s;
        }

        protected void Button_GotoDesks_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            bool Is_AM = (bool)ViewState["Session_is_AM"];
            if (Is_AM) Server.Transfer(@"../exams/TimetableDesks.aspx?Date=" + t0.ToShortDateString() + "&Session=AM");
            Server.Transfer(@"../exams/TimetableDesks.aspx?Date=" + t0.ToShortDateString() + "&Session=PM");
        }
    }
}