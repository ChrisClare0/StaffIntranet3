using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(DCGS_Staff_Intranet2.Startup))]

namespace DCGS_Staff_Intranet2
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/home_test.aspx"),
                
                CookieSecure = CookieSecureOption.SameAsRequest,
                ExpireTimeSpan = new TimeSpan(30,1,0,0)  //for production - 1 month...
            });

        }
    }
}
