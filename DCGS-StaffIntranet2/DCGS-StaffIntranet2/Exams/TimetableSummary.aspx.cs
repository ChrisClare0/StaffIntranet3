using System;
using Cerval_Library;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class TimetableSummary : System.Web.UI.Page
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "    Timetable Summary";
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            GetMasterValues();
            SetupSummaryTable();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
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
                    DateTime t1 = new DateTime();
                    DateTime t2 = new DateTime();
                    t1 = (Is_AM) ? t0.AddHours(8) : t0.AddHours(13);
                    t2 = (Is_AM) ? t0.AddHours(13) : t0.AddHours(18);
                    Panel_DayView.Visible = true;
                    Label_Date.Text = t0.ToLongDateString();
                    Encode en = new Encode();
                    string orderbyclause = " ORDER BY DateTime";
                    SqlDataSource1.SelectCommand = GetQueryStringDay(Year, t0.Month, t0.Day, orderbyclause);
                    SqlDataSource1.ConnectionString = en.GetDbConnection();
                    SqlDataSource1.DataBind();
                    UpdateButtons(t0);

                }
                catch
                {

                }

            }
        }

        protected void SetupSummaryTable()
        {
            DateTime t1 = new DateTime(Year, SeasonCode-2, 1);//so summer season might start in april???
            DateTime t0 = new DateTime();t0 = t1;
            //advance to Monday
            while (t0.DayOfWeek != DayOfWeek.Monday)
            {
                t0=t0.AddDays(1);
            }
            //this is our start date...
            string[] data = new string[120];//should be enough!
            for (int i = 0; i < 120; i++) data[i] = "";
            
            int n = 0;int nmin = 200;int nmax = -1;
            // list for the contents of this row...
            DateTime t2 = new DateTime();
            TimeSpan tspan1 = new TimeSpan();
            TimeSpan tspanWeek = new TimeSpan(0, 1, 0, 0);
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = GetQueryString(Year.ToString(), SeasonCode);
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            t2 = dr.GetDateTime(0);
                            tspan1 = t2 - t0;
                            n = tspan1.Days;
                            data[n] += dr.GetString(2) + ":" + dr.GetString(1) + ":" + dr.GetGuid(3).ToString() + (char)0x09;
                            //component title:option code
                            if (n > nmax) nmax = n;
                            if (n < nmin)nmin = n;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            ViewState["data"] = data;ViewState["nmax"] = nmax;ViewState["nmin"] = nmin;
            //so want to do whole weeks from t0;
            n = 0;while ((nmin - n) > 7) n += 7;
            //so we start the table at n...
            t2 = t0.AddDays(n);
            while (n < nmax)
            {
                TableRow r1 = new TableRow(); Table_Summary.Rows.Add(r1);
                TableCell c0 = new TableCell();c0.Text = t2.ToShortDateString();r1.Cells.Add(c0);
                for (int i = 0; i < 5; i++)
                {
                    TableCell c1 = new TableCell();
                    if (data[n + i] != "")
                    {
                        //Label l1 = new Label(); l1.Text = t2.ToString("M");
                        //c1.Controls.Add(l1);
                        Button b1 = new Button();
                        b1.Text= t2.ToString("M");
                        //b1.Text = "Edit";
                        c1.Controls.Add(b1);
                        b1.Click += B1_Click;
                        b1.ID = "Button" + (n + i).ToString();
                        if (!AllDesksAssigned(t2)) b1.BackColor = System.Drawing.Color.AntiqueWhite;
                    }
                    r1.Cells.Add(c1);t2=t2.AddDays(1);
                }
                n += 7; t2=t2.AddDays(2);
            }

        }

        protected bool AllDesksAssigned(DateTime t)
        {
            bool assigned = false;
            string s = "SELECT ScheduledComponentID ";
            s += " FROM dbo.tbl_Exams_ScheduledComponents ";
            s += " WHERE  (Desk IS NULL) AND (Year = '"+Year.ToString()+"') AND (Season = '"+SeasonCode.ToString()+"')";
            s += " AND (DateTime > CONVERT(DATETIME, '" + t.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))";
            s += " AND (DateTime < CONVERT(DATETIME, '" + t.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "', 102))";
            Encode en = new Encode();
            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            assigned = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return assigned;
        }

        private void B1_Click(object sender, EventArgs e)///button on summary grid
        {
            Button b1 = (Button)sender;
            string s = b1.ID;
            string s3 = b1.Text+ " "+Year.ToString();  //has date?
            DateTime t0 = new DateTime();t0 = System.Convert.ToDateTime(s3);
            int i = System.Convert.ToInt16(s.Substring(6));
            ViewState["EditDate"] = t0; ViewState["EditIndex"] = i;
            Panel_DayView.Visible = true;
            Label_Date.Text = t0.ToLongDateString();
            Encode en = new Encode();
            string orderbyclause = " ORDER BY DateTime";
            SqlDataSource1.SelectCommand = GetQueryStringDay(Year, t0.Month, t0.Day, orderbyclause);
            SqlDataSource1.ConnectionString = en.GetDbConnection();
            SqlDataSource1.DataBind();
            UpdateButtons(t0);
        }

        protected void UpdateButtons(DateTime t0)
        {
            ExamsUtility u = new ExamsUtility();
            DateTime t1 = new DateTime(); t1 = t0.AddHours(8).AddMinutes(39);//start at 8:40
            DateTime t2 = new DateTime(); t2 = t0.AddHours(18).AddMinutes(39);//end at 18:39
            Button1.Visible = false;
            Label_message.Visible = true;
            Label_message.Text = "There are NO clashes!";
            if (u.AreClashes(t1, t2))
            {

                Label_message.Text = "There are clashes to resolve";
                Button1.Visible = true;
            }

        }




        #region SQL Query String
        protected string GetQueryString(string Year, int SeasonCode)
        {
            ExamConversions u = new ExamConversions();
            string s = " SELECT DISTINCT dbo.tbl_Exams_ScheduledComponents.DateTime  ";
            s += ", dbo.tbl_Exams_Options.OptionCode, ";
            s += " dbo.tbl_Exams_Components.ComponentTitle, ";
            s += " dbo.tbl_Exams_ScheduledComponents.ComponentId ";
            s += " FROM dbo.tbl_Exams_ScheduledComponents INNER JOIN ";
            s += " dbo.tbl_Exams_Components ON dbo.tbl_Exams_ScheduledComponents.ComponentId = dbo.tbl_Exams_Components.ComponentID INNER JOIN ";
            s += " dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID INNER JOIN  ";
            s += " dbo.tbl_Exams_Options ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Options.OptionID ";
            s += "  WHERE(dbo.tbl_Exams_ScheduledComponents.Year = '" + Year + "') ";
            s += "  AND(dbo.tbl_Exams_ScheduledComponents.Season = '" + u.GetSeasonCode(SeasonCode) + "')  ";
            s += "  ORDER BY dbo.tbl_Exams_ScheduledComponents.DateTime";
            return s;
        }

        protected string GetQueryStringDay(int year,int month,int day, string orderbyclause)
        {
            DateTime t0 = new DateTime(year, month, day, 8, 0, 0);
            DateTime t1 = new DateTime(year, month, day, 23, 0, 0);
            string s = " SELECT ComponentCode, ComponentID, ";
            s += " ComponentTitle, DateTime AS StartTime, TimeAllowed , ";
            s += "COUNT(ScheduledComponentID) AS Number ";
            s += " FROM  qry_Cerval_Exams_ScheduledComponents ";
            s += " GROUP BY DateTime, ComponentId,ComponentCode, ";
            s += " ComponentTitle, ComponentID , TimeAllowed ";
            s += " HAVING (DateTime > CONVERT(DATETIME, '" + t0.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))";
            s+=" AND (DateTime < CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))";
            s += " ORDER BY DateTime";
            return  s;
            s += " HAVING(MONTH(DateTime) = '" + month + "' ) ";
            s += " AND(YEAR(DateTime) = '" + Year + "') ";
            s += "AND ( DAY(DateTime ) = '" + day + "'  ) ";
            s += orderbyclause;

            return s;
        }
        #endregion

        #region ResolveClashes
        protected void CheckClashes_Click(object sender, EventArgs e)
        {
            string[] data = (string[])ViewState["data"];
            int nmax = (int)ViewState["nmax"]; int nmin = (int)ViewState["nmin"];
            int i = (int)ViewState["EditIndex"];
            DateTime t0 = new DateTime();
            t0 = (DateTime)ViewState["EditDate"];
            string s = data[i]; char[] c1 = new char[1]; c1[0] = (char)0x09;

            DateTime t1 = new DateTime(); t1 = t0.AddHours(8).AddMinutes(49);//start at 8:40
            DateTime t2 = new DateTime(); t2 = t0.AddHours(18).AddMinutes(39);//end at 18:39
            DateTime AMSessionEnd = new DateTime(); AMSessionEnd = t0.AddMinutes(30).AddHours(12);
            DateTime PMSessionEnd = new DateTime(); PMSessionEnd = t0.AddMinutes(51).AddHours(16);

            ScheduledComponentList scl1 = new ScheduledComponentList();
            scl1.LoadList_orderbyStudentDateASC(t1, t2);//order by StudentId , DateTime, TimeAllowed DESC, ComponentCode ";

            Guid st1 = new Guid(); st1 = Guid.Empty;bool AM = true;
            foreach (ScheduledComponent sc in scl1.m_List)
            {
                AM = (sc.m_TimetabledSession == "A") ;
                if (sc.m_StudentId != st1)//new student so reset to start of day...
                {
                    st1 = sc.m_StudentId; t1 = t0;
                }
                //if this component starts before previous has " we have a clash
                if (sc.m_Date < t1)
                {
                    //clash....  need to move this component
                    //is there time in the session? it will now end at t1 +10 +time allowed
                    
                    if (((t1.AddMinutes(sc.m_TimeAllowed).AddMinutes(10) < AMSessionEnd) && AM) || ((t1.AddMinutes(sc.m_TimeAllowed).AddMinutes(10) < PMSessionEnd) && !AM))
                    {//simple add...
                        sc.m_Date = t1.AddMinutes(10);
                        sc.Save();
                        //move time on
                        t1 = sc.m_Date.AddMinutes(sc.m_TimeAllowed);//new end time
                    }
                    else
                    {
                        //so can't move within session.....need to find next free session......
                        //going to reload list 
                        ScheduledComponentList scl2 = new ScheduledComponentList();

                        //list is ordered by date asc
                        //if ours is PM... is there space AM... if ours is AM is there space PM???
                        if (AM)
                        {
                            scl2.LoadList_Student(t0.AddHours(13), t0.AddHours(23), sc.m_StudentId.ToString());
                            if (scl2.m_List.Count == 0)
                            {
                                //we can move it to PM
                                sc.m_Date = t0.AddHours(13).AddMinutes(40); sc.Save();
                                //need to add supervision....
                                AddSupervision(t1, sc.m_Date, sc);
                            }
                            else
                            {
                                //going to count hours pm...
                                int total = 0; DateTime t3 = new DateTime();
                                foreach (ScheduledComponent sc1 in scl2.m_List)
                                {
                                    total += sc1.m_TimeAllowed + 10; t3 = sc1.m_Date.AddMinutes(sc1.m_TimeAllowed + 10);
                                }
                                total += sc.m_TimeAllowed;
                                if (total <= 180)
                                {
                                    sc.m_Date = t3; sc.Save();
                                    AddSupervision(t1, t0.AddHours(13).AddMinutes(40), sc);
                                    //so can fit it in.....but ought to put at end???
                                }
                                else
                                {
                                    FindNextSlot(sc, t3);
                                }
                            }

                        }
                        else
                        {
                            //pm exam try to fit in am
                            scl2.LoadList_Student(t0, t0.AddHours(13), sc.m_StudentId.ToString());///am list
                            if (scl2.m_List.Count == 0)
                            {
                                //we can move it to AM
                                sc.m_Date = t0.AddHours(8).AddMinutes(50);
                                sc.Save();
                                //need to add supervision....
                                AddSupervision(sc.m_Date.AddMinutes(sc.m_TimeAllowed+1), t0.AddHours(13).AddMinutes(40), sc);
                            }
                            else
                            {
                                //going to count hours am...
                                int total = 0; DateTime t3 = new DateTime();
                                foreach (ScheduledComponent sc1 in scl2.m_List)
                                {
                                    total += sc1.m_TimeAllowed + 10; t3 = sc1.m_Date.AddMinutes(sc1.m_TimeAllowed + 10);
                                }
                                total += sc.m_TimeAllowed;
                                if (total <= 180)
                                {
                                    sc.m_Date = t3; sc.Save();
                                    AddSupervision(sc.m_Date, t0.AddHours(13).AddMinutes(40), sc);
                                    //so can fit it in.....but ought to put at end???
                                }
                                else
                                {
                                    //so no slots in am... need to go to next day...find time of last exam...
                                    scl2.LoadList_Student(t0.AddHours(13), t0.AddHours(23), sc.m_StudentId.ToString());
                                    foreach (ScheduledComponent sc1 in scl2.m_List){t3 = sc1.m_Date.AddMinutes(sc1.m_TimeAllowed + 10);}
                                    FindNextSlot(sc, t3);
                                }
                            }
                        }

                    }
                }
                else
                {
                    //no clash.. all is good
                    t1 = sc.m_Date.AddMinutes(sc.m_TimeAllowed);//new end time
                }
            }
            UpdateButtons(t0);
        }

        protected void FindNextSlot(ScheduledComponent sc, DateTime start_time)//start time is time of last exam..
        {
            //so we are going to look forward for next slot big enough to accomodate this boy and scehdule it??
            ScheduledComponentList scl2 = new ScheduledComponentList();
            //goto next day
            DateTime t0 = new DateTime(start_time.Year, start_time.Month, start_time.Day);
            bool found = false;
            while (!found)
            {
                t0 = t0.AddDays(1);
                if (t0.DayOfWeek == DayOfWeek.Saturday) t0 = t0.AddDays(2);//nightmare!

                scl2.LoadList_Student(t0.AddHours(8), t0.AddHours(12), sc.m_StudentId.ToString());
                if (scl2.m_List.Count == 0)
                {
                    sc.m_Date = t0.AddHours(12).AddMinutes(30).AddMinutes(-sc.m_TimeAllowed - 10);
                    sc.Save();
                    //need to add supervision....
                    AddSupervision(start_time, sc.m_Date, sc);
                    found = true;
                }
                else
                {
                    //going to count hours am...
                    int total = 0; DateTime t3 = new DateTime();
                    foreach (ScheduledComponent sc1 in scl2.m_List)
                    {
                        total += sc1.m_TimeAllowed + 10; t3 = sc1.m_Date.AddMinutes(sc1.m_TimeAllowed + 10);
                    }
                    total += sc.m_TimeAllowed;
                    if (total <= 180)
                    {
                        sc.m_Date = t3; sc.Save();
                        AddSupervision(start_time, sc.m_Date, sc); found = true;
                    }
                    else
                    {
                        scl2.LoadList_Student(t0.AddHours(13), t0.AddHours(18), sc.m_StudentId.ToString());
                        if (scl2.m_List.Count == 0)
                        {
                            sc.m_Date = t0.AddHours(13).AddMinutes(40);
                            sc.Save();
                            //need to add supervision....
                            AddSupervision(start_time, sc.m_Date, sc); found = true;
                        }
                        else
                        {
                            total = 0;
                            foreach (ScheduledComponent sc1 in scl2.m_List)
                            {
                                total += sc1.m_TimeAllowed + 10; t3 = sc1.m_Date.AddMinutes(sc1.m_TimeAllowed + 10);
                            }
                            total += sc.m_TimeAllowed;
                            if (total <= 180)
                            {
                                sc.m_Date = t3; sc.Save();
                                AddSupervision(start_time, sc.m_Date, sc); found = true;
                            }
                            else
                            {
                                found = false;
                            }
                        }
                    }
                }
            }

        }

        protected void AddSupervision(DateTime t_start, DateTime t_end, ScheduledComponent sc)
        {
            ScheduledComponent sched1 = new ScheduledComponent();
            TimeSpan tgap = new TimeSpan();
            tgap = t_end - t_start;
            if ((tgap.TotalMinutes >= 10) && (tgap.TotalDays < 2))
            {
                ExamConversions u = new ExamConversions();
                ExamComponent ec1 = new ExamComponent();
                ec1.m_ComponentCode = "Super";
                ec1.m_ComponentTitle = "Supervision";
                ec1.m_ComponentTitle += "-" + sc.GetHashCode().ToString();
                ec1.m_ExamBoardID = new Guid("436ff234-0457-430a-b1e2-b08758ff30ef");
                ec1.m_year = Year.ToString().Substring(2, 2);
                ec1.m_season = u.GetSeasonCode(SeasonCode);
                ec1.m_Teachermarks = "0"; ec1.m_MaximumMark = "0";
                ec1.m_Timetabled = "T"; ec1.m_TimetableDate = t_start;
                ec1.m_TimetableSession = "A";
                ec1.m_Time = tgap.TotalMinutes.ToString();
                //now if the brat has extra time we need to reduce...>!!!!
                StudentSENList ssen1 = new StudentSENList(sc.m_StudentId.ToString());
                double extra_time = 0;
                double time1 = tgap.TotalMinutes;
                //now if the brat has extra time we need to reduce.. becasue it will be increased by code when read...
                foreach (StudentSEN sen1 in ssen1.m_List)
                {
                    if (sen1.m_ExamsExtraTime > 0) extra_time = (double)sen1.m_ExamsExtraTime;
                }
                time1 = time1 / ((100 + extra_time) / 100);
                int i = (int)time1;
                ec1.m_Time = i.ToString();
                ec1.Create();

                ec1.m_ComponentID = ec1.Find_ComponentID(ec1.m_ComponentCode, ec1.m_ComponentTitle, ec1.m_ExamBoardID.ToString(), ec1.m_season, ec1.m_year);

                if (ec1.m_ComponentID != Guid.Empty)
                {
                    sched1.Load(ec1.m_ComponentID, sc.m_StudentId);

                    if (!sched1.m_valid)
                    {
                        sched1.m_StudentId = sc.m_StudentId;
                        sched1.m_ComponentId = ec1.m_ComponentID;
                        sched1.m_RoomId = Guid.Empty;
                        sched1.m_Year = Year.ToString();
                        sched1.m_Season = u.GetSeasonCode(SeasonCode);
                        sched1.m_valid = true;
                        sched1.m_Date = t_start;
                        sched1.m_Desk = "";
                        sched1.m_Will_Type = false;// do these later...
                        sched1.Save();
                    }
                }
            }
        }
        #endregion

        #region Edit Start Time

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView_DayView.Rows[index];
                Label_EditComponentID.Text= row.Cells[2].Text;
                Label_EditTime.Text = row.Cells[4].Text;
                TextBox_NewStartTime.Text= row.Cells[4].Text;
                VisibiltiyNewStartTime(true);
            }
        }
        protected void ButtonNewStartTime_Click(object sender, EventArgs e)
        {
            DateTime t0 = new DateTime(); DateTime t1 = new DateTime();
            t0 = System.Convert.ToDateTime(Label_EditTime.Text);//recover the current start time
            try
            {
                t1 = System.Convert.ToDateTime(TextBox_NewStartTime.Text);//recover the new time
                ScheduledComponentList scl1 = new ScheduledComponentList();
                scl1.LoadList(t0.AddMinutes(-1), t0.AddMinutes(1), Label_EditComponentID.Text);
                foreach (ScheduledComponent sc in scl1.m_List)
                {
                    sc.m_Date = t1; sc.Save();
                }
                TextBox_NewStartTime.BackColor = System.Drawing.Color.White;
                VisibiltiyNewStartTime(false);
                t1 = (DateTime)ViewState["EditDate"];
                Encode en = new Encode();
                SqlDataSource1.SelectCommand = GetQueryStringDay(Year, t0.Month, t0.Day, " ORDER BY DateTime");
                SqlDataSource1.ConnectionString = en.GetDbConnection();
                SqlDataSource1.DataBind();
            }
            catch
            {
                TextBox_NewStartTime.BackColor = System.Drawing.Color.Red;
            }
        }
#endregion

        protected void VisibiltiyNewStartTime(bool b)
        {
            ButtonNewStartTime.Visible = b;
            Label_NewStartTime.Visible = b;
            TextBox_NewStartTime.Visible = b;
        }
        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[2].Visible = false;//HIDE id COLUMN
        }
        protected void Button_RoomsAM_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            Server.Transfer(@"../exams/TimetableRooms.aspx?Date=" + t0.ToShortDateString() + "&Session=AM");
        }
        protected void Button_RoomsPM_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            Server.Transfer(@"../exams/TimetableRooms.aspx?Date=" + t0.ToShortDateString() + "&Session=PM");
        }

        protected void Button_DesksAM_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            Server.Transfer(@"../exams/TimetableDesks.aspx?Date=" + t0.ToShortDateString() + "&Session=AM");
        }

        protected void Button_DesksPM_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            Server.Transfer(@"../exams/TimetableDesks.aspx?Date=" + t0.ToShortDateString() + "&Session=PM");
        }

        protected void Button_PupilsAM_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            Server.Transfer(@"../exams/TimetableDetailEdit.aspx?Date=" + t0.ToShortDateString() + "&Session=AM");
        }

        protected void Button_PupilsPM_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            Server.Transfer(@"../exams/TimetableDetailEdit.aspx?Date=" + t0.ToShortDateString() + "&Session=PM");
        }
    }
}