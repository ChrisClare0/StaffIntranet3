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

        enum TextFileType { Unknown, AQA_GCE,OCR_GCE,EDEXCEL_GCE,AQA_GCSE,OCR_GCSE,EDEXCEL_GCSE,CIE,WJEC_GCE,WJEC_GCSE}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetMasterValues();

                foreach (TextFileType eb in Enum.GetValues(typeof(TextFileType)))
                {
                    ListItem l1 = new ListItem(eb.ToString(), eb.ToString());
                    DropDownList1.Items.Add(l1);
                }
            }
        }
        private void GetMasterValues()
        {
            SeasonCode = System.Convert.ToInt32((string)(Session["Season"]));
            Year = System.Convert.ToInt32((string)(Session["Year"]));

            //testing
            Year = 2019;SeasonCode = 6;
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

            int n_Year = 0; int n_Season = 0; int n_OptionCode = 0; int n_TQM = 0; int n_ComponentCode = 0; int n_ScaledMark = 0; int n_ComponentUMS = 0; int n_Status = 0;
            bool f_Year = false; bool f_Season = false; bool f_OptionCode = false;
            bool f_TQM = false; bool f_ComponentCode = false; bool f_ScaledMark = false; bool f_ComponentUMS = false; bool f_Status = false;


            int n_CandNo = 0;bool f_CandNo = false;

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
                    //going to look for our columns:
                    for (int i = 0; i <= t.number_fields; i++)
                    {
                        switch (t.field[i])
                        {
                            case "Series Year": n_Year = i; f_Year = true; break;
                            case "Series Code": n_Season = i; f_Season = true; break;
                            case "Entry Code": n_OptionCode = i;  f_OptionCode = true; break;
                            case "Result Mark": n_TQM = i; f_TQM = true; break;
                            case "Component Code": n_ComponentCode = i;f_ComponentCode = true; break;
                            case "Component UMS": n_ComponentUMS = i; f_ComponentUMS = true; break;
                            case "Component Scaled Mark": n_ScaledMark = i;f_ScaledMark = true; break;
                            case "Component Status Notes": n_Status = i;f_Status = true; break;
                            case "Candidate Number":    n_CandNo = i;f_CandNo = true; break;
                            default: break;
                        }
                    }

                    // so do we have all the required columns??
                    if (!(f_Year && f_Season && f_OptionCode  && f_TQM && f_ComponentCode && f_ScaledMark && f_ComponentUMS && f_Status && f_CandNo))
                    {

                        Label_Text.Text = "Can't recognise the format of this file. ";
                        if (!f_Year) { Label_Text.Text += "No YearColumn"; }
                        if (!f_Season) { Label_Text.Text += "No SeasonColumn"; }
                        if (!f_OptionCode) { Label_Text.Text += "No OptionCodeColumn"; }
                        if (!f_TQM) { Label_Text.Text += "No TQMColumn"; }
                        if (!f_ComponentCode) { Label_Text.Text += "No ComponnetCodeColumn"; }
                        if (!f_ComponentUMS) { Label_Text.Text += "No ComponentUMSColumn"; }
                        if (!f_ScaledMark) { Label_Text.Text += "No ScaledMarkColumn"; }
                        if (!f_Status) { Label_Text.Text += "No StatusColumn"; }
                    }
                    else
                    {
                        foreach (TextFileType eb in Enum.GetValues(typeof(TextFileType)))
                        {
                            if (DropDownList1.SelectedValue == eb.ToString()) { ftype1 = eb; break; }
                        }

                        Label_Text.Text = "File Format Fine:     Board: "+ftype1.ToString();
                    }

                }
                ViewState["TextFileType"] = ftype1;
                ViewState["n_Year"] = n_Year;
                ViewState["n_Season"] = n_Season;
                ViewState["n_OptionCode"] = n_OptionCode;
                ViewState["n_TQM"] = n_TQM;
                ViewState["n_ComponentCode"] = n_ComponentCode;
                ViewState["n_ComponentUMS"] = n_ComponentUMS;
                ViewState["n_ScaledMark"] = n_ScaledMark;
                ViewState["n_Status"] = n_Status;
                ViewState["n_CandNo"] = n_CandNo;


                TextBox1.Text = s1;
                Button_Display.Visible = false;
                Button_Process.Visible = true;
            }
        }

        protected void Button_Process_Click(object sender, EventArgs e)
        {
            string s = TextBox1.Text;
            TextFileType ftype1 = TextFileType.Unknown;
            try
            {
                ftype1 = (TextFileType)ViewState["TextFileType"];
            }
            catch
            {
                Label_Text.Text = "File Type not recognised"; return;
            }
            if (ftype1 == TextFileType.Unknown) { Label_Text.Text = "Board Not Set!!!"; return; }
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

            int n_Year = (int)ViewState["n_Year"];
            int n_Season = (int)ViewState["n_Season"];
            int n_OptionCode = (int)ViewState["n_OptionCode"];
            int n_TQM = (int)ViewState["n_TQM"];
            int n_ComponentCode = (int)ViewState["n_ComponentCode"];
            int n_ComponentUMS = (int)ViewState["n_ComponentUMS"];
            int n_ScaledMark = (int)ViewState["n_ScaledMark"];
            int n_Status = (int)ViewState["n_Status"];
            int n_CandNo = (int)ViewState["n_CandNo"];


            int number_entered = 0;
            //get exam board....
            switch (ftype1)
            {
                case TextFileType.Unknown:
                    return;
                case TextFileType.AQA_GCE:
                    eb1 = new Exam_Board("70");
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
                case TextFileType.WJEC_GCSE:
                    eb1 = new Exam_Board("40");
                    break;
                case TextFileType.WJEC_GCE:
                    eb1 = new Exam_Board("41");
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

                    adno1 = Convert.ToInt32(fields[n_CandNo]);
                    if (Year.ToString() != fields[n_Year]) { Label_Text.Text = "Year code mismatch in file, line" + number_entered.ToString(); return; }
                    if (SeasonCode.ToString() != fields[n_Season].Substring(0, 1)) { Label_Text.Text = "Series code mismatch in file, line" + number_entered.ToString(); return; }
                    component_code = fields[n_ComponentCode];
                    Option_Code = fields[n_OptionCode];
                    ScaledMark = fields[n_ScaledMark];
                    ComponentUMS = fields[n_ComponentUMS];
                    ComponentStatus = fields[n_Status];
                    TQM_value = fields[n_TQM];

                    ExamComponentResult r = new ExamComponentResult();

                    try
                    {
                        p1.Load_StudentIdOnly(adno1);
                        ec1.Load(component_code, SeasonCode.ToString(), YearCode.ToString());

                        exo1.Load(Option_Code, SeasonCode.ToString(), YearCode.ToString(), eb1.m_ExamBoardId);
                        r.OptionId = exo1.m_OptionID;
                        r.ComponentId = ec1.m_ComponentID;
                        r.StudentId = p1.m_StudentId;
                        try
                        {
                            r.ComponentScaledMark = System.Convert.ToInt32(ScaledMark);
                        }
                        catch
                        {
                            //assume it is non integer....
                            double d = Convert.ToDouble(ScaledMark);
                            r.ComponentScaledMark = Convert.ToInt32(d);

                        }
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
                        rl1.m_parameters = 0;
                        rl1.m_where = "WHERE (dbo.tbl_Core_Students.StudentId='" + r.StudentId.ToString() + "')AND(dbo.tbl_Core_Results.ExamsOptionId='" + r.OptionId.ToString() + "') ";
                        rl1.LoadList("", "");
                        Result r1 = new Result();
                        r1 = (Result)rl1._results[0];
                        r1.Text = "TQM=" + TQM_value;
                        r1.UpdateResultTextOnly();
                        number_entered++;
                    }
                    catch (Exception ex)
                    {
                        Label_Text.Text = ex.ToString();
                    }

                }
            }
            Label_Text.Text = "Correctly processed... " + number_entered.ToString() + " entries";
        }
    }
}