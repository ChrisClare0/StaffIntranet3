using System;
using System.IO;
using Cerval_Library;


namespace StudentInformation
{
    public partial class ExamResults : System.Web.UI.Page
    {
        public Guid PersonID = new Guid();
        string scontent = "";
        protected void Page_Load(object sender, EventArgs e)
        {     
            if (!IsPostBack)
            {             
                Utility u1 = new Utility();
                PersonID = u1.GetPersonIdfromRequest(Request);
                if(PersonID==new Guid("20744211-d0f0-4e69-af84-020c1023dfda"))//cc
                {
                    PersonID = u1.GetPersonIDX(@"CHALLONERS\mayank.sharma");//development 
                    PersonID = u1.GetPersonIDX(@"CHALLONERS\mayank.sharma");//development
                    PersonID = u1.GetPersonIDX(@"CHALLONERS\alex.robinson");//development
                }
#if DEBUG
                u1.Is_student = true;
                PersonID = u1.GetPersonIDX(@"CHALLONERS\mayank.sharma");//development    
                PersonID = u1.GetPersonIDX(@"CHALLONERS\alex.robinson");//development    
                PersonID = u1.GetPersonIDX(@"CHALLONERS\michael.taylor2");//development   
                PersonID = u1.GetPersonIDX(@"CHALLONERS\jack.bowe");//development   
                PersonID = u1.GetPersonIDX(@"CHALLONERS\alexander.lynch");//development 
                PersonID = u1.GetPersonIDX(@"CHALLONERS\adam.bosher");//development 
                PersonID = u1.GetPersonIDX(@"CHALLONERS\sam.coyne");//development 
                PersonID = u1.GetPersonIDX(@"CHALLONERS\kieran.Davis");//development
                PersonID = u1.GetPersonIDX(@"CHALLONERS\William.Kitchener");//development  
                PersonID = u1.GetPersonIDX(@"CHALLONERS\Oscar.Jones");//development
#endif
                Resultgrid1.StudentId = u1.GetStudentId(PersonID).ToString();
                Resultgrid1.DisplayType = ResultGrid.GridDisplayType.External;
                Resultgrid1.Visible = false;

                Resultgrid2.StudentId = Resultgrid1.StudentId;
                Resultgrid2.DisplayType = ResultGrid.GridDisplayType.Module;
                Resultgrid2.Visible = false;

                Resultgrid3.StudentId = Resultgrid1.StudentId;
                Resultgrid3.DisplayType = ResultGrid.GridDisplayType.Predicted;
                Resultgrid3.Visible = false;
       
                if ((PersonID != Guid.Empty) && (u1.Is_student))
                {
                    Resultgrid1.Visible = true; Resultgrid2.Visible = true; Resultgrid3.Visible = true;
                }
                else
                 {
                  ContentAdd("<center> No Data to display</center>");
                }

                //need to check embargo time
                DateTime d0 = new DateTime(2018, 08, 13, 0, 0, 10);//embargo time
                DateTime d1 = new DateTime(2018, 08, 16, 08, 00, 00);//release time
                Server_Heading.InnerHtml = "";
                try
                {
                    Cerval_Configuration c0 = new Cerval_Configuration("StudentInformation_ResultsEmbargoStart");
                    d0 = System.Convert.ToDateTime(c0.Value);
                    Cerval_Configuration c1 = new Cerval_Configuration("StudentInformation_ResultsEmbargoRelease");
                    d1 = System.Convert.ToDateTime(c1.Value);
#if DEBUG
                    d1 = new DateTime(2012, 3, 08, 6, 00, 05);//release time             
#endif


                    if ((DateTime.Now > d0) && (DateTime.Now < d1))
                    {
                        Resultgrid1.Visible = false;
                        Resultgrid2.Visible = false;
                        Resultgrid3.Visible = false;
                        Server_Heading.Visible = false;
                        VIdatadiv.Visible = false;
                        scontent = "<h3> Results screen will be available again at " + d1.ToLongTimeString() + " on " + d1.ToLongDateString() + ".</h3><br />   (time now " + DateTime.Now.ToShortTimeString() + " on " + DateTime.Now.ToLongDateString() + ")";

                    }
                }
                catch (Exception  e3)

                {
                    Response.Redirect("../content/StartForm.aspx");
                }


                string path = "";
                bool msg1 = false;
                string adno = u1.GetAdmissionNumber(PersonID).ToString();

                Cerval_Configuration c = new Cerval_Configuration("StudentInformation_ResultsMessageDisplay");

                //see if we want the message stuff
                string s = c.Value.ToUpper();
                if ((PersonID != Guid.Empty)&&(s=="TRUE"))
                {
                    if ((u1.Is_student) && (u1.GetAdmissionNumber(PersonID) != 0))
                    {
                        path = Server.MapPath(@"App_Data/Results_names.txt");
                        try
                        {
                            using (StreamReader sr = new StreamReader(path))
                            {
                                while ((s = sr.ReadLine()) != null)
                                {
                                    if (s.Contains(adno))
                                    {
                                        msg1 = true;
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                if (msg1)
                {
                    path = Server.MapPath(@"App_Data/Message1.txt");
                    try
                    {
                        using (StreamReader sr = new StreamReader(path))
                        {
                            Server_Heading.InnerHtml = sr.ReadToEnd();
                        }
                    }
                    catch
                    {
                    }
                }
                //see if we want the message stuff

                Cerval_Configuration c3 = new Cerval_Configuration("StudentInformation_ResultsVILink");
                s = c3.Value.ToUpper();
                if (s == "TRUE")
                {
                string[] split1 = new string[20];
                char[] c1 = new char[1]; c1[0] = (char)0x09;
                path = Server.MapPath(@"App_Data/VIOffers.txt");
                //this file is the output from the GCSE options package
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while ((s = sr.ReadLine()) != null)
                        {
                            split1 = s.Split(c1);
                            if (split1[0].Contains(adno))
                            {
                                VIdatadiv.InnerHtml = "Information regarding your Sixth Form Application can be viewed <a href=\"StudentSixthFormApp.aspx\">here</a>."; ;
                            }
                        }
                    }
                }
                catch { }
                }

                servercontent.InnerHtml = scontent;
            }
        }
        protected void ContentAdd(string s){scontent+=s;return;}

    }
}
