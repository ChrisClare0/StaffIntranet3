using System;
using System.Web.Services;
using Cerval_Library;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class Timetable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Encode en = new Encode();
                SqlDataSource1.SelectCommand = GetQueryString("2014", 6);
                SqlDataSource1.ConnectionString = en.GetDbConnection();
                SqlDataSource1.DataBind();
            }
        }

        protected string GetQueryString(string Year, int SeasonCode)
        {
            ExamConversions u = new ExamConversions();
            string s = " SELECT DISTINCT dbo.tbl_Exams_ScheduledComponents.DateTime  ";
            //s += ", dbo.tbl_Exams_Options.OptionCode, ";
            //s += " dbo.tbl_Exams_Components.ComponentTitle ";
            s += " FROM dbo.tbl_Exams_ScheduledComponents INNER JOIN ";
            s += " dbo.tbl_Exams_Components ON dbo.tbl_Exams_ScheduledComponents.ComponentId = dbo.tbl_Exams_Components.ComponentID INNER JOIN ";
            s += " dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID INNER JOIN  ";
            s += " dbo.tbl_Exams_Options ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Options.OptionID ";
            s += "  WHERE(dbo.tbl_Exams_ScheduledComponents.Year = '" + Year + "') ";
            s += "  AND(dbo.tbl_Exams_ScheduledComponents.Season = '" + u.GetSeasonCode(SeasonCode) + "')  ";
            s += "  ORDER BY dbo.tbl_Exams_ScheduledComponents.DateTime";
            return s;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string s = "";
            s = "j";
        }

        [WebMethod]
        public static string Name()
        {
            string Name = "Hello Rohatash Kumar";
            return Name;
        }

        //CODE BEHIND - On clicking the Button
        protected void check1(object sender, EventArgs e)
        {
            string str1 = "test";
            CheckCCN2(str1);
        }
        //Onblur of the Textbox txtNum   
        public void CheckCCN2(string strCCNNum)
        {
            //all your server side code  
            string Name = "Hello Rohatash Kumar";
            return;
        }

        [System.Web.Services.WebMethod]
        public static string InitializeTT(string year, string season)
        {
            ExamTimetable ext1 = new ExamTimetable();
            string Err = "";int n = 0;
            ext1.ClearTimetable(year, System.Convert.ToInt32(season), ref Err, ref n);
            return Err;
        }
        [System.Web.Services.WebMethod]
        public static string UpdateTT(string year, string season)
        {
            ExamTimetable ext1 = new ExamTimetable();
            string Err = ""; 
            ext1.UpdateTimetable(year, System.Convert.ToInt32(season), ref Err);
            return Err;
        }

        protected void GridView1_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditDay")
            {

                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                string s = row.Cells[1].Text;
                DateTime t1 = new DateTime();t1=Convert.ToDateTime(s);
                DateTime t2 = new DateTime(t1.Year, t1.Month, t1.Day);
                DateTime t3 = t2.AddHours(20);
                ScheduledComponentList scl1 = new ScheduledComponentList();
                scl1.LoadList_orderbyStudentDate(t2, t3);
                s= row.Cells[1].Text; 

            }
        }
    }
}