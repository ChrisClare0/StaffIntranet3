using System;
using System.Collections;
using Cerval_Library;

namespace StudentInformation
{
    public partial class ExamEntries : System.Web.UI.Page
    {
        public Guid PersonID = new Guid();
        protected void Page_Load(object sender, EventArgs e)
        {         
            if (!IsPostBack)
            {
                Utility u1 = new Utility();
                DateTime t1 = new DateTime();
                DateTime t0 = new DateTime();
                DateTime t2;
                t0 = u1.ThisExamSeasonStart(); t1 = u1.ThisExamSeasonEnd(t0);
                string season = u1.ThisSeason(t0);
                season = "3";
                ArrayList m_list = new ArrayList();
                PersonID = u1.GetPersonIdfromRequest(Request);

#if DEBUG
                u1.Is_student = true;
                u1.Is_student = true; PersonID = u1.GetPersonIDX(@"CHALLONERS\william.arnold");//development

#endif
                string s = ""; string s1 = "";
                if ((PersonID != Guid.Empty) && (u1.Is_student))
                {
                    s1+="<br><center><h2>Exam Entries</h2></center>";
                    s1+="<p  align=\"center\"><TABLE BORDER><Tr>";
                    s = "Option"; s1+="<Th>" + s + "</Th>";
                    s = "Code"; s1+="<Th>" + s + "</Th>";
                    s = "Qualification"; s1+="<Th>" + s + "</Th>";
                    s = "Level"; s1+="<Th>" + s + "</Th>";
                    s = "Status"; s1+="<Th>" + s + "</Th>";
                    s1+="</Tr>";
                    ExamEntries_List exl1 = new ExamEntries_List();
                    exl1.Load(u1.GetStudentId(PersonID));
                    foreach (Exam_Entry ex1 in exl1.m_list)
                    {
                        ExamOption exo1 = new ExamOption(); exo1.Load(ex1.m_OptionID);
                        //if ((ex1.m_season == season) && (ex1.m_year == t0.Year.ToString())&&(!ex1.m_withdrawn))
                            if ( (ex1.m_year == t0.Year.ToString()) && (!ex1.m_withdrawn))
                            {
                            //ExamOption exo1 = new ExamOption(); exo1.Load(ex1.m_OptionID);
                            //if (exo1.m_Item == "C")
                            {
                                s1+="<TR>";
                                s = exo1.m_OptionTitle; s1+="<TD>" + s + "</TD>";
                                s = exo1.m_OptionCode; s1+="<TD>" + s + "</TD>";
                                s = exo1.m_OptionQualification; s1+="<TD>" + s + "</TD>";
                                if (exo1.m_OptionLevel != "B") { s = exo1.m_OptionLevel; s1+="<TD>" + s + "</TD>"; } else s1+="<TD></TD>";
                                if (ex1.m_EntryFileID == Guid.Empty) 
                                
                                {
                                    //if DCGS board then Mock/Internal exam
                                    if (exo1.m_ExamBoardID.ToString() == "436ff234-0457-430a-b1e2-b08758ff30ef")
                                    {
                                        //so internal.. if Jan is mocks..
                                        if(exo1.m_Season_code=="1")
                                        s1 += "<td>Mock</td>";
                                        else
                                            s1 += "<td>Internal</td>";
                                    }
                                    else
                                        s1 += "<TD>Provisional</TD>"; 
                                } 
                                else s1+="<TD>Entered</TD>";
                                s1+="</TR>";
                            }

                            ExamCompononent_List excl1 = new ExamCompononent_List(); 
                            excl1.Load(ex1.m_OptionID);
                            foreach (ExamComponent ec in excl1.m_list)
                            {
                                if (ec.m_Timetabled == "T")
                                {
                                    ExamComponent ec2 = new ExamComponent(); ec2 = ec;
                                    m_list.Add(ec2); ec2.m_OptionTitle = exo1.m_Syllabus_Title;
                                }
                            }
                        }
                    }
                    s1+="</TABLE>";
                    s1+="<center>FC = Full Course, SC = Short(Half) Course<br>";

                    foreach (ExamComponent ec in m_list)
                    {
                        t2 = System.Convert.ToDateTime(ec.m_TimetableDate);
                        if (t2 < t1) t1 = System.Convert.ToDateTime(ec.m_TimetableDate);
                        if (t2 > t0) t0 = System.Convert.ToDateTime(ec.m_TimetableDate);
                    }
                    int month = t1.Month; ExamComponent ecx;
                    int day = t1.Day;
                    s1+="<br><br><br><center><h2>Outline Exam Timetable</h2>";
                    s1+="Full Exam Timetable can be found <a href=\"ExamTimetables.aspx?\">here</a></center>";
                    s1+="<center>Note that for GCSE Languages the tier (Foundation or Higher) is not significant. ";
                    s1+="<BR><p  align=\"center\"><TABLE BORDER><TR>";
                    s = "Date"; s1+="<Th>" + s + "</Th>";
                    s = "AM Session"; s1+="<Th>" + s + "</Th>";
                    s = "PM Session"; s1+="<Th>" + s + "</Th>";
                    s1+="</TR>";
                    //from t1 to t0...
                    t2 = t1; t0=t0.AddDays(1);
                    while (t2 < t0)
                    {
                        if ((t2.DayOfWeek.ToString() != "Saturday") && (t2.DayOfWeek.ToString() != "Sunday"))
                        {
                            s1+="<TR>";
                            s = t2.DayOfWeek.ToString(); s += "<br>" + t2.ToShortDateString(); s1+="<TD>" + s + "</TD>"; s = ""; ecx = null;
                            foreach (ExamComponent ec in m_list)
                            {
                                if ((t2 == System.Convert.ToDateTime(ec.m_TimetableDate)) && (ec.m_TimetableSession == "A"))
                                {
                                    s += ec.m_OptionTitle + ec.m_ComponentTitle.ToLower() + "<br>"; ecx = ec;
                                }
                            }
                            if (ecx != null) m_list.Remove(ecx);
                            s1+="<TD>" + s + "</TD>"; s = ""; ecx = null;
                            foreach (ExamComponent ec in m_list)
                            {
                                if ((t2 == System.Convert.ToDateTime(ec.m_TimetableDate)) && (ec.m_TimetableSession == "P"))
                                {
                                    s += ec.m_OptionTitle + ec.m_ComponentTitle.ToLower() + "<br>"; ecx = ec;
                                }
                            }
                            m_list.Remove(ecx);
                            s1+="<TD>" + s + "</TD>";
                            s1+="</TR>";
                        }
                        t2=t2.AddDays(1);
                    }
                    s1 += "</TABLE>";
                    servercontent.InnerHtml = s1;
                }
            }
        }
    }
}
