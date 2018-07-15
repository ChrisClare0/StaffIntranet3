using System;
using System.Web;
using Microsoft.AspNet.Identity;

namespace StudentInformation.content
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //do logoout by invalidating cookie
                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Response.Redirect("../content/StartForm.aspx");
            }
        }
    }
}