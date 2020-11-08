using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Cerval_Library;
using System.IO;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Google.Apis.Sheets.v4.Data;
using System.Text;
using System.Net;

namespace DCGS_Staff_Intranet2.content
{
   
    public partial class testform : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //RegisterAsyncTask(new PageAsyncTask(Test1));


                ISAMS_Student_List is1 = new ISAMS_Student_List("8104",true);

                foreach (ISAMS_Student s1 in is1.m_list)
                {
                    string s2 = s1.Surname;
                    byte[] bytes = Encoding.Default.GetBytes(s2);
                    string test1= Encoding.UTF8.GetString(bytes);
                    string s4 = s2;
                    s4 = Encoding.Default.ToString();
                    s4 = s4;

                }


                return;

            }

        }
        private void SearchDirectory(string dir, TreeNode n)
        {

            DirectoryInfo di = new DirectoryInfo(dir);
            try
            {
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo fiTemp in fi)
                {
                    TreeNode n1 = new TreeNode(fiTemp.Name, fiTemp.FullName);
                    n.ChildNodes.Add(n1);
                }
                DirectoryInfo[] dilist = di.GetDirectories();
                foreach (DirectoryInfo di1 in dilist)
                {
                    TreeNode n1 = new TreeNode(di1.Name, di1.FullName);
                    n.ChildNodes.Add(n1);
                    TreeNode n2 = new TreeNode("files", "files");
                    n1.ChildNodes.Add(n2);
                    //SearchDirectory(di1.FullName, n1);
                }
            }
            catch
            {
                //Response.Write(e1.Message + "<br>");
            }

        }

        protected List<string> Get_Registrations(string adno)
        {
            List<string> results = new List<string>();
            IntPtr token = IntPtr.Zero;

            bool isSuccess = LogonUser("test-staff", "challoners", "123Password",
            LOGON32_LOGON_NEW_CREDENTIALS,
            LOGON32_PROVIDER_DEFAULT, ref token);
            using (WindowsImpersonationContext person = new WindowsIdentity(token).Impersonate())
            {
                int i = 0; string s = ""; int imax = 0; string sfile1 = ""; string sfile = "";
                string[] fileEntries = Directory.GetFiles(@"\\registration.challoners.net\STEARsoft\Reg\data", "attend*.csv");
                foreach (string fileName in fileEntries)
                {
                    //find last one!
                    s = fileName.Substring(fileName.IndexOf("log"));
                    try
                    {
                        s = s.Substring(3, 2);
                        i = System.Convert.ToInt32(s);
                        if (i > imax) { imax = i; sfile1 = sfile; sfile = fileName; }
                    }
                    catch
                    {
                        try
                        {
                            s = s.Substring(3, 3);
                            i = System.Convert.ToInt32(s);
                            if (i > imax) { imax = i; sfile1 = sfile; sfile = fileName; }
                        }
                        catch { }
                    }



                }
                if (imax > 0)
                {
                    string s1 = "";
                    Label4.Text = sfile;
                    StreamReader sr1 = new StreamReader(sfile);
                    while (!sr1.EndOfStream)
                    {
                        s = sr1.ReadLine();
                        string[] fred = s.Split((char)(','));

                        if (fred[4] == adno)
                        {
                            results.Add(s);
                        }
                    }
                    sr1.Close();

                    //if (results.Count > 0)
                    {
                        //ought to try sfile1
                        StreamReader sr2 = new StreamReader(sfile1);
                        while (!sr2.EndOfStream)
                        {
                            s = sr2.ReadLine();
                            string[] fred = s.Split((char)(','));
                            if (fred[4] == adno)
                            {
                                results.Add(s);
                            }
                        }
                        sr2.Close();
                    }
                }



                person.Undo();
            }
            return results;

        }

        protected void TT_compare()
        {
            TTData t0 = new TTData();
            t0.Load(@"s:\tt_data1.ttd", DateTime.Now, DateTime.Now, false, false, "RG");
            TTData t1 = new TTData();
            t1.Load_DB(DateTime.Now);
            int n = 0; int n1 = 0;
            //sort out all that are the same.....
            foreach (TTData.TT_period p in t0.periodlist1.m_list)
            {
                p.SetName = p.SetName.Trim().ToUpper(); n++;
            }
            foreach (TTData.TT_period p in t1.periodlist1.m_list)
            {
                p.SetName = p.SetName.Trim().ToUpper(); n1++;
            }

            foreach (TTData.TT_period p in t0.periodlist1.m_list)
            {
                foreach (TTData.TT_period p1 in t1.periodlist1.m_list)
                {
                    if (p.Is_SameAs(p1) && p.valid && p1.valid)
                    {
                        p.valid = false;
                        p1.valid = false;
                        n++;
                        break;
                    }
                }
            }
            n = 0; n1 = 0;
            foreach (TTData.TT_period p in t0.periodlist1.m_list)
            {
                if (p.valid) n++;
            }
            foreach (TTData.TT_period p in t1.periodlist1.m_list)
            {
                if (p.valid) n1++;
            }
            TextBox1.Text = "";
            //  now find those where only room has changed
            n = 0;
            foreach (TTData.TT_period p in t0.periodlist1.m_list)
            {
                if (p.valid)
                {
                    foreach (TTData.TT_period p1 in t1.periodlist1.m_list)
                    {
                        if (p1.valid)
                        {
                            if ((p.SetName == p1.SetName) && (p.StaffId == p1.StaffId) && (p.PeriodId == p1.PeriodId))
                            {
                                //assume it is the room!!!
                                TextBox1.Text += p.SetName + "," + p.StaffCode + "," + p.DayNo.ToString() + "," + p.PeriodCode + "," + p.RoomCode;
                                TextBox1.Text += "," + p1.SetName + "," + p1.StaffCode + "," + p1.DayNo.ToString() + "," + p1.PeriodCode + "," + p1.RoomCode;
                                TextBox1.Text += Environment.NewLine;
                                p.valid = false;
                                p1.valid = false;
                                n++;
                                break;
                            }
                        }
                    }
                }
            }
            TextBox1.Text += Environment.NewLine; TextBox1.Text += Environment.NewLine;
            //  now find those where staff has changed
            n = 0;
            foreach (TTData.TT_period p in t0.periodlist1.m_list)
            {
                if (p.valid)
                {
                    foreach (TTData.TT_period p1 in t1.periodlist1.m_list)
                    {
                        if (p1.valid)
                        {
                            if ((p.SetName == p1.SetName) && (p.PeriodId == p1.PeriodId))
                            {
                                //assume it is the room!!!
                                TextBox1.Text += p.SetName + "," + p.StaffCode + "," + p.DayNo.ToString() + "," + p.PeriodCode + "," + p.RoomCode;
                                TextBox1.Text += "," + p1.SetName + "," + p1.StaffCode + "," + p1.DayNo.ToString() + "," + p1.PeriodCode + "," + p1.RoomCode;
                                TextBox1.Text += Environment.NewLine;
                                p.valid = false;
                                p1.valid = false;
                                n++;
                                break;
                            }
                        }
                    }
                }
            }

            n = 0;
            TextBox1.Text += Environment.NewLine; TextBox1.Text += Environment.NewLine;
            foreach (TTData.TT_period p in t0.periodlist1.m_list)
            {
                if (p.valid)
                {
                    TextBox1.Text += p.SetName + "," + p.StaffCode + "," + p.DayNo.ToString() + "," + p.PeriodCode + "," + p.RoomCode;
                    TextBox1.Text += Environment.NewLine;
                    p.valid = false;
                    n++;

                }
            }
            TextBox1.Text += Environment.NewLine; TextBox1.Text += Environment.NewLine;
            foreach (TTData.TT_period p in t1.periodlist1.m_list)
            {
                if (p.valid)
                {
                    TextBox1.Text += ",,,,,";
                    TextBox1.Text += p.SetName + "," + p.StaffCode + "," + p.DayNo.ToString() + "," + p.PeriodCode + "," + p.RoomCode;
                    TextBox1.Text += Environment.NewLine;
                    p.valid = false;
                    n++;

                }
            }

            n1 = 0;
            foreach (TTData.TT_period p in t0.periodlist1.m_list)
            {
                if (p.valid) n1++;
            }
            string s = "";

        }


        protected async Task Test1()
        {
            string credPath = @"d:\\project-id-9301225348112901974-d05770b23edc.json";
            DateTime t1 = new DateTime(); t1 = DateTime.Now;
            string json = File.ReadAllText(credPath);
            Newtonsoft.Json.Linq.JObject cr = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(json);
            string private_key = (string)cr.GetValue("private_key");
            string email = (string)cr.GetValue("Client_email");

            ServiceAccountCredential credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("104602038154026657417")
            {
                Scopes = new[] { SheetsService.Scope.Spreadsheets}
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
  
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);



            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            TimeSpan ts1 = new TimeSpan(); ts1 = DateTime.Now - t1;
            fred = fred;

            for( var i= 1;i<210;i++)
            {
                string s = (string)values[i][0];
                string s1 = "";
                if (s == "6752")
                {
                    s1 = (string)values[i][1];
                    s1 = s1;
                }
            }
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko");
                //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("Authorization", "SNAeoB88JTmAXfNTAfpR");

                try
                {
                    string response = client.DownloadString("https://sandbox.apply-for-teacher-training.service.gov.uk/api/v1");
                }
                catch (Exception e2)
                {
                    string s5455 = e2.ToString();
                    s5455 = e2.InnerException.ToString();
                }

            }

            return;


            ISAMS_ExamStudentAccessList list44 = new ISAMS_ExamStudentAccessList();
            list44.load(18);


            ISAMS_ExamSeatingPlan_List list33 = new ISAMS_ExamSeatingPlan_List();
            list33.Load(18);

           // ISAMS_Set_PTOList Leena = new ISAMS_Set_PTOList();
           // Leena.load(12);



            return;



            ExamComponentResultList ecrl1 = new ExamComponentResultList();
            ecrl1.Load_OptionStudent(new Guid("d22c0f3e-38ff-4a81-9c00-dc7892548f68"), new Guid("fbbd073f-b4cd-4fb1-a12f-e3a755e7cc63"));
            string s = "";
            foreach (ExamComponentResult r in ecrl1.m_list)
            {
                s = r.ComponentStatus;
            }
            Response.Redirect("../studentinformation/ComponentResults.aspx?StudentId=fbbd073f-b4cd-4fb1-a12f-e3a755e7cc63 &OptionId=d22c0f3e-38ff-4a81-9c00-dc7892548f68");




            TT_compare(); return;
            //call from button..
            RegistrationsList r1 = new RegistrationsList();
            r1.Get_Recent_Registrations("6368");
            foreach (Registrations r in r1.Registrations)
            {
                TextBox1.Text += r.m_date.ToString() + "  " + r.m_staff + Environment.NewLine;
            }

            return;


            List<string> test3 = Get_Registrations("6368");
            foreach (string s4 in test3)
            {
                TextBox1.Text += s4 + Environment.NewLine;
            }
            return;
            IntPtr token = IntPtr.Zero;
            string adno = "6368";
            bool isSuccess = LogonUser("test-staff", "challoners", "123Password",
            LOGON32_LOGON_NEW_CREDENTIALS,
            LOGON32_PROVIDER_DEFAULT, ref token);
            using (WindowsImpersonationContext person = new WindowsIdentity(token).Impersonate())
            {
                string path1 = @"\\registration.challoners.net\STEARsoft\Reg\data\cc\testfile.txt";
                //StreamReader sr1 = new StreamReader(path1);

                //string s = sr1.ReadLine();
                //Label4.Text = s;
                //s = s;
                int i = 0; s = ""; int imax = 0; string sfile = "";
                string[] fileEntries = Directory.GetFiles(@"\\registration.challoners.net\STEARsoft\Reg\data", "attend*.csv");
                foreach (string fileName in fileEntries)
                {
                    //find last one!
                    s = fileName.Substring(fileName.IndexOf("log"));
                    try
                    {
                        s = s.Substring(3, 2);
                        i = System.Convert.ToInt32(s);
                        if (i > imax) { imax = i; sfile = fileName; }
                    }
                    catch
                    {
                        try
                        {
                            s = s.Substring(3, 3);
                            i = System.Convert.ToInt32(s);
                            if (i > imax) { imax = i; sfile = fileName; }
                        }
                        catch { }
                    }



                }
                if (imax > 0)
                {
                    string s1 = "";
                    Label4.Text = sfile;

                    StreamReader sr1 = new StreamReader(sfile);
                    while (!sr1.EndOfStream)
                    {
                        s = sr1.ReadLine();
                        string[] fred = s.Split((char)(','));

                        if (fred[4] == adno)
                        {
                            s1 = fred[11];

                        }
                    }

                }


                person.Undo();
            }



            return;






            //adno	first name	surname	StudentId	CourseId	StaffId	CommentType 	poliyID	text
            Cerval_Library.TextReader text1 = new Cerval_Library.TextReader();
            Cerval_Library.TextRecord t = new TextRecord();
            string path = Server.MapPath(@"../App_Data/test.txt");
            FileStream f = new FileStream(path, FileMode.Open);
            text1.ReadTextLine(f, ref t);//header row

            while (text1.ReadTextLine(f, ref t) == Cerval_Library.TextReader.READ_LINE_STATUS.VALID)
            {
                ReportComment r = new ReportComment();
                r.m_studentId = new Guid(t.field[3]);
                r.m_courseId = new Guid(t.field[4]);
                r.m_staffId = new Guid(t.field[5]);
                r.m_commentType = System.Convert.ToInt32(t.field[6]);
                r.m_dateCreated = DateTime.Now;
                r.m_dateModified = r.m_dateCreated;
                r.m_collectionOutputPolicyId = new Guid(t.field[7]);
                r.m_content = t.field[8];
                r.Save();
            }
            f.Close();
        }

        #region imports 
        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string
        lpszUsername, string lpszDomain, string lpszPassword,
        int dwLogonType, int dwLogonProvider, ref
        IntPtr phToken);


        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = CharSet.Auto,
        SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle
        );

        [DllImport("advapi32.dll", CharSet = CharSet.Auto,
        SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr
        existingTokenHandle,
        int SECURITY_IMPERSONATION_LEVEL, ref IntPtr
        duplicateTokenHandle);
        #endregion
        #region logon consts 
        // logon types 
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        // logon providers 
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_PROVIDER_WINNT50 = 3;
        const int LOGON32_PROVIDER_WINNT40 = 2;
        const int LOGON32_PROVIDER_WINNT35 = 1;
        #endregion

        protected void Button_test2_Click(object sender, EventArgs e)
        {
            //this is button 2
        }

        protected void Button_test3_Click(object sender, EventArgs e)
        {
            //remake 9year or whatever
            string YearGroup = TextBox_YearGroup.Text;
            string GroupName = YearGroup + "Year";

            int year = DateTime.Now.Year; int month = DateTime.Now.Month;
            if (month > 8) year++;//so
            DateTime EndDate = new DateTime(year, 7, 31);
            DateTime StartDate = new DateTime(year - 1, 9, 4);

            GroupList gl1 = new GroupList();
            Utility u = new Utility();
            Group g2 = new Group();g2.Load(GroupName, DateTime.Now);
            if (!g2._valid)
            {
                g2._GroupCode = GroupName; g2._StartDate = StartDate;g2._EndDate = EndDate;g2._GroupName = GroupName;
                g2._CourseID = new Guid("f0e6828f-c6e1-4105-988c-83e350a39cc4");g2._GroupRegistrationType = 3;g2._GroupRegistrationYear = System.Convert.ToInt32(YearGroup);
                g2._GroupPrimaryAdministrative = false;g2._GroupManagedBy = Guid.Empty;
                g2.Save();
            }
            gl1.LoadList(DateTime.Now, GroupList.GroupListOrder.GroupName);
            foreach (Group g in gl1._groups)
            {
                if (g._GroupCode.ToUpper().Trim() == GroupName.ToUpper())
                {
                    //going to delete all gm here
                    StudentGroupMembershipList sgml0 = new StudentGroupMembershipList();
                    sgml0.LoadList_Group(g._GroupID, DateTime.Now);
                    foreach (StudentGroupMembership sg in sgml0.m_list)
                    {
                        sg.Delete();
                    }

                    foreach (Group g1 in gl1._groups)
                    {
                        if (g1._GroupCode.Contains("RG") && (g1._GroupCode.StartsWith(YearGroup)))
                        {
                            StudentGroupMembershipList sgml1 = new StudentGroupMembershipList();
                            sgml1.LoadList_Group(g1._GroupID, DateTime.Now);
                            foreach (StudentGroupMembership sg in sgml1.m_list)
                            {
                                StudentGroupMembership sm1 = new StudentGroupMembership();
                                sm1.m_Groupid = g._GroupID;
                                sm1.m_Studentid = sg.m_Studentid;
                                sm1.m_ValidFrom = StartDate;
                                sm1.m_ValidTo = EndDate;
                                sm1.Save();
                            }

                        }
                    }
                    break;
                }
            }
        }

        protected void ButtonDeleteLeavers_Click(object sender, EventArgs e)
        {

            ISAMS_Set_List SL1 = new ISAMS_Set_List();

            foreach (ISAMS_Set s1 in SL1.m_list)
            {
                if (s1.Year == 12)
                {
                    string s = "";
                    s = s1.SetCode;

                    ISAMS_Student_List stu1 = new ISAMS_Student_List(s1.Id);
                    foreach (ISAMS_Student s2 in stu1.m_list)
                    {
                        s = s2.ISAMS_SchoolId + "," + s1.SetCode + "," + s2.Adno.ToString() + "," + s2.Surname + "," + s2.FirstName;

                    }
                }
            }

            //process leavers
            int n = 0;
            StudentsOnRoleNotInRegGroup nr = new StudentsOnRoleNotInRegGroup();
            foreach (SimplePupil p in nr._studentlist)
            {
                StudentLeavingDetails s1 = new StudentLeavingDetails();
                s1.LeavingDate = new DateTime(2017, 9, 6);
                s1.Adno = p.m_adno;
                s1.valid = true;
                s1.SaveOffRole();
                n++;
            }
            n = n;
        }



    }

}
