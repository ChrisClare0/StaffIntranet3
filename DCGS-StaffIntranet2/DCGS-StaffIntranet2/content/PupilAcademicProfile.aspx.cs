using System;
using System.Collections.Generic;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class PupilAcademicProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string s = Request.QueryString["Year"];
            //YearDisplay(); return;
#if DEBUG
            if (s !=null){YearDisplay();return;}
#else

#endif
            s = Request.QueryString["Id"];
            StudentAccademicProfile StudentProfile = new StudentAccademicProfile(new Guid(s));
            SimplePupil p = new SimplePupil(); int KeyStage = 0;
            DateTime date1 = new DateTime(); date1 = DateTime.Now;
            //this is when we load tt etc... so if in summer gap... alter
            if(date1.Month==8)date1=date1.AddMonths(2);
            StudentProfile.Load_Profile(date1,ref p, ref KeyStage,true);
            StudentIncidentList stinl = new StudentIncidentList(); 
            stinl.LoadListDate(StudentProfile.StudentId,DateTime.Now.AddMonths(-1),DateTime.Now);
               
            Double d=0; int n = 0;
            //so now to display...
            // vary this with Key Stage...???
            string class1 = "";
            s = "<h3>Academic Profile for " + p.m_GivenName + " " + p.m_Surname + "</h3><br />";
            s+= "<table class=\"EventsTable\" > ";
            s += "<tr><th>Subject</th>";
            s += "<th>Current Grade</th>";
            //s += "<th>Prof Grade</th>";
            //s += "<th>Max grade</th>";
            s += "<th>Target Grade </th>";
            s += "<th>Commitment</th>";
            s += "<th>Last Internal Exam</th>";
            s += "<th>Year Average</th>";
            s += "<th>Year Position (percentile)</th>";
            s += "</tr>";
            double margin = 1;
            double xgrade = 20;
            double pgrade = 0;
            string Profile_approx = "";string Target_approx = "";
            foreach (StudentSubjectProfile sp in StudentProfile.profile)
            {
                s += "<tr><td>" + sp.course.CourseName + "</td>";
                pgrade = sp.PredictedGrade;
                pgrade = sp.Convert_to_scale(ref xgrade, ref d, ref Target_approx, ref Profile_approx);
                if (sp.latestProfileGrade != null)
                
                {
                    //pgrade = sp.Convert_to_scale(ref xgrade, ref d,ref Target_approx, ref Profile_approx);
                    margin = xgrade / 2;//half a grade
                    class1 = " class=\"tableHighlight3\"";
                    if ((pgrade - d) > margin) class1 = " class=\"tableHighlight4\"";
                    if ((pgrade - d) > 2* margin) class1 = " class=\"tableHighlight5\"";
                    if ((d - pgrade) > margin) class1 = " class=\"tableHighlight2\"";
                    if ((d - pgrade) > 2* margin) class1 = " class=\"tableHighlight1\"";
                    if (sp.PredictedGrade == 0) class1 = " class=\"tableHighlight3\"";
                    //s += "<td"+class1+" >" + d.ToString() + "</td>";
                    s += "<td" + class1 + " >" + Profile_approx + "</td>";
                    //s+= "<td >" + sp.latestProfileGrade.Value.ToString() + "</td>";
                    //s += "<td >" + sp.latestProfileGradeScale.MaxValue.ToString() + "</td>";
                }
                else
                    s += "<td></td>";
                //s+="<td></td><td></td>";

                //if (pgrade != 0) s += "<td>" + pgrade.ToString() + "</td>";else s += "<td></td>";
                if (pgrade != 0) s += "<td>" + Target_approx + "</td>"; else s += "<td></td>";
                if (sp.latestCommintmentGrade != null)
                {
                    class1 = " class=\"tableHighlight3\"";
                    d = System.Convert.ToDouble(sp.latestCommintmentGrade.Value);
                    if (d > 3) class1 = " class=\"tableHighlight2\"";
                    if (d > 4) class1 = " class=\"tableHighlight1\"";
                    if (d < 2) class1 = " class=\"tableHighlight4\"";
                    if (d < 1) class1 = " class=\"tableHighlight5\"";
                    s += "<td" + class1 + " >" + sp.latestCommintmentGrade.Value.ToString() + "</td>";
                }
                else
                    s += "<td></td>";

                if (sp.latestInternalExamResult != null)
                {
                    s += "<td>"+sp.latestInternalExamResult.Value+"</td>";
                    s += "<td>"+sp.AvgInternalResult.ToString()+"</td>";
                    n = (int) Round1(100 * ((double)sp.PositionInternalResult / (double)sp.NumberInternalResult));
                    class1 = " class=\"tableHighlight3\"";
                    if (n < 30) class1 = " class=\"tableHighlight2\"";
                    if (n < 15) class1 = " class=\"tableHighlight1\"";
                    if (n > 70 ) class1 = " class=\"tableHighlight4\"";
                    if (n > 85) class1 = " class=\"tableHighlight5\"";
                    s += "<td" + class1 + " >" + n.ToString() + "</td>";
                }
                else
                {
                    s += "<td></td>";
                    s += "<td></td>";
                    s += "<td></td>";
                }
                s += "</tr>";
            }
            s += "</table>";
            
            content0.InnerHtml = s;s="";
            if (stinl.m_list.Count > 0)
            {
                Utility u = new Utility();
                s = "Incdents in past month :<br />";
                s += "<table class=\"EventsTable\" > ";
                s += "<tr><th>Date</th>";
                s += "<th>Text</th>";
                s += "<th>Incident Pairs</th>";
                s += "<th>Staff</th>";
                s += "</tr>";
                foreach (StudentIncident si in stinl.m_list)
                {
                    s += "<tr>";
                    s += "<td>" + si.Date.ToShortDateString() + "</td>";
                    s += "<td>" + si.Text + "</td>";
                    s += "<td>" + si.IncidentPairs + "</td>";
                    s += "<td>" + u.Get_StaffCodefromStaffID(si.StaffID) + "</td>";
                    s += "</tr>";
                }
                s += "</table>";            
            }
            else
            {
                s += "No recorded Incidents in past month";
            }
            content1.InnerHtml = s;
        }


        public double Round1(double x)
        {
            int i = (int)(x * 10);
            return (double)i / 10;
        }

        public double Round0(double x)
        {
            int i = (int)(x);
            return (double)i;
        }



        protected void YearDisplay()
        {
            List<Course> StudentCourse = new List<Course>();
            string s = Request.QueryString["Year"];
            s = "10";
            SimpleStudentList stdlist1 = new SimpleStudentList();
            stdlist1.LoadList_atDate(DateTime.Now.AddMonths(2));
            stdlist1.Restrict_to_year(System.Convert.ToInt16(s));
            string class1 = "";
            StudentAccademicProfile StudentProfile = new StudentAccademicProfile();

            string[,] outputA = new string[250, 30];//students,subject
            outputA[0, 0] = "<th> Surname</th>"; outputA[0, 1] = "<th> GivenName</th> "; outputA[0, 2] = "<th> Adno</th> ";
            int no_cses = 0; int no_students = 0;
            int KeyStage = 0; int n = 0;
            switch (s)
            {
                case "7":
                case "8":
                case "9":
                    KeyStage = 3;
                    break;
                case "10":
                case "11":
                    KeyStage = 4;
                    break;
                case "12":
                case "13":
                    KeyStage = 5;
                    break;
            }
            CourseList cl1 = new CourseList(KeyStage);
            CourseList cl4 = new CourseList(50);//needed to get predicted grades for KS3.... ugh
            ResultsList rl1 = new ResultsList(); rl1.m_parameters = 1;

            foreach (SimplePupil p in stdlist1._studentlist)
            {
                rl1._results.Clear();
                StudentCourse.Clear();
                StudentProfile.profile.Clear();
                rl1.LoadList("dbo.tbl_Core_Students.StudentId", p.m_StudentId.ToString());
                ReportList repl = new ReportList(p.m_StudentId.ToString(), KeyStage - 2);
                //going to try to add the report grades in to the results...
                foreach (ReportValue v in repl.m_list)
                {
                    Result res1 = new Result();
                    rl1._results.Add(res1);
                    res1.External = false;
                    res1.Date = v.m_date;
                    res1.Shortname = "Report";
                    res1.Code = v.m_course;
                    if (v.m_IsCommitment) res1.Resulttype = 998; else res1.Resulttype = 999;
                    res1.Value = Round0(v.m_value).ToString();
                }
                //date sort with newest at front...
                Result r0 = new Result();
                Result r2 = new Result();
                for (int j = 0; j < rl1._results.Count; j++)
                {
                    for (int k = 0; k < (rl1._results.Count - 1 - j); k++)
                    {
                        r0 = (Result)rl1._results[k]; r2 = (Result)rl1._results[k + 1];
                        if (r0.Date < r2.Date)
                        {
                            rl1._results[k] = r2; rl1._results[k + 1] = r0;
                        }
                    }
                }
                double d = 0; int x = 0;

                ValueAddedMethodList vaml1 = new ValueAddedMethodList();
                ValueAddedMethod vam = new ValueAddedMethod();
                foreach (ValueAddedMethod VA_method1 in vaml1._ValueAddedMethodList)
                {
                    //going to use Yellis for KS4, CATs for KS3 and Alis for KS5..
                    if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("YE") && (KeyStage == 4)) vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID);
                    if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("AL") && (KeyStage == 5)) vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID);
                    if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("CATS") && (KeyStage == 3)) vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID);
                }
                ValueAddedConversionList vacl1 = new ValueAddedConversionList();
                ValueAddedEquation va1 = new ValueAddedEquation();

                //going to find his current courses....
                PupilPeriodList ppl1 = new PupilPeriodList();
                ppl1.LoadList("StudentId", p.m_StudentId.ToString(), true);
                foreach (ScheduledPeriod sc in ppl1.m_pupilTTlist)
                {
                    s = sc.m_groupcode;
                    if (KeyStage == 3)
                    {
                        if (s.Contains("-")) s = sc.m_groupcode.Substring(3, 2);
                        else
                            s = sc.m_groupcode.Substring(1, 2);//maths stes
                    }
                    else s = sc.m_groupcode.Substring(2, 2);
                    //going to assume that code is correct... ie 11HI4 is History KS4...
                    
                    foreach (Course c in cl1._courses)
                    {
                        if (c.CourseCode == s)
                        {
                            if (!StudentCourse.Contains(c))
                            {
                                StudentCourse.Add(c);
                                StudentSubjectProfile sbp1 = new StudentSubjectProfile();
                                sbp1.course = c;
                                sbp1.KeyStage = KeyStage;
                                StudentProfile.profile.Add(sbp1);
                            }
                        }
                    }
                }
                //so now we have a list of his courses.....
                //for each course we find latest grade and predicion?
                bool foundpg = false; bool foundcg = false; bool foundint = false; bool foundVAM = false;

                foreach (StudentSubjectProfile sp in StudentProfile.profile)
                {
                    foundpg = false; foundcg = false; foundint = false; foundVAM = false;
                    foreach (Result r in rl1._results)//they are in date order...
                    {
                        if (r.Resulttype == vam.m_ValueAddedBaseResultType)
                        {
                            foundVAM = true; sp.VA_Base_Score = System.Convert.ToDouble(r.Value);
                        }
                        if (r.Code == sp.course.CourseCode)
                        {

                            if ((r.Resulttype == 999) && !foundpg)
                            {
                                foundpg = true; sp.latestProfileGrade = r;
                            }
                            if ((r.Resulttype == 998) && !foundcg)
                            {
                                foundcg = true; sp.latestCommintmentGrade = r;
                            }
                            if (!foundint && (r.Resulttype == 5))
                            {
                                foundint = true; sp.latestInternalExamResult = r;
                                ResultsList rl2 = new ResultsList();
                                rl2.m_parameters = 3;
                                rl2.m_db_field2 = "CourseId"; rl2.m_value2 = r.CourseID.ToString();
                                string date1 = "CONVERT(DATETIME, '" + r.Date.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                                string date2 = "CONVERT(DATETIME, '" + r.Date.AddDays(5).ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                                rl2.m_db_extraquery = " AND (ResultDate > " + date1 + ") AND (ResultDate < " + date2 + ")";
                                //d = rl2.AverageResult("ResultType", "5", ref sp.NumberInternalResult);
                                x = (int)(d * 10);
                                //sp.AvgInternalResult = (double)x / 10;
                                //sp.PositionInternalResult = rl2.YearPosition("ResultType", "5", r.Value);
                            }
                            if (foundpg && foundcg && foundint && foundVAM) break;
                        }
                    }
                    if (sp.VA_Base_Score > 0)
                    {
                        string cse1id = sp.course._CourseID.ToString();
                        if (KeyStage == 3)
                        {
                            foreach (Course c in cl4._courses)
                            {
                                if (sp.course.CourseCode == c.CourseCode) cse1id = c._CourseID.ToString();
                            }
                        }
                        va1.Load1("WHERE (ValueAddedMethodID='" + vam.m_ValueAddedMethodID.ToString() + "') AND ( CourseID='" + cse1id + "' ) ");
                        sp.PredictedGrade = Round1(va1.m_coef0 + va1.m_coef1 * sp.VA_Base_Score + va1.m_coef2 * sp.VA_Base_Score * sp.VA_Base_Score);
                    }
                }
                //so now to add to list.....
                outputA[no_students + 1,0] = "<td>"+p.m_Surname+"</td>";
                outputA[no_students + 1, 1] = "<td>" + p.m_GivenName + "</td>";
                outputA[no_students + 1, 2] = "<td>" + p.m_adno.ToString() + "</td>";
                int cse_no = -1; 
                foreach (StudentSubjectProfile sp in StudentProfile.profile)
                {
                    cse_no = -1; 
                    for (int i = 0; i < no_cses+1;i++ )
                    {
                        if (outputA[0, 3 + i] == "<th>"+sp.course.CourseCode+"</th>")
                        {
                            cse_no = i; ;
                        }
                    }
                    if (cse_no < 0) 
                    {
                        outputA[0, 3 + no_cses] = "<th>" + sp.course.CourseCode + "</th>"; 
                        cse_no = no_cses; 
                        no_cses++; 
                    }


                    if (sp.latestProfileGrade != null)
                    {
                        d = System.Convert.ToDouble(sp.latestProfileGrade.Value);
                        if (KeyStage < 5) d = 34 + 6 * d;//convert to gcse points
                        if (KeyStage == 5) d = 60 + 20 * d;
                        class1 = " class=\"tableHighlight3\"";
                        if ((sp.PredictedGrade - d) > 1) class1 = " class=\"tableHighlight4\"";
                        if ((sp.PredictedGrade - d) > 2) class1 = " class=\"tableHighlight5\"";
                        if ((d - sp.PredictedGrade) > 1) class1 = " class=\"tableHighlight2\"";
                        if ((d - sp.PredictedGrade) > 2) class1 = " class=\"tableHighlight1\"";
                        if (sp.PredictedGrade == 0) class1 = " class=\"tableHighlight3\"";
                        s = "<td" + class1 + " >" + d.ToString() + "</td>";
                    }
                    else
                        s = "<td></td>";
                    outputA[no_students+1, cse_no+3] = s;

                    //if (sp.PredictedGrade != 0) s += "<td>" + sp.PredictedGrade.ToString() + "</td>"; else s += "<td></td>";
                    /*
                    if (sp.latestCommintmentGrade != null)
                    {
                        class1 = " class=\"tableHighlight3\"";
                        d = System.Convert.ToDouble(sp.latestCommintmentGrade.Value);
                        if (d > 3) class1 = " class=\"tableHighlight2\"";
                        if (d > 4) class1 = " class=\"tableHighlight1\"";
                        if (d < 2) class1 = " class=\"tableHighlight4\"";
                        if (d < 1) class1 = " class=\"tableHighlight5\"";
                        s += "<td" + class1 + " >" + sp.latestCommintmentGrade.Value.ToString() + "</td>";
                    }
                    else
                        s += "<td></td>";

                    if (sp.latestInternalExamResult != null)
                    {
                        s += "<td>" + sp.latestInternalExamResult.Value + "</td>";
                        s += "<td>" + sp.AvgInternalResult.ToString() + "</td>";
                        n = (int)Round1(100 * ((double)sp.PositionInternalResult / (double)sp.NumberInternalResult));
                        class1 = " class=\"tableHighlight3\"";
                        if (n < 30) class1 = " class=\"tableHighlight2\"";
                        if (n < 15) class1 = " class=\"tableHighlight1\"";
                        if (n > 70) class1 = " class=\"tableHighlight4\"";
                        if (n > 85) class1 = " class=\"tableHighlight5\"";
                        s += "<td" + class1 + " >" + n.ToString() + "</td>";
                    }
                    else
                    {
                        s += "<td></td>";
                        s += "<td></td>";
                        s += "<td></td>";
                    }
                    s += "</tr>";
                    */
                }


                no_students++;
               //if (no_students > 5) break;
            }




            //so now to display...
            string s1 = "";
            s = "<h3>Academic Profile for Year</h3><br />";
            s += "<table class=\"EventsTable\" > ";
            for (int i = 0; i < no_students+1; i++)
            {
                s += "<tr>";
                for (int j = 0; j < no_cses+3; j++)
                {
                    if (outputA[i, j] == null)
                    {
                        s += "<td></td>";
                    }
                    else
                    {
                        s += outputA[i, j];
                    }
                }
                s += "</tr>";
            }

            s += "</table>";

            content0.InnerHtml = s;
        }

    }

}
