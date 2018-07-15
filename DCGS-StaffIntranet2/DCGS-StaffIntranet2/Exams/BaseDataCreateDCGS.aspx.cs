using System;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.Collections.Generic;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class BaseDataCreateDCGS : System.Web.UI.Page
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"])+"   Create Base Data for Internal Exams";
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            //create table...

            List<ExamSyllabus> syllabuses = new List<ExamSyllabus>();
            List<ExamComponent> components=new List<ExamComponent>();
            List<ExamBaseOption> options=new List<ExamBaseOption>();
            List<ExamLinkComponent> Links = new List<ExamLinkComponent>();
            string s = TextBox1.Text; Label1.Text = "";
            Label1.BackColor = System.Drawing.Color.White;
            string[] s1 = new string[20];
            char[] c1 = new char[2];char c2= (char)0x09;c1[0] = c2;c1[1] = ',';
            string[] s2 = new string[1];s2[0] = Environment.NewLine;
            string [] s3 = new string[10];
            s1 = s.Split(s2, StringSplitOptions.RemoveEmptyEntries);
            int n = -1;bool correct = true;
            string errorstring = "";

            foreach (string s5 in s1)
            {
                n++;
                if (n == 0)
                {
                    //so this needs to be Subject	Option	Component   CODE    Date	Length	AM/PM
                    s3 = s5.Split(c1);
                    correct = (s3[0].ToUpper() == "SUBJECT") && (s3[1].ToUpper() == "OPTION") && (s3[2].ToUpper() == "COMPONENT");
                    correct = correct && (s3[3].ToUpper() == "CODE");
                    correct = correct && (s3[4].ToUpper() == "DATE") && (s3[5].ToUpper() == "LENGTH") && (s3[6].ToUpper() == "AM/PM");
                }
                if (!correct) { errorstring = "Header Row incorrect; should be  Subject	Option	Component	Date	Length	AM/PM"; break; }

                if (n > 0) { 
                s3 = s5.Split(c1); int k = 1;
                //so s3[0] has subject
                ExamSyllabus es1 = new ExamSyllabus();
                es1.m_Syllabus_Title = s3[0];


                es1.m_Syllabus_Code = s3[0].Substring(0, 2) + k.ToString();
                while (es1.m_Syllabus_Code.Length < 6) es1.m_Syllabus_Code += " ";
                while (es1.m_Syllabus_Title.Length < 36) es1.m_Syllabus_Title += " ";

                    //check code unique
                foreach (ExamSyllabus es in syllabuses)
                {
                    if ((es.m_Syllabus_Code == es1.m_Syllabus_Code)&&(es.m_Syllabus_Title!=es1.m_Syllabus_Title))
                        {
                            k++; es1.m_Syllabus_Code = s3[0].Substring(0, 2) + k.ToString();
                            while (es1.m_Syllabus_Code.Length < 6) es1.m_Syllabus_Code += " ";
                        }
                }

                syllabuses.Add(es1);
                    ExamBaseOption ebo1 = new ExamBaseOption();
                    ebo1.m_Title = s3[1]; k = 1;
                    if (s3[1].Contains("["))
                    {
                        //use defined code after [
                        int i1 = s3[1].IndexOf("[");
                        int i2= s3[1].IndexOf("]");
                        ebo1.m_OptionEntryCode = s3[1].Substring(i1 + 1, i2 - i1-1);
                        ebo1.m_Title = s3[1].Substring(0, i1);
                    }
                    else
                    {
                        ebo1.m_OptionEntryCode = s3[1].Substring(0, 2) + k.ToString();
                        //check code unique
                        foreach (ExamBaseOption ebo in options)
                        {
                            if ((ebo.m_OptionEntryCode == ebo1.m_OptionEntryCode) && (ebo.m_Title != ebo1.m_Title))
                            {
                                k++; ebo1.m_OptionEntryCode = s3[1].Substring(0, 2) + k.ToString();
                                while (ebo1.m_OptionEntryCode.Length < 6) ebo1.m_OptionEntryCode += " ";
                            }
                        }
                    }


                ebo1.m_SyllabusCode = es1.m_Syllabus_Code;
                while (ebo1.m_OptionEntryCode.Length < 6) ebo1.m_OptionEntryCode += " ";
                while (ebo1.m_Title.Length < 36) ebo1.m_Title += " ";

                options.Add(ebo1);

                ExamComponent ec1 = new ExamComponent();
                k = 1; int t1 = 0;
                ec1.m_ComponentCode = s3[3];
                ec1.m_ComponentTitle = s3[2];
                ec1.m_Time = s3[5];
                ec1.m_TimetableDate = new DateTime();
                try { ec1.m_TimetableDate = System.Convert.ToDateTime(s3[4]);}catch { errorstring = "Time must be in format dd/mm/yyyy"; correct = false; break; }
                ec1.m_TimetableSession = s3[6];
                try { t1 = Convert.ToInt16(s3[5]); } catch { errorstring = "Length must be an integer.";correct = false; break; }
                while (ec1.m_Time.Length < 3) ec1.m_Time = "0" + ec1.m_Time;
                while (ec1.m_ComponentCode.Length < 12) ec1.m_ComponentCode += " ";
                while (ec1.m_ComponentTitle.Length < 36) ec1.m_ComponentTitle += " ";

                //check code unique
                foreach (ExamComponent ec in components)
                {
                    if ((ec.m_ComponentCode == ec1.m_ComponentCode)&&(ec.m_ComponentTitle!=ec1.m_ComponentTitle)) { k++; ec1.m_ComponentCode = s3[2].Substring(0, 2) + k.ToString(); while (ec1.m_ComponentCode.Length < 12) ec1.m_ComponentCode += " "; }
                }
                components.Add(ec1);


                ExamLinkComponent el0 = new ExamLinkComponent();
                el0.m_ComponentCode = ec1.m_ComponentCode;
                el0.m_OptionCode = ebo1.m_OptionEntryCode;
                Links.Add(el0);


                TableRow r = new TableRow();
                Table1.Rows.Add(r);
                TableCell cell1 = new TableCell();cell1.Text = es1.m_Syllabus_Title;r.Cells.Add(cell1);
                TableCell cell2 = new TableCell(); cell2.Text = es1.m_Syllabus_Code; r.Cells.Add(cell2);
                TableCell cell3 = new TableCell(); cell3.Text = ebo1.m_Title; r.Cells.Add(cell3);
                TableCell cell4 = new TableCell(); cell4.Text = ebo1.m_OptionEntryCode; r.Cells.Add(cell4);
                TableCell cell5 = new TableCell(); cell5.Text = ec1.m_ComponentTitle; r.Cells.Add(cell5);
                TableCell cell6 = new TableCell(); cell6.Text = ec1.m_ComponentCode; r.Cells.Add(cell6);
                TableCell cell7 = new TableCell(); cell7.Text = ec1.m_TimetableDate.ToShortDateString(); r.Cells.Add(cell7);
                TableCell cell8 = new TableCell(); cell8.Text = ec1.m_Time.ToString(); r.Cells.Add(cell8);
                TableCell cell9 = new TableCell(); cell9.Text = ec1.m_TimetableSession; r.Cells.Add(cell9);
                }
            }
            if (correct)
            {
                Table1.Visible = true;
                Label1.Text = "Data Correctly Parsed.....";
                Button_Upload.Visible = true;
                ViewState["syllabuses"] = syllabuses;
                ViewState["components"] = components;
                ViewState["options"] = options;
                ViewState["Links"] = Links;

            }
            else{ Label1.Text = errorstring; Label1.BackColor = System.Drawing.Color.Red; }



        }
        protected void Button_Start_Click(object sender, EventArgs e)
        {
            Starting_information.Visible = false;
            Button_Start.Visible = false;
            Button1.Visible = true;
            Button1.Text = "Parse";
            Label1.Text = "Paste your data into the text box and the try to parse it";
            Label1.Visible = true;
        }
        protected void Button_Upload_Click(object sender, EventArgs e)
        {
            //going to create Base data file.....
            string s = "";
            if (!SaveFiles(ref s))
            {
                Label1.Text = s; Label1.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                Label1.Text = "Base Data Saved Successdully" ; Label1.BackColor = System.Drawing.Color.White;
            }
        }

        #region File Routines

        private string GetFilename()
        {
            ExamConversions c = new ExamConversions();
            string s_filename = "";
            string s = YearCode.ToString();
            s_filename = c.GetSeasonCode(SeasonCode) + "a" + s + "_99";
            return s_filename;
        }
        private System.IO.StreamWriter CreateFile(string type, int Record_Length, string s_filename, ref string ErrorS)
        {
            //open file and write header records...
            System.IO.Stream myStream;
            try
            {
                myStream = System.IO.File.Open(Server.MapPath("BaseData") + "\\" + type + s_filename + ".X01", System.IO.FileMode.Create);
                string s = type + "1" + s_filename + s_filename.Substring(0, 4) + "B52205  00112      ";
                System.IO.StreamWriter sw = new System.IO.StreamWriter(myStream);
                while (s.Length < Record_Length - 2) s += " ";
                sw.WriteLine(s);
                s = type + "3" + s_filename + s_filename.Substring(0, 5) + "001          ";
                while (s.Length < Record_Length - 2) s += " ";
                sw.WriteLine(s);
                return sw;
            }
            catch 
            {
                ErrorS = "Failed to create " + s_filename;
            }
            return null;

        }
        private void EndFile(string type, string s_filename, int n_records, DateTime date1, System.IO.StreamWriter fs, int Record_Length)
        {
            string s = ""; string s1 = "";
            s = type + "7" + s_filename.Substring(0, 5);
            s1 = n_records.ToString(); s1 += "0000000" + s1;
            s1 = s1.Substring(s1.Length - 7, 7);
            s += s1;
            s += date1.ToString("ddMMyy");
            while (s.Length < Record_Length - 2) s += " ";
            fs.WriteLine(s);

            //file trailer
            s = type + "9" + s_filename.Substring(0, 5);
            s1 = n_records.ToString(); s1 = s1 + "0000000" + s1;
            s1 = s1.Substring(s1.Length - 7, 7);
            s += s1;
            s += "0000001";
            while (s.Length < Record_Length - 2) s += " ";
            fs.WriteLine(s);
            fs.Close();
        }
        private bool SaveFiles(ref string ErrorS)
        {
            ErrorS = "";
            List<ExamBaseOption> options1 = new List<ExamBaseOption>();
            List<ExamBaseOption> options = new List<ExamBaseOption>();
            options1 = (List<ExamBaseOption>)ViewState["options"];
            List<ExamSyllabus> syllabuses1 = new List<ExamSyllabus>();
            List<ExamSyllabus> syllabuses = new List<ExamSyllabus>();
            syllabuses1 = (List<ExamSyllabus>)ViewState["syllabuses"];
            List<ExamComponent> components = new List<ExamComponent>();
            components = (List<ExamComponent>)ViewState["components"];
            List<ExamLinkComponent> Links = new List<ExamLinkComponent>();
            Links = (List<ExamLinkComponent>)ViewState["Links"];
            string s_filename = GetFilename();
            string s = "";
            int n = 0;

            bool found = false;
            //need to remove duplicates in S and O
            foreach (ExamSyllabus es in syllabuses1)
            {
                found = false;
                foreach(ExamSyllabus es1 in syllabuses)
                {
                    if (es1.m_Syllabus_Code == es.m_Syllabus_Code) found = true;
                }
                if (!found)
                    syllabuses.Add(es);
            }

            foreach (ExamBaseOption eo in options1)
            {
                found = false;
                foreach(ExamBaseOption eo1 in options)
                {
                    if (eo1.m_OptionEntryCode == eo.m_OptionEntryCode) found = true;
                   
                }
                if (!found) options.Add(eo);
            }

            //now save the O file......
            System.IO.StreamWriter sw = CreateFile("O", 42, s_filename, ref ErrorS);
            if (sw == null) return false;

            foreach (ExamBaseOption eo in options)
            {
                s = "O5" + eo.m_OptionEntryCode + eo.m_SyllabusCode + "INT B  U       E000000000000" + eo.m_Title + "N00000        M                10000   ";
                sw.WriteLine(s); n++;
            }
            EndFile("O", s_filename, n, DateTime.Now, sw, 42);


            //now save the S file......
            sw = CreateFile("S", 42, s_filename,ref ErrorS);
            if (sw == null) return false;
            n = 0;
            foreach (ExamSyllabus es in syllabuses)
            {
                s = "S5" + es.m_Syllabus_Code + "_" + es.m_Syllabus_Title;
                sw.WriteLine(s); n++;
            }
            EndFile("S", s_filename, n, DateTime.Now, sw, 42);


            //now save the C file......
            s = "";
            sw = CreateFile("C", 42, s_filename, ref ErrorS);
            if (sw == null) return false;
            n = 0; string s1 = ""; string s2 = "";
            foreach (ExamComponent ec in components)
            {
                s2 = ec.m_TimetableDate.Day.ToString(); if (s2.Length == 1) s2 = "0" + s2;
                s1 = ec.m_TimetableDate.Month.ToString(); if (s1.Length == 1) s1 = "0" + s1;
                s1 = s2 + s1 + ec.m_TimetableDate.Year.ToString().Substring(2, 2);
                s = "C5" + ec.m_ComponentCode + ec.m_ComponentTitle + "N100          T" + s1 + ec.m_TimetableSession.Substring(0, 1) + ec.m_Time;
                sw.WriteLine(s); n++;
            }
            EndFile("C", s_filename, n, DateTime.Now, sw, 42);


            //now the link file

            n = 0;
            sw = CreateFile("L", 42, s_filename, ref ErrorS);
            if (sw == null) return false;
            foreach (ExamLinkComponent l in Links)
            {
                s = "L5" + l.m_OptionCode + l.m_ComponentCode + "      ";
                sw.WriteLine(s); n++;
            }
            EndFile("L", s_filename, n, DateTime.Now, sw, 42);


            return true;
        }


        #endregion
    }
}