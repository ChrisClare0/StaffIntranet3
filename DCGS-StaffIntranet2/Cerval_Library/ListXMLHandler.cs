using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Json;
//see  https://blog.udemy.com/json-serializer-c-sharp/

namespace Cerval_Library
{
    public class  ListJsonHandler : IHttpJsonHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }
        public void ProcessRequest(HttpContext context)
        {

            Stream fred1 = context.Request.GetBufferedInputStream();
            StreamReader fred2 = new StreamReader(fred1);
            string s3 = fred2.ReadToEnd();
            System.Collections.Specialized.NameValueCollection fred4 = new System.Collections.Specialized.NameValueCollection();
            fred4 = context.Request.Headers;
            /*    old logging stuff
            string filename = context.Server.MapPath(@"~/App_Data/headers.txt");
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw1 = new StreamWriter(fs);
            string[] s4 = new string[20];
            foreach (string key in fred4.AllKeys)
            {
                s4 = fred4.GetValues(key);
                sw1.WriteLine(key + ":" + s4[0]);
            }
            sw1.Close(); fred2.Close();
            */


            XmlDocument doc = new XmlDocument();
            GroupListControl gc1 = new GroupListControl();
             
            string s1 = "";
            s1=context.Request.QueryString["Group"];
            char[] c1 = new char[1]; c1[0]=',';
            string[] sf = context.Request.QueryString["Fields"].Split(c1);
            int no_fields = sf.Count();
            Group g1 = new Group(); g1.Load(s1, DateTime.Now);
            Cerval_Library.Listitem l = new Listitem(s1, g1._GroupID);
            gc1.Groups.Add(l);
            gc1.ListDate = DateTime.Now.AddDays(2);
           
            foreach (string s2 in sf)
            {
                 foreach (GroupListControl.DisplayItem d in gc1.DisplayItems)
                 {
                     if (s2 == d._name.ToString())
                     {
                         d._display = true;
                     }
                 }
             }

             gc1.save();


            //so now to hand generate json
             string s = gc1.GenerateJson2();

             context.Response.Clear();
             context.Response.ContentType = "application/json";
             context.Response.Write(s);
             context.Response.End();

        }
        public FunctionType GetHelpFunction()
        {
            FunctionType f1 = new FunctionType("List.sync", "Returns a list of students in a group at the current date");
            Parameter p1 = new Parameter("Group", true, "", "The code for the group (eg 10NE1)"); f1.m_parameters.Add(p1);
            string s = "A comma separated list of the items to be returned. Any of the following: ";
            foreach (string val in Enum.GetNames(typeof(GroupListControl.DisplayItem.Fields)))
            {
                s += val + ",";
            }
            Parameter p2 = new Parameter("Fields", false, "", s); f1.m_parameters.Add(p2);

            return f1;
        }
    }

    public class MessageJsonHandler : IHttpJsonHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }
        public FunctionType GetHelpFunction()
        {
            FunctionType f1 = new FunctionType("Message.sync", "Gets the current messages for student.");
            Parameter p1 = new Parameter("Type", true, "Student", "1 for student current"); f1.m_parameters.Add(p1);
            Parameter p2 = new Parameter("Id", true, "", "The id - Adno/StudentId/GroupId/etc"); f1.m_parameters.Add(p2);
            return f1;
        }

        public void ProcessRequest(HttpContext context)
        {
            string s = "";
            string s_type = context.Request.QueryString["Type"].ToUpper();
            string s_id = context.Request.QueryString["Id"];
            string s3 = context.Request.QueryString["Parameters"];
            /*
            string s4 = context.Request.QueryString["Key"];
            if (s4 != "1sva64HxxX_cz8IcXZrwhVRlDzhwRSex4G0XxuUn9RTQ")
            {
                s = "{\"Error\" : \"Id not same as logged on user \" User is " + context.User.Identity.Name.ToString() + "}";
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.Write(s);
                context.Response.End();
                return;
            }
            */
            //try to get user info???
            Utility u1 = new Utility();
            //Guid PersonID = u1.GetPersonID(context.User.Identity.Name.ToString());
            //Guid PersonID = u1.GetPersonIdfromRequest(context.Request);
            /*  this to check we are logged in as same person??
            if (PersonID.ToString() != s2)
            {
                //ought to return clean error json
                s="{\"Error\" : \"Id not same as logged on user \" User is " + context.User.Identity.Name.ToString() + "}";
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.Write(s);
                context.Response.End();
                return;    
            }
             * */

            switch (s_type)
            {
                //ACTIVE_MESSAGES and ALL_MESSAGES have student ID in s_id
                case "ACTIVE_MESSAGES": s = GenerateJSON_Messages(new Guid(s_id), true); break;
                case "ALL_MESSAGES": s = GenerateJSON_Messages(new Guid(s_id), false); break;

                case "EXAM_ENTRIES": s = GenerateJSONExamEntries(new Guid(s_id)); break;

                case "GET_DATATYPES": s = GetDataTypes(); break;
                case "GET_GROUPDATA": s = GetGroupData(new Guid(s_id), s3); break;
                case "GET_STAFFID_FROMAPPSLOGIN": s = GetStaffId(s_id, s3); break;
                case "GET_STAFF_TEACHINGSETS": s = GetStaffSets(s_id); break;

                case "STAFF_TIMETABLE": s = GenerateJSONtimetable(s_id, false, TT_writer.TimetableType.Staff); break;//todays tt returned
                case "STAFF_TIMETABLE_FULL": s = GenerateJSONtimetable(s_id, true, TT_writer.TimetableType.Staff); break;//full week tt returned

                case "STUDENT_TIMETABLE": s = GenerateJSONtimetable(s_id, false, TT_writer.TimetableType.Student); break;//todays tt returned
                case "STUDENT_TIMETABLE_FULL": s = GenerateJSONtimetable(s_id, true, TT_writer.TimetableType.Student); break;//full week tt returned
                case "STUDENT_RESULTS": s = GenerateJSONResults(new Guid(s_id)); break;
                case "STUDENT_RETAKES": s = GenerateJSONRetakes(new Guid(s_id)); break;

                //MESSAGE_DELIVERED has MessageStudent_id in s_id
                case "MESSAGE_DELIVERED": s = DeliverMessage(new Guid(s_id)); break;
                case "MASTER_SHEET": s = GenerateMasterSheet(s_id); break;
                case "ISAMS_MASTER_SHEET": s = ISAMSGenerateMasterSheet(s_id); break;
                case "ISAMS_MASTER_SHEETX": s = ISAMSGenerateMasterSheetX(s_id,context); break;
                case "GETSETSFORSUBJECTS": s = GetSetsForSubject(s_id,s3); break;
                case "ISAMS_GETSETSFORSUBJECTS": s = ISAMSGetSetsForSubject(s_id, s3); break;
                case "ISAMS_GETSETSFORSUBJECTSX": s = ISAMSGetSetsForSubjectX(s_id, s3,context); break;
                case "ISAMS_GETSETMEMBERSHIPSX": s = ISAMSGetSetMembershipsX(s_id,  context); break;//id = year 
                case "ISAMS_GETSETMEMBERSHIPS": s = ISAMSGetSetMemberships(s_id); break;

                case "ISAMS_GETEXAMENTRIESX": s = ISAMSGetExamEntriesX(s_id,s3, context); break;
                case "ISAMS_GETEXAMENTRIES": s = ISAMSGetExamEntries(s_id,s3); break;
                case "ISAMS_LOADSTUDENTLISTEMAIL": s = ISAMSLoadStudentListEmail(s_id); break;// s_id is the email address 

                case "ISAMS_GETADDRESSLISTX":  s = ISAMSGetAddressListX(s_id, s3, context); break;//s_id= NCYear
                case "ISAMS_GETADDRESSLIST":   s = ISAMSGetAddressList(s_id, s3); break;

                case "ISAMS_GETSTUDENTLISTX": s = ISAMSGetStudentListX(s_id, s3, context); break;
                case "ISAMS_GETSTUDENTLIST": s = ISAMSGetStudentList(s_id, s3); break;


                default: s = "{\"Error\" : \"Type not recognised\" }"; break;
            }
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Write(s);
            context.Response.End();
        }

        private string ISAMSLoadStudentListEmail(string email)
        {
            ISAMS_Student_List sl1 = new ISAMS_Student_List();
            sl1.LoadListEmail(email);
            string result = "";
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<ISAMS_Student>));
            using (MemoryStream ms = new MemoryStream())
            {
                js.WriteObject(ms, sl1.m_list);
                result = System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            return result;
        }

        private string ISAMSGetExamEntries(string s_id, string s3)
        {

            ISAMS_ExamsEntry_List el1 = new ISAMS_ExamsEntry_List();

            if (s_id == "0"){ el1.Load(s3); }else { el1.Load(s_id, s3); }

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<ISAMS_ExamEntry>));
            MemoryStream ms = new MemoryStream();
            js.WriteObject(ms, el1.m_list);

            string result = System.Text.Encoding.Default.GetString(ms.ToArray());
            ms.Close();
            return result;
        }


        private string ISAMSGetExamEntriesX(string s_id, string s3, HttpContext context)
        {
            string result = "";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try
                {
                   result = client.DownloadString("http://10.1.3.165/admin/messagelist.sync?type=ISAMS_GETEXAMENTRIES&Id=" + s_id + "&parameters=" + s3);
                }
                catch (Exception ex)
                {
                    string s1 = ex.ToString();
                }
            }
            return result;
        }


        private string ISAMSGetAddressListX(string s_id, string s3, HttpContext context)
        {
            string result = "";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try
                {
                    result = client.DownloadString("http://10.1.3.165/admin/messagelist.sync?type=ISAMS_GETADDRESSLIST&Id=" + s_id + "&parameters=" + s3);
                }
                catch (Exception ex)
                {
                    string s1 = ex.ToString();
                }
            }
            return result;
        }

        private string ISAMSGetAddressList(string s_id, string s3)
        {

            ISAMS_AddressList al1 = new ISAMS_AddressList();

            //if (s_id == "0") { el1.Load(s3); } else { el1.Load(s_id, s3); }
            al1.load(System.Convert.ToInt32(s_id));
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<ISAMS_Address>));
            MemoryStream ms = new MemoryStream();
            js.WriteObject(ms, al1.m_list);

            string result = System.Text.Encoding.Default.GetString(ms.ToArray());
            ms.Close();
            return result;
        }





        private string ISAMSGetStudentListX(string s_id, string s3, HttpContext context)
        {
            string result = "";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try
                {
                    result = client.DownloadString("http://10.1.3.165/admin/messagelist.sync?type=ISAMS_GETSTUDENTLIST&Id=" + s_id + "&parameters=" + s3);
                }
                catch (Exception ex)
                {
                    string s1 = ex.ToString();
                }
            }
            return result;
        }

        private string ISAMSGetStudentList(string s_id, string s3)
        {

            ISAMS_Student_List sl1 = new ISAMS_Student_List(true);

            //if (s_id == "0") { el1.Load(s3); } else { el1.Load(s_id, s3); }
           
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<ISAMS_Student>));
            MemoryStream ms = new MemoryStream();
            js.WriteObject(ms, sl1.m_list);

            string result = System.Text.Encoding.Default.GetString(ms.ToArray());
            ms.Close();
            return result;
        }



        private string GetSetsForSubject(string subject, string year)
        {
            DateTime d1 = new DateTime(2016, 9, 9);
            int y = 0;int ks = 4;
            try
            {
                y = System.Convert.ToInt32(year);
                if (y > 11) ks = 5;
                if (y < 10) ks = 3;
            }
            catch { }
            CourseList cl1 = new CourseList(ks);
            string s = "";
            string ns = Environment.NewLine;
            s = "{" + ns + "\"GroupList\" :{" + ns;
            s += "\"Subject\":\"" + subject + "\"," + ns;
            s += "\"Year\":\"" + year + "\"," + ns;
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s += "\"Groups\": [" + ns;
            bool first = true;
            IndicatorSkillList skl = new IndicatorSkillList();
            foreach (Course c in cl1._courses)
            {
                if (c.CourseCode.ToUpper().Trim() == subject.ToUpper().Trim())
                {
                    GroupList gla = new GroupList(c._CourseID.ToString(), d1);
                    skl.LoadList(c._CourseID);
                    foreach (Group g in gla._groups)
                    {
                        if (g._GroupCode.StartsWith(year))
                        {
                            if (first) { first = false;}else { s += "," + ns; }
                            s += "{" + ns + "\"GroupCode\": \"" + g._GroupCode + "\"," + ns;
                            s += "\"ID\": \"" + g._GroupID + "\"" + ns;
                            s += "}";
                        }
                    }

                }
            }
            s += ns + "],";
            

            s += "\"Skills\": [" + ns;
            first = true;
            foreach (IndicatorSkill sk in skl.m_list)
            {
                if (first) { first = false; } else { s += "," + ns; }
                s += "{" + ns + "\"Skill\": \"" + sk.SkillText + "\"," + ns;
                s += "\"Id\": \"" + sk.SkillId.ToString() + "\"" + ns;
                s += "}";
            }

            s += ns + "],";

            ReportPolicyList rpl = new ReportPolicyList();
            s += "\"Collections\": [" + ns;
            first = true;
            foreach (ReportPolicy r in rpl.m_list)
            {
                if ((r.CollectionTo > DateTime.Now) && (r.AcademicYear == y))//future collection for this year...
                {
                    if (first) { first = false; } else { s += "," + ns; }
                    s += "{" + ns + "\"Name\": \"" + r.Name + "\"," + ns;
                    s += "\"Id\": \"" + r.OutputPolicyId.ToString() + "\"," + ns;
                    s += "\"CollectionTo\": \"" + r.CollectionTo.ToShortDateString() + "\"" + ns;
                    s += "}";
                }
            }
            s += ns + "]";

            s += ns + "}" + ns + "}";
            return s;
        }

        private string ISAMSGetSetsForSubjectX(string subject, string year,HttpContext context)
        {
            string result = "";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {


                try
                {
                    //byte[] response = client.DownloadData("10.1.84.130/admin/messagelist.sync?type=ISAMS_GETSETSFORSUBJECTS&Id=MA&parameters=13");
                    //byte[] response = client.UploadValues("http://10.1.84.130/admin/messagelist.sync?type=ISAMS_GETSETSFORSUBJECTS&Id="+subject+"&parameters="+year, new System.Collections.Specialized.NameValueCollection()
                    //{ { "Id",subject }, {"type","ISAMS_GETSETSFORSUBJECTS" }, { "parameters", "13"} });
                    result = client.DownloadString("http://10.1.3.165/admin/messagelist.sync?type=ISAMS_GETSETSFORSUBJECTS&Id=" + subject + "&parameters=" + year);
                    //result = System.Text.Encoding.UTF8.GetString(response);
                    
                }
                catch (Exception ex)
                {
                    string s1 = ex.ToString();
                    s1 = s1;
                }
            }
            return result;
        }
        private string ISAMSGenerateMasterSheetX(string GroupCode,HttpContext context)
        {
            string result = "";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                ///client.Headers.Add("user-agent", "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko");
                //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                try
                {
                    //byte[] response = client.DownloadData("10.1.84.130/admin/messagelist.sync?type=ISAMS_GETSETSFORSUBJECTS&Id=MA&parameters=13");
                    byte[] response = client.UploadValues("http://10.1.3.165/admin/messagelist.sync?type=ISAMS_MASTER_SHEET&Id=" + GroupCode + "&parameters=13", new System.Collections.Specialized.NameValueCollection()
                    { { "Id", GroupCode }, {"type","ISAMS_MASTER_SHEET" }, { "parameters", "13"} });
                    result = System.Text.Encoding.UTF8.GetString(response);
                }
                catch (Exception ex)
                {
                    string s1 = ex.ToString();
                    s1 = s1;
                }
            }
            return result;
        }

        private string ISAMSGetSetMembershipsX(string Year, HttpContext context)
        {
            string result = "";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try
                {
                    byte[] response = client.UploadValues("http://10.1.3.165/admin/messagelist.sync?type=ISAMS_GETSETMEMBERSHIPS&Id=" + Year+ "&parameters=13", new System.Collections.Specialized.NameValueCollection()
                    { { "Id", Year }, {"type","ISAMS_GETSETMEMBERSHIPS" }, { "parameters", "13"} });
                    result = System.Text.Encoding.UTF8.GetString(response);
                }
                catch (Exception ex)
                {
                    string s1 = ex.ToString();
                    s1 = s1;
                }
            }
            return result;
        }

        private string ISAMSGetSetMemberships(string Year)
        {
            ISAMS_Student_Set_List Issl1 = new ISAMS_Student_Set_List();
            Issl1.Load(Year, true);

            DateTime t1 = new DateTime(); t1 = DateTime.Now;

            string s = "";string ns = Environment.NewLine;
            s = "{" + ns + "\"StudentsSets\" :{" + ns;
            s += "\"Year\":\"" + Year + "\"," + ns;
            s += "\"Generated\": \"" + t1.ToString() + "\"," + ns;
            s += "\"Students\": [" + ns;
            bool FirstStudent = true;bool FirstSet = true;
            string CurrentStudentId = "";int n = 0;
            foreach (ISAMS_Student_Set iss1 in Issl1.m_list)
            {
                if (CurrentStudentId != iss1.student.ISAMS_SchoolId)
                {
                    CurrentStudentId = iss1.student.ISAMS_SchoolId;
                    if (FirstStudent) { FirstStudent = false; } else { s +="}"+ ns+"]"+ns+"}," + ns; }
                    FirstSet = true;
                    s += "{" + ns + "\"Adno\": \"" + iss1.student.Adno.ToString() + "\"," + ns;
                    s += "\"Surname\": \"" + iss1.student.Surname + "\"," + ns;
                    s += "\"Forename\": \"" + iss1.student.FirstName + "\"," + ns;
                    s += "\"PreferedName\": \"" + iss1.student.PreferedName + "\"," + ns;
                    s +="\"Gender\": \"" + iss1.student.Gender + "\"," + ns;
                    s += "\"Form\": \"" + iss1.student.Form + "\"," + ns;
                    s += "\"PP\": \"" + "" + "\"," + ns;                    //TODO
                    s += "\"StudentId\": \"" + iss1.student.ISAMS_SchoolId + "\"," + ns;
                    s += "\"Groups\":[" + ns;
                    // now the groups...
                    n++;
                }
                if (FirstSet) { FirstSet = false; } else { s += "}," + ns; };
                s += "{"+ns+ "\"GroupCode\": \"" + iss1.set.SetCode + "\"," + ns;
                //s += "\"GroupId\": \"" + iss1.set.Id.ToString() + "\"," + ns;
                s += "\"SubjectCode\":\"" + iss1.set.SubjectCode + "\"," + ns;
                s += "\"SubjectName\":\"" + iss1.set.SubjectName + "\"," + ns;
                //s += "\"StaffCode\":\"" + iss1.set.StaffCode + "\"," + ns;//not this is iSAMS link to staff from set NOT timetable...
                //s += "\"StaffEmail\": \"" + iss1.set.StaffEmail + "\"," + ns;
                //s += "\"StaffFirstName\": \"" + iss1.set.StaffFirstName + "\"," + ns;
                //s += "\"StaffSurname\": \"" + iss1.set.StaffSurname + "\"," + ns;
                s += "\"StaffFullName\":\"" + iss1.set.StaffTitle+" "+iss1.set.StaffFirstName+" "+iss1.set.StaffSurname + "\"," + ns;
                //if (n > 2) break;
            }
            s += ns +"}"+ns+ "]" + ns + "}" +ns+"]"+ns+"}"+ ns + "}";
            return s;

        }

        private string ISAMSGetSetsForSubject(string subject, string year)
        {
            DateTime d1 = new DateTime(2016, 9, 9);
            d1 = DateTime.Now;
            int y = 0; 
            try{ y = System.Convert.ToInt32(year);}catch { }
            ISAMS_Set_List ISl1 = new ISAMS_Set_List(subject, y);
            string s = "";
            string ns = Environment.NewLine;
            s = "{" + ns + "\"GroupList\" :{" + ns;
            s += "\"Subject\":\"" + subject + "\"," + ns;
            s += "\"Year\":\"" + year + "\"," + ns;
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s += "\"Groups\": [" + ns;
            bool first = true;
            

            foreach (ISAMS_Set is1 in ISl1.m_list)
            {
                if (first) { first = false; } else { s += "," + ns; }
                s += "{" + ns + "\"GroupCode\": \"" + is1.SetCode + "\"," + ns;
                s += "\"ID\": \"" + is1.Id.ToString() + "\"," + ns;
                s += "\"SubjectCode\":\"" + is1.SubjectCode + "\"," + ns;
                s +="\"SubjectName\":\""+is1.SubjectName + "\"," + ns;
                s += "\"StaffCode\":\"" + is1.StaffCode + "\"," + ns;//not this is iSAMS link to staff from set NOT timetable...
                s += "\"StaffEmail\": \"" + is1.StaffEmail + "\"," + ns;
                s += "\"StaffFirstName\": \"" + is1.StaffFirstName + "\"," + ns;
                s += "\"StaffSurname\": \"" + is1.StaffSurname + "\"," + ns;
                s += "}";
            }
            s += ns + "],";
            s += ns + "}" + ns + "}";
            return s;
        }

        private string GenerateMasterSheet(string GroupCode)
        {
            //for testing set datetime forward?
            DateTime t1 = new DateTime(2016, 9, 5);
            Group g1 = new Group();g1.Load(GroupCode, t1);
            PupilGroupList pp1 = new PupilGroupList();pp1.AddToList(GroupCode, t1);
            string s = "";
            string ns = Environment.NewLine;
            s = "{" + ns + "\"GroupDetails\" :{" + ns;
            s += "\"Group\":\"" + GroupCode + "\"," + ns;
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s += "\"Students\": [" + ns;
            bool first = true;
            foreach (SimplePupil p in pp1.m_pupilllist)
            {
                ReportList repl1 = new ReportList(p.m_StudentId.ToString(), g1._CourseID, t1.AddMonths(-5), t1);
                if (first) { first = false; } else { s += "," + ns; }
                s += "{" + ns + "\"Adno\": \"" + p.m_adno.ToString() + "\"," + ns;
                s += "\"Surname\": \"" +p.m_Surname + "\"," + ns;
                s += "\"Forename\": \"" + p.m_GivenName + "\"," + ns;
                s += "\"PreferedName\": \"" + p.m_InformalName + "\"," + ns;
                s += "\"Form\": \"" + p.m_form + "\"," + ns;
                s += "\"PP\": \"" + p.m_InReceiptPupilPremium.ToString() + "\"," + ns;
                s += "\"StudentId\": \"" + p.m_StudentId.ToString() + "\"," + ns;
                foreach(ReportValue r in repl1.m_list)
                {

                    s += "\"ReportValue\": \"" + p.m_StudentId.ToString() + "\"," + ns;
                }



                s += "}";

            }
            s += ns + "]" + ns + "}" + ns + "}";
            return s;
            //Surname, Forename, AdNo, Preferred name, Form, PP, [SEN, Medical (need to check DP on these…)] or just “details” link, group, target (9to1?) (with confidence limits?), most recent slider position (across year boundaries too).
        }

        private string ISAMSGenerateMasterSheet(string GroupCode)
        {
    
            DateTime t1 = new DateTime();t1 = DateTime.Now;

            string s = "";
            ISAMS_Student_List Isl1 = new ISAMS_Student_List(GroupCode);
            string ns = Environment.NewLine;
            s = "{" + ns + "\"GroupDetails\" :{" + ns;
            s += "\"Group\":\"" + GroupCode + "\"," + ns;
            s += "\"Generated\": \"" + t1.ToString() + "\"," + ns;
            s += "\"Students\": [" + ns;
            bool first = true;
            foreach (ISAMS_Student Is in Isl1.m_list)
            {
                //ReportList repl1 = new ReportList(p.m_StudentId.ToString(), g1._CourseID, t1.AddMonths(-5), t1);
                if (first) { first = false; } else { s += "," + ns; }
                s += "{" + ns + "\"Adno\": \"" + Is.Adno.ToString() + "\"," + ns;
                s += "\"Surname\": \"" + Is.Surname + "\"," + ns;
                s += "\"Forename\": \"" + Is.FirstName + "\"," + ns;
                s += "\"PreferedName\": \"" + Is.PreferedName + "\"," + ns;
                s += "\"Form\": \"" + Is.Form + "\"," + ns;
                s += "\"PP\": \"" + "" + "\"," + ns;                    //TODO
                s += "\"StudentId\": \"" + Is.ISAMS_SchoolId.ToString() + "\"," + ns;
                s += "}";

            }
            s += ns + "]" + ns + "}" + ns + "}";
            return s;
            //Surname, Forename, AdNo, Preferred name, Form, PP, [SEN, Medical (need to check DP on these…)] or just “details” link, group, target (9to1?) (with confidence limits?), most recent slider position (across year boundaries too).
        }

        private string GetStaffSets(string staff_code)
        {


            Utility u = new Utility();
            string staffid = u.Get_StaffID(staff_code);
            PupilPeriodList m_ppl = new PupilPeriodList();
            string s = "";
            string ns = Environment.NewLine;
            s = "{" + ns + "\"GroupList\" :{" + ns;
            s += "\"Staff\":\""+staffid+"\"," + ns;
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s += "\"Groups\": [" + ns;
            bool first = true;

            DateTime Time = new DateTime(); Time = DateTime.Now;
            m_ppl.LoadList("dbo.tbl_Core_Staff.StaffId", staffid, false, Time);
            GroupList gl1 = new GroupList();
            gl1.LoadList_NonNewDawnOnly(Time, GroupList.GroupListOrder.GroupName);
            foreach (Group g in gl1._groups)
            {
                foreach (ScheduledPeriod p in m_ppl.m_pupilTTlist) 
                { 
                    if (p.m_groupcode == g._GroupCode) 
                    {
                        if (first) { first = false; }
                        else { s += "," + ns; }
                        s += "{" + ns + "\"Name\": \"" + g._GroupName + "\"," + ns;
                        s += "\"GroupCode\": \"" + g._GroupCode + "\"," + ns;
                        s += "\"GroupId\": \"" + g._GroupID + "\"," + ns;
                        s += "\"PrimaryAdministrative\": \"" + g._GroupPrimaryAdministrative + "\"," + ns;
                        s += "\"GroupRegistrationYear\": \"" + g._GroupRegistrationYear + "\"," + ns;
                        s += "\"ValidFrom\": \"" + g._StartDate + "\"," + ns;
                        s += "\"ValidTo\": \"" + g._EndDate + "\"," + ns;
                        s += "}";
                        break; 
                    } 
                }
            }
            s += ns + "]," ;

            IndicatorSkillList skl = new IndicatorSkillList();
            skl.LoadList(new Guid("68df9b54-06fc-480e-9bf9-b0eeb6a03771"));//physics


            s += "\"Skills\": [" + ns;
            first = true;
            foreach (IndicatorSkill sk in skl.m_list)
            {
                if (first) { first = false; } else { s += "," + ns; }
                s += "{" + ns + "\"Skill\": \"" + sk.SkillText + "\"," + ns;
                s += "\"Id\": \"" + sk.SkillId.ToString() + "\"" + ns;
                s += "}";
            }
            s += ns + "],";


            ReportPolicyList rpl = new ReportPolicyList();
            s += "\"Collections\": [" + ns;
            first = true;
            foreach (ReportPolicy r in rpl.m_list)
            {
                if ((r.CollectionTo > DateTime.Now) && (r.AcademicYear == 10))//future collection for this year...
                {
                    if (first) { first = false; } else { s += "," + ns; }
                    s += "{" + ns + "\"Name\": \"" + r.Name + "\"," + ns;
                    s += "\"Id\": \"" + r.OutputPolicyId.ToString() + "\"," + ns;
                    s += "\"CollectionTo\": \"" + r.CollectionTo.ToShortDateString() + "\"" + ns;
                    s += "}";
                }
            }
            s += ns + "]";


            s +=  ns + "}" + ns + "}";
            return s;
        }
        private string GetStaffId(string id, string param)
        {
            string s = "";
            SimpleStaff staff1 = new SimpleStaff();
            staff1.Load_AppsLogon(id);
            string ns = Environment.NewLine;
            s = "{" + ns + "\"StaffInformation\" :{" + ns;
            s += "\"StaffCode\":\""+staff1.m_StaffCode+"\"," + ns;
            s += "\"StaffId\":\"" + staff1.m_StaffId + "\"," + ns;
            s += "\"PersonId\":\"" + staff1.m_PersonId + "\"," + ns;
            s += "\"PersonName\":\"" + staff1.m_PersonGivenName+" "+staff1.m_PersonSurname + "\"," + ns;
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s +=  "}" + ns + "}";
            return s;

        }
        private string GetGroupData(Guid GroupId, string param)
        {
            //so list is for group id as given
            return "";
        }

        private string GetDataTypes()
        {
            ResultTypeList tl1 = new ResultTypeList();
            string s="";
            string ns = Environment.NewLine;
            s = "{" + ns + "\"DataTypes\" :{" + ns;
            s += "\"Type\":\"Data\"," + ns;
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s += "\"Types\": [" + ns;
            bool first = true;
            foreach (ResultType t in tl1._resulttypelist)
            {
                if (first){ first = false; }
                else { s += "," + ns; }
                s += "{" + ns + "\"Name\": \"" + t.m_ShortName.Trim() + "\"," + ns;
                s += "\"Description\": \"" + t.m_Description.Trim() + "\"," + ns;
                s += "\"IsNumeric\": \"" + t.m_Numeric.ToString() + "\"," + ns;
                s += "\"TypeId\": \"" + t.m_ResultTypeID.ToString().Trim() + "\"," + ns;
                s += "\"MaxValue\": \"" + t.m_MaxValue.ToString().Trim() + "\"," + ns;
                s += "\"MinValue\": \"" + t.m_MinValue.ToString().Trim() + "\"," + ns;
                s += "\"IsExternal\": \"" + t.m_External.ToString().Trim() + "\"," + ns;
                s += "\"Allowedvalues\": \"" + t.m_AllowedValues.Trim() + "\"" + ns;
                s += "}";
            }
            s += ns + "]" + ns + "}" + ns + "}";
            return s;
        }
        private string GenerateJSONResults(Guid Student_id) 
        {
            JSONHelpers j = new JSONHelpers();
            string s= j.GenerateResultsJSON("4ff051b8-9e94-4c2a-addd-b11bd2925257", ResultGrid.GridDisplayType.External);
            return s;

        }
        private string GenerateJSONExamEntries(Guid Student_id) { return "{\"Error\" : \"Type not yet implimented\" }"; }
        private string GenerateJSONRetakes(Guid Student_id) { return "{\"Error\" : \"Type not yet implimented\" }"; }

        private string GenerateJSON_Messages(Guid StudentID,bool ActiveOnly)
        {
            MessageStudentList msl1 = new MessageStudentList();
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentID);
            msl1.LoadListStudent(StudentID); string s = "";
            string ns = Environment.NewLine;
            s = "{" + ns + "\"MessageList\" :{" + ns;
            s += "\"Type\":\"Student\"," + ns;
            s += "\"Student\":\"" + pupil1.m_GivenName + " " + pupil1.m_Surname + "\"," + ns;
            s += "\"Adno\":\"" + pupil1.m_adno.ToString() + "\"," + ns;
            s += "\"StudentId\":\"" + StudentID.ToString() + "\"," + ns;
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s += "\"Messages\": [" + ns;
            bool first = true;

            foreach (MessageStudent m in msl1.m_list)
            {
                if ((ActiveOnly && !m.Delivered) || (!ActiveOnly))
                {
                    SimpleStaff staff1 = new SimpleStaff(m._Message.StaffId);
                    if (first)
                    { first = false; }
                    else { s += "," + ns; }
                    s += "{" + ns + "\"Message\": \"" + m._Message.Msg.ToString().Trim() + "\"," + ns;
                    s += "\"Staff\": \"" + staff1.m_StaffCode + "\"," + ns;
                    s += "\"ValidFrom\": \"" + m._Message.ValidFrom.ToShortDateString() + "\"," + ns;
                    s += "\"ValidUntil\": \"" + m._Message.ValidUntil.ToShortDateString() + "\"," + ns;
                    s += "\"MessageId\": \"" + m.MessageId + "\"," + ns;
                    s += "\"MessageStudnetId\": \"" + m.Id + "\"," + ns;
                    s += "\"Delivered\": \"" + m.Delivered.ToString() + "\"" + ns;
                    s += "}";
                }
            }
            s += ns + "]" + ns + "}" + ns + "}";
            return s;
        }
        private string DeliverMessage(Guid MessageId)
        {
            MessageStudent m = new MessageStudent();
            m.Load(MessageId);
            m.DateDelivered = DateTime.Now.AddMinutes(-1);
            m.Delivered = true;
            m.Save();
            string s = "{\"Success\" : \"Message marked as delivered\" }";
            return s;
        }
        private string GenerateJSONtimetable(string BaseId, bool fullTT, TT_writer.TimetableType type1)
        {
            TT_writer TT1 = new TT_writer();string s="";
            DateTime Time = new DateTime(); Time = DateTime.Now;
            PupilPeriodList m_ppl = new PupilPeriodList();



            Time = new DateTime(Time.Year, Time.Month, Time.Day, 12, 0, 0);//set to midday..
            int day_no = -1;
            switch (Time.DayOfWeek)
            {
                case DayOfWeek.Monday: day_no = 0; break;
                case DayOfWeek.Tuesday: day_no = 1; break;
                case DayOfWeek.Wednesday: day_no = 2; break;
                case DayOfWeek.Thursday: day_no = 3; break;
                case DayOfWeek.Friday: day_no = 4; break;
                case DayOfWeek.Saturday: day_no = 5; break;
                case DayOfWeek.Sunday: day_no = 6; break;
                default: day_no = -1; break;
            }

            switch (type1)
            {
                case TT_writer.TimetableType.None:
                    break;
                case TT_writer.TimetableType.Student:
                    {
                        SimplePupil sp1 = new SimplePupil(); sp1.Load_Left(BaseId.ToString());
                        if (sp1.m_StudentId == Guid.Empty) { s = "{\"Error\" : \"Student not found \"}"; return s; }
                        m_ppl.LoadList("StudentId", sp1.m_StudentId.ToString(), true, Time);
                        s = TT1.OutputTT_Json(type1, ref m_ppl, Time, day_no, sp1.m_StudentId, sp1.m_GivenName + " " + sp1.m_Surname, fullTT);
                    }
                    break;
                case TT_writer.TimetableType.Staff:
                    {
                        SimpleStaff ss1 = new SimpleStaff(new Guid(BaseId));
                        m_ppl.LoadList("dbo.tbl_Core_Staff.StaffId", BaseId.ToString(), false, Time);
                        s = TT1.OutputTT_Json(type1, ref m_ppl, Time, day_no, ss1.m_StaffId, ss1.m_PersonGivenName + " " +ss1.m_PersonSurname, fullTT);
                    }
                    break;
                case TT_writer.TimetableType.Room:
                    {
                        SimpleRoom sr1 = new SimpleRoom(new Guid(BaseId));
                        m_ppl.LoadList("dbo.tbl_Core_Rooms.RoomId", BaseId.ToString(), false, Time);
                        s = TT1.OutputTT_Json(type1, ref m_ppl, Time, day_no, sr1.m_RoomID, sr1.m_roomcode, fullTT);
                    }
                    break;
                default:
                    s = "{\"Error\" : \"Type not recognised \"}";
                    break;
            }
            return s;
        }

    }


    public class TTJsonHandler : IHttpJsonHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            Guid id1 = new Guid();string Object_Code="";
            TT_writer.TimetableType type1 = new TT_writer.TimetableType(); type1 = TT_writer.TimetableType.None;
            DateTime Time = new DateTime(); Time = DateTime.Now;
            PupilPeriodList m_ppl = new PupilPeriodList();
            SimpleStaff ss1 = new SimpleStaff();
            SimpleRoom sr1 = new SimpleRoom();
            SimplePupil sp1 = new SimplePupil();

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            string s_date = context.Request.QueryString["Date"];

            if (s_date != null)
            {
                try
                {
                    Time = System.Convert.ToDateTime(s_date);
                }
                catch
                {
                    context.Response.Write("{\"Error\" : \"Date not in correct format \"}");
                    context.Response.End();
                    return;
                }
            }
            int day_no = -1;
            Time = new DateTime(Time.Year, Time.Month, Time.Day, 12, 0, 0);//set to midday..
            switch (Time.DayOfWeek)
            {
                case DayOfWeek.Monday: day_no = 0; break;
                case DayOfWeek.Tuesday: day_no = 1; break;
                case DayOfWeek.Wednesday: day_no = 2; break;
                case DayOfWeek.Thursday: day_no = 3; break;
                case DayOfWeek.Friday: day_no = 4; break;
                case DayOfWeek.Saturday: day_no = 5; break;
                case DayOfWeek.Sunday: day_no = 6; break;
                default: day_no=-1;break;
            }

            string s_staff = context.Request.QueryString["Staff"];
            if (s_staff != null)
            {
                s_staff = s_staff.ToUpper().Trim();
                ss1 = new SimpleStaff(s_staff);Object_Code=s_staff;
                if (ss1.m_StaffId == Guid.Empty) { context.Response.Write("{\"Error\" : \"Staff code not found \"}"); context.Response.End(); return; }
                type1 = TT_writer.TimetableType.Staff; id1 = ss1.m_StaffId;
                m_ppl.LoadList("dbo.tbl_Core_Staff.StaffId", ss1.m_StaffId.ToString(), false, Time);
            }
            string s_room = context.Request.QueryString["Room"];
            if (s_room != null)
            {
                s_room = s_room.ToUpper().Trim();
                sr1 = new SimpleRoom(s_room);Object_Code=s_room;
                if (sr1.m_RoomID==Guid.Empty) { context.Response.Write("{\"Error\" : \"Room code not found \"}"); context.Response.End(); return; }
                type1 = TT_writer.TimetableType.Room; id1 = sr1.m_RoomID;
                m_ppl.LoadList("dbo.tbl_Core_Rooms.RoomId", sr1.m_RoomID.ToString(), false, Time);
            }
            string s_student = context.Request.QueryString["Student"];
            if (s_student != null)
            {
                s_student = s_student.ToUpper().Trim();
                sp1.Load_Left(System.Convert.ToInt32(s_student));//load even if not on role
                if (sp1.m_StudentId == Guid.Empty) { context.Response.Write("{\"Error\" : \"Student not found \"}"); context.Response.End(); return; }
                Object_Code=s_student;
                type1 = TT_writer.TimetableType.Student; id1 = sp1.m_StudentId;
                m_ppl.LoadList("dbo.tbl_Core_Student_Groups.StudentId", sp1.m_StudentId.ToString(), true, Time);
            }
            if (type1 == TT_writer.TimetableType.None)
            {
                context.Response.Write("{\"Error\" : \"Must have one of Staff, Room or Student in parameter list \"}");
                context.Response.End();
                return;
            }
            //need to remove any scheduled periods which are over-ridden by cover etc periods...
            m_ppl.Clean_for_cover();

            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(SimpleStaff));
            //ser.WriteObject(context.Response.OutputStream, ss1);

            if ((day_no >= 0)&&(type1!=TT_writer.TimetableType.None))
            {
                TT_writer TT1 = new TT_writer();
                context.Response.Write(TT1.OutputTT_Json(type1, ref m_ppl, Time, day_no, id1,Object_Code,false));
            }
            context.Response.End();
        }

        public FunctionType GetHelpFunction()
        {
            FunctionType f1 = new FunctionType("timetable.sync", "Returns the timetable for a room or student or memebr of staff on a certain day");
            Parameter p1 = new Parameter("Date", true, "", "Date on which to return timetable"); f1.m_parameters.Add(p1);
            Parameter p2 = new Parameter("Staff", false, "", "Staff code - one of Staff/Room/Student required"); f1.m_parameters.Add(p2);
            Parameter p3 = new Parameter("Room", false, "", "Room code - one of Staff/Room/Student required"); f1.m_parameters.Add(p3);
            Parameter p4 = new Parameter("Student", false, "", "Student admission number - one of Staff/Room/Student required"); f1.m_parameters.Add(p4);
            return f1;
        }
    }

    public interface IHttpJsonHandler : IHttpHandler
    {
        FunctionType GetHelpFunction();
    }

    public class StaffFreeAtTimeJsonHandler : IHttpJsonHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            DateTime Start= new DateTime();DateTime End = new DateTime();
            context.Response.Clear(); context.Response.ContentType = "application/json";
            string s_date = context.Request.QueryString["Start"];
            if (s_date != null)
            {
                try
                {
                    Start = System.Convert.ToDateTime(s_date);
                }
                catch
                {
                    context.Response.Write("{\"Error\" : \"Start not in correct format \"}");
                    context.Response.End();
                    return;
                }
            }
            string e_date = context.Request.QueryString["End"];
            if (e_date != null)
            {
                try
                {
                    End = System.Convert.ToDateTime(e_date);
                }
                catch
                {
                    context.Response.Write("{\"Error\" : \"End not in correct format \"}");
                    context.Response.End();
                    return;
                }
            }

            if (Start.Date != End.Date)
            {
                context.Response.Write("{\"Error\" : \"Start and End must be same day \"}");
                context.Response.End();
                return;
            }

            //ok so it the db
            StaffList sl1 = new StaffList(); sl1.LoadList(Start, true);
            sl1.Restrict_to_Feee_at(Start, End);
            StaffList sl2 = new StaffList();
            List<string> results1 = new List<string>();
            foreach (SimpleStaff s1 in sl1.m_stafflist)
            {
                if (s1.m_valid)
                { sl2.m_stafflist.Add(s1); results1.Add(s1.m_StaffCode); }
            }
            //List<Type> fred1 = new List<Type>();fred1.Add(typeof(SimpleStaff));
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(StaffList),fred1);
            //ser.WriteObject(context.Response.OutputStream, sl2);
            //DataContractJsonSerializer ser1 = new DataContractJsonSerializer(typeof(List<string>));
            //ser1.WriteObject(context.Response.OutputStream, results1);
            context.Response.End();
        }
        public FunctionType GetHelpFunction()
        {
            FunctionType f1 = new FunctionType("StaffFreeAt.sync", "Returns a list of staff free at the given Date/Time");
            Parameter p1 = new Parameter("Start", true, "", "The date/time for start of period in dd/mm/yyyy hh:mm:ss format"); f1.m_parameters.Add(p1);
            Parameter p2 = new Parameter("End", true, "", "The date/time for end of period in dd/mm/yyyy hh:mm:ss format"); f1.m_parameters.Add(p2);
            return f1;
        }
    }

    public class Parameter
    {
        public Parameter(string name, bool required, string defaut, string description)
        {
            m_name = name; m_required = required; m_default = defaut; m_description = description;
        }
        public string m_name;
        public string m_description;
        public bool m_required;
        public string m_default;

    }
    public class FunctionType
    {
        public FunctionType(string url, string description)
        {
            m_url = url; m_description = description;
        }

        public string m_description;
        public string m_url;
        public List<Parameter> m_parameters = new List<Parameter>();
    }
    public class HelpJsonHandler : IHttpHandler
    {
        public HelpJsonHandler()
        {
            TTJsonHandler t1 = new TTJsonHandler();
            Functions.Add(t1.GetHelpFunction());
            ListJsonHandler l1 = new ListJsonHandler();
            Functions.Add(l1.GetHelpFunction());
            StaffFreeAtTimeJsonHandler f1 = new StaffFreeAtTimeJsonHandler();
            Functions.Add(f1.GetHelpFunction());
           
        }

        public List<FunctionType> Functions = new List<FunctionType>();
        public bool IsReusable
        {
            get { return false; }
        }


        public void ProcessRequest(HttpContext context)
        {
            Stream fred1 = context.Request.GetBufferedInputStream();
            StreamReader fred2 = new StreamReader(fred1);
            string s3=fred2.ReadToEnd();

            /* old logging stuff
            System.Collections.Specialized.NameValueCollection fred4 = new System.Collections.Specialized.NameValueCollection();
            fred4 = context.Request.Headers;
            string filename = context.Server.MapPath(@"~/App_Data/headers.txt");
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw1 = new StreamWriter(fs);
            string[] s4 = new string[20];
            foreach (string key in fred4.AllKeys)
            {
                s4 = fred4.GetValues(key);
                sw1.WriteLine(key + ":" + s4[0]);
            }
            sw1.Close();fred2.Close();
            */


            string ns = Environment.NewLine;
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            string s = "{" + ns + "\"Help\" :{" + ns + "\"Generated\": \"" + DateTime.Now.ToString() + "\"" + ns;
            s += "\"Functions\": [" + ns;

            foreach(FunctionType  f in Functions)
            {

                s += "{" + ns;
                s += "\"url\": \"" + f.m_url + "\"," + ns;
                s += "\"description\": \"" + f.m_description + "\"," + ns;
                s += "\"parameters\": ["+ns;
                foreach (Parameter p1 in f.m_parameters)
                {
                    s+= "{" + ns;
                    s += "\"name\": \"" + p1.m_name + "\"," + ns;
                    s += "\"description\": \"" + p1.m_description + "\"," + ns;
                    s += "\"required\": \"" + p1.m_required.ToString() + "\"," + ns;
                    s += "\"Default\": \"" + p1.m_default + "\"," + ns;
                    s+="}"+ns;
                }
                s += "]" + ns;
            }
            s += "]" + ns + "}" + ns + "}" + ns;

            context.Response.Write(s);
            context.Response.End();
        }
    }


}
