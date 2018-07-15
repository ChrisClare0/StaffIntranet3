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
    public partial class TimetableDesks : System.Web.UI.Page
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + " Desk Dialog";
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

                SetupDeskGrid(t1, t2);

            }

        }

        private void SetupDeskGrid(DateTime t1, DateTime t2)
        {

            System.Drawing.Color[] cell_colours = GetColors();

            Label_DeskDate.Text = t1.ToLongDateString();
            //setup dropdown list of rooms...
            List<Guid> Rooms = new List<Guid>();
            

            //need to load all rooms used...


            string s = "SELECT DISTINCT RoomId FROM  dbo.tbl_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            Encode en = new Encode();

            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (dr.IsDBNull(0)) Rooms.Add(Guid.Empty); else Rooms.Add(dr.GetGuid(0));
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

            if ((Rooms.Count == 1)&&(Rooms[0]==Guid.Empty))
            {
                Label_message.Text = " No Rooms Allocated...";
                Label_message.Visible = true;
            }

            DropDownListDeskRooms.Items.Clear();
            SimpleRoom r1 = new SimpleRoom();
            foreach (Guid d in Rooms)
            {
                r1.Load(d.ToString());
                ListItem l = new ListItem(r1.m_roomcode, d.ToString());
                DropDownListDeskRooms.Items.Add(l);
            }


            //need to find all the changes...
            DateTime t0 = new DateTime(); t0 = t1;
            ScheduledComponentList scl1 = new ScheduledComponentList(); scl1.LoadList_Date(t1, t2);
            while (GoToNextChange(ref t0, scl1, Rooms[0]))
            {
                ListItem l = new ListItem(t0.AddMinutes(-1).ToShortTimeString(), t0.ToString());
                DropDownListDeskTimes.Items.Add(l);
            }

            UpdateDeskGrid();
        }
        private void UpdateDeskGrid()
        {
            DateTime t1 = System.Convert.ToDateTime(DropDownListDeskTimes.SelectedValue);
            string s1 = "";

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
            DateTime t2 = new DateTime();
            t2 = (Is_AM) ? t0.AddHours(13) : t0.AddHours(18);

            System.Drawing.Color[] cell_colours = GetColors();

            Guid[] papers = new Guid[10]; string[] paper_title = new string[10];
            string s = "";
            int no_papers = 0;
            bool found = false;
            Label_DeskDate.Text = t1.ToLongDateString();
  


            ScheduledComponentList scl2 = new ScheduledComponentList(); scl2.LoadList(t0, t2);
            scl2.LoadList_orderbyComponent(t0, t2);
            //need to restrict to those current
            ScheduledComponentList scl1 = new ScheduledComponentList();
            foreach(ScheduledComponent sc in scl2.m_List)
            {
                if (sc.m_Date.AddMinutes(sc.m_TimeAllowed) > t1)
                {
                    scl1.m_List.Add(sc);
                }
            }


            t0 = System.Convert.ToDateTime(DropDownListDeskTimes.SelectedValue);

            TableDeskView.Rows.Clear();

            Guid Current_Room = new Guid();
            Current_Room = new Guid(DropDownListDeskRooms.SelectedValue);
            if (Current_Room == Guid.Empty) return;
            ExamRoom exr1 = new ExamRoom(); exr1.Load(Current_Room);
            int nrows = 0; int ncols = 0;
            foreach (ExamColumn ec in exr1.columns)
            {
                if (ec.count > nrows) nrows = ec.count;
            }
            TableDeskView.CssClass = "EventsTable";
            TableRow r0 = new TableRow();
            TableDeskView.Rows.Add(r0);
            TableCell c3 = new TableCell(); r0.Cells.Add(c3); ncols++;

            foreach (ExamColumn ec in exr1.columns)
            {
                if (ec.count > 0)
                {
                    TableCell c = new TableCell();
                    c.Width = 40; c.Text = ec.name;//header row
                    r0.Cells.Add(c); ncols++;
                }
            }
            TableCell c4 = new TableCell(); r0.Cells.Add(c4);
            TableCell c2 = new TableCell();
            r0.Cells.Add(c2);
            c2.Text = "Colour Code for Papers";
            c2.Width = 200;

            //for (int i = 0; i < nrows; i++)
            for (int i = nrows-1; i >-1; i--)
            {
                TableRow r = new TableRow();
                TableDeskView.Rows.Add(r);

                TableCell c1 = new TableCell(); c1.Text = (i + 1).ToString(); r.Cells.Add(c1);
                c1.Width = 40; c1.BackColor = System.Drawing.Color.LightGray;
                foreach (ExamColumn ec in exr1.columns)
                {
                    if (ec.count > 0)
                    {
                        TableCell c = new TableCell();
                        c.Width = 40; c.Text = "";
                        r.Cells.Add(c);
                    }
                }
                TableCell c5 = new TableCell(); r.Cells.Add(c5); c5.Width = 40;
                TableCell c6 = new TableCell(); r.Cells.Add(c6); c6.Width = 40;
            }
            t0 = t0.AddMinutes(2);

            try
            {
                s1 = TableDeskView.Rows[1].Cells[1].Text;
            }
            catch { }

            foreach (ScheduledComponent sc in scl1.m_List)
            {
                if ((Current_Room == sc.m_RoomId))
                {
                    if ((sc.m_Date < t0) && (sc.m_Date.AddMinutes(sc.m_TimeAllowed) > t0))
                    {
                        s = sc.m_Desk;

                        if ((s != "") && (s != null))
                        {
                            int col = 0;
                            foreach (TableCell tc in TableDeskView.Rows[0].Cells)
                            {
                                if (tc.Text == s.Substring(0, 1))
                                {
                                    col = TableDeskView.Rows[0].Cells.GetCellIndex(tc);
                                    break;
                                }
                            }
                            int row = System.Convert.ToInt32(s.Substring(1));
                            row = nrows + 1 - row;

                            TableDeskView.Rows[row].Cells[col].Text = sc.m_ExamNumber.ToString();


                            found = false;
                            for (int i = 0; i < no_papers; i++)
                            {
                                if (papers[i] == sc.m_ComponentId)
                                {
                                    found = true;
                                    TableDeskView.Rows[row].Cells[col].BackColor = cell_colours[i];
                                }
                            }
                            if (!found)
                            {
                                no_papers++;
                                papers[no_papers - 1] = sc.m_ComponentId;
                                TableDeskView.Rows[row].Cells[col].BackColor = cell_colours[no_papers - 1];
                                paper_title[no_papers - 1] = sc.m_ComponentCode + ":" + sc.m_ComponentTitle;
                            }
                        }
                    }
                }
            }



            for (int i = 0; i < no_papers; i++)
            {
                TableDeskView.Rows[i + 1].Cells[ncols+1].Text = paper_title[i];
                TableDeskView.Rows[i + 1].Cells[ncols+1].BackColor = cell_colours[i];
            }



        }
        protected System.Drawing.Color[] GetColors()
        {
            System.Drawing.Color[] cell_colours = new System.Drawing.Color[10];
            cell_colours[0] = System.Drawing.Color.HotPink; cell_colours[1] = System.Drawing.Color.Turquoise;
            cell_colours[2] = System.Drawing.Color.LawnGreen; cell_colours[3] = System.Drawing.Color.LightBlue;
            cell_colours[4] = System.Drawing.Color.MediumVioletRed; cell_colours[5] = System.Drawing.Color.Olive;
            cell_colours[6] = System.Drawing.Color.Yellow; cell_colours[7] = System.Drawing.Color.DarkGreen;
            cell_colours[8] = System.Drawing.Color.SandyBrown;
            return cell_colours;
        }
        protected void Button_AssignDesks_Click(object sender, EventArgs e)
        {
            string ErrorS = "Success!";

            bool Is_AM = true;
            DateTime t0 = new DateTime();
            Is_AM = (bool)ViewState["Session_is_AM"];
            t0 = (DateTime)ViewState["EditDate"];
            DateTime t1 = new DateTime();
            DateTime t2 = new DateTime();
            t1 = (Is_AM) ? t0.AddHours(8) : t0.AddHours(13);
            t2 = (Is_AM) ? t0.AddHours(13) : t0.AddHours(18);
            //going to assign desks for this session 
            //first clear all desks.....   
            ExamsUtility u = new ExamsUtility();
            u.ClearDeskAssignments(t1, t2);

            ScheduledComponentList scl1 = new ScheduledComponentList();
            scl1.LoadList_orderbyRoom(t1, t2);

            Guid room1 = new Guid(); room1 = Guid.Empty;

            foreach (ScheduledComponent sc in scl1.m_List)
            {
                if (sc.m_RoomId != room1)
                {
                    if (!AllocateDesksRoom(sc.m_RoomId, t1, t2, ref ErrorS))
                    {
                        Label_message.Text = ErrorS;
                        Label_message.Visible = true;
                        return;
                    }
                    room1 = sc.m_RoomId;
                }
            }
            scl1.LoadList(t1, t2);
            UpdateDeskGrid();

        }
        protected bool AllocateDesksRoom(Guid roomid, DateTime t1, DateTime t2, ref string ErrorS)
        {
            ScheduledComponentList scl2 = new ScheduledComponentList();
            scl2.LoadList_Room(t1, t2, roomid, " ORDER BY DateTime ASC, TimeAllowed DESC, ComponentId DESC, StudentExamNumber ASC");
            if (scl2.m_List.Count == 0) return true;
            ExamRoom exr1 = new ExamRoom(); exr1.Load(roomid);
            SimpleRoom room1 = new SimpleRoom(); room1.Load(roomid.ToString());
            ScheduledComponentList scl3 = new ScheduledComponentList();//used to check room empty below
            string s = ""; bool found = false;
            int column = 1; int desk = 1; int desk_inc = 1;
            DateTime time_last = new DateTime(2000, 1, 1);
            DateTime time_first = new DateTime(2000, 1, 1);
            bool room_full = false;
            int no_components = 1;
            ScheduledComponent scX = new ScheduledComponent();
            scX = (ScheduledComponent)scl2.m_List[0];
            foreach(ScheduledComponent sc in scl2.m_List)
            {
                if (scX.m_ComponentId != sc.m_ComponentId)
                {
                    scX = sc; no_components++;
                }
            }

            int Free_space_min = (exr1.m_capacity - scl2.m_List.Count)/no_components;
            //when we enter no deska allocated to this room so exr1.m_capacity is capacity
            //if we have n students and m components and (capacity -n)> m then we can try to insert gaps...
            // n< scl2.count ( might have later components... 


            scX = (ScheduledComponent)scl2.m_List[0];
            foreach (ScheduledComponent sc in scl2.m_List)
            {
                if(scX.m_ComponentId != sc.m_ComponentId)
                {
                    //new component...
                    scX = sc;
                    //if space insert blamk row
                    if((exr1.columns[column].count<Free_space_min)&&(CheckBox1.Checked))
                    {
                        desk = 1; column++;column++;
                    }
                }
                if ((sc.m_Date > time_last) && ((column != 1) || (desk != 1)))
                {
                    scl3.LoadList_Room(sc.m_Date.AddMinutes(-10), sc.m_Date, roomid, "");
                    if (scl3.m_List.Count == 0)
                    {
                        //DialogResult d = MessageBox.Show("It looks as if we can reset the desk alocations at " + sc.m_Date.ToShortTimeString() + ". Are you sure you wna to do this... ie do we have two separate internal exmas in the same session?", "Warning while rooming " + room1.m_roomcode, MessageBoxButtons.YesNo);
                        //if (d == DialogResult.Yes)
                        {
                            column = 1; desk = 1; desk_inc = 1;
                            time_last = sc.m_Date.AddMinutes(sc.m_TimeAllowed);
                            time_first = sc.m_Date.AddMinutes(-1);
                        }
                    }
                }

                if (sc.m_Date.AddMinutes(sc.m_TimeAllowed) > time_last)
                {
                    time_last = sc.m_Date.AddMinutes(sc.m_TimeAllowed);
                }

                //assign the lad a desk.....
                //unless he already has one!!!!

                found = false;
                s = exr1.columns[column].name + desk.ToString();
                foreach (ScheduledComponent sc1 in scl2.m_List)
                {
                    if ((sc1.m_StudentId == sc.m_StudentId) && (sc1.m_Date > time_first))
                    {
                        if (sc1.m_Desk != "")
                        {
                            s = sc1.m_Desk; found = true; break;//he already had  one
                        }
                    }
                }

                sc.m_Desk = s;
                if (!found)
                {//we have added one....
                    if(room_full)
                    {
                        ErrorS = "Room capacity exceeded....." + sc.m_ExamNumber.ToString() + ":" + sc.m_Surname + " " + sc.m_Givenname + "..." + sc.m_ComponentTitle;
                        return false;
                    }
                    desk = desk + desk_inc;
                    if (desk > exr1.columns[column].count)
                    {
                        column++;
                        desk = exr1.columns[column].count;
                        desk_inc = desk_inc * (-1);
                    }
                    if (desk < 1)
                    {
                        desk_inc = desk_inc * (-1);
                        desk = 1;
                        column++;
                    }
                }
                sc.Save();
                //check if we have exceeded capacity!
                if (column > exr1.no_columns + 1)
                {
                    room_full = true;
                }

            }
            return true;
        }


        private bool GoToNextChange(ref DateTime t0, ScheduledComponentList scl1, Guid RoomId)
        {
            //find next time ... returns in t0
            DateTime t3 = new DateTime(3000, 1, 1);//before this...
            bool found = false;
            //now go through our list and find next change time....

            foreach (ScheduledComponent sc in scl1.m_List)
            {
                //if ((RoomId == sc.m_RoomId)||((RoomId==Guid.Empty)&&sc.m_RoomId==null))
                {
                    if ((sc.m_Date > t0) || (sc.m_Date.AddMinutes(sc.m_TimeAllowed) > t0))
                    {
                        if (sc.m_Date.AddMinutes(sc.m_TimeAllowed) < t3)
                        {
                            t3 = sc.m_Date.AddMinutes(sc.m_TimeAllowed); found = true;
                        }
                        if ((sc.m_Date < t3) && (sc.m_Date > t0))
                        {
                            t3 = sc.m_Date; found = true;
                        }
                    }
                }
            }
            if (found)
            {
                t0 = t3.AddMinutes(1);
                return true;
            }
            return false;
        }

        protected void ButtonDesk_Next_Click(object sender, EventArgs e)
        {
            try
            {
                DropDownListDeskTimes.SelectedIndex++;
                UpdateDeskGrid();
            }
            catch
            {

            }

        }

        protected void DropDownListDeskRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDeskGrid();
        }

        protected void DropDownListDeskTimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDeskGrid();
        }

        protected void Button_GotoRooms_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            bool Is_AM = (bool)ViewState["Session_is_AM"];
            if(Is_AM) Server.Transfer(@"../exams/TimetableRooms.aspx?Date=" + t0.ToShortDateString() + "&Session=AM");
            Server.Transfer(@"../exams/TimetableRooms.aspx?Date=" + t0.ToShortDateString() + "&Session=PM");
        }

        protected void Button_GotoDaySummary_Click(object sender, EventArgs e)
        {
            //need to return to day summary ...
            DateTime t0 = (DateTime)ViewState["EditDate"];
            Server.Transfer(@"../exams/TimetableSummary.aspx?Date=" + t0.ToShortDateString() + "&Session=AM");
        }
    }
}