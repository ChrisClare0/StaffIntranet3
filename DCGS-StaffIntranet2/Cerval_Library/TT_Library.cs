using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Security.AccessControl;
using System.Security.Principal;



namespace Cerval_Library
{
    #region routines to deal with the TT data in raw format...
    //really windows based stuff
    public class TTDataUtility
    {
        public Guid newdawnCse = Cerval_Globals.newdawnCse;
        public Guid DCGSroot = Cerval_Globals.DCGSroot;
        public Guid newdawnLinearCse = Cerval_Globals.newdawnLinearCse;
        public void DeleteAllTimetable(DateTime start,bool DeleteGroupMemberships,out int no_gps_deleted,bool DeleteSchedule,out int no_sch_deleted)
        {
            string s = "DELETE FROM  tbl_Core_ScheduledPeriodValidity  WHERE (ValidityEnd > CONVERT(DATETIME, '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))";
            Encode en = new Encode();
            int ires = 0;
            if (DeleteSchedule)
            {
                ires = en.Execute_count_SQL(s);          
            }
            no_sch_deleted = ires; ires = 0;
            s = "DELETE FROM  tbl_Core_Student_Groups  WHERE (MemberUntil > CONVERT(DATETIME, '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102))";
            if (DeleteGroupMemberships)
            {
                ires = en.Execute_count_SQL(s);
          
            }
            no_gps_deleted = ires;


        }
    }

    public class TTData
    {
        #region variables

        public int[] Column_day = new int[200];// index is the column (x) on display and contents is day no (0=mMon)
        public Guid[] Column_period = new Guid[200];//index is the column (x) on display and contents is GUID of cerval period
        public string[] PeriodCodes = new string[200];//index is the column (x) on display and contents is periodcode (AM... 0..1..
        public int max_columns = 0;

        public Guid[] RoomList1 = new Guid[255];
        public string[] RoomCodes = new string[255];
        Guid[] StaffList1 = new Guid[255];
        public string[] StaffCodes = new string[255];

        RoomList rl1 = new RoomList();
        StaffList sl1 = new StaffList();
        GroupList groups1;
        public TT_PeriodList periodlist1 = new TT_PeriodList();
        public string Warnings = "";
        public DateTime time1;
        public DateTime time2;
        public int room_count;
        public int staff_count;
        public FileStream fs;
        private Encode en = new Encode();

        #endregion

        public class TTdataEventArgs : EventArgs
        {
            public readonly int count;
            public readonly string msg;
            public readonly int max;
            public TTdataEventArgs(int Count, string message,int maximum)
            {
                count = Count; msg = message; max = maximum;
            }
        }
        public TTData()
        {
        }

        public TTData(string filename, DateTime starttime, DateTime endtime, bool Create_Groups, bool SetPrimaryRegistration, string PrimaryRegistrationKey)
        {
            Load(filename, starttime, endtime, Create_Groups, SetPrimaryRegistration, PrimaryRegistrationKey);
        }
 
        public delegate void UpdateBars(object o, TTdataEventArgs e);
        public event UpdateBars Updating;

        public void Load_DB(DateTime time)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string date_s = "CONVERT(DATETIME, '" + time.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT  * FROM dbo.INTRANET_FullTimetable_CC ";
            s += " WHERE (ValidityEnd   >  "+date_s+" ) ";
            s += " AND   (ValidityStart <  "+date_s+" ) ";
            s += " ORDER BY GroupCode ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            TT_period p = new TT_period();
                            p.Hydrate(dr);
                            periodlist1.m_list.Add(p);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            //this loads all the TT_periods....
            //now to load the other stuff... make the room and staff lists..?

            //rooms and staff
            bool found = false;
            foreach (TT_period p in periodlist1.m_list)
            {
                found = false;
                //try to find in our list
                foreach (Guid g in RoomList1) { if (g == p.RoomId) { found = true; break; } }
                if (!found) { RoomList1[room_count] = p.RoomId; RoomCodes[room_count] = p.RoomCode; room_count++; }
                found = false;
                foreach (Guid g in StaffList1) { if (g == p.StaffId) { found = true; break; } }
                if (!found) { StaffList1[staff_count] = p.StaffId; StaffCodes[staff_count] = p.StaffCode; staff_count++; }
            }
            //ought to the 
        }

        public void Load_ISAMS(DateTime time)
        {
            RoomList rl1 = new RoomList();
            rl1.LoadList();
            PeriodList pl1 = new PeriodList();
            StaffList sl1 = new StaffList(); sl1.LoadFullList();
            DateTime starttime = new DateTime(2017, 9, 4, 01, 0, 0);
            groups1 = new GroupList(starttime, GroupList.GroupListOrder.GroupName);
            ISAMS_TimetableScheduledPeriodList TT1 = new ISAMS_TimetableScheduledPeriodList();
            TT1.LoadListCurrentTT();
            bool SetPrimaryRegistration = true;string PrimaryRegistrationKey = "RG";

            foreach (ISAMS_TimetableScheduledPeriod ip in TT1.m_list)
            {
                TT_period p = new TT_period(); periodlist1.m_list.Add(p);
                Group g1 = Find_Group(ip.SetName, starttime, new DateTime(2018, 7, 31, 01, 0, 0), true);
                if (SetPrimaryRegistration)
                {
                    if (g1._GroupName.ToUpper().Contains(PrimaryRegistrationKey.ToUpper()))
                    {
                        if (!g1._GroupPrimaryAdministrative)
                        {
                            g1._GroupPrimaryAdministrative = true;
                            g1.Save();
                        }
                    }
                    else
                    {
                        if (g1._GroupPrimaryAdministrative)
                        {
                            g1._GroupPrimaryAdministrative = false;
                            g1.Save();
                        }

                    }
                }
                p.GroupId = g1._GroupID;p.RoomCode = ip.RoomCode;p.SetName = ip.SetName;
                foreach(SimpleRoom r in rl1.m_roomlist)
                {
                    if (r.m_roomcode.ToUpper().Trim() == ip.RoomCode.ToUpper().Trim()) { p.RoomId = r.m_RoomID;break; }
                    if ("T"+r.m_roomcode.ToUpper().Trim() == ip.RoomCode.ToUpper().Trim()) { p.RoomId = r.m_RoomID; break; }
                }
                p.StaffCode = ip.StaffCode;
                try
                {
                    p.StaffId = new Guid(ip.PreviousMISId);
                }
                catch
                {
                    foreach(SimpleStaff s1 in sl1.m_stafflist)
                    {
                        if (s1.m_StaffCode.ToUpper().Trim() == ip.StaffCode){ p.StaffId = s1.m_StaffId; break; }
                    }
                }
                bool found = false;
                p.DayNo = ip.Day - 1;
                foreach(Period p1 in pl1.m_PeriodList)
                {
                    if (p1.m_periodcode.Trim().ToUpper() == ip.PeriodName.Trim().ToUpper())
                    {
                        p.PeriodId = p1.m_PeriodId;
                        found = true;
                        break;
                    }
                } 
                if(!found)
                {
                    string s2 = "";
                    s2=p.PeriodCode;
                }           
            }



            //Group g1 = Find_Group(is2.SetCode, new DateTime(2017, 9, 6, 01, 0, 0), new DateTime(2018, 7, 31, 01, 0, 0), true);
            //ScheduledPeriodRawList schprl1 = new ScheduledPeriodRawList(); schprl1.Load_for_Group(g1._GroupID);
        }


        public void Load(string filename, DateTime starttime, DateTime endtime, bool Create_Groups,bool SetPrimaryRegistration, string PrimaryRegistrationKey)
        {

           //WindowsIdentity winId = new WindowsIdentity("Test-staff", "user");
            //WindowsImpersonationContext ctx = null;
            //try
            {
                // Start impersonating
                //ctx = winId.Impersonate();
                // Now impersonating
                //filename = @"~/App_Data/tt_data.ttd";

                //filename = @"d:\admin-challoners\tt_data.ttd";
                string username = WindowsIdentity.GetCurrent().Name;
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                groups1 = new GroupList(starttime, GroupList.GroupListOrder.GroupName);
                //going to read the data file...
                time1 = starttime;
                time2 = endtime;
                //\\challoners.net\users\Departments\School-Administration\Daily-Orders\tt_data.ttd
                //filename = @"d:\admin-challoners\tt_data.ttd";

                //fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
                //sl1.LoadList(starttime, false);
                sl1.LoadFullList();
                Warnings = "";
                ReadMRecords(fs);
                ReadRooms(fs);
                ReadStaff(fs);
                ReadSets(fs, Create_Groups, SetPrimaryRegistration, PrimaryRegistrationKey);
                if (Updating != null) Updating(this, new TTdataEventArgs((int)fs.Length / 16, "Done", (int)fs.Length / 16));
                //so we have it/....    
                fs.Close();
            }
            // Prevent exceptions from propagating
            //catch
            {
            }
            //finally
            // Revert impersonation
            //if (ctx != null)
               // ctx.Undo();

        }
        public bool ReadMRecords(FileStream fs)
        {
            DayList dl1 = new DayList();
            PeriodList pl1 = new PeriodList();
            byte[] line = new byte[16]; string s1 = ""; bool found;
            while (fs.Position < fs.Length)
            {
                fs.Read(line, 0, 16);
                if (line[0] == 'M')
                {
                    s1 = ""; for (int j = 1; j < 7; j++)
                    {
                        if (line[j] != 0) s1 += (char)line[j];
                    }

                    if (line[2] == 'O') Column_day[line[10]] = 0;
                    if (line[2] == 'U') Column_day[line[10]] = 1;
                    if (line[2] == 'E') Column_day[line[10]] = 2;
                    if (line[2] == 'H') Column_day[line[10]] = 3;
                    if (line[2] == 'R') Column_day[line[10]] = 4;

                    found = false;
                    string s = s1.Substring(s1.IndexOf("-") + 1); s = s.Trim();
                    foreach (Period p in pl1.m_PeriodList)
                    {
                        if (p.m_periodcode.Trim() == s)
                        {
                            Column_period[line[10]] = p.m_PeriodId; found = true;
                            PeriodCodes[line[10]] = p.m_periodcode.Trim();
                        }
                    }
                    if (!found)
                    {
                        Warnings += "M period not found...." + s1 + Environment.NewLine;
                    }
                    if (line[10] > max_columns) max_columns = line[10];
                    //set up reverse map

                }
            }
            return true;
        }
        public bool ReadRooms(FileStream fs)
        {
            byte[] line = new byte[16]; string s1 = ""; bool found;
            rl1.LoadList(); room_count = 0;
            fs.Seek(0, System.IO.SeekOrigin.Begin);
            while (fs.Position < fs.Length)
            {
                fs.Read(line, 0, 16);
                if ((line[0] == 'R') && (line[1] == '1'))
                {
                    //we have a room... need to find it in the sql data...
                    s1 = "";
                    int room = (int)line[2];
                    int i = 3; while ((i < 11) && (line[i] > 0)) { s1 = s1 + (char)line[i]; i++; }
                    s1 = s1.ToUpper();
                    i = 0; found = false;
                    if (s1 == "HALL") s1 = "HAL";
                    foreach (SimpleRoom r in rl1.m_roomlist)
                    {
                        if (r.m_roomcode.Trim() == s1.Trim())
                        {
                            RoomList1[room] = r.m_RoomID; found = true;
                            RoomCodes[room] = r.m_roomcode.Trim();
                        }
                    }
                    if (!found)
                    {
                        Warnings += "Room not found...." + s1 + Environment.NewLine; ;
                    }
                    room_count++; RoomCodes[room_count] = "";
                }
            }
            return true;
        }
        public bool ReadStaff(FileStream fs)
        {
            byte[] line = new byte[16]; string s1 = ""; bool found = false;
            staff_count = 0;
            fs.Seek(0, System.IO.SeekOrigin.Begin);
            while (fs.Position < fs.Length)
            {
                fs.Read(line, 0, 16);
                if ((line[0] == 'T') && (line[1] == '1'))
                {
                    int staff = (int)line[2]; found = false;
                    s1 = ""; int i = 3; while ((line[i] > 0) && (i < 11)) { s1 += (char)line[i]; i++; }
                    StaffCodes[staff] = s1.Trim().ToUpper(); StaffList1[staff] = Guid.Empty;
                    foreach (SimpleStaff s in sl1.m_stafflist)
                    {
                        if (s1.Trim().ToUpper() == s.m_StaffCode.Trim().ToUpper())
                        {
                            StaffList1[staff] = s.m_StaffId; found = true;
                            StaffCodes[staff] = s.m_StaffCode.Trim();
                        }
                    }
                    if (!found)
                    {
                        Warnings += "Staff " + s1 + "not found in Cerval"+Environment.NewLine; ;
                    }
                    staff_count++; StaffCodes[staff_count] = "";
                }
            }
            return true;
        }
        public bool ReadSets(FileStream fs, bool Create_Groups, bool SetPrimaryRegistration, string PrimaryRegistrationKey)
        {
            int n=0;
            byte[] line = new byte[16]; string s1 = "";
            fs.Seek(0, System.IO.SeekOrigin.Begin);
            while (fs.Position < fs.Length)
            {
                fs.Read(line, 0, 16);
                if ((line[0] == 'S'))
                {
                    s1 = ""; int i = 1;n++;
                    while ((line[i] > 0) && (i < 8)) { s1 += (char)line[i]; i++; }; s1 = s1.Trim();
                    if(Updating!=null)Updating(this,new TTdataEventArgs(n,s1,(int)fs.Length/16));
                    Group g1 = Find_Group(s1, time1, time2, Create_Groups);//SetPrimaryRegistration,PrimaryRegistrationKey);
                    if (g1 == null)
                    {
                        Warnings += "Group not found...." + s1 + Environment.NewLine; ;
                    }
                    else
                    {
                        if (SetPrimaryRegistration)
                        {
                            if (g1._GroupName.ToUpper().Contains(PrimaryRegistrationKey.ToUpper()))
                            {
                                if (!g1._GroupPrimaryAdministrative)
                                {
                                    g1._GroupPrimaryAdministrative = true; 
                                    g1.Save();
                                }
                            }
                            else
                            {
                                if (g1._GroupPrimaryAdministrative)
                                {
                                    g1._GroupPrimaryAdministrative = false; 
                                    g1.Save();
                                }

                            }
                        }
                    }
                    TT_period p = new TT_period();
                    if (g1 != null) p.GroupId = g1._GroupID; else p.GroupId = Guid.Empty;
                    p.RoomId = RoomList1[line[9]];
                    p.StaffId = StaffList1[line[8]];
                    p.PeriodId = Column_period[line[10]];
                    p.DayNo = Column_day[line[10]];
                    p.SetName = s1;
                    p.StaffCode = StaffCodes[line[8]];
                    p.RoomCode = RoomCodes[line[9]];
                    p.column = line[10];
                    p.PeriodCode = PeriodCodes[line[10]];
                    periodlist1.m_list.Add(p);
                    p.Covered_StaffCode = StaffCodes[line[0x0E]]; if (p.Covered_StaffCode == "?") p.Covered_StaffCode = "";
                    p.Covered_StaffId = StaffList1[line[0x0E]];
                    p.Second_StaffCode = StaffCodes[line[0x0F]];
                    p.Second_StaffId = StaffList1[line[0x0F]];

                    //remember the file position and raw data
                    p.File_Position = fs.Position - 16;
                    for (int j = 0; j < 16; j++) p.line[j] = line[j];
                    p.valid = true;
                }
            }
            return true;
        }

        public int FindFreeRooms(int day_no, string Period_Code, ref string[] result)
        {
            bool[] RoomFree = new bool[255];
            for (int i = 0; i < 255; i++) RoomFree[i] = true;
            foreach (TT_period t in periodlist1.m_list)
            {
                if((t.DayNo==day_no)&&(t.PeriodCode==Period_Code))
                {
                    //find it in the room_list
                    for (int i = 0; i < 255; i++)
                    {
                        if (RoomList1[i] == t.RoomId) { RoomFree[i] = false; break; }
                    }
                }
            }
            int j = 0;
            for (int i = 0; i < room_count; i++)
            {
                if (RoomFree[i]) {result[j] = RoomCodes[i]; j++;}
            }
            return j;
        }

        public int FindFreeStaff(int day_no, string Period_Code, ref string[] result)
        {
            bool[] StaffFree = new bool[255];
            for (int i = 0; i < 255; i++) StaffFree[i] = true;
            foreach (TT_period t in periodlist1.m_list)
            {
                if ((t.DayNo == day_no) && (t.PeriodCode == Period_Code))
                {
                    //find it in the staff_list
                    for (int i = 0; i < 255; i++)
                    {
                        if (StaffList1[i] == t.StaffId) { StaffFree[i] = false; break; }
                    }
                } 
            }
            int j = 0;
            for (int i = 0; i < staff_count; i++)
            {
                if (StaffFree[i]) { result[j] = StaffCodes[i]; j++; }
            }
            return j;
        }

        public class TT_period : IComparable
        {

            public Guid ScheduledPeriodId;
            public Guid StaffId;
            public string StaffCode;

            public Guid RoomId;
            public string RoomCode;
            
            public Guid PeriodId;
            public string PeriodCode;
            public int column;

            public Guid GroupId;

            
            public int DayNo;
            public string SetName;
            
            public byte[] line = new byte[16];//raw line
            public long File_Position;
            public bool valid;

            public string Covered_StaffCode;
            public Guid Covered_StaffId;

            public string Second_StaffCode;
            public Guid Second_StaffId;

            public TT_period()
            {
                //valid = false;
            }
            public override string ToString()
            {
                return SetName;
            }

            public void Hydrate(SqlDataReader dr)
            {
                //hydrate from query SELECT  * FROM INTRANET_FullTimetable_CC ";
                ScheduledPeriodId = dr.GetGuid(14);
                StaffId = dr.GetGuid(13);
                StaffCode = dr.GetString(3);
                RoomId = dr.GetGuid(6);
                RoomCode = dr.GetString(5);
                PeriodId = dr.GetGuid(8);
                PeriodCode = dr.GetString(7).Trim();

                column = 0;
                GroupId = dr.GetGuid(2);
                DayNo = dr.GetByte(9);
                SetName = dr.GetString(0);
                valid = true;

                Covered_StaffCode = "";
                Covered_StaffId = Guid.Empty;
                Second_StaffCode = "";
                Second_StaffId = Guid.Empty;
            }

            public bool Is_SameAs(TT_period p1)
            {
                if (SetName.Trim().ToUpper() != p1.SetName.Trim().ToUpper()) return false;
                if (StaffId != p1.StaffId) return false;
                if (RoomId != p1.RoomId) return false;
                if (PeriodId != p1.PeriodId) return false;
                return true;

            }

            #region IComparable members
            public int CompareTo(object obj)
            {
                TT_period otherTT_period = obj as TT_period;
                if (otherTT_period != null) return this.SetName.CompareTo(otherTT_period.SetName);
                else
                    throw new ArgumentException("Object is not a TT_period");
            }
            #endregion

        }
        public class TT_PeriodList
        {
            public System.Collections.ArrayList m_list = new System.Collections.ArrayList();

        }
        public Group Find_Group(string group_name, DateTime start, DateTime end,bool CreateGroup)
        {
            foreach (Group g in groups1._groups)
            {
                if (group_name == g._GroupName)
                {
                    return g;
                }
            }
            Group g1 = new Group();
            Encode en = new Encode();
            string s = "SELECT GroupId FROM tbl_Core_Groups ";
            s += "WHERE ( GroupName = '" + group_name + "' ) ";
            s += "AND (GroupValidFrom <= CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (GroupValidUntil > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102)) ";
            Guid gp_id = en.FindSQL(s);



            if ((gp_id == Guid.Empty) && (CreateGroup))
            {


                gp_id = Make_Group(group_name, start, end);
            }



            if (gp_id == Guid.Empty) return null;
            g1.Load(gp_id);
            return g1;
        }
        public Guid Make_Group(string group_name, DateTime t1, DateTime t2)
        {
            //need to make a newgroup .. but will need to find course etc....
            //Write_to_log("Adding Group ..  " + group_name + "  Valid from  " + t1.ToLongDateString() + "   Valid until  " + t2.ToLongDateString());
            Encode en = new Encode();
            string course_type = "0";
            string ounit = "Not Known";
            //an acadmeic course... 1 for KS3, 2 for KS4 3 for V, 4 for reg
            //set will be 13ch3 or 9H-TE
            //need to find the year...  ugh  if 1st char ==1 then else
            int y = 15;//y is the year...
            course_type = "1";
            try
            {
                if (group_name.Substring(0, 1) == "1") y = System.Convert.ToInt32(group_name.Substring(0, 2));
                else y = System.Convert.ToInt32(group_name.Substring(0, 1));
            }
            catch 
            {
                y = 0;//no year grooup...
            }
            course_type = "1";
            if (y > 11) course_type = "3";
            if ((y > 9) && (y < 12)) course_type = "2";
            //now for y9 if normal tt write for september then all KS3 ie type =1
            //but if summer re-write then some are KS4
            if ((y == 9) && (t1.Month < 7) && (t1.Month > 4) && (!group_name.Contains("-")))//ie summer re-write
            {
                course_type = "2";// 9Gm2 is GCSE course in Year 9...
            }

            string course = "unknown";
            course = "XX";
            if (y >= 10)
            {
                course = group_name.Substring(2, 2);
                //have to allow for 13-2RG and 11H-RG
                if (course.IndexOf("-") >= 0)
                {
                    int l = group_name.Length;
                    course = group_name.Substring(l - 2, 2);
                }
            }
            if ((y < 9)&&(y>6)) course = group_name.Substring(3, 2);
            if (y == 9)
            {
                if (!group_name.Contains("-")) course = group_name.Substring(1, 2);
                else course = group_name.Substring(3, 2);
            }
            if (group_name == "13SDO") { course_type = "0"; course = "XX"; }
            if (course.ToUpper() == "RG") course_type = "4";
            if (course.ToUpper() == "YA") course_type = "0";
            if (course.ToUpper() == "AS") course_type = "0";
            if (course.ToUpper() == "FP") course_type = "0";
            if (course.ToUpper() == "LE") { course_type = "0"; course = "LC"; }
            if (course.ToUpper() == "HB") { course_type = "3"; course = "BI"; }
            if (course.ToUpper() == "YE") { course = "YA"; course_type = "0"; }
            if (course.ToUpper() == "CV") { course = "EX"; course_type = "0"; }


            Guid cse_Id = Find_Course(course, course_type, ounit, true);
            if (cse_Id == Guid.Empty)
            {
                throw new NotImplementedException("oops can't find course");
            }
            Group g1 = new Group();
            g1._CourseID = cse_Id; g1._EndDate = t2; g1._StartDate = t1.AddDays(-1);
            g1._GroupCode = group_name; g1._GroupID = Guid.Empty; g1._GroupName = group_name;
            g1._GroupPrimaryAdministrative = false;

            g1.Save();
            return g1._GroupID;
            /*
            string s = "INSERT INTO tbl_Core_Groups ";
            s += "(GroupName, GroupValidFrom, GroupValidUntil, GroupCode, CourseId, GroupRegistrationType , Version )"; //version as temp

            s += "VALUES ('" + group_name + "' , ";
            s += " CONVERT(DATETIME, '" + t1.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
            s += " CONVERT(DATETIME, '" + t2.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
            s += " '" + group_name + "' , ";
            s += "'" + cse_Id.ToString() + "' , ";
            s += "'3'";//registration type
            s += ", '5' "; //version
            s += ")";
            en.ExecuteSQL(s);

            s = "SELECT GroupId FROM tbl_Core_Groups ";
            s += "WHERE ( GroupName = '" + group_name + "' ) ";
            s += " AND (GroupValidFrom <=  CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ) ";
            s += " AND (GroupValidUntil > CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            return en.FindSQL(s);
             * */
        }
        private Guid Find_Course(string course_code, string course_type, string ounit, bool create)
        {
            Encode en = new Encode();
            string s = "SELECT CourseId FROM tbl_Core_Courses ";
            s += " WHERE (CourseCode = '" + course_code + "') AND (CourseType = '" + course_type + "') ";
            Guid cse_id = en.FindSQL(s);
            if ((cse_id == Guid.Empty) && create)
            {
                //try again for any type/.....
                s = "SELECT CourseId FROM tbl_Core_Courses ";
                s += " WHERE (CourseCode = '" + course_code + "')  ";
                cse_id = en.FindSQL(s);
            }

            if ((cse_id == Guid.Empty) && create)
            {
                throw new NotImplementedException("oops no course");
            }
            return cse_id;
        }
        public void SetValidityFalse(DateTime t1)
        {
            string s2 = " UPDATE tbl_Core_ScheduledPeriodValidity SET Version=9 ";
            s2 += " WHERE (ValidityEnd > CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ) ";
            en.ExecuteSQL(s2);
        }
        public void SetValidityTrue(Guid Id, DateTime t1)
        {
            string s2 = " UPDATE tbl_Core_ScheduledPeriodValidity SET Version=0 ,ValidityEnd=CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
            s2 += " WHERE (ScheduledPeriodValidityId = '" + Id.ToString() + "' ) ";
            en.ExecuteSQL(s2);
        }
        public void CleanValidity(DateTime t1)
        {
            string s2 = " UPDATE tbl_Core_ScheduledPeriodValidity SET ValidityEnd=CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), Version ='1'  ";
            s2 += " WHERE (Version = '9' ) AND (ValidityEnd > CONVERT(DATETIME, '" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ) ";
            en.ExecuteSQL(s2);
        }
        public void ResetScheduledPeriodVersion()
        {
            string s2 = " UPDATE tbl_Core_ScheduledPeriods SET Version='0'  ";
            s2 += " WHERE (Version > '0' )  ";
            en.ExecuteSQL(s2);

            s2 = " UPDATE tbl_Core_Groups SET Version='1'  ";
            s2 += " WHERE (Version > '1' )  ";
            en.ExecuteSQL(s2);
        }
        public Guid Find_Sched(Guid group_id, Guid staff_id, Guid room_id, Guid period_id, int day)
        {
            Guid id = Find_SchedX(group_id, staff_id, room_id, period_id, day);
            if (id == Guid.Empty) id = Make_Sched(group_id, staff_id, room_id, period_id, day);
            return id;
        }
        public  Guid Find_SchedX(Guid group_id, Guid staff_id, Guid room_id, Guid period_id, int day)
        {
            Encode en = new Encode();
            string s = "SELECT ScheduledPeriodId FROM tbl_Core_ScheduledPeriods ";
            s += "WHERE GroupId = '" + group_id.ToString() + "' ";
            s += " AND StaffId = '" + staff_id.ToString() + "' ";
            s += " AND RoomId = '" + room_id.ToString() + "' ";
            s += " AND PeriodId = '" + period_id.ToString() + "' ";
            s += " AND DayNo = '" + day.ToString() + "'";
            return en.FindSQL(s);
        }
        private Guid Make_Sched(Guid group_id, Guid staff_id, Guid room_id, Guid period_id, int day)
        {
            Encode en = new Encode();
            string s = "INSERT INTO tbl_Core_ScheduledPeriods ";
            s += " (GroupId , StaffId , RoomId , PeriodId , DayNo , NonScheduled, Version )";
            s += " VALUES ( '" + group_id.ToString() + "' , '" + staff_id.ToString() + "' , '" + room_id.ToString() + "' ,";
            s += "'" + period_id.ToString() + "', '" + day.ToString() + "' , '0' , '1' )";
            en.ExecuteSQL(s);
            return Find_SchedX(group_id, staff_id, room_id, period_id, day);
        }
    }
    #endregion

    #region  classes to deal with Block structure
    public class TTScheme
    {
        public Guid SchemeId = Guid.Empty;
        public DateTime SchemeStartDate;
        public bool Visible;
        public string SchemeName;
        public int Version;
        public bool valid = false;

        public TTScheme() { }

        public void Hydrate(SqlDataReader dr)
        {
            SchemeId = dr.GetGuid(0);
            SchemeStartDate = dr.GetDateTime(1);
            Visible = dr.GetBoolean(2);
            SchemeName = dr.GetString(3);
            Version = dr.GetInt32(4);
            valid = true;
        }

        public void Load(Guid Id)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT *   FROM tbl_TTPlan_Schemes ";
            s += " WHERE SchemeId ='" + Id.ToString() + "' ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Delete()
        {
            if (SchemeId == Guid.Empty) return;
            //need to delete all children!!!
            TTBlockList tbl1 = new TTBlockList();
            TTLinearGroupList tgl1 = new TTLinearGroupList();
            tbl1.LoadList(SchemeId);
            foreach (TTBlock b in tbl1.m_list) { b.Delete(); }

            string s = "DELETE FROM tbl_TTPlan_Schemes   ";
            s += " WHERE Id ='" + SchemeId.ToString() + "' ";
            Encode en = new Encode();
            en.ExecuteSQL(s);
            SchemeId = Guid.Empty;
        }
    }

    public class TTSchemeList
    {
        public System.Collections.ArrayList m_list = new System.Collections.ArrayList();
        public TTSchemeList() { }
        public void LoadList()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT  SchemeId, SchemeStartDate, Visibile, SchemeName, Version FROM tbl_TTPlan_Schemes ";
            s += " ORDER BY SchemeStartDate DESC  ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            TTScheme t1 = new TTScheme();
                            m_list.Add(t1);
                            t1.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class TTBlock
    {
        public Guid BlockId = Guid.Empty;
        public Guid TTSchemeId;
        public string BlockName;
        public int Year;
        public int Periods;
        public int Colour;
        public int RowOffset;

        public DateTime SchemeStartDate;
        public bool Visible;
        public string SchemeName;
        public int Version = 0;
        public bool valid = false;



        public TTBlock() { }

        public void Hydrate(SqlDataReader dr)
        {
            BlockId = dr.GetGuid(0);
            TTSchemeId = dr.GetGuid(1);
            BlockName = dr.GetString(2);
            Year = 0;
            if (!dr.IsDBNull(3)) Year = dr.GetInt32(3);
            Periods = dr.GetInt32(4);
            Colour = dr.GetInt32(5);
            RowOffset = dr.GetInt32(6);
            Version = dr.GetInt32(7);

            SchemeStartDate = dr.GetDateTime(8);
            Visible = dr.GetBoolean(9);
            SchemeName = dr.GetString(10);

            valid = true;
        }

        public void Load(Guid Id)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = " SELECT  dbo.tbl_TTPlan_Blocks.BlockId, dbo.tbl_TTPlan_Schemes.SchemeId AS Expr1, ";
            s += " dbo.tbl_TTPlan_Blocks.BlockName,";
            s += " dbo.tbl_TTPlan_Blocks.Year, dbo.tbl_TTPlan_Blocks.Periods, dbo.tbl_TTPlan_Blocks.TTColour, ";
            s += " dbo.tbl_TTPlan_Blocks.RowOffset, dbo.tbl_TTPlan_Blocks.Version, ";
            s += "  dbo.tbl_TTPlan_Schemes.SchemeStartDate, ";
            s += " dbo.tbl_TTPlan_Schemes.Visibile, dbo.tbl_TTPlan_Schemes.SchemeName  ";
            s += " FROM  dbo.tbl_TTPlan_Blocks ";
            s += " INNER JOIN dbo.tbl_TTPlan_Schemes ";
            s += " ON dbo.tbl_TTPlan_Blocks.TTSchemeId = dbo.tbl_TTPlan_Schemes.SchemeId  ";
            s += " WHERE dbo.tbl_TTPlan_Blocks.BlockId = '" + Id.ToString() + "' ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Save()
        {
            string s = "";
            if (BlockId != Guid.Empty)
            {
                //Update
                s = "UPDATE dbo.tbl_TTPlan_Blocks ";
                s += " SET BlockName = '" + BlockName + "' ";
                s += ", Year = '" + Year.ToString() + "' ";
                s += ", Periods = '" + Periods.ToString() + "' ";
                s += ", TTColour = '" + Colour.ToString() + "' ";
                s += ", RowOffset= '" + RowOffset.ToString() + "' ";
                s += ", Version= '" + Version.ToString() + "' ";
                s += " WHERE (BlockId = '" + BlockId.ToString() + "' ) ";
            }
            else
            {
                //Save
                BlockId = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_TTPlan_Blocks  ";
                s += " (BlockId, TTSchemeId,BlockName,  Year, Periods, TTColour,RowOffset, Version ) ";
                s += " VALUES ( '" + BlockId.ToString() + "', '";
                s += TTSchemeId.ToString() + "', '";
                s += BlockName + "', '";
                s += Year.ToString() + "', '";
                s += Periods.ToString() + "', '";
                s += Colour.ToString() + "',  '";
                s += RowOffset.ToString() + "', '";
                s += Version.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public void Delete()
        {
            if (BlockId == Guid.Empty) return;
            //ought to delete all children....
            TTLinearGroupList lgl1 = new TTLinearGroupList(); lgl1.LoadList(BlockId);
            foreach (TTLinearGroup g in lgl1.m_list) { g.Delete(); }

            string s = "DELETE FROM dbo.tbl_TTPlan_Blocks ";
            s += " WHERE (dbo.tbl_TTPlan_Blocks.BlockId = '" + BlockId.ToString() + "') ";
            Encode en = new Encode(); en.ExecuteSQL(s);
            BlockId = Guid.Empty;
        }
    }

    public class TTBlockList
    {
        public System.Collections.ArrayList m_list = new System.Collections.ArrayList();
        public TTBlockList() { }
        public void LoadList(Guid TTSchemeId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = " SELECT  dbo.tbl_TTPlan_Blocks.BlockId, dbo.tbl_TTPlan_Schemes.SchemeId AS Expr1, ";
            s += " dbo.tbl_TTPlan_Blocks.BlockName,";
            s += " dbo.tbl_TTPlan_Blocks.Year, dbo.tbl_TTPlan_Blocks.Periods, dbo.tbl_TTPlan_Blocks.TTColour, ";
            s += " dbo.tbl_TTPlan_Blocks.RowOffset, dbo.tbl_TTPlan_Blocks.Version, ";
            s += "  dbo.tbl_TTPlan_Schemes.SchemeStartDate, ";
            s += " dbo.tbl_TTPlan_Schemes.Visibile, dbo.tbl_TTPlan_Schemes.SchemeName  ";
            s += " FROM  dbo.tbl_TTPlan_Blocks ";
            s += " INNER JOIN dbo.tbl_TTPlan_Schemes ";
            s += " ON dbo.tbl_TTPlan_Blocks.TTSchemeId = dbo.tbl_TTPlan_Schemes.SchemeId  ";
            s += " WHERE dbo.tbl_TTPlan_Blocks.TTSchemeId = '" + TTSchemeId.ToString() + "' ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            TTBlock b1 = new TTBlock();
                            m_list.Add(b1);
                            b1.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

    }

    public class TTLinearGroup
    {
        public Guid LinearGroupId;
        public Guid TTBlockId;
        public string LinearGroupName;
        public int RowOffset;
        public int Version;
        public bool valid;
        public System.Collections.Generic.List<ScheduledPeriodPlan> Periods_List = new List<ScheduledPeriodPlan>();
        public TTLinearGroup() { LinearGroupId = Guid.Empty; }
        public void Hydrate(SqlDataReader dr)
        {
            LinearGroupId = dr.GetGuid(0);
            TTBlockId = dr.GetGuid(1);
            LinearGroupName = dr.GetString(2);
            RowOffset = 0;
            if (!dr.IsDBNull(3)) RowOffset = dr.GetInt32(3);
            Version = dr.GetInt32(4);
            valid = true;

        }
        public void LoadPeriods()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT ScheduledPeriodId";
            s += " FROM  tbl_TTPlan_ScheduledPeriods  ";
            s += " WHERE (TTPlanLinearGroupId = '" + LinearGroupId.ToString() + "' ) ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodPlan sp = new ScheduledPeriodPlan();
                            Periods_List.Add(sp);
                            sp.Load(dr.GetGuid(0));
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void Save()
        {
            string s = "";
            if (LinearGroupId != Guid.Empty)
            {
                //Update
                s = "UPDATE dbo.tbl_TTPlan_LinearGroups";
                s += " SET LinearGroupName = '" + LinearGroupName + "' ";
                s += ", RowOffset = '" + RowOffset.ToString() + "' ";
                s += ", Version= '" + Version.ToString() + "' ";
                s += " WHERE (BlockId = '" + LinearGroupId.ToString() + "' ) ";
            }
            else
            {
                //Save
                LinearGroupId = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_TTPlan_LinearGroups  ";
                s += " (LinearGroupId, TTBlockId, LinearGroupName, RowOffset, Version ) ";
                s += " VALUES ( '" + LinearGroupId.ToString() + "', '";
                s += TTBlockId.ToString() + "', '";
                s += LinearGroupName + "', '";
                s += RowOffset.ToString() + "', '";
                s += Version.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }
        public void Delete()
        {
            if (LinearGroupId == Guid.Empty) return;
            //ought to delete all children...
            ScheduledPeriodPlanList pl1 = new ScheduledPeriodPlanList();
            pl1.Load_LinearGroup(LinearGroupId);
            foreach (ScheduledPeriodPlan p in pl1.m_list) { p.Delete(); }

            string s = "DELETE FROM  dbo.tbl_TTPlan_LinearGroups";
            s += " WHERE ( LinearGroupId = '" + LinearGroupId.ToString() + "') ";
            Encode en = new Encode(); en.ExecuteSQL(s);
            LinearGroupId = Guid.Empty;
        }

        public void Load(Guid Id)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT LinearGroupId, TTBlockId, LinearGroupName, RowOffset, Version";
            s += " FROM  tbl_TTPlan_LinearGroups  ";
            s += " WHERE (LinearGroupId = '" + Id.ToString() + "' )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydrate(dr);
                            LoadPeriods();
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }


        public void Load(Guid SchemeId, string Name, int Year)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT dbo.tbl_TTPlan_LinearGroups.LinearGroupId, ";
            s += " dbo.tbl_TTPlan_LinearGroups.TTBlockId, dbo.tbl_TTPlan_LinearGroups.LinearGroupName, ";
            s += " dbo.tbl_TTPlan_LinearGroups.RowOffset, dbo.tbl_TTPlan_LinearGroups.Version   ";
            s += "FROM  dbo.tbl_TTPlan_LinearGroups INNER JOIN ";
            s += " dbo.tbl_TTPlan_Blocks ON dbo.tbl_TTPlan_LinearGroups.TTBlockId = dbo.tbl_TTPlan_Blocks.BlockId INNER JOIN ";
            s += "dbo.tbl_TTPlan_Schemes ON dbo.tbl_TTPlan_Blocks.TTSchemeId = dbo.tbl_TTPlan_Schemes.SchemeId  ";
            s += " WHERE (dbo.tbl_TTPlan_Blocks.Year = '" + Year.ToString() + "') ";
            s += " AND (dbo.tbl_TTPlan_Blocks.TTSchemeId = '" + SchemeId.ToString() + "') ";
            s += " AND (dbo.tbl_TTPlan_LinearGroups.LinearGroupName = N'" + Name + "') ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydrate(dr);
                            LoadPeriods();
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class TTLinearGroupList
    {
        public System.Collections.ArrayList m_list = new System.Collections.ArrayList();
        public TTLinearGroupList() { }
        public void LoadList(Guid TTBlockId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT LinearGroupId, TTBlockId, LinearGroupName, RowOffset, Version";
            s += " FROM  tbl_TTPlan_LinearGroups  ";
            s += " WHERE (TTBlockId = '" + TTBlockId.ToString() + "' )";
            s += " ORDER BY LinearGroupName  ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            TTLinearGroup b1 = new TTLinearGroup();
                            m_list.Add(b1);
                            b1.Hydrate(dr);
                            b1.LoadPeriods();
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class TTBinaryData
    {
        public TTBinaryData()
        {
        }
        public Guid Id = Guid.Empty;
        public Guid SchemeId = Guid.Empty;

        public byte[] RawData = new byte[8000];

        public void Load(Guid SchemeId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT *   FROM tbl_TTPlan_RawBinaryData ";
            s += " WHERE SchemeId ='" + SchemeId.ToString() + "' ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Id = dr.GetGuid(0);
                            SchemeId = dr.GetGuid(1);
                            long i = dr.GetBytes(2, 0, RawData, 0, 8000);
                            i = i;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Save(Guid SchemeId)
        {
            string s = "";
            if (Id != Guid.Empty)
            {
                //Update
                s = "UPDATE dbo.tbl_TTPlan_Blocks ";
                /*s += " SET BlockName = '" + BlockName + "' ";
                s += ", Year = '" + Year.ToString() + "' ";
                s += ", Periods = '" + Periods.ToString() + "' ";
                s += ", TTColour = '" + Colour.ToString() + "' ";
                s += ", RowOffset= '" + RowOffset.ToString() + "' ";
                s += ", Version= '" + Version.ToString() + "' ";
                s += " WHERE (BlockId = '" + BlockId.ToString() + "' ) ";
                 * */
            }
            else
            {
                //Save
                Id = Guid.NewGuid();
                s = " INSERT INTO tbl_TTPlan_RawBinaryData (Id, ttPlan_SchemeId, BinaryData)";
                s += " VALUES ( '" + Id.ToString() + "', '";
                s += SchemeId.ToString() + "', ";
                s += " CAST ( '0x";
                FileStream fs = new FileStream(@"z:\tt_data.ttd", FileMode.Open, FileAccess.Read);
                byte b;

                for (int i = 0; i < fs.Length; i++)
                {
                    b = (byte)fs.ReadByte();
                    s += b.ToString();
                }
                s += "' AS varbinary ) )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);

        }

    }

    #endregion
}
