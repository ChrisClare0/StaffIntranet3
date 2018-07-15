using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace Cerval_Library
{

    public class ComponentNotScheduled
    {
        public Guid StudentId = new Guid();
        public Guid ComponentId = new Guid();
    }
    public class ExamsUtility
    {
        public bool AddEntry(Guid StudentId, Exam_Board exbde1,int Year, int YearCode,int Season, string option, int EntryStatusCode, bool checkDisallowed, ref string ErrorMessage, ref Guid EntryId)
        {
            //Year is 2 digit code.....
            bool disallowed = false;ErrorMessage = "";
            ExamOption ex01 = new ExamOption();
            ExamOption ex02 = new ExamOption();
            ExamFiles ef1 = new ExamFiles();
            ex01.Load(option, Season.ToString(), YearCode.ToString(), exbde1.m_ExamBoardId);
            if (!ex01.m_valid)
            {
                //need to search basedata...
                ex01 = ef1.Find_Option(option, exbde1, Season.ToString(), YearCode.ToString());
            }
            if (ex01 != null)
            {
                ExamEntries_List exen1 = new ExamEntries_List();
                exen1.Load(StudentId, Year.ToString(), Season.ToString());
                if (checkDisallowed)
                {
                    foreach (Exam_Entry ex1 in exen1.m_list)
                    {
                        if (!ex1.m_withdrawn)
                        {
                            ex02.Load(ex1.m_OptionID);
                            disallowed = ef1.CombinationDisallowed(exbde1, ex01.m_OptionCode, ex02.m_OptionCode, ex01.m_SeriesIdentifier, ex01.m_year_Code);
                            if (disallowed)
                            {
                                ErrorMessage = "Combination of " + ex01.m_OptionCode + " and " + ex02.m_OptionCode + " not allowed!";
                                return false;
                            }
                        }
                    }
                }
                if (!disallowed)
                {
                    Exam_Entry entry1 = new Exam_Entry();
                    entry1.m_OptionID = ex01.m_OptionID;
                    entry1.m_StudentID = StudentId;
                    entry1.m_Date_Created = DateTime.Now;
                    entry1.m_season = Season.ToString();
                    entry1.m_year = Year.ToString();
                    entry1.m_ExamEntryID = Guid.Empty;
                    entry1.m_EntryStatus = EntryStatusCode;

                    foreach (Exam_Entry ex1 in exen1.m_list)
                    {
                        if (ex1.m_OptionID == ex01.m_OptionID)
                        {
                            //found this student, this series, this option
                            entry1.m_ExamEntryID = ex1.m_ExamEntryID;
                        }
                    }
                    entry1.Save();
                    EntryId = entry1.m_ExamEntryID;
                    return true;
                }
            }
            else
            {
                ErrorMessage= "Option code  " + option + " not found"; return false;
            }

            return false;
        }

        public bool AreRoomsMissing(DateTime t1, DateTime t2)
        {
            string s = "SELECT  COUNT(ScheduledComponentID) FROM dbo.tbl_Exams_ScheduledComponents ";
            s += " WHERE  (DateTime > CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))  ";
            s += "AND (DateTime < CONVERT(DATETIME, '" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))  ";
            s += "AND (RoomId IS NULL)  ";
            Encode en = new Encode();
            int n = System.Convert.ToInt32(en.ExecuteScalarSQL(s));
            if (n == 0)return false;
            return true;
        }
        public bool AreDesksMissing(DateTime t1, DateTime t2)
        {
            string s = "SELECT  COUNT(ScheduledComponentID) FROM dbo.tbl_Exams_ScheduledComponents ";
            s += " WHERE  (DateTime > CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))  ";
            s += "AND (DateTime < CONVERT(DATETIME, '" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))  ";
            s += "AND (Desk IS NULL)  ";
            Encode en = new Encode();
            int n = System.Convert.ToInt32(en.ExecuteScalarSQL(s));
            if (n == 0) return false;
            return true;
        }
        public bool AreClashes(DateTime t1, DateTime t2)
        {
            DateTime t0 = new DateTime(); t0 = t1;//remember start time
            ScheduledComponentList scl1 = new ScheduledComponentList();
            scl1.LoadList_orderbyStudentDateASC(t1, t2);//order by StudentId , DateTime, TimeAllowed DESC, ComponentCode ";

            Guid st1 = new Guid(); st1 = Guid.Empty;
            foreach (ScheduledComponent sc in scl1.m_List)
            {
                if (sc.m_StudentId != st1) { st1 = sc.m_StudentId; t1 = t0; }
                if (sc.m_Date < t1) return true;
                t1 = sc.m_Date.AddMinutes(sc.m_TimeAllowed);//new end time
            }
            return false;
        }
        public void ClearDeskAssignments(DateTime t1, DateTime t2)
        {
            Encode en = new Encode();
            string s = " UPDATE dbo.tbl_Exams_ScheduledComponents SET Desk = '' ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            int no = en.Execute_count_SQL(s);
            return;
        }
        public void ClearRoomAssignments(DateTime t1, DateTime t2)
        {
            Encode en = new Encode();
            string s = " UPDATE dbo.tbl_Exams_ScheduledComponents SET RoomId = NULL";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            en.ExecuteSQL(s);
        }


        public List<ComponentNotScheduled> LoadComponentsNotSchedlued(string year, string season, DateTime start, DateTime end)
        {
            //note year i 4 digits... 2017 etc
            string yearcode = year.Substring(2);
            string s1 = " CONVERT(DATETIME, '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s2 = " CONVERT(DATETIME, '" + end.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT DISTINCT TOP(100) PERCENT dbo.tbl_Exams_Entries.StudentID, dbo.tbl_Exams_Components.ComponentID  ";
            s += " FROM            dbo.tbl_Exams_Components INNER JOIN ";
            s += " dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID INNER JOIN ";
            s += " dbo.tbl_Exams_Entries ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Entries.OptionID ";
            s += " WHERE(dbo.tbl_Exams_Components.YearCode = '"+yearcode+"') AND(dbo.tbl_Exams_Components.SeasonCode = '"+season+"') ";
            s += " AND (dbo.tbl_Exams_Components.Timetabled = 'T') ";
            s += " AND (TimetabledDate >" + s1 + " ) AND (TimetabledDate <" + s2 + "  ) ";
            s += "AND(dbo.tbl_Exams_Entries.Withdrawn <> 1)"; 


            s += " EXCEPT SELECT  StudentId, ComponentId FROM  dbo.tbl_Exams_ScheduledComponents ";
            s += " WHERE(Year = '" + year + "') AND(Season = '" + season + "') ";

            Encode en = new Encode();
            List<ComponentNotScheduled> list1 = new List<ComponentNotScheduled>();
            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ComponentNotScheduled c = new ComponentNotScheduled();
                            c.StudentId = dr.GetGuid(0);
                            c.ComponentId = dr.GetGuid(1);
                            list1.Add(c);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return list1;
        }
        public List<ComponentNotScheduled> LoadComponentsNotSchedlued(string year, string season)
        {
            //note year i 4 digits... 2017 etc
            string yearcode = year.Substring(2);
            string s = " SELECT DISTINCT  dbo.tbl_Exams_Entries.StudentID, dbo.tbl_Exams_Components.ComponentID  ";
            s += " FROM dbo.tbl_Exams_Components INNER JOIN ";
            s += " dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID INNER JOIN ";
            s += " dbo.tbl_Exams_Entries ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Entries.OptionID ";
            s += " WHERE(dbo.tbl_Exams_Components.YearCode = '" + yearcode + "') AND(dbo.tbl_Exams_Components.SeasonCode = '" + season + "') ";
            s += " AND (dbo.tbl_Exams_Components.Timetabled = 'T') ";
            s += " EXCEPT SELECT  StudentId, ComponentId FROM  dbo.tbl_Exams_ScheduledComponents ";
            s += " WHERE(Year = '" + year + "') AND(Season = '" + season + "') ";
            s += "AND(dbo.tbl_Exams_Entries.Withdrawn <> 1)";

            Encode en = new Encode();
            List<ComponentNotScheduled> list1 = new List<ComponentNotScheduled>();
            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ComponentNotScheduled c = new ComponentNotScheduled();
                            c.StudentId = dr.GetGuid(0);
                            c.ComponentId = dr.GetGuid(1);
                            list1.Add(c);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return list1;
        }

    }
    public class ExamFiles
    {
        //public string path = @"D:\Users\chris\github\DCGS-Admin-Code\DCGS-StaffIntranet2\DCGS-StaffIntranet2\Exams\BaseData\";
        //public string path = @"D:\admin-challoners\staff\App_Data\BaseData\";
        public string path = @"c:\_TEMP_\BaseData\";
        //public string path = @"D:\admin-challoners\STAFF\Exams\BaseData\";
        //public string path = @"D:\beta-challoners\Admin-test\Exams\BaseData\";
        //string path = @"\\kamenev.challoners.net\SIMS-Admin\SIMS\EXAMIN\";
        //TODO sort where these files are... archive??
        public string ConvertExamBoardNameToCode(string ebname, string level)
        {
            //level only required for EDEXCEL
            string code = "";
            switch (ebname.ToUpper())
            {
                case "OCR": code = "01"; break;
                case "AQA": code = "70"; break;
                case "WJEC": code = "40"; break;
                case "EDEXCEL GCSE": code = "10"; break;
                case "EDEXCEL GCE": code = "11"; break;
                case "EDEXCEL":
                    switch (level)
                    {
                        case "GCSE": code = "10"; break;
                        case "GCE": code = "11"; break;
                        default: break;
                    }
                    break;
                default: code = ""; break;
            }
            return code;
        }
        public bool CombinationDisallowed(Exam_Board eb, string option1, string option2, string series, string year)
        {
            string search = "D" + series + year + "_" + eb.m_LegacyExamBdId + ".X??";
            string[] filelist = System.IO.Directory.GetFiles(path, search);
            string line = "";
            string code1 = "";
            string code2 = "";
            bool disallowed = false;
            foreach (string f in filelist)
            {
                Stream myStream;
                if ((myStream = File.Open(f, FileMode.Open)) != null)
                {
                    using (StreamReader sr = new StreamReader(myStream))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Substring(0, 2) == "D5")
                            {
                                code1 = line.Substring(2, 6);
                                //may have * in it......
                                if (code1.IndexOf("*") > 0)
                                {
                                    code1 = code1.Substring(0, code1.IndexOf("*"));
                                }
                                code2 = line.Substring(8, 6);
                                //may have * in it......
                                if (code2.IndexOf("*") > 0)
                                {
                                    code2 = code2.Substring(0, code2.IndexOf("*"));
                                }
                                if (code1 == option1.Substring(0, code1.Length))
                                {
                                    //first code matches..........
                                    if (code2 == option2.Substring(0, code2.Length))
                                    {
                                        disallowed = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return disallowed;
        }
        public void Write_to_file(string s, string filename)
        {
            StreamWriter ts = new StreamWriter(filename, true);
            ts.WriteLine(s);
            ts.Close();
        }
        public System.Collections.Generic.List<ExamComponent> ExamsComponentsFromBaseData(Exam_Board eb, string season, string year)
        {
            System.Collections.Generic.List<ExamComponent> temp = new List<ExamComponent>();
            while (year.Length > 2) year = year.Substring(1, year.Length - 1);
            string search = "C" + season + "?" + year + "_" + eb.m_LegacyExamBdId + ".X??";
            string[] filelist = System.IO.Directory.GetFiles(path, search);
            foreach (string f in filelist)
            {
                Stream myStream;
                string line;
                int JCQ_Version = 0;
                if ((myStream = File.Open(f, FileMode.Open)) != null)
                {
                    using (StreamReader sr = new StreamReader(myStream))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Substring(0, 2) == "C1") JCQ_Version = System.Convert.ToInt32(line.Substring(24, 2));
                            if (line.Substring(0, 2) == "C5")
                            {
                                ExamComponent eco1 = new ExamComponent();
                                eco1.LoadFromBaseData(line, JCQ_Version, eb.m_ExamBoardId.ToString());
                                temp.Add(eco1);
                            }
                        }
                    }
                }
            }
            temp.Sort();

            return temp;
        }

        public System.Collections.ArrayList ExamsOptionsFromBaseData(Exam_Board eb, string season, string year)
        {
            System.Collections.ArrayList temp = new System.Collections.ArrayList();
            while (year.Length > 2) year = year.Substring(1, year.Length - 1);
            string search = "O" + season + "?" + year + "_" + eb.m_LegacyExamBdId + ".X??";
            string[] filelist = System.IO.Directory.GetFiles(path, search);

            foreach (string f in filelist)
            {
                Stream myStream;
                string line;
                int JCQ_Version = 0;
                if ((myStream = File.Open(f, FileMode.Open)) != null)
                {
                    using (StreamReader sr = new StreamReader(myStream))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Substring(0, 2) == "O1") JCQ_Version = System.Convert.ToInt32(line.Substring(24, 2));
                            if (line.Substring(0, 2) == "O5")
                            {
                                ExamBaseOption ebo1 = new ExamBaseOption();
                                if (ebo1.Load(line, JCQ_Version))
                                {
                                    ebo1.m_file_path = f.Substring(path.Length);
                                    temp.Add(ebo1);
                                }
                            }
                        }
                    }
                }
            }
            temp.Sort();
            return temp;
        }

        public List<ExamSyllabus> ExamSyllabusFromBaseData(Exam_Board eb, string season, string year)
        {
            System.Collections.Generic.List<ExamSyllabus> temp = new List<ExamSyllabus>();
            while (year.Length > 2) year = year.Substring(1, year.Length - 1);
            string search = "S" + season + "?" + year + "_" + eb.m_LegacyExamBdId + ".X??";
            string[] filelist = System.IO.Directory.GetFiles(path, search);
            foreach (string f in filelist)
            {
                Stream myStream;
                string line;
                int JCQ_Version = 0;
                if ((myStream = File.Open(f, FileMode.Open)) != null)
                {
                    using (StreamReader sr = new StreamReader(myStream))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Substring(0, 2) == "S1") JCQ_Version = System.Convert.ToInt32(line.Substring(24, 2));
                            if (line.Substring(0, 2) == "S5")
                            {
                                ExamSyllabus es1 = new ExamSyllabus();
                                es1.LoadFromBaseData(line, JCQ_Version);
                                temp.Add(es1);
                            }
                        }
                    }
                }
            }
            temp.Sort();

            return temp;
        }

        public List<ExamLinkComponent> ExamsLinkComponentsFromBaseData(Exam_Board eb, string season, string year)

        {
            List<ExamLinkComponent> temp = new List<ExamLinkComponent>();
            while (year.Length > 2) year = year.Substring(1, year.Length - 1);
            string search = "L" + season + "?" + year + "_" + eb.m_LegacyExamBdId + ".X??";
            string[] filelist = System.IO.Directory.GetFiles(path, search);
            foreach (string f in filelist)
            {
                Stream myStream;
                string line;
                int JCQ_Version = 0;
                if ((myStream = File.Open(f, FileMode.Open)) != null)
                {
                    using (StreamReader sr = new StreamReader(myStream))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Substring(0, 2) == "L1") JCQ_Version = System.Convert.ToInt32(line.Substring(24, 2));
                            if (line.Substring(0, 2) == "L5")
                            {
                                ExamLinkComponent eco1 = new ExamLinkComponent();
                                eco1.LoadFromBaseData(line, JCQ_Version, eb.m_ExamBoardId.ToString());
                                temp.Add(eco1);
                            }
                        }
                    }
                }
            }
            //temp.Sort();

            return temp;
        }

        public ExamOption Find_Option(string code, Exam_Board eb, string season, string year)
        {
            //find the entry in db... or search basedata files
            code = code + "     ";
            if (code.Length > 6) code = code.Substring(0, 6);
            ExamOption exo1 = new ExamOption();
            exo1.Load(code, season, year, eb.m_ExamBoardId);
            if (exo1.m_valid) return exo1;

            string search = "O" + season + "?" + year + "_" + eb.m_LegacyExamBdId + ".X??";
            string[] filelist = System.IO.Directory.GetFiles(path, search);

            foreach (string s1 in filelist)
            {
                int n = s1.ToUpper().IndexOf("O");
                n = s1.ToUpper().LastIndexOf("O");
                string s = s1.Substring(n);
                string series = s.Substring(1, 2);
                exo1 = Find_OptionEntry1(s, code, eb, season, series, year);
                if (exo1 != null) return exo1;
            }
            return null;
        }

        private ExamOption Find_OptionEntry1(string f, string code, Exam_Board eb, string season, string series, string year)
        {

            ExamOption exo1 = new ExamOption();
            if (exo1.Load(code, season, year, eb.m_ExamBoardId)) return exo1;
            Stream myStream;
            string s = "";
            string s1 = "";
            string s2 = "";
            string s_file = "S" + f.Substring(1, f.Length - 1);
            string line;
            string Qualification;
            int JCQ_Version = 0;
            if ((myStream = File.Open(path + f, FileMode.Open)) != null)
            {
                using (StreamReader sr = new StreamReader(myStream))
                {
                    //ExamFiles exfiles1 = new ExamFiles();
                    while ((line = sr.ReadLine()) != null)
                    {
                        s = line.Substring(0, 2);
                        Qualification = line.Substring(14, 4);
                        s1 = line.Substring(2, 6);
                        if (code.Length > 6) code = code.Substring(0, 6);
                        s2 = line.Substring(8, 6);//syllabus code..
                        if (s == "O1") JCQ_Version = Convert.ToInt32(line.Substring(24, 2));
                        if ((s1.Trim().ToUpper() == code.Trim().ToUpper()) && (s == "O5"))
                        {
                            Load_Option(line, eb, season, series, year, s_file, path, JCQ_Version);
                            if (exo1.Load(s1, season, year, eb.m_ExamBoardId)) return exo1;
                        }
                    }
                }
            }
            return null;//not found
        }

        public void Load_Option(string line, Exam_Board eb, string season, string series, string year, string syl_file, string path, int JCQ_Version)
        {
            //to load from line (from basedata file) to make a new option
            string component_file = "c" + syl_file.Substring(1, syl_file.Length - 1);
            string link_file = "l" + syl_file.Substring(1, syl_file.Length - 1);

            ExamBaseOption ebo1 = new ExamBaseOption();

            ExamSyllabus syl = new ExamSyllabus();
            string s = line.Substring(0, 2);
            if (s == "O5")
            {
                if (!ebo1.Load(line, JCQ_Version)) return;

                syl = Find_SyllabusID(ebo1.m_SyllabusCode, year, season, eb, path + syl_file); // will find or make
                if (syl == null) return;

                s = "INSERT INTO dbo.tbl_Exams_Options  (SyllabusID ,OptionCode , OptionTitle, ";
                s += "OptionQualification, OptionLevel, OptionItem, OptionProcess, ";
                s += "QCAClassificationCode, QCAAccreditationNumber, OptionFee, OptionMaximumMark, YearCode, SeasonCode,  SeriesIdentifier, Version )";
                s += "VALUES  ('" + syl.m_SyllabusId.ToString() + "' , '";
                s += ebo1.m_OptionEntryCode + "' , '" + ebo1.m_Title + "' , '" + ebo1.m_Qualification + "' , '";
                s += ebo1.m_Level + "' , '" + ebo1.m_Item + "' , '" + ebo1.m_Process + "' , '";
                s += ebo1.m_QCACode + "' , '" + ebo1.m_QCANumber + "' , '";
                if (ebo1.m_FeeValid) s += ebo1.m_Fee.ToString(); else s += "-";
                s += "' , '" + ebo1.m_MaximiumMark + "' ,'";
                s += year + "' , '" + season + "' , '" + series + "', '2' ) ";
                ExecuteSQL(s);
                Install_Components(ebo1.m_OptionEntryCode, path + link_file, path + component_file, eb, season, year);
            }
        }

        private ExamSyllabus Find_SyllabusID(string syllabus_code, string year, string season, Exam_Board eb, string s_file)
        {
            bool found = false;
            string s;
            Stream myStream;
            ExamSyllabus syl = new ExamSyllabus();
            //syl.Load(syllabus_code, season, year, eb.m_ExamBoardId);
            //going to ignore the season and year for these records...????
            syl.Load(syllabus_code, season, year, eb.m_ExamBoardId);//find any with this code & board.. ignore season and year
            if (syl.m_valid) return syl;
            //not in cerval... need to make it...
            string line = "";
            try
            {
                if ((myStream = File.Open(s_file, FileMode.Open)) != null)
                {
                    using (StreamReader sr = new StreamReader(myStream))
                    {
                        while (((line = sr.ReadLine()) != null) && (!found))
                        {
                            syl.LoadFromBaseData(line, 11);
                            if (syl.m_Syllabus_Code == syllabus_code)
                            {
                                s = "INSERT INTO dbo.tbl_Exams_Syllabus ";
                                s += "(ExamBoardID, SyllabusCode, SyllabusTitle ,Version)";
                                s += "VALUES ( '" + eb.m_ExamBoardId.ToString() + "' , '";
                                s += syl.m_Syllabus_Code + "' , '" + syl.m_Syllabus_Title + "' ,'1' )";
                                ExecuteSQL(s);
                                found = true;
                                //scary - call ourselves to get Guid  with no sifle to stop infinite recursion!!!!
                                return Find_SyllabusID(syllabus_code, year, season, eb, "");
                            }
                        }//end of while more lines
                    }
                }
            }
            catch (Exception e1)
            {
                System.Windows.Forms.MessageBox.Show("Error writing the syllabus file... " + e1.Message, "File Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
            //not found!!
            //MessageBox.Show("Error finding the syllabus id.. ", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return null;
        }

        public void Install_Components(string OptionCode, string l_file, string c_file, Exam_Board ExamBoard, string season, string year)
        {
            string s, line;
            Stream myStream;
            string LinkOption = "";
            string LinkComponent = "";
            ExamOption opt1 = new ExamOption();
            Guid opt1ID = new Guid();
            Guid com1ID = new Guid();
            opt1.Load(OptionCode, season, year, ExamBoard.m_ExamBoardId);
            opt1ID = opt1.m_OptionID;

            //TODO    ADD EXCEPTION HANDLING... IF LINE <12

            if ((myStream = File.Open(l_file, FileMode.Open)) != null)
            {
                using (StreamReader sr = new StreamReader(myStream))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        s = line.Substring(0, 2);
                        LinkOption = line.Substring(2, 6);
                        if ((s == "L5") && (LinkOption == OptionCode))
                        {
                            LinkComponent = line.Substring(8, 12);
                            com1ID = Find_Component(c_file, LinkComponent, ExamBoard.m_ExamBoardId.ToString(), season, year,true);//forceupdate = true means it will search basedata to check OK
                            //now check the link file
                            ExamLinkComponent_List elcl1 = new ExamLinkComponent_List();elcl1.LoadList_Option(opt1.m_OptionID);
                            bool found = false;
                            foreach(ExamLinkComponent ec1 in elcl1.m_list)
                            {
                                if (ec1.m_ComponentId == com1ID) found = true;
                            }
                            if (!found)
                            {
                                s = " INSERT INTO dbo.tbl_Exams_Link  ( OptionID, ComponentID ) ";
                                s += " VALUES ('" + opt1ID.ToString() + "' , '" + com1ID.ToString() + "' )";
                                if ((opt1.m_valid) && (com1ID != Guid.Empty))
                                {
                                    ExecuteSQL(s);
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

        public Guid Find_Component(string c_file, string component, string ExamBoard, string season, string year, bool forceupdate)
        {
            ExamComponent com1 = new ExamComponent();
            Guid com1ID = new Guid(); com1ID = Guid.Empty;
            Stream myStream;
            string line = "";
            int JCQ_Version = 0;

            //try to find in db
            com1ID = com1.Find_ComponentID(component, ExamBoard, season, year);
            if ((com1ID != Guid.Empty) && !forceupdate) return com1ID;
            //so not in db... find in c_file and make...
            try
            {
                if ((myStream = File.Open(c_file, FileMode.Open)) != null)
                {
                    using (StreamReader sr = new StreamReader(myStream))
                    {
                        ExamComponent examcom1 = new ExamComponent();
                        ExamComponent examcom2 = new ExamComponent();//used for forced update
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Substring(0, 2) == "C1") JCQ_Version = System.Convert.ToInt32(line.Substring(24, 2));
                            if ((line.Substring(0, 2) == "C5") && (line.Substring(2, 12) == component))
                            {
                                examcom1.m_ComponentCode = component;
                                examcom1.LoadFromBaseData(line, JCQ_Version, ExamBoard);
                                examcom1.m_season = season; examcom1.m_year = year;
                                if (com1ID != Guid.Empty)
                                {
                                    // we are doing a forced update..... and already know the ID....
                                    examcom2.Load(com1ID);//has current db version.....
                                    //now if any significant changes need to delete current scheduled components...
                                    examcom1.m_ComponentID = com1ID;
                                    if (!examcom1.EqualTo(examcom2))
                                    {
                                        ScheduledComponentList scl1 = new ScheduledComponentList();
                                        scl1.LoadList(com1ID);
                                        foreach (ScheduledComponent sc1 in scl1.m_List)
                                        {
                                            sc1.Delete();
                                        }
                                    }
                                    examcom1.Save();
                                }
                                else
                                {
                                    examcom1.Create();
                                }
                                return Find_Component("", component, ExamBoard, season, year, false);
                            }
                        }
                    }
                }
            }
            catch (Exception e1)
            {
                System.Windows.Forms.MessageBox.Show("Error writing the component file... " + e1.Message, "File Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
            //not found - this could be a poblem
            System.Windows.Forms.MessageBox.Show("Warning.... Can't find a component: " + component, "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            return com1ID;
        }

        private void ExecuteSQL(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    cm.ExecuteNonQuery();
                }
                cn.Close();
            }
        }

        public string Calculate_UCI_Checksum(string CentreNumber, string board_code, string allocation_year, string ExamNumber)
        {
            while (allocation_year.Length > 2)//need last 2 digits
            {
                allocation_year = allocation_year.Substring(1, allocation_year.Length - 1);
            }
            board_code = board_code.Substring(board_code.Length - 1, 1);// one digit
            string s = "0000" + ExamNumber; s = s.Substring(s.Length - 4, 4); // need 4 digits...
            s = CentreNumber + board_code + allocation_year + s;

            //ok checksum.....
            int sum = 0; int digit = 0;
            for (int k = 0; k < 12; k++)
            {
                string s1 = s.Substring(k, 1);
                try
                {
                    digit = System.Convert.ToInt32(s1);
                }
                catch
                {
                    //assume not a digit...
                    char c = s1[0];
                    if (c >= 'A' && c <= 'P') { digit = (int)(c - 'A' + 1); }
                    else
                    {
                        digit = c - 71;//so Q is 81-71 = 10  etc
                        while (digit > 16) digit = digit - 7;/// so X = 88-71 = 17 .. -1 =10
                    }
                }

                sum += digit * (16 - k);
            }
            digit = sum % 17;
            if (digit < 8) s += (char)(digit + 65);
            if ((digit > 7) && (digit < 11)) s += (char)(digit + 65 + 2);//omits ij
            if (digit == 11) s += "R";
            if (digit == 12) s += "T";
            if (digit == 13) s += "V";
            if (digit == 14) s += "W";
            if (digit == 15) s += "X";
            if (digit == 16) s += "Y";
            return s;
        }
    }
    public class ExamConversions
    {
        public string GetSeasonString(int season)
        {
            if (season == 6) return "June";
            if (season == 1) return "January";
            if (season == 11) return "November";
            if (season == 5) return "May";
            if (season == 3) return "March";
            return "unknown";
        }
        public string GetSeasonCode(int season)
        {
            if (season < 10) return season.ToString();
            if (season == 10) return "A";
            if (season == 11) return "B";
            if (season == 12) return "C";
            return "?";
        }
        public Exam_Board GetExamBoard(string board)
        {
            string exam_bde = "";
            int i = -1;
            try
            {
                i = System.Convert.ToInt32(board);
                exam_bde = board;
            }
            catch 
            {
            }
            if (i < 0)
            {
                switch (board.ToUpper())
                {
                    case "OCR": exam_bde = "01"; break;
                    case "CIE": exam_bde = "02"; break;
                    case "EDEXCEL GCSE": exam_bde = "10"; break;
                    case "EDEXCEL GCE": exam_bde = "11"; break;
                    case "Edexcel BTEC": exam_bde = "15"; break;
                    case "WJEC GCSE": exam_bde = "40"; break;
                    case "WJEC GCE": exam_bde = "41"; break;
                    case "CCEA": exam_bde = "61"; break;
                    case "AQA": exam_bde = "70"; break;
                    case "STEP": exam_bde = "98"; break;
                    case "DCGS": exam_bde = "99"; break;

                    default: break;
                }
            }
            Exam_Board exbde1 = new Exam_Board(exam_bde);
            if (exbde1.m_valid) return exbde1;
            return null;
        }
    }
    public class ExamOutlineTTEntry
    {
        public DateTime date;
        public Guid ComponentId = new Guid();
        public string OptionCode;
        public string OptionTitle;
        public string ComponentTitle;

        public ExamOutlineTTEntry() { }

        public void Hydrate(SqlDataReader dr)
        {
            date = dr.GetDateTime(0);
            ComponentId = dr.GetGuid(3);
            OptionCode = dr.GetString(4);
            OptionTitle= dr.GetString(5);
            ComponentTitle= dr.GetString(6);
        }

    }
    public class ExamTimetable
    {
        public bool UpdateTimetable(string Year,int Seasoncode, ref string ErrorS)
        {
            //true if succeeds
            // add scheduled components for any components not scheduled...
            //TODO also delete any withdrawn....
            ExamConversions u = new ExamConversions();
            string Season = u.GetSeasonString(Seasoncode);
            ExamCompononent_List ecl1 = new ExamCompononent_List();
            ecl1.LoadAllComponents_NotScheduled(Year,Season);
            if (ecl1.m_count == 0)
            {
                ErrorS = "All Entries already scheduled...";return true;
            }
            ecl1.LoadAllComponents(Year, Season);

            Encode en = new Encode();
            string s = "";

            ScheduledComponent sched1 = new ScheduledComponent();
            s = ""; int n = 0;
            DateTime d1 = new DateTime();



            foreach (ExamComponent ec in ecl1.m_list)
            {
                ExamLinkComponent_List elcl1 = new ExamLinkComponent_List();
                elcl1.LoadList_Component(ec.m_ComponentID);
                foreach (ExamLinkComponent elc in elcl1.m_list)
                {
                    ExamEntries_List exl1 = new ExamEntries_List();
                    //now need all entries for this option.....
                    exl1.Load_Option(elc.m_OptionId);
                    n += exl1.m_list.Count;
                }
            }

            foreach (ExamComponent ec in ecl1.m_list)
            {
                if (ec.m_Timetabled == "T")
                {
                    d1 = ec.m_TimetableDate;
                    if (ec.m_TimetableSession == "A")
                    {
                        d1 = d1.AddHours(8); d1 = d1.AddMinutes(50);
                    }
                    else
                    {
                        d1 = d1.AddHours(13); d1 = d1.AddMinutes(40);
                    }
                    //need to find all entries that use this component.....
                    ExamLinkComponent_List elcl1 = new ExamLinkComponent_List();
                    elcl1.LoadList_Component(ec.m_ComponentID);
                    foreach (ExamLinkComponent elc in elcl1.m_list)
                    {
                        ExamEntries_List exl1 = new ExamEntries_List();
                        //now need all entries for this option.....
                        exl1.Load_Option(elc.m_OptionId);
                        foreach (Exam_Entry ex in exl1.m_list)
                        {
                            if (!ex.m_withdrawn)
                            {

                                sched1.Load(ec.m_ComponentID, ex.m_StudentID);
                                if (!sched1.m_valid)
                                {
                                    sched1.m_StudentId = ex.m_StudentID;
                                    sched1.m_ComponentId = ec.m_ComponentID;
                                    sched1.m_RoomId = Guid.Empty;
                                    sched1.m_Year = Year;
                                    sched1.m_Season =Season;
                                    sched1.m_valid = true;
                                    sched1.m_Date = d1;
                                    sched1.m_Desk = "";
                                    sched1.m_Will_Type = false;// do these later...
                                    sched1.Save();
                                }
                            }
                            else
                            {
                                sched1.Load(ec.m_ComponentID, ex.m_StudentID);
                                if (sched1.m_valid)
                                {
                                    //need to delete
                                    sched1.Delete();
                                }
                            }
                        }
                    }
                }
            }

            //now ought to update the willtype fields to agree with the cantype entry..
            //oohhh this is going to be messy.....
            //read from a querry..... then update Exams_Scheduled_components...
            CanTypeList typists = new CanTypeList();
            foreach (Guid g in typists.m_List)
            {
                s = "UPDATE dbo.tbl_Exams_ScheduledComponents SET WillType =1 WHERE (StudentId = '" + g.ToString() + "'  )AND (Year='" + Year + "' ) AND (Season ='" + Season + "' )";
                en.ExecuteSQL(s);
            }
            return true;

        }
        public bool ClearTimetable(string Year, int Seasoncode, ref string ErrorS, ref int number_cleared)
        {
            ExamConversions u = new ExamConversions();
            Encode en = new Encode();
            try
            {
            string s = "DELETE  FROM tbl_Exams_ScheduledComponents ";
            s += " WHERE (Year='" + Year + "' ) ";
            s += "  AND (Season ='" + u.GetSeasonCode(Seasoncode) + "' )";
            number_cleared = en.Execute_count_SQL(s);
            }
            catch (Exception e)
            {
                ErrorS = e.ToString();return false;
            }
            ErrorS = "finished deleting";
            return true;
        }
        public bool GetOUtlineTT(string Year, int SeasonCode, ref string ErrorS)
        {
            List<ExamOutlineTTEntry> m_list = new List<ExamOutlineTTEntry>();
            ExamConversions u = new ExamConversions();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();//let sql do the heavy lifting...
            string s = " SELECT DISTINCT dbo.tbl_Exams_ScheduledComponents.DateTime, dbo.tbl_Exams_ScheduledComponents.Year, ";
            s += " dbo.tbl_Exams_ScheduledComponents.Season, dbo.tbl_Exams_ScheduledComponents.ComponentId, dbo.tbl_Exams_Options.OptionCode, ";
            s += "dbo.tbl_Exams_Options.OptionTitle, dbo.tbl_Exams_Components.ComponentTitle ";
            s += " FROM dbo.tbl_Exams_ScheduledComponents INNER JOIN ";
            s += " dbo.tbl_Exams_Components ON dbo.tbl_Exams_ScheduledComponents.ComponentId = dbo.tbl_Exams_Components.ComponentID INNER JOIN ";
            s += " dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID INNER JOIN  ";
            s += " dbo.tbl_Exams_Options ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Options.OptionID ";
            s += "  WHERE(dbo.tbl_Exams_ScheduledComponents.Year = '" + Year + "') ";
            s += "  AND(dbo.tbl_Exams_ScheduledComponents.Season = '" + u.GetSeasonCode(SeasonCode) + "')  ";
            s += "  ORDER BY dbo.tbl_Exams_ScheduledComponents.DateTime";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamOutlineTTEntry p1 = new ExamOutlineTTEntry();
                            p1.Hydrate(dr);
                            m_list.Add(p1);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return true;
        }
    }

}
