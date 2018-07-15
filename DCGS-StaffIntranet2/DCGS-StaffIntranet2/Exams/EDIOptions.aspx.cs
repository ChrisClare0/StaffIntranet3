using Cerval_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class EDIOptions : System.Web.UI.Page
    { 
         #region variables

    public int Year
    {   //this is the year in 2016 format
        get { try { return (int)ViewState["Year"]; } catch { return 0; } }
        set { ViewState["Year"] = value; }
    }
    public int YearCode
    {   //this is the year in 16 format
        get { try { return (int)ViewState["YearCode"]; } catch { return 0; } }
        set { ViewState["YearCode"] = value; }
    }
    public int SeasonCode
    {
        get { try { return (int)ViewState["SeasonCode"]; } catch { return 0; } }
        set { ViewState["SeasonCode"] = value; }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetMasterValues();
                Label1.Text = "";
                string type=Request.QueryString.Get(0);
                switch (type)
                {
                    case "CheckULN":
                        Label1.Text = "This routine will check that all Entries have valid UCI/UPN/ULN";
                        TextBox1.Visible = false;
                        Button1.Text = "Run";
                        Label1.Text = " This routine will run through all students on Role, and where there is a missing UCI, but a valid Exam Number, it will assign a DCGS UCI";
                        break;
                    default:
                        break;
                }

            }
        }

        private void GetMasterValues()
        {
            SeasonCode = System.Convert.ToInt32((string)(Session["Season"]));
            Year = System.Convert.ToInt32((string)(Session["Year"]));
            YearCode = Year % 100;
            Label Label_Banner = (Label)Master.FindControl("Label_Banner");
            if (Label_Banner != null)
            {
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Transferring BaseData to Server.";
            }
        }

        protected string GetSQL_checkULN(string SeasonCode, string Year)
        {
            string s = "SELECT dbo.tbl_Core_Students.StudentAdmissionNumber, dbo.tbl_Core_People.PersonGivenName, dbo.tbl_Core_People.PersonSurname, ";
            s += " dbo.tbl_Core_Students.StudentUPN, dbo.tbl_Core_Students.StudentULN, ";
            s += " dbo.tbl_Core_Students.StudentUCI, dbo.tbl_Exams_Entries.ExamYear, dbo.tbl_Exams_Entries.ExamSeason ";
            s += " FROM dbo.tbl_Core_Students INNER JOIN ";
            s += "dbo.tbl_Core_People ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId INNER JOIN ";
            s += " dbo.tbl_Exams_Entries ON dbo.tbl_Core_Students.StudentId = dbo.tbl_Exams_Entries.StudentID";
            s += " GROUP BY dbo.tbl_Core_Students.StudentAdmissionNumber, dbo.tbl_Core_People.PersonGivenName, dbo.tbl_Core_People.PersonSurname, ";
            s += " dbo.tbl_Core_Students.StudentUPN, dbo.tbl_Core_Students.StudentULN, dbo.tbl_Core_Students.StudentUCI, dbo.tbl_Exams_Entries.ExamYear, ";
            s += " dbo.tbl_Exams_Entries.ExamSeason ";
            s += " HAVING        (dbo.tbl_Core_Students.StudentULN IS NULL OR dbo.tbl_Core_Students.StudentUCI IS NULL OR dbo.tbl_Core_Students.StudentUPN IS NULL) ";
            s += " AND(dbo.tbl_Exams_Entries.ExamSeason = '"+SeasonCode+"') AND(dbo.tbl_Exams_Entries.ExamYear = '"+Year+"')";

            return s;

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // going to run through db and add any missing uci where we have an exam no...

            SimpleStudentList sl1 = new SimpleStudentList(SimpleStudentList.LIST_TYPE.NOFORM_ONROLE);
            PupilDetails pupil1 = new PupilDetails();
            string s = "";

            ExamFiles ef = new ExamFiles();
            Encode en = new Encode();
            int n = 0; int n1 = 0;
            foreach (SimplePupil p in sl1._studentlist)
            {
                pupil1.m_UCI = "";
                pupil1.Load(p.m_StudentId.ToString());
                string year = DateTime.Now.Year.ToString();
                if ((pupil1.m_UCI == "") && (pupil1.m_examNo > 0))
                {
                    pupil1.m_UCI = ef.Calculate_UCI_Checksum("52205", "0", year, pupil1.m_examNo.ToString());
                    s = "UPDATE dbo.tbl_Core_Students SET StudentUCI='" + pupil1.m_UCI + "' ";
                    s += "WHERE StudentId = '" + p.m_StudentId.ToString() + "' ";
                    en.ExecuteSQL(s);
                    n++;
                }
                if (pupil1.m_examNo == 0) n1++;
            }
            Label1.Text = "Created " + n.ToString() + " new UCIs.  There were " + n1.ToString() + " students found with no Exam Number!";
        }
    }
}