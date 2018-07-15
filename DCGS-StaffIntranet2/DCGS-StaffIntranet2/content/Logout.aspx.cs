using Microsoft.AspNet.Identity;
using System;
using System.Web;


namespace DCGS_Staff_Intranet2.content
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //do logoout by invalidating cookie
                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                if (Request.QueryString.Count >= 1)//redirect from master page...
                {
                    string email = Request.QueryString["email"];
                    servercontent.InnerHtml = "<h2> Your email address(" + email + ") is not recognised as either a member of staff or a student at DCGS. Please contact IT support at DCGS </h2>";
                }          
                else
                {
#if DEBUG
                    Response.Redirect("/content/StartForm.aspx");
#else
                Response.Redirect("/x/admin/home.aspx");
#endif
                }
            }
        }
    }
}