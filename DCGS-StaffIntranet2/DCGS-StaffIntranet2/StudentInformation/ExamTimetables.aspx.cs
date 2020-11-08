using System;
using System.Collections;
using Cerval_Library;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;

namespace StudentInformation
{
    public partial class ExamTimetables : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                //going to try to get from iSAMS data
                iSAMSLoad(); return;

                Utility u1 = new Utility();
                DateTime t1 = new DateTime();
                DateTime t0 = new DateTime();
                t0 = DateTime.Now;
                t1 = t0.AddMonths(6);
                t0 = t0.AddMonths(-1);
                //SO   in future and up to 1 month in the past
                //string season = u1.ThisSeason(t0);

                //season = "3";

                ArrayList m_list = new ArrayList();
                Guid PersonID = new Guid();
                PersonID = u1.GetPersonIdfromRequest(Request);


#if DEBUG

                PersonID = u1.GetPersonIDX(@"CHALLONERS\charles.manning");//development

#endif

                //check they have read the exams notice...

                //string y = t0.Year.ToString();
                //bool v = false;
                //WarningNoticeRead w = new WarningNoticeRead(u1.GetStudentId(PersonID), u1.ThisSeason(t0), y, ref v);
                //if (!v)
                //{
                    //Server.Transfer("ExamWarnings.aspx");//go and make them read the notice...
                //}

                PupilDetails p1 = new PupilDetails(u1.GetStudentId(PersonID).ToString());
                {
                    string s0 = "";
                    string s = "PersonImagePage.aspx?id=" + p1.m_PersonId.ToString();
                    s = "\"" + s;
                    s += "\" width = \"110\" height=\"140\"";
                    s="<FONT FACE = \"Arial\"><p align=\"center\"> <img src=" + s + "></p>";
                    s+="<p align=\"center\">This page should be printed and brought to all exams</p>";
                    {
                        s+="<center><h2>Exam Timetable for " + p1.m_GivenName + "  " + p1.m_Surname + "</h2></center>";
                        StudentSENList sen1 = new StudentSENList(p1.m_StudentId.ToString()); s0 = "";
                        foreach (StudentSEN s1 in sen1.m_List)
                        {
                            if (s1.m_ExamsExtraTime > 0)
                            {
                                s0 += "Extra Time (" + s1.m_ExamsExtraTime.ToString() + "%)  ";
                            }
                            if (s1.m_ExamsCanType)
                            {
                                s0 += " Can Type(Please report to IT4 for any exam where you wish to type)";
                            }
                        }
                        if (s0 != "")
                        {
                            s += "<center><h3>" + s0 + "</h3></center>";
                        }
                        ScheduledComponentList scl1 = new ScheduledComponentList();
                        t0 = t0.AddMonths(-3);
                        scl1.LoadList_Student(t0, t1, p1.m_StudentId.ToString());
                        SimpleRoom room1 = new SimpleRoom();
                        s +="<BR><center><TABLE BORDER><TR>";
                        s +="<Th>Date</Th>";
                        s += "<Th>Day</Th>";
                        s +="<Th>Start Time</Th>";
                        s += "<Th>End Time</Th>";
                        s += "<Th>Paper Code</Th>";
                        s += "<Th>Paper Name</Th>";
                        s += "<Th>Room</Th>";
                        s += "<Th>Desk</Th></TR>";
                        foreach (ScheduledComponent sc in scl1.m_List)
                        {
                            s +="<TR><TD>" + sc.m_Date.ToShortDateString() + "</TD>";
                            s +="<TD>" + sc.m_Date.DayOfWeek.ToString() + "</TD>";
                            t1 = sc.m_Date;
                            s += "<TD>" + sc.m_Date.ToShortTimeString() + "</TD>";
                            t1 = t1.AddMinutes(sc.m_TimeAllowed);
                            s += "<TD>" + t1.ToShortTimeString() + "</TD>";
                            s += "<TD>" + sc.m_ComponentCode + "</TD>";
                            s  +="<TD>" + sc.m_ComponentTitle.ToLower() + "</TD>";
                            if (sc.m_RoomId != Guid.Empty)
                            {
                                room1.Load(sc.m_RoomId.ToString());
                                s+= "<TD>" + room1.m_roomcode + "</TD>";
                            }
                            else
                            {
                                s+="<TD>not yet assigned</TD>";
                            }

                            s +="<TD>" + sc.m_Desk + "</TD></TR>";
                        }
                        s += "</TABLE></center>";
                    }
                    s+="<br><center><h3>Candidate Number = " + p1.m_examNo.ToString() + " , UCI=" + p1.m_UCI + "</h3></center></Font>";
                    servercontent.InnerHtml = s;
                }
            }

        }

        private void iSAMSLoad()
        {

            //going to try to get from iSAMS data
            Utility u1 = new Utility();
            Guid PersonID = new Guid();
            PersonID = u1.GetPersonIdfromRequest(Request);
            //has old cerval personid will need the new iSAMS 
            SimplePupil p = new SimplePupil(); p.Load(u1.GetStudentId(PersonID).ToString());

            if (PersonID.ToString() == "20744211-d0f0-4e69-af84-020c1023dfda")//CC
            {
                p.Load("ce64da61-8505-4d07-9c9e-62dbe90d7dff");   //matt dagnal
            }
            string s = "";
            if (p.m_IsamsPupilId == null)
            {
                //try to find email
                //need to call load on remote server
                string result = "";
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    try
                    {
                        result = client.DownloadString("http://10.1.84.230/admin/messagelist.sync?type=ISAMS_LOADSTUDENTLISTEMAIL&Id=" + p.m_GoogleAppsLogin + "&parameters=" + "10");
                    }
                    catch (Exception ex)
                    {
                        s = "Error calling remote server : " + ex.ToString(); servercontent.InnerHtml = s; return;
                    }
                }
                ISAMS_Student_List sl1 = new ISAMS_Student_List();
                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<ISAMS_Student>));
                using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(result)))
                {
                    sl1.m_list = (js.ReadObject(stream) as List<ISAMS_Student>);
                }
                foreach (ISAMS_Student st in sl1.m_list)
                {
                    p.m_IsamsPupilId = st.ISAMS_SchoolId;
                }
            }

            //if(p.m_IsamsPupilId== null)p.m_IsamsPupilId = "690179871718";

            if (p.m_IsamsPupilId != null)
            {
                //OK we have isams id
                //need to call on-site server...
                string result = "";
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    try
                    {
                        result = client.DownloadString("http://10.1.84.230/admin/messagelist.sync?type=ISAMS_GETEXAMENTRIES&Id=" + p.m_IsamsPupilId + "&parameters=" + "0");
                    }
                    catch (Exception ex)
                    {
                        s = "Error calling remote server : "+ ex.ToString();servercontent.InnerHtml = s; return;
                    }
                }

                ISAMS_ExamsEntry_List el1 = new ISAMS_ExamsEntry_List();
                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<ISAMS_ExamEntry>));
                using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(result)))
                {
                  el1.m_list =(js.ReadObject(stream) as List<ISAMS_ExamEntry>);
                }
                s += "<center><h2>Exam Timetable for " + p.m_GivenName + "  " + p.m_Surname + "</h2></center>";
                s += "<BR><center><TABLE BORDER><TR>";
                s += "<Th>Date</Th>";
                s += "<Th>Start Time</Th>";
                s += "<Th>Paper Code</Th>";
                s += "<Th>Paper Name</Th>";
                s += "<Th>Room</Th>";
                s += "<Th>Desk</Th></TR>";

                foreach (ISAMS_ExamEntry ex in el1.m_list)
                {
                    s += "<TR><TD>" + ex.Date + "</TD>";
                    s += "<TD>" + ex.Time + "</TD>";
                    s += "<TD>" + ex.ComponentCode + "</TD>";
                    s += "<TD>" + ex.OptionTitle+": "+ex.ComponentTitle + "</TD>";
                    s += "<TD>" + ex.RoomName + "</TD>";
                    s += "<TD>" + ex.GivenSeat + "</TD></TR>";
                }
                s += "</TABLE></center>";
                
            }
            servercontent.InnerHtml = s;
        }
    }
}
