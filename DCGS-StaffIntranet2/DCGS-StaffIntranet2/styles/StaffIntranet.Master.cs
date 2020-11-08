using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;
using Microsoft.Owin;
using Cerval_Library;

namespace DCGS_Staff_Intranet2
{
    public partial class StaffIntranet : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

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



                //check here to ensure proper authentication and roles?
                string email = "";bool Is_Staff = false;
                bool Is_Student = false; PhysicsBooking.Visible = false; ExamsMenu.Visible = false;
                 IOwinContext ctx = Request.GetOwinContext();
                ClaimsPrincipal user = ctx.Authentication.User;
                IEnumerable<Claim> claims = user.Claims;
                foreach (Claim c2 in claims)
                {
                    if (c2.Type == "email") email = c2.Value;
                    if (c2.Type == "PhysicsCanEdit") PhysicsBooking.Visible = (c2.Value.ToUpper() == "TRUE");
                    if (c2.Type == "is_staff") Is_Staff = (c2.Value.ToUpper() == "TRUE");
                    if (c2.Type == "is_student") Is_Student = (c2.Value.ToUpper() == "TRUE");
                    if (c2.Type == "ExamsUser") ExamsMenu.Visible = (c2.Value.ToUpper() == "TRUE");
                }
#if DEBUG
                if (Is_Student) Response.Redirect("../StudentInformation/StartForm.aspx");//if we are here as Student - go away!
                if (!Is_Staff) Response.Redirect("../content/Logout.aspx?email="+ email);// - go away!
#else
                if (Is_Student) Response.Redirect("../StudentInformation/StartForm.aspx");//if we are here as Student - go away!
                if(!Is_Staff)  Response.Redirect("../admin/home_x.aspx");// - go away!
                if (!Is_Staff) Response.Redirect("../admin/content/Logout.aspx?email="+ email);// - go away!
                //for development server
                //if (Is_Student) Response.Redirect("/StudentInformation/StartForm.aspx");//if we are here as Student - go away!
                //if(!Is_Staff) Response.Redirect("/content/Logout.aspx?email="+ email);// - go away!
                           
#endif
                Label2.Text= "Logged on as:   " + email;

            }
        }
    }
}
