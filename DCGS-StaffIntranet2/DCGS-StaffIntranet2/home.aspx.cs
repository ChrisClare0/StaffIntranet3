#if DEBUG
#warning DEBUG is defined
#endif
#define XAdmin_test
using System;
using Cerval_Library;
using System.Web;
using System.Security.Claims;
using System.IO;

namespace DCGS_Staff_Intranet2
{
    public partial class home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

#if DEBUG

#else
                Context.User=Request.GetOwinContext().Authentication.User;
                //debug
                try { 
                //StreamWriter sw2 = new StreamWriter(@"c:/_TEMP_/StaffLogs1.txt", true);
                //sw2.WriteLine("In Home.aspx");bool found = false;
                foreach (Claim c in Request.GetOwinContext().Authentication.User.Claims)
                {
                    if (c.Type == "staff_code")
                    {
                        //sw2.WriteLine(c.Value);found = true;
                    }
                        if (c.Type == "person_id")
                        {
                           // sw2.WriteLine(c.Value); found = true;
                        }
                    }
                //if (!found) sw2.WriteLine("no staff code");
                //sw2.Close();
                }
                catch (Exception ex)
                {
                    //StreamWriter sw2 = new StreamWriter(@"c:/_TEMP_/StaffLogs2.txt", true);
                    //sw2.WriteLine("In Home.aspx but claims failed...");
                    //sw2.WriteLine(ex.ToString());
                    //sw2.WriteLine(Request.GetOwinContext().Authentication.User.ToString());
                    //sw2.Close();
                }


                Utility u = new Utility();
                string staff_code = u.GetsStaffCodefromRequest(Request);

#endif
                DateTime d0 = new DateTime(2012, 3, 07, 0, 0, 10);//embargo time
                DateTime d1 = new DateTime(2012, 3, 08, 6, 00, 05);//release time
                string s = "";
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
                        s = "<h4>Staff Intranet service is currently unavailable. ";
                        Cerval_Configuration c2 = new Cerval_Configuration("StaffIntranet_EmbargoReason");
                        s += c2.Value;
                        s += "<br />  <br /></h4>";
                        Response.Write(s); return;
                    }
                }
                catch
                { }

#if DEBUG
                //Response.Redirect("/content/ReportGenerator.aspx");
                //Response.Redirect("/content/SENEdit.aspx");
                //Response.Redirect("/content/ExamResultsforStaff.aspx");
                Response.Redirect("/content/testform.aspx");
                //Response.Redirect("/content/StartForm.aspx");
                //Server.Transfer("/content/StudentsComplexLists.aspx");
                //Response.Redirect("/content/music_stuff/UploadSets.aspx");
                Response.Redirect("/content/PupilAcademicProfile.aspx");
                Response.Redirect("/content/NextTT/NextYearTTChoice.aspx");
                Response.Redirect("/content/EditRooms.aspx");
                //Response.Redirect("/content/StartForm.aspx");
#else

#if Admin_test
                Response.Redirect("../admin-test/content/StartForm.aspx");   //for deploy version  TESTING site
#else

                
                switch (RadioButtonList1.SelectedIndex)
                {
                    case 0:
                        Response.Redirect("../admin/content/StartForm.aspx");   //for deploy version
                        break;
                    case 1:
                        Response.Redirect("../4matrix/");
                        break;
                    case 2:
                        Response.Redirect("../admin/content/Booking/Menu.aspx");
                        break;

                    default:
                        Response.Redirect("../admin/content/StartForm.aspx");   //for deploy version
                        break;
                }

                Response.Redirect("../admin/content/StartForm.aspx");   //for deploy version
#endif

                //Response.Redirect("/content/StartForm.aspx");         //for development server
                // have you corrected the links in StaffIntranetMaster.cs???

#endif
            }
        }
    }
}