using System;
using System.Collections.Generic;
using Cerval_Library;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Newtonsoft.Json;
using Google.Apis.Sheets.v4.Data;

namespace DCGS_Staff_Intranet2.StudentInformation
{
    public partial class GoogleData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }
        protected void  LoadData()
        {
            string credPath = @"d:\\project-id-9301225348112901974-d05770b23edc.json";
            DateTime t1 = new DateTime(); t1 = DateTime.Now;
            string json =  File.ReadAllText(credPath);
            Newtonsoft.Json.Linq.JObject cr = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(json);
            string private_key = (string)cr.GetValue("private_key");
            string email = (string)cr.GetValue("Client_email");

            ServiceAccountCredential credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("104602038154026657417")
            {
                Scopes = new[] { SheetsService.Scope.Spreadsheets }
            }.FromPrivateKey(private_key));


            // Create the service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "CC_Sheets_Service",
            });
            var fred = service.Spreadsheets.Get("10usxblP6yGQTB2VkYeA5xuLUMepf1TuIcyhig4H1ygk");
            fred.IncludeGridData = true;

            String spreadsheetId = "10usxblP6yGQTB2VkYeA5xuLUMepf1TuIcyhig4H1ygk";
            String range = "Sheet1!A1:AB210";

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            
            // now who are we??
            Guid PersonID = new Guid();
            Utility u1 = new Utility();
            PersonID = u1.GetPersonIdfromRequest(Request);
            if (PersonID == new Guid("20744211-d0f0-4e69-af84-020c1023dfda"))//cc
            {
                PersonID = u1.GetPersonIDX(@"CHALLONERS\william.kitchener");//development 
                //PersonID = u1.GetPersonIDX(@"CHALLONERS\George.pickford");//development 
            }
            int adno = u1.GetAdmissionNumber(PersonID);
            SimplePupil pupil1 = new SimplePupil();pupil1.Load(adno);
            
            string s1 = "<p ><h3 align=\"center\">";
            s1 += "Predicted Results for "; 
            s1 += pupil1.m_GivenName + " " + pupil1.m_Surname + "</h3></p>";
            s1 += "<br /><TABLE BORDER   class=\"ResultsTbl\"  align=\"center\" >";
            s1+= "<TR><TD>Course Code</TD><TD> Course Name </TD><TD> Grade </TD></TR>";

            int max_cols = values[0].Count;
            int max_rows = values.Count;

            //need to get names for course codes
            CourseList cl1 = new CourseList(5);


            for (var i = 1; i < max_rows; i++)
            {
                string s = (string)values[i][0];
                int ad2 = System.Convert.ToInt32(s);
                string cse = "";string cseN = "";
               
                if (ad2 == adno)
                {
                    max_cols = values[i].Count;
                    for (var j = 5; j < max_cols; j++)
                    {
                        s = (string)values[i][j];
                        cse = (string)values[0][j];
                        if (s != "")
                        {
                            foreach(Course c in cl1._courses) { if (c.CourseCode == cse) { cseN = c.CourseName; break; } }

                            //for 2018 only!!!
                            if (cse == "MFA") { cseN = "Further Maths"; }; if (cse == "MF") { cseN = "Mathematics"; }

                            s1 += "<TR><TD>" + cse + "</TD><TD> " +cseN+ "</TD><TD> " + s + " </TD></TR>";
                        }
                        cseN = "";
                    } 
                    break;
                }
            }
            s1 += "</TABLE></br>";
            TimeSpan ts1 = new TimeSpan(); ts1 = DateTime.Now - t1;
            s1 += "Time Taken :  " + ts1.TotalSeconds.ToString() + "<br/>";
            content.InnerHtml = s1;
        }
    }
}