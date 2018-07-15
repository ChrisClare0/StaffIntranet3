using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.IO;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class EDICreateEntryFile : System.Web.UI.Page
    {
        #region variables

        public string DisplayType
        {
            get { return (((String)ViewState["Type"] == null) ? String.Empty : (String)ViewState["Type"]); }
            set { ViewState["Type"] = value; }
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
                //are there any entries not sent
                Encode en = new Encode();
                Exam_Entry_CountbyBoardList el1 = new Exam_Entry_CountbyBoardList();
                el1.Load_NotSent(SeasonCode.ToString(), Year.ToString());//ie NOT sent
                if(el1.m_list.Count>0)//ie entries to send
                {
                    SqlDataSource1.SelectCommand = GetSummaryString();
                    SqlDataSource1.ConnectionString = en.GetDbConnection();
                    SqlDataSource1.DataBind();
                }
                else
                {
                    Label_message.Text = "No Entries to Send";
                }
            }
        }

        protected string GetSummaryString()
        {
            string s = "SELECT   dbo.tbl_Core_Organisations.OrganisationFriendlyName AS Board , dbo.tbl_Exams_Options.SeriesIdentifier,  ";
            s += "  COUNT(dbo.tbl_Exams_Entries.ExamEntryID) AS NumberNotSent, dbo.tbl_Core_ExamBoards.LegacyExamBdId AS BoardId";
            s += " FROM dbo.tbl_Exams_Entries INNER JOIN";
            s += " dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN ";
            s += " dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID INNER JOIN ";
            s += " dbo.tbl_Core_ExamBoards ON dbo.tbl_Exams_Syllabus.ExamBoardID = dbo.tbl_Core_ExamBoards.ExamBoardId INNER JOIN ";
            s += " dbo.tbl_Core_Organisations ON dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId = dbo.tbl_Core_Organisations.OrganisationId ";
            s += " GROUP BY dbo.tbl_Exams_Syllabus.ExamBoardID, dbo.tbl_Core_ExamBoards.LegacyExamBdId, dbo.tbl_Core_Organisations.OrganisationName, dbo.tbl_Core_Organisations.OrganisationFriendlyName, ";
            s += " dbo.tbl_Exams_Entries.EntryStatus, dbo.tbl_Exams_Entries.ExamSeason, dbo.tbl_Exams_Entries.ExamYear, ";
            s += " dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId, dbo.tbl_Exams_Options.SeriesIdentifier, dbo.tbl_Exams_Entries.EntryFileID ";
            s += " HAVING (dbo.tbl_Exams_Entries.EntryFileID IS NULL ) ";
            s += "AND(dbo.tbl_Exams_Entries.ExamSeason = '" + SeasonCode.ToString() + "') ";
            s += " AND(dbo.tbl_Exams_Entries.ExamYear = '" + Year.ToString() + "') ";
            return s;
        }
        private void GetMasterValues()
        {
            SeasonCode = System.Convert.ToInt32((string)(Session["Season"]));
            Year = System.Convert.ToInt32((string)(Session["Year"]));
            YearCode = Year % 100;
            Label Label_Banner = (Label)Master.FindControl("Label_Banner");
            if (Label_Banner != null)
            {
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]);
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Process")
            {

                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                string s = row.Cells[4].Text;
                Exam_Board eb1 = new Exam_Board(s);
                s = eb1.m_OrganisationFriendlyName;

                string series = row.Cells[2].Text;
                if (ProcessFile(eb1, series, ref s))
                {
                    s = "success";
                    Label_message.Text = "Success! ";
                }
                else
                {
                    Label_message.Text = s;
                }
                Encode en = new Encode();
                Exam_Entry_CountbyBoardList el1 = new Exam_Entry_CountbyBoardList();
                el1.Load(SeasonCode.ToString(), Year.ToString(), "4", false);//ie NOT sent
                if (el1.m_list.Count > 0)//ie entries to send
                {
                    SqlDataSource1.SelectCommand = GetSummaryString();
                    SqlDataSource1.ConnectionString = en.GetDbConnection();
                    SqlDataSource1.DataBind();
                }
                else
                {
                    Label_message.Text = "All Entries sent!";
                }
            }

        }

        private void Write_to_file(string s, string filename)
        {
            System.IO.StreamWriter ts = new System.IO.StreamWriter(filename, true);
            ts.WriteLine(s);
            ts.Close();
        }
        private StreamWriter OpenFile(Exam_Board exb1, char type, ref Exam_File ef1, int Record_Length, string series, string centre_number, string jcqVersion)
        {
            //open file and write header records...
            int sequence_no = ef1.FindSequencNumber(exb1.m_ExamBoardId.ToString()) + 1;
            string s = type + centre_number + exb1.m_LegacyExamBdId + ".X";
            string s1 = sequence_no.ToString(); s1 = "000" + s1; s1 = s1.Substring(s1.Length - 2, 2);
            string filename = s + s1;
            string path = Server.MapPath("ExamFiles");

            StreamWriter fs = new StreamWriter(path + "//"+ filename);
            s = type + "1" + centre_number + exb1.m_LegacyExamBdId.Trim() + series + Year.ToString().Substring(2, 2);
            s += "S" + centre_number + "  103" + jcqVersion.ToString();
            while (s.Length < Record_Length - 2) s += " ";
            fs.WriteLine(s);

            s = type + "3" + centre_number + exb1.m_LegacyExamBdId + series + Year.ToString().Substring(2, 2);
            s1 = sequence_no.ToString(); s1 = "000" + s1; s1 = s1.Substring(s1.Length - 3, 3);
            s += s1;
            s += "HP65HA  ";
            while (s.Length < Record_Length - 2) s += " ";
            fs.WriteLine(s);

            ef1.m_DateCreated = DateTime.Now;
            ef1.m_Fileame = filename;
            ef1.m_SequenceNo = sequence_no;
            ef1.m_ExamBoardID = exb1.m_ExamBoardId;
            ef1.Save();
            return fs;
        }
        private void EndFile(char type, int n_records, DateTime date1, StreamWriter fs, int Record_Length, Exam_File ef1)
        {
            string s = ""; string s1 = "";
            string centre_number = "52205";
            //TODO   CENTRE NUMBER????
            //centre trailer record
            s = type + "7" + centre_number; n_records = n_records + 2;
            s1 = n_records.ToString(); s1 = s1 + "0000000" + s1;
            s1 = s1.Substring(s1.Length - 7, 7);
            s += s1;
            s += date1.ToString("ddMMyy");
            while (s.Length < Record_Length - 2) s += " ";
            fs.WriteLine(s);

            //file trailer
            s = type + "9" + centre_number; n_records = n_records + 2;
            s1 = n_records.ToString(); s1 = s1 + "0000000" + s1;
            s1 = s1.Substring(s1.Length - 7, 7);
            s += s1;
            s += "0000001";
            while (s.Length < Record_Length - 2) s += " ";
            fs.WriteLine(s);
            fs.Close();

            string path  = Server.MapPath("ExamFiles") ;
            string outbox = path + @"\examout\" + ef1.m_Fileame;
            string examfiles = path + @"\" + ef1.m_Fileame;
            System.IO.File.Copy(examfiles, outbox, true);
        }

        private bool ProcessFile(Exam_Board exb1, string series, ref string Errors)
        {

            //so are we able to do E file or A file...
            string s = "";
            string line = "";
            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_Exams_CentreNumber");
            string centre_number = "";
            if (c.valid) centre_number = c.Value; else { Errors = "Centre Number not found in cerval..." + centre_number; return false; }


           

            //check that if we are asked for an E file we dont already have one...
            //could just check no entries have been sent for this series...
            ExamEntries_List exl0 = new ExamEntries_List();
            exl0.LoadAllSeries(Year.ToString(), series, true, exb1.m_ExamBoardId.ToString());
            char type =exl0.m_list.Count > 0 ? 'A' : 'E';

            int n1 = 0;
            Encode en = new Encode();
            PupilDetails p1 = new PupilDetails();
            DateTime date1 = new DateTime();
            date1 = DateTime.Now;


            if (centre_number.Length != 5){Errors="Centre Number wrong!" + centre_number; return false; }
 
            Exam_File ef1 = new Exam_File();
            StreamWriter fs = OpenFile(exb1, type, ref ef1, 194,series,centre_number,"14");
            if (fs == null) { Errors = "Error opening file"; return false; }
            if (type == 'A')
            {
                //going to find any not yet sent and for these students clear the sent dates...
                exl0.m_list.Clear();



                exl0.LoadAllSeries(Year.ToString(), series, false, exb1.m_ExamBoardId.ToString());
                
                
                //now for each student in this list we need to re-send entries....
                foreach (Exam_Entry ex in exl0.m_list)
                {
                    ExamOption exo0 = new ExamOption();
                    exo0.Load(ex.m_OptionID);
                    ExamEntries_List exl2 = new ExamEntries_List();
                    exl2.Load(ex.m_StudentID, Year.ToString(), series, true, exb1.m_ExamBoardId.ToString());
                    foreach (Exam_Entry ex0 in exl2.m_list)
                    {
                        s = "UPDATE tbl_Exams_Entries SET EntryFileID = NULL , DateEntered = NULL WHERE (ExamEntryID ='" + ex0.m_ExamEntryID.ToString() + "')";
                        n1 = en.Execute_count_SQL(s);
                    }
                }
            }

            ExamEntries_List exl1 = new ExamEntries_List();
            exl1.LoadAllSeries(Year.ToString(), series, false, exb1.m_ExamBoardId.ToString());
            int n = 0;
            int n_record = -1;//don't write first one
            int n_records = 0;
            Guid std1 = Guid.Empty;

            foreach (Exam_Entry ex1 in exl1.m_list)
            {
                if (ex1.m_StudentID != std1)
                {
                    //have a new student
                    while (line.Length < 192) line += " ";
                    if (n_record >= 0) { fs.WriteLine(line); n_records++; }
                    std1 = ex1.m_StudentID;
                    p1.m_UCI = "";
                    p1.Load(std1.ToString());
                    n_record = 0;
                    line = "";
                }
                if (n_record == 12)
                {
                    while (line.Length < 192) line += " ";
                    fs.WriteLine(line); n_records++;
                    line = ""; n_record = 0;
                }
                n++; 
                if (line == "")
                {
                    line = type + "5";
                    if (p1.m_IsOnRole) line += "C"; else line += "P";
                    line += centre_number;
                    s = p1.m_examNo.ToString();
                    while (s.Length < 4) s = "0" + s;
                    line += s;
                    //going to strip , from middle names to space...
                    s = p1.m_Surname + ":" + p1.m_GivenName + " " + p1.m_MiddleName.Replace(",", " ");
                    while (s.Length < 40) s += " ";
                    if (s.Length > 40) s.Substring(0, 40);
                    line += s;
                    if (p1.m_Gender == "F") line += "F"; else line += "M";
                    if (p1.m_dob == null) s = "000000"; else s = p1.m_dob.ToString("ddMMyy");
                    if (s == "010101") s = "000000";//010101  appears to the the defauult date if date in NULL in db
                    line += s;
                    if ((p1.m_UCI.Length != 13) || (p1.m_examNo == 0))
                    {
                        // todo  .. need to hanlde this error
                        /// clean up or offer UCI generation....
                        Errors = "No UCI for " + p1.m_GivenName + " " + p1.m_Surname + "(" + p1.m_adno.ToString() + ")";
                        Errors += "Please create UCI for student and re-run";
                        //first mark any entries made for this file as not made........
                        s = "UPDATE dbo.tbl_Exams_Entries SET EntryFileID = NULL , DateEntered = NULL ";
                        s += " WHERE (EntryFileID ='" + ef1.m_EntryFileId.ToString() + "' )";
                        Encode en1 = new Encode();
                        n1 = en1.Execute_count_SQL(s);
                        fs.Close();
                        return false;
                    }
                    s = p1.m_UCI;
                    line += s;
                    line += p1.m_upn.Trim();

                    //TODO....
                    line+=p1.m_uln.Trim();
                    //line += "          ";//uln
                    //now the guest flag..... hmm...
                    //ought to do this really for cand from collaboration...

                    line += "             ";// up to byte 108 in jcq mess...


                }
                ExamOption exo1 = new ExamOption();
                exo1.Load(ex1.m_OptionID);
                if (exo1.m_ExamBoardID == exb1.m_ExamBoardId)
                {
                    //add it to the file unless withdrawn
                    if (!ex1.m_withdrawn)
                    {
                        s = exo1.m_OptionCode + "      "; s = s.Substring(0, 6);
                        line += s + " "; n_record++;
                    }
                    //now update the Exam Entry record...
                    s = "UPDATE tbl_Exams_Entries SET EntryFileID ='" + ef1.m_EntryFileId.ToString() + "' ";
                    s += " , DateEntered =CONVERT(DATETIME, '" + ef1.m_DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)  ";
                    s += " , EntryStatus = '4' ";
                    s += " WHERE (ExamEntryID = '" + ex1.m_ExamEntryID.ToString() + "' )";
                    en.ExecuteSQL(s);
                }
            }
            while (line.Length < 192) line += " ";
            if (n_record >= 0) { fs.WriteLine(line); n_records++; }
            EndFile(type, n_records, date1, fs, 194, ef1);
            return true;
        }

    }
}