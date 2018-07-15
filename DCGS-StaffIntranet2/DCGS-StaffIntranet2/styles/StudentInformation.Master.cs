using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;
using Microsoft.Owin;
using System.Web.UI.WebControls;

namespace StudentInformation.styles
{
    public partial class StudentInformation : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //check here to ensure proper authentication and roles?
            string email = ""; bool Is_Staff = false;
            bool Is_Student = false; SchoolCaptain.Visible = false;
            IOwinContext ctx = Request.GetOwinContext();
            ClaimsPrincipal user = ctx.Authentication.User;
            IEnumerable<Claim> claims = user.Claims;
            foreach (Claim c2 in claims)
            {
                if (c2.Type == "email") email = c2.Value;
                if (c2.Type == "SchoolOfficial") SchoolCaptain.Visible = (c2.Value.ToUpper() == "TRUE");
                if (c2.Type == "is_staff") Is_Staff = (c2.Value.ToUpper() == "TRUE");
                if (c2.Type == "is_student") Is_Student = (c2.Value.ToUpper() == "TRUE");
            }
#if DEBUG

#else

            if ((!Is_Staff)&&(!Is_Student)) Response.Redirect("../admin/content/Logout.aspx?email=" + email);// - go away!
                                                                                                             //for development server
                                                                                                             //if (Is_Student) Response.Redirect("/StudentInformation/StartForm.aspx");//if we are here as Student - go away!
                                                                                                             //if(!Is_Staff) Response.Redirect("/content/Logout.aspx?email="+ email);// - go away!

#endif
            Label2.Text = "Logged on as:   " + email;

        }
    }
}