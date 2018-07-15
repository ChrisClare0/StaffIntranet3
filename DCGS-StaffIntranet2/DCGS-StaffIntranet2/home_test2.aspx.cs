#define XAdmin_test
using System;
using System.Linq;
using System.Net;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using Cerval_Library;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Microsoft.Owin.Security;
using System.IO;

namespace DCGS_Staff_Intranet2
{
    public partial class home_test2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string s = Response.Status.ToString();
                //StreamWriter sw2 = new StreamWriter(@"c:/_TEMP_/temp.txt", true);
                //sw2.WriteLine("NEW:" + s);
                s = Request.QueryString["state"];
                string s1 = Request.QueryString["code"];
                s = Request.QueryString["state"];
                //sw2.WriteLine(s);
                //FileStream f1 = new FileStream(@"c:/_TEMP_/temp.txt", FileMode.OpenOrCreate);



                string result="";

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko");
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    try {
                        byte[] response =
                        client.UploadValues("https://www.googleapis.com/oauth2/v4/token", new System.Collections.Specialized.NameValueCollection()
                        {
           { "code", s1 },
           {"client_id", "1071688065998-77amat8g8k6gsbuhp9gpfuseuc00fvve.apps.googleusercontent.com"},
                    {"client_secret", "GVmcAzy1Cq92-FIRCPi_s-CI" },
                    { "grant_type", "authorization_code"},
#if DEBUG
                    {"redirect_uri", "http://localhost:50418/home_test2.aspx" }
#else
#if Admin_test
                        {"redirect_uri", "https://internal.challoners.com/x/beta/admin-test/home_test2.aspx" }
#else

                        {"redirect_uri", "https://internal.challoners.com/x/admin/home_test2.aspx" }
#endif
#endif

                        });
                        result = System.Text.Encoding.UTF8.GetString(response);
                    }
                    catch(Exception ex)
                    {
                       // sw2.WriteLine("EXCEPTION:" + ex.ToString());
                    }




                    //sw2.WriteLine("now the result:\n");
                    //sw2.WriteLine(result);
                    JwtPayload f = new JwtPayload();
                    f = JsonExtensions.DeserializeJwtPayload(result);
                    s = (string)f["id_token"];
                    string email = "";string personid = "";
                    JwtSecurityToken token = new JwtSecurityToken(s);
                    Claim[] claims = token.Claims.ToArray<Claim>();
                    List<Claim> fred1 = new List<Claim>();
                    foreach (Claim c4 in claims)
                    {
                        fred1.Add(c4);
                        //sw2.WriteLine(c4.Type + " : " + c4.Value);
                    }
                    for (int i = 0; i < claims.Length; i++)
                    {
                        s = claims[i].Value;
                        if (claims[i].Type == "email")
                        {
                            email = s;
                        }

                    }


                    Utility u = new Utility();
                    List<Claim> fred2 = new List<Claim>();
                    u.ValidateGoogeLogin(email, out fred2);
                    if (u.Is_staff || u.Is_student)
                    {
                        //sw2.WriteLine("after Google Validate");
                        foreach (Claim c4 in fred2)
                        {
                            fred1.Add(c4);
                            //sw2.WriteLine(c4.Type + " : " + c4.Value);
                            if(c4.Type == "person_id")
                            {
                                personid = c4.Value;
                            }
                        }
                        try
                        {
                            HttpContext.Current.Session["RunSession"] = "1";
                            //workround for wierdness in logonsessions....
                            // See http://stackoverflow.com/questions/20737578/asp-net-sessionid-owin-cookies-do-not-send-to-browser/
                            var id = new ClaimsIdentity(fred1, DefaultAuthenticationTypes.ApplicationCookie);
                        Request.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, id);
                        }
                        catch(Exception ex)
                        {

                            //StreamWriter sw3 = new StreamWriter(@"c:/_TEMP_/catchinhometest2.txt", true);
                            //sw3.WriteLine(ex.ToString());
                            
                            //sw3.Close();
                        }


#if DEBUG
                        Response.Redirect("/content/StartForm.aspx");
#else

#if Admin_test
                        Response.Redirect("/x/beta/admin-test/home.aspx");
#else
                        //sw2.WriteLine("ResponseHeaders");
                        foreach (string s5 in Response.Headers.Keys)
                        {
                            //sw2.WriteLine("Header:"+s5);
                            foreach (string s6 in Response.Headers.GetValues(s5))
                            {
                                //sw2.WriteLine(s6);
                            }
                            
                        }

                        //sw2.Close();
                        //StreamWriter sw5 = new StreamWriter(@"c:/_TEMP_/beforedirect.txt", true);
                       // sw5.WriteLine(email+"   :    "+personid);
                       // sw5.Close();

                        Response.Redirect("../admin/home.aspx");
                        //Response.Redirect("/x/admin/home.aspx");
#endif
                        //StreamWriter sw4 = new StreamWriter(@"c:/_TEMP_/aferredirect.txt", true);
                        //sw4.WriteLine(email);
                        //sw4.Close();
#endif
                    }
                    else
                    {
                        logonmessage.InnerHtml = "<h2>You must logon to this site using a valid DCGS Google account. <br/><br/>If you continue to experience difficulties please contact IT support.</h2>";
                        logonmessage.InnerHtml += "</br>The email you tried to use was  " + email;
                    }

                }

                    /*
                    Google.Apis.Services.BaseClientService.Initializer fred_base = new Google.Apis.Services.BaseClientService.Initializer();

                    DriveService sx = new DriveService(fred_base);
                    FilesResource.ListRequest request = sx.Files.List();
                    FileList files = request.Execute();
                    foreach (File item in files.Items)
                    {
                        s = item.Title;
                        s = s;
                    }
                    */
                }
            }


        protected void Button1_Click(object sender, EventArgs e)
        {
#if DEBUG
            Response.Redirect("/content/StartForm.aspx"); ;
#else
#if Admin_test
                        Response.Redirect("/x/beta/admin-test/home.aspx");
#else
            Response.Redirect("/x/admin/home.aspx");
#endif
#endif
        }
    }
}
 