using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class ExamResultsforStaff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //first find out who we are...
                Utility u = new Utility();
                string s = "";
                //string staff = u.GetStaffCodefromContext(Context);
                string staff = u.GetsStaffCodefromRequest(Request);
                DateTime date1 = new DateTime(); date1 = DateTime.Now;
                //date1 = date1.AddMonths(-12); //for testing we go back a year..
                date1 = date1.AddMonths(-3);// far enough back??
                string ds = "CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";
                Guid g1 = new Guid(u.Get_StaffID(staff));
                //now find out our teaching program before these exams.
                GroupList gl1 = new GroupList();
                gl1.LoadStaff(staff, date1, GroupList.GroupListOrder.GroupName);

                ServerContent.InnerHtml = "<h3>Results for sets you taught this year (possibly)</h3><br />";
                string[] titles = new string[20];
                string[] line = new string[20];
                int n_titles = 0; int n1 = 0;
                ResultsList rl1 = new ResultsList();
                foreach (Group g in gl1._groups)
                {
                    for (int i = 0; i < 20; i++) titles[i] = "";
                    if (g._GroupCode.StartsWith("11"))
                    {
                        s = "";
                        StudentGroupMembershipList sgml1 = new StudentGroupMembershipList();
                        sgml1.LoadList_Group(g._GroupID, date1);
                        foreach (StudentGroupMembership sg in sgml1.m_list)
                        {
                            for (int i = 0; i < 20; i++) line[i] = "";
                            SimplePupil pupil1 = new SimplePupil();
                            pupil1.Load(sg.m_Studentid);
                            pupil1.m_year = 11;
                            //get their results....
                            rl1.m_parameters = 3; rl1.m_db_field2 = "dbo.tbl_Core_Results.CourseID"; rl1.m_value2 = g._CourseID.ToString();
                            rl1.m_db_extraquery = " AND (dbo.tbl_Core_Results.ResultDate > " + ds + " ) ";
                            rl1.LoadList("dbo.tbl_Core_Results.StudentId", pupil1.m_StudentId.ToString());
                            //add name
                            //string predict = "";
                            //StudentAccademicProfile sadp1 = new StudentAccademicProfile(pupil1.m_StudentId);
                            //sadp1.Load_Profile(date1, ref pupil1, ref n1, false);
                            //foreach (StudentSubjectProfile sp1 in sadp1.profile)
                            //{
                              //  if (sp1.course._CourseID == g._CourseID)
                                   // predict = sp1.PredictedGrade.ToString();
                            //}
                            s += "<tr><td>" + pupil1.m_GivenName + "</td><td> " + pupil1.m_Surname + "</td>";
                            //<td>" + predict + "</td>";
                            foreach (Result r in rl1._results)
                            {
                                n1 = -1;
                                //need to find in titles
                                for (int i = 0; i < n_titles; i++)
                                {
                                    if (titles[i] == r.OptionCode)
                                    {
                                        n1 = i; break;
                                    }
                                }
                                if (n1 == -1)
                                {
                                    n1 = n_titles; n_titles++; titles[n1] = r.OptionCode;
                                }
                                line[n1] = "<td>" + r.Value + "</td>";
                            }
                            for (int i = 0; i < n_titles; i++) s += line[i];
                            s += "</tr>";
                        }

                        //now done all members of group so can add header rows!!!

                        string s1 = g._GroupCode + "<br/><br /><TABLE BORDER   class=\"ResultsTbl\" style = \"font-size:small ;  \">";
                        s1 += "<tr><th>GivenName</th><th>Surname</th>";
                        //<th>Predict</th>";
                        for (int i = 0; i < n_titles; i++) s1 += "<th>" + titles[i].Trim() + "</th>";
                        s1 += "</tr>";
                        s = s1 + s + "</TABLE><br /><br />";
                        ServerContent.InnerHtml += s;
                    }
                }
                //ServerContent.InnerHtml += "<h4>Prediction is from Alis Equations for A-level results and from Yellis test for GCSE</h4><br />";
            }
        }
    }
}
