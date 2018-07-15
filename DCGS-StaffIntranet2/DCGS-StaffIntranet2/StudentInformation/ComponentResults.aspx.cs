using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.StudentInformation
{
    public partial class ComponentResults : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Count >= 2)
                {
                    string s = "test";
                    string id0 = Request.QueryString["StudentId"];
                    ViewState.Add("StudentId", id0);
                    string id1 = Request.QueryString["OptionId"];
                    ExamComponentResultList ecrl1 = new ExamComponentResultList();
                    ecrl1.Load_OptionStudent(new Guid(id1), new Guid(id0));
                    SimplePupil pupil1 = new SimplePupil();pupil1.Load(id0);
                    ExamOption exo1 = new ExamOption();exo1.Load(new Guid(id1));
                    ExamComponent ec1 = new ExamComponent();
                    ResultsList rl1 = new ResultsList();
                    rl1.LoadListSimple(" WHERE (ExamsOptionId = '" + id1 + "') AND (StudentID = '" + id0 + "')");
                    Result r1 = new Result();
                    r1 = (Result)rl1._results[0];
                    s= " <p><h3> Component Scores for ";
                    s += pupil1.m_GivenName + " " + pupil1.m_Surname + " for "+exo1.m_OptionTitle+"("+r1.Text+")</h3></p>";
                    s += "This table gives your component marks for this subject. Please note these are <b>not</b> UMS marks. ";
                    s += "<br/> The sum of the component marks gives your TQM (Total Qualification Mark), which leads to your final grade.";
                    s += "The table below tells you how the TQM is related to the final grade, i.e. the grade boundaries, if we know them! If not, please speak to a member of staff in school.";
                    
                    s += "<br/><br /><TABLE BORDER   class=\"ResultsTbl\" style = \"font-size:small\"   align=\"center\">";
                    s += "<TR><th>Syllabus Code</th><th>Option Code</th><th>Component Code</th><th>Component Name</th>";
                    s+="<th>Mark</th> <th>Max Mark</th></tr>";
                    foreach (ExamComponentResult r in ecrl1.m_list)
                    {
                        ec1.Load(r.ComponentId);
                        s += "<tr align=\"center\">";
                        s += "<td>" + exo1.m_Syllabus_Code + "</td>";
                        s += "<td>" + exo1.m_OptionCode + "</td>";
                        s += "<td>" + ec1.m_ComponentCode + "</td>";
                        s += "<TD>" + ec1.m_ComponentTitle + "</td>";
                        s += "<td>" + r.ComponentScaledMark.ToString() + "</td>";
                        s += "<td>"+ec1.m_MaximumMark.ToString()+"</td>";
                        s+=" </tr>";
                    }
                    s += "</table>";
                    ExamTQMBoundaryList bl1 = new ExamTQMBoundaryList();//will be ordered largest first
                    bl1.LoadList(new Guid(id1), exo1.m_year_Code, exo1.m_Season_code);

                    s += "<br/><br/>"; s += " <p><h3 align=\"center\" > TQM Grade Boundaries</h3></p>";
                    s += "<TABLE BORDER   class=\"TimetableTable\" style = \"font-size:small\" align=\"center\"  >";
                    s += "<tr><th>Grade</th><th>  TQM   </th></tr>";
                    foreach(ExamTQMBoundary eb in bl1.m_list)
                    {
                        s += "<tr><td>"+eb.Grade+"</td><td>"+eb.Mark+"</td></tr>";
                    }

                    s += "</table>";

                    content.InnerHtml = s;
                }
            }
        }
    }
}