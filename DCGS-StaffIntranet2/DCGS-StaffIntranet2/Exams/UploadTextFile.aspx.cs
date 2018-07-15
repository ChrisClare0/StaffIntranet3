using System;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.IO;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class UploadTextFile : System.Web.UI.Page
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

        enum TextFileType {Unknown,Students,Sets}

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Uploading from Text File";
            }
        }

        protected void Button_Upload_Click(object sender, EventArgs e)
        {
            if (!FileUpload1.HasFile) return;
            TextFileType ftype1 = TextFileType.Unknown;
            
            string s = Server.MapPath("BaseData") + "\\" + FileUpload1.FileName;

            Cerval_Library.TextReader TxtRd1 = new Cerval_Library.TextReader();
            TextRecord t = new TextRecord();int l = 0;int n = 0;
            string s1 = ""; char ct = (char)0x09;

            while (TxtRd1.ReadTextLine(FileUpload1.FileContent,ref t) == Cerval_Library.TextReader.READ_LINE_STATUS.VALID)
            {
                l = s1.Length;
                for (int i = 0; i < t.number_fields; i++)
                {
                    if (t.field[i].Length ==0) break;
                    else s1 += t.field[i] + ct;
                }
                if(s1.Length>l)s1 += Environment.NewLine;
                if (n == 0)
                {
                    //this is first line so has headers....
                    if (t.field[0].ToUpper().Trim() == "SET") ftype1 = TextFileType.Sets;
                    if (t.field[0].ToUpper().Trim() == "ADNO") ftype1 = TextFileType.Students;
                    if (t.field[0].ToUpper().Trim() == "ADMISSION NUMBER") ftype1 = TextFileType.Students;
                    switch (ftype1)
                    {
                        case TextFileType.Unknown:
                            Label_Text.Text = "First column must be either 'Set' or 'adno' or 'Admission Number'";
                            break;
                        case TextFileType.Students:
                            Label_Text.Text = "Assuming columns are: Admission Number,Surname, GivenName,Board,Syllabus,Option...";
                            break;
                        case TextFileType.Sets:
                            Label_Text.Text = "Assuming columns are: Set, Board, Syllabus, Option.......";
                            break;
                        default:
                            break;
                    }
                }
            }
            ViewState["TextFileType"] = ftype1;
            TextBox1.Text = s1;
            Button_CheckTime.Visible = false;
            Button_Upload.Visible = false;
            Button_Process.Visible = true;          
        }

        protected void Button_Process_Click(object sender, EventArgs e)
        {
            string s = TextBox1.Text;
            string ErrorMsg = "";
            TextFileType ftype1 = TextFileType.Unknown;
            try
            {
                ftype1 = (TextFileType)ViewState["TextFileType"];
            }
            catch
            {
                Label_Text.Text = "File Type not recognised"; return;
            }
            char[] ct1 = new char[1];ct1[0]= (char)0x09;
            string[] fields = new string[20];

            ExamsUtility u = new ExamsUtility();
            PupilGroupList pgl = new PupilGroupList();
            ExamConversions Ec = new ExamConversions();
            SimplePupil pupil1 = new SimplePupil();
            Guid g1 = new Guid();g1 = Guid.Empty;
            int number_entered = 0;
            using (StringReader sr = new StringReader(s))
            {
                string firstline = sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    fields = line.Split(ct1);
                    
                    switch (ftype1)
                    {
                        case TextFileType.Unknown:
                            Label_Text.Text = "File Type not recognised"; 
                            break;
                        case TextFileType.Students:   //Admission Number,Surname, GivenName,Board,Syllabus,Option...";                      
                            try
                            {
                                Exam_Board exbde1 = Ec.GetExamBoard(fields[3]);
                                pupil1.Load_StudentIdOnly(System.Convert.ToInt32(fields[0]));///loads lft students aswell
                                if (!u.AddEntry(pupil1.m_StudentId, exbde1, Year, YearCode, SeasonCode, fields[5], 3, true, ref ErrorMsg,ref g1))
                                {
                                    Label_Text.Text = ErrorMsg; return;
                                }
                                else
                                {
                                    number_entered++;
                                }
                            }
                            catch (Exception ex)
                            {
                                Label_Text.Text = ex.ToString(); return;
                            }
                            break;
                        case TextFileType.Sets:      //Set, Board, Syllabus, Option.                    
                            pgl.m_pupilllist.Clear();
                            pgl.AddToList(fields[0], DateTime.Now);
                            
                            foreach (SimplePupil p in pgl.m_pupilllist)
                            {
                                try
                                {
                                    Exam_Board exbde1 = Ec.GetExamBoard(fields[1]);
                                    if (!u.AddEntry(p.m_StudentId, exbde1, Year, YearCode, SeasonCode, fields[3], 3, true, ref ErrorMsg,ref g1))
                                    {
                                        Label_Text.Text = ErrorMsg; return;
                                    }
                                    else
                                    {
                                        number_entered++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Label_Text.Text = ex.ToString(); return;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            Label_Text.Text = "Correctly processed... " + number_entered.ToString()+" entries";
        }

        protected void Button_CheckTime_Click(object sender, EventArgs e)
        {
            string s = TextBox1.Text;
            string ErrorMsg = "";
            TextFileType ftype1 = TextFileType.Unknown;
            try
            {
                ftype1 = (TextFileType)ViewState["TextFileType"];
            }
            catch
            {
                Label_Text.Text = "File Type not recognised"; return;
            }
            char[] ct1 = new char[1]; ct1[0] = (char)0x09;
            string[] fields = new string[20];
            ExamsUtility u = new ExamsUtility();
            PupilGroupList pgl = new PupilGroupList();
            ExamConversions Ec = new ExamConversions();
            SimplePupil pupil1 = new SimplePupil();
            Guid g1 = new Guid(); g1 = Guid.Empty;
            DateTime t1 = new DateTime();t1 = DateTime.Now;
            using (StringReader sr = new StringReader(s))
            {
                string firstline = sr.ReadLine();
                string line = sr.ReadLine();
                fields = line.Split(ct1);
                Exam_Board exbde1 = Ec.GetExamBoard(fields[1]);
                switch (ftype1)
                {
                    case TextFileType.Unknown:
                        Label_Text.Text = "File Type not recognised";
                        break;
                    case TextFileType.Students:   //Admission Number,Surname, GivenName,Board,Syllabus,Option...";                      
                        try
                        {
                            pupil1.Load_StudentIdOnly(System.Convert.ToInt32(fields[0]));///loads lft students aswell
                            if (!u.AddEntry(pupil1.m_StudentId, exbde1, Year, YearCode, SeasonCode, fields[3], 3, true, ref ErrorMsg, ref g1))
                            {
                                Label_Text.Text = ErrorMsg; return;
                            }
                            else
                            {
                                Exam_Entry en1 = new Exam_Entry(); en1.m_ExamEntryID = g1; en1.Delete();
                            }
                        }
                        catch (Exception ex)
                        {
                            Label_Text.Text = ex.ToString(); return;
                        }
                        break;
                    case TextFileType.Sets:
                        pgl.m_pupilllist.Clear();
                        pgl.AddToList(fields[0], DateTime.Now);
                        foreach (SimplePupil p in pgl.m_pupilllist)
                        {
                            try
                            {
                                if (!u.AddEntry(p.m_StudentId, exbde1, Year, YearCode, SeasonCode, fields[3], 3, true, ref ErrorMsg, ref g1))
                                {
                                    Label_Text.Text = ErrorMsg; return;
                                }
                                else
                                {
                                    Exam_Entry en1 = new Exam_Entry(); en1.m_ExamEntryID = g1; en1.Delete();
                                }
                            }
                            catch (Exception ex)
                            {
                                Label_Text.Text = ex.ToString(); return;
                            }
                        }
                        break;
                    default:
                        break;
                }

                //done one set or one student...scale up....
                TimeSpan ts1 = DateTime.Now - t1; TimeSpan ts2 = new TimeSpan(); ts2 = ts1;
                while ((line = sr.ReadLine()) != null)
                {
                    ts2 += ts1;
                }
                TextBox_Warning.Text = "WARNING:  This operation is likely to take about " + ts2.ToString() + Environment.NewLine + "Press Process Button to continue...";
                TextBox_Warning.Visible = true;
                Button_CheckTime.Visible = false;
                Button_Process.Visible = true;
            }
        }
    }
}