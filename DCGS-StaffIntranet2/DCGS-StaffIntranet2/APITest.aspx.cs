using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2
{
    public partial class APITest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                /*
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko");
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    client.Headers.Add("Authorization", "SNAeoB88JTmAXfNTAfpR");

                    string response = "";
                    try
                    {
                        
                        response = client.DownloadString("https://sandbox.apply-for-teacher-training.service.gov.uk/api/v1/applications?since=2018-10-01T10:00:00Z");
                    }
                    catch (Exception e2)
                    {
                        string s5455 = e2.ToString();
                        s5455 = e2.InnerException.ToString();
                    }
                    string s = response;
                }
               */

                using (var client = new System.Net.Http.HttpClient() { DefaultRequestHeaders = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "{SNAeoB88JTmAXfNTAfpR}") } })
                {
                    client.BaseAddress = new Uri("https://sandbox.apply-for-teacher-training.service.gov.uk/api/v1");

                    HttpRequestMessage mess = new HttpRequestMessage(HttpMethod.Post, "https://sandbox.apply-for-teacher-training.service.gov.uk/api/v1/test-data/generate");
                    mess.Content = new StringContent("{\"Bearer\": \"SNAeoB88JTmAXfNTAfpR\" \"meta\": { \"attribution\": {\"full_name\":\"ChrisClare\" \"email\":\"cc@challoners.org\" \"user_id\":\"1234\"}} \"timestamp\":\"2019 - 10 - 16T15: 33:49.216Z\"  }");
                    string s = "{\"Bearer\": \"SNAeoB88JTmAXfNTAfpR\" \"meta\": { \"attribution\": {\"full_name\":\"ChrisClare\" \"email\":\"cc@challoners.org\" \"user_id\":\"1234\"} \"timestamp\":\"2019 - 10 - 16T15: 33:49.216Z\" } }";
                    var res1 = client.SendAsync(mess).Result;


                    // var response = client.GetAsync("/applications ? since = 2018 - 10 - 01T10:00:00Z");
                    var response = client.GetAsync("/applications? since = 2018 - 10 - 01T10:00:00Z ").Result;
                    response = response;

                }
                    return;
            }

        }
    }
}