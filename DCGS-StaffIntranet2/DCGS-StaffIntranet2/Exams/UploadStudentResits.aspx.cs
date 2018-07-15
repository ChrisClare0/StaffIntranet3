using System;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class UploadStudentResits : System.Web.UI.Page
    {
        #region variables
        public string StudentID
        {
            get { return (((String)ViewState["StudentID"] == null) ? String.Empty : (String)ViewState["StudentID"]); }
            set { ViewState["StudentID"] = value; }
        }
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Uploading entries from Student Website.";
            }
        }

        private string GetQueryString()
        {
            string s = " SELECT Id, OptionCode, OptionTitle, DateCreated, Cost ";
            s += " FROM qry_Cerval_Exams_ResitEntries";
            s += " WHERE StudentId = '" + StudentID + "' ";
            return s;
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            try
            {
                i = System.Convert.ToInt32(TextBox_StudentAdno.Text);
                SimplePupil p = new SimplePupil(); p.Load_Left(i);
                StudentID = p.m_StudentId.ToString();
                Encode en = new Encode();
                SqlDataSource1.SelectCommand = GetQueryString();
                SqlDataSource1.ConnectionString = en.GetDbConnection();
                SqlDataSource1.DataBind();
                Label_Grid.Text = "Entries for " + p.m_GivenName + " " + p.m_Surname + " (" + p.m_adno.ToString() + ")";

            }
            catch { }
        }

        protected void Button_Upload_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow r in GridView1.Rows)
            {
                if (r.BackColor == System.Drawing.Color.Red)
                {
                    string ErrorMsg = ""; Label_Error.Text = "";

                    Exam_ResitEntry exr1 = new Exam_ResitEntry();
                    Guid g1 = new Guid(r.Cells[1].Text);
                    exr1.Load(g1);

                    ExamOption exo1 = new ExamOption();
                    exo1.Load(exr1.m_OptionId);
                    Exam_Board exbde1 = new Exam_Board(exo1.m_ExamBoardID);
                    Guid g2 =new Guid();g2 = Guid.Empty;
                    ExamsUtility u = new ExamsUtility();
                    try
                    {
                        if (u.AddEntry(new Guid(StudentID), exbde1, Year, YearCode, SeasonCode, r.Cells[2].Text, 1, true, ref ErrorMsg, ref g2))
                        {
                            //it worked so delete the resit entry
                            exr1.Delete();
                            Encode en = new Encode();
                            SqlDataSource1.SelectCommand = GetQueryString();
                            SqlDataSource1.ConnectionString = en.GetDbConnection();
                            SqlDataSource1.DataBind();
                        }
                        else
                        {
                            //it didn't
                            Label_Error.Text = ErrorMsg; break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Label_Error.Text = ex.ToString(); break;
                    }
                }

            }
            int cost = 0;

            foreach (GridViewRow r in GridView1.Rows)
            {
                if (r.BackColor == System.Drawing.Color.Red)
                {
                    cost += System.Convert.ToInt32(r.Cells[5].Text);
                }
            }
            double costd = (double)cost / 100;
            Label_cost.Text = " Total cost of selected = £" + costd.ToString();
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            //about to select a row...toggle the colour
            GridViewRow row = GridView1.Rows[e.NewSelectedIndex];
            if (row.BackColor == System.Drawing.Color.Red) row.BackColor = System.Drawing.Color.Empty;
            else row.BackColor = System.Drawing.Color.Red;
            int cost = 0;
            foreach (GridViewRow r in GridView1.Rows)
            {
                if (r.BackColor == System.Drawing.Color.Red)
                {
                    cost += System.Convert.ToInt32(r.Cells[5].Text);
                }
            }
            double costd = (double)cost / 100;
            Label_cost.Text = " Total cost of selected = £"+costd.ToString();
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {

            //going to check if any need teacher marks.
            string s = "";
            foreach (GridViewRow r in GridView1.Rows)
            {
                Exam_ResitEntry exr1 = new Exam_ResitEntry();
                Guid g1 = new Guid(r.Cells[1].Text);
                exr1.Load(g1);
                ExamOption exo1 = new ExamOption();exo1.Load(exr1.m_OptionId);
                if(exo1.NeedsTeacherMarks())
                {
                    s += "WARNING: Option " + r.Cells[2].Text + " requires Teacher Marks!   ";
                }
            }
            Label_Error.Text = s;
        }
    }
}