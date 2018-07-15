using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace StudentInformation
{
    public partial class ExamWarnings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

            }
            if (IsPostBack)
            {
                //we have read it all and can update db                
                Guid PersonID = new Guid();
                Utility u1 = new Utility();
                PersonID = u1.GetPersonIDX(Context.User.Identity.Name.ToString());
                //PersonID = u1.GetPersonIDX(@"CHALLONERS\Richard.Baldwin");//development
                //PersonID = u1.GetPersonIDX(@"CHALLONERS\mark.lilley");//development
#if DEBUG
                PersonID = u1.GetPersonIDX(@"CHALLONERS\Nicholas.Brabbs");//development
                //PersonID = u1.GetPersonIDX(@"CHALLONERS\Richard.Baldwin");//development
#endif
                DateTime d1 = new DateTime();
                d1= u1.ThisExamSeasonStart();
                string y=d1.Year.ToString();
                WarningNoticeRead w = new WarningNoticeRead();
                w.m_StudentId = u1.GetStudentId(PersonID);
                w.m_Year = d1.Year.ToString();
                w.m_Season = u1.ThisSeason(d1);
                w.m_DateRead = DateTime.Now;
                w.Save();
                Server.Transfer("ExamTimetables.aspx");
            }

        }
    }
}
