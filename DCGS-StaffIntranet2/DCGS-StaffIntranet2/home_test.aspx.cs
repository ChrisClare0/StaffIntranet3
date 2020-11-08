#define XAdmin_test
using System;
using System.Net;
using System.IO;


namespace DCGS_Staff_Intranet2
{
    public partial class home_test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string s = "";

            s = Response.Status.ToString();
            //StreamWriter sw2 = new StreamWriter(@"c:/_TEMP_/temp.txt", true);
            //sw2.WriteLine("NEW:" + s);
            //sw2.Close();
        }



        protected void Button1_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko");

            Stream data = client.OpenRead("https://accounts.google.com/.well-known/openid-configuration");
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            Label1.Text = s;

            int i = s.IndexOf("authorization_endpoint");
            s = s.Substring(i);
            i = s.IndexOf(":");
            s = s.Substring(i + 3);
            i = s.IndexOf(",");
            s = s.Substring(0, i - 1);
            Label1.Text = s;


            WebClient client1 = new WebClient();
            client.Headers.Add("user-agent", "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko");
            client.QueryString.Add("client_id", "1071688065998-77amat8g8k6gsbuhp9gpfuseuc00fvve.apps.googleusercontent.com");
            client.QueryString.Add("response_type", "code");
            client.QueryString.Add("scope", "openid email");
            client.QueryString.Add("nonce", "fred1234");
            //client.QueryString.Add("scope", "openid email DriveService.Scope.Drive DriveService.Scope.DriveFile");

#if DEBUG
            client.QueryString.Add("redirect_uri", "http://localhost:50418/home_test2.aspx");
#else

#if Admin_test
#warning Admin_test is defined

            client.QueryString.Add("redirect_uri", "https://internal.challoners.com/x/beta/admin-test/home_test2.aspx");
#else
            client.QueryString.Add("redirect_uri", "https://internal.challoners.com/x/admin/home_test2.aspx");
#endif

#endif



            //StreamWriter sw2 = new StreamWriter(@"c:/_TEMP_/temp.txt", true);
            //sw2.WriteLine("CLIENT:" + client.QueryString.ToString());
            //sw2.Close();


            Guid g1 = new Guid();g1 = Guid.NewGuid();
            //client.QueryString.Add("state", "%3D138r5719ru3e1%26url%3Dhttps://oauth2-login-demo.example.com/myHome");
            client.QueryString.Add("state", g1.ToString());
            Stream data1 = client.OpenRead("https://accounts.google.com/o/oauth2/v2/auth");
            //Stream data1 = client.OpenRead(s);



            StreamReader reader1 = new StreamReader(data1);
            s = reader1.ReadToEnd();




            //Response.Write(s);
            //Label1.Text = s;
            s = "https://accounts.google.com/o/oauth2/v2/auth?client_id=1071688065998-c6mkpr28h0lu4ts5n1f2givml865t0jv.apps.googleusercontent.com";
#if DEBUG

            s += "&response_type=code&scope=openid%20profile%20email&redirect_uri=http://localhost:50418/home_test2.aspx";
#else
#if Admin_test
            s += "&response_type = code &scope = openid % 20email &redirect_uri =https://internal.challoners.com/x/beta/admin-test/home_test2.aspx";
#else
            s += "&response_type=code&scope=openid%20email&redirect_uri=https://internal.challoners.com/x/admin/home_test2.aspx";
#endif
#endif
            //s += " &state = security_token % 3D138r5719ru3e1 % 26url % 3Dhttps://oauth2-login-demo.example.com/myHome";
            s += "&state=" + g1.ToString();
            s += "&nonce=fred1234";
            Response.Write(s);
            Response.Redirect(s);

        }
    }
}