using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Cerval_Library;
using Microsoft.Owin;

namespace DCGS_Staff_Intranet2
{
    public partial class home_x : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //testing

            IOwinContext ctx = Request.GetOwinContext();
            ClaimsPrincipal user = ctx.Authentication.User;
            IEnumerable<Claim> claims = user.Claims;
            foreach (Claim c2 in claims)
            {
                TextBox1.Text += c2.Type.ToString() + ":    " + c2.Value.ToString() + Environment.NewLine;
            }
            return;
            /*

            string s1 = Request.QueryString["token"];
            string s = "";
            string email = "";
            JwtSecurityToken token = new JwtSecurityToken(s1);
            Claim[] claims = token.Claims.ToArray<Claim>();
            List<Claim> fred1 = new List<Claim>();
            foreach (Claim c4 in claims)
            {
                fred1.Add(c4);
            }
            for (int i = 0; i < claims.Length; i++)
            {
                s = claims[i].Value;
                if (claims[i].Type == "email")
                {
                    email = s;
                }
            }
            var id = new ClaimsIdentity(fred1, DefaultAuthenticationTypes.ApplicationCookie);
            Request.GetOwinContext().Authentication.SignIn(id);
            Response.Redirect("/x/beta/admin-test/home.aspx");
            /*
            Utility u = new Utility(); Guid staff_id = new Guid();
            s = u.GetStaffCodefromGoogeLogin(email, out staff_id);
            if (u.Is_staff)
            {
                Claim test1 = new Claim("Staff_code", s); fred1.Add(test1);
                Claim test2 = new Claim("Staff_id", staff_id.ToString()); fred1.Add(test2);
                var id = new ClaimsIdentity(fred1, DefaultAuthenticationTypes.ApplicationCookie);
                Request.GetOwinContext().Authentication.SignIn(id);
                Response.Redirect("/home.aspx");
            }
            else
            {
                //bad login... well google is OK but not one of our staff
                Response.Redirect("/bad_login.aspx");
            }
            */

        }
    }
}