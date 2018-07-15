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
    public partial class UploadComponentResults : System.Web.UI.Page
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

        enum TextFileType { Unknown, AQA_GCE,OCR_GCE,EDEXCEL_GCE,AQA_GCSE,OCR_GCSE,EDEXCEL_GCSE,CIE }

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

            //testing
            Year = 2017;SeasonCode = 6;
            YearCode = Year % 100;
            Label Label_Banner = (Label)Master.FindControl("Label_Banner");
            if (Label_Banner != null)
            {
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Uploading Component Results from Text File";
            }
        }

        protected void Button_Display_Click(object sender, EventArgs e)
        {
            if (!FileUpload1.HasFile) return;
            TextFileType ftype1 = TextFileType.Unknown;
            Cerval_Library.TextReader TxtRd1 = new Cerval_Library.TextReader();
            TextRecord t = new TextRecord(); int l = 0; int n = 0;
            string s1 = ""; char ct = (char)0x09;
            //AQA_GCE file structure: 
            //Series Year	Series Code	Candidate Number	Candidate Name	Candidate Status	Specification Code	Entry Code	Entry Title (and Suffix)	Result Grade	Result UMS/Points	Result Mark	Result Status Notes	Component Code	Component Title	Component Moderated Mark	Component Scaled Mark	Component UMS	Component Status Notes
            string AQA_GCE =  "Series Year	Series Code	Candidate Number	Candidate Name	Candidate Status	Specification Code	Entry Code	Entry Title (and Suffix)	Result Grade	Result UMS/Points	Result Mark	Result Status Notes	Component Code	Component Title	Component Moderated Mark	Component Scaled Mark	Component UMS	Component Status Notes"+ ct+ Environment.NewLine;
            //string s = Server.MapPath("BaseData") + "\\" + FileUpload1.FileName;

            //2017 AQA = PH Ch Hi Ec El Ar/Gr GS"
            string AQA_GCSE = "G" + AQA_GCE;
            //2017 AQA = Bi CH Ph Ge Ar Gr PE El En
            string EDEXCEL_GCSE = "EDEXCELSession";
            //2017 Ma Mu Dr
            string EDEXCEL_GCE = "Session	Centre	Candidate Number	UCI	DOB	Sex	Last Name	First Name	Subject	Subject Name	Subject Mark	Max Mark	Uniform Mark	Max Uniform Mark	Grade	Paper	Paper Weighting	Paper Mark	Paper Max Mark" + ct + Environment.NewLine; ;
            //2017 EDEXCEL =  Po Mu Dr 
            string OCR_GCSE = "";
            //2017 Rs FSMQ
            string OCR_GCE = "Series	Qualification	Code	Title	Candidate No	Candidate Name	UMS/Points	Grade	Weighted Mark	Component 01 Raw Mark	Component 02 Raw Mark" + ct + Environment.NewLine; ;
            //2017 Bi Et Ma FR Gm Sp Ge RS
            string CIE = "Syllabus	Option code	Centre number	Candidate number	Candidate name	Component Code	Component Mark	Syllabus total mark	Syllabus grade ";


            while (TxtRd1.ReadTextLine(FileUpload1.FileContent, ref t) == Cerval_Library.TextReader.READ_LINE_STATUS.VALID)
            {
                l = s1.Length;
                for (int i = 0; i <= t.number_fields; i++)
                {
                    s1 += t.field[i] + ct;
                }
                if (s1.Length > l) s1 += Environment.NewLine;
                if (n == 0)
                {
                    //this is first line so has headers....

                    if(s1==AQA_GCE)
                    {
                        ftype1 = TextFileType.AQA_GCE;
                    }

                    if (s1 == AQA_GCSE)
                    {
                        ftype1 = TextFileType.AQA_GCSE;

                    }

                    if (t.field[0].ToUpper().Trim() == "CIE")
                    {
                        ftype1 = TextFileType.CIE;
                    }

                    if (t.field[0].ToUpper().Trim() == "Series Year") ftype1 = TextFileType.AQA_GCE;//unique??
                    if (t.field[0].ToUpper().Trim() == EDEXCEL_GCE)
                    {
                        ftype1 = TextFileType.EDEXCEL_GCE;
                    }

                    if (t.field[0].ToUpper().Trim() == EDEXCEL_GCSE.ToUpper().Trim())
                    {
                        ftype1 = TextFileType.EDEXCEL_GCSE;
                    }

                    if (s1== OCR_GCE)//???? I think there may be more components...
                    {
                        ftype1 = TextFileType.OCR_GCE;
                    }



                    ///edexcel
                    ///ocr

                    switch (ftype1)
                    {
                        case TextFileType.Unknown:
                            Label_Text.Text = "Can't recognise the format of this file....";
                            break;
                        case TextFileType.AQA_GCE:
                            Label_Text.Text = "This is an AQA GCE file ...Column 0 = year, 1=series, 2=Candidate Number etc...";
                            break;
                        case TextFileType.OCR_GCE:
                            Label_Text.Text = "This is an OCR GCE file ...Column 0 = Series	 1= Qualification	2 =Code etc...";
                            break;
                        case TextFileType.EDEXCEL_GCE:
                            Label_Text.Text = "This is an EDEXCEL GCE file ...Column 0 =  etc...";
                            break;
                        case TextFileType.CIE:
                            Label_Text.Text = "This is an CIE file ...Column 0 =  etc...";
                            break;
                        default:
                            break;
                    }
                }
            }
            ViewState["TextFileType"] = ftype1;
            TextBox1.Text = s1;
            Button_Display.Visible = false;
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
            char[] ct1 = new char[1]; ct1[0] = (char)0x09;
            string[] fields = new string[20];

            ExamsUtility u = new ExamsUtility();
            PupilGroupList pgl = new PupilGroupList();
            ExamConversions Ec = new ExamConversions();
            SimplePupil pupil1 = new SimplePupil();
            Guid g1 = new Guid(); g1 = Guid.Empty;
            Exam_Board eb1 = new Exam_Board();
            SimplePupil p1 = new SimplePupil();
            ExamComponent ec1 = new ExamComponent();

            ExamOption exo1 = new ExamOption();
            ExamComponentResultList ecrl1 = new ExamComponentResultList();
            string component_code = "";
            int adno1 = 0;
            string Option_Code = "";
            string ScaledMark = "";
            string ComponentUMS = "";
            string ComponentStatus = "";
            string TQM_value = "";
            string ResultGrade = "";

            //TODO  change this for real version
            //YearCode = 16;//for TESTING

            int number_entered = 0;
            switch (ftype1)
            {
                case TextFileType.Unknown:
                    return;
                    break;
                case TextFileType.AQA_GCE:
                    eb1 = new Exam_Board("70");
                    //eb1 = new Exam_Board("01");
                    break;
                case TextFileType.OCR_GCE:
                    eb1 = new Exam_Board("01");
                    break;
                case TextFileType.EDEXCEL_GCE:
                    eb1 = new Exam_Board("11");
                    break;
                case TextFileType.AQA_GCSE:
                    eb1 = new Exam_Board("70");
                    break;
                case TextFileType.OCR_GCSE:
                    break;
                case TextFileType.EDEXCEL_GCSE:
                    eb1 = new Exam_Board("10");
                    break;
                case TextFileType.CIE:
                    eb1 = new Exam_Board("02");
                    break;
                default:
                    break;
            }

            using (StringReader sr = new StringReader(s))
            {
                string firstline = sr.ReadLine();
                string line;
                
                while ((line = sr.ReadLine()) != null)
                {
                    fields = line.Split(ct1);
                    adno1 = 0;component_code = "";Option_Code = ""; ScaledMark = ""; ComponentUMS = ""; ComponentStatus = "";
                    switch (ftype1)
                    {
                        case TextFileType.Unknown:
                            return;
                            break;
                        case TextFileType.AQA_GCE:
                            adno1 = System.Convert.ToInt32(fields[2]);
                            if (Year.ToString() != fields[0]) { Label_Text.Text = "Year code mismatch in file, line" + number_entered.ToString(); return; }
                            if (SeasonCode.ToString() != fields[1].Substring(0, 1)) { Label_Text.Text = "Series code mismatch in file, line" + number_entered.ToString(); return; }
                            component_code = fields[12];
                            Option_Code = fields[6];
                            ResultGrade = fields[8];
                            ScaledMark = fields[15];
                            ComponentUMS = fields[16];
                            ComponentStatus = fields[17];
                            TQM_value = fields[10];
                            break;
                        case TextFileType.AQA_GCSE:
                            adno1 = System.Convert.ToInt32(fields[2]);
                            if (Year.ToString() != fields[0]) { Label_Text.Text = "Year code mismatch in file, line" + number_entered.ToString(); return; }
                            if (SeasonCode.ToString() != fields[1].Substring(0, 1)) { Label_Text.Text = "Series code mismatch in file, line" + number_entered.ToString(); return; }
                            component_code = fields[12];
                            Option_Code = fields[6];
                            ScaledMark = fields[15];
                            ComponentUMS = fields[9];
                            ComponentStatus = fields[17];
                            TQM_value = fields[10];

                            break;

                        case TextFileType.CIE:
                            adno1 = System.Convert.ToInt32(fields[3]);

                            component_code = fields[5];
                            Option_Code = fields[1];
                            
                            ComponentUMS = fields[6];
                            ScaledMark = ComponentUMS;
                            ComponentStatus = "";
                            TQM_value = fields[7];

                            break;

                        case TextFileType.EDEXCEL_GCSE:

                            adno1 = System.Convert.ToInt32(fields[2]);

                            component_code = fields[16];
                            Option_Code = fields[8];

                            ComponentUMS = "0";
                            ScaledMark = fields[18];
                            ComponentStatus = "";
                            TQM_value = fields[10];

                            break;




                        case TextFileType.OCR_GCE:
                            adno1 = System.Convert.ToInt32(fields[4]);
                            if (Year.ToString() != fields[0].Substring(4,2)) { Label_Text.Text = "Year code mismatch in file, line" + number_entered.ToString(); return; }
                            component_code = fields[2];
                            Option_Code = fields[2];
                            //!!!!!

                            break;
                        case TextFileType.EDEXCEL_GCE:
                            adno1 = System.Convert.ToInt32(fields[2]);
                            if (YearCode.ToString() != fields[0].Substring(0, 2)) { Label_Text.Text = "Year code mismatch in file, line" + number_entered.ToString(); return; }
                            if (SeasonCode.ToString() != fields[1].Substring(3, 1)) { Label_Text.Text = "Series code mismatch in file, line" + number_entered.ToString(); return; }
                            //so EdExcel seem to add 0X to the option code for component x so 6CH0101 is component 1 of Option 6CH01
                            //so component code is field[8] + "0" + number for field[15]
                            //only seen examples with 1 component!!... might have more cols??!???!?
                            component_code = fields[12] + "0" + fields[15];
                            Option_Code = fields[8];
                            ScaledMark = fields[17];
                            ComponentUMS = fields[12];
                            TQM_value = fields[10];//??????????????

                            break;

                        case TextFileType.OCR_GCSE:
                            break;

                        default:
                            break;
                    }


                    ExamComponentResult r = new ExamComponentResult();
                    switch (ftype1)
                    {
                        case TextFileType.Unknown:
                            Label_Text.Text = "File Type not recognised";
                            break;
                        case TextFileType.AQA_GCE:  //0 Series Year	1 Series Code	2 Candidate Number	3 Candidate Name	4 Candidate Status	5 Specification Code	6 Entry Code	7 Entry Title (and Suffix)	8 Result Grade	9 Result UMS/Points	10 Result Mark	11 Result Status Notes	12 Component Code	13 Component Title	14 Component Moderated Mark	15 Component Scaled Mark	16 Component UMS	17 Component Status Notes                  
                            try
                            {
                                p1.Load_StudentIdOnly(adno1);
                                ec1.Load(component_code, SeasonCode.ToString(), YearCode.ToString());
                                exo1.Load(Option_Code, SeasonCode.ToString(), YearCode.ToString(), eb1.m_ExamBoardId);
                                r.OptionId = exo1.m_OptionID;
                                r.ComponentId = ec1.m_ComponentID;
                                r.StudentId = p1.m_StudentId;
                                r.ComponentScaledMark = System.Convert.ToInt32(ScaledMark);
                                try{r.ComponentUMS = System.Convert.ToInt32(ComponentUMS);}catch { }
                                r.ComponentStatus = ComponentStatus;

                                switch (ecrl1.Load_OptionStudent( r.OptionId,r.StudentId,r.ComponentId))
                                {
                                    case 0:
                                        //so can write in
                                        //r.Save();
                                        break;
                                    case 1:
                                        //so update
                                        r.ComponentResultId = ecrl1.m_list[0].ComponentResultId;
                                        //r.Save();
                                        break;
                                    case 2:
                                        //so problem
                                        break;
                                    default:
                                        break;
                                }
                                //now find the result....
                                ResultsList rl1 = new ResultsList();
                                rl1.m_parameters = 0;rl1.m_where = "WHERE (dbo.tbl_Core_Students.StudentId='" + r.StudentId.ToString() + "')AND(dbo.tbl_Core_Results.ExamsOptionId='" + r.OptionId.ToString() + "') ";
                                rl1.LoadList("", "");
                                Result r1 = new Result();r1 = (Result)rl1._results[0];
                                r1.Text = "TQM=" + TQM_value;
                                if (r1.Value.Trim() != ResultGrade)
                                {
                                    s = r1.Value;
                                }
                                //r1.UpdateResultTextOnly();
                                number_entered++;
                            }
                            catch (Exception ex)
                            {
                                Label_Text.Text = ex.ToString();
                            }
                            break;

                        case TextFileType.AQA_GCSE:  //0 Series Year	1 Series Code	2 Candidate Number	3 Candidate Name	4 Candidate Status	5 Specification Code	6 Entry Code	7 Entry Title (and Suffix)	8 Result Grade	9 Result UMS/Points	10 Result Mark	11 Result Status Notes	12 Component Code	13 Component Title	14 Component Moderated Mark	15 Component Scaled Mark	16 Component UMS	17 Component Status Notes                  
                            try
                            {
                                p1.Load_StudentIdOnly(adno1);
                                ec1.Load(component_code, SeasonCode.ToString(), YearCode.ToString());
                                exo1.Load(Option_Code, SeasonCode.ToString(), YearCode.ToString(), eb1.m_ExamBoardId);
                                r.OptionId = exo1.m_OptionID;
                                r.ComponentId = ec1.m_ComponentID;
                                r.StudentId = p1.m_StudentId;
                                r.ComponentScaledMark = System.Convert.ToInt32(ScaledMark);
                                try { r.ComponentUMS = System.Convert.ToInt32(ComponentUMS); } catch { }
                                r.ComponentStatus = ComponentStatus;

                                switch (ecrl1.Load_OptionStudent(r.OptionId, r.StudentId, r.ComponentId))
                                {
                                    case 0:
                                        //so can write in
                                        r.Save();
                                        break;
                                    case 1:
                                        //so update
                                        r.ComponentResultId = ecrl1.m_list[0].ComponentResultId;
                                        r.Save();
                                        break;
                                    case 2:
                                        //so problem
                                        break;
                                    default:
                                        break;
                                }
                                //now find the result....
                                ResultsList rl1 = new ResultsList();
                                rl1.m_parameters = 0; rl1.m_where = "WHERE (dbo.tbl_Core_Students.StudentId='" + r.StudentId.ToString() + "')AND(dbo.tbl_Core_Results.ExamsOptionId='" + r.OptionId.ToString() + "') ";
                                rl1.LoadList("", "");
                                Result r1 = new Result(); r1 = (Result)rl1._results[0];
                                r1.Text = "TQM=" + TQM_value;
                                r1.UpdateResultTextOnly();
                                number_entered++;

                            }
                            catch (Exception ex)
                            {
                                Label_Text.Text = ex.ToString();
                            }
                            break;



                        case TextFileType.CIE:
                        case TextFileType.EDEXCEL_GCSE:                
                            try
                            {
                                p1.Load_StudentIdOnly(adno1);
                                ec1.Load(component_code, SeasonCode.ToString(), YearCode.ToString());
                                exo1.Load(Option_Code, SeasonCode.ToString(), YearCode.ToString(), eb1.m_ExamBoardId);
                                r.OptionId = exo1.m_OptionID;
                                r.ComponentId = ec1.m_ComponentID;
                                r.StudentId = p1.m_StudentId;
                                r.ComponentScaledMark = System.Convert.ToInt32(ScaledMark);
                                try { r.ComponentUMS = System.Convert.ToInt32(ComponentUMS); } catch { }
                                r.ComponentStatus = ComponentStatus;

                                switch (ecrl1.Load_OptionStudent(r.OptionId, r.StudentId, r.ComponentId))
                                {
                                    case 0:
                                        //so can write in
                                        r.Save();
                                        break;
                                    case 1:
                                        //so update
                                        r.ComponentResultId = ecrl1.m_list[0].ComponentResultId;
                                        r.Save();
                                        break;
                                    case 2:
                                        //so problem
                                        break;
                                    default:
                                        break;
                                }
                                //now find the result....
                                ResultsList rl1 = new ResultsList();
                                rl1.m_parameters = 0; rl1.m_where = "WHERE (dbo.tbl_Core_Students.StudentId='" + r.StudentId.ToString() + "')AND(dbo.tbl_Core_Results.ExamsOptionId='" + r.OptionId.ToString() + "') ";
                                rl1.LoadList("", "");
                                Result r1 = new Result(); r1 = (Result)rl1._results[0];
                                r1.Text = "TQM=" + TQM_value;
                                r1.UpdateResultTextOnly();
                                number_entered++;
                                return;
                            }
                            catch (Exception ex)
                            {
                                Label_Text.Text = ex.ToString();
                            }
                            break;


                        default:
                            break;
                    }
                }
            }
            Label_Text.Text = "Correctly processed... " + number_entered.ToString() + " entries";
        }
    }
}