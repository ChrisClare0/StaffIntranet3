using System;
using System.Security.Claims;
using Cerval_Library;
using System.Web;
using Microsoft.Owin;
using System.Collections.Generic;
using System.IO;
using Cerval_Library;

namespace StudentInformation
{
    public partial class StartForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Security.Principal.IIdentity fred = HttpContext.Current.User.Identity;
                string s = HttpContext.Current.User.Identity.Name;
                string s5 = Context.User.Identity.Name;
                string email = "";

                IOwinContext ctx = Request.GetOwinContext();
                ClaimsPrincipal user = ctx.Authentication.User;
                IEnumerable<Claim> claims = user.Claims;
                foreach (Claim c2 in claims)
                {
                    s = c2.Value;
                    if (c2.Type == "email")
                    {
                        email = s;
                    }
                    if (c2.Type == "person_id")
                    {
                        s5 = s;
                    }
                }
                //StreamWriter sw2 = new StreamWriter(@"c:/_TEMP_/StudentLogs_StartForm.txt", true);
                //sw2.WriteLine(email + "  :  " + s5+DateTime.Now.ToString()+" :  "+DateTime.Now.ToString());
                //sw2.Close();



                s = Request.QueryString["target"];
                if (s == "error")
                {
                    servercontent.InnerHtml = "<h3>error</h3>An unexpected error has occured. Please inform CC.";
                }
                string path = "";
                Guid PersonID = new Guid();
                Utility u1 = new Utility();
                u1.GetPersonIdfromRequest(Request);

#if DEBUG

#endif
                Cerval_Configuration c = new Cerval_Configuration("StudentInformation_HomePageMessage");
                servercontent.InnerHtml = c.Value;
                bool show_photos = false;

                Utility u2 = new Utility();
                if (u1.Is_staff && (u2.Get_StaffCode(PersonID).Trim().ToUpper() == "CC"))
                {
                    show_photos = true;
                }
                //show_photos = true;
                
                if (PersonID != Guid.Empty)
                {
                    if ((u1.Is_student) && (u1.GetAdmissionNumber(PersonID) != 0))
                    {
                        string adno = u1.GetAdmissionNumber(PersonID).ToString();
                        path = Server.MapPath(@"App_Data/Results_names.txt");
                        try
                        {
                            using (StreamReader sr = new StreamReader(path))
                            {
                                while ((s = sr.ReadLine()) != null)
                                {
                                    if (s.Contains(adno))
                                    {
                                        show_photos = true;
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (u1.Is_staff && (u2.Get_StaffCode(PersonID).Trim().ToUpper() == "CC"))
                    {
                        show_photos = true;
                    }

                }
            }
        }
    }
}
