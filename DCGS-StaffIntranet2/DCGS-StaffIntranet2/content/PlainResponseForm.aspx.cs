using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class PlainResponseForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string warning = Request.QueryString["TimeWarning"];
                if (warning == "true")
                {
                    string s =  Request.QueryString["GroupId"];
                    string s2 = Request.QueryString["GroupName"];
                    string s_link = "Click <a href=\"PlainResponseForm.aspx?Type=TargetOutput&GroupId=" + s + "&GroupName=" + s2 + "&TimeWarning=false  \">here </a> to continue.<br>";
                    Server_Content.InnerHtml = "<div><h2> Warning generating this table will take a while... please be patient!";
                    Server_Content.InnerHtml += "</br>"+ s_link+"</h2></div>";
                    return;
                }

                string type = Request.QueryString["Type"];
                if (type == "PhotoOnly")
                {
                    Cerval_Library.GroupListControl GroupListControl1 = new Cerval_Library.GroupListControl();
                    Server_Content.Controls.Add(GroupListControl1);
                    string s =  Request.QueryString["GroupId"];
                    string s2 = Request.QueryString["GroupName"];
                    //string s3 = Request.QueryString["GroupDate"];
                    GroupListControl1.PhotoOnly = true;
                    //GroupListControl1.ListDate = System.Convert.ToDateTime(s3);
                    GroupListControl1.Groups.Add(new Cerval_Library.Listitem(s2, new Guid(s)));
                }
                if (type == "GroupIncidents")
                {
                    string s = Request.QueryString["GroupId"];
                    string s2 = Request.QueryString["GroupName"];
                    GroupIncidentControl1.GroupCode = s2;
                    GroupIncidentControl1.Visible = true;
                }
                if (type == "TargetOutput")
                {
                    string s = Request.QueryString["GroupId"];
                    string s2 = Request.QueryString["GroupName"];
                    s = GenerateVA(s,s2);
                    Server_Content.InnerHtml = s;
                }


                if (type == "GroupStudentDevelopment")
                {
                    string s = Request.QueryString["GroupId"];
                    string s2 = Request.QueryString["GroupName"];
                    GroupStudentDevelopmentControl.GroupCode = s2;
                    GroupStudentDevelopmentControl.Visible = true;
                }
                if (type == "FullAddressList")
                {
                    GroupListControl1.Visible = true;
                    string s = Request.QueryString["GroupId"];
                    string s2 = Request.QueryString["GroupName"];
                    string s3 = Request.QueryString["ListDate"];
                    GroupListControl1.FullAddressList = true;
                    GroupListControl1.ListDate = System.Convert.ToDateTime(s3);
                    GroupListControl1.Groups.Add(new Cerval_Library.Listitem(s2, new Guid(s)));
                    GroupListControl1.save(); 
                }
                if (type == "MusicGroupChanges")
                {
                    GroupListControl1.Visible = false;
                    GroupIncidentControl1.Visible = false;
                    string Group = Request.QueryString["Group"];
                    string time = Request.QueryString["Time"];
                    string s = "";
                    Cerval_Library.ScheduledPeriodRawList fred = new Cerval_Library.ScheduledPeriodRawList();
                    fred.Load_for_Group(new Guid(Group));
                    Cerval_Library.DayList daylist1 = new Cerval_Library.DayList();
                    Cerval_Library.Period period1 = new Cerval_Library.Period();
                    Cerval_Library.Group g1 = new Cerval_Library.Group(); g1.Load(new Guid(Group));
                    s = "<h2><p align=\"center\"> Future Scheduling for " + g1._GroupCode + "</p></h2>";
                    
                    s += "<p  align=\"center\"><TABLE BORDER  class= \"TimetableTable\" > ";
                    s += "<TR><TD>From</TD><TD>To</TD><TD>Day</TD><TD>Period</TD><TD>Time Slot</TD><TD>Staff</TD><TD>Room</TD></TR>";
                    foreach (Cerval_Library.ScheduledPeriodRaw r in fred.m_list)
                    {
                        s += "<TR>";
                        s += "<TD>" + r.ValidityStart.ToShortDateString() +"</TD>";
                        s += "<TD>" + r.ValidityEnd.ToShortDateString() + "</TD>";
                        foreach (Cerval_Library.days d in daylist1.m_DayList)
                        {
                            if (d.m_daycode == r.DayNo) s += "<TD>" + d.m_dayname + "</TD>";
                        }
                        period1.Load(r.PeriodId);
                        s += "<TD>" + period1.m_periodcode + "</TD>";
                        s += "<TD>" + period1.m_PeriodStart.ToShortTimeString() + "-" + period1.m_PeriodEnd.ToShortTimeString() + "</TD>";
                        Cerval_Library.SimpleStaff staff1 = new Cerval_Library.SimpleStaff(r.StaffId);
                        s += "<TD>" + staff1.m_StaffCode+ "</TD>";
                        Cerval_Library.SimpleRoom room1 = new Cerval_Library.SimpleRoom(r.RoomId);
                        s += "<TD>" + room1.m_roomcode + "</TD></TR>";
                    }
                    s+="</TABLE>";


                    Server_Content.InnerHtml = s;
                }
                if (type == "FSM")
                {
                    string s = "";
                    Cerval_Library.SimpleStudentList sstl1 = new Cerval_Library.SimpleStudentList("");
                    sstl1.LoadList_FreeMealsOnly();
                    s = "<h2><p align=\"center\"> FSM list </p></h2>";

                    s += "<p  align=\"center\"><TABLE BORDER  class= \"TimetableTable\" > ";
                    s += "<TR><TD>Given Name</TD><TD>Surname</TD><TD>Form</TD></TR>";
                    foreach (Cerval_Library.SimplePupil p in sstl1._studentlist)
                    {
                        s += "<tr><td>" + p.m_GivenName + "</td><td>" + p.m_Surname + "</td><td>" + p.m_form + "</td></tr>";
                    }
                    s += "</table>";

                    sstl1.LoadHMFList();
                    s+= "<br/><br/><h2><p align=\"center\"> HMF list </p></h2>";

                    s += "<p  align=\"center\"><TABLE BORDER  class= \"TimetableTable\" > ";
                    s += "<TR><TD>Given Name</TD><TD>Surname</TD><TD>Form</TD></TR>";
                    foreach (Cerval_Library.SimplePupil p in sstl1._studentlist)
                    {
                        s += "<tr><td>" + p.m_GivenName + "</td><td>" + p.m_Surname + "</td><td>" + p.m_form + "</td></tr>";
                    }
                    s += "</table>";
                    Server_Content.InnerHtml = s;
                }
            }
        }
        class VAresult
        {
            public string adno; public string course; public string profile; public string predict;
            public string surname; public string givename;
            public double xgrade;public double x1;public double x2;
        }
        protected string GenerateVA(string GroupID, string GroupName)
        {
            string s = ""; string s1 = ""; List<VAresult> list1 = new List<VAresult>();
            PupilGroupList pgl = new PupilGroupList(); pgl.AddToList(new Guid(GroupID), DateTime.Now);
            //going to get the data
            double xgrade = 0;
            double x1 = 0;
            double x2 = 0;
            foreach (SimplePupil p in pgl.m_pupilllist)
            {
                StudentAccademicProfile StudentProfile = new StudentAccademicProfile(p.m_StudentId);
                int KeyStage = 0;
                DateTime date1 = new DateTime(); date1 = DateTime.Now;
            //this is when we load tt etc... so if in summer gap... alter
                if (date1.Month == 8) date1 = date1.AddMonths(2);
                SimplePupil p1 = new SimplePupil(); p1 = p;
                StudentProfile.Load_Profile(date1, ref p1, ref KeyStage, false);
                string Profile_approx = ""; string Target_approx = "";
                foreach (StudentSubjectProfile sp in StudentProfile.profile)
                {
                    try
                    {
                        if (sp.latestProfileGrade != null)
                        {
                            x2 = sp.Convert_to_scale(ref xgrade, ref x1,ref Target_approx,ref Profile_approx);
                            VAresult v1 = new VAresult();
                            v1.adno = p1.m_adno.ToString();
                            v1.course = sp.course.CourseCode;
                            //v1.profile = x1.ToString();
                            v1.profile = Profile_approx;
                            //v1.predict = x2.ToString();
                            v1.predict = Target_approx;
                            v1.surname = p.m_Surname; v1.givename = p.m_GivenName;
                            v1.x1 = x1;v1.x2 = x2;
                            v1.xgrade = xgrade;
                            list1.Add(v1);
                        }
                    }
                    catch {  }                 
                }
            }

            //find all the courses in the list...
            List<string> courselist = new List<string>();
            bool found = false;
            foreach (VAresult v in list1)
            {
                found = false;
                foreach (string s4 in courselist)
                {
                    if (s4 == v.course) { found = true; break; }
                }
                //not found
                if(!found)courselist.Add(v.course);
            }
            courselist.Sort();
            //find all the students
            List<string> adnolist = new List<string>();
            foreach (VAresult v in list1)
            {
                found = false;
                foreach (string s4 in adnolist)
                {
                    if (s4 == v.adno) { found = true; break; }
                }
                //not found
                if(!found)adnolist.Add(v.adno);
            }
            string [,] list2 = new string[adnolist.Count+3,2*courselist.Count+6];
            double [,] list3 = new double[adnolist.Count + 3, 2 * courselist.Count + 6];
            double [,] listx = new double[adnolist.Count + 3, 2 * courselist.Count + 6];
            int i=3;
            foreach (string s5 in courselist)
            {
                list2[0, i] = s5; i++;
                list2[0, i] = s5; i++;
            }
            int j = 1;
            foreach (VAresult v in list1)
            {
                if (list2[j, 0] != v.adno)
                {
                    j++; list2[j, 0] = v.adno; list2[j, 1] = v.givename; list2[j, 2] = v.surname;
                }
                //now find the course
                i = 3;
                foreach (string s5 in courselist)
                {
                    if (s5 == v.course)
                    {
                        list2[j, i] = v.predict;
                        list2[j, i + 1] = v.profile;
                        list3[j, i] = v.xgrade;
                        list3[j, i + 1] = v.xgrade;
                        listx[j, i] = v.x2;
                        listx[j, i + 1] = v.x1;
                        break;
                    }
                    i++; i++;
                }
            }
            s = "<h2><p align=\"center\"> Latest Profile Score v Predicted Grade </p></h2>";
            s += "<div><p  align=\"center\"><table class=\"EventsTable\"> ";
            s += "<TR><TD >Cells have:   </TD><td>course</br>Profile Grade</br>Predicted Grade</td>";
            s += "<td>Predictions are from Alis (KS5) / ";
            s += " Yellis (KS4)</br>Points are UCAS at KS5 (A=120) and GCSE at KS4 (A=52 or new 1-9 scale)</td>";
            s += "</tr></table></p>";
            s += "Colour indicates how far they exceed / failed to reach target";
            s += "<table class=\"EventsTable\"> ";
            s += "<TR><TD class= \"tableHighlight1\">Exceed by 1 grade</TD>";
            s += "<TD class= \"tableHighlight2\">Exceed by 0.5 grade</TD>";
            s += "<TD class= \"tableHighlight4\">Below by 0.5 grade</TD>";
            s += "<TD class= \"tableHighlight5\">Below by 1 grade</TD>";
            s += "</tr></table></div>";
            s += "<p  align=\"center\"><table class=\"EventsTable\"> ";
            s += "<TR><td>Adno</td><TD>Given Name</TD><TD>Surname</TD>";
            s += "<TD>Subjects</TD><TD>...............</TD></TR>";

            for (int i1 = 1; i1 < adnolist.Count + 2; i1++)
            {
                s += "<tr>";
                for (int j1 = 0; j1 < 3; j1++)
                {
                    s += "<td>" + list2[i1, j1] + "</td>";
                }

                for (int j1 = 4; j1 < 2 * courselist.Count + 4; j1 = j1 + 2)
                {
                    s1 = "<td>" + list2[0, j1] + "</br>";
                    try
                    {
                        x1 = listx[i1, j1];
                        x2 = listx[i1, j1 - 1];
                        xgrade = list3[i1, j1];
                        double xd = x2 - x1;
                        if (x1 > 0)
                        {
                            if (x2 > 0)
                            {
                                if (xd > xgrade / 2)
                                {
                                    s1 = "<td class= \"tableHighlight4\" >" + list2[0, j1] + "</br>";
                                    if (xd > xgrade) { s1 = "<td class= \"tableHighlight5\" >" + list2[0, j1] + "</br>"; }
                                }
                                else if (xd < -xgrade / 2)
                                {
                                    s1 = "<td class= \"tableHighlight2\" >" + list2[0, j1] + "</br>";
                                    if (xd < -xgrade)
                                    { s1 = "<td class= \"tableHighlight1\" >" + list2[0, j1] + "</br>"; }
                                }
                                //s1 += x1.ToString() + "</br>";
                                //s1 += x2.ToString();
                                s1+=list2[i1,j1] + "</br>";
                                s1 += list2[i1, j1 + 1];

                            }
                            else
                            {
                                s1 = "<td>" + list2[0, j1] + "</br>";
                                s1 += x1.ToString() + "</br>";
                            }
                        }
                        else s1 = "<td></br>";
                    }
                    catch { };

                    s += s1 + "</TD>";
                }
                s += "</tr>";
            }
            s += "</table></div></p>";


            //going to add to pure data table....


            s += "</br><h2><p align=\"center\"> Data as flat table for copying to sheets etc. </p></h2></br>";
            s += "</p><div><p  align=\"center\"><table class=\"EventsTable\"> ";
            s += "<TR><td>Adno</td><TD>Given Name</TD><TD>Surname</TD>";
            s += "<TD>Subjects</TD><TD>Always prediction then Profile...</TD></TR>";
            for (int i1 = 0; i1 < adnolist.Count + 2; i1++)
            {
                s+="<tr>";
                for (int j1 = 0; j1 < 2 * courselist.Count + 4; j1 = j1 + 2)
                {
                    s += "<td>" + list2[i1, j1] + "</td>";
                    s += "<td>" + list2[i1, j1+1] + "</td>";
                }
                s += "</tr>";
            }
            s += "</table></div></p>";
            return s;
        }
    }
}
