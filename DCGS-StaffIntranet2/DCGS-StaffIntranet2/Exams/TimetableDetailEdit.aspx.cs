using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class TimetableDetailEdit : System.Web.UI.Page
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
            SetupGrid();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                //setup the list of selected students..
                List<Guid> selected = new List<Guid>();
                ViewState["Selected"] = selected;
            //GetMasterValues();
           // SetupGrid();
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "   Student Details";
            }
        }
        protected void SetupGrid()
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
            }
            catch
            {
                //try to get from session state?
                Is_AM = (bool)Session["Session_is_AM"];
                t0 = (DateTime)Session["EditDate"];
            }
            DateTime t1 = new DateTime();
            DateTime t2 = new DateTime();
            t1 = (Is_AM) ? t0.AddHours(8) : t0.AddHours(13);
            t2 = (Is_AM) ? t0.AddHours(13) : t0.AddHours(18);
            Label_Date.Text = t0.ToLongDateString();
            Encode en = new Encode();
            string orderbyclause = " ORDER BY DateTime";
            SqlDataSource1.SelectCommand = GetQuery(t1, t2,orderbyclause);
            SqlDataSource1.ConnectionString = en.GetDbConnection();
            SqlDataSource1.DataBind();
            string s = "SELECT DISTINCT  dbo.tbl_Core_Rooms.RoomId AS Id, dbo.tbl_Core_Rooms.RoomCode AS Name ";
            s += " FROM  dbo.tbl_Core_Rooms INNER JOIN dbo.tbl_Exams_RoomLayouts ON dbo.tbl_Core_Rooms.RoomId = dbo.tbl_Exams_RoomLayouts.RoomId ";
            s += " ORDER BY dbo.tbl_Core_Rooms.RoomCode ";
            SqlDataSource2.SelectCommand = s;
            SqlDataSource2.ConnectionString = en.GetDbConnection();
            SqlDataSource2.DataBind();

        }
        protected string GetQuery(DateTime t1, DateTime t2,string orderby)
        {
            string s = "SELECT CONVERT(char(10), dbo.qry_Cerval_Exams_ScheduledComponents.DateTime, 108) AS Time,  ";
            s+=" StudentExamNumber AS Adno, PersonGivenName AS GivenName , PersonSurname AS Surname, ComponentTitle AS Component, ";
            s += " dbo.tbl_Core_Rooms.RoomCode AS Room,  ";
            s+=" Desk AS Desk, WillType AS Typist, ScheduledComponentID  AS ID ";
            s += " FROM  dbo.qry_Cerval_Exams_ScheduledComponents INNER JOIN ";
            s += " dbo.tbl_Core_Rooms ON dbo.qry_Cerval_Exams_ScheduledComponents.RoomId = dbo.tbl_Core_Rooms.RoomId ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += orderby;
            return s;
        }

        protected string GetQuery_One(DateTime t1, DateTime t2, string orderby,string Adno)
        {
            string s = "SELECT CONVERT(char(10), dbo.qry_Cerval_Exams_ScheduledComponents.DateTime, 108) AS Time,  ";
            s += " StudentExamNumber AS Adno, PersonGivenName AS GivenName , PersonSurname AS Surname, ComponentTitle AS Component, ";
            s += " dbo.tbl_Core_Rooms.RoomCode AS Room,  ";
            s += " Desk AS Desk, WillType AS Typist, ScheduledComponentID  AS ID ";
            s += " FROM  dbo.qry_Cerval_Exams_ScheduledComponents INNER JOIN ";
            s += " dbo.tbl_Core_Rooms ON dbo.qry_Cerval_Exams_ScheduledComponents.RoomId = dbo.tbl_Core_Rooms.RoomId ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (StudentExamNumber='" + Adno + "' ) ";
            s += orderby;
            return s;
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                //col 9 has id.... scheduled component ID I thinks
                TableCell c = row.Cells[1];
                string s = row.Cells[9].Text;
                Guid g = new Guid(s);
                List<Guid> selected = new List<Guid>();
                selected =(List<Guid>)ViewState["Selected"];
                if (selected.Contains(g))
                {
                    selected.Remove(g);
                    c.BackColor = System.Drawing.Color.White;
                }
                else
                {
                    selected.Add(g);
                    c.BackColor = System.Drawing.Color.Aqua;
                }
                ViewState["Selected"] = selected;
            }
            if (e.CommandName == "xx2")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                //col 9 has id.... scheduled component ID I thinks
                string s = row.Cells[9].Text;
                Guid g = new Guid(s);
                ScheduledComponent sc = new ScheduledComponent();
                sc.Load(g);
                Panel1.Visible = false;
                Panel2.Visible = true;
                Label_EditName.Text = sc.m_Givenname + " " + sc.m_Surname + " (" + sc.m_ExamNumber + ")";
                TextBox_Desk.Text = sc.m_Desk;
                ViewState["CurrentEdit"] = g;
                DropDownList_Rooms.SelectedValue = sc.m_RoomId.ToString();
                TextBox_StartTime.Text = sc.m_Date.ToShortTimeString();
            }

        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[9].Visible = false;//HIDE id COLUMN
        }

        protected void Button_EditSave_Click(object sender, EventArgs e)
        {
            Guid g = (Guid)ViewState["CurrentEdit"];
            ScheduledComponent sc = new ScheduledComponent();
            sc.Load(g);
            sc.m_Desk = TextBox_Desk.Text;
            sc.m_RoomId =new Guid( DropDownList_Rooms.SelectedValue);

            //starttime box is hh:mm
            int hour = System.Convert.ToInt16(TextBox_StartTime.Text.Substring(0, 2));
            int min = Convert.ToInt16(TextBox_StartTime.Text.Substring(3, 2));
            DateTime t1 = new DateTime(sc.m_Date.Year, sc.m_Date.Month, sc.m_Date.Day, hour, min, 0);
            sc.m_Date = t1;
            sc.Save();

            Panel1.Visible = true;
            Panel2.Visible = false;
            SetupGrid();
            GridView1.DataBind();
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
            SetupGrid();
            GridView1.DataBind();
        }

        protected void Button_Restrict_Click(object sender, EventArgs e)
        {
            string adno =TextBox_Adno.Text;

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
                Is_AM = (bool)Session["Session_is_AM"];
                t0 = (DateTime)Session["EditDate"];
            }
            DateTime t1 = new DateTime();
            DateTime t2 = new DateTime();
            t1 = (Is_AM) ? t0.AddHours(8) : t0.AddHours(13);
            t2 = (Is_AM) ? t0.AddHours(13) : t0.AddHours(18);
            Encode en = new Encode();
            string orderbyclause = " ORDER BY DateTime";
            SqlDataSource1.SelectCommand = GetQuery_One(t1, t2, orderbyclause,adno);
            SqlDataSource1.ConnectionString = en.GetDbConnection();
            SqlDataSource1.DataBind();
            GridView1.DataBind();
        }

        protected void Button_Clear_Click(object sender, EventArgs e)
        {
            SetupGrid();
            GridView1.DataBind();
        }

        protected void Button_Return_Click(object sender, EventArgs e)
        {
            DateTime t0 = (DateTime)ViewState["EditDate"];
            bool Is_AM = (bool)ViewState["Session_is_AM"];
            if(Is_AM) Server.Transfer(@"../exams/TimetableSummary.aspx?Date=" + t0.ToShortDateString() + "&Session=AM");
            else Server.Transfer(@"../exams/TimetableSummary.aspx?Date=" + t0.ToShortDateString() + "&Session=PM");
        }
    }
}