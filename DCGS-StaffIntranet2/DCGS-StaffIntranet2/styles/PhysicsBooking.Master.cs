using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.styles
{
    public partial class PhysicsBooking : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //check here to ensure proper authentication and roles?
                string email = ""; bool Is_Staff = false;
                bool Is_Student = false; bool PhysicsStaff=false;
                IOwinContext ctx = Request.GetOwinContext();
                ClaimsPrincipal user = ctx.Authentication.User;
                IEnumerable<Claim> claims = user.Claims;
                foreach (Claim c2 in claims)
                {
                    if (c2.Type == "email") email = c2.Value;
                    if (c2.Type == "is_staff") Is_Staff = (c2.Value.ToUpper() == "TRUE");
                    if (c2.Type == "is_student") Is_Student = (c2.Value.ToUpper() == "TRUE");
                    if (c2.Type == "PhysicsCanEdit") PhysicsStaff = (c2.Value.ToUpper() == "TRUE");
                }
#if DEBUG
                if (Is_Student) Response.Redirect("../StudentInformation/StartForm.aspx");//if we are here as Student - go away!
                if (!Is_Staff) Response.Redirect("../content/Logout.aspx?email="+ email);// - go away!
#else
                if (Is_Student) Response.Redirect("../StudentInformation/StartForm.aspx");//if we are here as Student - go away!
                if ( Is_Staff && (!PhysicsStaff)) Response.Redirect("../admin/content/StartForm.aspx");
                if (!Is_Staff) Response.Redirect("../admin/content/Logout.aspx?email=" + email);// - go away!
                                                                                                //for development server
                                                                                                //if (Is_Student) Response.Redirect("/StudentInformation/StartForm.aspx");//if we are here as Student - go away!
                                                                                                //if(!Is_Staff) Response.Redirect("/content/Logout.aspx?email="+ email);// - go away!

#endif


            }
        }
    }
}
