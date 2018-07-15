
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class TimetableInitialize : System.Web.UI.Page
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

        protected void Page_Init(object sender, EventArgs e)
        {
            GetMasterValues();
            SetupSummaryTable();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                GetMasterValues();
            }
        }

        private void GetMasterValues()
        {
            SeasonCode = System.Convert.ToInt32((string)(Session["Season"]));
            Year = System.Convert.ToInt32((string)(Session["Year"]));
            YearCode = Year % 100;
            Label Label_Banner = (Label)Master.FindControl("Label_Banner");
            if (Label_Banner != null)
            {
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Timetable Initilalize";
            }
        }
        protected void SetupSummaryTable()
        {
            DateTime t1 = new DateTime(Year, SeasonCode - 2, 1);//so summer season might start in april???
            DateTime t0 = new DateTime(); t0 = t1;
            //advance to Monday
            while (t0.DayOfWeek != DayOfWeek.Monday)
            {
                t0 = t0.AddDays(1);
            }
            //this is our start date...
            string[] data = new string[300];//should be enough!
            for (int i = 0; i < 120; i++) data[i] = "";

            int n = 0; int nmin = 200; int nmax = -1;
            // list for the contents of this row...
            DateTime t2 = new DateTime();
            TimeSpan tspan1 = new TimeSpan();
            TimeSpan tspanWeek = new TimeSpan(0, 1, 0, 0);
            ExamCompononent_List ecl1 = new ExamCompononent_List();
            ecl1.LoadAllComponentsSeason(YearCode.ToString(), SeasonCode.ToString());
            ExamsUtility u = new ExamsUtility();

            //ecl1.LoadAllComponents_NotScheduled(Year.ToString(), SeasonCode.ToString());
            foreach (ExamComponent ec in ecl1.m_list)
            {
                t2 = ec.m_TimetableDate; tspan1 = t2 - t0; n = tspan1.Days;
                data[n] += ec.m_ComponentTitle + ":" + ec.m_ComponentCode + ":" +ec.m_ComponentID.ToString() + (char)0x09;
                //component title:option code
                if (n > nmax) nmax = n;
                if (n < nmin) nmin = n;
            }
            List<ComponentNotScheduled> list1 = new List<ComponentNotScheduled>();
            //so want to do whole weeks from t0;
            n = 0; while ((nmin - n) > 7) n += 7;
            //so we start the table at n...
            t2 = t0.AddDays(n);
            while (n < nmax)
            {
                TableRow r1 = new TableRow(); Table_Summary.Rows.Add(r1);
                TableCell c0 = new TableCell(); c0.Text = t2.ToShortDateString(); r1.Cells.Add(c0);
                for (int i = 0; i < 5; i++)
                {
                    //so date is t2
                    list1 =u.LoadComponentsNotSchedlued(Year.ToString(), SeasonCode.ToString(), t2.AddHours(-1), t2.AddHours(22));
                    TableCell c1 = new TableCell();
                    if (list1.Count>0)
                    {

                        Button b1 = new Button();
                        b1.Text = t2.ToString("M");

                        c1.Controls.Add(b1);
                        b1.Click += B1_Click;
                        b1.ID = "Button" + (n + i).ToString();
                        b1.BackColor = System.Drawing.Color.AntiqueWhite;
                    }
                    r1.Cells.Add(c1); t2 = t2.AddDays(1);
                }
                n += 7; t2 = t2.AddDays(2);
            }
        }
        private void B1_Click(object sender, EventArgs e)///button on summary grid
        {
            Button b1 = (Button)sender;
            string s = b1.ID;
            string s3 = b1.Text + " " + Year.ToString();  //has date?
            DateTime t0 = new DateTime(); t0 = System.Convert.ToDateTime(s3);
            int i = System.Convert.ToInt16(s.Substring(6));
            UpdateTimetable(t0, t0);
            b1.BackColor = System.Drawing.Color.LightGray;

        }

        protected void Button_Initialize_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false; Panel2.Visible = true;
            TextBox_Warning.Text = "Are you sure you want to Initialise the Timetable? This will reset all rooms/desks etc..";
            Button_OK.Text = "Initialize!";
        }
        protected void Button_Clear_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false; Panel2.Visible = true;
            TextBox_Warning.Text = "Are you sure you want to CLEAR the Timetable? This will reset all rooms/desks etc..";
            Button_OK.Text = "Clear!";
        }

        protected void Button_Update_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false; Panel2.Visible = true;
            TextBox_Warning.Text = "Are you sure you want to Update the Timetable? This will add any exams not scheduled yet. Only days that need updating will be shown.";
            Button_OK.Text = "Update!";
        }

        protected void DoDeleteTimeTable(string year,string season)
        {
            Encode en = new Encode();
            string s = "DELETE  FROM tbl_Exams_ScheduledComponents ";
            s+=" WHERE (Year='" +year + "' ) AND (Season ='" +season + "' )";
            int no1 = en.Execute_count_SQL(s);
        }
        protected void Button_OK_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            switch (b.Text)
            {
                case "Update!":
                    Panel1.Visible = false;
                    Panel2.Visible = false;Panel3.Visible = true;
                    Label_Panel3.Text = " Click on the day to Schedule.";
                    break;
                case "Clear!":
                    Panel2.Visible = false;
                    DoDeleteTimeTable(Year.ToString(), SeasonCode.ToString());
                    Panel1.Visible = true;
                    break;
                case "Initialize!":
                    Panel1.Visible = false;
                    DoDeleteTimeTable(Year.ToString(), SeasonCode.ToString());
                    Panel2.Visible = false; Panel3.Visible = true;
                    Label_Panel3.Text = " Click on the day to Schedule.";
                    break;
                default:
                    break;
            }
            Panel2.Visible = false;
  
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true; Panel2.Visible = false;
        }

        protected bool Check_All_Scheduled()
        {
            ExamCompononent_List ecl1 = new ExamCompononent_List();
            ecl1.LoadAllComponents_NotScheduled(Year.ToString(), SeasonCode.ToString());
            if (ecl1.m_count == 0)
            {
                return true;
            }
            return false;
        }

        protected void DeleteTimetable()
        {
            //wil open the dialog and wait for confirm
            Panel1.Visible = false; Panel2.Visible = true;
        }
        protected void UpdateTimetable(DateTime start,DateTime end)
        {
            // add scheduled components for any components not scheduled...
            //TODO also delete any withdrawn....
            ExamConversions u = new ExamConversions();
            ExamCompononent_List ecl1 = new ExamCompononent_List();

            //ecl1.LoadAllComponents(Year.ToString(), SeasonCode.ToString());
            ecl1.LoadAllComponentsSeasonDate(YearCode.ToString(), SeasonCode.ToString(), start, end);
            Encode en = new Encode();
            string s = "";

            ScheduledComponent sched1 = new ScheduledComponent();
            s = ""; int n = 0;
            DateTime d1 = new DateTime();
            foreach (ExamComponent ec in ecl1.m_list)
            {
                ExamLinkComponent_List elcl1 = new ExamLinkComponent_List();
                elcl1.LoadList_Component(ec.m_ComponentID);
                foreach (ExamLinkComponent elc in elcl1.m_list)
                {
                    ExamEntries_List exl1 = new ExamEntries_List();
                    //now need all entries for this option.....
                    exl1.Load_Option(elc.m_OptionId);
                    n += exl1.m_list.Count;
                }
            }

            int n1 = 0;int n2 = 0;
            foreach (ExamComponent ec in ecl1.m_list)
            {
                if (ec.m_Timetabled == "T")
                {
                    d1 = ec.m_TimetableDate;
                    if (ec.m_TimetableSession == "A")
                    {
                        d1 = d1.AddHours(8); d1 = d1.AddMinutes(50);
                    }
                    else
                    {
                        d1 = d1.AddHours(13); d1 = d1.AddMinutes(40);
                    }
                    //need to find all entries that use this component.....
                    ExamLinkComponent_List elcl1 = new ExamLinkComponent_List();
                    elcl1.LoadList_Component(ec.m_ComponentID);
                    foreach (ExamLinkComponent elc in elcl1.m_list)
                    {
                        ExamEntries_List exl1 = new ExamEntries_List();
                        //now need all entries for this option.....
                        exl1.Load_Option(elc.m_OptionId);
                        foreach (Exam_Entry ex in exl1.m_list)
                        {
                            if (!ex.m_withdrawn)
                            {
                                sched1.Load(ec.m_ComponentID, ex.m_StudentID);
                                if (!sched1.m_valid)
                                {
                                    sched1.m_StudentId = ex.m_StudentID;
                                    sched1.m_ComponentId = ec.m_ComponentID;
                                    sched1.m_RoomId = Guid.Empty;
                                    sched1.m_Year = Year.ToString();
                                    sched1.m_Season = u.GetSeasonCode(SeasonCode);
                                    sched1.m_valid = true;
                                    sched1.m_Date = d1;
                                    sched1.m_Desk = "";
                                    sched1.m_Will_Type = false;// do these later...
                                    sched1.Save();
                                    n2++;

                                }
                            }
                            else
                            {
                                sched1.Load(ec.m_ComponentID, ex.m_StudentID);
                                if (sched1.m_valid)
                                {
                                    //need to delete
                                    sched1.Delete();
                                    n1++;
                                }
                            }
                        }
                    }
                }
            }


            //now ought to update the willtype fields to agree with the cantype entry..
            //oohhh this is going to be messy.....
            //read from a querry..... then update Exams_Scheduled_components...

            CanTypeList typists = new CanTypeList();
            foreach (Guid g in typists.m_List)
            {
                s = "UPDATE dbo.tbl_Exams_ScheduledComponents SET WillType =1 WHERE (StudentId = '" + g.ToString() + "'  )AND (Year='" + YearCode.ToString() + "' ) AND (Season ='" + u.GetSeasonCode(SeasonCode) + "' )";
                en.ExecuteSQL(s);
            }
        }


    }
}