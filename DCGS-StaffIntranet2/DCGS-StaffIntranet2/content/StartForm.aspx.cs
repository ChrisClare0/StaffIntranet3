using System;
using System.Security.Claims;
using Cerval_Library;
using System.Web;
using Microsoft.Owin;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.IO;

namespace DCGS_Staff_Intranet2.content
{
    public partial class StartForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
#if DEBUG
                /*
                ISAMS_TimetableScheduleList i1 = new ISAMS_TimetableScheduleList();
                ISAMS_Period_List ipl1 = new ISAMS_Period_List();
                ISAMS_RoomList irl1 = new ISAMS_RoomList();
                i1.LoadListCurrentTT();string isamss = "";
                string isamsR = "";string isamsP = "";string isamsD = "";
                char c11 = (char)0x09; 
                foreach(ISAMS_TimeTableSchedule is2 in i1.m_list)
                {
                    isamsR = "";isamsP = "";isamsD = "";
                    foreach(ISAMS_Period ip in ipl1.m_ISAMS_PeriodList)
                    {
                        if (ip.Id == is2.PeriodId) { isamsP = ip.ShortName; isamsD = ip.day.ToString(); break; }
                    }
                    foreach(ISAMS_Room ir in irl1.m_list)
                    {
                        if (is2.RoomId == ir.Id) { isamsR = ir.Code; break; }
                    }
                    isamss += is2.SetCode + c11 + is2.StaffInitials + c11+ isamsD+c11+isamsP+c11+isamsR+c11+Environment.NewLine;

                    //find the  group ID and ScheduledPeriod Id if possible
                    TTData t1 = new TTData();
                    Group g1 = t1.Find_Group(is2.SetCode, new DateTime(2017, 9, 6, 01, 0, 0), new DateTime(2018, 7, 31, 01, 0, 0), true);
                    ScheduledPeriodRawList schprl1 = new ScheduledPeriodRawList();schprl1.Load_for_Group(g1._GroupID);

                }
                //servercontent.InnerHtml = isamss;
                Utility u = new Utility();
                u.WriteToLogFile(@"D:/temp2.txt", isamss);
                */

#endif

                System.Security.Principal.IIdentity fred = HttpContext.Current.User.Identity;
                string s = HttpContext.Current.User.Identity.Name;
                string s5 = Context.User.Identity.Name;
                string email="";

                IOwinContext ctx = Request.GetOwinContext();
                ClaimsPrincipal user = ctx.Authentication.User;
                IEnumerable<Claim> claims = user.Claims;
                foreach ( Claim c2 in claims)
                {
                    s = c2.Value;
                    if (c2.Type == "email")
                    {
                        email = s;
                    }
                    if (c2.Type=="staff_id")
                    {
                        s5 = s;
                    }
                }
                StreamWriter sw2 = new StreamWriter(@"c:/_TEMP_/StaffLogs_StartForm.txt", true);
                sw2.WriteLine(email + "  :  " + s5+DateTime.Now.ToString() + " :  " + DateTime.Now.ToString());
                sw2.Close();
                //servercontent.InnerHtml = "<BR/>This is a test site...please report issues to CC.<br/><br/>";
                ViewState.Add("date", DateTime.Now);

                Label fred2 = (Label) Master.FindControl("Label2");
                if (fred2!= null)
                {
                    fred2.Text = "Logged on as:   " + email;
                }
                System.Web.UI.HtmlControls.HtmlGenericControl fred3 = (System.Web.UI.HtmlControls.HtmlGenericControl)Master.FindControl("dcgsNavBar1");
                if(fred3!= null)
                {
                    Encode en1 = new Encode();
                    string s_db = en1.GetDbConnection();
                    string[] s_dbfields = new string[10];
                    char[] splits = new char[2];splits[0] = ';';splits[1] = '=';
                    s_dbfields = s_db.Split(splits);
                    fred3.InnerText = "Staff Intranet (db:" + s_dbfields[1] + ",    Server:"+s_dbfields[3]+")";
                }


                DateTime d0 = new DateTime(2012, 3, 07, 0, 0, 10);//embargo time
                DateTime d1 = new DateTime(2012, 3, 08, 6, 00, 05);//release time
                try
                {
                    Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_EmbargoStart");
                    d0 = System.Convert.ToDateTime(c.Value);
                    Cerval_Configuration c1 = new Cerval_Configuration("StaffIntranet_EmbargoRelease");
                    d1 = System.Convert.ToDateTime(c1.Value);

#if DEBUG
                    d1 = new DateTime(2012, 3, 08, 6, 00, 05);//release time             
#endif
                    if ((DateTime.Now > d0) && (DateTime.Now < d1))
                    {
                        Response.Redirect("../Home.aspx");
                    }
                }
                catch
                {
                }

                try
                {
                    Cerval_Configuration c2 = new Cerval_Configuration("StaffIntranet_Welcome_Screen_HTML");
                    s = c2.Value;
                }

                catch (System.Exception e1)
                {
                    s = "Configuration setting not found. " + e1.Message;
                }
                servercontent.InnerHtml += s;
                ViewState.Add("date", DateTime.Now);
            }
            else
            {

            }
        }

        public void MarkAsLeft(DateTime leavingDate, Guid StudentId)
        {
            string date_s = "CONVERT(DATETIME, '" + leavingDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET  StudentIsOnRole ='false' , ";
            s += " StudentLeavingDate = " + date_s ;
            s += " WHERE (StudentId = '" + StudentId.ToString() + "')";
            en.ExecuteSQL(s);

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            Response.Redirect("../xmatrix/selectmodel.aspx");
            //ReportScale r = new ReportScale(new Guid("5e5d8ab4-aa9a-46cf-a1c2-83352bb2ef57"));
            //string s = r.AssessmentLevelDetail.LevelName;
            //s = s;

            //toclean up left students...
            return;


            SimpleStudentList stl1 = new SimpleStudentList(SimpleStudentList.LIST_TYPE.NOFORM_ONROLE);
            Group g1 = new Group();
            StudentGroupMembershipList sg1 = new StudentGroupMembershipList();
            g1.Load("13year", new DateTime(2019, 4, 1));
            sg1.LoadList_Group(g1._GroupID,new DateTime(2019, 4, 1));

            //mark all as left in 2019

            foreach (StudentGroupMembership s1 in sg1.m_list)
            {
                MarkAsLeft(new DateTime(2019, 7, 31), s1.m_Studentid);
            }

            g1.Load("13year", new DateTime(2018, 4, 1));
            sg1.LoadList_Group(g1._GroupID, new DateTime(2018, 4, 1));

            foreach (StudentGroupMembership s1 in sg1.m_list)
            {
                MarkAsLeft(new DateTime(2018, 7, 31), s1.m_Studentid);
            }

            return;

            foreach (SimplePupil p in stl1._studentlist)
            {
                //so going to find last known rg group??
                sg1.LoadList(p.m_StudentId, new DateTime(2019, 4, 1));
                foreach (StudentGroupMembership s1 in sg1.m_list)
                {
                    g1.Load(s1.m_Groupid);
                    if (g1._GroupPrimaryAdministrative)
                    {
                        MarkAsLeft(new DateTime(2018, 7, 31), s1.m_Studentid);
                    }
                }

                sg1 = new StudentGroupMembershipList();
                sg1.LoadList(p.m_StudentId, new DateTime(2018, 4, 1));
                foreach (StudentGroupMembership s1 in sg1.m_list)
                {
                    g1.Load(s1.m_Groupid);
                    if (g1._GroupPrimaryAdministrative)
                    {
                        //so was here in 2018, but not in 2019
                        //so leaving date is 31/7/2018
                        MarkAsLeft(new DateTime(2018, 7, 31), s1.m_Studentid);

                    }
                }

                sg1 = new StudentGroupMembershipList();
                sg1.LoadList(p.m_StudentId, new DateTime(2017, 4, 1));
                foreach (StudentGroupMembership s1 in sg1.m_list)
                {
                    g1.Load(s1.m_Groupid);
                    if (g1._GroupPrimaryAdministrative)
                    {
                        //so was here in 2017 but not in 2018
                    }
                }

            }

            Response.Redirect("../content/TestForm.aspx");
            //Response.Redirect("../Exams/UploadComponentResults.aspx");
        }
    }
}
