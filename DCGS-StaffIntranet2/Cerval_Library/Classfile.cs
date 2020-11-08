
//#define ADD_URL_TO_MESSAGE

using System;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Web;
using System.Security.Claims;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Runtime.Serialization.Json;
using System.Web.Hosting;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Data.SqlTypes;

namespace Cerval_Library
{
    public class Cerval_Configuration
    {
        public Guid Id;
        public string Key;
        public string Value;
        public string Notes;
        public int TypeId;
        public string Type;
        public bool valid = false;

        public Cerval_Configuration() { }

        public Cerval_Configuration(string key)
        {
            Encode en = new Encode();
            string s = "SELECT *,dbo.tbl_List_DisplayKeyTypes.Type FROM dbo.tbl_Core_DisplayKeys INNER JOIN ";
            s += "dbo.tbl_List_DisplayKeyTypes ON dbo.tbl_Core_DisplayKeys.Type = dbo.tbl_List_DisplayKeyTypes.Id";
            s += "  WHERE ([Key]='" + key + "' )";
            SqlConnection cn = new SqlConnection(en.GetDbConnection()); cn.Open();
            SqlCommand cm = new SqlCommand(s, cn);
            SqlDataReader dr = cm.ExecuteReader();
            while (dr.Read())
            {
                Hydrate(dr); valid = true;
            }
            dr.Close(); cn.Close();

        }

        public override string ToString()
        {
            return Key;
        }

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            Key = dr.GetString(1);
            Value = dr.GetString(2);
            Notes = dr.GetString(3);//4 and 5 are int id
            TypeId = dr.GetInt32(4);
            Type = dr.GetString(6);
        }

        public void Save()
        {
            //if id exists then update - else create...
            string s = "";
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Core_DisplayKeys (ID,[Key], Value, notes, type )";
                s += "VALUES ('" + Id.ToString() + "', '" + Key + "' , '" + Value + "' , '" + Notes + "' , '" + TypeId + "'  )";
            }
            else
            {
                s = "UPDATE dbo.tbl_Core_DisplayKeys SET Value='" + Value + "'  ";
                s += " WHERE (ID ='" + Id.ToString() + "' )";
            }
            Encode en = new Encode(); en.ExecuteSQL(s);
        }

    }


    public class Cerval_Configurations
    {
        public System.Collections.Generic.List<Cerval_Configuration> list = new List<Cerval_Configuration>();
        public bool Is_loaded = false;

        public void Load_All()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM dbo.tbl_Core_DisplayKeys INNER JOIN ";
            s += "dbo.tbl_List_DisplayKeyTypes ON dbo.tbl_Core_DisplayKeys.Type = dbo.tbl_List_DisplayKeyTypes.Id";
            s += " ORDER BY [Key]";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Cerval_Configuration c = new Cerval_Configuration();
                            c.Hydrate(dr);
                            list.Add(c);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            Is_loaded = true;
        }

        public bool Find_Key(string key, out string value, out string notes)
        {
            value = ""; notes = "";
            if (!Is_loaded) Load_All();
            foreach (Cerval_Configuration c in list)
            {
                if (c.Key == key) { value = c.Value; notes = c.Notes; return true; }
            }
            return false;
        }

        public bool Find_Key(string key, out string value)
        {
            value = "";
            if (!Is_loaded) Load_All();
            foreach (Cerval_Configuration c in list)
            {
                if (c.Key == key) { value = c.Value; return true; }
            }
            return false;
        }

    }

    public class Cerval_Globals
    {
        public static Guid newdawnCse = new Guid("2aa660b7-394a-403f-853d-99c81f762e78");
        public static Guid DCGSroot = new Guid("fa6d2a80-531d-49c9-b043-d8370b8cf163");
        public static Guid newdawnLinearCse = new Guid("057d1186-875d-48e8-8d32-d46794330df6");
        public static Guid RegistrationCse = new Guid("e51cea19-5056-4841-a048-05c0df7e684e");
        public static Guid IndividualStaffGroup = new Guid("0d1cdbaf-d8c1-4425-9a08-8b1b41e98a6e");
        public static List<Guid> NewStructureCourses = new List<Guid>();



        public Cerval_Globals()
        {
            NewStructureCourses.Add(newdawnCse);
            NewStructureCourses.Add(newdawnLinearCse);
            NewStructureCourses.Add(DCGSroot);
        }
    }
    public class Classfile
    {
        public byte[] GetPersonPhoto(Guid PersonId)
        {
            byte[] b3;
            string s = " SELECT * FROM dbo.tbl_Core_PeopleImages ";
            s += " WHERE (PersonId = '" + PersonId.ToString() + "' ) ";
            s += " ORDER BY ImageDate DESC";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            System.Data.SqlTypes.SqlBinary b1 = new System.Data.SqlTypes.SqlBinary();
                            b1 = dr.GetSqlBinary(2);
                            b3 = new byte[b1.Length];
                            b3 = (byte[])b1;
                        }
                        else
                        {
                            b3 = null;
                        }
                        dr.Close();
                    }
                }
            }
            return b3;
        }
    }
    public class Contact
    {
        public int m_ContactID;
        public string m_ContactType;
        public string m_Contact_Value;
        public bool m_Contact_Private;
    }
    public class ContactList
    {
        public ArrayList m_contactlist = new ArrayList();
        public ContactList() { m_contactlist.Clear(); }
        public void LoadList(string PersonID)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_List_ContactTypes.ContactType, dbo.tbl_List_ContactTypes.Id, ";
            s += "dbo.tbl_Core_People_Contacts.ContactValue, dbo.tbl_List_ContactTypes.Private  ";
            s += " FROM dbo.tbl_Core_People_Contacts INNER JOIN ";
            s += "dbo.tbl_List_ContactTypes ON dbo.tbl_Core_People_Contacts.ContactType = dbo.tbl_List_ContactTypes.Id";
            s += " WHERE dbo.tbl_Core_People_Contacts.PersonId =  '";
            s += PersonID + "'";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Contact c = new Contact();
                            m_contactlist.Add(c);
                            c.m_ContactID = dr.GetInt32(1);
                            c.m_ContactType = dr.GetString(0);
                            c.m_Contact_Value = dr.GetString(2);
                            c.m_Contact_Private = dr.GetBoolean(3);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class Course
    {
        public Guid _CourseID;
        public string CourseCode;
        public string CourseName;
        public string CourseType;
        public int KeyStage;
        public Guid _OrganisationalUnit;
        public bool _valid = false;
        public Course()
        {
            _CourseID = Guid.Empty;
        }

        public void Load(Guid CourseId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //get a list of all courses.... 
            string s = "SELECT dbo.tbl_Core_Courses.CourseId, dbo.tbl_List_CourseTypes.CourseType, ";
            s = s + "dbo.tbl_List_CourseTypes.KeyStage, dbo.tbl_Core_Courses.CourseCode, dbo.tbl_Core_Courses.CourseName, dbo.tbl_Core_Courses.OrganisationalUnitId ";
            s = s + "FROM dbo.tbl_List_CourseTypes INNER JOIN dbo.tbl_Core_Courses ON dbo.tbl_List_CourseTypes.Id = dbo.tbl_Core_Courses.CourseType";
            s += " WHERE dbo.tbl_Core_Courses.CourseId = '" + CourseId.ToString() + "'";
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

        public void Hydrate(SqlDataReader dr)
        {
            _CourseID = dr.GetGuid(0);
            CourseType = dr.GetString(1);
            if (!dr.IsDBNull(2)) KeyStage = dr.GetInt32(2); else KeyStage = 0;
            CourseCode = dr.GetString(3);
            CourseName = dr.GetString(4);
            if (!dr.IsDBNull(5)) _OrganisationalUnit = dr.GetGuid(5); else _OrganisationalUnit = Guid.Empty;
            _valid = true;
        }

        public void Save()
        {
            string s = "";
            if (_CourseID == Guid.Empty)
            {
                _CourseID = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Core_Courses (CourseID, OrganisationalUnitId, CourseCode, CourseName, CourseType, Version ) ";
                s += " VALUES ( '" + _CourseID.ToString() + "', '";
                s += _OrganisationalUnit.ToString() + "', '";
                s += CourseCode + "', '";
                s += CourseName + "',  '";
                s += CourseType + "', '5' )";
            }
            else
            {
                //update...
                s = "UPDATE dbo.tbl_Core_Courses ";
                s += " SET OrganisationalUnitId = '" + _OrganisationalUnit.ToString() + "' ";
                s += ", CourseCode = '" + CourseCode + "' ";
                s += ", CourseName = '" + CourseName + "' ";
                s += ", CourseType = '" + CourseType + "' ";
                s += " WHERE (CourseID = '" + _CourseID.ToString() + "' ) ";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public bool GetResultType(ref int ResultType, DateTime CourseStartDate)
        {
            ResultType = 0;
            if (_CourseID == Guid.Empty) return false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //get a list of all courses.... 
            string s = "SELECT  CourseResultTypeID, CourseID, ResultType, ValidFrom, ValidUntil, Version ";
            s = s + "FROM tbl_Core_Course_ResultType WHERE tbl_Core_Course_ResultType.CourseId = '" + _CourseID.ToString() + "'";
            DateTime time1 = new DateTime(1900, 1, 1); DateTime time2 = new DateTime(2200, 1, 1);
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            time1 = new DateTime(1900, 1, 1); time2 = new DateTime(2200, 1, 1);
                            if (!dr.IsDBNull(3)) time1 = dr.GetDateTime(3);//valid from
                            if (!dr.IsDBNull(4)) time2 = dr.GetDateTime(4);//valid until
                            if ((CourseStartDate > time1) && (CourseStartDate < time2))
                            {
                                ResultType = dr.GetInt32(2);
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            if (ResultType > 0)
                return true;

            return false;
        }
    }
    public class CourseList
    {
        public ArrayList _courses = new ArrayList();
        public CourseList(int Key_Stage)
        {
            //if Key_Stage==0 then return all, esle just for that KS
            _courses.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //get a list of all courses.... 
            string s = "SELECT dbo.tbl_Core_Courses.CourseId, dbo.tbl_List_CourseTypes.CourseType, ";
            s = s + "dbo.tbl_List_CourseTypes.KeyStage, dbo.tbl_Core_Courses.CourseCode, dbo.tbl_Core_Courses.CourseName, dbo.tbl_Core_Courses.OrganisationalUnitId ";
            s = s + "FROM dbo.tbl_List_CourseTypes INNER JOIN dbo.tbl_Core_Courses ON dbo.tbl_List_CourseTypes.Id = dbo.tbl_Core_Courses.CourseType";
            if (Key_Stage != 0) s = s + " WHERE dbo.tbl_List_CourseTypes.KeyStage = '" + Key_Stage.ToString() + "'";
            s = s + " ORDER BY dbo.tbl_Core_Courses.CourseName   ASC";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Course c = new Course();
                            _courses.Add(c);
                            c._CourseID = dr.GetGuid(0);
                            c.CourseType = dr.GetString(1);
                            if (!dr.IsDBNull(2)) c.KeyStage = dr.GetInt32(2); else c.KeyStage = 0;
                            c.CourseCode = dr.GetString(3);
                            c.CourseName = dr.GetString(4);
                            if (!dr.IsDBNull(5)) c._OrganisationalUnit = dr.GetGuid(5); else c._OrganisationalUnit = Guid.Empty;

                            c._valid = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public CourseList(Guid OptionId)
        {
            _courses.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT dbo.tbl_Core_Courses.CourseId, dbo.tbl_List_CourseTypes.CourseType, ";
            s = s + "dbo.tbl_List_CourseTypes.KeyStage, dbo.tbl_Core_Courses.CourseCode, dbo.tbl_Core_Courses.CourseName ";
            s = s + "FROM dbo.tbl_List_CourseTypes INNER JOIN dbo.tbl_Core_Courses ON dbo.tbl_List_CourseTypes.Id = dbo.tbl_Core_Courses.CourseType ";
            s += "INNER JOIN dbo.tbl_Exams_Options_Courses ON dbo.tbl_Exams_Options_Courses.CourseId=dbo.tbl_Core_Courses.CourseId ";
            s += " WHERE ( OptionId = '" + OptionId.ToString() + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Course c = new Course();
                            _courses.Add(c);
                            c._CourseID = dr.GetGuid(0);
                            c.CourseType = dr.GetString(1);
                            if (!dr.IsDBNull(2)) c.KeyStage = dr.GetInt32(2); else c.KeyStage = 0;
                            c.CourseCode = dr.GetString(3);
                            c.CourseName = dr.GetString(4);
                            c._valid = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }


    public class CoverPeriod
    {
        //class to hold data for a single cover period
        public Guid Id;
        public Guid StaffAbsenceId;
        public Guid ScheduledPeriodId;
        public Guid CoveringStaffId;
        public Guid RoomId;
        public DateTime date;
        public int Version;

        public CoverPeriod() { }
        public CoverPeriod(Guid CoverPeriodId)
        {
            string s = "SELECT * FROM tbl_Core_CoverPeriods WHERE Id = '" + CoverPeriodId.ToString() + "' ";
            Load1(s);

        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
                    }
                }
                cn.Close();
            }

        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            StaffAbsenceId = dr.GetGuid(1);
            ScheduledPeriodId = dr.GetGuid(2);
            CoveringStaffId = dr.GetGuid(3);
            RoomId = dr.GetGuid(4);
            date = dr.GetDateTime(5);
            Version = dr.GetInt32(6);
        }
        public void Save()
        {
            string s = "";
            if (Id == Guid.Empty)
            {
                // make a new record..
                Id = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Core_CoverPeriods (Id,StaffAbsenceId, ScheduledPeriodId , CoveringStaffId, RoomId, Date, Version )";
                s += "VALUES ('" + Id.ToString() + "', '" + StaffAbsenceId.ToString() + "' , '" + ScheduledPeriodId.ToString() + "' , '" + CoveringStaffId.ToString() + "' , '" + RoomId.ToString() + "' , CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , '1' )";
            }
            else
            {
                s = "UPDATE dbo.tbl_Core_CoverPeriods SET StaffAbsenceId='" + StaffAbsenceId.ToString() + "',  ScheduledPeriodId='" + ScheduledPeriodId.ToString() + "', CoveringStaffId = '" + CoveringStaffId.ToString() + "' ,  RoomId = '" + RoomId.ToString() + "' , Date =  CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , Version = '" + Version.ToString() + "' ";
                s += " WHERE (Id ='" + Id.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }
        public bool Delete()
        {
            if ((Id == null) || (Id == Guid.Empty)) return false;
            Encode en = new Encode(); string s = "";
            s = "DELETE FROM dbo.tbl_Core_CoverPeriods WHERE (Id = '" + Id.ToString() + "' ) ";
            en.ExecuteSQL(s);
            return true;
        }

    }
    public class CoverPeriodList
    {
        public ArrayList CoverPeriods = new ArrayList();
        private void LoadList(string s)
        {
            CoverPeriods.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            CoverPeriod p = new CoverPeriod();
                            CoverPeriods.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public CoverPeriodList()
        {
            CoverPeriods.Clear();
        }
        public void LoadList_for_Absence(Guid AbsenceId)
        {
            string s = "SELECT * FROM tbl_Core_CoverPeriods WHERE StaffAbsenceId = '" + AbsenceId.ToString() + "' ";
            LoadList(s);
        }
    }
    [Serializable]
    public class days
    {
        public int m_daycode;
        public string m_dayname;
    }
    public class DayList
    {
        public ArrayList m_DayList = new ArrayList();
        public DayList()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM tbl_List_Days ORDER BY id";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            days d = new days();
                            m_DayList.Add(d);
                            d.m_daycode = (int)dr.GetByte(0);
                            d.m_dayname = dr.GetString(1);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public string FindDayName_fromId(int Id)
        {
            foreach (days d in m_DayList)
            {
                if (d.m_daycode == Id) return d.m_dayname;
            }
            return "";
        }
        public string FindDayName_fromId(string Id)
        {
            foreach (days d in m_DayList)
            {
                if (d.m_daycode.ToString() == Id) return d.m_dayname;
            }
            return "";
        }
    }




    public class Encode
    {
        private byte[] Key = { 0x5f, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        private byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        public string GetFilePath(string filename)
        {
            string s = "";
            s = HostingEnvironment.MapPath(@"/App_Data/" + filename);//OK for web apps
            if (s == null)
            {
                s = @"../App_Data/" + filename;
            }
            return s;
        }

        public string GetDbConnection()
        {
#if DEBUG
            string s = "";
            //s = ReadEncrypted(GetFilePath("CervalDb.txt"));
            return " Initial Catalog = Civet; Data Source = 192.168.1.98; user ID =sa; password = djgt290652";
            return " Initial Catalog = Cerval; Data Source = 192.168.1.98; user ID =sa; password = djgt290652";
            //s=HostingEnvironment.MapPath(@"/App_Data/CervalDb.txt");
            //AppSettingsReader ar = new AppSettingsReader();sdf:!602
            //s = ar.GetValue("CervalDb", typeof(string)).ToString();
            //s = ReadEncrypted(HostingEnvironment.MapPath(@"/App_Data/CervalDb.txt"));
           // return "Initial Catalog = Cerval; Data Source = 10.1.84.230,1433; user ID =sa; password = 4h4h7..>sapa";
            s = ReadEncrypted(GetFilePath("CervalDb.txt"));
            s = "Initial Catalog=Cerval;Data Source=postyshev.challoners.net;user ID=cc-development;password=sdf:!602";
            return ReadEncrypted(GetFilePath("CervalDb.txt"));
            return "Initial Catalog = iSAMS_Challoners; Data Source = tcp: 95.138.141.94,29347; user ID = wwwiSAMS_DCGSAccess; password = Flw5P % sdp1mq6f76";
#else
            return "Initial Catalog = Cerval; Data Source = 10.1.84.230,1433; user ID =sa; password = 4h4h7..>sapa";
            return ReadEncrypted(GetFilePath("CervalDb.txt"));
            return ReadEncrypted(HostingEnvironment.MapPath(@"/App_Data/CervalDb.txt"));

#endif
        }
        public string GetISAMSDBConnection()
        {
            string s = ReadEncrypted(GetFilePath("iSAMSDB.txt"));

            return ReadEncrypted(GetFilePath("iSAMSDB.txt"));
        }

        public string GetCaracalDBConnection()
        {
            string s = "Initial Catalog = Caracal; Data Source = 10.1.84.230,1433; user ID =sa; password = 4h4h7..>sapa";

            return s;
        }


        public string GetGladstoneDBConnection()
        {
            return ReadEncrypted(GetFilePath("GladstoneDB.txt"));
        }
        public string GetCervalConnectionString()
        {
            return ReadEncrypted(GetFilePath("CervalConnectionDB.txt"));
        }
        public string ReadEncrypted(string file_name)
        {
            RijndaelManaged RMCrypto = new RijndaelManaged();
            FileStream fs = File.Open(file_name, FileMode.Open, FileAccess.Read, FileShare.Read);
            CryptoStream CryptStream = new CryptoStream(fs, RMCrypto.CreateDecryptor(Key, IV), CryptoStreamMode.Read);
            StreamReader SReader = new StreamReader(CryptStream);
            string s = SReader.ReadToEnd();
            SReader.Close();
            fs.Close();
            return s;
        }

        public void WriteEncrypted(string Text, string filename)
        {
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(Text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            using (FileStream fs = File.Open(filename, FileMode.OpenOrCreate))
            {
                fs.Write(encrypted, 0, encrypted.Length);
            }

        }

        public void ExecuteSQL(string s)
        {
            string db_connection = GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    cm.ExecuteNonQuery();
                    s = s.Trim();
                }
                cn.Close();
            }
        }
        public string ExecuteScalarSQL(string s)
        {
            string db_connection = GetDbConnection();
            string s1;
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    s1 = cm.ExecuteScalar().ToString();
                }
                cn.Close();
            }
            return s1;
        }
        public int Execute_count_SQL(string s)
        {
            string db_connection = GetDbConnection();
            int i = 0;
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    i = (int)cm.ExecuteNonQuery();
                }
                cn.Close();
            }
            return i;
        }

        public string Execute_one_SQL(string s)
        {
            string db_connection = GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    s = cm.ExecuteScalar().ToString();
                }
                cn.Close();
            }
            return s;
        }
        public Guid FindId(string querry)
        {
            //finds the Id of a record from a table matching the querry....  returns empty if not founnd or duplicate
            bool found = false; Guid id = new Guid(); id = Guid.Empty;
            using (SqlConnection cn = new SqlConnection(GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(querry, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            //we have a match
                            if (found)
                            {
                                //already found one.... !!! argh....
                                return Guid.Empty;
                            }
                            id = dr.GetGuid(0); found = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            if (found) return id;
            return Guid.Empty;
        }
        public Guid FindSQL(string querry)
        {
            bool found = false;
            Guid id = new Guid(); id = Guid.Empty;
            string db_connection = GetDbConnection();

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(querry, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            //we have a match
                            if (found)
                            {
                                //already found one.... !!! argh....
                                //MessageBox.Show("We have duplicate ...", "Find SQL Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            id = dr.GetGuid(0); found = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            if (found) return id;
            return Guid.Empty;
        }
        public string ConvertDateTime_SQL(DateTime date)
        {
            return "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
        }
    }

    public class Ethnicity
    {
        public int m_Id;
        public string m_EthnicOrigin;
        public string m_Description;

        public void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetInt32(0);
            m_EthnicOrigin = dr.GetString(1);
            m_Description = dr.GetString(2);
        }

    }
    public class EthnicityList
    {
        public System.Collections.Generic.List<Ethnicity> m_list = new System.Collections.Generic.List<Ethnicity>();

        public EthnicityList()
        {
            m_list.Clear();
            Load1("SELECT * FROM  tbl_List_EthnicOrigins ");
        }
        private void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Ethnicity e = new Ethnicity();
                            m_list.Add(e);
                            e.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }

    [Serializable]
    public class CalendarTime
    {
        public int hour;
        public int minute;

        public override string ToString()
        {
            return (hour.ToString() + ":" + minute.ToString());
        }
        public CalendarTime(string s)
        {
            hour = 0; minute = 0;
            if (s.Contains(":"))
            {
                int i = s.IndexOf(":");
                hour = System.Convert.ToInt32(s.Substring(0, i));
                minute = System.Convert.ToInt32(s.Substring(i + 1, 2));
            }
        }
    }
    [Serializable]
    public class CalendarEvent
    {
        public DateTime date;
        public CalendarTime start;
        public CalendarTime end;
        public string title;
        public string location;
        public string description;
        public System.Collections.Generic.List<string> Visibilities = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<string> Categories = new System.Collections.Generic.List<string>();
        public CalendarEvent()
        {

        }
        public void LoadXML(XmlNode n, DateTime d)
        {
            date = d;
            foreach (XmlNode x in n.ChildNodes)
            {
                switch (x.Name.ToLower())
                {
                    case "timed":
                        foreach (XmlNode x1 in x.ChildNodes)
                        {
                            switch (x1.Name.ToLower())
                            {
                                case "start":
                                    start = new CalendarTime(x1.FirstChild.Value);
                                    break;
                                case "end":
                                    end = new CalendarTime(x1.FirstChild.Value);
                                    break;
                            }
                        }
                        break;
                    case "start":
                        start = new CalendarTime(x.FirstChild.Value);
                        break;
                    case "end":
                        end = new CalendarTime(x.FirstChild.Value);
                        break;
                    case "title":
                        title = x.FirstChild.Value;
                        break;
                    case "description":
                        description = x.FirstChild.Value;
                        break;
                    case "location":
                        location = x.FirstChild.Value;
                        int i = 0;
                        while (location.Contains("DCGS Room"))
                        {
                            i = location.IndexOf("DCGS Room");
                            location = location.Substring(0, i) + location.Substring(i + 10);
                        }
                        break;
                    case "categories":
                        foreach (XmlNode x1 in x.ChildNodes)
                        {
                            Categories.Add(x1.FirstChild.Value);
                        }
                        break;
                    case "visibilities":
                        foreach (XmlNode x1 in x.ChildNodes)
                        {
                            Visibilities.Add(x1.FirstChild.Value);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

    }
    [Serializable]
    public class CalendarEventList
    {
        public List<CalendarEvent> EventList = new List<CalendarEvent>();
        public CalendarEventList() { }

        public void LoadXML_months(string XMLpath, int month_start, int no_months, int year)
        {
            XmlDocument doc = new XmlDocument();
            DirectoryInfo di = new DirectoryInfo(XMLpath);

            FileInfo[] fi = di.GetFiles();
            int y = 0; int m = 0; int d = 0;
            int m1 = year * 12 + month_start; int m2 = year * 12 + month_start + no_months - 1;
            int m0 = 0;
            foreach (FileInfo f in fi)
            {
                y = System.Convert.ToInt32(f.Name.Substring(0, 4));
                m = System.Convert.ToInt32(f.Name.Substring(4, 2));
                m0 = 12 * y + m;
                if ((m0 >= m1) && (m0 <= m2))
                {
                    doc.Load(f.FullName);
                    //date is set by file name....
                    d = System.Convert.ToInt32(f.Name.Substring(6, 2));

                    DateTime date = new DateTime(y, m, d);
                    XmlElement root = doc.DocumentElement;
                    foreach (XmlNode x in root.ChildNodes)
                    {
                        if (x.Name.ToLower() == "events")
                        {
                            foreach (XmlNode x1 in x.ChildNodes)
                            {
                                if (x1.Name.ToLower() == "event")
                                {
                                    CalendarEvent e = new CalendarEvent();
                                    e.LoadXML(x1, date);
                                    EventList.Add(e);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void LoadXML_day(string XMLpath, DateTime date)
        {
            XmlDocument doc = new XmlDocument();
            DirectoryInfo di = new DirectoryInfo(XMLpath);
            try
            {
                FileInfo[] fi = di.GetFiles();
                int y = 0; int m = 0; int d = 0;
                int m1 = 31 * (date.Year * 12 + date.Month) + date.Day;
                int m0 = 0;
                foreach (FileInfo f in fi)
                {
                    y = System.Convert.ToInt32(f.Name.Substring(0, 4));
                    m = System.Convert.ToInt32(f.Name.Substring(4, 2));
                    d = System.Convert.ToInt32(f.Name.Substring(6, 2));
                    m0 = 31 * (12 * y + m) + d;
                    if (m0 == m1)
                    {
                        doc.Load(f.FullName);
                        XmlElement root = doc.DocumentElement;
                        foreach (XmlNode x in root.ChildNodes)
                        {
                            if (x.Name.ToLower() == "events")
                            {
                                foreach (XmlNode x1 in x.ChildNodes)
                                {
                                    if (x1.Name.ToLower() == "event")
                                    {
                                        CalendarEvent e = new CalendarEvent();
                                        e.LoadXML(x1, date);
                                        EventList.Add(e);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
        }
        public string RenderEventsAsTable(bool ShowDate)
        {
            if (EventList.Count == 0) return "";
            string s = "<table  class=\"EventsTable\" >";
            s += "<tr>";
            if (ShowDate) s += "<th>Date</th>";
            s += "<th>Title</th><th>Location</th><th>Description</th><th>Category</th><th>Start</th><th>End</th></tr>";
            foreach (CalendarEvent ev in EventList)
            {
                //add this event...
                s += "<tr><td>";
                if (ShowDate) s += ev.date.ToShortDateString() + "</td><td>";
                if (ev.title != null) s += ev.title;
                s += "</td><td>";
                if (ev.location != null) s += ev.location.ToString();
                s += "</td><td>";
                if (ev.description != null) s += ev.description;
                s += "</td><td>";
                foreach (string s2 in ev.Categories) { s += s2 + "  "; }
                s += "</td><td>";
                if (ev.start != null) s += ev.start.ToString();
                s += "</td><td>";
                if (ev.end != null) s += ev.end.ToString();
                s += "</td></tr>";
            }
            s += "</table>";
            return s;
        }

    }
    [Serializable]
    public class ExamLinkComponent
    {
        public string m_OptionCode;
        public string m_ComponentCode;
        public bool m_flag;
        public Guid m_Id;
        public Guid m_OptionId;
        public Guid m_ComponentId;

        public bool LoadFromBaseData(string line, int JCQ_Version, string ExamBoardId)
        {
            if (JCQ_Version < 11) return false;
            m_OptionCode = line.Substring(2, 6);
            m_ComponentCode = line.Substring(8, 12);
            return true;
        }
        public void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetGuid(0);
            m_OptionId = dr.GetGuid(1);
            m_ComponentId = dr.GetGuid(2);
        }

        public void CreateNew(string Version)
        {
            string s = "";
            s = " INSERT INTO dbo.tbl_Exams_Link  ( OptionID, ComponentID ) ";
            s += " VALUES ('" + m_OptionId.ToString() + "' , '" + m_ComponentId.ToString() + "' )";
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }


    }
    public class ExamLinkComponent_List
    {
        public ArrayList m_list = new ArrayList();
        public ExamLinkComponent_List()
        {
        }

        public void LoadList_Component(Guid ComponentId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_list.Clear();
            string s = "SELECT * FROM tbl_Exams_Link WHERE (ComponentID='" + ComponentId.ToString() + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamLinkComponent p = new ExamLinkComponent();
                            m_list.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_Option(Guid OptionId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_list.Clear();
            string s = "SELECT * FROM tbl_Exams_Link WHERE (OptionID='" + OptionId.ToString() + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamLinkComponent p = new ExamLinkComponent();
                            m_list.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_OptionComponent(Guid OptionId, Guid ComponentId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_list.Clear();
            string s = "SELECT * FROM tbl_Exams_Link WHERE (OptionID='" + OptionId.ToString() + "' )  AND (ComponentID='" + ComponentId.ToString() + "' )  ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamLinkComponent p = new ExamLinkComponent();
                            m_list.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

    }

    public class ExamMarkBreakdown
    {
        public string OptionCode;
        public string Year;
        public string Season;
        public string FilePath;
        public Guid Id;
        public Guid ExamOptionId;
        public bool valid;

        public ExamMarkBreakdown()
        {
            ExamOptionId = Guid.Empty; FilePath = ""; valid = false;
        }

        public ExamMarkBreakdown(string optioncode, string year, string season)
        {
            OptionCode = optioncode; Season = season; Year = year; ExamOptionId = Guid.Empty;
            FilePath = ""; valid = false;
            Load();
        }

        public bool Load()
        {
            bool success = false;
            string s = "SELECT  Id, FilePath ";
            s += "FROM dbo.tbl_Exams_MarkBreakdowns   ";
            if (ExamOptionId == Guid.Empty)
            {
                s += " WHERE (ExamOptionCode = '" + OptionCode + "' )  ";
                s += " AND (ExamYear = '" + Year + "' ) ";
                s += " AND (ExamSeason = '" + Season + "' ) ";
            }
            else
            {
                s += " WHERE (ExamOptionId = '" + ExamOptionId.ToString() + "' )  ";
            }
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            FilePath = dr.GetString(1); success = true;
                        }
                    }
                }
                cn.Close();
            }
            valid = success;
            return success;
        }
    }

    public class CSVWriter
    {
        //write csv file to response stream for excel
        public void OutputToCSV(System.Web.HttpResponse response, ArrayList a1, int max_x, int max_y, string title)
        {
            StringWriter sw = new StringWriter();
            string s = "";
            response.ClearContent();
            response.AddHeader("content-disposition", "attachment;filename=" + title + ".csv");
            response.ContentType = "application/vnd.ms-excel";
            //response.ContentType = "application / vnd.google - apps.spreadsheet";
            for (int y = 0; y <= max_y; y++)
            {
                s = "";
                for (int x = 0; x <= max_x; x++)
                {
                    foreach (Grid_Element gr1 in a1)
                    {
                        if ((gr1.m_x == x) && (gr1.m_y == y)) { s += gr1.m_value + ","; break; }
                    }
                }
                sw.WriteLine(s);
            }
            response.Write(sw.ToString());
            response.End();
        }
    }

    public class ExcelXmlWriter
    {
        ///
        ///No longer used!
        ///

        private static string OFFICE_NAMESPACE = "urn:schemas-microsoft-com:office:office";
        private static string EXCEL_NAMESPACE = "urn:schemas-microsoft-com:office:excel";
        private static string SPREADSHEET_NAMESPACE = "urn:schemas-microsoft-com:office:spreadsheet";
        //private static string HTML_NAMESPACE = "http:////www.w3.org/TR/REC-html40";

        public void OutputToExcel(System.Web.HttpResponse response, ArrayList a1, int max_x, int max_y, string title)
        {


            StringWriter sw = new StringWriter();

            sw.WriteLine("\"First Name\",\"Last Name\",\"DOB\",\"Email\"");

            response.ClearContent();
            response.AddHeader("content-disposition", "attachment;filename=Exported_Users.csv");
            response.ContentType = "application/vnd.ms-excel";

            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                                           "fred",
                                           "line.LastName",
                                           "line.Dob",
                                           "line.Email"));
            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                               "fred",
                               "line.LastName",
                               "line.Dob",
                               "line.Email"));

            response.Write(sw.ToString());

            response.End();
            return;

            response.ContentType = "text/xml";
            XmlWriter l_xw = CreateExcelXmlDocument(response.OutputStream);
            l_xw.WriteStartElement("ExcelWorkbook", EXCEL_NAMESPACE);
            l_xw.WriteElementString("WindowHeight", "8445");
            l_xw.WriteElementString("WindowWidth", "15195");
            l_xw.WriteElementString("WindowTopX", "0");
            l_xw.WriteElementString("WindowTopY", "45");
            l_xw.WriteElementString("ProtectStructure", "False");
            l_xw.WriteElementString("ProtectWindows", "False");
            l_xw.WriteEndElement();
            l_xw.WriteStartElement("Styles");
            l_xw.WriteStartElement("Style");
            l_xw.WriteAttributeString("ss", "ID", SPREADSHEET_NAMESPACE, "Default");
            l_xw.WriteAttributeString("ss", "Name", SPREADSHEET_NAMESPACE, "Normal");
            l_xw.WriteStartElement("Alignment");
            l_xw.WriteAttributeString("ss", "Vertical", SPREADSHEET_NAMESPACE, "Bottom");
            l_xw.WriteEndElement();
            l_xw.WriteElementString("Borders", "");
            l_xw.WriteElementString("Font", "");
            l_xw.WriteElementString("Interior", "");
            l_xw.WriteElementString("NumberFormat", "");
            l_xw.WriteElementString("Protection", "");
            l_xw.WriteEndElement();

            l_xw.WriteStartElement("Style");
            l_xw.WriteAttributeString("ss", "ID", SPREADSHEET_NAMESPACE, "HeaderStyle");
            l_xw.WriteStartElement("Font");
            l_xw.WriteAttributeString("x", "Family", EXCEL_NAMESPACE, "Swiss");
            l_xw.WriteAttributeString("ss", "Bold", SPREADSHEET_NAMESPACE, "1");
            l_xw.WriteEndElement();
            l_xw.WriteEndElement();

            l_xw.WriteEndElement();

            l_xw.WriteStartElement("Worksheet");
            l_xw.WriteAttributeString("ss", "Name", SPREADSHEET_NAMESPACE, title);

            l_xw.WriteStartElement("Table");
            int mx = 2 + max_x; int my = 2 + max_y;
            l_xw.WriteAttributeString("ss", "ExpandedColumnCount", SPREADSHEET_NAMESPACE, mx.ToString());
            l_xw.WriteAttributeString("ss", "ExpandedRowCount", SPREADSHEET_NAMESPACE, my.ToString());
            l_xw.WriteAttributeString("x", "FullColumns", SPREADSHEET_NAMESPACE, mx.ToString());
            l_xw.WriteAttributeString("x", "FullRows", SPREADSHEET_NAMESPACE, my.ToString());

            for (int y = 0; y <= max_y; y++)
            {
                l_xw.WriteStartElement("Row");
                for (int x = 0; x <= max_x; x++)
                {
                    l_xw.WriteStartElement("Cell");
                    l_xw.WriteStartElement("Data");
                    l_xw.WriteAttributeString("ss", "Type", SPREADSHEET_NAMESPACE, "String");
                    foreach (Grid_Element gr1 in a1)
                    {
                        if ((gr1.m_x == x) && (gr1.m_y == y)) { l_xw.WriteString(gr1.m_value); break; }
                    }
                    l_xw.WriteEndElement();
                    l_xw.WriteEndElement();
                }
                l_xw.WriteEndElement();
            }
            l_xw.WriteEndElement();
            l_xw.WriteEndElement();
            l_xw.WriteEndDocument();
            l_xw.Flush();
            l_xw.Close();
            response.End();
        }
        private XmlWriter CreateExcelXmlDocument(System.IO.Stream stream)
        {

            XmlTextWriter l_xw = new System.Xml.XmlTextWriter(stream, System.Text.Encoding.ASCII);
            l_xw.Namespaces = true;
            l_xw.Formatting = System.Xml.Formatting.Indented;
            l_xw.Indentation = 1;
            l_xw.WriteStartDocument();
            l_xw.WriteProcessingInstruction("mso-application", "progid='Excel.Sheet'");
            l_xw.WriteStartElement("Workbook", SPREADSHEET_NAMESPACE);
            l_xw.WriteStartElement("DocumentProperties", OFFICE_NAMESPACE);
            l_xw.WriteStartElement("Author", OFFICE_NAMESPACE);
            l_xw.WriteString(this.GetType().Assembly.FullName);
            l_xw.WriteEndElement();
            l_xw.WriteStartElement("LastAuthor");
            if (Environment.UserDomainName.ToString() != "") l_xw.WriteString(Environment.UserDomainName + "\\" + Environment.UserName);
            else l_xw.WriteString(Environment.UserName);

            l_xw.WriteEndElement();
            l_xw.WriteStartElement("Created");
            l_xw.WriteString(DateTime.Now.ToShortDateString() + "T" + DateTime.Now.ToShortTimeString() + "Z");
            l_xw.WriteEndElement();
            l_xw.WriteStartElement("Company");
            l_xw.WriteString("DCGS");
            l_xw.WriteEndElement();
            l_xw.WriteStartElement("Version");
            l_xw.WriteString("11");
            l_xw.WriteEndElement();
            l_xw.WriteEndElement();
            l_xw.WriteStartElement("OfficeDocumentSettings", OFFICE_NAMESPACE);
            l_xw.WriteElementString("DownloadComponents", "");
            l_xw.WriteElementString("LocationOfComponents", "");
            l_xw.WriteEndElement();
            return l_xw;
        }

    }
    public class GobalData
    {
        //class to hold various lists that we don't want to keep on loading
        public SimpleStudentList SimpleStudentList1 = new SimpleStudentList();
        public PeriodList PeriodList1 = new PeriodList();
        public DayList DayList1 = new DayList();
        public PupilPeriodList PupilPeriodList1;
        public StudentGroupMembershipList StudentGroupMembership1;
        public CourseList CourseList1 = new CourseList(0);
        public StaffList stafflist1;
        public RoomList roomlist1;


        public GobalData()
        {
            PupilPeriodList1 = null;
            StudentGroupMembership1 = null;
            stafflist1 = null;
        }

        public SimpleStaff FindStaff(Guid StaffId)
        {
            LoadStaffList();
            foreach (SimpleStaff s in stafflist1.m_stafflist)
            {
                if (s.m_StaffId == StaffId)
                {
                    return s;
                }
            }
            //might be not workiing here anymore ... try db load
            SimpleStaff ss1 = new SimpleStaff(StaffId);
            return ss1;
        }

        public SimpleRoom FindRoom(Guid RoomId)
        {
            LoadRoomList();
            foreach (SimpleRoom s in roomlist1.m_roomlist)
            {
                if (s.m_RoomID == RoomId)
                {
                    return s;
                }
            }
            SimpleRoom sr1 = new SimpleRoom(RoomId);
            return sr1;
        }

        public void LoadStaffList()
        {
            if (stafflist1 != null) return;
            stafflist1 = new StaffList();
            stafflist1.LoadList(DateTime.Now, false);
        }

        public void LoadRoomList()
        {
            if (roomlist1 != null) return;
            roomlist1 = new RoomList();
            roomlist1.LoadList();
        }

        public void LoadPupilPeriodList1()
        {
            if (PupilPeriodList1 != null) return;
            PupilPeriodList1 = new PupilPeriodList();
            PupilPeriodList1.LoadList("", "", false);
        }

        public void LoadPupilPeriodList1(DateTime date)
        {
            if (PupilPeriodList1 != null) return;
            PupilPeriodList1 = new PupilPeriodList();
            PupilPeriodList1.LoadList("", "", false, date);
        }

        public void LoadStudentGroupMembership1()
        {
            if (StudentGroupMembership1 != null) return;
            StudentGroupMembership1 = new StudentGroupMembershipList();
            StudentGroupMembership1.LoadList_All(DateTime.Now);
        }

        public DateTime EndofYearDate()
        {
            //return the date of teh end of this current year....  31st july
            DateTime t = DateTime.Now;
            int y = t.Year; if (t.Month > 7) y++;
            DateTime t1 = new DateTime(y, 7, 31, 0, 0, 0);
            return t1;
        }

    }
    public class Grid_Element
    {
        public int m_x;
        public int m_y;
        public string m_value;
        public Grid_Element() { }
        public Grid_Element(int x, int y, string value) { m_x = x; m_y = y; m_value = value; }
    }
    [Serializable]
    public class Group
    {
        public Guid _GroupID;
        public Guid _CourseID;
        public string _GroupCode;
        public string _GroupName;
        public DateTime _StartDate;
        public DateTime _EndDate;
        public bool _GroupPrimaryAdministrative = false; //true if reg group  default value false
        public int _GroupRegistrationType = 3;// depreciated... was used by reg software default value 3
        public int _GroupRegistrationYear = 0;// allow nulls
        public Guid _GroupManagedBy = Guid.Empty;// used to show ownership in early code release... allow nulls.
        public bool _valid = false;

        public void Load(Guid GroupId)
        {
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += "FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupId = '" + GroupId.ToString() + "' ) ";
            Load1(s);
        }

        public void Load(string GroupCode, DateTime date)
        {
            string date_s = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += "FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupCode = '" + GroupCode + "' ) ";
            s += " AND (GroupValidFrom < " + date_s + " ) AND (GroupValidUntil > " + date_s + " )";
            Load1(s);
        }

        public void Load(string GroupCode, Guid CourseId)
        {
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += "FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupCode = '" + GroupCode + "' ) ";
            s += " AND (CourseId = '" + CourseId.ToString() + "')";
            Load1(s);
        }

        public void Load1(string s)
        {
            _valid = false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
                    }
                }
                cn.Close();
            }

        }

        public void Hydrate(SqlDataReader dr)
        {
            _GroupID = dr.GetGuid(0);
            _GroupCode = dr.GetString(1);
            _GroupName = dr.GetString(2);
            _StartDate = dr.GetDateTime(3);
            _EndDate = dr.GetDateTime(4);
            if (!dr.IsDBNull(5)) _CourseID = dr.GetGuid(5);
            _GroupPrimaryAdministrative = dr.GetBoolean(6);
            _GroupRegistrationType = dr.GetByte(7);
            if (!dr.IsDBNull(8)) _GroupRegistrationYear = dr.GetByte(8);
            if (!dr.IsDBNull(9)) _GroupManagedBy = dr.GetGuid(9);
            _valid = true;
        }

        public void Save()
        {
            if (_StartDate > _EndDate) throw new Exception("Dates don't match");
            Encode en = new Encode(); string s = "";
            if ((_GroupID == null) || (_GroupID == Guid.Empty))
            {
                //make a new one.....
                Guid g1 = new Guid(); g1 = Guid.NewGuid();
                s = "INSERT INTO tbl_Core_Groups ";
                s += "(GroupId, GroupName, GroupValidFrom, GroupValidUntil, GroupCode, CourseId, GroupRegistrationType, GroupPrimaryAdministrative, ";
                if (_GroupRegistrationYear != 0) s += " GroupRegistrationYear ,";
                if (_GroupManagedBy != Guid.Empty) s += " GroupManagedBy , ";
                s += " Version )"; //version as temp
                s += "VALUES ('" + g1.ToString() + "' , '" + _GroupName + "' , ";
                s += " CONVERT(DATETIME, '" + _StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s += " CONVERT(DATETIME, '" + _EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s += " '" + _GroupCode + "' , ";
                s += "'" + _CourseID.ToString() + "' , ";
                s += "'" + _GroupRegistrationType.ToString() + "' , ";
                s += "'" + _GroupPrimaryAdministrative.ToString() + "' , ";
                if (_GroupRegistrationYear != 0) s += " '" + _GroupRegistrationYear.ToString() + "' , ";
                if (_GroupManagedBy != Guid.Empty) s += " '" + _GroupManagedBy.ToString() + "' , ";
                s += " '9' "; //version for New Dawn
                s += ")";
                en.ExecuteSQL(s);
                //now need to get the id....
                _GroupID = g1;
            }
            else
            {
                s = " UPDATE tbl_Core_Groups SET ";
                s += " GroupName = '" + _GroupName + "' ";
                s += ", GroupCode = '" + _GroupCode + "' ";
                s += ", CourseId = '" + _CourseID.ToString() + "' ";
                s += ", GroupValidFrom = CONVERT(DATETIME, '" + _StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                s += ", GroupValidUntil = CONVERT(DATETIME, '" + _EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                s += ", GroupRegistrationType = '" + _GroupRegistrationType.ToString() + "' ";
                s += ", GroupPrimaryAdministrative = '" + _GroupPrimaryAdministrative.ToString() + "' ";
                if (_GroupRegistrationYear != 0) s += ", GroupRegistrationYear ='" + _GroupRegistrationYear.ToString() + "' ";
                if (_GroupManagedBy != Guid.Empty) s += ", GroupManagedBy ='" + _GroupManagedBy.ToString() + "' ";
                s += " WHERE (GroupId = '" + _GroupID.ToString() + "' ) ";
                en.ExecuteSQL(s);
            }
        }

        public bool Delete()
        {
            if ((_GroupID == null) || (_GroupID == Guid.Empty)) return false;
            Encode en = new Encode(); string s = "";
            s = en.ExecuteScalarSQL("SELECT COUNT(*) FROM [tbl_Core_Groups_Groups] WHERE (ParentId ='" + _GroupID.ToString() + "' ) ");
            if (s != "0") return false;
            s = "DELETE FROM tbl_Core_Groups_Groups WHERE (ChildId ='" + _GroupID.ToString() + "' ) ";
            int n = en.Execute_count_SQL(s);
            //ought to delete any group memberships.....

            s = "DELETE FROM tbl_Core_Student_Groups WHERE  (GroupId = '" + _GroupID.ToString() + "' )";
            n = en.Execute_count_SQL(s);

            //now to delete scheduled periods we have to delete the validity periods....
            ScheduledPeriodRawList spl1 = new ScheduledPeriodRawList();
            spl1.Load_for_Group(_GroupID);
            foreach (ScheduledPeriodRaw p in spl1.m_list)
            {
                s = "DELETE FROM dbo.tbl_Core_ScheduledPeriodValidity ";
                s += " WHERE (dbo.tbl_Core_ScheduledPeriodValidity.ScheduledPeriodId  = '" + p.Id.ToString() + "') ";
                n = en.Execute_count_SQL(s);
            }
            s = "DELETE FROM tbl_Core_ScheduledPeriods WHERE (GroupId = '" + _GroupID.ToString() + "' )";
            en.ExecuteSQL(s);
            s = "DELETE FROM tbl_Core_Groups WHERE (GroupId = '" + _GroupID.ToString() + "' ) ";
            en.ExecuteSQL(s);
            return true;
        }
        #region QanStuff

        public string FindQAN_Disc_Code()
        {
            string DiscCode = "";
            string s = "SELECT * FROM dbo.tbl_Core_Group_QANs WHERE ";
            s += " (GroupId = '" + _GroupID.ToString() + "' ) ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DiscCode = dr.GetString(3);
                        }
                    }
                }
                cn.Close();
            }
            return DiscCode;
        }

        public string FindQAN_Code()
        {
            string QANCode = "";
            string s = "SELECT * FROM dbo.tbl_Core_Group_QANs WHERE ";
            s += " (GroupId = '" + _GroupID.ToString() + "' ) ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            QANCode = dr.GetString(2);
                        }
                    }
                }
                cn.Close();
            }
            return QANCode;
        }

        public void WriteQAN_Data(string Disc_Code, string QAN, DateTime AccEndDate)
        {
            //if record exists we need to uopdate
            if (FindQAN_Disc_Code() == Disc_Code)
            {
                string s = "UPDATE dbo.tbl_Core_Group_QANs ";
                s += " SET Qan = '" + QAN + "' ";
                s += " , DiscCode = '" + Disc_Code + "' ";
                s += " , AccEndDate = CONVERT(DATETIME, '" + AccEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += " , LAEndOffsetInYears='0' ";
                s += " , LastUpdated=CONVERT(DATETIME, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += " WHERE (GroupId = '" + _GroupID.ToString() + "' ) ";
                Encode en = new Encode();
                en.ExecuteSQL(s);
            }
            else
            {
                string s = "INSERT INTO dbo.tbl_Core_Group_QANs ";
                s += " (GroupId, Qan, DiscCode, AccEndDate, LAEndOffsetInYears ,LastUpdated ) ";
                s += " VALUES ( '" + _GroupID.ToString() + "' , '" + QAN + "' , '" + Disc_Code + "' , ";
                s += " CONVERT(DATETIME, '" + AccEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ,";
                if (_GroupCode.StartsWith("12")) s += " '0', "; else s += " '0', ";
                s += " CONVERT(DATETIME, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
                Encode en = new Encode();
                en.ExecuteSQL(s);
            }
        }
        #endregion

    }
    public class GroupList_SL
    {
        public ArrayList _groups = new ArrayList();
        public GroupList_SL(string staffcode)
        {
            _groups.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //get alist of all courses.... 

            string s = "SELECT  dbo.tbl_Core_Groups.GroupId, dbo.tbl_Core_Groups.GroupCode, ";
            s += " dbo.tbl_Core_Groups.GroupName, dbo.tbl_Core_Groups.GroupValidFrom, ";
            s += " dbo.tbl_Core_Groups.GroupValidUntil, dbo.tbl_Core_Groups.CourseId ,";
            s += " dbo.tbl_Core_Groups.GroupPrimaryAdministrative, dbo.tbl_Core_Groups.GroupRegistrationType, dbo.tbl_Core_Groups.GroupRegistrationYear,  dbo.tbl_Core_Groups.GroupManagedBy ";
            s += " FROM         dbo.tbl_Core_Groups ";
            s += " INNER JOIN dbo.tbl_Core_Courses ON dbo.tbl_Core_Groups.CourseId = dbo.tbl_Core_Courses.CourseId ";
            s += " INNER JOIN dbo.tbl_Core_OrganisationalUnits ON dbo.tbl_Core_Courses.OrganisationalUnitId = dbo.tbl_Core_OrganisationalUnits.OrganisationalUnitId ";
            s += " INNER JOIN dbo.tbl_Core_Staff ON dbo.tbl_Core_OrganisationalUnits.OrganisationalUnitHead = dbo.tbl_Core_Staff.StaffId ";
            if (staffcode.Length > 0) s += " WHERE (dbo.tbl_Core_Staff.StaffCode = '" + staffcode + "')  ";
            s += " ORDER BY dbo.tbl_Core_Groups.GroupName ASC";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Group g = new Group();
                            _groups.Add(g);
                            g.Hydrate(dr);
                            g._valid = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class GroupLink
    {
        public Guid Id = Guid.Empty;
        public Guid ParentId;
        public Guid ChildId;

        public void Delete()
        {
            string s = "";
            if (Id == Guid.Empty)
            {
                //do it by the parent n child
                if ((ParentId == Guid.Empty) || (ChildId == Guid.Empty)) return;
                else
                {
                    s = "DELETE FROM tbl_Core_Groups_Groups WHERE ( ParentId = '" + ParentId.ToString() + "' ) ";
                    s += " AND (ChildId = '" + ChildId.ToString() + "' ) ";
                }

            }
            else
            {
                s = s = "DELETE FROM tbl_Core_Groups_Groups WHERE ( Id = '" + Id.ToString() + "' ) ";
            }
            Encode en = new Encode();
            int n = en.Execute_count_SQL(s);
        }

        public void Save()
        {
            Encode en = new Encode(); string s = "";
            if ((Id == null) || (Id == Guid.Empty))
            {
                //make a new one.....
                Id = Guid.NewGuid();
                s = "INSERT INTO tbl_Core_Groups_Groups ";
                s += "(Id, ParentId, ChildId )";
                s += " VALUES ('" + Id.ToString() + "', '" + ParentId.ToString() + "' , ";
                s += " '" + ChildId.ToString() + "' ) ";
                en.ExecuteSQL(s);
            }
            else
            {
                s = " UPDATE tbl_Core_Groups_Groups SET ";
                s += " ParentId = '" + ParentId.ToString() + "' ";
                s += ", ChildId = '" + ChildId.ToString() + "' ";
                s += " WHERE (Id = '" + Id.ToString() + "' ) ";
                en.ExecuteSQL(s);
            }

        }

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            ParentId = dr.GetGuid(1);
            ChildId = dr.GetGuid(2);
        }

    }
    public class GroupLinkList
    {
        public ArrayList links = new ArrayList();

        public GroupLinkList()
        {
        }
        public void LoadAll()
        {
            links.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //get alist of all courses.... 
            string s = "SELECT  Id, ParentId, ChildId ";
            s += " FROM dbo.tbl_Core_Groups_Groups   ";
            GroupListLoad(s);
        }

        public GroupLinkList(DateTime time)
        {
            LoadList(time);
        }
        public void LoadList(DateTime time)
        {
            //going to load all the links relevant to current groups
            //get all valid groups at this time....
            links.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //get alist of all courses.... 
            string s = "SELECT  Id, ParentId, ChildId ";
            s += " FROM dbo.tbl_Core_Groups_Groups   ";
            s += " INNER JOIN dbo.tbl_Core_Groups  ON dbo.tbl_Core_Groups_Groups.ParentId=dbo.tbl_Core_Groups.GroupId  ";
            s += " WHERE (dbo.tbl_Core_Groups.GroupValidFrom < CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102) )";
            s += " AND (dbo.tbl_Core_Groups.GroupValidUntil > CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102))";
            s += " ORDER BY dbo.tbl_Core_Groups.GroupName ASC ";
            GroupListLoad(s);
        }
        private void GroupListLoad(string s)
        {
            links.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            GroupLink g = new GroupLink();
                            links.Add(g);
                            g.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class GroupList
    {
        public ArrayList _groups = new ArrayList();
        public enum GroupListOrder { GroupName, GroupId };
        private void GroupListLoad(string s)
        {
            _groups.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Group g = new Group();
                            _groups.Add(g);
                            g.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public GroupList()
        {
            _groups.Clear();
        }

        public void LoadStaff(string staff_code, DateTime date, GroupListOrder orderby)
        {
            string date_s = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT  DISTINCT dbo.tbl_Core_Groups.GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += " FROM  dbo.tbl_Core_Groups  ";
            s += " INNER JOIN dbo.tbl_Core_ScheduledPeriods ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_ScheduledPeriods.GroupId ";
            s += " INNER JOIN dbo.tbl_Core_ScheduledPeriodValidity ON dbo.tbl_Core_ScheduledPeriods.ScheduledPeriodId = dbo.tbl_Core_ScheduledPeriodValidity.ScheduledPeriodId ";
            s += " INNER JOIN dbo.tbl_Core_Staff ON dbo.tbl_Core_ScheduledPeriods.StaffId = dbo.tbl_Core_Staff.StaffId ";
            s += " WHERE (dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart < " + date_s + ") ";
            s += " AND (dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd > " + date_s + ") ";
            s += " AND (dbo.tbl_Core_Groups.GroupValidUntil > " + date_s + ") ";
            s += " AND (dbo.tbl_Core_Groups.GroupValidFrom < " + date_s + ") ";
            s += " AND (dbo.tbl_Core_Staff.StaffCode = '" + staff_code + "') ";
            switch (orderby)
            {
                case GroupListOrder.GroupName:
                    s += " ORDER BY GroupName ASC";
                    break;
                case GroupListOrder.GroupId:
                    s += " ORDER BY GroupId ASC";
                    break;
                default:
                    break;
            }
            GroupListLoad(s);
        }

        public GroupList(string subject_Guid, DateTime date)
        {
            string date_s = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            if (subject_Guid == "") subject_Guid = null;//get alist of all courses.... 
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += "FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupValidFrom < " + date_s + " ) AND (GroupValidUntil > " + date_s + " )";
            if (subject_Guid != null) s += " AND ( CourseId = '" + subject_Guid + "') ";
            s += " ORDER BY GroupName ASC";
            GroupListLoad(s);
        }

        public GroupList(string subject_Guid, string date)
        {
            if (subject_Guid == "") subject_Guid = null;//get alist of all courses.... 
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += "FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupValidFrom < " + date + " ) AND (GroupValidUntil > " + date + " )";
            if (subject_Guid != null) s += " AND ( CourseId = '" + subject_Guid + "') ";
            s += " ORDER BY GroupName ASC";
            GroupListLoad(s);
        }
        public GroupList(string subject_Guid)
        {
            if (subject_Guid == "") subject_Guid = null;//get alist of all courses.... 
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += "FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupValidFrom < { fn NOW() }) AND (GroupValidUntil > { fn NOW() })";
            if (subject_Guid != null) s += " AND ( CourseId = '" + subject_Guid + "') ";
            s += " ORDER BY GroupName ASC";
            GroupListLoad(s);
        }
        public GroupList(DateTime time, GroupListOrder orderby)
        {
            //get all valid groups at this time....
            _groups.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += " FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupValidFrom < CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102) )";
            s += " AND (GroupValidUntil > CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102))";
            switch (orderby)
            {
                case GroupListOrder.GroupName:
                    s += " ORDER BY GroupName ASC";
                    break;
                case GroupListOrder.GroupId:
                    s += " ORDER BY GroupId ASC";
                    break;
                default:
                    break;
            }
            GroupListLoad(s);
        }

        public void LoadList_Future(DateTime date)
        {
            //load all groups with valid from in the future....
            _groups.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += " FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupValidFrom > CONVERT(DATETIME,'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',102) )";

            GroupListLoad(s);
        }
        public void LoadList_Student(Guid StudentId, DateTime date)
        {
            string date_s = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT  dbo.tbl_Core_Groups.GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += " FROM         dbo.tbl_Core_Groups INNER JOIN ";
            s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId ";
            s += "WHERE (StudentID = '" + StudentId.ToString() + "')";
            s += " AND  (dbo.tbl_Core_Groups.GroupValidFrom < " + date_s + ") AND (dbo.tbl_Core_Groups.GroupValidUntil > " + date_s + ")";
            s += " AND  (dbo.tbl_Core_Student_Groups.MemberUntil > " + date_s + ") AND (dbo.tbl_Core_Student_Groups.MemberFrom < " + date_s + ") ";
            GroupListLoad(s);

        }
        public void LoadList_NonNewDawnOnly(DateTime time, GroupListOrder orderby)
        {
            _groups.Clear(); Cerval_Globals globals = new Cerval_Globals();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += " FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupValidFrom < CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102) )";
            s += " AND (GroupValidUntil > CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102))";
            foreach (Guid g in Cerval_Globals.NewStructureCourses)
            {
                s += " AND ( CourseId<>'" + g.ToString() + "' ) ";
            }

            switch (orderby)
            {
                case GroupListOrder.GroupName:
                    s += " ORDER BY GroupName ASC";
                    break;
                case GroupListOrder.GroupId:
                    s += " ORDER BY GroupId ASC";
                    break;
                default:
                    break;
            }
            GroupListLoad(s);

        }

        public void LoadList_StaffPrivateGroups(string staff_code, GroupListOrder orderby)
        {
            //going to get groups from new dawn structure...
            //whose great grand parent is dcgs root (ie level 3) 
            // and whose parent staffGroups
            //this will give the structure group from the surriculum year and staff groups.

            string sg = Cerval_Globals.DCGSroot.ToString();
            string SelectL1 = "( SELECT ChildId FROM tbl_Core_Groups_Groups WHERE (ParentId = '" + sg + "')) ";
            string SelectL2 = "( SELECT ChildId FROM tbl_Core_Groups_Groups ";
            SelectL2 += "WHERE ( ParentId IN  " + SelectL1 + ") )";


            _groups.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += " FROM  tbl_Core_Groups  ";
            s += " INNER JOIN  tbl_Core_Groups_Groups ON tbl_Core_Groups_Groups.ChildId = tbl_Core_Groups.GroupId ";
            s += " WHERE ( tbl_Core_Groups_Groups.ParentId IN  " + SelectL2 + " ) AND (tbl_Core_Groups.GroupCode LIKE '" + staff_code + "%')   ";


            switch (orderby)
            {
                case GroupListOrder.GroupName:
                    s += " ORDER BY GroupName ASC";
                    break;
                case GroupListOrder.GroupId:
                    s += " ORDER BY GroupId ASC";
                    break;
                default:
                    break;
            }
            GroupListLoad(s);

        }

        public void LoadList(DateTime time, GroupListOrder orderby)
        {
            _groups.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += " FROM dbo.tbl_Core_Groups   ";
            s += " WHERE (GroupValidFrom < CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102) )";
            s += " AND (GroupValidUntil > CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102))";
            switch (orderby)
            {
                case GroupListOrder.GroupName:
                    s += " ORDER BY GroupName ASC";
                    break;
                case GroupListOrder.GroupId:
                    s += " ORDER BY GroupId ASC";
                    break;
                default:
                    break;
            }
            GroupListLoad(s);
        }
        public void FindAllChildren(Guid ParentId, DateTime time)
        {
            string t1 = "CONVERT(DATETIME,'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "',102)";
            string s = "SELECT  GroupId, GroupCode, GroupName, GroupValidFrom, GroupValidUntil, CourseId ";
            s += ", GroupPrimaryAdministrative, GroupRegistrationType ,  GroupRegistrationYear , GroupManagedBy ";
            s += "FROM dbo.tbl_Core_Groups INNER JOIN  ";
            s += "dbo.tbl_Core_Groups_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Groups_Groups.ChildId ";
            s += " WHERE (dbo.tbl_Core_Groups_Groups.ParentId = '" + ParentId.ToString() + "') ";
            s += " AND (GroupValidFrom < " + t1 + " ) AND (GroupValidUntil >" + t1 + ") ";
            s += " ORDER BY GroupCode ASC";
            GroupListLoad(s);
        }

        public void FindAllChildren_fromMemory(Guid ParentId, ref GroupLinkList links, ref GroupList groups)
        {
            _groups.Clear();
            foreach (GroupLink g in links.links)
            {
                if (g.ParentId == ParentId)
                {
                    foreach (Group g1 in groups._groups)
                    {
                        if (g.ChildId == g1._GroupID)
                        {
                            _groups.Add(g1);
                            break;
                        }
                    }
                }
            }
        }

        public void FindAllDescendents_fromMemory(Guid ParentId, ref GroupLinkList links, ref GroupList groups)
        {
            _groups.Clear();
            FindAllDescendents_fromMemoryX(ParentId, ref links, ref groups);
        }

        private void FindAllDescendents_fromMemoryX(Guid ParentId, ref GroupLinkList links, ref GroupList groups)
        {
            foreach (GroupLink g in links.links)
            {
                if (g.ParentId == ParentId)
                {
                    foreach (Group g1 in groups._groups)
                    {
                        if (g.ChildId == g1._GroupID)
                        {
                            _groups.Add(g1);
                            FindAllDescendents_fromMemoryX(g1._GroupID, ref links, ref groups);
                            break;
                        }
                    }
                }
            }
        }
    }




    public class IndicatorSkill
    {
        public string SkillText;
        public Guid SkillId;
        public bool SkillinUse;
        public Guid CourseId;

        public void Hydrate(SqlDataReader dr)
        {
            SkillId = dr.GetGuid(0);
            SkillText = dr.GetString(1);
            CourseId = dr.GetGuid(2);
            SkillinUse = dr.GetBoolean(3);
        }
    }

    public class IndicatorSkillList
    {
        public List<IndicatorSkill> m_list = new List<IndicatorSkill>();

        public void LoadList(Guid CourseID)
        {
            string s = " SELECT dbo.tbl_Core_Assessment_Skills.SkillID, dbo.tbl_Core_Assessment_Skills.SkillText, dbo.tbl_Core_Assessment_Skills_Courses.CourseID, dbo.tbl_Core_Assessment_Skills.SkillInUse";
            s += " FROM  dbo.tbl_Core_Assessment_Skills INNER JOIN";
            s += " dbo.tbl_Core_Assessment_Skills_Courses ON dbo.tbl_Core_Assessment_Skills.SkillID = dbo.tbl_Core_Assessment_Skills_Courses.SkillID";
            s += " WHERE(dbo.tbl_Core_Assessment_Skills_Courses.CourseID = '" + CourseID.ToString() + "') AND(dbo.tbl_Core_Assessment_Skills.SkillInUse = 1)";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            IndicatorSkill m = new IndicatorSkill();
                            m.Hydrate(dr);
                            m_list.Add(m);
                        }
                    }
                }
                cn.Close();
            }
        }
    }

    public class IndicatorSavePoint
    {
        public Guid Id;
        public Guid StudentId;
        public Guid StaffId;
        public Guid SkillId;
        public decimal Value;
        public DateTime Date;
        public Guid CourseId;

        public IndicatorSavePoint()
        {
        }


        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            StudentId = dr.GetGuid(1);
            StaffId = dr.GetGuid(2);
            SkillId = dr.GetGuid(3);
            Value = dr.GetDecimal(4);
            Date = dr.GetDateTime(5);
            if (!dr.IsDBNull(7)) CourseId = dr.GetGuid(7); else CourseId = Guid.Empty;
        }

        public void Save()
        {
            Encode en = new Encode(); string s = "";
            Id = Guid.NewGuid();
            s = "INSERT INTO tbl_Core_Assessment_IndicatorSavePoints ";
            s += "(IndicatorSavePointID, StudentID, StaffID, SkillID, IndicatorValue, DateCreated, CourseId  )";
            s += "VALUES ('" + Id.ToString() + "' , '" + StudentId.ToString() + "' , '";
            s += StaffId.ToString() + "' , '" + SkillId.ToString() + "' , '";
            s += Value.ToString() + "' , ";
            s += " CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
            s += "'" + CourseId.ToString() + "'  )";
            en.ExecuteSQL(s);
        }

    }
    public class PersonImage
    {
        public Guid m_PersonId;
        public System.Drawing.Bitmap m_Bitmap;
        public bool m_valid;

        public PersonImage(Guid person)
        {
            m_PersonId = person; m_valid = false;
            string s = " SELECT * FROM dbo.tbl_Core_PeopleImages ";
            s += " WHERE (PersonId = '" + m_PersonId.ToString() + "' ) ";
            s += " ORDER BY ImageDate DESC";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        try
                        {
                            if (dr.Read())
                            {
                                System.Data.SqlTypes.SqlBinary b1 = new System.Data.SqlTypes.SqlBinary();
                                b1 = dr.GetSqlBinary(2);
                                byte[] b3 = new byte[b1.Length];
                                b3 = (byte[])b1;
                                MemoryStream ms1 = new MemoryStream(b3);
                                m_Bitmap = new System.Drawing.Bitmap(ms1);
                                m_valid = true;
                            }
                            dr.Close();
                        }
                        catch
                        {
                            //assume sql error/timeout
                            dr.Close();
                        }
                    }
                }
                cn.Close();
            }
        }
    }

    public class JSONHelpers
    {
        #region To Generate results
        public ArrayList GenerateResultsTable(string StudentId, ResultGrid.GridDisplayType type, ref int max_x)
        {
            ArrayList a1 = new ArrayList();
            //bool display_external_message = false;
            if (StudentId == "") { return a1; }
            try { Guid g1 = new Guid(StudentId); } catch { return a1; }
            ResultsList rl = new ResultsList();
            rl.LoadList_OrderByDate("dbo.tbl_Core_Students.StudentId", StudentId);
            //bool AnyBreakdowns = false;

            int x = 0;
            int max_cols = 1;
            int y = 0;
            double d1 = 0;
            string s = "";

            switch (type)
            {
                case ResultGrid.GridDisplayType.External:
                    s = "Date"; OutputElement(s, a1, x, y); x++;
                    s = "Description"; OutputElement(s, a1, x, y); x++;
                    s = "Syllabus Code"; OutputElement(s, a1, x, y); x++;
                    s = "Syllabus Title"; OutputElement(s, a1, x, y); x++;
                    s = "Result"; OutputElement(s, a1, x, y); x++;
                    s = "Add Info"; OutputElement(s, a1, x, y); x++;
                    s = "Board"; OutputElement(s, a1, x, y); x++;
                    y++; max_cols = x;
                    break;
                case ResultGrid.GridDisplayType.Module:
                    s = "Date"; OutputElement(s, a1, x, y); x++;
                    s = "Subject"; OutputElement(s, a1, x, y); x++;
                    s = "Option Title"; OutputElement(s, a1, x, y); x++;
                    s = "Option Code"; OutputElement(s, a1, x, y); x++;
                    s = "Result"; OutputElement(s, a1, x, y); x++;
                    s = "Max Mark"; OutputElement(s, a1, x, y); x++;
                    s = "Grade"; OutputElement(s, a1, x, y); x++;
                    y++; max_cols = x;
                    break;
                case ResultGrid.GridDisplayType.Internal:
                    ReportList repl = new ReportList(StudentId.ToString());
                    //try to compact down to one attainment grade /date....
                    //they are ordered by date (normalised) and then course, so should be easy
                    repl.ReduceToAttainment();
                    //going to try to add the report grades in to the results...

                    foreach (ReportValue v in repl.m_list)
                    {
                        Result res1 = new Result();
                        rl._results.Add(res1);
                        res1.External = false;
                        res1.Date = v.m_date;
                        res1.Shortname = "Report";
                        res1.Code = v.m_course;
                        if (v.m_IsCommitment) res1.Shortname = "Commitment";
                        x = (int)(v.m_value * 10);
                        d1 = (float)x / 10;
                        res1.Value = d1.ToString();
                        switch (v.m_courseType)
                        {
                            case 1://KS 3
                            case 2://KS 4
                            case 3://KS 5
                                res1.Value = "";
                                if (v.m_value > 0.0) res1.Value = "D";
                                if (v.m_value > 1) res1.Value = "C";
                                if (v.m_value > 2) res1.Value = "B";
                                if (v.m_value > 3) res1.Value = "A";
                                if (v.m_value > 4) res1.Value = "*";
                                if (v.m_value > 4.5) res1.Value = "**";//!! not going to happen
                                break;
                        }
                    }
                    Result r0 = new Result();
                    Result r2 = new Result();
                    //would be nice to sort the list by date.....  cos at the mo we have report stuf at end
                    for (int j = 0; j < rl._results.Count; j++)
                    {
                        for (int k = 0; k < (rl._results.Count - 1 - j); k++)
                        {
                            r0 = (Result)rl._results[k]; r2 = (Result)rl._results[k + 1];
                            if (r0.Date > r2.Date)
                            {
                                rl._results[k] = r2; rl._results[k + 1] = r0;
                            }
                        }
                    }
                    break;
                case ResultGrid.GridDisplayType.Predicted:
                    s = "Date"; OutputElement(s, a1, x, y); x++;
                    s = "Subject"; OutputElement(s, a1, x, y); x++;
                    s = "Predicted Grade"; OutputElement(s, a1, x, y); x++;
                    y++; max_cols = x;

                    break;

            }
            string temps = ""; int max_d = 1;
            string[,] grades = new string[50, 100];// [date, subject
            grades[0, 0] = "Date";

            foreach (Result r1 in rl._results)
            {
                switch (type)
                {
                    case ResultGrid.GridDisplayType.External:
                        if (r1.External)//only external
                        {
                            if (r1.Resulttype == 34)
                            {
                                //expq
                            }

                            if (r1.Resulttype == 25)
                            {
                                //STEP - no basedate/options data
                                x = 0;
                                OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                                OutputElement(r1.Description, a1, x, y); x++;
                                OutputElement("", a1, x, y); x++;
                                OutputElement(r1.Coursename, a1, x, y); x++;
                                OutputElement(r1.Value, a1, x, y); x++;
                                OutputElement(r1.Text, a1, x, y); x++;
                                y++;
                            }
                            if ((r1.OptionItem == "C") || ((r1.OptionItem == "") && (r1.Resulttype != 27) && (r1.Resulttype != 11)))//new options or not module
                            {
                                //exo1.Load(r1.OptionId);
                                x = 0;
                                OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                                OutputElement(r1.Shortname, a1, x, y); x++;
                                OutputElement(r1.SyllabusCode, a1, x, y); x++;
                                OutputElement(r1.OptionTitle, a1, x, y); x++;
                                OutputElement(r1.Value, a1, x, y); x++;
                                OutputElement(r1.Text, a1, x, y); x++;
                                ExamOption exo1 = new ExamOption(); exo1.Load(r1.OptionId);
                                Exam_Board eb1 = new Exam_Board(exo1.m_ExamBoardID);
                                OutputElement(eb1.m_OrganisationFriendlyName, a1, x, y); x++;

                                y++;
                            }
                            else
                            {
                                if ((r1.Resulttype == 10) || (r1.Resulttype == 35))
                                {
                                    x = 0;
                                    OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                                    OutputElement(r1.Shortname + "external", a1, x, y); x++;
                                    OutputElement("Unkown", a1, x, y); x++;
                                    OutputElement(r1.Coursename, a1, x, y); x++;
                                    OutputElement(r1.Value, a1, x, y); x++;
                                    OutputElement(r1.Text, a1, x, y); x++;
                                    y++;
                                }
                            }
                        }
                        break;
                    case ResultGrid.GridDisplayType.Module:
                        if (r1.OptionItem == "U")
                        {//we have an exam result
                            x = 0;
                            OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                            OutputElement(r1.Coursename, a1, x, y); x++;
                            OutputElement(r1.OptionTitle, a1, x, y); x++;
                            OutputElement(r1.OptionCode, a1, x, y); x++;
                            OutputElement(r1.Value, a1, x, y); x++;
                            OutputElement(r1.OptionMaximumMark, a1, x, y); x++;
                            s = "";

                            if (r1.Resulttype == 11)//GCE modules
                            {
                                try
                                {
                                    double v1 = System.Convert.ToDouble(r1.Value);
                                    double v2 = System.Convert.ToDouble(r1.OptionMaximumMark);
                                    v1 = v1 / v2;
                                    if (v1 >= 0.8) s = "a";
                                    if ((v1 < 0.8) && (v1 >= 0.7)) s = "b";
                                    if ((v1 < 0.7) && (v1 >= 0.6)) s = "c";
                                    if ((v1 < 0.6) && (v1 >= 0.5)) s = "d";
                                    if ((v1 < 0.5) && (v1 >= 0.4)) s = "e";
                                    if ((v1 < 0.4)) s = "u";
                                }
                                catch (Exception exc1)
                                {
                                    string sx = exc1.Message;
                                }
                            }
                            if (r1.Resulttype == 27)//GCSE modules
                            {
                                if (r1.Text != "")
                                {
                                    s = r1.Text;
                                }
                                else
                                {

                                    try
                                    {
                                        double v1 = System.Convert.ToDouble(r1.Value);
                                        double v2 = System.Convert.ToDouble(r1.OptionMaximumMark);
                                        v1 = v1 / v2;
                                        if (v1 >= 0.9) s = "a*";
                                        if ((v1 < 0.9) && (v1 >= 0.8)) s = "a";
                                        if ((v1 < 0.8) && (v1 >= 0.7)) s = "b";
                                        if ((v1 < 0.7) && (v1 >= 0.6)) s = "c";
                                        if ((v1 < 0.6) && (v1 >= 0.5)) s = "d";
                                        if ((v1 < 0.5) && (v1 >= 0.4)) s = "e";
                                        if ((v1 < 0.4)) s = "u";
                                    }
                                    catch (Exception exc1)
                                    {
                                        string sx = exc1.Message;
                                    }
                                }

                            }
                            OutputElement(s, a1, x, y); x++;
                            y++;
                        }
                        break;
                    case ResultGrid.GridDisplayType.Internal:
                        if (!r1.External)
                        {
                            temps = r1.Date.ToShortDateString() + "  " + r1.Shortname.Trim(); y = -1;
                            for (int i = 0; i < max_d; i++)
                            {
                                if (temps == grades[0, i]) y = i;
                            }
                            if (y < 0)
                            {
                                y = max_d;
                                grades[0, y] = temps;
                                max_d++;
                            }
                            //need to find subject it or add it...
                            x = -1;
                            for (int i = 1; i < max_cols; i++)
                            {
                                if (grades[i, 0] == r1.Code.Substring(0, 2)) x = i;
                            }
                            if (x < 0)
                            {
                                x = max_cols; grades[x, 0] = r1.Code.Substring(0, 2); max_cols++;
                            }
                            grades[x, y] = r1.Value;
                        }
                        break;
                    case ResultGrid.GridDisplayType.Predicted:
                        if (r1.Resulttype == 6)
                        {//we have a predicted result
                            x = 0;
                            OutputElement(r1.Date.ToShortDateString(), a1, x, y); x++;
                            OutputElement(r1.Coursename, a1, x, y); x++;
                            OutputElement(r1.Value, a1, x, y); x++;
                        }
                        break;

                }
            }
            //going to copy header row....
            y = max_d; for (int i = 0; i < max_cols; i++)
            {
                grades[i, y] = grades[i, 0];
            }
            max_d++;


            if (type == ResultGrid.GridDisplayType.Internal)
            {
                //now would be quite nice to sort in x....
                for (int i = 0; i < (max_cols); i++)
                {
                    for (int j = 1; j < (max_cols - 1 - i); j++)//don't move col 0 -- date
                    {
                        if (grades[j, 0].CompareTo(grades[j + 1, 0]) > 0)
                        {
                            //swop...
                            for (int k = 0; k < max_d; k++)
                            {
                                s = grades[j, k]; grades[j, k] = grades[j + 1, k]; grades[j + 1, k] = s;
                            }
                        }
                    }
                }
                for (y = 0; y < max_d; y++)
                {
                    for (x = 0; x < max_cols; x++)
                    {
                        OutputElement(grades[x, y], a1, x, y);
                    }
                }
                y = max_d;
            }
            max_x = max_cols;
            return a1;
        }

        public string GenerateResultsTableHTML(string StudentId, ResultGrid.GridDisplayType type)
        {
            ArrayList a1 = new ArrayList(); int max_x = 0;
            a1 = GenerateResultsTable(StudentId, type, ref max_x);
            string s = ""; string css_class = "";
            bool display_external_message = false;
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId);
            s = "<p ><h3>";
            switch (type)
            {
                case ResultGrid.GridDisplayType.External:
                    s += "Certified Results for "; css_class = "ResultsTbl";
                    display_external_message = true;
                    break;
                case ResultGrid.GridDisplayType.Module:
                    s += "Module Results for "; css_class = "ResultsTbl";
                    break;
                case ResultGrid.GridDisplayType.Internal:
                    s += "Internal Results for "; css_class = "ResultsTbl";
                    break;
                case ResultGrid.GridDisplayType.Predicted:
                    s += "Predicted Grades for "; css_class = "ResultsTbl";
                    break;
            }
            s += pupil1.m_GivenName + " " + pupil1.m_Surname + "</h3>";

            s += "<br /><TABLE BORDER   class=\"" + css_class + "\" style = \"font-size:small ;  \">";
            foreach (Grid_Element g in a1)
            {
                if (g.m_x == 0) s += "<TR>";
                if (g.m_y == 0) s += "<Th>" + g.m_value + "</Th>";
                else s += "<TD>" + g.m_value + "</TD>";
                if (g.m_x == max_x) s += "</TR>";
            }
            s += "</table></p>";
            if (display_external_message)
            {
                s += "</br>For external results: 'External' in the Add info column means we have seen some collaborating information (results slip etc) and 'External-verified' means we have seen the actual certificates.";
            }
            return s;
        }

        public string GenerateResultsJSON(string StudentId, ResultGrid.GridDisplayType type)
        {
            ArrayList a1 = new ArrayList(); int max_x = 0;
            a1 = GenerateResultsTable(StudentId, type, ref max_x);
            string s = "";
            SimplePupil pupil1 = new SimplePupil(); pupil1.Load(StudentId);
            string ns = Environment.NewLine;
            s = "{" + ns + "\"ResultList\" :{" + ns;
            s += "\"Type\":\"" + type.ToString() + "\"," + ns;
            s += "\"Student\":\"" + pupil1.m_GivenName + " " + pupil1.m_Surname + "\"," + ns;
            s += "\"Adno\":\"" + pupil1.m_adno.ToString() + "\"," + ns;
            s += "\"StudentId\":\"" + StudentId + "\"," + ns;
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s += "\"Results\": [" + ns;

            bool first = true;
            string[] categories = new string[20]; int n_cols = 0;

            foreach (Grid_Element g in a1)
            {
                if (first) { first = false; } else { s += "," + ns; }
                if (g.m_y == 0)
                {
                    categories[n_cols] = g.m_value; n_cols++;
                }
                else
                {
                    if (g.m_x == n_cols)
                    {
                        s += "\"" + categories[g.m_x] + "\": \"" + g.m_value + "\"" + ns; s += "}";
                    }
                    else
                    {
                        s += "\"" + categories[g.m_x] + "\": \"" + g.m_value + "\"," + ns;
                    }
                }
            }
            s += ns + "]" + ns + "}" + ns + "}";
            return s;
        }

        private ArrayList OutputElement(string s, ArrayList a, int x, int y)
        {
            //Response.Write("<TD>"+s+"</TD>");
            Grid_Element g = new Grid_Element();
            a.Add(g); g.m_x = x; g.m_y = y; g.m_value = s;
            return a;
        }
        #endregion

    }


    [Serializable]
    public class Listitem
    {
        public string m_text;
        public Guid m_value;
        public override string ToString()
        {
            return m_text;
        }
        public Listitem(string text, Guid value)
        {
            m_text = text; m_value = value;
        }
    }
    public class Listitem_Int
    {
        public string m_text;
        public int m_value;
        public override string ToString()
        {
            return m_text;
        }
        public Listitem_Int(string text, int value)
        {
            m_text = text; m_value = value;
        }
    }
    public class MailHelper
    {
        public bool sendcomplete = false;
        public string ErrorMessage = "";
        private static void SendCompletedCallback(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;
            MailHelper M1 = (MailHelper)sender;
            M1.sendcomplete = true;
        }
        public void SendMail(string from_email, string to_email, string attach_file, string text, string cc_email, string subject)
        {

            Attachment data = null;
            sendcomplete = false;

            //SmtpClient client = new SmtpClient("smtp.challoners.net");
            //NetworkCredential cred1 = new NetworkCredential("test-staff@challoners.net", "123Password");
            //client.EnableSsl = false;

            //fallback
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            NetworkCredential cred1 = new NetworkCredential("test-staff@challoners.org", "123Password");
            client.EnableSsl = true;

            //try to get from Database table DisplayKeys
            Cerval_Configurations configs = new Cerval_Configurations(); configs.Load_All();
            Cerval_Configuration c2 = new Cerval_Configuration();
            Cerval_Configuration c4 = new Cerval_Configuration();
            Cerval_Configuration c5 = new Cerval_Configuration();
            Cerval_Configuration c6 = new Cerval_Configuration();
            Cerval_Configuration c7 = new Cerval_Configuration();
            Cerval_Configuration c8 = new Cerval_Configuration();
            Cerval_Configuration c9 = new Cerval_Configuration();
            c2 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SMTP_Host");
            c4 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SMTP_Port");
            int port = 0;
            try { port = System.Convert.ToInt32(c4.Value); }
            catch { }
            c5 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SMTP_UserName");
            c6 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SMTP_Password");
            c7 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SMTP_EnableSSL");
            c8 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SMTP_FromAddress");
            c9 = configs.list.Find(c3 => c3.Key == "StaffIntranet_SMTP_FromDisplayName");
            bool EnableSSL = true;
            try { EnableSSL = System.Convert.ToBoolean(c7.Value); }
            catch { }
            if ((c2 != null) && (c4 != null) && (c5 != null) && (c6 != null) && (port != 0) && (c7 != null))
            {
                client = new SmtpClient(c2.Value, port);
                cred1 = new NetworkCredential(c5.Value, c6.Value);
                client.EnableSsl = EnableSSL;
            }

            client.Credentials = cred1;


            //MailAddress from = new MailAddress("test-staff@challoners.org");
            MailAddress from;
            if (c9 != null)
            {
                from = new MailAddress(c8.Value, c9.Value);
            }
            else {
                from = new MailAddress(c8.Value);
            }
            //from_email = "chrisclare0@hotmail.com";
            MailAddress replyto = new MailAddress(from_email);

            //if to_email has several addresses 1;2; we need to split
            //get the first one
            //to_email = "cc@challoners.org";  //testing 
            string s = to_email;
            int i = to_email.IndexOf(";");

            if (i > 0) s = to_email.Substring(0, i);
            MailAddress to = new MailAddress(s);
            MailMessage message = new MailMessage(from, to);
            while (i > 0)
            {
                to_email = to_email.Substring(i + 1);
                i = to_email.IndexOf(";");
                if (i > 0)
                {
                    s = to_email.Substring(0, i);
                }
                else
                {
                    s = to_email;
                }
                s.Trim();
                if (s != "")
                {
                    MailAddress to1 = new MailAddress(s); message.To.Add(to1);
                }

            }
            message.Body = text;
            if (cc_email != null)
            {
                if (cc_email != "")
                {
                    MailAddress cc = new MailAddress(cc_email);
                    message.CC.Add(cc);
                }
            }
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.ReplyTo = replyto;
            //message.ReplyToList.Add(replyto);///.net 4
            //message.Headers.Add("Precedence", "bulk");
            message.Headers.Add("Auto-Submitted", "auto-generated");
            message.Headers.Add("X-Auto-Response-Suppress", "All");

            if (attach_file != null)
            {
                if (attach_file != "")
                {
                    data = new Attachment(attach_file, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(attach_file);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(attach_file);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(attach_file);
                    message.Attachments.Add(data);
                }
            }

            //client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback); only needed for async send
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            message.IsBodyHtml = true;
            try
            {
                client.Send(message);
                sendcomplete = true;
            }
            catch (Exception e)
            {
                sendcomplete = false;
                ErrorMessage = e.Message + e.InnerException.Message + e.InnerException.InnerException.Message;
            }
            message.Dispose();
            if (data != null) data.Dispose();
        }
    }
    [Serializable]
    public class MarkbookAssessment
    {
        public Guid AssessmentId;
        public Guid CourseId;
        public string AssessmentName;
        public string AssessmentCode;
        public Guid CreatingStaffId;
        public DateTime DateCreated;
        public bool IsNumeric;
        public int MaxMark;
        public int MinMark;
        public bool IsPublic;
        public int Version;

        public MarkbookAssessment() { }

        public void Hydrate(SqlDataReader dr)
        {
            AssessmentId = dr.GetGuid(0);
            CourseId = dr.GetGuid(1);
            AssessmentName = dr.GetString(2);
            AssessmentCode = dr.GetString(3);
            CreatingStaffId = dr.GetGuid(4);
            if (!dr.IsDBNull(5)) DateCreated = dr.GetDateTime(5);
            IsNumeric = dr.GetBoolean(6);
            if (!dr.IsDBNull(7)) MaxMark = dr.GetInt32(7);
            if (!dr.IsDBNull(8)) MinMark = dr.GetInt32(8);
            IsPublic = dr.GetBoolean(9);
            if (!dr.IsDBNull(10)) Version = dr.GetInt32(10);
        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
                    }
                }
                cn.Close();
            }
        }
        public void Load(Guid AssessmentId)
        {
            string s = "SELECT * FROM dbo.tbl_Markbook_Assessments WHERE AssessmentId = '" + AssessmentId.ToString() + "' ";
            Load1(s);
        }
        public void Delete()
        {
            string s = "";
            s = "DELETE * FROM dbo.tbl_Markbook_Assessments WHERE (AssessmentId = '" + AssessmentId.ToString() + "' ) ";
            AssessmentId = Guid.Empty;
        }
        public void Save()
        {
            string s = "";
            string date_s = "CONVERT(DATETIME, '" + DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            if (AssessmentId == Guid.Empty)
            {
                AssessmentId = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Markbook_Assessments (AssessmentId,AssessmentName, AssessmentCode, CreatedByStaffId, DateCreated,IsNumeric,MaxMark,MinMark,IsPublic,Version) ";
                s += " VALUES ( '" + AssessmentId.ToString() + "', '";
                s += AssessmentName + "', '";
                s += AssessmentCode + "', '";
                s += CreatingStaffId.ToString() + "', '";
                s += date_s + "', '";
                if (IsNumeric) { s += "1', '"; } else { s += "0', '"; }
                s += MaxMark.ToString() + "',  '";
                s += MinMark.ToString() + "',  '";
                if (IsPublic) { s += "1', '"; } else { s += "0', '"; }
                s += Version.ToString() + "' )";
            }
            else
            {
                //update...
                s = "UPDATE dbo.tbl_Markbook_Assessments ";
                s += ", AssessmentName = '" + AssessmentName + "' ";
                s += ", AssessmentCode = '" + AssessmentCode + "' ";
                s += ", CreatedByStaffId = '" + CreatingStaffId.ToString() + "' ";
                s += ", DateCreated = " + date_s;
                s += ", IsNumeric = "; if (IsNumeric) { s += "1"; } else { s += "0"; }
                s += ", MaxMark = '" + MaxMark.ToString() + "' ";
                s += ", MinMark = '" + MinMark.ToString() + "' ";
                s += ", IsPublic = "; if (IsPublic) { s += "1"; } else { s += "0"; }
                s += ", Version = '" + Version.ToString() + "' ";
                s += " WHERE (AssessmentId = '" + AssessmentId.ToString() + "' ) ";

            }
            Encode en = new Encode();
            en.ExecuteSQL(s);

        }
    }
    [Serializable]
    public class MarkbookAssessmentList
    {
        public List<MarkbookAssessment> m_list = new List<MarkbookAssessment>();
        public MarkbookAssessmentList() { }

        public void LoadListStaff(Guid StaffId)
        {
            m_list.Clear();
            string s = "SELECT * FROM dbo.tbl_Markbook_Assessments WHERE CreatedByStaffId = '" + StaffId.ToString() + "' ";
            Load1(s);
        }
        public void LoadListGroup(Guid CourseId)
        {
            m_list.Clear();
            string s = "SELECT * FROM dbo.tbl_Markbook_Assessments WHERE CourseId = '" + CourseId.ToString() + "' ";
            Load1(s);
        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            MarkbookAssessment m = new MarkbookAssessment();
                            m.Hydrate(dr);
                            m_list.Add(m);
                        }
                    }
                }
                cn.Close();
            }
        }
    }
    [Serializable]
    public class MarkbookMark
    {
        public Guid MarkId;
        public Guid StudentId;
        public Guid AssessmentId;
        public Guid GroupId;
        public Guid StaffId;
        public DateTime Date;
        public string Mark;
        public int version;

        public MarkbookMark() { }

        public void Hydrate(SqlDataReader dr)
        {
            MarkId = dr.GetGuid(0);
            StudentId = dr.GetGuid(1);
            AssessmentId = dr.GetGuid(2);
            GroupId = dr.GetGuid(3);
            StaffId = dr.GetGuid(4);
            if (!dr.IsDBNull(5)) Date = dr.GetDateTime(5);
            if (!dr.IsDBNull(6)) Mark = dr.GetString(6);
            if (!dr.IsDBNull(7)) version = dr.GetInt32(7);
        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
                    }
                }
                cn.Close();
            }
        }
        public void Load(Guid MarkId)
        {
            string s = "SELECT * FROM dbo.tbl_Markbook_Marks WHERE MarkId = '" + MarkId.ToString() + "' ";
            Load1(s);
        }

        public void Delete()
        {
            string s = "";
            s = "DELETE * FROM dbo.tbl_Markbook_Marks WHERE (MarkId = '" + MarkId.ToString() + "' ) ";
            MarkId = Guid.Empty;
        }
        public void Save()
        {
            string s = "";
            string date_s = "CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            if (MarkId == Guid.Empty)
            {
                MarkId = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Markbook_Marks (MarkId,StudentId,AssessmentId,GroupId,StaffId,DateOfAssessment,Mark,Version) ";
                s += " VALUES ( '" + MarkId.ToString() + "', '";
                s += StudentId.ToString() + "', '";
                s += AssessmentId.ToString() + "', '";
                s += GroupId.ToString() + "', '";
                s += StaffId.ToString() + "', '";
                s += date_s + "', '";
                s += Mark + "', '";
                s += version.ToString() + "' )";
            }
            else
            {
                //update...
                s = "UPDATE dbo.tbl_Markbook_Marks ";
                s += " SET StudentId = '" + StudentId.ToString() + "' ";
                s += ", DateOfAssessment = " + date_s;
                s += ", AssessmentId = '" + AssessmentId.ToString() + "' ";
                s += ", GroupId = '" + GroupId.ToString() + "' ";
                s += ", StaffId = '" + StaffId.ToString() + "' ";
                s += ", Mark = '" + Mark + "' ";
                s += ", Version = '" + version.ToString() + "' ";
                s += " WHERE (MarkId = '" + MarkId.ToString() + "' ) ";

            }
            Encode en = new Encode();
            en.ExecuteSQL(s);

        }




    }
    [Serializable]
    public class MarkbookMarkList
    {
        public List<MarkbookMark> m_list;
        public MarkbookMarkList() { }
        public void LoadListAssessment(Guid AssessmentId)
        {
            m_list.Clear();
            string s = "SELECT * FROM dbo.tbl_Markbook_Marks WHERE AssessmentId = '" + AssessmentId.ToString() + "' ";
            Load1(s);
        }
        public void LoadListStudent(Guid StudentId)
        {
            m_list.Clear();
            string s = "SELECT * FROM dbo.tbl_Markbook_Marks WHERE StudentId = '" + StudentId.ToString() + "' ";
            Load1(s);
        }
        public void LoadListStaff(Guid StaffId)
        {
            m_list.Clear();
            string s = "SELECT * FROM dbo.tbl_Markbook_Marks WHERE StaffId = '" + StaffId.ToString() + "' ";
            Load1(s);
        }
        public void LoadListGroup(Guid GroupId)
        {
            m_list.Clear();
            string s = "SELECT * FROM dbo.tbl_Markbook_Marks WHERE GroupId = '" + GroupId.ToString() + "' ";
            Load1(s);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            MarkbookMark m = new MarkbookMark();
                            m.Hydrate(dr);
                            m_list.Add(m);
                        }
                    }
                }
                cn.Close();
            }
        }
    }
    [Serializable]
    public class Message
    {
        public Guid Id { get; set; }
        public string Msg { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public Guid StaffId { get; set; }       //normal messages have staff id, but might be null of massage is from, say,school captain
        public int Version { get; set; }
        bool valid { get; set; }
        public Guid PersonId { get; set; }       //only populated if staffid is null?? as alternative???

#if ADD_URL_TO_MESSAGE
        public string DocumentURL;
#endif
        public Message()
        {
            Id = Guid.Empty; valid = false;
            ValidFrom = DateTime.Now;
            ValidUntil = DateTime.Now.AddDays(7);
            Version = 0; StaffId = Guid.Empty; PersonId = Guid.Empty;
#if ADD_URL_TO_MESSAGE
            DocumentURL = "";
#endif
        }
        public bool Exists(Guid Id)
        {
            bool found = false;
            string s = "SELECT MessageId FROM dbo.tbl_Core_Messages WHERE MessageId = '" + Id.ToString() + "' ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            found = true;
                        }
                    }
                }
                cn.Close();
            }
            return found;
        }
        public void Load(Guid Id)
        {
            string s = "SELECT MessageId,Message,ValidFrom,ValidUntil],StaffId,Version,PersonId,DocumentURL FROM dbo.tbl_Core_Messages WHERE MessageId = '" + Id.ToString() + "' ";
            Load1(s);
        }
        protected void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            valid = true;
                            Hydrate(dr);
                        }
                    }
                }
                cn.Close();
            }
        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            Msg = dr.GetString(1);
            if (!dr.IsDBNull(2)) ValidFrom = dr.GetDateTime(2);
            if (!dr.IsDBNull(3)) ValidUntil = dr.GetDateTime(3);
            if (!dr.IsDBNull(4)) StaffId = dr.GetGuid(4);
            if (!dr.IsDBNull(5)) Version = dr.GetInt32(5);
            if (!dr.IsDBNull(6)) PersonId = dr.GetGuid(6);
#if ADD_URL_TO_MESSAGE
            if (!dr.IsDBNull(7)) DocumentURL = dr.GetString(6);//note only in debug db at present
#endif

        }
        public Guid Save()
        {
            string s = "";
            if (Id != Guid.Empty)
            {
                Message m1 = new Message(); m1.Load(Id);
                if (m1.valid)
                {
                    //it exists....  so update

                    s = "UPDATE dbo.tbl_Core_Messages SET Message = '" + Msg + "' ";
                    s += ", ValidFrom = CONVERT(DATETIME, '" + ValidFrom.ToShortDateString() + "', 103) ";
                    s += ", ValidUntil = CONVERT(DATETIME, '" + ValidUntil.ToShortDateString() + "', 103) ";
                    if (StaffId != Guid.Empty) { s += ", StaffId  = '" + StaffId.ToString() + "' "; }
                    if (PersonId != Guid.Empty) { s += ", PersonId  = '" + PersonId.ToString() + "' "; }
                    s += ",  Version = '" + Version.ToString() + "'  ";
#if ADD_URL_TO_MESSAGE
                    s += ", DocumentURL = '" + DocumentURL + "'  ";
#endif
                    s += " WHERE MessageId = '" + Id.ToString() + "' ";
                }
                else
                {
                    s = " INSERT INTO dbo.tbl_Core_Messages ( MessageId, Message, ValidFrom, ValidUntil";
                    if (StaffId != Guid.Empty) { s += ",StaffId "; }
                    if (PersonId != Guid.Empty) { s += ", PersonId "; }
#if ADD_URL_TO_MESSAGE
                    s +=", DocumentURL, Version ) ";
#else
                    s += ", Version ) ";
#endif
                    s += " VALUES ('" + Id.ToString() + "' , '" + Msg + "' , CONVERT(DATETIME, '" + ValidFrom.ToShortDateString() + "', 103),  ";
                    s += "CONVERT(DATETIME, '" + ValidUntil.ToShortDateString() + "', 103) ";
                    if (StaffId != Guid.Empty) { s += ", '" + StaffId.ToString() + "' "; }
                    if (PersonId != Guid.Empty) { s += ",'" + PersonId.ToString() + "' "; }


#if ADD_URL_TO_MESSAGE
                    s += ", '" + DocumentURL + "'  ";
#endif
                    s += ", '" + Version.ToString() + "' )";
                }
            }
            else
            {
                Id = Guid.NewGuid();
                s = " INSERT INTO dbo.tbl_Core_Messages ( MessageId, Message, ValidFrom, ValidUntil";
                if (StaffId != Guid.Empty) { s += ",StaffId "; }
                if (PersonId != Guid.Empty) { s += ", PersonId "; }
#if ADD_URL_TO_MESSAGE
                    s +=", DocumentURL , Version ) ";
#else
                s += ", Version ) ";
#endif
                s += " VALUES ('" + Id.ToString() + "' , '" + Msg + "' , CONVERT(DATETIME, '" + ValidFrom.ToShortDateString() + "', 103),  ";
                s += "CONVERT(DATETIME, '" + ValidUntil.ToShortDateString() + "', 103) ";
                if (StaffId != Guid.Empty) { s += ", '" + StaffId.ToString() + "' "; }
                if (PersonId != Guid.Empty) { s += ",'" + PersonId.ToString() + "' "; }
#if ADD_URL_TO_MESSAGE
                s += ", '" + DocumentURL + "'  ";
#endif
                s += ", '" + Version.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
            return Id;
        }
    }
    public class MessageList
    {
        public ArrayList m_list = new ArrayList();



        public void LoadList_Staff(Guid StaffId, int Number)
        {
            m_list.Clear();
            string s = "SELECT TOP(" + Number.ToString() + ")  * ";
            s += " FROM dbo.tbl_Core_Messages ";
            s += " WHERE     (dbo.tbl_Core_Messages.StaffId = '" + StaffId.ToString() + "' )";
            s += " ORDER BY  dbo.tbl_Core_Messages.ValidFrom DESC ";
            Load1(s);
        }

        public void LoadList_Person(Guid PersonId, int number)
        {
            m_list.Clear();
            string s = "SELECT TOP(" + number.ToString() + ")  * ";
            s += " FROM dbo.tbl_Core_Messages ";
            s += " WHERE     (dbo.tbl_Core_Messages.PersonId = '" + PersonId.ToString() + "' )";
            s += " ORDER BY  dbo.tbl_Core_Messages.ValidFrom DESC ";
            Load1(s);
            return;
        }

        public void LoadList_Staff(Guid StaffId)
        {
            m_list.Clear();
            string s = "SELECT  ";
            s += " FROM dbo.tbl_Core_Messages ";
            s += " WHERE     (dbo.tbl_Core_Messages.StaffId = '" + StaffId.ToString() + "' )";
            s += " ORDER BY  dbo.tbl_Core_Messages.ValidFrom DESC ";
            Load1(s);
        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Message m = new Message();
                            m_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class MessageGroup
    {
        public Guid Id;
        public Guid MessageId;
        public Guid GroupId;
        public bool Delivered;
        public DateTime DateDelivered;
        public Message _Message;
        public int Version;
        public bool valid;
        public MessageGroup()
        {
            Id = Guid.Empty;
            valid = false;
            Version = 0;
            Delivered = false;
            _Message = new Message();
        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            MessageId = dr.GetGuid(1);
            GroupId = dr.GetGuid(2);
            if (dr.IsDBNull(3)) { Delivered = false; }
            else { DateDelivered = dr.GetDateTime(3); Delivered = true; }
            if (!dr.IsDBNull(4)) Version = dr.GetInt32(4);
            _Message.Id = MessageId;
            _Message.Msg = dr.GetString(5);
            if (!dr.IsDBNull(6)) _Message.ValidFrom = dr.GetDateTime(6);
            if (!dr.IsDBNull(7)) _Message.ValidUntil = dr.GetDateTime(7);
            if (!dr.IsDBNull(8)) _Message.StaffId = dr.GetGuid(8);
            if (!dr.IsDBNull(9)) _Message.Version = dr.GetInt32(9);
            valid = true;
        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            valid = true;
                            Hydrate(dr);
                        }
                    }
                }
                cn.Close();
            }
        }
        public void Load(Guid ID)
        {
            string s = "SELECT dbo.tbl_Core_Messages_Groups.Id , dbo.tbl_Core_Messages_Groups.MessageId , ";
            s += " dbo.tbl_Core_Messages_Groups.GroupId , dbo.tbl_Core_Messages_Groups.DateDelivered , dbo.tbl_Core_Messages_Groups.Version ";
            s += ", dbo.tbl_Core_Messages.Message , dbo.tbl_Core_Messages.ValidFrom , ";
            s += " dbo.tbl_Core_Messages.ValidUntil , dbo.tbl_Core_Messages.StaffId , ";
            s += " dbo.tbl_Core_Messages.Version ";
            s += " FROM dbo.tbl_Core_Messages_Groups ";
            s += "INNER JOIN dbo.tbl_Core_Messages ON dbo.tbl_Core_Messages.MessageId = dbo.tbl_Core_Messages_Groups.MessageId ";
            s += "WHERE dbo.tbl_Core_Messages_Groups.Id = '" + ID.ToString() + "' ";
            Load1(s);
        }
        public Guid Save()
        {
            string s = "";
            if (Id != Guid.Empty)
            {
                MessageGroup m1 = new MessageGroup(); m1.Load(Id);
                if (m1.valid)
                {
                    //it exists....  so update
                    s = "UPDATE dbo.tbl_Core_Messages_Groups SET MessageId = '" + MessageId.ToString() + "' , ";
                    s += "  GroupId = '" + GroupId.ToString() + "' ";
                    if (Delivered)
                        s += ", DateDelivered = CONVERT(DATETIME, '" + DateDelivered.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                    s += ", Version = '" + Version.ToString() + "' ";
                    s += " WHERE Id = '" + Id.ToString() + "' ";
                }
                else
                {
                    s = " INSERT INTO dbo.tbl_Core_Messages_Groups( Id, MessageId, GroupId ";
                    if (Delivered) s += ", DateDelivered ";
                    s += ", Version ) VALUES ('" + Id.ToString() + "' , '" + MessageId.ToString() + "' , '" + GroupId.ToString() + "' ";
                    if (Delivered) s += ", CONVERT(DATETIME, '" + DateDelivered.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                    s += ", '" + Version.ToString() + "'  ) ";
                }
            }
            else
            {
                Id = Guid.NewGuid();
                s = " INSERT INTO dbo.tbl_Core_Messages_Groups( Id, MessageId, GroupId ";
                if (Delivered) s += ", DateDelivered ";
                s += ", Version  ) VALUES ('" + Id.ToString() + "' , '" + MessageId.ToString() + "' , '" + GroupId.ToString() + "' ";
                if (Delivered) s += ", CONVERT(DATETIME, '" + DateDelivered.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += ", '" + Version.ToString() + "'  ) ";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
            return Id;
        }
    }
    public class MessageGroupList
    {
        public ArrayList m_list = new ArrayList();
        public void LoadList(Guid GroupId)
        {
            m_list.Clear();
            string s = "SELECT dbo.tbl_Core_Messages_Groups.Id , dbo.tbl_Core_Messages_Groups.MessageId , ";
            s += " dbo.tbl_Core_Messages_Groups.GroupId , dbo.tbl_Core_Messages_Groups.DateDelivered , dbo.tbl_Core_Messages_Groups.Version ";
            s += ", dbo.tbl_Core_Messages.Message , dbo.tbl_Core_Messages.ValidFrom , ";
            s += " dbo.tbl_Core_Messages.ValidUntil , dbo.tbl_Core_Messages.StaffId , ";
            s += " dbo.tbl_Core_Messages.Version ";
            s += " FROM dbo.tbl_Core_Messages_Groups ";
            s += "INNER JOIN dbo.tbl_Core_Messages ON dbo.tbl_Core_Messages.MessageId = dbo.tbl_Core_Messages_Groups.MessageId ";
            s += " WHERE     (dbo.tbl_Core_Messages_Groups.GroupId = '" + GroupId.ToString() + "' )";
            Load1(s);
        }
        public void LoadList_Message(Guid MessageId)
        {
            m_list.Clear();
            string s = "SELECT dbo.tbl_Core_Messages_Groups.Id , dbo.tbl_Core_Messages_Groups.MessageId , ";
            s += " dbo.tbl_Core_Messages_Groups.GroupId , dbo.tbl_Core_Messages_Groups.DateDelivered , dbo.tbl_Core_Messages_Groups.Version ";
            s += ", dbo.tbl_Core_Messages.Message , dbo.tbl_Core_Messages.ValidFrom , ";
            s += " dbo.tbl_Core_Messages.ValidUntil , dbo.tbl_Core_Messages.StaffId , ";
            s += " dbo.tbl_Core_Messages.Version ";
            s += " FROM dbo.tbl_Core_Messages_Groups ";
            s += "INNER JOIN dbo.tbl_Core_Messages ON dbo.tbl_Core_Messages.MessageId = dbo.tbl_Core_Messages_Groups.MessageId ";
            s += " WHERE     (dbo.tbl_Core_Messages.MessageId = '" + MessageId.ToString() + "' )";
            Load1(s);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            MessageGroup m = new MessageGroup();
                            m_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class MessageStudent
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime DateDelivered { get; set; }
        public bool Delivered { get; set; }
        public Message _Message { get; set; }
        public int Version { get; set; }
        public bool valid { get; set; }
        public MessageStudent()
        {
            Id = Guid.Empty;
            valid = false;
            Version = 0;
            Delivered = false;
            _Message = new Message();
        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            MessageId = dr.GetGuid(1);
            StudentId = dr.GetGuid(2);
            if (dr.IsDBNull(3))
            {
                Delivered = false;
            }
            else
            {
                DateDelivered = dr.GetDateTime(3); Delivered = true;
            }
            if (!dr.IsDBNull(4)) Version = dr.GetInt32(4);
            _Message.Id = MessageId;
            _Message.Msg = dr.GetString(5);
            if (!dr.IsDBNull(6)) _Message.ValidFrom = dr.GetDateTime(6);
            if (!dr.IsDBNull(7)) _Message.ValidUntil = dr.GetDateTime(7);
            if (!dr.IsDBNull(8)) _Message.StaffId = dr.GetGuid(8);
            if (!dr.IsDBNull(9)) _Message.Version = dr.GetInt32(9);
            valid = true;
        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            valid = true;
                            Hydrate(dr);
                        }
                    }
                }
                cn.Close();
            }
        }
        public void Load(Guid ID)
        {
            string s = "SELECT dbo.tbl_Core_Messages_Students.Id , dbo.tbl_Core_Messages_Students.MessageId , ";
            s += " dbo.tbl_Core_Messages_Students.StudentId , dbo.tbl_Core_Messages_Students.DateDelivered , dbo.tbl_Core_Messages_Students.Version ";
            s += ", dbo.tbl_Core_Messages.Message , dbo.tbl_Core_Messages.ValidFrom , ";
            s += " dbo.tbl_Core_Messages.ValidUntil , dbo.tbl_Core_Messages.StaffId , ";
            s += " dbo.tbl_Core_Messages.Version ";
            s += " FROM dbo.tbl_Core_Messages_Students ";
            s += "INNER JOIN dbo.tbl_Core_Messages ON dbo.tbl_Core_Messages.MessageId = dbo.tbl_Core_Messages_Students.MessageId ";
            s += "WHERE dbo.tbl_Core_Messages_Students.Id = '" + ID.ToString() + "' ";
            Load1(s);
        }
        public Guid Save()
        {
            string s = "";
            if (Id != Guid.Empty)
            {
                MessageStudent m1 = new MessageStudent(); m1.Load(Id);
                if (m1.valid)
                {
                    //it exists....  so update
                    s = "UPDATE dbo.tbl_Core_Messages_Students SET MessageId = '" + MessageId.ToString() + "' ";
                    s += ",  StudentId = '" + StudentId.ToString() + "' ";

                    if (Delivered)
                        s += ", DateDelivered = CONVERT(DATETIME, '" + DateDelivered.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                    s += ",  Version = '" + Version.ToString() + "' ";
                    s += " WHERE Id = '" + Id.ToString() + "' ";
                }
                else
                {
                    s = " INSERT INTO dbo.tbl_Core_Messages_Students ( Id, MessageId, StudentId ";
                    if (Delivered) s += ", DateDelivered ";
                    s += ", Version ) VALUES ('" + Id.ToString() + "' , '" + MessageId.ToString() + "' , '" + StudentId.ToString() + "' ";
                    if (Delivered) s += ", CONVERT(DATETIME, '" + DateDelivered.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                    s += " , '" + Version.ToString() + "' ) ";
                }
            }
            else
            {
                Id = Guid.NewGuid();
                s = " INSERT INTO dbo.tbl_Core_Messages_Students ( Id, MessageId, StudentId ";
                if (Delivered) s += ", DateDelivered ";
                s += ", Version ) VALUES ('" + Id.ToString() + "' , '" + MessageId.ToString() + "' , '" + StudentId.ToString() + "' ";
                if (Delivered) s += ", CONVERT(DATETIME, '" + DateDelivered.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += " , '" + Version.ToString() + "' ) ";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
            return Id;
        }
    }
    public class MessageStudentList
    {
        public ArrayList m_list = new ArrayList();
        public void LoadListStudent(Guid StudentId)
        {
            m_list.Clear();
            string s = "SELECT dbo.tbl_Core_Messages_Students.Id , dbo.tbl_Core_Messages_Students.MessageId , ";
            s += " dbo.tbl_Core_Messages_Students.StudentId , dbo.tbl_Core_Messages_Students.DateDelivered , dbo.tbl_Core_Messages_Students.Version ";
            s += ", dbo.tbl_Core_Messages.Message , dbo.tbl_Core_Messages.ValidFrom  ";
            s += ", dbo.tbl_Core_Messages.ValidUntil ,dbo.tbl_Core_Messages.StaffId ";
            s += ", dbo.tbl_Core_Messages.Version ";
            s += " FROM dbo.tbl_Core_Messages_Students  ";
            s += "INNER JOIN dbo.tbl_Core_Messages ON dbo.tbl_Core_Messages.MessageId = dbo.tbl_Core_Messages_Students.MessageId ";
            s += " WHERE     (dbo.tbl_Core_Messages_Students.StudentId = '" + StudentId.ToString() + "' )";
            Load1(s);
        }
        public void LoadList_Message(Guid MessageId)
        {
            m_list.Clear();
            string s = "SELECT dbo.tbl_Core_Messages_Students.Id , dbo.tbl_Core_Messages_Students.MessageId , ";
            s += " dbo.tbl_Core_Messages_Students.StudentId , dbo.tbl_Core_Messages_Students.DateDelivered , dbo.tbl_Core_Messages_Students.Version ";
            s += ", dbo.tbl_Core_Messages.Message , dbo.tbl_Core_Messages.ValidFrom  ";
            s += ", dbo.tbl_Core_Messages.ValidUntil ,dbo.tbl_Core_Messages.StaffId ";
            s += ", dbo.tbl_Core_Messages.Version ";
            s += " FROM dbo.tbl_Core_Messages_Students  ";
            s += "INNER JOIN dbo.tbl_Core_Messages ON dbo.tbl_Core_Messages.MessageId = dbo.tbl_Core_Messages_Students.MessageId ";
            s += " WHERE     (dbo.tbl_Core_Messages.MessageId = '" + MessageId.ToString() + "' )";
            Load1(s);
        }

        public void LoadList_Group(Guid GroupID, DateTime date)
        {
            //this is to load all the messages by student for those in this group. Will only get those current.
            m_list.Clear();
            string d = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string select1 = " ( SELECT dbo.tbl_Core_Student_Groups.StudentId FROM dbo.tbl_Core_Student_Groups WHERE ";
            select1 += "( GroupID='" + GroupID.ToString() + "' ) ";
            select1 += " AND (MemberFrom <= " + d + " ) AND (MemberUntil > " + d + " ) )";

            string s = "SELECT dbo.tbl_Core_Messages_Students.Id , dbo.tbl_Core_Messages_Students.MessageId , ";
            s += " dbo.tbl_Core_Messages_Students.StudentId , dbo.tbl_Core_Messages_Students.DateDelivered , dbo.tbl_Core_Messages_Students.Version ";
            s += ", dbo.tbl_Core_Messages.Message , dbo.tbl_Core_Messages.ValidFrom  ";
            s += ", dbo.tbl_Core_Messages.ValidUntil ,dbo.tbl_Core_Messages.StaffId ";
            s += ", dbo.tbl_Core_Messages.Version ";
            s += " FROM dbo.tbl_Core_Messages_Students  ";
            s += "INNER JOIN dbo.tbl_Core_Messages ON dbo.tbl_Core_Messages.MessageId = dbo.tbl_Core_Messages_Students.MessageId ";
            s += " WHERE    (  (dbo.tbl_Core_Messages_Students.StudentId IN " + select1 + ")  )";
            s += " AND ( dbo.tbl_Core_Messages.ValidUntil>" + d + ") ";
            s += " AND ( dbo.tbl_Core_Messages.ValidFrom<" + d + ") ";
            //s += " AND ( dbo.tbl_Core_Messages_Students.DateDelivered IS NOT NULL)";
            s += " ORDER BY dbo.tbl_Core_Messages.MessageId ";
            Load1(s);

        }


        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            MessageStudent m = new MessageStudent();
                            m_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    [Serializable]
    public class OptionStructure
    {
        [Serializable]
        public class OptionSet
        {
            public int no_students;
            public string name;
            public string subject;
            public int[] students = new int[80];
        }
        [Serializable]
        public class OptionBlock
        {
            public int no_sets;
            public string name;
            public OptionSet[] Sets = new OptionSet[20];
            public int width;// only used at present with cerval stuff
            public int colour;// only used at present with cerval stuff
            public int rowoffset;// only used at present with cerval stuff
            public Guid SchemeId;// only used at present with cerval stuff
            public int year;// only used at present with cerval stuff

            public bool DoesChoiceFit(string sub, ref StudentOptionSolution fit, int index, string exact_set) //exact_set="" if nont required else only this set - for MF and doubel set options
            {

                if (sub == "")
                {
                    return true;
                }
                bool fits = false;
                int n = 999;
                for (int i = 0; i < no_sets; i++)
                {
                    if (Sets[i].subject == sub)
                    {
                        if ((exact_set == "") || (exact_set == Sets[i].name))
                        {
                            if (!fit.locked[index])
                            {
                                if (Sets[i].no_students < n)
                                {
                                    n = Sets[i].no_students + 1;
                                    fit.sets[index] = Sets[i].name;
                                    fit.setsize[index] = Sets[i].no_students + 1;
                                    fit.subjects[index] = Sets[i].subject;
                                    fits = true;
                                }
                            }
                            else
                            {
                                if (Sets[i].name == fit.sets[index])
                                {
                                    n = Sets[i].no_students + 1;
                                    fit.setsize[index] = Sets[i].no_students + 1;
                                    fit.subjects[index] = Sets[i].subject;
                                    fits = true;
                                }
                            }
                        }
                    }
                }
                return fits;
            }
        }
        public OptionStructure()
        {
            bestfit = new StudentOptionSolution();
            //initialise blocks

            for (int i = 0; i < 5; i++)
            {
                Blocks[i] = new OptionBlock();
                for (int j = 0; j < 20; j++)
                {
                    Blocks[i].Sets[j] = new OptionSet();
                }
            }
            //initialise choices
            for (int i = 0; i < 300; i++)
            {
                Choices[i] = new StudentChoice();
            }
        }
        [Serializable]
        public class StudentChoice
        {
            //raw option list
            public int Adno;
            public int no_choices;
            public string Surname;
            public string GivenName;
            public string[] choices = new string[5];
            public string[] fittedSets = new string[5];
            public bool[] lockedSet = new bool[5];
            public StudentChoice()
            {
                for (int i = 0; i < 5; i++)
                {
                    lockedSet[i] = false; fittedSets[i] = "";
                }
            }
            public enum SixthFormDecision { None, Accept, Reject, Discuss, Not_Applied }
            public SixthFormDecision Decision;
            public string Message;
        }
        [Serializable]
        public class StudentOptionSolution : IComparable
        {
            public static int MAX_COLS = 7;
            public string[] subjects = new string[MAX_COLS];
            public string[] sets = new string[MAX_COLS];
            public int[] setsize = new int[MAX_COLS];
            public bool[] locked;
            public int PreferredSize = 24;
            public bool valid = false;

            public StudentOptionSolution()
            {
                locked = new bool[MAX_COLS];
            }

            public void Initialise(int PrefSetSize)
            {
                for (int i = 0; i < MAX_COLS; i++)
                {
                    subjects[i] = ""; sets[i] = ""; setsize[i] = 0; valid = false;
                    locked[i] = false;
                }
                PreferredSize = PrefSetSize;
            }
            public void Copy(StudentOptionSolution s)
            {
                for (int k = 0; k < MAX_COLS; k++)
                {
                    subjects[k] = s.subjects[k];
                    sets[k] = s.sets[k];
                    setsize[k] = s.setsize[k];
                    locked[k] = s.locked[k];
                    valid = s.valid;
                    PreferredSize = s.PreferredSize;
                }
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                //less 0 we are less than other
                StudentOptionSolution other = obj as StudentOptionSolution;
                if (other != null)
                {
                    int n1 = 0; int n2 = 0;
                    for (int k = 0; k < MAX_COLS; k++)
                    {
                        if (setsize[k] > PreferredSize)
                            n1 += setsize[k] - PreferredSize;
                        if (other.setsize[k] > PreferredSize)
                            n2 += other.setsize[k] - PreferredSize;

                    }
                    return (n1 - n2);
                }
                else
                    throw new ArgumentException("Object is not a StudentOptionSolution");
            }

            #endregion
        }

        public OptionBlock[] Blocks = new OptionBlock[5];
        public StudentChoice[] Choices = new StudentChoice[300];
        public int no_blocks = 5;
        public int no_students = 0;
        public StudentOptionSolution bestfit;

        public void LoadBlocks(string filename)
        {
            //load a txt sep file 
            TextReader t1 = new TextReader();
            FileStream fs = new FileStream(filename, FileMode.Open);
            TextRecord tr1 = new TextRecord();
            t1.ReadTextLine(fs, ref tr1);
            //this is headr
            //if no_blocks not set ... find out from file
            if (no_blocks == 0) no_blocks = tr1.number_fields + 1;
            for (int i = 0; i < no_blocks; i++)
            {
                Blocks[i].name = tr1.field[i];
            }
            int j = 0; int y = 0;
            while (t1.ReadTextLine(fs, ref tr1) != TextReader.READ_LINE_STATUS.ENDFILE)
            {
                if (tr1.field[0].StartsWith("W") || tr1.field[0].StartsWith("C") || tr1.field[0].StartsWith("R") || tr1.field[0].StartsWith("Y"))
                {
                    for (int i = 0; i < no_blocks; i++)
                    {
                        int v = System.Convert.ToInt32(tr1.field[i].Substring(1, 2));
                        switch (tr1.field[0].Substring(0, 1))
                        {
                            case "W"://width
                                Blocks[i].width = v;
                                break;
                            case "C"://colour
                                Blocks[i].colour = v;
                                break;
                            case "R": //rowoffset
                                Blocks[i].rowoffset = v;
                                break;
                            case "Y": //year
                                Blocks[i].year = v;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    //each line is a set of (pos) sets
                    for (int i = 0; i < no_blocks; i++)
                    {
                        Blocks[i].Sets[j].name = tr1.field[i];
                        Blocks[i].no_sets++;
                        Blocks[i].Sets[j].subject = "";
                        //now subject is from 10CH1... or 9CH1... 
                        try
                        {
                            y = System.Convert.ToInt32(tr1.field[i].Substring(0, 1));
                            if (y < 9) { Blocks[i].Sets[j].subject = tr1.field[i].Substring(2, 2).ToUpper(); }
                            else { Blocks[i].Sets[j].subject = tr1.field[i].Substring(1, 2).ToUpper(); }
                        }
                        catch
                        {
                        }
                    }
                    j++;
                }

            }
        }

        public void SaveChoices(string filename)
        {

            //filename=@"C:\apps\Options 2013\Decisions.txt";  //testing
            StreamWriter st = new StreamWriter(filename, false);
            char c = (char)0x09;
            string s = "Adno" + c + "Fname" + c + "Sname" + c + "1" + c + "2" + c + "3" + c + "4" + c + "5" + c + "Decision" + c + "Message";
            st.WriteLine(s);
            for (int j = 0; j < no_students; j++)
            {
                s = Choices[j].Adno.ToString() + c;
                s += Choices[j].GivenName + c;
                s += Choices[j].Surname + c;
                for (int k = 0; k < 5; k++)
                {
                    s += Choices[j].choices[k] + c;
                }
                s += Choices[j].Decision.ToString() + c;
                s += Choices[j].Message;
                st.WriteLine(s);
            }
            st.Close();
        }

        public void LoadChoices(string filename)
        {
            //load a txt sep file 
            //each line has adno, forename,surname,choice1, etc
            int msg_no = 0; int Decision_no = 0; string s = "";
            TextReader t1 = new TextReader();
            FileStream fs = new FileStream(filename, FileMode.Open);
            TextRecord tr1 = new TextRecord();
            t1.ReadTextLine(fs, ref tr1);
            for (int i = 0; i <= tr1.number_fields; i++)
            {
                if (tr1.field[i] == "Decision")
                {
                    Decision_no = i;
                }
                if (tr1.field[i] == "Message")
                {
                    msg_no = i;
                }
            }
            //this is header
            int j = 0;
            while (t1.ReadTextLine(fs, ref tr1) != TextReader.READ_LINE_STATUS.ENDFILE)
            {
                Choices[j].Adno = System.Convert.ToInt32(tr1.field[0]);
                Choices[j].GivenName = tr1.field[1];
                Choices[j].Surname = tr1.field[2];
                Choices[j].no_choices = 0;
                for (int k = 0; k < no_blocks; k++)
                {
                    Choices[j].choices[k] = tr1.field[3 + k].Trim().ToUpper();
                    Choices[j].no_choices++;
                }
                //now after the choices fields may have the decisions...
                if (Decision_no > 0)
                {
                    s = tr1.field[Decision_no];
                    switch (s)
                    {
                        case "Accept": Choices[j].Decision = StudentChoice.SixthFormDecision.Accept; break;
                        case "Reject": Choices[j].Decision = StudentChoice.SixthFormDecision.Reject; break;
                        case "Discuss": Choices[j].Decision = StudentChoice.SixthFormDecision.Discuss; break;
                        case "Not_Applied": Choices[j].Decision = StudentChoice.SixthFormDecision.Not_Applied; break;
                        default: Choices[j].Decision = StudentChoice.SixthFormDecision.None; break;
                    }
                }
                if (msg_no > 0)
                {
                    Choices[j].Message = tr1.field[msg_no];
                }

                j++;
            }
            no_students = j;
            fs.Close();

        }

        public string CompareChoices(string filename)
        {
            //Compare to a txt sep file 
            //each line has adno, forename,surname,choice1, etc
            string s = ""; int adno = 0;
            string sname = ""; string fname = "";
            TextReader t1 = new TextReader();
            FileStream fs = new FileStream(filename, FileMode.Open);
            TextRecord tr1 = new TextRecord();
            t1.ReadTextLine(fs, ref tr1);

            //this is header

            while (t1.ReadTextLine(fs, ref tr1) != TextReader.READ_LINE_STATUS.ENDFILE)
            {
                adno = System.Convert.ToInt32(tr1.field[0]);
                sname = tr1.field[2];
                fname = tr1.field[1];
                foreach (StudentChoice sc in Choices)
                {
                    if (sc.Adno == adno)
                    {
                        if (sc.GivenName != tr1.field[1]) { s += "GivenName Error " + sc.Adno.ToString() + Environment.NewLine; }
                        if (sc.Surname != tr1.field[2]) { s += "SurName Error " + sc.Adno.ToString() + Environment.NewLine; }
                        for (int k = 0; k < no_blocks; k++)
                        {
                            if (sc.choices[k] != tr1.field[3 + k].Trim().ToUpper())
                            {
                                s += "Choice Error " + sc.Adno.ToString() + "(" + sc.GivenName + " " + sc.Surname + ")" + Environment.NewLine;
                            }
                        }
                        break;
                    }
                }

                foreach (StudentChoice sc in Choices)
                {
                    if ((sc.Surname == sname) && (sc.GivenName == fname))
                    {
                        if (sc.Adno != adno) { s += "Adno Error " + sc.Adno.ToString() + "?" + adno.ToString() + Environment.NewLine; }
                        for (int k = 0; k < no_blocks; k++)
                        {
                            if (sc.choices[k] != tr1.field[3 + k].Trim().ToUpper())
                            {
                                s += "Choice Error " + sc.Adno.ToString() + "(" + sc.GivenName + " " + sc.Surname + ")" + Environment.NewLine;
                            }
                        }
                        break;
                    }
                }



            }
            fs.Close();
            return s;

        }

        public string UpdateChoices(string filename)
        {
            //Compare to a txt sep file 
            //each line has adno, forename,surname,choice1, etc
            string s = ""; int adno = 0;
            string sname = ""; string fname = "";
            TextReader t1 = new TextReader();
            FileStream fs = new FileStream(filename, FileMode.Open);
            TextRecord tr1 = new TextRecord();
            t1.ReadTextLine(fs, ref tr1);

            //this is header
            while (t1.ReadTextLine(fs, ref tr1) != TextReader.READ_LINE_STATUS.ENDFILE)
            {
                adno = System.Convert.ToInt32(tr1.field[0]);
                sname = tr1.field[2];
                fname = tr1.field[1];
                foreach (StudentChoice sc in Choices)
                {
                    if (sc.Adno == adno)
                    {
                        for (int k = 0; k < no_blocks; k++)
                        {
                            if (sc.choices[k] != tr1.field[3 + k].Trim().ToUpper())
                            {
                                sc.choices[k] = tr1.field[3 + k].Trim().ToUpper();
                                s += "Choice Error " + sc.Adno.ToString() + "(" + sc.GivenName + " " + sc.Surname + ")" + Environment.NewLine;
                            }
                        }
                        break;
                    }
                }

                foreach (StudentChoice sc in Choices)
                {
                    if ((sc.Surname == sname) && (sc.GivenName == fname))
                    {
                        if (sc.Adno != adno)
                        {
                            sc.Adno = adno;
                        }
                        break;
                    }
                }
            }
            fs.Close();
            return s;

        }

        public StudentOptionSolution FitStudent(bool add_to_Sets, int s, int PrefSetSize)
        {
            //so we take the choices for student s;
            string[] c = new string[7];
            bestfit.Initialise(PrefSetSize);
            StudentOptionSolution fit = new StudentOptionSolution();
            fit.Initialise(PrefSetSize);
            //if fitted already... clear....
            for (int k = 0; k < no_blocks; k++)
            {

                if (Choices[s].lockedSet[k])
                {
                    //if locked set up fit to say so...
                    fit.sets[k] = Choices[s].fittedSets[k];
                    fit.locked[k] = true;
                }
                //now take the student out of the set lists...
                for (int j = 0; j < Blocks[k].no_sets; j++)
                {
                    for (int i = 0; i < Blocks[k].Sets[j].no_students; i++)
                    {
                        if ((Blocks[k].Sets[j].students[i] == Choices[s].Adno))
                        {
                            //remove it 
                            for (int l = i; l < (Blocks[k].Sets[j].no_students - 1); l++)
                            {
                                Blocks[k].Sets[j].students[l] = Blocks[k].Sets[j].students[l + 1];
                            }
                            Blocks[k].Sets[j].students[Blocks[k].Sets[j].no_students] = 0;
                            Blocks[k].Sets[j].no_students--;
                        }
                        Choices[s].fittedSets[k] = "";
                    }
                }
            }
            //so now all clear except for locked sets...
            int i1 = 0; string s1 = "";

            //set up the c array of temp choices...
            for (int i = 0; i < no_blocks; i++)
            {
                if (Choices[s].choices[i].Trim() != "")
                {
                    c[i] = Choices[s].choices[i]; i1++;
                }
            }



            for (int i = 0; i < no_blocks; i++)
            {
                s1 = fit.sets[i];
                if ((Choices[s].lockedSet[i]) && (s1 != ""))
                {
                    i1--;
                    //now remove the choice from c...
                    s1 = fit.sets[i];
                    s1 = s1.Substring(2, 2);
                    for (int k = 0; k < 7; k++)
                    {
                        if (c[k] == s1)
                        {
                            c[k] = "";
                            //copy down...
                            for (int k1 = k; k1 < 6; k1++)
                            {
                                c[k1] = c[k1 + 1];
                            }
                            break;
                        }
                    }
                }
            }



            // delete choices that are locked

            bool t = Fit(fit, c, i1);
            if (bestfit.valid && add_to_Sets)
            {
                //now add this best fit to sets...
                for (int k = 0; k < no_blocks; k++)
                {
                    if (bestfit.sets[k] != "")
                    {
                        for (int j = 0; j < Blocks[k].no_sets; j++)
                        {
                            if ((Blocks[k].Sets[j].name == bestfit.sets[k]))
                            {
                                //add student ...unless locked when it is already there....                        
                                Blocks[k].Sets[j].students[Blocks[k].Sets[j].no_students] = Choices[s].Adno;
                                Blocks[k].Sets[j].no_students++;
                                Choices[s].fittedSets[k] = bestfit.sets[k];
                            }
                        }
                    }
                }
            }
            return bestfit;
        }

        public bool Fit(StudentOptionSolution fit, string[] choices, int no_ch)
        {
            if (no_ch == 0)//at end of the line....
            {
                //so has a fit.... need to check MF sets....
                string s1 = ""; string s2 = "";
                for (int i = 0; i < no_blocks; i++)
                {
                    if (fit.sets[i].Contains("MF"))
                    {
                        if (s1 == "")
                            s1 = fit.sets[i];
                        else
                            s2 = fit.sets[i];
                    }
                }
                if (s1 != s2) return false;

                //and SE sets
                for (int i = 0; i < no_blocks; i++)
                {
                    if (fit.sets[i].Contains("SE"))
                    {
                        if (s1 == "")
                            s1 = fit.sets[i];
                        else
                            s2 = fit.sets[i];
                    }
                }

                //and BE sets
                for (int i = 0; i < no_blocks; i++)
                {
                    if (fit.sets[i].Contains("BB"))
                    {
                        if (s1 == "")
                            s1 = fit.sets[i];
                        else
                            s2 = fit.sets[i];
                    }
                }
                if (s1 != s2) return false;




                fit.valid = true;
                if (!bestfit.valid)
                {
                    bestfit.Copy(fit);
                }
                else
                {
                    if (bestfit.CompareTo(fit) > 0)
                    {
                        bestfit.Copy(fit);
                    }
                }
                return true;
            }
            Random r = new Random();
            string[] c2 = new string[5]; string exact_fit = "";
            StudentOptionSolution fit1 = new StudentOptionSolution();
            fit1.Copy(fit);
            for (int i = 0; i < no_ch; i++)//try each subject
            {

                if (r.NextDouble() < 0.5)
                {
                    for (int j = no_blocks - 1; j > -1; j--)//try each block onli if this block is empty
                    {
                        if ((fit1.subjects[j].Trim() == ""))
                        {
                            if (choices[i] == "MF")
                            {
                                for (int k = 0; k < no_blocks; k++)
                                {
                                    if (fit.subjects[k] == "MF")
                                        exact_fit = fit.sets[k];
                                }
                            }

                            if (choices[i] == "SE")
                            {
                                for (int k = 0; k < no_blocks; k++)
                                {
                                    if (fit.subjects[k] == "SE")
                                        exact_fit = fit.sets[k];
                                }
                            }

                            //only call if not locked...

                            if (Blocks[j].DoesChoiceFit(choices[i], ref fit1, j, exact_fit))
                            {
                                //so choice i fits in block j... need to copy down....
                                int i0 = 0;
                                for (int i2 = 0; i2 < (no_ch - 1); i2++)
                                {
                                    if (i0 == i) i0++;//skip
                                    c2[i2] = choices[i0]; i0++;
                                }
                                if (!Fit(fit1, c2, no_ch - 1))
                                {
                                }
                                //clean up...
                                fit1.subjects[j] = "";
                                fit1.sets[j] = "";
                            }
                        }//endif block empty
                    }//for each block
                }
                else
                {
                    for (int j = 0; j < no_blocks; j++)//try each block onli if this block is empty/try each block onli if this block is empty
                    {
                        if ((fit1.subjects[j].Trim() == ""))
                        {
                            if (choices[i] == "MF")
                            {
                                for (int k = 0; k < no_blocks; k++)
                                {
                                    if (fit.subjects[k] == "MF")
                                        exact_fit = fit.sets[k];
                                }
                            }
                            if (choices[i] == "SE")
                            {
                                for (int k = 0; k < no_blocks; k++)
                                {
                                    if (fit.subjects[k] == "SE")
                                        exact_fit = fit.sets[k];
                                }
                            }

                            //only call if not locked...

                            if (Blocks[j].DoesChoiceFit(choices[i], ref fit1, j, exact_fit))
                            {
                                //so choice i fits in block j... need to copy down....
                                int i0 = 0;
                                for (int i2 = 0; i2 < (no_ch - 1); i2++)
                                {
                                    if (i0 == i) i0++;//skip
                                    c2[i2] = choices[i0]; i0++;
                                }
                                if (!Fit(fit1, c2, no_ch - 1))
                                {
                                }
                                //clean up...
                                fit1.subjects[j] = "";
                                fit1.sets[j] = "";
                            }
                        }//endif block empty
                    }//for each block
                }
            }//for each subject
            return bestfit.valid;
        }
        public void Serialize(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Close();
        }

    }


    public class PastStudentList
    {
        public ArrayList _studentlist = new ArrayList();
        public PastStudentList(string query)
        {
            _studentlist.Clear();
            //string db_connection;
            //System.Configuration.AppSettingsReader ar=new AppSettingsReader();
            //db_connection=ar.GetValue("db",typeof(string)).ToString();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  PersonSurname, PersonGivenName, StudentId, PersonId, Expr2, Expr18 , Expr5  FROM  qry_Cerval_Core_Student ";
            if (query != "")
            {
                s += " WHERE (" + query + ") ";
            }
            s += " ORDER BY PersonSurname";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            _studentlist.Add(p);
                            p.m_Surname = dr.GetString(0);
                            p.m_GivenName = dr.GetString(1);
                            p.m_StudentId = dr.GetGuid(2);
                            p.m_PersonId = dr.GetGuid(3);
                            p.m_adno = dr.GetInt32(4);
                            if (!dr.IsDBNull(5)) p.m_dol = dr.GetDateTime(5);
                            if (!dr.IsDBNull(6)) p.m_doa = dr.GetDateTime(6);
                            p.m_year = 14;//default;

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }


    }
    [Serializable]
    public class Period
    {
        public string m_periodcode;
        public string m_periodname;
        public DateTime m_PeriodStart;
        public DateTime m_PeriodEnd;
        public Guid m_PeriodId;
        public bool m_AccademicPeriod;
        public Guid m_Parent_Id;
        public int m_ISAMSPeriodcode;   //only used in ISAMS upload in CErval Students

        public Period() { m_PeriodId = Guid.Empty; m_Parent_Id = Guid.Empty; }
        public Period(Guid id)
        {
            Load(id);
        }
        public Period(string id)
        {
            Load(new Guid(id));
        }
        public override string ToString()
        {
            return m_periodcode;
        }
        public void Hydrate(SqlDataReader dr)
        {
            m_PeriodId = dr.GetGuid(0);
            m_periodcode = dr.GetString(1);
            m_periodname = dr.GetString(2);
            m_PeriodStart = dr.GetDateTime(3);
            m_PeriodEnd = dr.GetDateTime(4);
            m_AccademicPeriod = dr.GetBoolean(5);
            m_Parent_Id = Guid.Empty;
            if (!dr.IsDBNull(6)) m_Parent_Id = dr.GetGuid(6);
        }
        public void Load(Guid Id)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM tbl_Core_Periods WHERE ( PeriodId = '" + Id.ToString() + "' )";
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

        public Guid Save()
        {
            string s = "";
            if (m_PeriodId == Guid.Empty)
            {
                m_PeriodId = Guid.NewGuid();
                s = " INSERT INTO dbo.tbl_Core_Periods (PeriodId,PeriodCode,PeriodName,PeriodStart,PeriodEnd,AcademicPeriod,";
                if (m_Parent_Id != Guid.Empty) s += "Parent_Id,";
                s += "Version)";
                s += " VALUES ( '" + m_PeriodId.ToString() + "','" + m_periodcode + "','" + m_periodname + "'";
                s += ", CONVERT(DATETIME, '" + m_PeriodStart.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                s += ", CONVERT(DATETIME, '" + m_PeriodEnd.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                s += ",'" + m_AccademicPeriod.ToString() + "'";
                if (m_Parent_Id != Guid.Empty) s += ",'" + m_Parent_Id.ToString() + "'";
                s += " , '2' )";
            }
            else
            {
                //update...
                s = "UPDATE dbo.tbl_Core_Periods ";
                s += " SET PeriodName = '" + m_periodname + "' ";
                s += ", PeriodCode = '" + m_periodcode + "' ";
                s += ", PeriodStart = CONVERT(DATETIME, '" + m_PeriodStart.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += ", PeriodEnd = CONVERT(DATETIME, '" + m_PeriodEnd.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += ", AcademicPeriod = '" + m_AccademicPeriod.ToString() + "' ";
                if (m_Parent_Id != Guid.Empty) s += ", Parent_Id = '" + m_Parent_Id.ToString() + "' ";
                s += " WHERE (PeriodId= '" + m_PeriodId.ToString() + "' ) ";

            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
            return m_PeriodId;
        }

        public DateTime GetStartDate(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, m_PeriodStart.Hour, m_PeriodStart.Minute, m_PeriodStart.Second);
        }
        public DateTime GetEndDate(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, m_PeriodEnd.Hour, m_PeriodEnd.Minute, m_PeriodEnd.Second);
        }

    }
    public class PeriodList
    {
        public ArrayList m_PeriodList = new ArrayList();
        public PeriodList()
        {
            string s = "SELECT * FROM tbl_Core_Periods WHERE ( Parent_Id IS NULL ) AND (PeriodCode <> '6') ORDER BY PeriodStart ";
            Load1(s);
        }

        public PeriodList(bool LoadFullList)
        {
            string s = "";
            if (LoadFullList)
            {
                s = "SELECT * FROM tbl_Core_Periods  ORDER BY PeriodStart ";
            }
            else
            {
                s = "SELECT * FROM tbl_Core_Periods WHERE ( Parent_Id IS NULL ) AND (PeriodCode <> '6') ORDER BY PeriodStart ";
            }
            Load1(s);
        }

        protected void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Period p = new Period();
                            m_PeriodList.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }

    }

    public class Person
    {
        public Guid m_PersonId;
        public string m_Gender;
        public int m_Title;
        public string m_Surname;
        public string m_GivenName;
        public string m_MiddleName;
        public DateTime m_dob;

        public Guid Save()
        {
            Encode en = new Encode();
            string date_s = "CONVERT(DATETIME, '" + m_dob.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";

            if (m_PersonId == Guid.Empty)
            {
                m_PersonId = Guid.NewGuid();


                string s = "INSERT INTO dbo.tbl_Core_People ";
                s += " (PersonId, PersonTitle, PersonGender, PersonGivenName, PersonSurname, PersonDateOfBirth, Version  )";
                s += " VALUES ( '" + m_PersonId.ToString() + "' , '" + m_Title.ToString() + "'";
                s += " ,'" + m_Gender.ToString() + "' , '" + m_GivenName + "' ";
                s += " , '" + m_Surname + "' , " + date_s + " , '200' )";
                en.ExecuteSQL(s);
            }
            else
            {

            }
            return m_PersonId;
        }
    }
    public class PersonDetails
    {
        public Guid m_PersonId;
        public string m_Gender;
        public string m_Title;
        public string m_Surname;
        public string m_GivenName;
        public string m_MiddleName;
        public DateTime m_dob;
        public bool m_deceased;
        public string m_address;
        public string[] m_address_elements;
        public ContactList m_contacts;
        public bool m_valid = false;
        public string m_InformalName;
        public bool m_PP;
        public PersonDetails(string PersonId)
        {
            m_address_elements = new string[10];
            Load(PersonId);
        }

        private void Load(string PersonId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_Person WHERE PersonId = '" + PersonId + "'";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            //0 is timestamp
                            m_PersonId = dr.GetGuid(1);
                            m_Gender = dr.GetString(3);
                            if (!dr.IsDBNull(4)) m_GivenName = dr.GetString(4);
                            if (!dr.IsDBNull(5)) m_Surname = dr.GetString(5);
                            if (!dr.IsDBNull(6)) m_MiddleName = dr.GetString(6);
                            if (!dr.IsDBNull(8)) m_dob = dr.GetDateTime(8);
                            if (!dr.IsDBNull(9)) m_deceased = dr.GetBoolean(9);
                            if (!dr.IsDBNull(10)) m_Title = dr.GetString(10);
                            m_valid = true;

                        }
                        dr.Close();
                    }
                }
                if (m_valid)
                {
                    s = "SELECT * FROM qry_Cerval_Core_AddressForPerson ";
                    s += "WHERE (PersonId ='" + m_PersonId.ToString() + "' )";
                    using (SqlCommand cm = new SqlCommand(s, cn))
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                for (int i = 2; i < 10; i++)
                                {
                                    if (!dr.IsDBNull(i))
                                    {
                                        m_address += dr.GetString(i) + "  ";
                                        m_address_elements[i] = dr.GetString(i);
                                    }
                                }
                            }
                            dr.Close();
                        }
                    }
                    m_contacts = new ContactList();
                    m_contacts.LoadList(m_PersonId.ToString());
                }
            }
        }


    }
    public class PersonToken
    {
        public Guid m_TokenId;
        public Guid m_PersionId;
        public int m_TokenType;
        public string m_TokenValue;
        public bool m_valid = false;

        public PersonToken()
        {
            m_TokenId = Guid.Empty;
        }

        public PersonToken(Guid Id)
        {
            LoadPersonToken(Id.ToString());
        }

        public PersonToken(string token)
        {
            LoadPersonToken(token);
        }

        public void LoadPersonToken(string token)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_TokenId = Guid.Empty;
            string s = "SELECT * FROM tbl_Core_People_Tokens WHERE ( TokenValue = '" + token + "' )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            m_TokenType = dr.GetInt32(2);
                            if (m_TokenType == 1)
                            {
                                m_TokenId = dr.GetGuid(0);
                                m_PersionId = dr.GetGuid(1);
                                m_TokenType = dr.GetInt32(2);
                                m_TokenValue = dr.GetString(3);
                                m_valid = true;
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }


    [Serializable]
    public class PhysicsBooking
    {
        public Guid BookingId;
        public Guid StaffId;
        public Guid PeriodId;
        public Guid RoomId;
        public int DayId;
        public DateTime Date;
        public Guid GroupId;
        public string Notes;
        public PhysicsBooking()
        {
            BookingId = Guid.Empty; Notes = "";
        }
        public void Save()
        {
            Encode en = new Encode();
            string date_s = "CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            Utility u = new Utility();
            string s1 = u.CleanInvertedCommas(Notes);
            if (BookingId == Guid.Empty)
            {
                BookingId = Guid.NewGuid();
                string s = "INSERT INTO dbo.tbl_Physics_Bookings ";
                s += " (BookingId, StaffId, PeriodId, DayId, RoomId, Date, ";
                if (GroupId != Guid.Empty) s += " GroupId, ";
                s += " Notes, Version )";
                s += " VALUES ( '" + BookingId.ToString() + "' , '" + StaffId.ToString() + "'";
                s += " ,'" + PeriodId.ToString() + "' , '" + DayId.ToString() + "' ";
                s += " , '" + RoomId.ToString() + "' , " + date_s + " , ";
                if (GroupId != Guid.Empty) s += "'" + GroupId.ToString() + "', ";
                s += "'" + s1 + "' , '0' )";
                en.ExecuteSQL(s);
            }
            else
            {
                string s = "UPDATE dbo.tbl_Physics_Bookings ";//only notes should change...??
                s += "  SET Notes = '" + s1 + "'";
                s += "WHERE  (BookingId='" + BookingId.ToString() + "') ";
                en.ExecuteSQL(s);
            }
        }

        public void Delete()
        {
            if (BookingId == Guid.Empty) return;
            Encode en = new Encode();
            //first delete all the equipment associated...

            string s = "DELETE FROM dbo.tbl_Physics_Bookings_Equipment ";
            s += " WHERE (dbo.tbl_Physics_Bookings_Equipment.BookingId  = '" + BookingId.ToString() + "') ";
            int i = en.Execute_count_SQL(s);

            //now the booking
            s = "DELETE FROM dbo.tbl_Physics_Bookings ";
            s += " WHERE (BookingId  = '" + BookingId.ToString() + "') ";
            i = en.Execute_count_SQL(s);
            BookingId = Guid.Empty;

        }
        public bool Load(string Id)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            bool found = false;

            string s = "SELECT * FROM tbl_Physics_Bookings  ";
            s += "WHERE (BookingId='" + Id.ToString() + "'  )";
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
                            found = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return found;
        }
        public bool FindBooking()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            bool found = false;
            string date_s = "CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT * FROM tbl_Physics_Bookings  ";
            s += "WHERE (StaffId='" + StaffId.ToString() + "'  )";
            s += " AND (PeriodId='" + PeriodId.ToString() + "'  )";
            s += " AND (RoomId='" + RoomId.ToString() + "' )";
            s += " AND (DayId='" + DayId.ToString() + "' )";
            s += " AND (Date=" + date_s + " )";
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
                            found = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return found;
        }
        public bool FindBooking(Guid staff, Guid period, Guid room, int day, DateTime Date)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            bool found = false;
            string date_s = "CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT * FROM tbl_Physics_Bookings  ";
            s += "WHERE (StaffId='" + staff.ToString() + "'  )";
            s += " AND (PeriodId='" + period.ToString() + "'  )";
            s += " AND (RoomId='" + room.ToString() + "' )";
            s += " AND (DayId='" + day.ToString() + "' )";
            s += " AND (Date=" + date_s + " )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydrate(dr); found = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return found;
        }
        public void Hydrate(SqlDataReader dr)
        {
            BookingId = dr.GetGuid(0);
            StaffId = dr.GetGuid(1);
            PeriodId = dr.GetGuid(2);
            DayId = dr.GetInt32(3);
            RoomId = dr.GetGuid(4);
            Date = dr.GetDateTime(5);
            if (!dr.IsDBNull(6)) GroupId = dr.GetGuid(6);
            if (!dr.IsDBNull(7)) Notes = dr.GetString(7);
        }
    }
    public class PhysicsBookingList
    {
        public List<PhysicsBooking> m_list = new List<PhysicsBooking>();

        public PhysicsBookingList() { }

        public void LoadList_Date(DateTime d)
        {
            //need to zero the time!!
            DateTime date = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string date_s = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT * FROM tbl_Physics_Bookings  ";
            s += "WHERE (Date=" + date_s + " )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            PhysicsBooking p1 = new PhysicsBooking();
                            p1.Hydrate(dr);
                            m_list.Add(p1);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }

        public void LoadList_Group(Guid groupId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM tbl_Physics_Bookings  ";
            s += "WHERE (GroupId ='" + groupId.ToString() + "' ) ";
            s += " ORDER BY Date DESC ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            PhysicsBooking p1 = new PhysicsBooking();
                            p1.Hydrate(dr);
                            m_list.Add(p1);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    [Serializable]
    public class PhysicsEquipmentItem
    {
        public Guid EquipmentItemId;
        public string EquipmentItemCode;
        public string EquipmentItemDescription;
        public Guid EquipmentItemRoomId;
        public string EquipmentItemLocation;
        public string EquipmentItemSupplierCode;
        public Guid Bookings_EquipmentId;
        public Guid Experiment_EquipmentId;

        public PhysicsEquipmentItem()
        {
            EquipmentItemCode = ""; EquipmentItemDescription = "";
            EquipmentItemLocation = ""; EquipmentItemSupplierCode = "";
            Bookings_EquipmentId = Guid.Empty; Experiment_EquipmentId = Guid.Empty;
        }
        public bool Load(Guid id)
        {
            Encode en = new Encode(); bool found = false;
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM tbl_Physics_EquipmentItems WHERE (EquipmentItemId='" + id.ToString() + "') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydrate(dr); found = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return found;
        }
        public bool Find(string Code)
        {
            Encode en = new Encode(); bool found = false;
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM tbl_Physics_EquipmentItems WHERE (EquipmentItemCode ='" + Code + "') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydrate(dr); found = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return found;

        }
        public void Hydrate(SqlDataReader dr)
        {
            EquipmentItemId = dr.GetGuid(0);
            EquipmentItemCode = dr.GetString(1);
            if (!dr.IsDBNull(2)) EquipmentItemDescription = dr.GetString(2); else EquipmentItemDescription = "";
            if (!dr.IsDBNull(3)) EquipmentItemRoomId = dr.GetGuid(3); else EquipmentItemRoomId = Guid.Empty;
            if (!dr.IsDBNull(4)) EquipmentItemLocation = dr.GetString(4); else EquipmentItemLocation = "";

        }
        public void Save()
        {
            Encode en = new Encode(); bool RoomOK = true;
            if ((EquipmentItemRoomId == null) || (EquipmentItemRoomId == Guid.Empty)) RoomOK = false;
            if ((EquipmentItemId == null) || (EquipmentItemId == Guid.Empty))
            {
                EquipmentItemId = Guid.NewGuid();
                string s = "INSERT INTO dbo.tbl_Physics_EquipmentItems ";
                s += " (EquipmentItemId, EquipmentItemCode,EquipmentItemDescription, ";
                if (RoomOK) s += " EquipmentItemRoomId, ";
                s += " EquipmentItemLocation, EquipmentItemSupplierCode, Version  )";
                s += " VALUES ( '" + EquipmentItemId.ToString() + "' , '" + EquipmentItemCode + "' , ";
                s += " '" + EquipmentItemDescription + "', ";
                if (RoomOK) s += " '" + EquipmentItemRoomId.ToString() + "' , ";
                s += " '" + EquipmentItemLocation + "' , ";
                s += " '" + EquipmentItemSupplierCode + "' , '1' )";
                en.ExecuteSQL(s);
            }
            else
            {
                string s = "UPDATE dbo.tbl_Physics_EquipmentItems ";
                s += "  SET EquipmentItemCode = '" + EquipmentItemCode + "'";
                s += "  , EquipmentItemDescription = '" + EquipmentItemDescription + "'";
                if (RoomOK) s += "  , EquipmentItemRoomId = '" + EquipmentItemRoomId.ToString() + "'";
                s += "  , EquipmentItemLocation = '" + EquipmentItemLocation + "'";
                s += "  , EquipmentItemSupplierCode = '" + EquipmentItemSupplierCode + "'";
                s += "WHERE  (EquipmentItemId='" + EquipmentItemId.ToString() + "') ";
                en.ExecuteSQL(s);
            }
        }

        public bool Delete()
        {
            if (EquipmentItemId == null) return false;
            if (EquipmentItemId == Guid.Empty) return false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "DELETE FROM dbo.tbl_Physics_EquipmentItems ";
            s += " WHERE (dbo.tbl_Physics_EquipmentItems.EquipmentItemId  = '" + EquipmentItemId.ToString() + "') ";
            int i = en.Execute_count_SQL(s);
            if (i == 1) return true;
            return false;

        }
    }
    [Serializable]
    public class PhysicsEquipmentItemList
    {
        public List<PhysicsEquipmentItem> m_list = new List<PhysicsEquipmentItem>();
        public PhysicsEquipmentItemList() { m_list.Clear(); }
        public void LoadList(PhysicsBooking bk)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * ,tbl_Physics_Bookings_Equipment.Id FROM tbl_Physics_EquipmentItems  ";
            s += "INNER JOIN tbl_Physics_Bookings_Equipment ON tbl_Physics_EquipmentItems.EquipmentItemId= tbl_Physics_Bookings_Equipment.EquipmentItemId ";
            s += "WHERE (tbl_Physics_Bookings_Equipment.BookingId='" + bk.BookingId.ToString() + "'  )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            PhysicsEquipmentItem e = new PhysicsEquipmentItem();
                            e.Hydrate(dr);
                            if (!dr.IsDBNull(7)) e.Bookings_EquipmentId = dr.GetGuid(7); else e.Bookings_EquipmentId = Guid.Empty;
                            m_list.Add(e);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void LoadList(PhysicsExperiment Px)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * ,tbl_Physics_Experiment_Equipment.Id FROM tbl_Physics_EquipmentItems  ";
            s += "INNER JOIN tbl_Physics_Experiment_Equipment ON tbl_Physics_EquipmentItems.EquipmentItemId= tbl_Physics_Experiment_Equipment.EquipmentItemId ";
            s += "WHERE (tbl_Physics_Experiment_Equipment.ExperimentId='" + Px.Id.ToString() + "'  )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            PhysicsEquipmentItem e = new PhysicsEquipmentItem();
                            e.Hydrate(dr);
                            if (!dr.IsDBNull(7)) e.Experiment_EquipmentId = dr.GetGuid(7); else e.Bookings_EquipmentId = Guid.Empty;
                            m_list.Add(e);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_All()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT *  FROM tbl_Physics_EquipmentItems  ";
            s += " ORDER BY tbl_Physics_EquipmentItems.EquipmentItemCode ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            PhysicsEquipmentItem e = new PhysicsEquipmentItem();
                            e.Hydrate(dr);
                            m_list.Add(e);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class PhysicsEquipmentBooking
    {
        public Guid Id;
        public Guid BookingId;
        public Guid EquipmentId;

        public PhysicsEquipmentBooking() { }

        public bool Delete_by_Id()
        {
            if (Id == null) return false;
            if (Id == Guid.Empty) return false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "DELETE FROM dbo.tbl_Physics_Bookings_Equipment ";
            s += " WHERE (dbo.tbl_Physics_Bookings_Equipment.Id  = '" + Id.ToString() + "') ";
            int i = en.Execute_count_SQL(s);
            if (i == 1) return true;
            return false;
        }




        public void Save()
        {
            Encode en = new Encode();

            string s = "INSERT INTO dbo.tbl_Physics_Bookings_Equipment ";
            s += " (BookingId, EquipmentItemId )";
            s += " VALUES ( '" + BookingId.ToString() + "' , '" + EquipmentId.ToString() + "' ) ";
            en.ExecuteSQL(s);


        }
    }
    [Serializable]
    public class PhysicsExperiment
    {
        public Guid Id;
        public string ExperimentCode = "";
        public string ExperimentDescription = "";
        public int KeyStage = 5;
        public string SpecificationReference = "";
        public string topic = "";
        public string notes = "";
        public int Version = 0;

        public PhysicsEquipmentItemList EquipmentList = new PhysicsEquipmentItemList();
        public PhysicsExperiment() { }
        public void RemoveEquipmentItem(Guid Experiment_EquipmentItemId)
        {
            Encode en = new Encode();
            //now the list...going to remove all the linked equipmentitems
            string s = " DELETE FROM dbo.tbl_Physics_Experiment_Equipment ";
            s += " WHERE (dbo.tbl_Physics_Experiment_Equipment.Id='" + Experiment_EquipmentItemId.ToString() + "' ) ";
            en.Execute_count_SQL(s);
            PhysicsEquipmentItem i1 = new PhysicsEquipmentItem();
            foreach (PhysicsEquipmentItem i in EquipmentList.m_list)
            {
                if (i.Experiment_EquipmentId == Experiment_EquipmentItemId)
                {
                    i1 = i;
                }
            }
            EquipmentList.m_list.Remove(i1);
        }
        public void AddEquipmentItem(Guid EquipmentItemId)
        {
            PhysicsEquipmentItem ei = new PhysicsEquipmentItem();
            ei.Load(EquipmentItemId);
            EquipmentList.m_list.Add(ei);
            Encode en = new Encode();
            string s = "INSERT INTO dbo.tbl_Physics_Experiment_Equipment ";
            s += " ( ExperimentId,EquipmentItemId ) ";
            s += "VALUES ( '" + Id.ToString() + "' , '" + EquipmentItemId.ToString() + "' )";
            en.ExecuteSQL(s);

        }
        public void Load()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM tbl_Physics_Experiments  ";
            s += "WHERE (ExperimentId='" + Id.ToString() + "'  )";

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
            EquipmentList.LoadList(this);
        }

        public void Delete()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "DELETE  FROM tbl_Physics_Experiments  ";
            s += "WHERE (ExperimentId='" + Id.ToString() + "'  )";
            SqlConnection cn = new SqlConnection(db_connection); cn.Open();
            SqlCommand cm = new SqlCommand(s, cn);
            cm.ExecuteNonQuery(); cn.Close();
        }

        public void Save()
        {
            Utility u = new Utility();
            Encode en = new Encode(); string s = "";
            //so need to update the EquipmentList and the experiment record...
            if ((Id == null) || (Id == Guid.Empty))
            {
                //make it...
                Id = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Physics_Experiments ";
                s += " (ExperimentId, ExperimentCode,ExperimentDescription, KeyStage, ";
                s += " SpecificationReference, topic, notes, Version  )";
                s += " VALUES ( '" + Id.ToString() + "' , '" + ExperimentCode + "' , ";
                s += " '" + u.CleanInvertedCommas(ExperimentDescription) + "','" + KeyStage.ToString() + "' , ";
                s += " '" + SpecificationReference + "' ,'" + u.CleanInvertedCommas(topic) + "', '" + u.CleanInvertedCommas(notes) + "','1' )";
                en.ExecuteSQL(s);
            }
            else
            {
                s = "UPDATE dbo.tbl_Physics_Experiments ";
                s += "  SET ExperimentCode = '" + ExperimentCode + "'";
                s += "  , ExperimentDescription = '" + ExperimentDescription + "'";
                s += "  , KeyStage = '" + KeyStage.ToString() + "'";
                s += "  , SpecificationReference = '" + SpecificationReference + "'";
                s += "  , topic = '" + topic + "' , notes = '" + notes + "'";
                s += "  , Version = '" + "2" + "'";
                s += "  WHERE  (ExperimentId='" + Id.ToString() + "') ";
                en.ExecuteSQL(s);
            }
            //now the list...going to remove all the linked equipmentitems
            s = " DELETE FROM dbo.tbl_Physics_Experiment_Equipment ";
            s += " WHERE (dbo.tbl_Physics_Experiment_Equipment.ExperimentId='" + Id.ToString() + "' ) ";
            int n = en.Execute_count_SQL(s);
            foreach (PhysicsEquipmentItem i in EquipmentList.m_list)
            {
                s = "INSERT INTO dbo.tbl_Physics_Experiment_Equipment ";
                s += " ( ExperimentId,EquipmentItemId ) ";
                s += "VALUES ( '" + Id.ToString() + "' , '" + i.EquipmentItemId.ToString() + "' )";
                en.ExecuteSQL(s);
            }
        }


        public bool Find_By_Code(string code)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            bool found = false;
            string s = "SELECT * FROM tbl_Physics_Experiments  ";
            s += "WHERE (ExperimentCode='" + code + "'  )";

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
                            found = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            if (found) Load();
            return found;
        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            ExperimentCode = dr.GetString(1);
            if (!dr.IsDBNull(2)) ExperimentDescription = dr.GetString(2);
            if (!dr.IsDBNull(3)) KeyStage = dr.GetInt32(3);
            if (!dr.IsDBNull(4)) SpecificationReference = dr.GetString(4);
            if (!dr.IsDBNull(5)) topic = dr.GetString(5);
            if (!dr.IsDBNull(6)) notes = dr.GetString(6);
            Version = dr.GetInt32(7);
        }
    }
    public class PhysicsExperimentList
    {
        public List<PhysicsExperiment> m_list = new List<PhysicsExperiment>();
        public PhysicsExperimentList() { m_list.Clear(); }

        public void Load_All()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT *  FROM tbl_Physics_Experiments  ";
            s += " ORDER BY tbl_Physics_Experiments.ExperimentCode ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            PhysicsExperiment e = new PhysicsExperiment();
                            e.Hydrate(dr);
                            m_list.Add(e);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Load_Equipment(PhysicsEquipmentItem i)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT *  FROM tbl_Physics_Experiments  ";
            s += " INNER JOIN tbl_Physics_Experiment_Equipment ON tbl_Physics_Experiment_Equipment.ExperimentId = tbl_Physics_Experiments.ExperimentId ";
            s += " WHERE (tbl_Physics_Experiment_Equipment.EquipmentItemId = '" + i.EquipmentItemId.ToString() + "' )";
            s += " ORDER BY tbl_Physics_Experiments.ExperimentCode ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            PhysicsExperiment e = new PhysicsExperiment();
                            e.Hydrate(dr);
                            m_list.Add(e);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }


    public class PupilDetails
    {
        public Guid m_StudentId;
        public Guid m_PersonId;
        public string m_Surname;
        public string m_GivenName;
        public string m_MiddleName;
        public int m_adno;
        public string m_upn;
        public int m_examNo;
        public string m_UCI;
        public DateTime m_dob;
        public DateTime m_doa;
        public DateTime m_dol;
        public string m_ethnicity;
        public string m_religion;
        public string m_language;
        public int m_year;
        public bool m_IsOnRole;
        public string m_address;
        public string m_address_commasep;
        public string[] m_address_elements;
        public RelationshipList m_relationships;
        public ContactList m_contacts;
        public bool m_valid = false;
        public string m_BTECNumber;
        public string m_uln;
        public string m_InformalName;
        public string m_PP;/// defined but not yet used...
        public string m_GoogleAppsLogin;
        public string m_Gender;

        public PupilDetails()
        {
            m_relationships = new RelationshipList();
            m_address_elements = new string[10];
            m_contacts = new ContactList();
        }

        public PupilDetails(string studentId)
        {
            m_relationships = new RelationshipList();
            m_address_elements = new string[10];
            m_contacts = new ContactList();
            Load(studentId);

        }


        public void Load(string studentId)
        {
            string s = "SELECT * FROM  qry_Cerval_Core_Student WHERE StudentId = '" + studentId + "'";
            Load1(s);
        }
        public void Load_adno(string Adno)
        {
            string s = "SELECT * FROM  qry_Cerval_Core_Student WHERE Expr2 = '" + Adno + "'";
            Load1(s);
        }

        public void Load1(string s)
        {
            m_contacts.m_contactlist.Clear();
            m_relationships.m_Relationshiplist.Clear();
            //going to hydrate the object
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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

        public void Hydrate(SqlDataReader dr)
        {
            //0 is timestamp
            m_PersonId = dr.GetGuid(1);
            //title
            m_Gender = dr.GetString(3);
            m_Surname = dr.GetString(5);
            m_GivenName = dr.GetString(4);
            if (!dr.IsDBNull(6)) m_MiddleName = dr.GetString(6); else m_MiddleName = "";
            if (!dr.IsDBNull(7)) m_InformalName = dr.GetString(7); else m_InformalName = "";
            if (!dr.IsDBNull(8)) m_dob = dr.GetDateTime(8);
            //deceased
            //title
            m_StudentId = (Guid)dr.GetSqlGuid(11);
            m_adno = dr.GetInt32(12);
            if (!dr.IsDBNull(13)) m_upn = dr.GetString(13);
            if (!dr.IsDBNull(14)) m_examNo = dr.GetInt32(14);
            //ad date ethnic-code ethnic-name
            if (!dr.IsDBNull(15)) m_doa = dr.GetDateTime(15);
            if (!dr.IsDBNull(17)) m_ethnicity = dr.GetString(17);
            if (!dr.IsDBNull(19)) m_religion = dr.GetString(19);
            if (!dr.IsDBNull(21)) m_language = dr.GetString(21);
            if (!dr.IsDBNull(23)) m_IsOnRole = dr.GetBoolean(23);
            //fsm flag ,  in care flag , 
            //dbo.tbl_Core_Students.StudentDoctor AS Expr17, 
            //dbo.tbl_Core_Students.StudentLeavingDate AS Expr18
            if (!dr.IsDBNull(27)) m_dol = dr.GetDateTime(27);
            //dbo.tbl_Core_Students.StudentLeavingReason AS Expr19,
            //dbo.tbl_List_LeavingReasons.LeavingReason AS Expr20, dbo.tbl_Core_Students.StudentLeavingDestination AS Expr21, dbo.tbl_Core_Students.StudentLeavingDetails AS Expr22,
            if (!dr.IsDBNull(32)) m_UCI = dr.GetString(32);
            if (!dr.IsDBNull(33)) m_uln = dr.GetString(33); else m_uln = "";
            if (!dr.IsDBNull(37)) m_BTECNumber = dr.GetString(37); else m_BTECNumber = "";
            m_valid = true;

            //get student address
            if (m_valid)
            {
                Encode en = new Encode();
                string db_connection = en.GetDbConnection();
                SqlConnection cn = new SqlConnection(db_connection);
                cn.Open();
                string s = "";
                m_address = "";
                m_address_commasep = "";
                for (int i = 0; i < 10; i++) { m_address_elements[i] = ""; }
                s = "SELECT * FROM qry_Cerval_Core_AddressForPerson ";
                s += "WHERE (PersonId ='" + m_PersonId.ToString() + "' )";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr1 = cm.ExecuteReader())
                    {
                        while (dr1.Read())
                        {
                            for (int i = 2; i < 10; i++)
                            {
                                if (!dr1.IsDBNull(i))
                                {
                                    s = dr1.GetString(i);
                                    m_address += s + "  ";
                                    m_address_commasep += s + ",";
                                    m_address_elements[i] = s;
                                }
                            }
                        }
                        dr1.Close();
                    }
                }
                if (m_address_commasep.Length > 0) m_address_commasep = m_address_commasep.Substring(0, m_address_commasep.Length - 1);
                m_relationships.LoadList(m_StudentId.ToString());
                m_contacts.LoadList(m_PersonId.ToString());
                cn.Close();
            }
        }

    }
    public class PupilGroupList
    {
        public ArrayList m_pupilllist = new ArrayList();
        public int m_count;
        public PupilGroupList()
        {
            m_pupilllist.Clear();
            m_count = 0;
        }
        private void AddToList1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            {
                                SimplePupil p = new SimplePupil();
                                p.Hydrate_new(dr);
                                m_pupilllist.Add(p); m_count++;
                            }

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        private string GetSelectString(DateTime time_slot, string Where_Group)
        {
            string date_s = "CONVERT(DATETIME, '" + time_slot.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "  SELECT  dbo.tbl_Core_Students.StudentId, ";
            s += " dbo.tbl_Core_Students.StudentAdmissionNumber, ";
            s += " dbo.tbl_Core_Students.StudentUPN, ";
            s += " dbo.tbl_Core_Students.StudentExamNumber, ";
            s += " dbo.tbl_Core_Students.StudentAdmissionDate, ";
            s += " dbo.tbl_Core_Students.StudentIsOnRole, ";//5
            s += " dbo.tbl_Core_People.PersonTitle, ";
            s += " dbo.tbl_Core_People.PersonGivenName, ";//7
            s += " dbo.tbl_Core_People.PersonSurname, ";
            s += " dbo.tbl_Core_People.PersonMiddleName, ";//9
            s += " dbo.tbl_Core_People.PersonDateOfBirth, ";//10
            s += " dbo.tbl_Core_People.PersonId, dbo.tbl_Core_Groups.GroupValidFrom, ";
            s += " dbo.tbl_Core_Groups.GroupValidUntil, ";//13

            s += " Form_Group, ";//14
            s += " dbo.tbl_Core_People.PersonInformalName, ";//15
            s += " dbo.tbl_Core_Students.GoogleAppsLogin, ";//16
            s += " dbo.tbl_Core_Student_Groups.MemberFrom AS Date_from, ";//17
            s += " dbo.tbl_Core_Student_Groups.MemberUntil AS Date_To, ";//18
            s += " dbo.tbl_Core_Students.StudentInReceiptOfPupilPremium  ";//19
            s += " , dbo.tbl_Core_Students.iSAMStxtSchoolID";//20
            s += " FROM  dbo.tbl_Core_Students INNER JOIN ";
            s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Students.StudentId = dbo.tbl_Core_Student_Groups.StudentId INNER JOIN ";
            s += " dbo.tbl_Core_Groups ON dbo.tbl_Core_Student_Groups.GroupId = dbo.tbl_Core_Groups.GroupId INNER JOIN ";
            s += " dbo.tbl_Core_People ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId ";
            s += " INNER JOIN dbo.Forms_at_Date('" + time_slot.ToLongDateString() + "') AS p1 ON p1.ID=dbo.tbl_Core_Students.StudentId  ";
            s += Where_Group;



            s += " AND  (dbo.tbl_Core_Groups.GroupValidFrom < " + date_s + ") AND (dbo.tbl_Core_Groups.GroupValidUntil > " + date_s + ")  ";
            s += " AND  (dbo.tbl_Core_Student_Groups.MemberUntil > " + date_s + ") AND (dbo.tbl_Core_Student_Groups.MemberFrom < " + date_s + ") ) ";


            //this to fix the wierdo SQL 2014 "bug"????   OR  groupID = "non-existant group...."
            s += "  OR  (dbo.tbl_Core_Groups.GroupId = '65010186-10aa-4160-bcc2-afedd6192172') ";
            //otherwise it times out...???!!!

            s += " ORDER BY dbo.tbl_Core_People.PersonSurname ASC, dbo.tbl_Core_People.PersonGivenName ASC, ";
            s += "  dbo.tbl_Core_Student_Groups.MemberFrom DESC, dbo.tbl_Core_Student_Groups.MemberUntil DESC ";
            return s;
        }
        public void AddToList(string group_code, DateTime time_slot)
        {
            string s = GetSelectString(time_slot, " WHERE ((dbo.tbl_Core_Groups.GroupCode ='" + group_code + "' )  ");
            AddToList1(s);
        }
        public void AddToList(Guid group_ID, DateTime time_slot)
        {
            string s = GetSelectString(time_slot, " WHERE ((dbo.tbl_Core_Groups.GroupId ='" + group_ID.ToString() + "' ) ");
            AddToList1(s);
        }
        public void AddToList(GroupList gl1, DateTime time_slot)
        {
            foreach (Group g in gl1._groups)
            {
                AddToList(g._GroupID, time_slot);
            }
        }

    }
    public class PupilPeriodList : IEnumerable
    {
        public ArrayList m_pupilTTlist = new ArrayList();
        public PupilPeriodList() { }
        public void LoadList(string dbfield, string match, bool student_join, DateTime date)
        {
            m_pupilTTlist.Clear();
            string date_s = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_Core_ScheduledPeriods.DayNo, dbo.tbl_Core_ScheduledPeriods.NonScheduled, ";
            s += " dbo.tbl_List_Days.[Day], dbo.tbl_Core_Periods.PeriodCode, dbo.tbl_Core_Periods.PeriodName, dbo.tbl_Core_Staff.StaffCode, ";
            s += " dbo.tbl_Core_Rooms.RoomCode, dbo.tbl_Core_Periods.PeriodStart, dbo.tbl_Core_Groups.GroupCode ";
            s += ", dbo.tbl_Core_ScheduledPeriods.ScheduledPeriodId   ,dbo.tbl_Core_Groups.GroupId , dbo.tbl_Core_Periods.PeriodEnd ";
            s += ", dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart, dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd ";
            s += ", dbo.tbl_Core_ScheduledPeriods.RoomId ";
            s += " FROM         dbo.tbl_Core_Groups INNER JOIN ";
            s += " dbo.tbl_Core_ScheduledPeriods ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_ScheduledPeriods.GroupId INNER JOIN ";
            s += " dbo.tbl_Core_Periods ON dbo.tbl_Core_ScheduledPeriods.PeriodId = dbo.tbl_Core_Periods.PeriodId INNER JOIN ";
            s += " dbo.tbl_List_Days ON dbo.tbl_Core_ScheduledPeriods.DayNo = dbo.tbl_List_Days.Id INNER JOIN ";
            if (student_join)
            {
                s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN ";
            }
            s += "  dbo.tbl_Core_Staff ON dbo.tbl_Core_ScheduledPeriods.StaffId = dbo.tbl_Core_Staff.StaffId INNER JOIN ";
            s += " dbo.tbl_Core_Rooms ON dbo.tbl_Core_ScheduledPeriods.RoomId = dbo.tbl_Core_Rooms.RoomId ";
            s += " INNER JOIN ";
            s += "tbl_Core_ScheduledPeriodValidity ON tbl_Core_ScheduledPeriods.ScheduledPeriodId = tbl_Core_ScheduledPeriodValidity.ScheduledPeriodId ";
            if (dbfield != "") s += "WHERE (" + dbfield + " = '" + match + "') AND ";
            else s += " WHERE ";
            s += "  (dbo.tbl_Core_Groups.GroupValidFrom < " + date_s + ") AND (dbo.tbl_Core_Groups.GroupValidUntil > " + date_s + ")";
            if (student_join)
            {
                s += " AND  (dbo.tbl_Core_Student_Groups.MemberUntil > " + date_s + ") AND (dbo.tbl_Core_Student_Groups.MemberFrom < " + date_s + ") ";
            }
            s += " AND  (dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd > " + date_s + ") AND (dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart < " + date_s + ") ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriod p = new ScheduledPeriod();
                            m_pupilTTlist.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void LoadList(string dbfield, string match, bool student_join)
        {
            m_pupilTTlist.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_Core_ScheduledPeriods.DayNo, dbo.tbl_Core_ScheduledPeriods.NonScheduled, ";
            s += " dbo.tbl_List_Days.[Day], dbo.tbl_Core_Periods.PeriodCode, dbo.tbl_Core_Periods.PeriodName, dbo.tbl_Core_Staff.StaffCode, ";
            s += " dbo.tbl_Core_Rooms.RoomCode, dbo.tbl_Core_Periods.PeriodStart, dbo.tbl_Core_Groups.GroupCode ";
            s += ", dbo.tbl_Core_ScheduledPeriods.ScheduledPeriodId  ,dbo.tbl_Core_Groups.GroupId , dbo.tbl_Core_Periods.PeriodEnd ";
            s += ", dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart, dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd, ";
            s += " dbo.tbl_Core_Rooms.RoomId ";
            s += " FROM         dbo.tbl_Core_Groups INNER JOIN ";
            s += " dbo.tbl_Core_ScheduledPeriods ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_ScheduledPeriods.GroupId INNER JOIN ";
            s += " dbo.tbl_Core_Periods ON dbo.tbl_Core_ScheduledPeriods.PeriodId = dbo.tbl_Core_Periods.PeriodId INNER JOIN ";
            s += " dbo.tbl_List_Days ON dbo.tbl_Core_ScheduledPeriods.DayNo = dbo.tbl_List_Days.Id INNER JOIN ";
            if (student_join)
            {
                s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN ";
            }
            s += "  dbo.tbl_Core_Staff ON dbo.tbl_Core_ScheduledPeriods.StaffId = dbo.tbl_Core_Staff.StaffId INNER JOIN ";
            s += " dbo.tbl_Core_Rooms ON dbo.tbl_Core_ScheduledPeriods.RoomId = dbo.tbl_Core_Rooms.RoomId ";
            s += " INNER JOIN ";
            s += "tbl_Core_ScheduledPeriodValidity ON tbl_Core_ScheduledPeriods.ScheduledPeriodId = tbl_Core_ScheduledPeriodValidity.ScheduledPeriodId ";
            s += "WHERE ";
            if (dbfield != "") s += " ( " + dbfield + " = '" + match + "')  AND  ";
            s += " (dbo.tbl_Core_Groups.GroupValidFrom < { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupValidUntil > { fn NOW() })";
            if (student_join)
            {
                s += " AND  (dbo.tbl_Core_Student_Groups.MemberUntil > { fn NOW() }) AND (dbo.tbl_Core_Student_Groups.MemberFrom < { fn NOW() }) ";
            }
            s += " AND  (dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd > { fn NOW() }) AND (dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart < { fn NOW() }) ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriod p = new ScheduledPeriod();
                            m_pupilTTlist.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_between(string dbfield, string match, bool student_join, DateTime date1, DateTime date2)
        {
            m_pupilTTlist.Clear();
            string date_s = "CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string date_end = "CONVERT(DATETIME, '" + date2.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_Core_ScheduledPeriods.DayNo, dbo.tbl_Core_ScheduledPeriods.NonScheduled, ";
            s += " dbo.tbl_List_Days.[Day], dbo.tbl_Core_Periods.PeriodCode, dbo.tbl_Core_Periods.PeriodName, dbo.tbl_Core_Staff.StaffCode, ";
            s += " dbo.tbl_Core_Rooms.RoomCode, dbo.tbl_Core_Periods.PeriodStart, dbo.tbl_Core_Groups.GroupCode ";
            s += ", dbo.tbl_Core_ScheduledPeriods.ScheduledPeriodId   ,dbo.tbl_Core_Groups.GroupId , dbo.tbl_Core_Periods.PeriodEnd ";
            s += ", dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart, dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd ";
            s += ", dbo.tbl_Core_ScheduledPeriods.RoomId ";
            s += " FROM         dbo.tbl_Core_Groups INNER JOIN ";
            s += " dbo.tbl_Core_ScheduledPeriods ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_ScheduledPeriods.GroupId INNER JOIN ";
            s += " dbo.tbl_Core_Periods ON dbo.tbl_Core_ScheduledPeriods.PeriodId = dbo.tbl_Core_Periods.PeriodId INNER JOIN ";
            s += " dbo.tbl_List_Days ON dbo.tbl_Core_ScheduledPeriods.DayNo = dbo.tbl_List_Days.Id INNER JOIN ";
            if (student_join)
            {
                s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN ";
            }
            s += "  dbo.tbl_Core_Staff ON dbo.tbl_Core_ScheduledPeriods.StaffId = dbo.tbl_Core_Staff.StaffId INNER JOIN ";
            s += " dbo.tbl_Core_Rooms ON dbo.tbl_Core_ScheduledPeriods.RoomId = dbo.tbl_Core_Rooms.RoomId ";
            s += " INNER JOIN ";
            s += "tbl_Core_ScheduledPeriodValidity ON tbl_Core_ScheduledPeriods.ScheduledPeriodId = tbl_Core_ScheduledPeriodValidity.ScheduledPeriodId ";
            if (dbfield != "") s += "WHERE (" + dbfield + " = '" + match + "') AND ";
            else s += " WHERE ";
            s += "  (dbo.tbl_Core_Groups.GroupValidFrom < " + date_s + ") AND (dbo.tbl_Core_Groups.GroupValidUntil > " + date_end + ")";
            if (student_join)
            {
                s += " AND  (dbo.tbl_Core_Student_Groups.MemberUntil > " + date_s + ") AND (dbo.tbl_Core_Student_Groups.MemberFrom < " + date_end + ") ";
            }
            s += " AND  (dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd > " + date_s + ") AND (dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart < " + date_s + ") ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriod p = new ScheduledPeriod();
                            m_pupilTTlist.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Clean_for_cover()
        {
            //going to remove any scheduled periods from list where there is a shorter validity period one...

            foreach (ScheduledPeriod p1 in m_pupilTTlist)
            {
                foreach (ScheduledPeriod p2 in m_pupilTTlist)
                {
                    if ((p1.m_Id != p2.m_Id) && (p1.m_daycode == p2.m_daycode) && (p1.m_periodcode == p2.m_periodcode))
                    {
                        //not the same.. but same period and day....
                        TimeSpan t1 = p1.m_ValidityEnd - p1.m_ValidityStart;
                        TimeSpan t2 = p2.m_ValidityEnd - p2.m_ValidityStart;
                        if (t2 > t1) p2.m_valid = false;
                        if (t1 > t2) p1.m_valid = false;
                    }
                }
            }

        }


        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return (m_pupilTTlist as IEnumerable).GetEnumerator();
        }

        #endregion
    }

    public class Registrations
    {
        public string m_Adno;
        public string m_staff;
        public DateTime m_date;
        public string m_period;
        public string status;

        public Registrations()
        {
            m_date = new DateTime();
        }
    }

    public class RegistrationsList
    {
        public List<Registrations> Registrations = new List<Registrations>();

        public void Get_Recent_Registrations(string adno)
        {
            IntPtr token = IntPtr.Zero;

            bool isSuccess = LogonUser("test-staff", "challoners", "123Password",
            LOGON32_LOGON_NEW_CREDENTIALS,
            LOGON32_PROVIDER_DEFAULT, ref token);
            using (WindowsImpersonationContext person = new WindowsIdentity(token).Impersonate())
            {
                int i = 0; string s = ""; int imax = 0; string sfile1 = ""; string sfile = "";
                string[] fileEntries = Directory.GetFiles(@"\\registration.challoners.net\STEARsoft\Reg\data", "attend*.csv");
                foreach (string fileName in fileEntries)
                {
                    //find last one!
                    s = fileName.Substring(fileName.IndexOf("log"));
                    try
                    {
                        s = s.Substring(3, 2);
                        i = System.Convert.ToInt32(s);
                        if (i > imax) { imax = i; sfile1 = sfile; sfile = fileName; }
                    }
                    catch
                    {
                        try
                        {
                            s = s.Substring(3, 3);
                            i = System.Convert.ToInt32(s);
                            if (i > imax) { imax = i; sfile1 = sfile; sfile = fileName; }
                        }
                        catch { }
                    }



                }
                if (imax > 0)
                {

                    //if (Registrations.Count== 0)
                    {
                        //ought to try sfile1
                        StreamReader sr2 = new StreamReader(sfile1);
                        while (!sr2.EndOfStream)
                        {
                            s = sr2.ReadLine();
                            string[] fred = s.Split((char)(','));
                            if (fred[4] == adno)
                            {
                                Registrations r = new Registrations();
                                r.m_Adno = adno; r.m_staff = fred[0]; r.m_period = fred[9]; r.status = fred[11];
                                switch (fred[11].Substring(0, 1))
                                {
                                    case "1": r.status = "Present   "; break;
                                    case "0": r.status = "Absent   "; break;
                                    case "L": r.status = "Late   "; break;
                                    default: break;
                                }
                                if (fred[11].Length > 1) r.status += fred[11].Substring(1);
                                r.m_date = System.Convert.ToDateTime(fred[1] + " " + fred[2]);

                                Registrations.Add(r);
                            }
                        }
                        sr2.Close();

                        StreamReader sr1 = new StreamReader(sfile);
                        while (!sr1.EndOfStream)
                        {
                            s = sr1.ReadLine();
                            string[] fred = s.Split((char)(','));

                            if (fred[4] == adno)
                            {
                                Registrations r = new Registrations();
                                r.m_Adno = adno; r.m_staff = fred[0]; r.m_period = fred[9]; r.status = fred[11];

                                switch (fred[11].Substring(0, 1))
                                {
                                    case "1": r.status = "Present  "; break;
                                    case "0": r.status = "Absent   "; break;
                                    case "L": r.status = "Late   "; break;
                                    default: break;
                                }
                                if (fred[11].Length > 1) r.status += fred[11].Substring(1);
                                r.m_date = System.Convert.ToDateTime(fred[1] + " " + fred[2]);

                                Registrations.Add(r);
                            }
                        }
                        sr1.Close();
                    }
                }
                //list will be in reverse order
                Registrations.Reverse();
                person.Undo();
            }
        }
        #region imports 
        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string
        lpszUsername, string lpszDomain, string lpszPassword,
        int dwLogonType, int dwLogonProvider, ref
        IntPtr phToken);


        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = CharSet.Auto,
        SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle
        );

        [DllImport("advapi32.dll", CharSet = CharSet.Auto,
        SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr
        existingTokenHandle,
        int SECURITY_IMPERSONATION_LEVEL, ref IntPtr
        duplicateTokenHandle);
        #endregion
        #region logon consts 
        // logon types 
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        // logon providers 
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_PROVIDER_WINNT50 = 3;
        const int LOGON32_PROVIDER_WINNT40 = 2;
        const int LOGON32_PROVIDER_WINNT35 = 1;
        #endregion
    }


    public class Relationship
    {
        public Guid m_PersonID;
        public string m_RelationshipType;
        public string m_RelationshipDesc;
        public string m_Title;
        public string m_PersonGivenName;
        public string m_PersonSurname;
        public string m_Address;
        public int m_RelationshipTypeId;
        public int m_RelationshipPriority;
        public ContactList m_contactlist;
        public Relationship()
        {
            m_contactlist = new ContactList();
        }
    }
    public class RelationshipList
    {
        public ArrayList m_Relationshiplist = new ArrayList();
        public RelationshipList() { m_Relationshiplist.Clear(); }
        public void LoadList(string StudentID)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT dbo.tbl_List_RelationshipTypes.RelationshipType, dbo.tbl_List_RelationshipTypes.Description, dbo.tbl_List_PersonalTitles.Title, ";
            s += "dbo.tbl_Core_People.PersonGivenName, dbo.tbl_Core_People.PersonSurname, dbo.tbl_Core_Addresses.AddressSubDwelling, ";
            s += "dbo.tbl_Core_Addresses.AddressDwelling, dbo.tbl_Core_Addresses.AddressStreet, dbo.tbl_Core_Addresses.AddressLocality, ";
            s += "dbo.tbl_Core_Addresses.AddressTown, dbo.tbl_Core_Addresses.AddressPostTown, dbo.tbl_Core_Addresses.AddressPostCode, ";
            s += " dbo.tbl_Core_Student_Relationships.PersonId, dbo.tbl_List_RelationshipTypes.Id, tbl_Core_Student_Relationships.RelationshipPriority ";
            s += "FROM         dbo.tbl_List_RelationshipTypes INNER JOIN ";
            s += "dbo.tbl_Core_Student_Relationships ON dbo.tbl_List_RelationshipTypes.Id = dbo.tbl_Core_Student_Relationships.RelationshipType INNER JOIN ";
            s += "dbo.tbl_Core_People ON dbo.tbl_Core_Student_Relationships.PersonId = dbo.tbl_Core_People.PersonId INNER JOIN ";
            s += "dbo.tbl_List_PersonalTitles ON dbo.tbl_Core_People.PersonTitle = dbo.tbl_List_PersonalTitles.Id INNER JOIN ";
            s += "dbo.tbl_Core_People_Addresses ON dbo.tbl_Core_People.PersonId = dbo.tbl_Core_People_Addresses.PersonId INNER JOIN ";
            s += "dbo.tbl_Core_Addresses ON dbo.tbl_Core_People_Addresses.AddressId = dbo.tbl_Core_Addresses.AddressId ";
            s += " WHERE dbo.tbl_Core_Student_Relationships.StudentId ='{";
            s += StudentID + "}'  AND dbo.tbl_Core_People.PersonDeceased = '0' ";
            s += " ORDER BY tbl_Core_Student_Relationships.RelationshipPriority ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Relationship r = new Relationship();
                            m_Relationshiplist.Add(r);
                            r.m_RelationshipType = dr.GetString(0);
                            if (!dr.IsDBNull(1)) r.m_RelationshipDesc = dr.GetString(1);
                            r.m_Title = dr.GetString(2);
                            r.m_PersonGivenName = dr.GetString(3);
                            r.m_PersonSurname = dr.GetString(4);
                            r.m_Address = "";
                            if (!dr.IsDBNull(5)) r.m_Address = dr.GetString(5);
                            for (int i = 6; i < 12; i++)
                            {
                                if (!dr.IsDBNull(i)) r.m_Address += "  " + dr.GetString(i);
                            }
                            r.m_PersonID = dr.GetGuid(12);
                            r.m_RelationshipTypeId = dr.GetInt32(13);
                            r.m_RelationshipPriority = dr.GetByte(14);
                            r.m_contactlist.LoadList(r.m_PersonID.ToString());
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class ReportComment
    {
        public Guid m_studentId;
        public Guid m_courseId;
        public Guid m_staffId;
        public string m_content;
        public DateTime m_dateCreated;
        public DateTime m_dateModified;
        public Guid m_collectionOutputPolicyId;
        public int m_commentType;

        public int m_PolicyYear;
        public string m_PolicyName;
        public DateTime m_PolicyCollectionEnd;

        public string m_CourseName;
        public string m_staffCode;
        public string m_OutputPolicyFreeTextInternalName;// seems to be a copy of comment type...??

        public ReportComment()
        {
        }

        public void Hydrate(SqlDataReader dr)
        {
            m_studentId = dr.GetGuid(0);
            m_courseId = dr.GetGuid(1);
            m_staffId = dr.GetGuid(2);
            m_content = dr.GetString(3);
            m_dateCreated = dr.GetDateTime(4);
            m_dateModified = dr.GetDateTime(5);
            m_collectionOutputPolicyId = dr.GetGuid(6);
            if (!dr.IsDBNull(7)) m_commentType = dr.GetInt32(7); else m_commentType = 0;
            m_PolicyYear = dr.GetInt32(8);
            m_PolicyName = dr.GetString(9);
            m_PolicyCollectionEnd = dr.GetDateTime(10);
            m_CourseName = dr.GetString(11);
            m_staffCode = dr.GetString(12);
            m_OutputPolicyFreeTextInternalName = dr.GetString(13);
            if (m_commentType == 0)
            {
                if (m_OutputPolicyFreeTextInternalName == "PNTSIMPR") m_commentType = 1;
                if (m_OutputPolicyFreeTextInternalName == "FURTCOMM") m_commentType = 2;
            }
        }


        public void Save()
        {
            Utility u = new Utility();
            Encode en = new Encode();
            if (m_commentType == 1) m_OutputPolicyFreeTextInternalName = "PNTSIMPR";
            if (m_commentType == 2) m_OutputPolicyFreeTextInternalName = "FURTCOMM";
            string date_s = "CONVERT(DATETIME, '" + m_dateCreated.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "INSERT INTO dbo.tbl_Core_Assessment_FreeTextComments ";
            s += " (StudentID, CourseID, StaffID, FreeTextContent,DateCreated, DateLastModified,CollectionOutputPolicyId, ";
            s += " CommentType, CollectionOutputPolicyFreeTextInternalName )";
            s += " VALUES ( '" + m_studentId.ToString() + "' , '" + m_courseId.ToString() + "' , '" + m_staffId.ToString() + "' , ";
            s += " '" + u.CleanInvertedCommas(m_content) + "'," + date_s + " , " + date_s + " , ";
            s += " '" + m_collectionOutputPolicyId.ToString() + "','" + m_commentType.ToString() + "' , '" + m_OutputPolicyFreeTextInternalName + "' ) ";
            en.ExecuteSQL(s);
        }

    }
    public class ReportCommentList
    {
        public ArrayList m_list = new ArrayList();
        public ReportCommentList()
        {
            m_list.Clear();
        }




        public ReportCommentList(string student_id)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT  dbo.tbl_Core_Assessment_FreeTextComments.StudentID, dbo.tbl_Core_Assessment_FreeTextComments.CourseID, ";
            s += " dbo.tbl_Core_Assessment_FreeTextComments.StaffID, dbo.tbl_Core_Assessment_FreeTextComments.FreeTextContent,  ";
            s += " dbo.tbl_Core_Assessment_FreeTextComments.DateCreated, dbo.tbl_Core_Assessment_FreeTextComments.DateLastModified,  ";
            s += " dbo.tbl_Core_Assessment_FreeTextComments.CollectionOutputPolicyId, dbo.tbl_Core_Assessment_FreeTextComments.CommentType,  ";
            s += " dbo.tbl_Core_Assessment_OutputPolicies.AcademicYear, dbo.tbl_Core_Assessment_OutputPolicies.Name,  ";
            s += " dbo.tbl_Core_Assessment_OutputPolicies.CollectionTo, dbo.tbl_Core_Courses.CourseName, dbo.tbl_Core_Staff.StaffCode,  ";
            s += " dbo.tbl_Core_Assessment_FreeTextComments.CollectionOutputPolicyFreeTextInternalName ";
            s += " FROM  dbo.tbl_Core_Assessment_FreeTextComments INNER JOIN ";
            s += " dbo.tbl_Core_Courses ON dbo.tbl_Core_Assessment_FreeTextComments.CourseID = dbo.tbl_Core_Courses.CourseId INNER JOIN ";
            s += " dbo.tbl_Core_Staff ON dbo.tbl_Core_Assessment_FreeTextComments.StaffID = dbo.tbl_Core_Staff.StaffId INNER JOIN ";
            s += " dbo.tbl_Core_Assessment_OutputPolicies ON  ";
            s += " dbo.tbl_Core_Assessment_FreeTextComments.CollectionOutputPolicyId = dbo.tbl_Core_Assessment_OutputPolicies.OutputPolicyId ";
            s += " WHERE (dbo.tbl_Core_Assessment_FreeTextComments.StudentID = '" + student_id.ToString() + "') ";
            s += " ORDER BY dbo.tbl_Core_Assessment_FreeTextComments.DateLastModified DESC ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ReportComment r = new ReportComment();
                            r.Hydrate(dr);
                            //now only really want if it is a new comment.....
                            if (!ContainsComment(r)) m_list.Add(r);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public bool ContainsComment(ReportComment r0)
        {
            foreach (ReportComment r in m_list)
            {
                if ((r0.m_studentId == r.m_studentId) && (r0.m_courseId == r.m_courseId) && (r0.m_staffId == r.m_staffId) && (r0.m_commentType == r.m_commentType) && (r0.m_collectionOutputPolicyId == r.m_collectionOutputPolicyId)) return true;
            }
            return false;
        }

    }

    public class ReportScale
    {
        public decimal MaxValue;
        public Report_Assessment_Level AssessmentLevelDetail = new Report_Assessment_Level();
        public ReportScale(Guid SkillID)
        {
            //so going to check the tbl_Core_Assessment_Levels for max value for this skill
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT MAX(LevelNumeric) FROM tbl_Core_Assessment_Levels WHERE SkillID = '" + SkillID.ToString() + "' ";
            string s1 = en.ExecuteScalarSQL(s);
            //this should be a numeric value
            try
            {
                MaxValue = System.Convert.ToDecimal(s1);
                s = "SELECT LevelID FROM tbl_Core_Assessment_Levels WHERE ( SkillID = '" + SkillID.ToString() + "')  AND (LevelNumeric='" + s1 + "') ";
                string s2 = en.ExecuteScalarSQL(s);
                AssessmentLevelDetail.Load(s2);
            }
            catch
            {
                MaxValue = 0;
            }
        }
    }

    public class Report_Assessment_Level
    {
        public Guid LevelID { get; set; }
        public Guid SkillID { get; set; }
        public string LevelName { get; set; }
        public string LevelGrade { get; set; }
        public string LevelDesc { get; set; }
        public int LevelNumeric { get; set; }
        public string LevelDisplayFormat { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public void Load(string LevelID)
        {
            string s = LevelID;
            Guid g = new Guid(LevelID);
            Load(g);
        }

        public void Load(Guid LevelID)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM tbl_Core_Assessment_Levels WHERE LevelID = '" + LevelID.ToString() + "' ";
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

        public void Hydrate(SqlDataReader dr)
        {
            LevelID = dr.GetGuid(0);
            SkillID = dr.GetGuid(1);
            LevelName = dr.GetString(2);
            LevelGrade = dr.GetString(3);
            LevelDesc = dr.GetString(4);
            LevelNumeric = dr.GetInt32(5);
            if (!dr.IsDBNull(6)) LevelDisplayFormat = dr.GetString(6);
            if (!dr.IsDBNull(7)) ValidFrom = dr.GetDateTime(7);
            if (!dr.IsDBNull(8)) ValidTo = dr.GetDateTime(8);
        }
    }

    public class ReportValue
    {
        public double m_value;
        public string m_course;
        public int m_courseType;
        public DateTime m_date;
        public Guid m_skillID;
        public string m_skillText;
        public bool m_IsCommitment;
        public string m_staff;//TO DO>>>
        public int m_MaxLevel;
        public string m_Max_Grade;//  will be n/a for commitment and either * for old GCSE etc or 10...???

        public void Hydrate(SqlDataReader dr)
        {
            m_course = dr.GetString(0);
            m_value = (double)dr.GetDecimal(1);
            m_date = dr.GetDateTime(2);
            m_courseType = dr.GetInt32(3);
            m_skillID = dr.GetGuid(4);
            m_skillText = dr.GetString(5);
            m_staff = dr.GetString(6);
            if (!dr.IsDBNull(7)) m_MaxLevel = dr.GetInt32(7); else m_MaxLevel = 0;
            if (!dr.IsDBNull(8)) m_Max_Grade = dr.GetString(8); else m_Max_Grade = "";
            if (m_skillText.ToLower().Contains("commitment")) m_IsCommitment = true; else m_IsCommitment = false;
        }
    }
    public class ReportList
    {
        public ArrayList m_list = new ArrayList();
        public ReportList()
        {
            m_list.Clear();
        }

        public void Load1(string s)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ReportValue r = new ReportValue();
                            m_list.Add(r);
                            r.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            //now go to sort these into the right averages... or maybe just take latest ones each term?
            //going to normalise dates to be next end of term... 20/12   or 1/4  or 31/7
            int m = 0; int d = 0;
            foreach (ReportValue r in m_list)
            {
                m = r.m_date.Month; d = r.m_date.Day;
                if ((m >= 4) && (m < 9))
                {
                    r.m_date = new DateTime(r.m_date.Year, 7, 31);
                }
                if ((m >= 9))
                {
                    r.m_date = new DateTime(r.m_date.Year, 12, 20);
                }
                if ((m >= 1) && (m < 4))
                {
                    r.m_date = new DateTime(r.m_date.Year, 4, 1);
                }

            }

        }


        public string ReportListQuery(string where)
        {
            string s = "SELECT tbl_Core_Courses.CourseCode, ";
            s += " tbl_Core_Assessment_IndicatorSavePoints.IndicatorValue, ";
            s += "tbl_Core_Assessment_IndicatorSavePoints.DateCreated, ";
            s += " tbl_Core_Courses.CourseType, tbl_Core_Assessment_IndicatorSavePoints.SkillID, ";
            s += " tbl_Core_Assessment_Skills.SkillText, ";
            s += " dbo.tbl_Core_Staff.StaffCode, ";
            s += " MAX(dbo.tbl_Core_Assessment_Levels.LevelNumeric) AS MaxLevel, ";
            s += " MIN(dbo.tbl_Core_Assessment_Levels.LevelGrade)AS Expr1 ";

            s += "FROM  tbl_Core_Assessment_IndicatorSavePoints ";
            s += " INNER JOIN  tbl_Core_Courses ";
            s += " ON tbl_Core_Assessment_IndicatorSavePoints.CourseID = tbl_Core_Courses.CourseId ";
            s += "  INNER JOIN  tbl_Core_Assessment_Skills ";
            s += " ON tbl_Core_Assessment_IndicatorSavePoints.SkillID = tbl_Core_Assessment_Skills.SkillID ";
            s += " INNER JOIN dbo.tbl_Core_Staff ON dbo.tbl_Core_Staff.StaffId = tbl_Core_Assessment_IndicatorSavePoints.StaffID ";
            s += " INNER JOIN dbo.tbl_Core_Assessment_Levels ";
            s += " ON dbo.tbl_Core_Assessment_Levels.SkillID = dbo.tbl_Core_Assessment_IndicatorSavePoints.SkillID ";

            s += where;
            s += "GROUP BY dbo.tbl_Core_Courses.CourseCode, dbo.tbl_Core_Assessment_IndicatorSavePoints.IndicatorValue, ";
            s += "dbo.tbl_Core_Assessment_IndicatorSavePoints.DateCreated, dbo.tbl_Core_Courses.CourseType, ";
            s += " dbo.tbl_Core_Assessment_IndicatorSavePoints.SkillID, dbo.tbl_Core_Assessment_Skills.SkillText, dbo.tbl_Core_Staff.StaffCode ";
            s += " ORDER BY tbl_Core_Assessment_IndicatorSavePoints.DateCreated ASC, tbl_Core_Courses.CourseCode ";
            return s;

        }
        public ReportList(string student_id)
        {

            string s = "WHERE (tbl_Core_Assessment_IndicatorSavePoints.StudentId = '" + student_id.ToString() + "') ";
            Load1(ReportListQuery(s));
        }
        public ReportList(string student_id, int Course_type)
        {
            string s = "WHERE (tbl_Core_Assessment_IndicatorSavePoints.StudentId = '" + student_id.ToString() + "') ";
            s += " AND (tbl_Core_Courses.CourseType = '" + Course_type.ToString() + "')";
            Load1(ReportListQuery(s));

        }
        public ReportList(string student_id, Guid CourseID)
        {
            string s = "WHERE (tbl_Core_Assessment_IndicatorSavePoints.StudentId = '" + student_id.ToString() + "') ";
            s += " AND (tbl_Core_Assessment_IndicatorSavePoints.CourseID = '" + CourseID.ToString() + "')";
            Load1(ReportListQuery(s));
        }
        public ReportList(string student_id, Guid CourseID, DateTime StartDate, DateTime EndDate)
        {
            string date_s = "CONVERT(DATETIME, '" + StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string date_e = "CONVERT(DATETIME, '" + EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " AND (tbl_Core_Assessment_IndicatorSavePoints.CourseID = '" + CourseID.ToString() + "')";
            s += " AND (tbl_Core_Assessment_IndicatorSavePoints.DateCreated >" + date_s + " ) ";
            s += " AND (tbl_Core_Assessment_IndicatorSavePoints.DateCreated <" + date_e + " ) ";
            Load1(ReportListQuery(s));
        }
        public void ReduceToAttainment()
        {
            //going to reduce all the skills to one attainment skill...
            ReportValue v1; ReportValue v2;
            bool ff = false; int n = 0; double sum = 0;
            //need to sort list so all commitment at either start or end.....
            ArrayList m_list2 = new ArrayList();
            ArrayList m_list3 = new ArrayList();
            for (int i = 0; i < m_list.Count; i++)
            {
                v1 = (ReportValue)m_list[i];
                if (!v1.m_IsCommitment)
                {
                    m_list2.Add(m_list[i]);
                }
                else
                {
                    m_list3.Add(m_list[i]);
                }
            }


            for (int i = 1; i < m_list2.Count; i++)
            {
                v1 = (ReportValue)m_list2[i];
                v2 = (ReportValue)m_list2[i - 1];

                if ((v1.m_course == v2.m_course) && (v1.m_date == v2.m_date) && (!v1.m_IsCommitment) && (!v2.m_IsCommitment))
                {
                    sum += v2.m_value; n++; ff = true;
                    v2.m_value = 0;
                }
                else
                {
                    if (ff)
                    {
                        ff = false;
                        sum += v2.m_value; n++;
                        v2.m_value = sum / n;
                        v2.m_skillText = "Attainment";//mark as composite...
                        //v2.m_skillID = Guid.Empty;
                        sum = 0.0; n = 0;
                    }
                }
            }
            //now remove the nulls...
            for (int i = 0; i < m_list2.Count; i++)
            {
                v1 = (ReportValue)m_list2[i];
                if (v1.m_value == 0)
                {
                    m_list2.Remove(v1); i--;
                }
            }
            m_list.Clear();
            for (int i = 0; i < m_list2.Count; i++)
            {
                m_list.Add(m_list2[i]);
            }
            for (int i = 0; i < m_list3.Count; i++)
            {
                m_list.Add(m_list3[i]);
            }
        }

    }
    public class ReportPolicy
    {
        public Guid OutputPolicyId;
        public int AcademicYear;
        public string Name;
        public DateTime CollectionFrom;
        public DateTime CollectionTo;
        public void Hydrate(SqlDataReader dr)
        {
            OutputPolicyId = dr.GetGuid(0);
            AcademicYear = dr.GetInt32(1);
            Name = dr.GetString(2);
            CollectionFrom = dr.GetDateTime(3);
            CollectionTo = dr.GetDateTime(4);
        }
        public void Load(Guid OutputPolicyId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT OutputPolicyId, AcademicYear, Name, CollectionFrom, CollectionTo ";
            s += "FROM tbl_Core_Assessment_OutputPolicies ";
            s += " WHERE OutputPolicyId='" + OutputPolicyId.ToString() + "' ";
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
                    }
                }
            }

        }
    }
    public class ReportPolicyList
    {
        public List<ReportPolicy> m_list = new List<ReportPolicy>();
        public ReportPolicyList()
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT OutputPolicyId, AcademicYear, Name, CollectionFrom, CollectionTo ";
            s += "FROM tbl_Core_Assessment_OutputPolicies ";
            s += " ORDER BY CollectionTo DESC ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ReportPolicy p = new ReportPolicy();
                            p.Hydrate(dr);
                            m_list.Add(p);
                        }
                    }
                }
            }

        }
    }
    [Serializable]
    public class Result
    {
        #region "Private Variables"

        private Guid m_personID;
        private string m_surname;
        private string m_givenname;
        private Guid m_studentID;
        private string m_value;
        private string m_text;
        private System.DateTime m_date;
        private string m_coursename;
        private string m_code;//course code 2 char
        private string m_shortname;
        private bool m_external;
        private bool m_numeric;
        private string m_description;
        private string m_allowed;
        private int m_max;
        private int m_min;
        private string m_coursetype;
        private int m_keystage;
        private int m_adno;
        private Guid m_CourseID;
        private bool m_valid = false;
        private int m_ResultType;
        private Guid m_OptionID;
        private string m_OptionCode;
        private string m_SyllabusCode;
        private string m_OptionTitle;
        private string m_SyllabusTitle;
        private string m_OptionQualification;
        private string m_OptionLevel;
        private string m_OptionItem;
        private string m_OptionMaximumMark;
        private Guid m_ResultID;

        #endregion

        #region "Properties"
        public string Surname { get { return (m_surname); } set { m_surname = value; } }
        public Guid PersonID { get { return (m_personID); } }
        public Guid StudentID { get { return (m_studentID); } set { m_studentID = value; } }
        public string Givenname { get { return (m_givenname); } }
        public string Value { get { return (m_value); } set { m_value = value; } }
        public string Text { get { return (m_text); } set { m_text = value; } }
        public System.DateTime Date { get { return (m_date); } set { m_date = value; } }
        public string Coursename { get { return (m_coursename); } }
        public string Code { get { return (m_code); } set { m_code = value; } }
        public string Shortname { get { return (m_shortname); } set { m_shortname = value; } }
        public bool External { get { return (m_external); } set { m_external = value; } }
        public bool Numeric { get { return (m_numeric); } }
        public string Description { get { return (m_description); } }
        public string Allowed { get { return (m_allowed); } }
        public int Max { get { return (m_max); } }
        public int Min { get { return (m_min); } }
        public string Coursetype { get { return (m_coursetype); } }
        public int Keystage { get { return (m_keystage); } }
        public int Adno { get { return (m_adno); } }
        public Guid CourseID { get { return (m_CourseID); } set { m_CourseID = value; } }
        public bool Valid { get { return (m_valid); } set { m_valid = value; } }
        public int Resulttype { get { return (m_ResultType); } set { m_ResultType = value; } }

        public Guid OptionId { get { return (m_OptionID); } set { m_OptionID = value; } }
        public string OptionCode { get { return (m_OptionCode); } }
        public string SyllabusCode { get { return (m_SyllabusCode); } }
        public string OptionTitle { get { return (m_OptionTitle); } }
        public string SyllabusTitle { get { return (m_SyllabusTitle); } }
        public string OptionQualification { get { return (m_OptionQualification); } }
        public string OptionLevel { get { return (m_OptionLevel); } }
        public string OptionItem { get { return (m_OptionItem); } }
        public string OptionMaximumMark { get { return (m_OptionMaximumMark); } }
        public Guid ResultID { get { return (m_ResultID); } set { m_ResultID = value; } }
        #endregion

        #region "Methods"

        public Result()
        {
            m_ResultID = Guid.Empty;
        }
        /// <summary>
        /// Overloaded method to create & hydrate
        /// </summary>
        /// <param name="dr"></param>
        public Result(SqlDataReader dr)
        {
            Hydrate(dr);
        }

        /// <summary>
        /// Method to load data from the sqldata reader dr into the object.
        /// 
        /// </summary>
        /// <param name="dr"></param>
        public void Hydrate(SqlDataReader dr)
        {
            m_valid = false;
            try
            {
                m_personID = dr.GetGuid(0);
                m_surname = dr.GetString(1);
                m_givenname = dr.GetString(2);
                m_studentID = dr.GetGuid(3);
                if (!dr.IsDBNull(4)) m_value = dr.GetString(4); else m_value = "";
                if (!dr.IsDBNull(5)) m_text = dr.GetString(5); else m_text = "";
                m_date = dr.GetDateTime(6);
                m_coursename = dr.GetString(7);
                m_code = dr.GetString(8);
                m_shortname = dr.GetString(9);
                m_external = dr.GetBoolean(10);
                m_numeric = dr.GetBoolean(11);
                m_description = dr.GetString(12);
                m_allowed = dr.GetString(13);
                if (!dr.IsDBNull(14)) m_max = dr.GetInt32(14); else m_max = 0;
                if (!dr.IsDBNull(15)) m_min = dr.GetInt32(15); else m_min = 0;
                m_coursetype = dr.GetString(16);
                if (!dr.IsDBNull(17)) m_keystage = dr.GetInt32(17); else m_keystage = 0;
                m_adno = dr.GetInt32(18);
                if (!dr.IsDBNull(19)) m_OptionID = dr.GetGuid(19);
                if (!dr.IsDBNull(20)) m_OptionCode = dr.GetString(20); else m_OptionCode = "";
                if (!dr.IsDBNull(21)) m_OptionTitle = dr.GetString(21); else m_OptionTitle = "";
                m_CourseID = dr.GetGuid(22);
                m_ResultType = dr.GetInt32(23);
                if (!dr.IsDBNull(24)) m_OptionQualification = dr.GetString(24);
                if (!dr.IsDBNull(25)) m_OptionLevel = dr.GetString(25);
                if (!dr.IsDBNull(26)) m_OptionItem = dr.GetString(26);
                if (!dr.IsDBNull(27)) m_OptionMaximumMark = dr.GetString(27);
                if (!dr.IsDBNull(28)) m_SyllabusCode = dr.GetString(28);
                if (!dr.IsDBNull(29)) m_SyllabusTitle = dr.GetString(29);
                if (!dr.IsDBNull(30)) m_ResultID = dr.GetGuid(30);
                m_valid = true;
            }
            catch
            {
                m_valid = false;
            }

        }

        public void Save(string OrganisationID)
        {
            string s = "";
            if (m_ResultID == Guid.Empty)
            {
                s = "INSERT INTO dbo.tbl_Core_Results (ResultType, StudentID, ResultValue, ResultDate, ResultText, ";
                s += "ExamsOptionId, CourseId, OrganisationTakenAtId, Version )";
                s += " VALUES ( '" + m_ResultType + "', '";
                s += m_studentID.ToString() + "', '";
                s += m_value + "', ";
                s += " CONVERT(DATETIME, '" + m_date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '";
                s += m_text + "',  '";
                s += m_OptionID.ToString() + "', '";
                s += m_CourseID.ToString() + "', '";
                s += OrganisationID + "' ,'5' )";
            }
            else
            {
                //update...
                s = "UPDATE dbo.tbl_Core_Results ";
                s += " SET ResultType = '" + m_ResultType + "' ";
                s += ", ResultValue = '" + m_value + "' ";
                s += ", StudentID = '" + m_studentID.ToString() + "' ";
                s += ", ResultDate = CONVERT(DATETIME, '" + m_date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += ", ResultText = '" + m_text + "' ";
                s += ", ExamsOptionId = '" + m_OptionID.ToString() + "' ";
                s += ", CourseId = '" + m_CourseID.ToString() + "' ";
                s += ", OrganisationTakenAtId = '" + OrganisationID + "' ";
                s += " WHERE (ResultID = '" + m_ResultID.ToString() + "' ) ";

            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public void Update()
        {
            string s = ""; //update...
            if (m_ResultID == Guid.Empty) return;
            s = "UPDATE dbo.tbl_Core_Results ";
            s += " SET ResultType = '" + m_ResultType + "' ";
            s += ", ResultValue = '" + m_value + "' ";
            s += ", StudentID = '" + m_studentID.ToString() + "' ";
            s += ", ResultDate = CONVERT(DATETIME, '" + m_date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
            s += ", ResultText = '" + m_text + "' ";
            s += ", ExamsOptionId = '" + m_OptionID.ToString() + "' ";
            s += ", CourseId = '" + m_CourseID.ToString() + "' ";
            s += " WHERE (ResultID = '" + m_ResultID.ToString() + "' ) ";

            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public void UpdateResultTextOnly()
        {
            string s = ""; //update...
            if (m_ResultID == Guid.Empty) return;
            s = "UPDATE dbo.tbl_Core_Results ";
            s += " SET ResultText = '" + m_text + "' ";
            s += " WHERE (ResultID = '" + m_ResultID.ToString() + "' ) ";
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public void Save_NoExamOption_Unique(string OrganisationID)
        {
            //only insert new if no previous result of this type...
            //used for mean GCSE updating...
            string s = "";
            Encode en = new Encode();
            if (m_ResultID == Guid.Empty)
            {
                s = "SELECT * FROM dbo.tbl_Core_Results ";
                s += "WHERE (ResultType='" + m_ResultType + "' ) ";
                s += " AND (StudentID='" + m_studentID.ToString() + "' ) ";
                s += " AND (CourseId='" + m_CourseID.ToString() + "' ) ";
                m_ResultID = en.FindId(s);
            }
            Save_NoExamOption(OrganisationID);
        }

        public void UpdateDateOnly()
        {
            string s = "";
            Encode en = new Encode();
            if (m_ResultID == Guid.Empty) return;
            s = "UPDATE dbo.tbl_Core_Results ";
            s += " SET ResultDate = CONVERT(DATETIME, '" + m_date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
            s += " WHERE (ResultID = '" + m_ResultID.ToString() + "' ) ";
            en.ExecuteSQL(s);
        }

        public void Save_NoExamOption(string OrganisationID)
        {
            string s = "";
            Encode en = new Encode();
            if (m_ResultID == Guid.Empty)
            {
                //try to load it first...???
                //look for same data within a month??
                s = "SELECT * FROM dbo.tbl_Core_Results ";
                s += "WHERE (ResultType='" + m_ResultType + "' ) ";
                s += " AND (StudentID='" + m_studentID.ToString() + "' ) ";
                s += " AND (ResultDate>CONVERT(DATETIME, '" + m_date.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ) ";
                s += " AND (CourseId='" + m_CourseID.ToString() + "' ) ";
                m_ResultID = en.FindId(s);
            }

            if (m_ResultID != Guid.Empty)
            {
                //so have id....
                s = "UPDATE dbo.tbl_Core_Results ";
                s += " SET ResultType = '" + m_ResultType + "' ";
                s += ", ResultValue = '" + m_value + "' ";
                s += ", StudentID = '" + m_studentID.ToString() + "' ";
                s += ", ResultDate = CONVERT(DATETIME, '" + m_date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += ", ResultText = '" + m_text + "' ";
                s += ", CourseId = '" + m_CourseID.ToString() + "' ";
                s += ", OrganisationTakenAtId = '" + OrganisationID + "' ";
                s += " WHERE (ResultID = '" + m_ResultID.ToString() + "' ) ";

                en.ExecuteSQL(s);
            }
            else
            {
                s = "INSERT INTO dbo.tbl_Core_Results (ResultType, StudentID, ResultValue, ResultDate, ResultText, ";
                s += " CourseId, OrganisationTakenAtId, Version )";
                s += " VALUES ( '" + m_ResultType + "', '";
                s += m_studentID.ToString() + "', '";
                s += m_value + "', ";
                s += " CONVERT(DATETIME, '" + m_date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '";
                s += m_text + "',  '";
                s += m_CourseID.ToString() + "', '";
                s += OrganisationID + "' ,'0' )";
                en.ExecuteSQL(s);
            }
        }
        #endregion
    }
    [Serializable]
    public class ResultsList
    {
        public ArrayList _results = new ArrayList();
        public int m_parameters = 1;
        public string m_db_field2 = "";
        public string m_value2 = "";
        public string m_db_extraquery = "";
        public string m_where = "";//used if m_parameters =0 as full WHERE clause

        public ResultsList()
        {
            m_parameters = 1;
            m_db_field2 = "";
            m_value2 = "";
            m_db_extraquery = "";// use if param=3 as extra where cluase..
        }

        public void LoadListSimple(string where)// loads only type date and value
        {
            _results.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = " SELECT ";
            s += "ResultValue, ";
            s += "ResultText, ";
            s += "ResultDate, ";
            s += "ResultType, ";
            s += "StudentID, ";
            s += "CourseId  ";
            s += " FROM dbo.tbl_Core_Results ";
            s += where;
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Result c = new Result();
                            _results.Add(c);
                            c.Value = dr.GetString(0);
                            if (!dr.IsDBNull(1)) c.Text = dr.GetString(1);
                            if (!dr.IsDBNull(2)) c.Date = dr.GetDateTime(2);
                            c.Resulttype = dr.GetInt32(3);
                            c.StudentID = dr.GetGuid(4);
                            if (!dr.IsDBNull(5)) c.CourseID = dr.GetGuid(5);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList(string db_field, string Value)
        {
            _results.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //open a db connection
            //string s= "SELECT  qry_cc_development_results.*  FROM  qry_cc_development_results ";

            string s = " SELECT ";
            s += "dbo.tbl_Core_Students.StudentPersonId, dbo.tbl_Core_People.PersonSurname, ";
            s += "dbo.tbl_Core_People.PersonGivenName, ";
            s += "dbo.tbl_Core_Students.StudentId, ";
            s += "dbo.tbl_Core_Results.ResultValue, ";
            s += "dbo.tbl_Core_Results.ResultText, ";
            s += "dbo.tbl_Core_Results.ResultDate, ";
            s += "dbo.tbl_Core_Courses.CourseName, ";
            s += "dbo.tbl_Core_Courses.CourseCode, ";
            s += "dbo.tbl_List_ResultType.ShortName, ";
            s += "dbo.tbl_List_ResultType.[External], ";
            s += "dbo.tbl_List_ResultType.Numeric, ";
            s += "dbo.tbl_List_ResultType.Description, ";
            s += "dbo.tbl_List_ResultType.AllowedValues, ";
            s += "dbo.tbl_List_ResultType.MaxValue, ";
            s += "dbo.tbl_List_ResultType.MinValue, ";
            s += "dbo.tbl_List_CourseTypes.CourseType, ";
            s += "dbo.tbl_List_CourseTypes.KeyStage, ";
            s += "dbo.tbl_Core_Students.StudentAdmissionNumber, ";
            s += "dbo.tbl_Core_Results.ExamsOptionId, ";
            s += "dbo.tbl_Exams_Options.OptionCode, ";
            s += "dbo.tbl_Exams_Options.OptionTitle, ";
            s += "dbo.tbl_Core_Courses.CourseId, ";
            s += "dbo.tbl_Core_Results.ResultType, ";
            s += "dbo.tbl_Exams_Options.OptionQualification, ";
            s += "dbo.tbl_Exams_Options.OptionLevel, ";
            s += "dbo.tbl_Exams_Options.OptionItem, ";
            s += "dbo.tbl_Exams_Options.OptionMaximumMark, ";
            s += "dbo.tbl_Exams_Syllabus.SyllabusCode, ";
            s += "dbo.tbl_Exams_Syllabus.SyllabusTitle, ";
            s += "dbo.tbl_Core_Results.ResultID ";
            s += " FROM	dbo.tbl_Core_Students ";
            s += " INNER JOIN dbo.tbl_Core_Results ";
            s += " INNER JOIN dbo.tbl_List_ResultType ";
            s += " ON dbo.tbl_Core_Results.ResultType = dbo.tbl_List_ResultType.Id  ";
            s += " INNER JOIN   dbo.tbl_List_CourseTypes ";
            s += " INNER JOIN dbo.tbl_Core_Courses ";
            s += " ON dbo.tbl_List_CourseTypes.Id = dbo.tbl_Core_Courses.CourseType ";
            s += " ON dbo.tbl_Core_Results.CourseId = dbo.tbl_Core_Courses.CourseId ";
            s += " ON dbo.tbl_Core_Students.StudentId = dbo.tbl_Core_Results.StudentID ";
            s += " INNER JOIN dbo.tbl_Core_People ";
            s += " ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId ";
            s += " LEFT OUTER JOIN dbo.tbl_Exams_Options ";
            s += " ON dbo.tbl_Core_Results.ExamsOptionId = dbo.tbl_Exams_Options.OptionID ";
            s += " LEFT OUTER JOIN dbo.tbl_Exams_Syllabus ";
            s += " ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID ";

            if (m_parameters == 0) s += m_where;
            else
            {

                s += " WHERE  (" + db_field + " = '" + Value + "')  ";
                if (m_parameters > 1) s += " AND (" + m_db_field2 + " ='" + m_value2 + "') ";
                if (m_parameters == 3) s += m_db_extraquery;
            }
            s += " ORDER BY dbo.tbl_Core_People.PersonSurname ASC  ";
            s += ", dbo.tbl_Core_Results.ResultType ASC ";
            s += ", dbo.tbl_Core_Results.ResultDate  DESC ";
            s += ", dbo.tbl_Core_Courses.CourseName  ASC ";
            s += ", dbo.tbl_Exams_Options.OptionCode  ASC ";
            //s+=", qry_cc_development_results.ExaminedSpecificationCode ASC";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Result c = new Result(dr);
                            if (c.Valid) _results.Add(c);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_OrderByDate(string db_field, string Value)
        {
            _results.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //open a db connection
            //string s= "SELECT  qry_cc_development_results.*  FROM  qry_cc_development_results ";

            string s = " SELECT ";
            s += "dbo.tbl_Core_Students.StudentPersonId, dbo.tbl_Core_People.PersonSurname, ";
            s += "dbo.tbl_Core_People.PersonGivenName, ";
            s += "dbo.tbl_Core_Students.StudentId, ";
            s += "dbo.tbl_Core_Results.ResultValue, ";
            s += "dbo.tbl_Core_Results.ResultText, ";
            s += "dbo.tbl_Core_Results.ResultDate, ";
            s += "dbo.tbl_Core_Courses.CourseName, ";
            s += "dbo.tbl_Core_Courses.CourseCode, ";
            s += "dbo.tbl_List_ResultType.ShortName, ";
            s += "dbo.tbl_List_ResultType.[External], ";
            s += "dbo.tbl_List_ResultType.Numeric, ";
            s += "dbo.tbl_List_ResultType.Description, ";
            s += "dbo.tbl_List_ResultType.AllowedValues, ";
            s += "dbo.tbl_List_ResultType.MaxValue, ";
            s += "dbo.tbl_List_ResultType.MinValue, ";
            s += "dbo.tbl_List_CourseTypes.CourseType, ";
            s += "dbo.tbl_List_CourseTypes.KeyStage, ";
            s += "dbo.tbl_Core_Students.StudentAdmissionNumber, ";
            s += "dbo.tbl_Core_Results.ExamsOptionId, ";
            s += "dbo.tbl_Exams_Options.OptionCode, ";
            s += "dbo.tbl_Exams_Options.OptionTitle, ";
            s += "dbo.tbl_Core_Courses.CourseId, ";
            s += "dbo.tbl_Core_Results.ResultType, ";
            s += "dbo.tbl_Exams_Options.OptionQualification, ";
            s += "dbo.tbl_Exams_Options.OptionLevel, ";
            s += "dbo.tbl_Exams_Options.OptionItem, ";
            s += "dbo.tbl_Exams_Options.OptionMaximumMark, ";
            s += "dbo.tbl_Exams_Syllabus.SyllabusCode, ";
            s += "dbo.tbl_Exams_Syllabus.SyllabusTitle, ";
            s += "dbo.tbl_Core_Results.ResultID ";
            s += " FROM	dbo.tbl_Core_Students ";
            s += " INNER JOIN dbo.tbl_Core_Results ";
            s += " INNER JOIN dbo.tbl_List_ResultType ";
            s += " ON dbo.tbl_Core_Results.ResultType = dbo.tbl_List_ResultType.Id  ";
            s += " INNER JOIN   dbo.tbl_List_CourseTypes ";
            s += " INNER JOIN dbo.tbl_Core_Courses ";
            s += " ON dbo.tbl_List_CourseTypes.Id = dbo.tbl_Core_Courses.CourseType ";
            s += " ON dbo.tbl_Core_Results.CourseId = dbo.tbl_Core_Courses.CourseId ";
            s += " ON dbo.tbl_Core_Students.StudentId = dbo.tbl_Core_Results.StudentID ";
            s += " INNER JOIN dbo.tbl_Core_People ";
            s += " ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId ";
            s += " LEFT OUTER JOIN dbo.tbl_Exams_Options ";
            s += " ON dbo.tbl_Core_Results.ExamsOptionId = dbo.tbl_Exams_Options.OptionID ";
            s += " LEFT OUTER JOIN dbo.tbl_Exams_Syllabus ";
            s += " ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID ";

            s += " WHERE  (" + db_field + " = '" + Value + "')  ";
            if (m_parameters > 1) s += " AND (" + m_db_field2 + " ='" + m_value2 + "') ";
            if (m_parameters == 3) s += m_db_extraquery;

            s += " ORDER BY dbo.tbl_Core_People.PersonSurname ASC  ";
            s += ", dbo.tbl_Core_Results.ResultDate  DESC ";
            s += ", dbo.tbl_Core_Results.ResultType ASC ";
            s += ", dbo.tbl_Core_Courses.CourseName  ASC ";
            s += ", dbo.tbl_Exams_Options.OptionCode  ASC ";
            //s+=", qry_cc_development_results.ExaminedSpecificationCode ASC";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Result c = new Result(dr);
                            if (c.Valid) _results.Add(c);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }


        public double AverageResult(string db_field, string Value, ref int Number_results)
        {
            _results.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            //open a db connection
            //string s= "SELECT  qry_cc_development_results.*  FROM  qry_cc_development_results ";
            double value = 0; int number = 0; Number_results = 0;
            string s1 = "";
            string s2 = "";
            string s = " SELECT SUM(CAST ( dbo.tbl_Core_Results.ResultValue AS INT)) FROM dbo.tbl_Core_Results ";
            if (m_parameters == 0) s1 += m_where;
            else
            {
                s1 += " WHERE  (" + db_field + " = '" + Value + "')  ";
                if (m_parameters > 1) s1 += " AND (" + m_db_field2 + " ='" + m_value2 + "') ";
                if (m_parameters == 3) s1 += m_db_extraquery;
            }
            try
            {
                using (SqlConnection cn = new SqlConnection(db_connection))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(s + s1, cn))
                    {
                        s2 = cm.ExecuteScalar().ToString();
                        value = System.Convert.ToDouble(s2);
                    }
                    cn.Close();
                }
            }
            catch
            {
                s = " SELECT SUM(CAST ( dbo.tbl_Core_Results.ResultValue AS decimal)) FROM dbo.tbl_Core_Results ";
                if (m_parameters == 0) s1 += m_where;
                else
                {
                    s1 = " WHERE  (" + db_field + " = '" + Value + "')  ";
                    if (m_parameters > 1) s1 += " AND (" + m_db_field2 + " ='" + m_value2 + "') ";
                    if (m_parameters == 3) s1 += m_db_extraquery;
                }
                using (SqlConnection cn = new SqlConnection(db_connection))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(s + s1, cn))
                    {
                        try
                        {
                            s2 = cm.ExecuteScalar().ToString();
                            value = System.Convert.ToDouble(s2);
                        }
                        catch { value = 0; }
                    }
                    cn.Close();
                }

            }




            s = " SELECT COUNT(dbo.tbl_Core_Results.ResultValue) FROM dbo.tbl_Core_Results ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s + s1, cn))
                {
                    s2 = cm.ExecuteScalar().ToString();
                    number = System.Convert.ToInt16(s2);
                    Number_results = number;
                }
                cn.Close();
            }
            if (number > 0) value = value / number; else value = 0;
            return value;
        }

        public int YearPosition(string db_field, string Value, string Score)
        {
            _results.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            int number = 0;
            string s1 = "";
            string s = " SELECT COUNT(dbo.tbl_Core_Results.ResultValue) FROM dbo.tbl_Core_Results ";
            if (m_parameters == 0) s1 += m_where;
            else
            {
                s1 += " WHERE  (" + db_field + " = '" + Value + "')  ";
                s1 += " AND (CAST ( dbo.tbl_Core_Results.ResultValue AS INT)>'" + Score.ToString() + "') ";
                if (m_parameters > 1) s1 += " AND (" + m_db_field2 + " ='" + m_value2 + "') ";
                if (m_parameters == 3) s1 += m_db_extraquery;
            }
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s + s1, cn))
                {
                    try
                    {
                        string s2 = cm.ExecuteScalar().ToString();
                        number = System.Convert.ToInt16(s2);
                    }
                    catch { number = 0; }
                }
                cn.Close();
            }
            return number;

        }

    }
    public class ResultType
    {
        public int m_ResultTypeID;
        public string m_ShortName;
        public bool m_External;
        public bool m_Numeric;
        public string m_Description;
        public string m_AllowedValues;
        public int m_MinValue;
        public int m_MaxValue;
        public bool m_valid = false;
    }
    public class ResultTypeList
    {
        public ArrayList _resulttypelist = new ArrayList();
        public ResultTypeList()
        {
            _resulttypelist.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  Id, ShortName, [External], [Numeric], Description, AllowedValues, MaxValue, MinValue FROM  tbl_List_ResultType ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ResultType r = new ResultType();
                            _resulttypelist.Add(r);
                            r.m_ResultTypeID = dr.GetInt32(0);
                            r.m_ShortName = dr.GetString(1);
                            r.m_External = dr.GetBoolean(2);
                            r.m_Numeric = dr.GetBoolean(3);
                            r.m_Description = dr.GetString(4);
                            r.m_AllowedValues = dr.GetString(5);
                            if (!dr.IsDBNull(6)) r.m_MaxValue = dr.GetInt32(6);
                            if (!dr.IsDBNull(7)) r.m_MinValue = dr.GetInt32(7);
                            r.m_valid = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class RoomList
    {
        public ArrayList m_roomlist = new ArrayList();
        public RoomList() { m_roomlist.Clear(); }
        public void LoadList()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT RoomId, RoomCode FROM tbl_Core_Rooms WHERE RoomIsTeaching ='1' ORDER BY RoomCode ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimpleRoom r = new SimpleRoom();
                            m_roomlist.Add(r);
                            r.m_roomcode = dr.GetString(1);
                            r.m_RoomID = dr.GetGuid(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList(Guid OrganisationalUnitId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT RoomId, RoomCode FROM tbl_Core_Rooms ";
            s += " WHERE (RoomIsTeaching ='1' ) AND (OrganisationalUnitId = '" + OrganisationalUnitId.ToString() + "')  ";
            s += " ORDER BY RoomCode ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimpleRoom r = new SimpleRoom();
                            m_roomlist.Add(r);
                            r.m_roomcode = dr.GetString(1);
                            r.m_RoomID = dr.GetGuid(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

    }


    public class Sanction
    {
        public int Id;
        public string Name;
        public string Description;
        public int EscalationTariff;
        public int EscalationId;
        public int SanctionProcessId;

        public Sanction()
        {
        }

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            Name = dr.GetString(1);
            if (!dr.IsDBNull(2)) Description = dr.GetString(2);
            if (!dr.IsDBNull(3)) EscalationTariff = dr.GetInt32(3);
            if (!dr.IsDBNull(4)) EscalationId = dr.GetInt32(4);
            if (!dr.IsDBNull(5)) SanctionProcessId = dr.GetInt32(5);
        }

        public void Load(int Id)
        {
            string s = "SELECT * FROM dbo.tbl_List_Sanctions WHERE (Id=" + Id + ") ";
            Encode en = new Encode();
            string db = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read()) Hydrate(dr);
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class SanctionList
    {
        public ArrayList _list;
        public int count;

        public SanctionList()
        {
            _list.Clear(); count = 0;
            string s = "SELECT * FROM dbo.tbl_List_Sanctions ";
            Encode en = new Encode();
            string db = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Sanction s1 = new Sanction();
                            s1.Hydrate(dr);
                            _list.Add(s1);
                            count++;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class ScheduledPeriod
    {
        public string m_periodcode;
        public int m_daycode;
        public string m_staffcode;
        public string m_roomcode;
        public string m_groupcode;
        public Guid m_groupId;
        public Guid m_RoomId;
        public string m_dayname;
        public Guid m_Id = Guid.Empty;
        public DateTime m_PeriodStart;
        public DateTime m_PeriodEnd;
        public DateTime m_ValidityStart;
        public DateTime m_ValidityEnd;
        public bool m_valid = false;


        public void Hydrate(SqlDataReader dr)
        {
            m_daycode = (int)dr.GetByte(0);
            m_dayname = dr.GetString(2);
            m_periodcode = dr.GetString(3);
            m_staffcode = dr.GetString(5);
            m_roomcode = dr.GetString(6);
            m_PeriodStart = dr.GetDateTime(7);
            m_groupcode = dr.GetString(8);
            m_Id = dr.GetGuid(9);
            m_groupId = dr.GetGuid(10);
            m_PeriodEnd = dr.GetDateTime(11);
            m_ValidityStart = dr.GetDateTime(12);
            m_ValidityEnd = dr.GetDateTime(13);
            m_RoomId = dr.GetGuid(14);
            m_valid = true;
        }

        public void Load(Guid Id)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_Core_ScheduledPeriods.DayNo, dbo.tbl_Core_ScheduledPeriods.NonScheduled, ";
            s += " dbo.tbl_List_Days.[Day], dbo.tbl_Core_Periods.PeriodCode, dbo.tbl_Core_Periods.PeriodName, dbo.tbl_Core_Staff.StaffCode, ";
            s += " dbo.tbl_Core_Rooms.RoomCode, dbo.tbl_Core_Periods.PeriodStart, dbo.tbl_Core_Groups.GroupCode ";
            s += ", dbo.tbl_Core_ScheduledPeriods.ScheduledPeriodId   ,dbo.tbl_Core_Groups.GroupId , dbo.tbl_Core_Periods.PeriodEnd ";
            s += ", dbo.tbl_Core_ScheduledPeriodValidity.ValidityStart, dbo.tbl_Core_ScheduledPeriodValidity.ValidityEnd ";
            s += ", dbo.tbl_Core_ScheduledPeriods.RoomId";
            s += " FROM         dbo.tbl_Core_Groups INNER JOIN ";
            s += " dbo.tbl_Core_ScheduledPeriods ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_ScheduledPeriods.GroupId INNER JOIN ";
            s += " dbo.tbl_Core_Periods ON dbo.tbl_Core_ScheduledPeriods.PeriodId = dbo.tbl_Core_Periods.PeriodId INNER JOIN ";
            s += " dbo.tbl_List_Days ON dbo.tbl_Core_ScheduledPeriods.DayNo = dbo.tbl_List_Days.Id INNER JOIN ";
            s += "  dbo.tbl_Core_Staff ON dbo.tbl_Core_ScheduledPeriods.StaffId = dbo.tbl_Core_Staff.StaffId INNER JOIN ";
            s += " dbo.tbl_Core_Rooms ON dbo.tbl_Core_ScheduledPeriods.RoomId = dbo.tbl_Core_Rooms.RoomId ";
            s += " INNER JOIN ";
            s += "tbl_Core_ScheduledPeriodValidity ON tbl_Core_ScheduledPeriods.ScheduledPeriodId = tbl_Core_ScheduledPeriodValidity.ScheduledPeriodId ";
            s += "WHERE (dbo.tbl_Core_ScheduledPeriods.ScheduledPeriodId  = '" + Id.ToString() + "')";

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
                            m_valid = true;
                        }
                    }
                }
            }
        }

        public bool UpdateRoom()
        {
            //just updates the room...
            if ((m_valid == false) || (m_Id == Guid.Empty)) return false;
            Encode en = new Encode(); string s = "";
            s = "UPDATE tbl_Core_ScheduledPeriods ";
            s += " SET RoomId = '" + m_RoomId.ToString() + "' ";
            s += " WHERE (ScheduledPeriodId = '" + m_Id.ToString() + "' )";
            en.ExecuteSQL(s);
            return true;
        }

        public int ConvertDateTimeDaytoCode(DateTime d)
        {
            int day = -1;
            switch (d.DayOfWeek)
            {
                case DayOfWeek.Monday: day = 0; break;
                case DayOfWeek.Tuesday: day = 1; break;
                case DayOfWeek.Wednesday: day = 2; break;
                case DayOfWeek.Thursday: day = 3; break;
                case DayOfWeek.Friday: day = 4; break;
                case DayOfWeek.Saturday: day = 5; break;
                case DayOfWeek.Sunday: day = 6; break;
                default:; day = -1; break;
            }
            return day;
        }


        public DayOfWeek ConvertDayToDayofWeek()
        {
            DayOfWeek dw = new DayOfWeek();
            switch (m_daycode)
            {
                case 0: dw = DayOfWeek.Monday; break;
                case 1: dw = DayOfWeek.Tuesday; break;
                case 2: dw = DayOfWeek.Wednesday; break;
                case 3: dw = DayOfWeek.Thursday; break;
                case 4: dw = DayOfWeek.Friday; break;
                case 5: dw = DayOfWeek.Saturday; break;
                case 6: dw = DayOfWeek.Sunday; break;
                default: dw = DayOfWeek.Monday; break;
            }
            return dw;
        }

        public DateTime GetStartDate(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, m_PeriodStart.Hour, m_PeriodStart.Minute, m_PeriodStart.Second);
        }
        public DateTime GetEndDate(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, m_PeriodEnd.Hour, m_PeriodEnd.Minute, m_PeriodEnd.Second);
        }

    }
    public class ScheduledPeriodRaw
    {
        public Guid Id;
        public Guid PeriodId;
        public Guid GroupId;
        public Guid StaffId;
        public Guid RoomId;
        public int DayNo;
        public bool NonScheduled;
        public Guid ScehduledPeriodValidityId;
        public DateTime ValidityStart;
        public DateTime ValidityEnd;
        public bool found_in_checking;

        public ScheduledPeriodRaw()
        {
            ScehduledPeriodValidityId = Guid.Empty;
            Id = Guid.Empty;
            found_in_checking = false;
        }
        public Guid Save()
        {
            if (Id == Guid.Empty)
            {
                Id = Find_Sched(GroupId, StaffId, RoomId, PeriodId, DayNo);
            }
            //firstly does the scheduled period exist....
            Guid Id_0 = Find_SchedX(GroupId, StaffId, RoomId, PeriodId, DayNo);
            if (Id_0 == Id)
            {
                //the record exists and hasn't been altered....  so only need to worry about validity
            }
            else
            {
                //need to close down the old scheduled period...  and all validity periods
                Delete();
                //recreate it....
                Id = Find_Sched(GroupId, StaffId, RoomId, PeriodId, DayNo);
            }
            string s = "";
            Encode en = new Encode();
            //do we have a validity... if so update it....
            ScehduledPeriodValidityId = Find_Validity(Id, ValidityStart, ValidityEnd);
            if (ScehduledPeriodValidityId != Guid.Empty)
            {
                s = "UPDATE tbl_Core_ScheduledPeriodValidity ";
                s += " SET ValidityStart = CONVERT(DATETIME, '" + ValidityStart.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += ", ValidityEnd = CONVERT(DATETIME, '" + ValidityEnd.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += " WHERE (ScheduledPeriodValidityId = '" + ScehduledPeriodValidityId.ToString() + "' )";
                en.ExecuteSQL(s);
            }
            //create
            else
            {
                s = "INSERT INTO tbl_Core_ScheduledPeriodValidity ";
                s += " (ScheduledPeriodId, ValidityStart, ValidityEnd ,Version)";
                s += " VALUES ( '" + Id.ToString() + "', CONVERT(DATETIME, '" + ValidityStart.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), ";
                s += " CONVERT(DATETIME, '" + ValidityEnd.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '0' )";
                en.ExecuteSQL(s);
                //now need the new id.....
                ScehduledPeriodValidityId = Find_Validity(Id, ValidityStart, ValidityEnd);
            }
            return Id;
        }
        private Guid Find_Validity(Guid ScheduledPeriod_Id, DateTime ValidityStart, DateTime ValidityEnd)
        {
            Encode en = new Encode();
            string s = "SELECT ScheduledPeriodValidityId FROM tbl_Core_ScheduledPeriodValidity  ";
            s += " WHERE (ScheduledPeriodId='" + ScheduledPeriod_Id.ToString() + "' ) ";
            s += " AND (ValidityStart = CONVERT(DATETIME, '" + ValidityStart.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += " AND (ValidityEnd = CONVERT(DATETIME, '" + ValidityEnd.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            return en.FindSQL(s);
        }
        private Guid Find_Sched(Guid group_id, Guid staff_id, Guid room_id, Guid period_id, int day)
        {
            Guid id = Find_SchedX(group_id, staff_id, room_id, period_id, day);
            if (id == Guid.Empty) id = Make_Sched(group_id, staff_id, room_id, period_id, day);
            return id;
        }
        private Guid Find_SchedX(Guid group_id, Guid staff_id, Guid room_id, Guid period_id, int day)
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
            s += "'" + period_id.ToString() + "', '" + day.ToString() + "' , '0' , '0' )";
            en.ExecuteSQL(s);
            return Find_SchedX(group_id, staff_id, room_id, period_id, day);
        }
        public Guid FindId()
        {
            return Find_SchedX(GroupId, StaffId, RoomId, PeriodId, DayNo);
        }
        public void Delete()
        {
            if (Id == null) return;
            //need to delete all the validity records that correspond to this Scheduled period....
            string s = "DELETE FROM dbo.tbl_Core_ScheduledPeriodValidity ";
            s += " WHERE (dbo.tbl_Core_ScheduledPeriodValidity.ScheduledPeriodId  = '" + Id.ToString() + "') ";
            Encode en = new Encode();
            en.ExecuteSQL(s);
            // so now no validity records....

            s = "DELETE FROM dbo.tbl_Core_ScheduledPeriods ";
            s += " WHERE (dbo.tbl_Core_ScheduledPeriods.ScheduledPeriodId = '" + Id.ToString() + "') ";
            en.ExecuteSQL(s);
            Id = Guid.Empty;
            ScehduledPeriodValidityId = Guid.Empty;
        }
    }
    public class ScheduledPeriodRawList
    {
        public ArrayList m_list = new ArrayList();
        public int Load_Staff(Guid StaffId, DateTime Date, int Day, Guid PeriodId)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity WHERE (PeriodId = '" + PeriodId.ToString() + "' ) AND ";
            s += " (StaffId='" + StaffId.ToString() + "')  AND (DayNo = '" + Day.ToString() + "') ";
            s += " AND ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load_Staff(Guid StaffId, DateTime Date, int Day)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity ";
            s += "WHERE (StaffId='" + StaffId.ToString() + "')  AND (DayNo = '" + Day.ToString() + "') ";
            s += " AND ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load_Staff(Guid StaffId, DateTime Date)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity WHERE  ";
            s += " (StaffId='" + StaffId.ToString() + "')   ";
            s += " AND ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load_Room(Guid RoomId, DateTime Date, int Day, Guid PeriodId)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity WHERE (PeriodId = '" + PeriodId.ToString() + "' ) AND ";
            s += " (RoomId='" + RoomId.ToString() + "')  AND (DayNo = '" + Day.ToString() + "') ";
            s += " AND ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load(Guid GroupId, DateTime Date)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity ";
            s += "WHERE  (GroupId='" + GroupId.ToString() + "') ";
            s += " AND ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load_for_Group(Guid GroupId)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity ";
            s += "WHERE  (GroupId='" + GroupId.ToString() + "') ";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load(DateTime Date)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity ";
            s += "WHERE ";
            s += " ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load(DateTime Date, int Day)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity ";
            s += "WHERE (DayNo = '" + Day.ToString() + "') ";
            s += " AND ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load(DateTime Date, Period period, int Day)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity ";
            s += "WHERE (PeriodId = '" + period.m_PeriodId.ToString() + "') ";
            s += " AND (DayNo = '" + Day.ToString() + "') ";
            s += " AND ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load_Group(Guid GroupId, DateTime Date, int Day, Guid PeriodId)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity ";
            s += " WHERE (PeriodId = '" + PeriodId.ToString() + "' ) AND ";
            s += " (GroupId='" + GroupId.ToString() + "')   AND (DayNo = '" + Day.ToString() + "') ";
            s += " AND ( ValidityStart<=CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
        public int Load_ScheduledPeriod(Guid ScheduledPeriodId, DateTime Date)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  qry_Cerval_Core_ScheduledPeriodValidity ";
            s += "WHERE  (ScheduledPeriodId='" + ScheduledPeriodId.ToString() + "') ";
            s += " AND ( ValidityStart<CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "AND (ValidityEnd>CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            s += "  ORDER BY ValidityStart ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodRaw p = new ScheduledPeriodRaw();
                            m_list.Add(p);
                            p.Id = dr.GetGuid(0);
                            p.PeriodId = dr.GetGuid(1);
                            p.GroupId = dr.GetGuid(2);
                            p.StaffId = dr.GetGuid(3);
                            p.RoomId = dr.GetGuid(4);
                            p.DayNo = dr.GetByte(5);
                            p.NonScheduled = dr.GetBoolean(6);
                            p.ScehduledPeriodValidityId = dr.GetGuid(8);
                            p.ValidityStart = dr.GetDateTime(9); ;
                            p.ValidityEnd = dr.GetDateTime(10);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;

        }
    }


    public class ScheduledPeriodPlan  // used for TT plan when we have no validity records
    {
        public Guid m_Id = Guid.Empty;
        public Guid m_PeriodId;
        public Guid m_GroupId;
        public Guid m_StaffId;
        public Guid m_RoomId;
        public int m_Daycode;
        public bool m_Non_Scheduled;
        public Guid m_TTPlanLinearGroupId;
        public int m_Version;
        public string m_Groupcode;

        public bool m_Valid;


        public ScheduledPeriodPlan() { m_Id = Guid.Empty; m_Valid = false; }

        public void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetGuid(0);
            m_TTPlanLinearGroupId = dr.GetGuid(1);//cant be null
            if (dr.IsDBNull(2)) { m_PeriodId = Guid.Empty; } else { m_PeriodId = dr.GetGuid(2); }
            if (dr.IsDBNull(3)) { m_GroupId = Guid.Empty; } else { m_GroupId = dr.GetGuid(3); }
            if (dr.IsDBNull(4)) { m_StaffId = Guid.Empty; } else { m_StaffId = dr.GetGuid(4); }
            if (dr.IsDBNull(5)) { m_RoomId = Guid.Empty; } else { m_RoomId = dr.GetGuid(5); }
            if (dr.IsDBNull(6)) { m_Daycode = -1; } else { m_Daycode = (int)dr.GetByte(6); }
            if (dr.IsDBNull(7)) { m_Non_Scheduled = true; } else { m_Non_Scheduled = dr.GetBoolean(7); }

            m_Version = dr.GetInt32(8);
            m_Groupcode = dr.GetString(9);
            m_Valid = true;
        }

        public void Load(Guid Id)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT dbo.tbl_TTPlan_ScheduledPeriods.*, ";
            s += " dbo.tbl_Core_Groups.GroupCode  ";
            s += " FROM  dbo.tbl_TTPlan_ScheduledPeriods INNER JOIN  ";
            s += " dbo.tbl_Core_Groups ON dbo.tbl_TTPlan_ScheduledPeriods.GroupId = dbo.tbl_Core_Groups.GroupId ";
            s += " WHERE (dbo.tbl_TTPlan_ScheduledPeriods.ScheduledPeriodId = '" + Id.ToString() + "') ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydrate(dr); m_Valid = true;
                        }
                    }
                }
            }
        }

        public void Delete()
        {
            if (m_Id == null) return;
            if (m_Id == Guid.Empty) return;
            string s = "DELETE FROM dbo.tbl_TTPlan_ScheduledPeriods ";
            s += " WHERE (dbo.tbl_TTPlan_ScheduledPeriods.ScheduledPeriodId = '" + m_Id.ToString() + "') ";
            Encode en = new Encode();
            en.ExecuteSQL(s);
            m_Id = Guid.Empty;
        }

    }

    public class ScheduledPeriodPlanList
    {
        public ArrayList m_list = new ArrayList();
        public int Load_LinearGroup(Guid LinearGroupId)
        {
            m_list.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_TTPlan_ScheduledPeriods.*, ";
            s += " dbo.tbl_Core_Groups.GroupCode  ";
            s += " FROM  dbo.tbl_TTPlan_ScheduledPeriods INNER JOIN  ";
            s += " dbo.tbl_Core_Groups ON dbo.tbl_TTPlan_ScheduledPeriods.GroupId = dbo.tbl_Core_Groups.GroupId ";
            s += " WHERE (dbo.tbl_TTPlan_ScheduledPeriods.TTPlanLinearGroupId = '" + LinearGroupId.ToString() + "') ";
            s += " ORDER BY dbo.tbl_Core_Groups.GroupCode , dbo.tbl_TTPlan_ScheduledPeriods.StaffId ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledPeriodPlan p = new ScheduledPeriodPlan();
                            p.Hydrate(dr);
                            m_list.Add(p);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list.Count;
        }
    }


    public class ScheduledValidity
    {
        public Guid Id;
        public Guid ScheduledPeriodId;
        public DateTime Start;
        public DateTime End;

        public ScheduledValidity()
        {
            Id = Guid.Empty;
        }

        public void Load(Guid ID)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM dbo.tbl_Core_ScheduledPeriodValidity WHERE ( ScheduledPeriodValidityId='" + ID.ToString() + "' ) ";
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
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            ScheduledPeriodId = dr.GetGuid(1);
            Start = dr.GetDateTime(2);
            if (!dr.IsDBNull(3)) End = dr.GetDateTime(2); else End = new DateTime(3000, 1, 1);
        }
        public Guid Save()
        {
            string s = "";
            Encode en = new Encode();
            //do we have a validity... if so update it....
            if (Id != Guid.Empty)
            {
                s = "UPDATE tbl_Core_ScheduledPeriodValidity ";
                s += " SET ValidityStart = CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += ", ValidityEnd = CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += " WHERE (ScheduledPeriodValidityId = '" + Id.ToString() + "' )";
                en.ExecuteSQL(s);
            }
            //create
            else
            {
                Id = Guid.NewGuid();
                s = "INSERT INTO tbl_Core_ScheduledPeriodValidity ";
                s += " (ScheduledPeriodValidityId, ScheduledPeriodId, ValidityStart, ValidityEnd ,Version)";
                s += " VALUES ( '" + Id.ToString() + "', '" + ScheduledPeriodId.ToString() + "', CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), ";
                s += " CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '5' )";
                en.ExecuteSQL(s);
            }
            return Id;

        }

    }
    public class ScheduledValidityList
    {
        public ArrayList m_list = new ArrayList();
        public ScheduledValidityList()
        {
            m_list.Clear();
        }
        public void Load(Guid ScheduledPeriodID)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM dbo.tbl_Core_ScheduledPeriodValidity WHERE ( ScheduledPeriodId='" + ScheduledPeriodID.ToString() + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledValidity sv1 = new ScheduledValidity();
                            sv1.Hydrate(dr);
                            m_list.Add(sv1);
                        }
                    }
                }
            }
        }
    }
    public class SENType
    {
        public int id;
        public string SENtype;
        public string Description;
        public bool _valid = false;
        public SENType() { }

        public override string ToString()
        {
            return SENtype;
        }
    }
    public class SENTypeList
    {
        public ArrayList _List = new ArrayList();
        public SENTypeList()
        {
            _List.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  * FROM dbo.tbl_List_SENTypes ";
            string ds = "CONVERT(DATETIME, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";
            s += "WHERE ((ValidityEnd>" + ds + ") AND (ValidityStart<" + ds + ")) OR ((ValidityStart<" + ds + ") AND (validityEnd IS NULL) )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SENType s1 = new SENType();
                            _List.Add(s1);
                            s1._valid = true;
                            if (!dr.IsDBNull(0)) s1.id = dr.GetInt32(0);
                            s1.SENtype = dr.GetString(1);
                            s1.Description = dr.GetString(2);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class SENStatus
    {
        public int Id;
        public string Status;
        public string SENStatusDescription;
        public string SENStatusCode;
        public bool valid;
        public SENStatus() { }
        public void Hydrate(SqlDataReader dr)
        {
            valid = true;
            if (!dr.IsDBNull(0)) Id = dr.GetInt32(0);
            Status = dr.GetString(1);
            if (!dr.IsDBNull(2)) SENStatusDescription = dr.GetString(2);
            SENStatusCode = dr.GetString(3);
        }
    }
    public class SENStatusList
    {
        public ArrayList _List = new ArrayList();

        public SENStatusList()
        {
            _List.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string ds = "CONVERT(DATETIME, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";
            string s = "SELECT  * FROM dbo.tbl_List_SENStatusTypes ";
            s += "WHERE ((ValidityEnd>" + ds + ") AND (ValidityStart<" + ds + ")) OR ((ValidityStart<" + ds + ") AND (validityEnd IS NULL) )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SENStatus s1 = new SENStatus();
                            _List.Add(s1); s1.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    [Serializable]
    public class SimplePupil
    {
        public Guid m_StudentId;
        public Guid m_PersonId;
        public string m_Surname;
        public string m_GivenName;
        public string m_MiddleName;
        public string m_InformalName;
        public int m_adno;
        public int m_year;
        public string m_form;
        public DateTime m_dob;
        public DateTime m_dol;
        public DateTime m_doa;
        public string m_upn;
        public int m_exam_no;
        public bool m_IsOnRole = false;
        public bool m_valid = false;
        public string m_ExtraData;
        public string m_GoogleAppsLogin;
        public bool m_InReceiptPupilPremium;
        public string m_IsamsPupilId; // iSAMS calls this schoolId

        public SimplePupil()
        {
        }

        public override string ToString()
        {
            return m_GivenName + " " + m_Surname + ":" + m_adno.ToString();
        }

        public void CreateNew(int Title, string Gender, string Surname, string GivenName, int Adno, DateTime Dob, string iSAMSId, string GoogleLogin)
        {
            Person p = new Person();
            p.m_Gender = Gender;
            p.m_GivenName = GivenName;
            p.m_Surname = Surname;
            p.m_Title = Title;
            p.m_dob = Dob;
            m_PersonId = p.Save();

            Encode en = new Encode();

            if (m_PersonId != Guid.Empty)
            {
                string s = "INSERT INTO dbo.tbl_Core_Students ";
                s += " (StudentPersonId, StudentAdmissionNumber, Version  )";
                s += " VALUES ( '" + m_PersonId.ToString() + "' , '" + Adno.ToString() + "', '200' )";
                en.ExecuteSQL(s);

                Utility u = new Utility();
                Guid st1 = u.GetStudentId(m_PersonId);


                s = "UPDATE dbo.tbl_Core_Students SET iSAMStxtSchoolID ='" + iSAMSId + "' WHERE (StudentId = '" + st1.ToString() + "')";
                en.ExecuteSQL(s);

                s = "UPDATE dbo.tbl_Core_Students SET  GoogleAppsLogin ='" + GoogleLogin + "' WHERE (StudentId = '" + st1.ToString() + "')";
                en.ExecuteSQL(s);
            }
            else
            {

            }

        }

        public void UpdateUCI(string uci)
        {
            if (!Check_UCI_Checksum(uci)) return;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET StudentUCI='" + uci + "' WHERE (StudentId = '" + m_StudentId.ToString() + "')";
            en.ExecuteSQL(s);
        }

        public void UpdateAdno(string adno)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET StudentAdmissionNumber='" + adno + "' WHERE (StudentId = '" + m_StudentId.ToString() + "')";
            en.ExecuteSQL(s);
        }


        public void UpdateUPN(string upn)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET StudentUPN='" + upn + "' WHERE (StudentId = '" + m_StudentId.ToString() + "')";
            en.ExecuteSQL(s);
        }

        public void UpdateULN(string uln)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET StudentULN='" + uln + "' WHERE (StudentId = '" + m_StudentId.ToString() + "')";
            en.ExecuteSQL(s);
        }

        public void UpdateIsamsId(string iSAMSId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET iSAMStxtSchoolID ='" + iSAMSId + "' WHERE (StudentId = '" + m_StudentId.ToString() + "')";
            en.ExecuteSQL(s);
        }

        public void UpdateGoogleLogon(string email)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET  GoogleAppsLogin ='" + email + "' WHERE (StudentId = '" + m_StudentId.ToString() + "')";
            en.ExecuteSQL(s);
        }


        public void MarkAsOnRole(DateTime AdmissionDate)
        {
            string date_s = "CONVERT(DATETIME, '" + AdmissionDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET  StudentIsOnRole ='true' ,";
            s += " StudentLeavingDate = NULL , StudentAdmissionDate = "+date_s;
            s += " WHERE (StudentId = '" + m_StudentId.ToString() + "')";
            en.ExecuteSQL(s);

        }


        public void MarkAsLeft(DateTime leavingDate)
        {
            string date_s = "CONVERT(DATETIME, '" + leavingDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "UPDATE dbo.tbl_Core_Students SET  StudentIsOnRole ='false' ,";
            s += " StudentLeavingDate = " + date_s;
            s += "   WHERE (StudentId = '" + m_StudentId.ToString() + "')";
            en.ExecuteSQL(s);

        }

        public bool Check_UCI_Checksum(string uci)
        {
            if (uci == null) return false;
            string s = uci.Substring(0, 12);
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
            if (s == uci) return true; else return false;
        }

        public bool Load(int StudentAdno)//overload...
        {
            return Load1("StudentAdmissionNumber ='" + StudentAdno.ToString() + "'");
        }

        public bool Load(string StudentId)//overload...
        {
            return ((Load1("StudentId ='" + StudentId + "'")) ? true : Load_Left(StudentId));
        }

        public bool Load(Guid StudentId)//overload...
        {
            return ((Load1("StudentId ='" + StudentId.ToString() + "'")) ? true : Load_Left(StudentId.ToString()));
        }


        public bool Load_StudentIdOnly(int StudentAdno)//overload...
        {
            Encode en = new Encode();
            bool found = false;
            string db_connection = en.GetDbConnection();
            string s = "SELECT  StudentId FROM  dbo.tbl_Core_Students WHERE (StudentAdmissionNumber='" + StudentAdno + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            m_StudentId = dr.GetGuid(0);
                            m_adno = StudentAdno;
                            m_valid = false;
                        }
                        dr.Close();
                    }
                }
            }
            return found;
        }
        public bool Load_Left(string StudentId)
        {
            bool found = false;
            try
            {
                PupilDetails pupil1 = new PupilDetails(StudentId);
                m_StudentId = pupil1.m_StudentId;
                m_adno = pupil1.m_adno; m_doa = pupil1.m_doa; m_dob = pupil1.m_dob;
                m_exam_no = pupil1.m_examNo; m_form = "left"; m_GivenName = pupil1.m_GivenName;
                m_MiddleName = pupil1.m_MiddleName; m_PersonId = pupil1.m_PersonId;
                m_Surname = pupil1.m_Surname; m_upn = pupil1.m_upn; m_valid = true; m_year = 14;
                m_IsOnRole = pupil1.m_IsOnRole; m_GoogleAppsLogin = pupil1.m_GoogleAppsLogin;
                found = true;
            }
            catch
            {

            }
            return found;
        }

        public bool Load_Left(int Adno)
        {
            bool found = false;
            try
            {
                PupilDetails pupil1 = new PupilDetails();
                pupil1.Load_adno(Adno.ToString());
                m_StudentId = pupil1.m_StudentId;
                m_adno = pupil1.m_adno; m_doa = pupil1.m_doa; m_dob = pupil1.m_dob;
                m_exam_no = pupil1.m_examNo; m_form = "left"; m_GivenName = pupil1.m_GivenName;
                m_MiddleName = pupil1.m_MiddleName; m_PersonId = pupil1.m_PersonId;
                m_Surname = pupil1.m_Surname; m_upn = pupil1.m_upn; m_valid = true; m_year = 14;
                m_IsOnRole = pupil1.m_IsOnRole; m_GoogleAppsLogin = pupil1.m_GoogleAppsLogin;
                found = true;
            }
            catch
            {

            }
            return found;
        }

        public bool Load1(string where)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "";

#if DEBUG
            //s = "SELECT  * FROM  INTRANET_Student_Details_Testing ";
            //s = "SELECT  * FROM  qry_User_Student_Details";
            s = "SELECT  * FROM  INTRANET_Student_Details ";
#else
            //s = "SELECT  * FROM  qry_User_Student_Details_VBR_Test ";
            s = "SELECT  * FROM  INTRANET_Student_Details ";
#endif

            if (where != "") s += " WHERE (" + where + ") ";
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
            }
            return m_valid;
        }
        public bool Hydrate(SqlDataReader dr)// used with the query INTRANET_Student_Details_Testing 
        {
            m_valid = true;
            m_StudentId = dr.GetGuid(0);
            m_adno = dr.GetInt32(1);
            if (!dr.IsDBNull(2)) m_upn = dr.GetString(2);
            if (!dr.IsDBNull(3)) m_exam_no = dr.GetInt32(3);
            if (!dr.IsDBNull(4)) m_doa = dr.GetDateTime(4);
            m_IsOnRole = dr.GetBoolean(5);
            m_GivenName = dr.GetString(7);
            m_Surname = dr.GetString(8);
            if (!dr.IsDBNull(9)) m_MiddleName = dr.GetString(9);
            if (!dr.IsDBNull(10)) m_dob = dr.GetDateTime(10);
            m_PersonId = dr.GetGuid(11);
            m_form = dr.GetString(14);//this will be group name....  8T-Rg  or 12-1Rg  dbo.tbl_Core_Groups.GroupCode, 
            if (!dr.IsDBNull(15)) m_InformalName = dr.GetString(15);
            //16  dbo.tbl_Core_Students.StudentFreeSchoolMeals, 
            if (!dr.IsDBNull(17)) m_GoogleAppsLogin = dr.GetString(17);
            if (!dr.IsDBNull(18)) m_InReceiptPupilPremium = dr.GetBoolean(18);
            m_year = 14;//default;    
            if (m_form.StartsWith("7")) m_year = 7;
            if (m_form.StartsWith("8")) m_year = 8;
            if (m_form.StartsWith("9")) m_year = 9;
            if (m_form.StartsWith("10")) m_year = 10;
            if (m_form.StartsWith("11")) m_year = 11;
            if (m_form.StartsWith("12")) m_year = 12;
            if (m_form.StartsWith("13")) m_year = 13;
            return m_valid;
        }

        public bool Hydrate_new(SqlDataReader dr)
        {
            m_valid = true;
            m_StudentId = dr.GetGuid(0);
            m_adno = dr.GetInt32(1);
            if (!dr.IsDBNull(2)) m_upn = dr.GetString(2);
            if (!dr.IsDBNull(3)) m_exam_no = dr.GetInt32(3);
            if (!dr.IsDBNull(4)) m_doa = dr.GetDateTime(4);
            m_IsOnRole = dr.GetBoolean(5);
            m_GivenName = dr.GetString(7);
            m_Surname = dr.GetString(8);
            if (!dr.IsDBNull(9)) m_MiddleName = dr.GetString(9);
            if (!dr.IsDBNull(10)) m_dob = dr.GetDateTime(10);
            m_PersonId = dr.GetGuid(11);
            m_form = dr.GetString(14);//this will be group name....  8T-Rg  or 12-1Rg
            if (!dr.IsDBNull(15)) m_InformalName = dr.GetString(15);
            if (!dr.IsDBNull(16)) m_GoogleAppsLogin = dr.GetString(16);
            if (!dr.IsDBNull(19)) m_InReceiptPupilPremium = dr.GetBoolean(19);
            if (!dr.IsDBNull(20)) m_IsamsPupilId = dr.GetString(20);
            m_year = 14;//default;    
            if (m_form.StartsWith("7")) m_year = 7;
            if (m_form.StartsWith("8")) m_year = 8;
            if (m_form.StartsWith("9")) m_year = 9;
            if (m_form.StartsWith("10")) m_year = 10;
            if (m_form.StartsWith("11")) m_year = 11;
            if (m_form.StartsWith("12")) m_year = 12;
            if (m_form.StartsWith("13")) m_year = 13;
            return m_valid;
        }

        public string FindLastForm(ref DateTime Validto)
        {
            //going to try to find the last registration group....
            //even if not on role.....
            string Group = "";
            string s = "SELECT dbo.tbl_Core_Groups.GroupCode, dbo.tbl_Core_Student_Groups.MemberUntil, dbo.tbl_Core_Student_Groups.MemberFrom ";
            s += " FROM  dbo.tbl_Core_Student_Groups INNER JOIN ";
            s += " dbo.tbl_Core_Groups ON dbo.tbl_Core_Student_Groups.GroupId = dbo.tbl_Core_Groups.GroupId ";
            s += " INNER JOIN dbo.tbl_Core_Students ON dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students.StudentId ";
            s += " WHERE (dbo.tbl_Core_Groups.GroupPrimaryAdministrative = 1) AND (dbo.tbl_Core_Students.StudentId = '" + m_StudentId.ToString() + "') ";
            s += " ORDER BY dbo.tbl_Core_Student_Groups.MemberUntil DESC  ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Group = dr.GetString(0);
                            Validto = dr.GetDateTime(1);
                        }
                        dr.Close();
                    }
                }
            }
            return Group;
        }
    }
    [Serializable]
    public class SimpleRoom
    {
        public Guid m_RoomID;
        public string m_roomcode;
        public int m_capacity;

        public SimpleRoom() { }
        public SimpleRoom(Guid RoomId)
        {
            Load1("SELECT * FROM tbl_Core_Rooms WHERE RoomId='" + RoomId + "' ");
        }
        public SimpleRoom(string RoomCode)
        {
            Load1("SELECT * FROM tbl_Core_Rooms WHERE RoomCode='" + RoomCode + "' ");
        }

        public Guid Create(string RoomCode)
        {
            Guid Id = new Guid(); Id = Guid.NewGuid();
            Encode en = new Encode(); string s = "";
            s = "INSERT INTO dbo.tbl_Core_Rooms ";
            s += " (RoomId, RoomCode, Version  )";
            s += " VALUES ( '" + Id.ToString() + "' , '" + RoomCode + "', '20' )";
            en.ExecuteSQL(s);
            return Id;

        }

        public override string ToString()
        {
            return (m_roomcode);
        }

        public void Hydrate(SqlDataReader dr)
        {
            while (dr.Read())
            {
                m_roomcode = dr.GetString(2);
                m_RoomID = dr.GetGuid(0);
                m_capacity = 0;
                if (!dr.IsDBNull(4)) m_capacity = dr.GetInt32(4);
            }
            dr.Close();
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        Hydrate(dr);
                    }
                }
                cn.Close();
            }
        }

        public void Load(string RoomId)
        {
            string s = "SELECT * FROM tbl_Core_Rooms WHERE RoomId='" + RoomId + "' ";
            Load1(s);
        }
    }
    [Serializable]

    public class SimpleStaff
    {
        [DataMember]
        public string m_PersonGivenName;
        [DataMember]
        public string m_PersonSurname;
        [DataMember]
        public string m_Title;
        [DataMember]
        public string m_StaffCode;
        [DataMember]
        public Guid m_StaffId;
        [DataMember]
        public int m_Post;
        [DataMember]
        public Guid m_PersonId;
        public bool m_valid = false;

        public SimpleStaff()
        {
            m_StaffId = Guid.Empty;
        }

        private void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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

        public void Hydrate(SqlDataReader dr)
        {
            m_PersonGivenName = dr.GetString(4);
            m_PersonSurname = dr.GetString(5);
            m_Title = dr.GetString(10);
            m_StaffCode = dr.GetString(12);
            m_StaffId = dr.GetGuid(11);
            m_PersonId = dr.GetGuid(1);
            m_valid = true;

        }

        public void Load_AppsLogon(string logon)
        {
            string s = "SELECT * FROM qry_Cerval_Core_Staff WHERE (GoogleAppsLogin = '" + logon + "' )";
            Load1(s);
        }

        public SimpleStaff(string staffcode)
        {

            string s = "SELECT * FROM qry_Cerval_Core_Staff WHERE (StaffCode = '" + staffcode + "' )";
            Load1(s);
        }

        public SimpleStaff(Guid StaffId)
        {

            string s = "SELECT * FROM qry_Cerval_Core_Staff WHERE (StaffId = '" + StaffId.ToString() + "' )";
            Load1(s);
        }

        public bool CheckStaffFree(DateTime t1, DateTime t2)
        {
            //so we need to check scheduled periods.....
            //t1 and t2 are same day... different times...
            bool free = true;
            PupilPeriodList m_ppl1 = new PupilPeriodList();
            m_ppl1.LoadList_between("dbo.tbl_Core_Staff.StaffId", m_StaffId.ToString(), false, t1, t2);
            //so are any of these periods taking place between t1 and t2;
            foreach (ScheduledPeriod p in m_ppl1.m_pupilTTlist)
            {
                if (p.ConvertDayToDayofWeek() == t1.DayOfWeek)
                {
                    if ((p.GetStartDate(t1) > t1) && (p.GetStartDate(t1) < t2)) free = false;
                    if ((p.GetEndDate(t1) > t1) && (p.GetEndDate(t1) < t2)) free = false;
                    if ((p.GetStartDate(t1) < t1) && (p.GetEndDate(t1) > t2)) free = false;
                }
            }

            return free;
        }

        public Guid CreateNew(int Title, string Gender, string Surname, string GivenName, DateTime Dob, string staffCode, string GoogleLogin)
        {
            Person p = new Person();
            p.m_Gender = Gender;
            p.m_GivenName = GivenName;
            p.m_Surname = Surname;
            p.m_Title = Title;
            p.m_dob = Dob;
            m_PersonId = p.Save();
            string s = "";

            //now check that teh staff record doesn'e exist


            Encode en = new Encode();
            SimpleStaff s1 = new SimpleStaff(staffCode);
            if (s1.m_valid)
            {
                s = "UPDATE dbo.tbl_Core_Staff SET  StaffPersonId='" + m_PersonId.ToString() + "' WHERE (StaffId = '" + s1.m_StaffId.ToString() + "')";
                en.ExecuteSQL(s);
            }
            else
            {

                if (m_PersonId != Guid.Empty)
                {
                    s = "INSERT INTO dbo.tbl_Core_Staff ";
                    s += " (StaffPersonId, StaffCode, Version  )";
                    s += " VALUES ( '" + m_PersonId.ToString() + "' , '" + staffCode + "', '20' )";
                    en.ExecuteSQL(s);

                    SimpleStaff s2 = new SimpleStaff(staffCode);

                    s = "UPDATE dbo.tbl_Core_Staff SET  GoogleAppsLogin ='" + GoogleLogin + "' WHERE (StaffId = '" + s2.m_StaffId.ToString() + "')";
                    en.ExecuteSQL(s);
                }
                else
                {

                }
            }
            return m_PersonId;
        }


    }
    [Serializable]
    public class SimpleStudentFullList : IEnumerable
    {
        //use load left to get a list of all students... this will be slow!
        public ArrayList _studentlist = new ArrayList();
        public int count;
        public SimpleStudentFullList()
        {
            _studentlist.Clear();
            Encode en = new Encode();
            count = 0;
            string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_Core_Students.StudentId, dbo.tbl_Core_People.PersonSurname, ";
            s += " dbo.tbl_Core_People.PersonGivenName, dbo.tbl_Core_Students.StudentIsOnRole,  ";
            s += " dbo.tbl_Core_People.PersonId, dbo.tbl_Core_People.PersonMiddleName, ";
            s += " dbo.tbl_Core_People.PersonDateOfBirth, dbo.tbl_Core_Students.StudentAdmissionNumber, ";
            s += " dbo.tbl_Core_People.PersonInformalName, dbo.tbl_Core_People.PersonId, ";
            s += " dbo.tbl_Core_Students.StudentExamNumber,  ";
            s += " dbo.tbl_Core_Students.StudentAdmissionDate, dbo.tbl_Core_Students.StudentLeavingDate, ";
            s += " dbo.tbl_Core_Students.StudentUPN  ";
            s += " FROM  dbo.tbl_Core_Students ";
            s += " INNER JOIN dbo.tbl_Core_People ";
            s += " ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            p.Load_Left(dr.GetGuid(0).ToString());
                            _studentlist.Add(p);
                            p.m_StudentId = dr.GetGuid(0);
                            p.m_Surname = dr.GetString(1);
                            p.m_GivenName = dr.GetString(2);
                            p.m_IsOnRole = dr.GetBoolean(3);
                            p.m_PersonId = dr.GetGuid(4);
                            if (!dr.IsDBNull(5)) p.m_MiddleName = dr.GetString(5);
                            if (!dr.IsDBNull(6)) p.m_dob = dr.GetDateTime(6);
                            p.m_adno = dr.GetInt32(7);
                            if (!dr.IsDBNull(8)) p.m_InformalName = dr.GetString(8);
                            if (!dr.IsDBNull(10)) p.m_exam_no = dr.GetInt32(10);
                            if (!dr.IsDBNull(11)) p.m_doa = dr.GetDateTime(11);
                            if (!dr.IsDBNull(12)) p.m_dol = dr.GetDateTime(12);
                            if (!dr.IsDBNull(13)) p.m_upn = dr.GetString(13);
                            count++;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }


        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return (_studentlist as IEnumerable).GetEnumerator();
        }

        #endregion
    }
    [Serializable]
    public class SimpleStudentList : IEnumerable
    {
        //uses INTRANET_Student_Details to return a quick list of current students...
        // ie those on role AND in reg groups....
        public ArrayList _studentlist = new ArrayList();
        public int count;
        public SimpleStudentList()
        {
        }

        public SimpleStudentList(LIST_TYPE t)
        {
            string s = "";
            switch (t)
            {
                case LIST_TYPE.EMPTY: break;
                case LIST_TYPE.FULL: LoadFull(); break;
                case LIST_TYPE.NOFORM:
                    s = GetSimpleQuery(" ORDER BY    dbo.qry_Cerval_Core_Student.PersonSurname ");
                    Load1(s);
                    break;
                case LIST_TYPE.NOFORM_ONROLE:
                    s = GetSimpleQuery(" WHERE ( dbo.qry_Cerval_Core_Student.Expr13 ='1')   ORDER BY    dbo.qry_Cerval_Core_Student.PersonSurname ");
                    Load1(s);
                    break;

            }
            return;
        }
        public void Load1(string qry)
        {
            _studentlist.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(qry, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            _studentlist.Add(p);
                            p.m_Surname = dr.GetString(0);
                            p.m_GivenName = dr.GetString(1);
                            p.m_StudentId = dr.GetGuid(2);
                            p.m_PersonId = dr.GetGuid(3);
                            p.m_adno = dr.GetInt32(4);
                            p.m_IsOnRole = dr.GetBoolean(5);
                            if (!dr.IsDBNull(6)) p.m_dob = dr.GetDateTime(6);
                            if (!dr.IsDBNull(7)) p.m_exam_no = dr.GetInt32(7);
                            if (!dr.IsDBNull(8)) p.m_GoogleAppsLogin = dr.GetString(8);
                            if (!dr.IsDBNull(9)) p.m_IsamsPupilId = dr.GetString(9);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void LoadPartial()
        {
            string s = GetSimpleQuery(" ORDER BY    dbo.qry_Cerval_Core_Student.PersonSurname ");
            Load1(s);
        }
        public void LoadFull()
        {
            string s = GetSimpleQuery(" ORDER BY    dbo.qry_Cerval_Core_Student.PersonSurname ");
            Load1(s);
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                DateTime date1 = new DateTime(); date1 = DateTime.Now;
                // now need to find the year group.... need to do via reg groups....  
                //do a select on the groups table to find reg groups....
                s = " SELECT dbo.tbl_Core_Students.StudentId, dbo.tbl_Core_Groups.GroupCode, dbo.tbl_List_CourseTypes.CourseType ";
                s += "  FROM  dbo.tbl_Core_Groups INNER JOIN ";
                s += " dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Groups.GroupId = dbo.tbl_Core_Student_Groups.GroupId INNER JOIN";
                s += " dbo.tbl_Core_Students ON dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students.StudentId INNER JOIN";
                s += " dbo.tbl_Core_Courses ON dbo.tbl_Core_Groups.CourseId = dbo.tbl_Core_Courses.CourseId INNER JOIN";
                s += " dbo.tbl_List_CourseTypes ON dbo.tbl_Core_Courses.CourseType = dbo.tbl_List_CourseTypes.Id";
                //s+=" WHERE     (dbo.tbl_List_CourseTypes.CourseType = 'Registration Group') ";
                s += " WHERE  (dbo.tbl_Core_Groups.GroupPrimaryAdministrative = '1' ) ";
                //s += " AND (dbo.tbl_Core_Groups.GroupValidFrom < CONVERT(DATETIME, '2007-07-01 00:00:00', 102)) ";
                s += " AND (dbo.tbl_Core_Groups.GroupValidFrom < CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
                s += " AND (dbo.tbl_Core_Groups.GroupValidUntil > CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
                s += " AND (dbo.tbl_Core_Student_Groups.MemberFrom < CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
                s += " AND (dbo.tbl_Core_Student_Groups.MemberUntil > CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";


                //(GroupValidFrom > CONVERT(DATETIME, '2007-03-01 00:00:00', 102))
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        Guid guid = new Guid();
                        string reg;
                        int year = 0;
                        while (dr.Read())
                        {
                            guid = dr.GetGuid(0);    // student id
                            reg = dr.GetString(1);
                            if (reg.StartsWith("7")) year = 7;
                            if (reg.StartsWith("8")) year = 8;
                            if (reg.StartsWith("9")) year = 9;
                            if (reg.StartsWith("10")) year = 10;
                            if (reg.StartsWith("11")) year = 11;
                            if (reg.StartsWith("12")) year = 12;
                            if (reg.StartsWith("13")) year = 13;

                            //process reg group string
                            s = reg;
                            if (s != "")
                            {
                                if (s.IndexOf("-Rg") > 0)
                                {
                                    s = s.Substring(0, s.IndexOf("-Rg")); reg = s;
                                }
                                if (s.IndexOf("Rg") > 0)
                                {
                                    s = s.Substring(0, s.IndexOf("Rg")); reg = s;
                                }
                            }

                            //now need to match these up.......
                            foreach (SimplePupil p in _studentlist)
                            {
                                if (p.m_StudentId == guid) { p.m_year = year; p.m_valid = true; p.m_form = reg; }
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        private string GetSimpleQuery(string where)
        {
            string s = " ";
            s = "  SELECT dbo.qry_Cerval_Core_Student.PersonSurname, dbo.qry_Cerval_Core_Student.PersonGivenName, dbo.qry_Cerval_Core_Student.StudentId, ";
            s += " dbo.qry_Cerval_Core_Student.PersonId, dbo.qry_Cerval_Core_Student.Expr2, dbo.qry_Cerval_Core_Student.Expr13, ";
            s += " dbo.qry_Cerval_Core_Student.PersonDateOfBirth, dbo.qry_Cerval_Core_Student.Expr4, dbo.tbl_Core_Students.GoogleAppsLogin, dbo.tbl_Core_Students.iSAMStxtSchoolID ";
            s += " FROM            dbo.qry_Cerval_Core_Student ";
            s += " INNER JOIN   dbo.tbl_Core_Students ON dbo.qry_Cerval_Core_Student.StudentId = dbo.tbl_Core_Students.StudentId   ";
            return s + where;
        }

        public enum LIST_TYPE
        {
            FULL,
            NOFORM,
            EMPTY,
            NOFORM_ONROLE
        };
        public SimpleStudentList(string query)
        {
            _studentlist.Clear();
            Encode en = new Encode();
            count = 0;
            string db_connection = en.GetDbConnection();
            string s = "SELECT  *  FROM  INTRANET_Student_Details  ";
            if (query != "")
            {
                s += " WHERE (" + query + ") ";
            }
            s += " ORDER BY PersonSurname ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            _studentlist.Add(p); count++;
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_atDate(DateTime date1)
        {
            _studentlist.Clear();
            Encode en = new Encode();
            DateTime t1 = new DateTime(); DateTime t2 = new DateTime();
            string ds = "CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";

            string db_connection = en.GetDbConnection();

            string s = "       SELECT  dbo.tbl_Core_Students.StudentId, ";
            s += " dbo.tbl_Core_Students.StudentAdmissionNumber, ";
            s += " dbo.tbl_Core_Students.StudentUPN, ";
            s += "   dbo.tbl_Core_Students.StudentExamNumber, ";
            s += " dbo.tbl_Core_Students.StudentAdmissionDate, ";
            s += " dbo.tbl_Core_Students.StudentIsOnRole, ";//5
            s += "  dbo.tbl_Core_People.PersonTitle, ";
            s += " dbo.tbl_Core_People.PersonGivenName, ";//7
            s += " dbo.tbl_Core_People.PersonSurname, ";
            s += " dbo.tbl_Core_People.PersonMiddleName, ";//9
            s += " dbo.tbl_Core_People.PersonDateOfBirth, ";//10
            s += " dbo.tbl_Core_People.PersonId, dbo.tbl_Core_Groups.GroupValidFrom, ";
            s += "  dbo.tbl_Core_Groups.GroupValidUntil, ";//13
            s += " dbo.tbl_Core_Groups.GroupCode, ";//14
            s += " dbo.tbl_Core_People.PersonInformalName, ";//15
            s += "   dbo.tbl_Core_Students.GoogleAppsLogin, ";//16
            s += "  dbo.tbl_Core_Student_Groups.MemberFrom AS Date_from, ";
            s += "  dbo.tbl_Core_Student_Groups.MemberUntil AS Date_To ";
            s += " dbo.tbl_Core_Students.StudentInReceiptOfPupilPremium ";//19
            s += " , dbo.tbl_Core_Students.iSAMStxtSchoolID";//20
            s += " FROM         dbo.tbl_Core_Students ";
            s += " INNER JOIN dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Students.StudentId = dbo.tbl_Core_Student_Groups.StudentId ";
            s += " INNER JOIN dbo.tbl_Core_Groups ON dbo.tbl_Core_Student_Groups.GroupId = dbo.tbl_Core_Groups.GroupId ";
            s += " INNER JOIN dbo.tbl_Core_People ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId ";

            s += " WHERE     (dbo.tbl_Core_Groups.GroupPrimaryAdministrative = 1) AND ";
            s += " (dbo.tbl_Core_Students.StudentIsOnRole = 1)";
            s += "  AND  (dbo.tbl_Core_Student_Groups.MemberUntil >" + ds + ") AND ";
            s += " (dbo.tbl_Core_Student_Groups.MemberFrom < " + ds + ") ";
            s += " AND  (dbo.tbl_Core_Groups.GroupValidFrom < " + ds + ") AND ";
            s += " (dbo.tbl_Core_Groups.GroupValidUntil >  " + ds + ") ";
            s += " ORDER BY dbo.tbl_Core_People.PersonSurname";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    cm.CommandTimeout = 0;
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            t1 = dr.GetDateTime(17);//member from
                            t2 = dr.GetDateTime(18);
                            if ((date1 >= t1) && (date1 <= t2))
                            {
                                SimplePupil p = new SimplePupil();
                                _studentlist.Add(p); count++;
                                p.Hydrate_new(dr);
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_atDate(DateTime date1, bool Only_OnRole)
        {
            _studentlist.Clear();
            Encode en = new Encode();
            string ds = "CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";

            string db_connection = en.GetDbConnection();

            string s = " SELECT  dbo.tbl_Core_Students.StudentId, ";
            s += " dbo.tbl_Core_Students.StudentAdmissionNumber, ";
            s += " dbo.tbl_Core_Students.StudentUPN, ";
            s += "   dbo.tbl_Core_Students.StudentExamNumber, ";
            s += " dbo.tbl_Core_Students.StudentAdmissionDate, ";
            s += " dbo.tbl_Core_Students.StudentIsOnRole, ";//5
            s += "  dbo.tbl_Core_People.PersonTitle, ";
            s += " dbo.tbl_Core_People.PersonGivenName, ";//7
            s += " dbo.tbl_Core_People.PersonSurname, ";
            s += " dbo.tbl_Core_People.PersonMiddleName, ";//9
            s += " dbo.tbl_Core_People.PersonDateOfBirth, ";//10
            s += " dbo.tbl_Core_People.PersonId, dbo.tbl_Core_Groups.GroupValidFrom, ";
            s += "  dbo.tbl_Core_Groups.GroupValidUntil, ";//13
            s += " Form_Group, ";//14
            s += " dbo.tbl_Core_People.PersonInformalName, ";//15
            s += "   dbo.tbl_Core_Students.GoogleAppsLogin, ";//16

            s += "  dbo.tbl_Core_Student_Groups.MemberFrom AS Date_from, ";//17
            s += "  dbo.tbl_Core_Student_Groups.MemberUntil AS Date_To, ";//18
            s += " dbo.tbl_Core_Students.StudentInReceiptOfPupilPremium ";//19
            s += " , dbo.tbl_Core_Students.iSAMStxtSchoolID";//20

            s += " FROM         dbo.tbl_Core_Students INNER JOIN dbo.tbl_Core_Student_Groups ON dbo.tbl_Core_Students.StudentId = dbo.tbl_Core_Student_Groups.StudentId ";
            s += " INNER JOIN dbo.tbl_Core_Groups ON dbo.tbl_Core_Student_Groups.GroupId = dbo.tbl_Core_Groups.GroupId ";
            s += " INNER JOIN dbo.tbl_Core_People ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId ";

            s += " WHERE     (dbo.tbl_Core_Groups.GroupPrimaryAdministrative = 1) AND ";
            if (Only_OnRole) s += " (dbo.tbl_Core_Students.StudentIsOnRole = 1) AND ";
            s += " (dbo.tbl_Core_Student_Groups.MemberUntil >" + ds + ") AND ";
            s += " (dbo.tbl_Core_Student_Groups.MemberFrom < " + ds + ") AND ";
            s += " (dbo.tbl_Core_Groups.GroupValidFrom < " + ds + ") AND ";
            s += " (dbo.tbl_Core_Groups.GroupValidUntil >  " + ds + ") ";
            s += " ORDER BY tbl_Core_People.PersonSurname ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            _studentlist.Add(p); count++;
                            p.Hydrate_new(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Restrict_to_year(int year)
        {
            SimplePupil p = new SimplePupil();
            for (int i = 0; i < _studentlist.Count; i++)
            {
                try
                {
                    p = (SimplePupil)_studentlist[i];
                    if (p.m_year != year)
                    {
                        _studentlist.RemoveAt(i);
                        i--;
                    }
                }
                catch
                {
                }
            }
        }

        public void Restrict_to_those_studying(Guid courseID, DateTime date1)
        {

            Encode en = new Encode();
            string ds = "CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";
            List<Guid> student_list = new List<Guid>();
            string db_connection = en.GetDbConnection();
            string s = " SELECT dbo.tbl_Core_Student_Groups.StudentId  ";
            s += "FROM  dbo.tbl_Core_Student_Groups INNER JOIN ";
            s += "dbo.tbl_Core_Students ON dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students.StudentId INNER JOIN ";
            s += "dbo.tbl_Core_Groups ON dbo.tbl_Core_Student_Groups.GroupId = dbo.tbl_Core_Groups.GroupId INNER JOIN ";
            s += "dbo.tbl_Core_Courses ON dbo.tbl_Core_Groups.CourseId = dbo.tbl_Core_Courses.CourseId ";
            s += " WHERE (dbo.tbl_Core_Courses.CourseId = '" + courseID.ToString() + "') ";
            s += " AND (dbo.tbl_Core_Student_Groups.MemberUntil > " + ds + ") ";
            s += " AND (dbo.tbl_Core_Student_Groups.MemberFrom < " + ds + ")  ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Guid g = new Guid();
                            g = dr.GetGuid(0);
                            student_list.Add(g);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }


            SimplePupil p1 = new SimplePupil();
            for (int i = 0; i < _studentlist.Count; i++)
            {
                try
                {
                    p1 = (SimplePupil)_studentlist[i];
                    if (!student_list.Contains(p1.m_StudentId))
                    {
                        _studentlist.RemoveAt(i);
                        i--;
                    }
                }
                catch
                {
                }
            }


        }

        public void Restrict_to_those_NOTstudying(Guid courseID, DateTime date1)
        {
            Encode en = new Encode();
            string ds = "CONVERT(DATETIME, '" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ";
            List<Guid> student_list = new List<Guid>();
            string db_connection = en.GetDbConnection();
            string s = " SELECT dbo.tbl_Core_Student_Groups.StudentId  ";
            s += "FROM  dbo.tbl_Core_Student_Groups INNER JOIN ";
            s += "dbo.tbl_Core_Students ON dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students.StudentId INNER JOIN ";
            s += "dbo.tbl_Core_Groups ON dbo.tbl_Core_Student_Groups.GroupId = dbo.tbl_Core_Groups.GroupId INNER JOIN ";
            s += "dbo.tbl_Core_Courses ON dbo.tbl_Core_Groups.CourseId = dbo.tbl_Core_Courses.CourseId ";
            s += " WHERE (dbo.tbl_Core_Courses.CourseId = '" + courseID.ToString() + "') ";
            s += " AND (dbo.tbl_Core_Student_Groups.MemberUntil > " + ds + ") ";
            s += " AND (dbo.tbl_Core_Student_Groups.MemberFrom < " + ds + ")  ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Guid g = new Guid();
                            g = dr.GetGuid(0);
                            student_list.Add(g);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            SimplePupil p1 = new SimplePupil();
            for (int i = 0; i < _studentlist.Count; i++)
            {
                try
                {
                    p1 = (SimplePupil)_studentlist[i];
                    if (student_list.Contains(p1.m_StudentId)) { _studentlist.RemoveAt(i); i--; }
                }
                catch
                {
                }
            }

        }

        public void Restrict_FreeAt(Period period, int Day, DateTime date)
        {
            //going to load all students who DONT have a scheduled period
            //this has been created already
            ScheduledPeriodRawList spl1 = new ScheduledPeriodRawList();
            spl1.Load(date, period, Day); //contains all seched period for this period
            PupilGroupList pgl1 = new PupilGroupList();
            foreach (ScheduledPeriodRaw p1 in spl1.m_list)
            {
                pgl1.AddToList(p1.GroupId, date);
            }
            foreach (SimplePupil p2 in pgl1.m_pupilllist)
            {
                foreach (SimplePupil p1 in _studentlist)
                {
                    if (p2.m_StudentId == p1.m_StudentId) p1.m_valid = false;
                }
            }
            SimplePupil p = new SimplePupil();
            for (int i = 0; i < _studentlist.Count; i++)
            {
                try
                {
                    p = (SimplePupil)_studentlist[i];
                    if (!p.m_valid) { _studentlist.RemoveAt(i); i--; }
                }
                catch
                {
                }
            }


        }

        public void LoadSiblings(Guid studentId)
        {
            _studentlist.Clear();
            Guid[] parents = new Guid[5]; int no_parents = 0; int[] types = new int[5];
            Guid[] siblings = new Guid[10]; int no_siblings = 0;
            Encode en = new Encode(); count = 0; int Relationshiptype = 0;
            string db_connection = en.GetDbConnection();
            string s = "SELECT  *  FROM tbl_Core_Student_Relationships WHERE (tbl_Core_Student_Relationships.StudentId='" + studentId.ToString() + "') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Relationshiptype = dr.GetInt32(3);
                            if ((Relationshiptype == 1) || (Relationshiptype == 2))
                            { parents[no_parents] = dr.GetGuid(2); types[no_parents] = dr.GetInt32(3); no_parents++; }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            //so now have the guys real parents... 
            for (int i = 0; i < no_parents; i++)
            {
                LoadChildren(parents[i], types[i]);
                //so now we have a list of students who have this parent
                foreach (SimplePupil p0 in _studentlist)
                {
                    if (p0.m_StudentId != studentId)
                    {
                        //add to siblings if not already there.....
                        bool found = false;
                        for (int j = 0; j < no_siblings; j++)
                        {
                            if (siblings[j] == p0.m_StudentId) found = true;
                        }
                        if (!found)
                        {
                            siblings[no_siblings] = p0.m_StudentId; no_siblings++;
                        }
                    }
                }
            }
            _studentlist.Clear(); count = 0;
            for (int j = 0; j < no_siblings; j++)
            {
                SimplePupil p = new SimplePupil(); _studentlist.Add(p); count++;
                if (!p.Load(siblings[j].ToString()))
                {
                    //didn't load... assume xstudent (simplpupil uses query cross join with registration
                    p.m_StudentId = siblings[j];
                    PupilDetails pupil1 = new PupilDetails(siblings[j].ToString());
                    p.m_adno = pupil1.m_adno; p.m_doa = pupil1.m_doa; p.m_dob = pupil1.m_dob;
                    p.m_exam_no = pupil1.m_examNo; p.m_form = "left"; p.m_GivenName = pupil1.m_GivenName;
                    p.m_MiddleName = pupil1.m_MiddleName; p.m_PersonId = pupil1.m_PersonId; p.m_GoogleAppsLogin = pupil1.m_GoogleAppsLogin;
                    p.m_Surname = pupil1.m_Surname; p.m_upn = pupil1.m_upn; p.m_valid = true; p.m_year = 14;
                }
            }
        }

        public void LoadHMFList()
        {
            _studentlist.Clear();
            Encode en = new Encode();
            count = 0;
            string db_connection = en.GetDbConnection();


            string s = "SELECT dbo.INTRANET_Student_Details.* FROM  dbo.qry_User_Student_Details_VBR_Test INNER JOIN dbo.PLASC_Pupils_HMServices ON dbo.qry_User_Student_Details_VBR_Test.StudentId = dbo.PLASC_Pupils_HMServices.StudentId ";

#if DEBUG
            s = "SELECT dbo.INTRANET_Student_Details.* FROM  dbo.qry_User_Student_Details INNER JOIN dbo.PLASC_Pupils_HMServices ON dbo.qry_User_Student_Details.StudentId = dbo.PLASC_Pupils_HMServices.StudentId ";
            s += " ORDER BY qry_User_Student_Details.PersonSurname ";
#else
            s = "SELECT dbo.INTRANET_Student_Details.* FROM  dbo.qry_User_Student_Details_VBR_Test INNER JOIN dbo.PLASC_Pupils_HMServices ON dbo.qry_User_Student_Details_VBR_Test.StudentId = dbo.PLASC_Pupils_HMServices.StudentId ";
            s += " ORDER BY INTRANET_Student_Details.PersonSurname ";
#endif

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            _studentlist.Add(p); count++;
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_FreeMealsOnly()
        {
            _studentlist.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT   StudentId FROM dbo.tbl_Core_Students WHERE (StudentFreeSchoolMeals='1') AND (StudentIsOnRole='1') ";
            Guid g1 = new Guid();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            g1 = dr.GetGuid(0);
                            SimplePupil p = new SimplePupil();
                            _studentlist.Add(p); count++;
                            p.Load(g1);//will return details of all including not on role
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_SENOnly(int SENStatusType)
        {

            _studentlist.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = " SELECT dbo.qry_Cerval_Core_Student.StudentId ";
            s += " FROM  dbo.tbl_List_SenTypes INNER JOIN  ";
            s += " dbo.tbl_Core_Students_Sens ON dbo.tbl_List_SenTypes.Id = dbo.tbl_Core_Students_Sens.Type INNER JOIN   ";
            s += " dbo.qry_Cerval_Core_Student ON dbo.tbl_Core_Students_Sens.StudentId = dbo.qry_Cerval_Core_Student.StudentId INNER JOIN  ";
            s += " dbo.tbl_List_SenStatusTypes ON dbo.tbl_Core_Students_Sens.Status = dbo.tbl_List_SenStatusTypes.Id ";
            s += " WHERE dbo.tbl_List_SenStatusTypes.Id='" + SENStatusType.ToString() + "' ";
            Guid g1 = new Guid();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            g1 = dr.GetGuid(0);
                            SimplePupil p = new SimplePupil();
                            p.Load(g1);//will return details of all including not on role
                            if (p.m_IsOnRole) { _studentlist.Add(p); count++; }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }

        public void LoadChildren(Guid PersonID, int RelationshipType)
        {
            _studentlist.Clear();
            Encode en = new Encode();
            count = 0;
            string db_connection = en.GetDbConnection();
            string s = "SELECT  *  FROM tbl_Core_Student_Relationships WHERE (tbl_Core_Student_Relationships.PersonId='" + PersonID.ToString() + "')  ";
            s += " AND (RelationshipType = " + RelationshipType.ToString() + " ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            _studentlist.Add(p); count++;
                            p.m_StudentId = dr.GetGuid(1);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            foreach (SimplePupil p in _studentlist)
            {
                p.Load(p.m_StudentId.ToString());
            }
        }



        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return (_studentlist as IEnumerable).GetEnumerator();
        }

        #endregion

    }
    [DataContract]
    public class StaffList
    {
        [DataMember]
        public ArrayList m_stafflist = new ArrayList();
        public StaffList() { m_stafflist.Clear(); }
        public void LoadList(DateTime date, bool IsTeaching)//if teaching then only teaching staff, else all
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  DISTINCT dbo.tbl_Core_People.PersonGivenName, dbo.tbl_Core_People.PersonSurname, ";
            s += "dbo.tbl_List_PersonalTitles.Title, dbo.tbl_Core_Staff.StaffCode,  ";
            s += "dbo.tbl_Core_Staff.StaffId, dbo.tbl_Core_People.PersonId,  ";
            s += "  tbl_Core_TEMP_StaffContracts.Post ";
            s += " FROM  dbo.tbl_Core_People INNER JOIN  dbo.tbl_Core_Staff ";
            s += " ON dbo.tbl_Core_People.PersonId = dbo.tbl_Core_Staff.StaffPersonId INNER JOIN dbo.tbl_List_PersonalTitles ";
            s += " ON dbo.tbl_Core_People.PersonTitle = dbo.tbl_List_PersonalTitles.Id ";
            s += " INNER JOIN tbl_Core_TEMP_StaffContracts ON dbo.tbl_Core_Staff.StaffId = tbl_Core_TEMP_StaffContracts.StaffId ";
            s += " WHERE  ( ((tbl_Core_TEMP_StaffContracts.ContractEnd > CONVERT(DATETIME,'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " OR (tbl_Core_TEMP_StaffContracts.ContractEnd IS NULL))";
            s += "  AND (tbl_Core_TEMP_StaffContracts.ContractStart < CONVERT(DATETIME,'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            if (IsTeaching)
            {
                s += "  AND ( tbl_Core_TEMP_StaffContracts.Post < 7) ";
            }
            s += " )";

            s += " ORDER BY PersonSurname, PersonGivenName ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimpleStaff ss = new SimpleStaff();
                            m_stafflist.Add(ss);
                            ss.m_PersonGivenName = dr.GetString(0);
                            ss.m_PersonSurname = dr.GetString(1);
                            ss.m_Title = dr.GetString(2);
                            ss.m_StaffCode = dr.GetString(3);
                            //ss.m_IsTeaching = dr.GetBoolean(4);
                            ss.m_StaffId = dr.GetGuid(4);
                            ss.m_PersonId = dr.GetGuid(5);
                            ss.m_Post = dr.GetInt32(6);
                            ss.m_valid = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void LoadFullList()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_Core_People.PersonGivenName, dbo.tbl_Core_People.PersonSurname, ";
            s += "dbo.tbl_List_PersonalTitles.Title, dbo.tbl_Core_Staff.StaffCode,  ";
            s += "dbo.tbl_Core_Staff.StaffId, dbo.tbl_Core_People.PersonId  ";

            s += " FROM  dbo.tbl_Core_People INNER JOIN  dbo.tbl_Core_Staff ";
            s += " ON dbo.tbl_Core_People.PersonId = dbo.tbl_Core_Staff.StaffPersonId INNER JOIN dbo.tbl_List_PersonalTitles ";
            s += " ON dbo.tbl_Core_People.PersonTitle = dbo.tbl_List_PersonalTitles.Id ";

            s += " ORDER BY PersonSurname, PersonGivenName ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimpleStaff ss = new SimpleStaff();
                            m_stafflist.Add(ss);
                            ss.m_PersonGivenName = (!dr.IsDBNull(0)) ? dr.GetString(0) : "";
                            ss.m_PersonSurname = (!dr.IsDBNull(1)) ? dr.GetString(1) : "";
                            ss.m_Title = (!dr.IsDBNull(2)) ? dr.GetString(2) : "";
                            ss.m_StaffCode = (!dr.IsDBNull(3)) ? dr.GetString(3) : "";
                            //ss.m_IsTeaching = dr.GetBoolean(4);
                            ss.m_StaffId = dr.GetGuid(4);
                            ss.m_PersonId = dr.GetGuid(5);
                            //ss.m_Post = dr.GetInt32(6);
                            ss.m_valid = true;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void Restrict_to_Feee_at(DateTime t1, DateTime t2)
        {
            foreach (SimpleStaff s1 in m_stafflist)
            {
                if (!s1.CheckStaffFree(t1, t2)) s1.m_valid = false;

            }
            return;
        }

    }
    public class StaffAbsence
    {
        public Guid Id;
        public Guid StaffId;
        public int CategoryId;
        public string Notes;
        public DateTime StartTime;
        public DateTime EndTime;


        public void Load(Guid Id)
        {
            string s = "SELECT  AbsenceId, StaffId, Category AS CategoryId, Notes, StartDate, EndDate  FROM dbo.tbl_Core_Staff_Absences ";
            s += " WHERE AbsenceId = '" + Id.ToString() + "' ";
            Load1(s);
        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            StaffId = dr.GetGuid(1);
            CategoryId = dr.GetInt32(2);
            if (!dr.IsDBNull(3)) Notes = dr.GetString(3); else Notes = "";
            StartTime = dr.GetDateTime(4);
            EndTime = dr.GetDateTime(5);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
                    }
                }
                cn.Close();
            }
        }
        public void Save()
        {
            string s = "";
            if (Id == Guid.Empty)
            {
                // make a new record..
                Id = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Core_Staff_Absences (AbsenceId, StaffId, Category , Notes, StartDate, EndDate )";
                s += "VALUES ('" + Id.ToString() + "', '" + StaffId.ToString() + "' , '" + CategoryId.ToString() + "' , '" + Notes + "' , CONVERT(DATETIME, '" + StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , CONVERT(DATETIME, '" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            }
            else
            {
                s = "UPDATE dbo.tbl_Core_Staff_Absences SET StaffId='" + StaffId.ToString() + "',  Category='" + CategoryId.ToString() + "', Notes = '" + Notes + "' , StartDate =  CONVERT(DATETIME, '" + StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) , EndDate= CONVERT(DATETIME, '" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
                s += " WHERE (AbsenceId ='" + Id.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }
    }
    public class StaffAbsenceList
    {
        public ArrayList m_list = new ArrayList();

        public void LoadListDate(DateTime Start, DateTime End)
        {
            m_list.Clear();
            string s = "SELECT  *  FROM tbl_Core_Staff_Absences ";
            s += " WHERE (StartDate < CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) AND (EndDate > CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
            Load1(s);
        }

        public void LoadListDate(Guid StaffId, DateTime Start, DateTime End)
        {
            m_list.Clear();
            string s = "SELECT  *  FROM tbl_Core_Staff_Absences ";
            s += " WHERE  (StaffId = '" + StaffId.ToString() + "') AND (StartDate < CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) AND (EndDate > CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
            Load1(s);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StaffAbsence m = new StaffAbsence();
                            m_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class StaffAbsenceCategory
    {
        public int Id;
        public string Category;
        public string Description;
        public int ParentId;


        public void Load(int Id)
        {
            string s = "SELECT * FROM  tbl_List_StaffAbsenceCategory ";
            s += " WHERE Id = '" + Id.ToString() + "' ";
            Load1(s);
        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            Category = dr.GetString(1);
            if (!dr.IsDBNull(2)) Description = dr.GetString(2); else Description = "";
            if (!dr.IsDBNull(3)) ParentId = dr.GetInt32(3); else ParentId = -1;
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
                    }
                }
                cn.Close();
            }
        }


    }



    public class StaffAbsenceCategoryList
    {

        public ArrayList m_list = new ArrayList();

        public StaffAbsenceCategoryList()
        {
            m_list.Clear();
            string s = "SELECT * FROM tbl_List_StaffAbsenceCategory  ORDER BY Description";
            Load1(s);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StaffAbsenceCategory m = new StaffAbsenceCategory();
                            m_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class StaffCoverRotaPeriod
    {
        public Guid Id;
        public Guid StaffId;
        public Guid PeriodId;
        public int DayNo;
        public int Priority;
        public DateTime ValidFrom;
        public DateTime ValidTo;


        public void Load(Guid Id)
        {
            string s = "SELECT  *  FROM dbo.tbl_Core_StaffCoverRota ";
            s += " WHERE StaffCoverRotaId = '" + Id.ToString() + "' ";
            Load1(s);
        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            StaffId = dr.GetGuid(1);
            PeriodId = dr.GetGuid(2);
            DayNo = dr.GetInt32(3);
            Priority = dr.GetInt32(4);
            ValidFrom = dr.GetDateTime(5);
            ValidTo = dr.GetDateTime(6);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
                    }
                }
                cn.Close();
            }
        }
        public void Save()
        {
            string s = "";
            if (Id == Guid.Empty)
            {
                // make a new record..
                Id = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Core_StaffCoverRota (StaffCoverRotaId,StaffId, PeriodId , DayNo,Priority, ValidFrom, ValidTo,Version )";
                s += "VALUES ('" + Id.ToString() + "', '" + StaffId.ToString() + "' , '" + PeriodId.ToString() + "' , '" + DayNo.ToString() + "' , '" + Priority.ToString() + "' , CONVERT(DATETIME, '" + ValidFrom.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , CONVERT(DATETIME, '" + ValidTo.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , '1' )";
                //really ought to re-load it to get id....
            }
            else
            {
                s = "UPDATE dbo.tbl_Core_StaffCoverRota SET StaffId='" + StaffId.ToString() + "',  PeriodId='" + PeriodId.ToString() + "', DayNo = '" + DayNo.ToString() + "' ,  Priority = '" + Priority.ToString() + "' , ValidFrom=  CONVERT(DATETIME, '" + ValidFrom.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ValidTo= CONVERT(DATETIME, '" + ValidTo.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += " WHERE (StaffCoverRotaId ='" + Id.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }
        public bool Delete()
        {
            if ((Id == null) || (Id == Guid.Empty)) return false;
            Encode en = new Encode(); string s = "";
            s = "DELETE FROM tbl_Core_StaffCoverRota WHERE (StaffCoverRotaId = '" + Id.ToString() + "' ) ";
            en.ExecuteSQL(s);
            return true;
        }
    }
    public class StaffCoverRotaList
    {

        public ArrayList m_list = new ArrayList();

        public void LoadList_All(DateTime ValidTime)
        {
            m_list.Clear();
            string s = "SELECT  *  FROM dbo.tbl_Core_StaffCoverRota ";
            s += " WHERE (ValidFrom < CONVERT(DATETIME, '" + ValidTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) AND (ValidTo > CONVERT(DATETIME, '" + ValidTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
            Load1(s);
        }

        public void LoadListPeriod(Guid PeriodId, int DayNo, DateTime ValidTime)
        {
            m_list.Clear();
            string s = "SELECT  *  FROM dbo.tbl_Core_StaffCoverRota ";
            s += " WHERE (ValidFrom < CONVERT(DATETIME, '" + ValidTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) AND (ValidTo > CONVERT(DATETIME, '" + ValidTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
            s += "  AND (PeriodId = '" + PeriodId.ToString() + "' ) ";
            s += "  AND (DayNo = '" + DayNo.ToString() + "' ) ";
            Load1(s);
        }

        public void LoadListStaff(Guid StaffId, DateTime ValidTime)
        {
            m_list.Clear();
            string s = "SELECT  *  FROM dbo.tbl_Core_StaffCoverRota ";
            s += " WHERE  (StaffId = '" + StaffId.ToString() + "') AND (ValidFrom < CONVERT(DATETIME, '" + ValidTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) AND (ValidTo > CONVERT(DATETIME, '" + ValidTime.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
            Load1(s);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StaffCoverRotaPeriod m = new StaffCoverRotaPeriod();
                            m_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class StaffEmploymentCategory
    {
        public int Id;
        public string Category;
        public string Description;

        public StaffEmploymentCategory()
        {
        }
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            Category = dr.GetString(1);
            Description = dr.GetString(2);
        }

    }
    public class StaffEmploymentCategoryList
    {
        public List<StaffEmploymentCategory> m_list = new List<StaffEmploymentCategory>();

        public StaffEmploymentCategoryList()
        {
            //load from db....
            Encode en = new Encode();
            string s = "SELECT * FROM   tbl_List_StaffPosts ";
            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StaffEmploymentCategory se = new StaffEmploymentCategory();
                            se.Hydrate(dr);
                            m_list.Add(se);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }
    public class StaffPresence
    {
        public Guid m_Id;
        public Guid m_StaffId;
        public Guid m_DeviceId;
        public Guid m_RoomId;
        public DateTime m_date;
        public int m_status;
        public int m_version;
        public string m_StaffCode;
        public bool m_IsTeaching;

        public StaffPresence() { m_Id = Guid.Empty; m_IsTeaching = false; }
        public StaffPresence(Guid Id)
        {
            Encode en = new Encode();
            m_Id = Guid.Empty; m_IsTeaching = false;
            string s = "SELECT * FROM tbl_Core_Staff_Presence WHERE ( StaffPresenceId = '" + Id.ToString() + "' )";
            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read()) Hydrate(dr);
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetGuid(0);
            m_StaffId = dr.GetGuid(1);
            m_DeviceId = dr.GetGuid(2);
            m_RoomId = dr.GetGuid(3);
            m_date = dr.GetDateTime(4);
            m_status = dr.GetInt32(5);
            m_version = dr.GetInt32(6);
            m_StaffCode = dr.GetString(7);
        }
        public void Save()
        {
            string s = "";
            if (m_Id == Guid.Empty)
            {
                s = "INSERT INTO tbl_Core_Staff_Presence (StaffPresenceStaffId, StaffPresenceDeviceId, ";
                s += " StaffPresenceLocationId, StaffPresenceDate, StaffPresenceStatusId, Version )";
                s += " VALUES ( '" + m_StaffId.ToString() + "', '";
                s += m_DeviceId.ToString() + "', '";
                s += m_RoomId.ToString() + "', ";
                s += " CONVERT(DATETIME, '" + m_date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '";
                s += m_status.ToString() + "' ,'2' )";
            }
            else
            {
                //update...
                s = "UPDATE tbl_Core_Staff_Presence  ";
                s += " SET StaffPresenceStaffId = '" + m_StaffId.ToString() + "' ";
                s += ", StaffPresenceDeviceId = '" + m_DeviceId.ToString() + "' ";
                s += ", StaffPresenceLocationId = '" + m_RoomId.ToString() + "' ";
                s += ", StaffPresenceDate = CONVERT(DATETIME, '" + m_date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '";
                s += ", StaffPresenceStatusId = '" + m_status.ToString() + "' ";
                s += ", Version = '" + m_version.ToString() + "' ";
                s += " WHERE (StaffPresenceId = '" + m_Id.ToString() + "' ) ";

            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

    }
    public class StaffPresenceList
    {
        public List<StaffPresence> m_List = new List<StaffPresence>();
        public StaffPresenceList() { }
        public StaffPresenceList(DateTime Start, DateTime End)
        {
            LoadList(Start, End);
        }
        public void LoadList(DateTime Start, DateTime End)
        {
            m_List.Clear();
            string d1 = "CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string d2 = "CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT dbo.tbl_Core_Staff_Presence.* , dbo.tbl_Core_Staff.StaffCode  ";
            s += " FROM dbo.tbl_Core_Staff_Presence ";
            s += "INNER JOIN dbo.tbl_Core_Staff ON dbo.tbl_Core_Staff_Presence.StaffPresenceStaffId = dbo.tbl_Core_Staff.StaffId  ";

            s += " WHERE   (StaffPresenceDate > " + d1 + " ) AND (StaffPresenceDate < " + d2 + " ) ";
            s += " ORDER BY dbo.tbl_Core_Staff.StaffCode ";
            Load1(s);
        }

        private void Load1(string s)
        {
            Encode en = new Encode();
            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StaffPresence m = new StaffPresence();
                            m_List.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }


    public class StaffContract
    {
        //TODO
        public StaffContract() { }
        public Guid Id;
        public int ContractType;
        public Guid m_StaffId;
        public DateTime start;
        public DateTime end;
        public int Post;
        public int PrimaryRole;

        public void Load(Guid StaffId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  * FROM tbl_Core_TEMP_StaffContracts WHERE (StaffId = '" + StaffId + "' ) ";
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
                            ContractType = dr.GetInt32(2);
                            m_StaffId = StaffId;
                            start = dr.GetDateTime(3);
                            if (!dr.IsDBNull(4)) { end = dr.GetDateTime(4); }
                            Post = dr.GetInt32(5);
                            PrimaryRole = dr.GetInt32(6);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Save()
        {
            if (start > end) throw new Exception("Dates don't match");
            Encode en = new Encode(); string s = "";
            if ((Id == null) || (Id == Guid.Empty))
            {
                //make a new one.....
                Guid Id = new Guid(); Id = Guid.NewGuid();
                s = "INSERT INTO tbl_Core_TEMP_StaffContracts Id, StaffId, ContractType,start, end, Post,PrimaryRole)";

                s += "VALUES ('" + Id.ToString() + "' , '" + m_StaffId.ToString() + "' ,  '" + ContractType + "',";
                s += " CONVERT(DATETIME, '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s += " CONVERT(DATETIME, '" + end.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s += " '" + Post + "' , ";
                s += "'" + PrimaryRole + "' )";
                en.ExecuteSQL(s);
            }
            else
            {
                throw new Exception("Update not yet implemented");
            }
        }
    }



    public class StudentSENStrategy
    {
        public Guid Id;
        public string Strategy_Value;
        public bool valid;

        public void Load(String StrategyID)
        {
            if (StrategyID == "") return;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  * FROM dbo.tbl_Core_Students_Sens_Strategies WHERE (Id = '" + StrategyID + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            valid = true;
                            Id = dr.GetGuid(0);
                            Strategy_Value = dr.GetString(2);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void Update()
        {
            Encode en = new Encode();
            string s = "UPDATE dbo.tbl_Core_Students_Sens_Strategies SET StrategyValue ='";
            s += Strategy_Value + "'  WHERE (Id = '" + Id.ToString() + "' ) ";
            en.ExecuteSQL(s);
        }
        public void Delete()
        {
            Encode en = new Encode();
            string s = "DELETE FROM tbl_Core_Students_Sens_Strategies WHERE (Id = '" + Id.ToString() + "' ) ";
            en.ExecuteSQL(s);
        }
        public void Create(string StudentSenId)
        {
            Encode en = new Encode();
            string s = "INSERT INTO dbo.tbl_Core_Students_Sens_Strategies (StudentSenId, StrategyValue, Version  ) ";
            s += "VALUES ( '" + StudentSenId + "' , '" + Strategy_Value + "' , '0'  )";
            en.ExecuteSQL(s);
        }


    }


    public class StudentAccademicProfile
    {
        public List<StudentSubjectProfile> profile = new List<StudentSubjectProfile>();
        public Guid StudentId;
        public StudentAccademicProfile(Guid Id)
        {
            StudentId = Id;
        }
        public StudentAccademicProfile()
        {
            StudentId = Guid.Empty;
        }
        public int GetKeyStage_fromYear(SimplePupil p)
        {
            int KeyStage = 0; int year = p.m_year;
            if (year == 14)
            {
                GroupList gl1 = new GroupList();
                gl1.LoadList_Student(p.m_StudentId, DateTime.Now.AddMonths(-10));

                foreach (Group g in gl1._groups)
                {
                    if (g._GroupCode.ToUpper().Contains("YEAR"))
                    {
                        year = g._GroupCode.ToUpper().IndexOf("YEAR");
                        year = System.Convert.ToInt16(g._GroupCode.Substring(0, year));
                        year++;
                    }
                }
            }
            switch (year)
            {
                case 7:
                case 8:
                case 9:
                    KeyStage = 3;
                    break;
                case 10:
                case 11:
                    KeyStage = 4;
                    break;
                case 12:
                case 13:
                    KeyStage = 5;
                    break;
                case 14:
                    //this measn he isn't currently in a year group.... try to find previous?

                    break;
            }
            return KeyStage;
        }
        public double Round1(double x)
        {
            int i = (int)(x * 10);
            return (double)i / 10;
        }
        public double Round0(double x)
        {
            int i = (int)(x);
            return (double)i;
        }
        public void Load_Profile(DateTime date, ref SimplePupil p, ref int KeyStage, bool LoadExamData)
        {
            List<Course> StudentCourse = new List<Course>();
            string s = "";
            p.Load(StudentId);
            KeyStage = GetKeyStage_fromYear(p);
            CourseList cl1 = new CourseList(KeyStage);
            CourseList cl4 = new CourseList(4);//needed to get predicted grades for KS3.... ugh
            int x = 0;

            ResultsList rl1 = new ResultsList(); rl1.m_parameters = 1;

            rl1.LoadList("dbo.tbl_Core_Students.StudentId", p.m_StudentId.ToString());

            ReportList repl = new ReportList(p.m_StudentId.ToString(), KeyStage - 2);
            //going to try to add the report grades into the results...

            foreach (ReportValue v in repl.m_list)
            {
                Result res1 = new Result();
                rl1._results.Add(res1);
                res1.External = false;
                res1.Date = v.m_date;
                res1.Shortname = "Report";
                res1.Code = v.m_course;
                if (v.m_IsCommitment) res1.Resulttype = 998; else res1.Resulttype = 999;
                res1.Value = Round1(v.m_value).ToString();
                res1.ResultID = v.m_skillID;//needed in 2016 to go back and find max skill value
            }
            //date sort with newest at front...
            Result r0 = new Result();
            Result r2 = new Result();
            for (int j = 0; j < rl1._results.Count; j++)
            {
                for (int k = 0; k < (rl1._results.Count - 1 - j); k++)
                {
                    r0 = (Result)rl1._results[k]; r2 = (Result)rl1._results[k + 1];
                    if (r0.Date < r2.Date)
                    {
                        rl1._results[k] = r2; rl1._results[k + 1] = r0;
                    }
                }
            }
            double d = 0;

            ValueAddedMethodList vaml1 = new ValueAddedMethodList();
            ValueAddedMethod vam = new ValueAddedMethod();

            DateTime CourseStartDate = DateTime.Now;
            //need to get to course startdate...can we trust group vaid from?
            ValueAddedConversionList vacl1 = new ValueAddedConversionList();
            ValueAddedEquation va1 = new ValueAddedEquation();
            //going to find his current courses....
            PupilPeriodList ppl1 = new PupilPeriodList();
            ppl1.LoadList("StudentId", p.m_StudentId.ToString(), true, date);
            foreach (ScheduledPeriod sc in ppl1.m_pupilTTlist)
            {
                s = sc.m_groupcode;
                if (KeyStage == 3)
                {
                    if (s.Contains("-")) s = sc.m_groupcode.Substring(3, 2);
                    else
                        s = sc.m_groupcode.Substring(1, 2);//maths stes
                }
                else s = sc.m_groupcode.Substring(2, 2);
                //going to assume that code is correct... ie 11HI4 is History KS4...
                foreach (Course c in cl1._courses)
                {
                    if (c.CourseCode == s)
                    {
                        if (!StudentCourse.Contains(c))
                        {
                            StudentCourse.Add(c);
                            StudentSubjectProfile sbp1 = new StudentSubjectProfile();
                            sbp1.course = c;
                            sbp1.KeyStage = KeyStage;
                            profile.Add(sbp1);
                        }
                    }
                }
            }

            //so now we have a list of his courses.....
            //for each course we find latest grade and predicion?
            bool foundpg = false; bool foundcg = false; bool foundint = false; bool foundVAM = false;

            foreach (StudentSubjectProfile sp in profile)
            {
                foundpg = false; foundcg = false; foundint = false; foundVAM = false;

                //added



                CourseStartDate = new DateTime(DateTime.Now.Year, 9, 2);
                if (DateTime.Now.Month < 9) CourseStartDate = CourseStartDate.AddYears(-1);
                if (p.m_year == 11) CourseStartDate = CourseStartDate.AddYears(-1);
                if (p.m_year == 13) CourseStartDate = CourseStartDate.AddYears(-1);

                vam = vaml1.FindVAMethod(p.m_StudentId, sp.course._CourseID, CourseStartDate);
                sp.VA_ResultType = vam.m_ValueAddedOutputResultType;
                //added



                foreach (Result r in rl1._results)//they are in date order...
                {
                    if (r.Resulttype == vam.m_ValueAddedBaseResultType)
                    {
                        foundVAM = true; sp.VA_Base_Score = System.Convert.ToDouble(r.Value);
                    }
                    if (r.Code == sp.course.CourseCode)
                    {
                        if ((r.Resulttype == 999) && !foundpg)
                        {
                            foundpg = true; sp.latestProfileGrade = r;
                            ReportScale rs1 = new ReportScale(r.ResultID);//resultID is skillID I hope
                            sp.latestProfileGradeScale = rs1;
                        }
                        if ((r.Resulttype == 998) && !foundcg)
                        {
                            foundcg = true; sp.latestCommintmentGrade = r;
                            ReportScale rs1 = new ReportScale(r.ResultID);//resultID is skillID I hope
                            sp.latestCommintmentGradeScale = rs1;
                        }
                        if (!foundint && (r.Resulttype == 5) && LoadExamData)
                        {
                            foundint = true; sp.latestInternalExamResult = r;
                            ResultsList rl2 = new ResultsList();
                            rl2.m_parameters = 3;
                            rl2.m_db_field2 = "CourseId"; rl2.m_value2 = r.CourseID.ToString();
                            string date1 = "CONVERT(DATETIME, '" + r.Date.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                            string date2 = "CONVERT(DATETIME, '" + r.Date.AddDays(5).ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                            rl2.m_db_extraquery = " AND (ResultDate > " + date1 + ") AND (ResultDate < " + date2 + ")";
                            d = rl2.AverageResult("ResultType", "5", ref sp.NumberInternalResult);
                            x = (int)(d * 10);
                            sp.AvgInternalResult = (double)x / 10;
                            sp.PositionInternalResult = rl2.YearPosition("ResultType", "5", r.Value);
                        }
                        if (foundpg && foundcg && foundint && foundVAM) break;
                    }
                }
                if (sp.VA_Base_Score > 0)
                {
                    string cse1id = sp.course._CourseID.ToString();
                    if (KeyStage == 3)
                    {
                        foreach (Course c in cl1._courses)// change to get KS3 courseas???
                        {
                            if (sp.course.CourseCode == c.CourseCode) cse1id = c._CourseID.ToString();
                        }
                    }
                    va1.Load1("WHERE (ValueAddedMethodID='" + vam.m_ValueAddedMethodID.ToString() + "') AND ( CourseID='" + cse1id + "' ) ");
                    sp.PredictedGrade = 0;
                    if (va1.m_valid) sp.PredictedGrade = Round1(va1.m_coef0 + va1.m_coef1 * sp.VA_Base_Score + va1.m_coef2 * sp.VA_Base_Score * sp.VA_Base_Score);


                }
            }
        }
    }
    public class StudentSubjectProfile
    {
        public Course course;
        public Result latestProfileGrade;
        public ReportScale latestProfileGradeScale;
        public Result latestCommintmentGrade;
        public ReportScale latestCommintmentGradeScale;
        public Result latestInternalExamResult;
        public double AvgInternalResult;
        public int NumberInternalResult;
        public int PositionInternalResult;
        public double VA_Base_Score;
        public double PredictedGrade;
        public int KeyStage;
        public int VA_ResultType;

        public StudentSubjectProfile()
        {
            PredictedGrade = 0;
            VA_Base_Score = 0;
        }
        public enum ScaleType { GCSE_QCA, GCSE_19, AlevelQCA }


        //routine to convert the vlaue in x into a string for a predicted grade etc.... so 8.0 to 8 8.5 to A+ etc
        public string Score_to_String(double x)
        {
            if (x == 0) return "NA";
            double interval = 6; double top = 58;//score for exact top grade
            ScaleType t = new ScaleType();
            switch (VA_ResultType)
            {
                case 10: t = ScaleType.GCSE_QCA; break;
                case 44: t = ScaleType.GCSE_19; break;
                case 9: t = ScaleType.AlevelQCA; break;
                default: return "nan0";
            }

            string[] g = new string[10];
            int max_s = 0;
            switch (t)
            {
                case ScaleType.GCSE_QCA:
                    interval = 6; top = 58; max_s = 5;
                    g = new string[] { "A*", "A", "B", "C", "D", "E" };
                    break;
                case ScaleType.GCSE_19:
                    interval = 1; top = 9.0; max_s = 8;
                    g = new string[] { "9", "8", "7", "6", "5", "4", "3", "2", "1" };
                    break;
                case ScaleType.AlevelQCA:
                    interval = 20; top = 140; max_s = 6;
                    g = new string[] { "A*", "A", "B", "C", "D", "E", "u" };
                    break;
                default:
                    return "nan";
                    break;
            }

            double x1 = (top - interval / 2 - x) / interval + 0.01;

            //now round fractionup
            int i = (int)Math.Ceiling(x1);
            string s1 = "";
            try
            {
                s1 += g[i];//has main grade....
            }
            catch
            {
                s1 += "NA "; return s1;
            }
            // find the difference
            x1 = i - x1;// so will be  0.66-1 if -  0.33-0.66 if stet  0-0.33 if +
            if (x1 > 0.66) s1 += "+";
            else
            { if (x1 < 0.33) s1 += "-"; }


            string s = "";
            switch (t)
            {
                case ScaleType.GCSE_QCA:
                    if (x > 61) { s = "A*++"; break; }
                    if (x > 59) { s = "A*+"; break; }
                    if (x > 57) { s = "A*"; break; }
                    if (x > 55) { s = "A*-"; break; }//
                    if (x > 53) { s = "A+"; break; }
                    if (x > 51) { s = "A"; break; }
                    if (x > 49) { s = "A-"; break; }
                    if (x > 47) { s = "B+"; break; }
                    if (x > 45) { s = "B"; break; }
                    if (x > 43) { s = "B-"; break; }
                    if (x > 41) { s = "C+"; break; }
                    if (x > 39) { s = "C"; break; }
                    if (x > 37) { s = "C-"; break; }
                    if (x > 35) { s = "D+"; break; }
                    if (x > 33) { s = "D"; break; }
                    if (x > 31) { s = "D-"; break; }
                    if (x > 29) { s = "E+"; break; }
                    if (x > 27) { s = "E"; break; }
                    if (x > 25) { s = "E-"; break; }
                    if (x < 23) { s = "u"; break; }

                    break;
                case ScaleType.GCSE_19:
                    if (x > 9.5) { s = "9++"; break; }
                    if (x > 9.17) { s = "9+"; break; }
                    if (x > 8.83) { s = "9"; break; }
                    if (x > 8.5) { s = "9-"; break; }
                    if (x > 8.17) { s = "8+"; break; }
                    if (x > 7.83) { s = "8"; break; }
                    if (x > 7.5) { s = "8-"; break; }
                    if (x > 7.16) { s = "7+"; break; }
                    if (x > 6.83) { s = "7"; break; }
                    if (x > 6.5) { s = "7-"; break; }
                    if (x > 6.16) { s = "6+"; break; }
                    if (x > 5.83) { s = "6"; break; }
                    if (x > 5.5) { s = "6-"; break; }
                    if (x > 5.16) { s = "5+"; break; }
                    if (x > 4.83) { s = "5"; break; }
                    if (x > 4.5) { s = "5-"; break; }
                    if (x > 4.16) { s = "4+"; break; }
                    if (x > 3.83) { s = "4"; break; }
                    if (x > 3.5) { s = "4-"; break; }
                    if (x > 3.16) { s = "3+"; break; }
                    if (x > 2.83) { s = "3"; break; }
                    if (x > 2.5) { s = "3-"; break; }
                    if (x > 2.16) { s = "2+"; break; }
                    if (x > 1.83) { s = "2"; break; }
                    if (x > 1.5) { s = "2-"; break; }
                    if (x > 0.83) { s = "1"; break; }
                    if (x < 0.83) { s = "1-"; break; }
                    break;
                case ScaleType.AlevelQCA:
                    if (x > 150) { s = "A*++"; break; }
                    if (x > 143.33) { s = "A*+"; break; }
                    if (x > 136.66) { s = "A*"; break; }
                    if (x > 130) { s = "A*-"; break; }//
                    if (x > 123.33) { s = "A+"; break; }
                    if (x > 116.66) { s = "A"; break; }
                    if (x > 110) { s = "A-"; break; }
                    if (x > 103.33) { s = "B+"; break; }
                    if (x > 96.66) { s = "B"; break; }
                    if (x > 90) { s = "B-"; break; }
                    if (x > 83.33) { s = "C+"; break; }
                    if (x > 76.66) { s = "C"; break; }
                    if (x > 70) { s = "C-"; break; }
                    if (x > 63.33) { s = "D+"; break; }
                    if (x > 56.66) { s = "D"; break; }
                    if (x > 50) { s = "D-"; break; }
                    if (x > 43.33) { s = "E+"; break; }
                    if (x > 36.66) { s = "E"; break; }
                    if (x > 30) { s = "E-"; break; }
                    if (x < 23.33) { s = "u"; break; }
                    break;
                default:
                    s = "NA";
                    break;
            }

            return s1;
        }


        public double Convert_to_scale(ref double xgrade, ref double profilegrade, ref string PredictedGradeS, ref string ProfileGradeS)
        {
            //returns the profile grade as a GCSE/A-level score 
            // and the grade interval (xgrdae) and the predicted grade
            double r = PredictedGrade;//normal....
            PredictedGradeS = Score_to_String(r);

            xgrade = 20;//default for A-level in 2016
            try
            {
                profilegrade = System.Convert.ToDouble(latestProfileGrade.Value);
                //scale to 0-5or 1-6
                switch (KeyStage)
                {
                    case 3:
                    case 4:

                        if (latestProfileGradeScale.MaxValue > 6)
                        {
                            //on new 1-9 scale I think
                            profilegrade = profilegrade + 3;
                            xgrade = 1;// 1 grade
                            ProfileGradeS = Score_to_String(profilegrade);
                        }
                        else
                        {
                            profilegrade = 31 + 6 * profilegrade;//convert to old gcse points
                            xgrade = 6;
                            ProfileGradeS = Score_to_String(profilegrade);
                        }

                        break;
                    case 5:
                        profilegrade = 20 + 20 * profilegrade;//convert to ucas points
                        ProfileGradeS = Score_to_String(profilegrade);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                profilegrade = 0;
            }
            //round both to 1dp
            profilegrade = System.Math.Round(profilegrade, 1);
            r = System.Math.Round(r, 1);
            return r;
        }
    }


    public class StudentConsent
    {
        public string m_ConsentType;
        public string m_ConsentDescription;
        public string m_ConsentSource;
        public DateTime m_ConsentFrom;
        public DateTime m_ConsentTo;

        public bool Load(string StudentID, int consent_type)
        {
            string s = "";
            bool found = false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                s = " SELECT dbo.tbl_List_ConsentTypes.ConsentType AS Expr1, dbo.tbl_List_ConsentTypes.Description, ";
                s += " dbo.tbl_List_ConsentSources.Description AS Expr2, ";
                s += " dbo.tbl_Core_Student_Consents.ConsentFrom, dbo.tbl_Core_Student_Consents.ConsentTo ";
                s += " FROM dbo.tbl_Core_Student_Consents INNER JOIN ";
                s += " dbo.tbl_List_ConsentTypes ON dbo.tbl_Core_Student_Consents.ConsentType = dbo.tbl_List_ConsentTypes.Id ";
                s += " INNER JOIN dbo.tbl_List_ConsentSources ON dbo.tbl_Core_Student_Consents.ConsentSource = dbo.tbl_List_ConsentSources.Id ";
                s += " WHERE ( StudentID = '" + StudentID + "' ) AND ";
                s += " ( dbo.tbl_Core_Student_Consents.ConsentType ='" + consent_type.ToString() + "' )";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            m_ConsentType = dr.GetString(0);
                            m_ConsentDescription = dr.GetString(1);
                            m_ConsentSource = dr.GetString(2);
                            m_ConsentFrom = dr.GetDateTime(3);
                            if (!dr.IsDBNull(4)) m_ConsentTo = dr.GetDateTime(4);
                            found = true;
                        }
                        dr.Close();
                    }
                }
            }
            return found;
        }
    }
    public class StudentFSM
    {
        public Guid FSMId;
        public Guid StudentId;
        public DateTime StartDate;
        public DateTime EndDate;
        public bool ValidNow = false;
        public bool ValidLast6year = false;
        public string FSMCountry;

        public StudentFSM() { FSMId = Guid.Empty; }
        public void Hydrate(SqlDataReader dr)
        {
            FSMId = dr.GetGuid(0);
            StudentId = dr.GetGuid(1);
            if (!dr.IsDBNull(2)) StartDate = dr.GetDateTime(2);//can't be null....
            if (!dr.IsDBNull(3))
            {
                EndDate = dr.GetDateTime(3);
                if ((EndDate > DateTime.Now) && (StartDate < DateTime.Now)) ValidNow = true;
            }
            else
            {
                ValidNow = true;
            }
            //need to find date of January 6 years ago to do 6ever...
            DateTime t1 = new DateTime(DateTime.Now.Year - 6, 1, 1);//jan census 6 years ago...
            if (StartDate > t1) ValidLast6year = true;
            FSMCountry = dr.GetString(4);
        }
    }
    public class StudentFSMList
    {
        public ArrayList m_list = new ArrayList();
        public StudentFSMList()
        {
            m_list.Clear();
        }
        public void LoadListStudent(Guid StudentID)
        {
            m_list.Clear();
            string s = " SELECT * FROM dbo.tbl_Core_Student_FSMs ";
            s += " WHERE (StudentId ='" + StudentID.ToString() + "' ) ";
            s += " ORDER BY FSMEndDate  ";//will get nulls first....
            Load1(s);
        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentFSM m = new StudentFSM();
                            m_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class StudentPPList
    {
        public ArrayList m_list = new ArrayList();
        public StudentPPList()
        {
            m_list.Clear();
        }
        public int LoadList()
        {
            m_list.Clear();
            Encode en = new Encode();
            int count = 0;
            string db_connection = en.GetDbConnection();
            string s = "SELECT  *  FROM  INTRANET_Student_Details  ";
            s += " WHERE (StudentInReceiptOfPupilPremium  = '1' ) ";
            s += " ORDER BY PersonSurname ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            m_list.Add(p); count++;
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return count;
        }
    }
    public class StudentIncident
    {
        public Guid ID;
        public Guid StudentID;
        public int AdmissionNumber;
        public DateTime Date;
        public int Type;
        public string Text;
        public Guid StaffID;
        public string IncidentPairs;

        public StudentIncident()
        {
            ID = Guid.Empty;
        }

        public void Hydrate(SqlDataReader dr)
        {
            ID = dr.GetGuid(0);
            StudentID = dr.GetGuid(1);
            AdmissionNumber = dr.GetInt32(2);
            if (!dr.IsDBNull(3)) Date = dr.GetDateTime(3);
            if (!dr.IsDBNull(4)) Type = dr.GetInt32(4);
            Text = dr.GetString(5);
            StaffID = dr.GetGuid(6);
            IncidentPairs = dr.GetString(7);
        }

        public void Load(Guid Id)
        {
            string s = " SELECT * FROM dbo.tbl_Core_Students_Incidents ";
            s += " WHERE (IncidentID='" + Id + "' ) ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read()) Hydrate(dr);
                    }
                }
            }
        }

        public void Save()
        {
            string d1 = "CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "";
            if (ID != Guid.Empty)
            {
                s = "UPDATE dbo.tbl_Core_Students_Incidents SET ";
                s += " IncidentDate = " + d1 + " , ";
                s += " IncidentType = '" + Type.ToString() + "' , ";
                s += " IncidentText = '" + Text + "' , ";
                s += " IncidentReportingStaffID = '" + StaffID.ToString() + "' , ";
                s += " IncidentPairs = '" + IncidentPairs + "' ";
                s += " WHERE  (IncidentID = '" + ID.ToString() + "' )";
            }
            else
            {
                ID = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Core_Students_Incidents ( IncidentID, StudentID, StudentAdmissionNumber, IncidentDate, IncidentType ,  IncidentText, IncidentReportingStaffID, IncidentPairs , Version ) ";
                s += "VALUES ('" + ID.ToString() + "', '" + StudentID.ToString() + "' , '" + AdmissionNumber.ToString() + "' , " + d1 + " , '" + Type.ToString() + "', '" + Text + "' , '" + StaffID.ToString() + "' , '" + IncidentPairs + "' , '1' ) ";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public bool Delete()
        {
            if ((ID == null) || (ID == Guid.Empty)) return false;
            Encode en = new Encode();
            string s = "DELETE FROM dbo.tbl_Core_Students_Incidents WHERE (IncidentID = '" + ID.ToString() + "' ) ";
            en.ExecuteSQL(s);
            return true;
        }

    }
    public class StudentIncidentList
    {
        public ArrayList m_list = new ArrayList();
        public StudentIncidentList()
        {
            m_list.Clear();
        }

        public void LoadListDate(DateTime Start, DateTime End)
        {
            m_list.Clear();
            string d1 = "CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string d2 = "CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT * FROM dbo.tbl_Core_Students_Incidents ";
            s += " WHERE (IncidentDate <" + d1 + " ) AND (IncidentDate >" + d2 + " ) ";
            Load1(s);
        }

        public void LoadListDate(Guid StundentId, DateTime Start, DateTime End)
        {
            m_list.Clear();
            string d1 = "CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string d2 = "CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT * FROM dbo.tbl_Core_Students_Incidents ";
            s += " WHERE  (StudentID = '" + StundentId.ToString() + "') ";
            s += " AND (IncidentDate <" + d1 + " ) AND (IncidentDate >" + d2 + " ) ";
            Load1(s);
        }
        public void LoadList(Guid StundentId)
        {
            m_list.Clear();
            string s = " SELECT * FROM dbo.tbl_Core_Students_Incidents ";
            s += " WHERE  (StudentID = '" + StundentId.ToString() + "') ";
            Load1(s);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentIncident m = new StudentIncident();
                            m_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }


    // Student Development
    public class StudentDevelopment
    {
        public Guid ID;
        public Guid StudentID;
        public int AdmissionNumber;
        public DateTime Date;
        public int Type;
        public string Text;
        public Guid StaffID;
        public string StudentDevelopmentPairs;

        public StudentDevelopment()
        {
            ID = Guid.Empty;
        }

        public void Hydrate(SqlDataReader dr)
        {
            ID = dr.GetGuid(0);
            StudentID = dr.GetGuid(1);
            AdmissionNumber = dr.GetInt32(2);
            if (!dr.IsDBNull(3)) Date = dr.GetDateTime(3);
            // if (!dr.IsDBNull(4)) Type = dr.GetInt32(4);
            Text = dr.GetString(4);
            StaffID = dr.GetGuid(5);
            StudentDevelopmentPairs = dr.GetString(6);
        }

        public void Load(Guid Id)
        {
            string s = " SELECT * FROM dbo.tbl_Core_Students_StudentDevelopment ";
            s += " WHERE (StudentDevelopmentID='" + Id + "' ) ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read()) Hydrate(dr);
                    }
                }
            }
        }

        public void Save()
        {
            string d1 = "CONVERT(DATETIME, '" + Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "";
            if (ID != Guid.Empty)
            {
                s = "UPDATE dbo.tbl_Core_Students_StudentDevelopment SET ";
                s += " StudentDevelopmentDate = " + d1 + " , ";
                //s += " StudentDevelopmentType = '" + Type.ToString() + "' , ";
                s += " StudentDevelopmentText = '" + Text + "' , ";
                s += " StudentDevelopmentReportingStaffID = '" + StaffID.ToString() + "' , ";
                s += " StudentDevelopmentPairs = '" + StudentDevelopmentPairs + "' ";
                s += " WHERE  (StudentDevelopmentID = '" + ID.ToString() + "' )";
            }
            else
            {
                ID = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Core_Students_StudentDevelopment ( StudentDevelopmentID, StudentID, StudentAdmissionNumber, StudentDevelopmentDate ,  StudentDevelopmentText, StudentDevelopmentReportingStaffID, StudentDevelopmentPairs , Version ) ";
                s += "VALUES ('" + ID.ToString() + "', '" + StudentID.ToString() + "' , '" + AdmissionNumber.ToString() + "' , " + d1 + " , '" + Text + "' , '" + StaffID.ToString() + "' , '" + StudentDevelopmentPairs + "' , '1' ) ";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public bool Delete()
        {
            if ((ID == null) || (ID == Guid.Empty)) return false;
            Encode en = new Encode();
            string s = "DELETE FROM dbo.tbl_Core_Students_StudentDevelopment WHERE (StudentDevelopmentID = '" + ID.ToString() + "' ) ";
            en.ExecuteSQL(s);
            return true;
        }

    }
    public class StudentDevelopmentList
    {
        public ArrayList m2_list = new ArrayList();
        public StudentDevelopmentList()
        {
            m2_list.Clear();
        }

        public void LoadListDate(DateTime Start, DateTime End)
        {
            m2_list.Clear();
            string d1 = "CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string d2 = "CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT * FROM dbo.tbl_Core_Students_StudentDevelopment ";
            s += " WHERE (StudentDevelopmentDate <" + d1 + " ) AND (StudentDevelopmentDate >" + d2 + " ) ";
            Load1(s);
        }

        public void LoadListDate(Guid StundentId, DateTime Start, DateTime End)
        {
            m2_list.Clear();
            string d1 = "CONVERT(DATETIME, '" + End.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string d2 = "CONVERT(DATETIME, '" + Start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " SELECT * FROM dbo.tbl_Core_Students_StudentDevelopment ";
            s += " WHERE  (StudentID = '" + StundentId.ToString() + "') ";
            s += " AND (StudentDevelopmentDate <" + d1 + " ) AND (StudentDevelopmentDate >" + d2 + " ) ";
            Load1(s);
        }
        public void LoadList(Guid StundentId)
        {
            m2_list.Clear();
            string s = " SELECT * FROM dbo.tbl_Core_Students_StudentDevelopment ";
            s += " WHERE  (StudentID = '" + StundentId.ToString() + "') ";
            Load1(s);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentDevelopment m = new StudentDevelopment();
                            m2_list.Add(m);
                            m.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    //
    public class StudentLeavingDetails
    {
        public int Adno;
        public Guid LeavingDestinationId;
        public DateTime LeavingDate = new DateTime();
        public string LeavingDetails;
        public int LeavingReasonid;
        public bool valid = false;

        public StudentLeavingDetails()
        {
            valid = false;
            LeavingReasonid = 0; //null value
            LeavingDetails = "";
            LeavingDestinationId = Guid.Empty;
        }

        public StudentLeavingDetails(int AdmissionNumber)
        {
            valid = false;
            LeavingReasonid = 0; //null value
            LeavingDetails = "";
            LeavingDestinationId = Guid.Empty;

            Adno = AdmissionNumber; Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = " SELECT StudentIsOnRole, StudentLeavingDate, StudentLeavingReason, StudentLeavingDestination, StudentLeavingDetails FROM tbl_Core_Students ";
            s += " WHERE (StudentAdmissionNumber = '" + Adno.ToString() + "' )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();

                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            valid = !dr.GetBoolean(0);//only if not on role!
                            if (valid)
                            {
                                if (!dr.IsDBNull(1)) LeavingDate = dr.GetDateTime(1);
                                if (!dr.IsDBNull(2)) LeavingReasonid = dr.GetInt32(2);
                                if (!dr.IsDBNull(3)) LeavingDestinationId = dr.GetGuid(3);
                                if (!dr.IsDBNull(4)) LeavingDetails = dr.GetString(4);
                            }
                        }
                        dr.Close();
                    }
                }
            }

        }


        public void Save()
        {
            if (valid)
            {
                string s = "UPDATE tbl_Core_Students ";
                s += " SET ";
                if (LeavingDestinationId != Guid.Empty) s += " StudentLeavingDestination ='" + LeavingDestinationId.ToString() + "', ";
                s += " StudentLeavingDetails ='" + LeavingDetails + "' , ";
                s += " StudentLeavingReason ='" + LeavingReasonid.ToString() + "' ";
                s += " WHERE (StudentAdmissionNumber ='" + Adno.ToString() + "' )";
                Encode en = new Encode();
                en.ExecuteSQL(s);
            }

        }

        public void SaveOffRole()
        {
            if (valid)
            {
                string d1 = "CONVERT(DATETIME, '" + LeavingDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
                string s = "UPDATE tbl_Core_Students ";
                s += " SET ";
                s += "StudentLeavingDate =" + d1 + " , ";
                s += " StudentLeavingDetails ='' , ";
                s += " StudentLeavingReason ='0' ";//not yet entered
                s += ", StudentIsOnRole = '0'";
                s += " WHERE (StudentAdmissionNumber ='" + Adno.ToString() + "' )";
                Encode en = new Encode();
                en.ExecuteSQL(s);
            }
        }
    }


    public class StudentMedical
    {
        public string m_MedicalNotes;
        public string m_address;
        public string[] m_address_elements;
        public Relationship m_doctor;
        public string m_doctorPhoneNo;
        public ArrayList m_Consents = new ArrayList();

        public void SaveMedicalNote(string studentID, string MedicalNote)
        {
            bool found = false;
            Guid MedID = Guid.Empty;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = " SELECT * FROM tbl_Core_Students_MedicalNotes ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                s += " WHERE ( StudentID = '" + studentID + "' )";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            found = true;
                            MedID = dr.GetGuid(0);
                        }
                        dr.Close();
                    }
                }
                if (found)
                {
                    s = "UPDATE tbl_Core_Students_MedicalNotes SET MedicalNotes = '";
                    s += MedicalNote;
                    s += "'   WHERE Id='";
                    s += MedID.ToString() + "'  ";
                    en.ExecuteSQL(s);
                }
                else
                {
                    s = "INSERT INTO tbl_Core_Students_MedicalNotes (StudentId, MedicalNotes, Version ) ";
                    s += "VALUES ( '" + studentID.ToString() + "' , '";
                    s += MedicalNote + "' , '1' )";
                    en.ExecuteSQL(s);
                }
                cn.Close();
            }
        }

        public void Load(string StudentID)
        {
            m_doctor = new Relationship();
            string s = "SELECT StudentDoctor FROM dbo.tbl_Core_Students WHERE StudentID = '";
            s += StudentID + "' ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0)) m_doctor.m_PersonID = dr.GetGuid(0);
                        }
                        dr.Close();
                    }
                }
                if (m_doctor.m_PersonID != Guid.Empty)
                {
                    s = " SELECT dbo.tbl_Core_People.*, dbo.tbl_List_PersonalTitles.Title FROM dbo.tbl_Core_People ";
                    s += " INNER JOIN dbo.tbl_List_PersonalTitles ON dbo.tbl_Core_People.PersonTitle = dbo.tbl_List_PersonalTitles.Id ";
                    s += "WHERE (PersonId ='" + m_doctor.m_PersonID.ToString() + "' )";
                    using (SqlCommand cm = new SqlCommand(s, cn))
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                m_doctor.m_PersonGivenName = dr.GetString(3);
                                m_doctor.m_PersonSurname = dr.GetString(4);
                                m_doctor.m_Title = dr.GetString(11);
                            }
                            dr.Close();
                        }
                    }

                    s = "SELECT * FROM qry_Cerval_Core_AddressForPerson ";
                    s += "WHERE (PersonId ='" + m_doctor.m_PersonID.ToString() + "' )";
                    using (SqlCommand cm = new SqlCommand(s, cn))
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                m_address_elements = new string[20];
                                for (int i = 2; i < 10; i++)
                                {
                                    if (!dr.IsDBNull(i))
                                    {
                                        m_address += dr.GetString(i) + "  ";
                                        m_address_elements[i] = dr.GetString(i);
                                    }
                                }
                            }
                            dr.Close();
                        }
                    }
                    m_doctor.m_contactlist = new ContactList();
                    m_doctor.m_contactlist.LoadList(m_doctor.m_PersonID.ToString());

                    if (m_doctor.m_contactlist.m_contactlist.Count > 0)
                    {
                        Contact c = (Contact)m_doctor.m_contactlist.m_contactlist[0];
                        m_doctorPhoneNo = c.m_Contact_Value;
                    }
                }
                s = " SELECT dbo.tbl_List_ConsentTypes.ConsentType AS Expr1, dbo.tbl_List_ConsentTypes.Description, ";
                s += " dbo.tbl_List_ConsentSources.Description AS Expr2, ";
                s += " dbo.tbl_Core_Student_Consents.ConsentFrom, dbo.tbl_Core_Student_Consents.ConsentTo ";
                s += " FROM dbo.tbl_Core_Student_Consents INNER JOIN ";
                s += " dbo.tbl_List_ConsentTypes ON dbo.tbl_Core_Student_Consents.ConsentType = dbo.tbl_List_ConsentTypes.Id ";
                s += " INNER JOIN dbo.tbl_List_ConsentSources ON dbo.tbl_Core_Student_Consents.ConsentSource = dbo.tbl_List_ConsentSources.Id ";
                s += " WHERE ( StudentID = '" + StudentID + "' )";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentConsent s1 = new StudentConsent();
                            s1.m_ConsentType = dr.GetString(0);
                            s1.m_ConsentDescription = dr.GetString(1);
                            s1.m_ConsentSource = dr.GetString(2);
                            s1.m_ConsentFrom = dr.GetDateTime(3);
                            if (!dr.IsDBNull(4)) s1.m_ConsentTo = dr.GetDateTime(4);
                            m_Consents.Add(s1);
                        }
                        dr.Close();
                    }
                }
                s = " SELECT * FROM tbl_Core_Students_MedicalNotes ";
                s += " WHERE ( StudentID = '" + StudentID + "' )";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            m_MedicalNotes = dr.GetString(2);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        bool Load_ConsentOnly(string StudentID, int consent_type)
        {
            bool found = false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                string s = "";
                s = " SELECT dbo.tbl_List_ConsentTypes.ConsentType AS Expr1, dbo.tbl_List_ConsentTypes.Description, ";
                s += " dbo.tbl_List_ConsentSources.Description AS Expr2, ";
                s += " dbo.tbl_Core_Student_Consents.ConsentFrom, dbo.tbl_Core_Student_Consents.ConsentTo ";
                s += " FROM dbo.tbl_Core_Student_Consents INNER JOIN ";
                s += " dbo.tbl_List_ConsentTypes ON dbo.tbl_Core_Student_Consents.ConsentType = dbo.tbl_List_ConsentTypes.Id ";
                s += " INNER JOIN dbo.tbl_List_ConsentSources ON dbo.tbl_Core_Student_Consents.ConsentSource = dbo.tbl_List_ConsentSources.Id ";
                s += " WHERE ( StudentID = '" + StudentID + "' ) AND ";
                s += " ( ConsentType ='" + consent_type.ToString() + "' )";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentConsent s1 = new StudentConsent();
                            s1.m_ConsentType = dr.GetString(0);
                            s1.m_ConsentDescription = dr.GetString(1);
                            s1.m_ConsentSource = dr.GetString(2);
                            s1.m_ConsentFrom = dr.GetDateTime(3);
                            if (!dr.IsDBNull(4)) s1.m_ConsentTo = dr.GetDateTime(4);
                            m_Consents.Add(s1);
                            found = true;
                        }
                        dr.Close();
                    }
                }
            }
            return found;
        }
    }
    public class StudentsOnRoleNotInRegGroup
    {
        //uses qryStudentsWithoutPrimaryAdministrativeGroups to return a quick list of current students...
        // ie those on role AND NOT in reg groups....
        public ArrayList _studentlist = new ArrayList();
        public int count;
        public StudentsOnRoleNotInRegGroup()
        {
            _studentlist.Clear();
            Encode en = new Encode();
            count = 0;
            string db_connection = en.GetDbConnection();
            string s = "SELECT  *  FROM  qryStudentsWithoutPrimaryAdministrativeGroups";
            s += " ORDER BY PersonSurname ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            _studentlist.Add(p); count++;
                            p.m_StudentId = dr.GetGuid(0);
                            p.m_Surname = dr.GetString(2);
                            p.m_GivenName = dr.GetString(1);
                            p.m_adno = dr.GetInt32(4);
                            if (!dr.IsDBNull(5)) p.m_doa = dr.GetDateTime(5);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class StudentSanction
    {
        public Guid Id;
        public Guid IncidentId;
        public int SanctionType;
        public DateTime SanctionDate;
        public Guid StaffId;
        public bool completed;
        public string workset;
        public int count;

        public StudentSanction()
        {
            Id = Guid.Empty;
        }

        public void MarkAsComplete()
        {
            string s = "UPDATE dbo.tbl_Core_Students_Sanctions SET SanctionCompleted = '1' ";
            s += " WHERE (dbo.tbl_Core_Students_Sanctions.SanctionId='" + Id + "' ) ";
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public void Save()
        {
            string d1 = "CONVERT(DATETIME, '" + SanctionDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string com = "0"; if (completed) com = "1";
            string s = "";
            if (Id != Guid.Empty)
            {
                s = "UPDATE dbo.tbl_Core_Students_Sanctions SET IncidentId = '" + IncidentId.ToString() + "' , ";
                s += " SanctionType = '" + SanctionType.ToString() + "' ,";
                s += " SanctionDate = " + d1 + " , ";
                s += " StaffId = '" + StaffId.ToString() + "' , ";
                s += " SanctionCompleted = '" + com + "' , ";
                s += " WorkSet = '" + workset + "' , ";
                s += " Count = '" + count.ToString() + "' ";
                s += " WHERE  (SanctionId = '" + Id.ToString() + "' )";
            }
            else
            {
                Id = Guid.NewGuid();
                s = "INSERT INTO dbo.tbl_Core_Students_Sanctions ( SanctionId, IncidentId, SanctionType, SanctionDate, StaffId,  SanctionCompleted, WorkSet, Count ) ";
                s += "VALUES ('" + Id.ToString() + "', '" + IncidentId.ToString() + "' , '" + SanctionType.ToString() + "' , " + d1 + " , '" + StaffId.ToString() + "', '" + com + "' , '" + workset + "' , '" + count.ToString() + "' ) ";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);


        }
        public bool Delete()
        {
            if ((Id == null) || (Id == Guid.Empty)) return false;
            Encode en = new Encode();
            string s = "DELETE FROM dbo.tbl_Core_Students_Sanctions WHERE (SanctionId = '" + Id.ToString() + "' ) ";
            en.ExecuteSQL(s);
            return true;
        }
        public void Load(string SanctionId)
        {
            string s = " SELECT * FROM dbo.tbl_Core_Students_Sanctions ";
            s += " WHERE (dbo.tbl_Core_Students_Sanctions.SanctionId='" + SanctionId + "' ) ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read()) Hydrate(dr);
                    }
                }
            }
        }

        public void Load_forIncident(Guid IncidentId)
        {
            string s = " SELECT * FROM dbo.tbl_Core_Students_Sanctions ";
            s += " INNER JOIN dbo.tbl_Core_Students_Incidents ON dbo.tbl_Core_Students_Sanctions.IncidentId = dbo.tbl_Core_Students_Incidents.IncidentID ";
            s += " WHERE (dbo.tbl_Core_Students_Sanctions.IncidentId='" + IncidentId.ToString() + "' )  ORDER BY  dbo.tbl_Core_Students_Incidents.IncidentDate DESC";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read()) Hydrate(dr);
                    }
                }
            }
        }


        //
        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            IncidentId = dr.GetGuid(1);
            SanctionType = dr.GetInt32(2);
            if (!dr.IsDBNull(3)) SanctionDate = dr.GetDateTime(3);
            if (!dr.IsDBNull(4)) StaffId = dr.GetGuid(4);
            completed = dr.GetBoolean(5);
            if (!dr.IsDBNull(6)) workset = dr.GetString(6);
            count = dr.GetInt32(7);
        }

    }



    public class StudentSanctionList : IEnumerable
    {
        public ArrayList _sanctionlist = new ArrayList();
        public int count;
        public StudentSanctionList() { }
        public void Load(Guid StudentId, DateTime start, int SanctionType)
        {
            _sanctionlist.Clear();
            Encode en = new Encode();
            count = 0;
            string db_connection = en.GetDbConnection();
            string s = " SELECT * FROM dbo.tbl_Core_Students_Sanctions ";
            s += "  INNER JOIN dbo.tbl_Core_Students_Incidents ON dbo.tbl_Core_Students_Incidents.IncidentId=dbo.tbl_Core_Students_Sanctions.IncidentId ";
            s += " WHERE (dbo.tbl_Core_Students_Incidents.StudentId='" + StudentId.ToString() + "' ) ";
            s += " AND (dbo.tbl_Core_Students_Sanctions.SanctionType='" + SanctionType.ToString() + "' ) ";
            s += " AND (dbo.tbl_Core_Students_Sanctions.SanctionDate>" + en.ConvertDateTime_SQL(start) + " ) ";
            s += "  ORDER BY dbo.tbl_Core_Students_Sanctions.SanctionDate DESC ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentSanction p = new StudentSanction();
                            _sanctionlist.Add(p); count++;
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }


        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return (_sanctionlist as IEnumerable).GetEnumerator();
        }

        #endregion

    }
    public class StudentSEN
    {
        public Guid m_SENid;
        public Guid m_StudentId;
        public DateTime m_StartDate;
        public int m_SenType;
        public string m_SenDescription;
        public int m_SenStatus;
        public int m_ExamsExtraTime;
        public bool m_ExamsCanType;
        public DateTime m_EndDate;
        public bool m_EndDateValid;
        public bool m_valid;
        public StudentSENStrategyList m_strategies;
        public bool m_SpecialAccess;

        public StudentSEN(Guid Id, DateTime start, int Type, string Desc, int status, int ExtraTime, bool cantype, DateTime end)
        {
            m_SENid = Id; m_StartDate = start; m_SenType = Type; m_SenDescription = Desc; m_SenStatus = status;
            m_ExamsExtraTime = ExtraTime; m_ExamsCanType = cantype; m_EndDate = end;
            if (Id == Guid.Empty) m_valid = false; else m_valid = true; m_EndDateValid = true;
            m_strategies = new StudentSENStrategyList();
        }
        public StudentSEN(Guid Id, DateTime start, int Type, string Desc, int status, int ExtraTime, bool cantype)
        {
            m_SENid = Id; m_StartDate = start; m_SenType = Type; m_SenDescription = Desc; m_SenStatus = status;
            m_ExamsExtraTime = ExtraTime; m_ExamsCanType = cantype;
            if (Id == Guid.Empty) m_valid = false; else m_valid = true; m_EndDateValid = false;
            m_strategies = new StudentSENStrategyList();
        }

        public StudentSEN()
        {
            m_valid = false; m_EndDateValid = false;
            m_strategies = new StudentSENStrategyList();
        }

        public void Hydate(SqlDataReader dr)
        {
            m_valid = true;
            if (!dr.IsDBNull(0)) m_SENid = dr.GetGuid(0);
            if (!dr.IsDBNull(1)) m_StudentId = dr.GetGuid(1);
            if (!dr.IsDBNull(2)) m_StartDate = dr.GetDateTime(2);
            m_SenType = dr.GetInt32(3);
            if (!dr.IsDBNull(4)) m_SenDescription = dr.GetString(4);
            m_SenStatus = dr.GetInt32(5);
            if (!dr.IsDBNull(6)) m_ExamsExtraTime = dr.GetInt32(6);
            if (!dr.IsDBNull(7)) m_ExamsCanType = dr.GetBoolean(7);
            m_EndDateValid = false;
            if (!dr.IsDBNull(8))
            {
                m_EndDate = dr.GetDateTime(8); m_EndDateValid = true;
            }
            m_SpecialAccess = false;
            if (!dr.IsDBNull(9)) m_SpecialAccess = dr.GetBoolean(9);
            m_strategies.LoadList(m_SENid.ToString());

        }
        public void Load(string SENId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  * FROM dbo.tbl_Core_Students_Sens WHERE (Id = '" + SENId + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Hydate(dr);
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
            Encode en = new Encode();
            if (m_SENid != Guid.Empty)
            {
                s = " UPDATE dbo.tbl_Core_Students_Sens";
                s += " SET StartDate = " + en.ConvertDateTime_SQL(m_StartDate) + " ,  ";
                s += " Type = '" + m_SenType.ToString() + "' , ";
                s += "Description = '" + m_SenDescription + "' , ";
                s += " Status = '" + m_SenStatus.ToString() + "' , ";
                s += " ExamsExtraTime = '" + m_ExamsExtraTime.ToString() + "' , ";
                s += "ExamsCanType = '" + m_ExamsCanType + "' ";
                if (m_EndDateValid)
                { s += ", EndDate = " + en.ConvertDateTime_SQL(m_EndDate) + "  "; }
                s += " WHERE Id='" + m_SENid.ToString() + "'";
            }
            else
            {
                m_SENid = Guid.NewGuid();
                s = "INSERT  INTO   dbo.tbl_Core_Students_Sens";
                s += "( Id, StudentId, StartDate, Type, Description, Status, ExamsExtraTime, ExamsCanType ";
                if (m_EndDateValid) s += ", EndDate ";
                s += ", Version ) ";
                s += " VALUES ('" + m_SENid.ToString() + "', '" + m_StudentId.ToString() + "', " + en.ConvertDateTime_SQL(m_StartDate) + " , '";
                s += m_SenType.ToString() + "' , '" + m_SenDescription + "' , '";
                s += m_SenStatus.ToString() + "' , '" + m_ExamsExtraTime.ToString() + "' , '";
                s += m_ExamsCanType + "' ";
                if (m_EndDateValid)
                {
                    s += ", " + en.ConvertDateTime_SQL(m_EndDate) + " ";
                }
                s += ", '0' ) ";
            }
            en.ExecuteSQL(s);
        }
        public void Delete()
        {
            //first delete the strategies...
            foreach (StudentSENStrategy s in m_strategies)
            {
                s.Delete();
            }
            Encode en = new Encode();
            string s1 = "DELETE FROM dbo.tbl_Core_Students_Sens WHERE (Id = '" + m_SENid.ToString() + "' ) ";
            en.ExecuteSQL(s1);
        }

    }
    public class StudentSENList : IEnumerable
    {
        public ArrayList m_List = new ArrayList();
        public StudentSENList(String StudentId)
        {
            m_List.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT  * FROM dbo.tbl_Core_Students_Sens WHERE (StudentId = '" + StudentId + "' ) AND ( (EndDate > { fn NOW() }) OR (EndDate IS NULL)) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentSEN s1 = new StudentSEN();
                            m_List.Add(s1);
                            s1.Hydate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return (m_List as IEnumerable).GetEnumerator();
        }

        #endregion

    }
    public class StudentSENStrategyList : IEnumerable
    {
        public ArrayList m_List = new ArrayList();
        public void LoadList(String StudentSenId)
        {
            m_List.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT  * FROM dbo.tbl_Core_Students_Sens_Strategies WHERE (StudentSenId = '" + StudentSenId + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentSENStrategy s1 = new StudentSENStrategy();
                            m_List.Add(s1);
                            s1.valid = true;
                            if (!dr.IsDBNull(0)) s1.Id = dr.GetGuid(0);
                            if (!dr.IsDBNull(2)) s1.Strategy_Value = dr.GetString(2);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return (m_List as IEnumerable).GetEnumerator();
        }

        #endregion
    }

    public class StudentSENException
    {
        public Guid m_Id;
        public Guid m_StudentId;
        public Guid m_CourseId;
        public Guid m_ComponentId;
        public bool m_CanType;
        public int m_ExtraTime;

        public StudentSENException()
        {
            m_Id = new Guid();
            m_StudentId = new Guid(); m_StudentId = Guid.Empty;
            m_CourseId = new Guid(); m_CourseId = Guid.Empty;
        }

        public void Hydate(SqlDataReader dr)
        {
            m_Id = dr.GetGuid(0);
            m_StudentId = dr.GetGuid(1);
            m_CourseId = dr.GetGuid(2);
            m_ComponentId = dr.GetGuid(3);
            m_CanType = dr.GetBoolean(4);
            m_ExtraTime = dr.GetInt32(5);
        }
    }
    public class StudentSENExceptionList
    {
        public List<StudentSENException> m_list = new List<StudentSENException>();

        public void Load_Student(Guid StudentId, string YearCode, string SeasonCode)
        {
            string s = " SELECT DISTINCT dbo.tbl_Core_Students_Sen_Course_Access.Id, ";
            s += " dbo.tbl_Core_Students_Sen_Course_Access.StudentId, ";
            s += " dbo.tbl_Core_Students_Sen_Course_Access.CourseId, ";
            s += " dbo.tbl_Exams_Components.ComponentID, dbo.tbl_Core_Students_Sen_Course_Access.WillType, ";
            s += " dbo.tbl_Core_Students_Sen_Course_Access.ExtraTime ";
            s += " FROM  dbo.tbl_Exams_Components INNER JOIN ";
            s += " dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID INNER JOIN ";
            s += " dbo.tbl_Exams_Options ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN ";
            s += " dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID INNER JOIN ";
            s += " dbo.tbl_Core_Course_ExamsSyllabus ON dbo.tbl_Exams_Syllabus.SyllabusID = dbo.tbl_Core_Course_ExamsSyllabus.ExamsSyllabusId INNER JOIN ";
            s += " dbo.tbl_Core_Students_Sen_Course_Access ON dbo.tbl_Core_Course_ExamsSyllabus.CourseId = dbo.tbl_Core_Students_Sen_Course_Access.CourseId ";
            s += " WHERE(dbo.tbl_Exams_Components.YearCode = '" + YearCode + "') AND(dbo.tbl_Exams_Components.SeasonCode = '" + SeasonCode + "') ";
            s += "AND ( dbo.tbl_Core_Students_Sen_Course_Access.StudentId = '" + StudentId.ToString() + "')";
            Encode en = new Encode();
            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentSENException s1 = new StudentSENException();
                            m_list.Add(s1);
                            s1.Hydate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

            //now need to check the component_course file...
            s = " SELECT DISTINCT dbo.tbl_Core_Students_Sen_Course_Access.Id, ";
            s += " dbo.tbl_Core_Students_Sen_Course_Access.StudentId, ";
            s += " dbo.tbl_Core_Students_Sen_Course_Access.CourseId, ";
            s += "dbo.tbl_Exams_Course_Components.ComponentId, ";
            s += " dbo.tbl_Core_Students_Sen_Course_Access.WillType, ";
            s += " dbo.tbl_Core_Students_Sen_Course_Access.ExtraTime ";
            s += " FROM  dbo.tbl_Core_Students_Sen_Course_Access INNER JOIN ";
            s += " dbo.tbl_Exams_Course_Components ";
            s += " ON dbo.tbl_Exams_Course_Components.CourseId =  dbo.tbl_Core_Students_Sen_Course_Access.CourseId ";
            s += " INNER JOIN dbo.tbl_Exams_Components ";
            s += " ON dbo.tbl_Exams_Components.ComponentId =  dbo.tbl_Exams_Course_Components.ComponentId ";
            s += " WHERE(dbo.tbl_Exams_Components.YearCode = '" + YearCode + "') AND(dbo.tbl_Exams_Components.SeasonCode = '" + SeasonCode + "') ";
            s += "AND ( dbo.tbl_Core_Students_Sen_Course_Access.StudentId = '" + StudentId.ToString() + "')";

            using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentSENException s1 = new StudentSENException();
                            m_list.Add(s1);
                            s1.Hydate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }

    public class StudentYearList
    {
        public StudentYearList(System.Web.UI.WebControls.ListBox listbox, string yearstring, string datestring)
        {
            int count = StudentYearList_Load(listbox, yearstring, datestring);
            if (count == 0)
            {
                //try setting date forward to october ... assuming this is in the summer break?
                string s2 = " CONVERT(DATETIME, '" + DateTime.Now.Year.ToString() + "-10-01 00:00:00', 102)";
                //string s2 = datestring;
                count = StudentYearList_Load(listbox, yearstring, s2);
                if (count == 0)
                {
                    //possibly is year 12 where no new list yet made.... so try to go back a year???

                    s2 = " CONVERT(DATETIME, '" + DateTime.Now.Year.ToString() + "-05-01 00:00:00', 102)";
                    try
                    {
                        int i = System.Convert.ToInt16(yearstring.Substring(0, 2));
                        if (i == 12)
                        {
                            count = StudentYearList_Load(listbox, "11Year", s2);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public StudentYearList(System.Web.UI.WebControls.ListBox listbox, string yearstring, DateTime date)
        {
            int count = StudentYearList_Load(listbox, yearstring, date);
            if (count == 0)
            {
                //try setting date forward to october ... assuming this is in the summer break?
                date = new DateTime(date.Year, 10, 1);
                count = StudentYearList_Load(listbox, yearstring, date);
                if (count == 0)
                {
                    //possibly is year 12 where no new list yet made.... so try to go back a year???
                    date = new DateTime(date.Year, 5, 1);
                    try
                    {
                        int i = System.Convert.ToInt16(yearstring.Substring(0, 2));
                        if (i == 12)
                        {
                            count = StudentYearList_Load(listbox, "11Year", date);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public int StudentYearList_Load(System.Web.UI.WebControls.ListBox listbox, string yearstring, string datestring)
        {
            listbox.Items.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  tbl_Core_People.PersonSurname, tbl_Core_People.PersonGivenName,  ";
            s += "tbl_Core_Students.StudentId ";
            s += "FROM  tbl_Core_Students ";
            s += "INNER JOIN tbl_Core_People ON tbl_Core_Students.StudentPersonId = tbl_Core_People.PersonId ";
            s += "INNER JOIN tbl_Core_Student_Groups ON tbl_Core_Students.StudentId = tbl_Core_Student_Groups.StudentId ";
            s += "INNER JOIN tbl_Core_Groups ON tbl_Core_Student_Groups.GroupId = tbl_Core_Groups.GroupId ";
            if (yearstring != "")
            {
                s += "WHERE (tbl_Core_Groups.GroupName = '" + yearstring + "') AND ";
            }
            else
            {
                s += "WHERE  ";
            }
            s += " (tbl_Core_Student_Groups.MemberFrom < " + datestring + ") AND (tbl_Core_Student_Groups.MemberUntil > " + datestring + ") ";
            s += "ORDER BY tbl_Core_People.PersonSurname, tbl_Core_People.PersonGivenName ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    cm.CommandTimeout = 0;
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ListItem i = new ListItem(dr.GetString(1) + " " + dr.GetString(0), dr.GetGuid(2).ToString());
                            listbox.Items.Add(i);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return listbox.Items.Count;
        }

        public int StudentYearList_Load(System.Web.UI.WebControls.ListBox listbox, string yearstring, DateTime date)
        {
            //yearstring default to "{ fn NOW() }"
            listbox.Items.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  tbl_Core_People.PersonSurname, tbl_Core_People.PersonGivenName,  ";
            s += "tbl_Core_Students.StudentId, ";
            s += "  tbl_Core_Student_Groups.MemberFrom,tbl_Core_Student_Groups.MemberUntil ";
            s += "FROM  tbl_Core_Students ";
            s += "INNER JOIN tbl_Core_People ON tbl_Core_Students.StudentPersonId = tbl_Core_People.PersonId ";
            s += "INNER JOIN tbl_Core_Student_Groups ON tbl_Core_Students.StudentId = tbl_Core_Student_Groups.StudentId ";
            s += "INNER JOIN tbl_Core_Groups ON tbl_Core_Student_Groups.GroupId = tbl_Core_Groups.GroupId ";
            if (yearstring != "")
            {
                s += "WHERE (tbl_Core_Groups.GroupName = '" + yearstring + "') ";
            }
            s += "ORDER BY tbl_Core_People.PersonSurname, tbl_Core_People.PersonGivenName ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    cm.CommandTimeout = 0;
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if ((dr.GetDateTime(3) < date) && (dr.GetDateTime(4) > date))
                            {
                                ListItem i = new ListItem(dr.GetString(1) + " " + dr.GetString(0), dr.GetGuid(2).ToString());
                                listbox.Items.Add(i);
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return listbox.Items.Count;
        }

        public void Load_NotOnRole_ExamEntries(System.Web.UI.WebControls.ListBox listbox, string year, string season)
        {
            listbox.Items.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT DISTINCT TOP(100) PERCENT dbo.tbl_Core_Students.StudentId, dbo.tbl_Core_People.PersonGivenName, ";
            s += " dbo.tbl_Core_People.PersonSurname  FROM  dbo.tbl_Exams_Entries INNER JOIN  ";
            s += " dbo.tbl_Core_Students ON dbo.tbl_Exams_Entries.StudentID = dbo.tbl_Core_Students.StudentId INNER JOIN ";
            s += " dbo.tbl_Core_People ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId ";
            s += " WHERE(dbo.tbl_Core_Students.StudentIsOnRole = 0) AND(dbo.tbl_Exams_Entries.ExamYear = '2017') ";
            s += " AND(dbo.tbl_Exams_Entries.ExamSeason = '6') ";
            s += "ORDER BY tbl_Core_People.PersonSurname, tbl_Core_People.PersonGivenName ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    cm.CommandTimeout = 0;
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            {
                                ListItem i = new ListItem(dr.GetString(1) + " " + dr.GetString(2), dr.GetGuid(0).ToString());
                                listbox.Items.Add(i);
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }
    public class StudentGroupMembershipList : IEnumerable
    {
        public ArrayList m_list = new ArrayList();

        public void LoadList1(string s)
        {
            Encode en = new Encode();
            m_list.Clear();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            StudentGroupMembership sgm1 = new StudentGroupMembership();
                            sgm1.Hydrate(dr);
                            m_list.Add(sgm1);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
        public void LoadList(Guid StudentID, Guid GroupID, DateTime date)
        {
            string d = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT * FROM dbo.tbl_Core_Student_Groups WHERE ";
            s += "(StudentID = '" + StudentID.ToString() + "' ) AND ( GroupID='" + GroupID.ToString() + "' ) ";
            s += " AND (MemberFrom <= " + d + " ) AND (MemberUntil > " + d + " )";
            LoadList1(s);
        }
        public void LoadList(Guid StudentID, DateTime date)
        {
            string d = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT * FROM dbo.tbl_Core_Student_Groups WHERE ";
            s += "(StudentID = '" + StudentID.ToString() + "' ) ";
            s += " AND (MemberFrom < " + d + " ) AND (MemberUntil > " + d + " )";
            LoadList1(s);

        }
        public void LoadList_Group(Guid GroupID, DateTime date)
        {
            string d = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT * FROM dbo.tbl_Core_Student_Groups WHERE ";
            s += "( GroupID='" + GroupID.ToString() + "' ) ";
            s += " AND (MemberFrom <= " + d + " ) AND (MemberUntil > " + d + " )";
            LoadList1(s);
        }
        public void LoadList_All(DateTime date)
        {
            string d = "CONVERT(DATETIME, '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT * FROM dbo.tbl_Core_Student_Groups WHERE ";
            s += " (MemberFrom < " + d + " ) AND (MemberUntil > " + d + " )  ";
            s += "ORDER BY dbo.tbl_Core_Student_Groups.StudentId ";
            LoadList1(s);
        }


        #region IEnumerable Members
        public IEnumerator GetEnumerator()
        {
            return (m_list as IEnumerable).GetEnumerator();
        }

        #endregion
    }
    public class StudentGroupMembership
    {
        public Guid m_ID;
        public Guid m_Studentid;
        public Guid m_Groupid;
        public DateTime m_ValidFrom;
        public DateTime m_ValidTo;
        public bool m_valid;

        public StudentGroupMembership()
        {
            m_ValidFrom = new DateTime();
            m_ValidFrom = new DateTime();
            m_valid = false;
        }

        public void Hydrate(SqlDataReader dr)
        {
            m_ID = dr.GetGuid(0);
            m_Studentid = dr.GetGuid(1);
            m_Groupid = dr.GetGuid(2);
            m_ValidFrom = dr.GetDateTime(3);
            m_ValidTo = dr.GetDateTime(4);
            m_valid = true;
        }

        public void Save()
        {
            string d1 = "CONVERT(DATETIME, '" + m_ValidFrom.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "";
            string d2 = "CONVERT(DATETIME, '" + m_ValidTo.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            if (m_ID != Guid.Empty)
            {
                s = "UPDATE dbo.tbl_Core_Student_Groups SET MemberFrom = " + d1 + " ";
                s += " , MemberUntil = " + d2 + "  WHERE  (Id = '" + m_ID.ToString() + "' )";
                Encode en = new Encode();
                en.ExecuteSQL(s);
            }
            else
            {
                s = "INSERT INTO dbo.tbl_Core_Student_Groups ( StudentId, GroupId, MemberFrom, MemberUntil , Version ) ";
                s += "VALUES ('" + m_Studentid.ToString() + "' , '" + m_Groupid.ToString() + "' , " + d1 + " , " + d2 + " , '4' ) ";
                Encode en = new Encode();
                en.ExecuteSQL(s);
            }
        }

        public void ResetVersion(int Version)
        {
            Encode en = new Encode();
            string s = " UPDATE dbo.tbl_Core_Student_Groups SET Version='" + Version.ToString() + "'  WHERE  (Id = '" + m_ID.ToString() + "' )";
            en.ExecuteSQL(s);

        }

        public bool Delete()
        {
            if ((m_ID == null) || (m_ID == Guid.Empty)) return false;
            Encode en = new Encode();
            string s = "DELETE FROM dbo.tbl_Core_Student_Groups WHERE (Id = '" + m_ID.ToString() + "' ) ";
            en.ExecuteSQL(s);
            return true;
        }
    }


    /// <summary>
    /// This class will be used to
    /// store  ThreadResult in a
    /// HashTable which has been
    /// declared as static.
    /// </summary>
    public class ThreadResult
    {
        private static System.Collections.Hashtable
        ThreadsList = new System.Collections.Hashtable();

        public static int Progress = 0;
        public static string UpdateString;

        public static void Add(string key, object value)
        {
            ThreadsList.Add(key, value);
        }

        public static object Get(string key)
        {
            return ThreadsList[key];
        }

        public static void Remove(string key)
        {
            ThreadsList.Remove(key);
        }

        public static bool Contains(string key)
        {
            return ThreadsList.ContainsKey(key);
        }

    }

    public class TT_writer
    {
        /// <summary>
        /// class writes a timetable to http output stream as a table... or as an ordered set for json...
        /// </summary>
        private PeriodList m_pl;
        private DayList m_dl;
        public class TT_writer_data
        {
            public string l1;
            public string l2;
            public string l3;
            public TT_writer_data() { l1 = ""; l2 = ""; l3 = ""; }
            public TT_writer_data(string text1) { l1 = text1; l2 = ""; l3 = ""; }
        }
        public ArrayList m_list = new ArrayList();

        public enum TimetableType
        {
            None, Student, Staff, Room, Subject, Booking
        }

        public TT_writer()
        {

        }

        public string OutputTT_ShowFree(List<Guid> staff_ids, DateTime time)
        {
            string s = "";
            TTData tt1 = new TTData();
            TTData.TT_PeriodList pl2 = new TTData.TT_PeriodList();
            tt1.Load_DB(DateTime.Now);//load current timetable
            foreach (TTData.TT_period t in tt1.periodlist1.m_list)
            {
                if (staff_ids.Contains(t.StaffId))
                {
                    pl2.m_list.Add(t);
                }
            }
            m_pl = new PeriodList(false);// this get only major periods (ie not with parent)
            m_dl = new DayList();//get daylist
            string s1 = "";
            //assume we have 5 days.....
            //count periods
            int no_periods = 0;
            s += "<p  align=\"center\"><TABLE BORDER  class= \"TimetableTable\"  ><TR><TD></TD> ";
            foreach (Period p in m_pl.m_PeriodList)
            {
                no_periods++;
                s1 = p.m_periodname;
                if (s1 == "Registration (AM)") s1 = "AM";// to tidy up widths..
                if (s1 == "Registration (PM)") s1 = "PM";
                s += "<TD>" + s1 + "</TD>";
            }
            s += "</TR>";

            //now we need 5 more rows.... rows are in order
            foreach (days d in m_dl.m_DayList)
            {
                if ((d.m_daycode < 5))
                {
                    s += "<TR><TD>" + d.m_dayname + "</TD>";
                    // now write the periods
                    foreach (Period p in m_pl.m_PeriodList)
                    {
                        bool free = true; s1 = "";
                        foreach (TTData.TT_period t in pl2.m_list)
                        {
                            if ((d.m_daycode == t.DayNo) && (p.m_PeriodId == t.PeriodId))
                            {
                                if (staff_ids.Contains(t.StaffId))
                                {
                                    free = false;
                                    s1 += t.StaffCode + ":" + t.SetName + "</br>";
                                }
                            }
                        }
                        if (free)
                        {
                            s += "<td class =\"TimetableSplitPeriod\"></td>";
                        }
                        else
                        {
                            s += "<td>" + s1 + "</td>";
                        }

                    }
                    s += "</TR>";
                }
            }
            s += "</table></p>";
            return s;
        }

        public string OutputTT_string(string javacmd, bool GroupListLink, TimetableType type, ref PupilPeriodList m_ppl, DateTime time)
        {
            if (m_ppl == null) return "";
            if (type == TimetableType.None) return "";
            string s = "";
            string Extra_classes = "";
            //going to write a table... first get day & periods
            m_pl = new PeriodList(false);// this get only major periods (ie not with parent)
            m_dl = new DayList();
            Utility u = new Utility();
            PhysicsBooking phb1 = new PhysicsBooking(); bool Booking_found = false;//needed for bookings...
            string s1 = ""; string s2 = ""; string s3 = "";
            //assume we have 5 days.....
            //count periods
            int no_periods = 0;
            s += "<p  align=\"center\"><TABLE BORDER  class= \"TimetableTable\" " + javacmd + "  ><TR><TD></TD> ";
            foreach (Period p in m_pl.m_PeriodList)
            {
                no_periods++;
                s1 = p.m_periodname;
                if (s1 == "Registration (AM)") s1 = "AM";// to tidy up widths..
                if (s1 == "Registration (PM)") s1 = "PM";
                s += "<TD>" + s1 + "</TD>";
            }
            s += "</TR>";

            //now we need 5 more rows.... rows are in order
            foreach (days d in m_dl.m_DayList)
            {
                //code 0 = nt known
                if ((d.m_daycode < 5))
                {
                    s += "<TR><TD>" + d.m_dayname + "</TD>";
                    // now write the periods
                    foreach (Period p1 in m_pl.m_PeriodList)
                    {
                        s3 = "<TD>";
                        s1 = p1.m_periodcode;
                        foreach (ScheduledPeriod p2 in m_ppl.m_pupilTTlist)
                        {
                            string s11 = s1.Trim();
                            if ((p2.m_periodcode != s1) && (p2.m_periodcode.Substring(0, s11.Length) == s11) && (p2.m_daycode == d.m_daycode))
                            {
                                //we have a sub period
                                s3 = "<TD  class= \"TimetableSplitPeriod\">";//mark colour or cell
                                if (GroupListLink)
                                {
                                    s2 = "SimpleLists.aspx?Type=Group&Group=" + p2.m_groupcode;
                                    s2 += "&Time=" + time.ToShortDateString();
                                    s2 = "<A HREF=\"" + s2 + "\">" + p2.m_groupcode + "</A>";
                                }
                                else
                                {
                                    s2 = p2.m_groupcode;
                                }
                                Extra_classes += "<div  class= \"TimetableSplitPeriod\">" + p2.m_dayname + ", " + p2.m_PeriodStart.ToShortTimeString() + "-" + p2.m_PeriodEnd.ToShortTimeString() + ": " + s2;
                                switch (type)
                                {
                                    case TimetableType.None: break;
                                    case TimetableType.Student: Extra_classes += ", with " + p2.m_staffcode + ", in " + p2.m_roomcode; break;
                                    case TimetableType.Staff: Extra_classes += ", in " + p2.m_roomcode; break;
                                    case TimetableType.Room: Extra_classes += ", with " + p2.m_staffcode; break;
                                    default: break;
                                }
                                s2 = "PlainResponseForm.aspx?Type=MusicGroupChanges&Group=" + p2.m_groupId.ToString();
                                s2 += "&Time=" + time.ToShortDateString();
                                s2 = "<A HREF=\"" + s2 + "\">Future Changes</A>";
                                Extra_classes += " until" + p2.m_ValidityEnd.ToShortDateString() + ".  " + s2 + "</div>";
                                //would like to discover when this ends and next period....                          
                            }
                        }
                        if (type == TimetableType.Booking)
                        {
                            //if booking type check if we have a booking...
                            foreach (ScheduledPeriod p2 in m_ppl.m_pupilTTlist)
                            {
                                if ((p2.m_periodcode == s1) && (p2.m_daycode == d.m_daycode))
                                {
                                    //find if we have a booking...
                                    phb1.DayId = System.Convert.ToInt16(d.m_daycode);
                                    phb1.StaffId = new Guid(u.Get_StaffID(p2.m_staffcode));
                                    phb1.Date = System.Convert.ToDateTime(time.ToShortDateString()).AddDays(phb1.DayId);
                                    phb1.PeriodId = u.Get_PeriodId_fromcode(p2.m_periodcode);
                                    phb1.RoomId = u.Get_RoomId_fromCode(p2.m_roomcode);
                                    Booking_found = phb1.FindBooking();
                                    if (Booking_found) s3 = "<TD  class= \"TimetableBookedPeriod\">";//mark colour or cell
                                }
                            }
                        }

                        s += s3;//this adds the <td> tag and split class if needed
                        //so we are on day d and period p1.m_periodcode
                        foreach (ScheduledPeriod p2 in m_ppl.m_pupilTTlist)
                        {
                            if ((p2.m_periodcode == s1) && (p2.m_daycode == d.m_daycode))
                            {
                                if (GroupListLink)
                                {
                                    s2 = "SimpleLists.aspx?Type=Group&Group=" + p2.m_groupcode;
                                    s2 += "&Time=" + time.ToShortDateString();
                                    s2 = "<A HREF=\"" + s2 + "\">" + p2.m_groupcode + "</A>";
                                }
                                else
                                {
                                    s2 = p2.m_groupcode;
                                }
                                s += s2 + "<BR>";

                                switch (type)
                                {
                                    case TimetableType.None: break;
                                    case TimetableType.Student: s += p2.m_staffcode + "<BR>"; s += p2.m_roomcode; break;
                                    case TimetableType.Staff: s += p2.m_roomcode + "<BR>"; break;
                                    case TimetableType.Room: s += p2.m_staffcode + "<BR>"; break;
                                    case TimetableType.Booking:
                                        s += p2.m_roomcode + "<BR>";
                                        //find if we have a booking...
                                        phb1.DayId = System.Convert.ToInt16(p2.m_daycode);
                                        phb1.StaffId = new Guid(u.Get_StaffID(p2.m_staffcode));
                                        phb1.Date = System.Convert.ToDateTime(time.ToShortDateString()).AddDays(phb1.DayId);
                                        phb1.PeriodId = u.Get_PeriodId_fromcode(p2.m_periodcode);
                                        phb1.RoomId = u.Get_RoomId_fromCode(p2.m_roomcode);

                                        s2 = "BookingList.aspx?Staff=" + p2.m_staffcode;
                                        s2 += "&Room=" + p2.m_roomcode;
                                        s2 += "&Group=" + p2.m_groupcode;
                                        s2 += "&Period=" + p2.m_periodcode;
                                        s2 += "&Day=" + p2.m_daycode;
                                        s2 += "&Date=" + time.ToShortDateString();
                                        if (phb1.FindBooking()) s2 = "<A HREF=\"" + s2 + "\"> Edit </A>";
                                        else s2 = "<A HREF=\"" + s2 + "\"> Book </A>";
                                        s += s2;
                                        break;
                                    default: break;
                                }
                                //break;
                            }
                        }

                        s += "</TD>";
                    }
                    s += "</TR>";
                }
            }
            s += "</table></p>";
            s += Extra_classes;
            return s;
        }


        public ArrayList OutputTT_RAW(TimetableType type, ref PupilPeriodList m_ppl)
        {
            if (m_ppl == null) return m_list;
            if (type == TimetableType.None) return m_list;
            //going to write a table... first get day & periods
            m_pl = new PeriodList();
            m_dl = new DayList();
            string s1 = "";
            //assume we have 5 days.....
            //count periods
            int no_periods = 0;
            TT_writer_data td1 = new TT_writer_data(); m_list.Add(td1);
            foreach (Period p in m_pl.m_PeriodList)
            {
                no_periods++;
                s1 = p.m_periodname;
                if (s1 == "Registration (AM)") s1 = "AM";// to tidy up widths..
                if (s1 == "Registration (PM)") s1 = "PM";
                TT_writer_data td = new TT_writer_data(s1); m_list.Add(td);
            }
            //now we need 5 more row.... rows are in order
            foreach (days d in m_dl.m_DayList)
            {
                //code 0 = nt known
                if ((d.m_daycode < 5))
                {
                    TT_writer_data td = new TT_writer_data(d.m_dayname); m_list.Add(td);
                    // now write the periods
                    foreach (Period p1 in m_pl.m_PeriodList)
                    {
                        TT_writer_data td0 = new TT_writer_data(); m_list.Add(td0);
                        s1 = p1.m_periodcode;
                        //so we are on day d and period p1.m_periodcode
                        foreach (ScheduledPeriod p2 in m_ppl.m_pupilTTlist)
                        {
                            if ((p2.m_periodcode == s1) && (p2.m_daycode == d.m_daycode))
                            {

                                td0.l1 = p2.m_groupcode;
                                switch (type)
                                {
                                    case TimetableType.None: break;
                                    case TimetableType.Student: td0.l2 = p2.m_staffcode; td0.l3 = p2.m_roomcode; break;
                                    case TimetableType.Staff: td0.l2 = p2.m_roomcode; break;
                                    case TimetableType.Room: td0.l2 = p2.m_staffcode; break;
                                    default: break;
                                }
                                break;
                            }
                        }

                    }

                }
            }
            return m_list;
        }

        public string OutputTT_Json(TimetableType type, ref PupilPeriodList m_ppl, DateTime time, int Day_no, Guid Object_Id, string Object_Code, bool fullTT)
        {
            string ns = Environment.NewLine;
            if (type == TimetableType.None) return "";
            string s = "{" + ns + "\"Timetable\" :{" + ns;
            switch (type)
            {
                case TimetableType.None: s += "\"Type\":\"None\"," + ns; break;
                case TimetableType.Student:
                    s += "\"Type\":\"Student\"," + ns;
                    s += "\"Student\":\"" + Object_Code + "\"," + ns;
                    s += "\"StudentId\":\"" + Object_Id.ToString() + "\"," + ns;
                    break;
                case TimetableType.Staff:
                    s += "\"Type\":\"Staff\"," + ns;
                    s += "\"Staff\":\"" + Object_Code.Trim() + "\"," + ns;
                    s += "\"StaffId\":\"" + Object_Id.ToString() + "\"," + ns;
                    break;
                case TimetableType.Room:
                    s += "\"Type\":\"Room\"," + ns;
                    s += "\"Room\":\"" + Object_Code.Trim() + "\"," + ns;
                    s += "\"RoomId\":\"" + Object_Id.ToString() + "\"," + ns;
                    break;
                default: break;
            }
            s += "\"Generated\": \"" + DateTime.Now.ToString() + "\"," + ns;
            s += "\"Date\": \"" + time.ToString() + "\"," + ns;
            s += "\"Periods\": [" + ns;
            bool first = true;
            bool includep = false;
            foreach (ScheduledPeriod p2 in m_ppl.m_pupilTTlist)
            {
                includep = false;
                if ((p2.m_daycode == Day_no) || fullTT) includep = true;
                if (!p2.m_valid) includep = false;
                if (includep)
                {
                    if (first) first = false;
                    else { s += "," + ns; }
                    s += "{" + ns + "\"Day\": \"" + p2.m_dayname.ToString().Trim() + "\"," + ns;
                    s += "\"Period\": \"" + p2.m_periodcode.Trim() + "\"," + ns;
                    s += "\"Staff\": \"" + p2.m_staffcode.Trim() + "\"," + ns;
                    s += "\"Room\": \"" + p2.m_roomcode.Trim() + "\"," + ns;
                    s += "\"Group\": \"" + p2.m_groupcode.Trim() + "\"," + ns;
                    s += "\"PeriodStart\": \"" + p2.m_PeriodStart.ToShortTimeString() + "\"," + ns;
                    s += "\"PeriodEnd\": \"" + p2.m_PeriodEnd.ToShortTimeString() + "\"," + ns;
                    s += "\"ValidityStart\": \"" + p2.m_ValidityStart.ToString() + "\"," + ns;
                    s += "\"ValidityEnd\": \"" + p2.m_ValidityEnd.ToString() + "\"" + ns;
                    s += "}";
                }
            }
            s += ns + "]" + ns + "}" + ns + "}";
            return s;
        }
    }
    public class TextRecord
    {//general class for reading tab sepaated text files....  by default  will read comma sep ones too...
        public byte[] raw = new byte[255];//raw string data inc tabs....
        public string[] field = new string[255];
        public int number_fields;
        public int NoBytes = 0;
    }
    public class TextReader
    {
        public enum READ_LINE_STATUS
        {
            ENDFILE,
            VALID,
            NOTVALID
        };

        public TextReader()
        {
        }

        public READ_LINE_STATUS ReadRawLine(FileStream f, ref byte[] buffer, ref int len)
        {
            //read a line from the file into the buffer
            //try to allow for 0a & od oa line terminators..
            int n, l = 0; byte[] b = new byte[1];
            do
            {
                n = f.Read(b, 0, 1);
                if ((b[0] == 9) || (b[0] > 31)) buffer[l] = b[0];//i e ignore oa od chars...
                l++;
            }
            while ((n == 1) && (b[0] != 0x0a) && (l < len)); l--;
            len = l;
            if (n != 1) return (READ_LINE_STATUS.ENDFILE);
            if (b[0] == 0x0a) return (READ_LINE_STATUS.VALID);
            return (READ_LINE_STATUS.NOTVALID);
        }
        public READ_LINE_STATUS ReadTextLine(FileStream f, ref TextRecord e)
        {
            //
            int i, n = 0;
            string s1; char ct = (char)0x09;//tab//
            int length = 4000;
            byte[] buffer = new byte[4096];
            StringBuilder s2 = new StringBuilder();
            READ_LINE_STATUS status = ReadRawLine(f, ref buffer, ref length);
            e.NoBytes = length;
            for (int n1 = 0; n1 < 200; n1++) { e.field[n1] = ""; }
            if (status == READ_LINE_STATUS.VALID)
            {
                for (int k = 0; k < (length - 1); k++)
                {
                    s2.Append((char)buffer[k]);
                }
                s1 = s2.ToString();
                e.raw = buffer;
                while ((n < 255) && (s1.IndexOf(ct) >= 0))
                {
                    i = s1.IndexOf(ct); e.field[n] = s1.Substring(0, i); s1 = s1.Substring(i + 1);
                    n++;
                };
                if (n < 255) e.field[n] = s1;
            };
            e.number_fields = n;
            return (status);
        }
        //overloaded versions...
        public READ_LINE_STATUS ReadRawLine(Stream f, ref byte[] buffer, ref int len)//overloaded to read from a Memory Stream
        {
            //read a line from the file into the buffer
            //try to allow for 0a & od oa line terminators..
            int n, l = 0; byte[] b = new byte[1];
            do
            {
                n = f.Read(b, 0, 1);
                if ((b[0] == 9) || (b[0] > 31)) buffer[l] = b[0];//i e ignore oa od chars...
                l++;
            }
            while ((n == 1) && (b[0] != 0x0a) && (l < len)); l--;
            len = l;
            if (n != 1) return (READ_LINE_STATUS.ENDFILE);
            if (b[0] == 0x0a) return (READ_LINE_STATUS.VALID);
            return (READ_LINE_STATUS.NOTVALID);
        }
        public READ_LINE_STATUS ReadTextLine(Stream f, ref TextRecord e)//overloaded to read from a stream
        {
            //
            int i, n = 0;
            string s1; char ct = (char)0x09;//tab//
            int length = 4000;
            byte[] buffer = new byte[4096];
            StringBuilder s2 = new StringBuilder();
            READ_LINE_STATUS status = ReadRawLine(f, ref buffer, ref length);
            e.NoBytes = length;
            if (status == READ_LINE_STATUS.VALID)
            {
                for (int k = 0; k < (length - 1); k++)
                {
                    s2.Append((char)buffer[k]);
                }
                s1 = s2.ToString();
                e.raw = buffer;
                while ((n < 255) && (s1.IndexOf(ct) >= 0))
                {
                    i = s1.IndexOf(ct); e.field[n] = s1.Substring(0, i); s1 = s1.Substring(i + 1);
                    n++;
                };
                if (n < 255) e.field[n] = s1;
            };
            e.number_fields = n;
            return (status);
        }

        public READ_LINE_STATUS ReadTextLineCSV(FileStream f, ref TextRecord e)
        {
            //
            int i, n = 0;
            string s1; char ct = ',';//comma
            int length = 4000;
            byte[] buffer = new byte[4096];
            StringBuilder s2 = new StringBuilder();
            READ_LINE_STATUS status = ReadRawLine(f, ref buffer, ref length);
            e.NoBytes = length;
            if (status == READ_LINE_STATUS.VALID)
            {
                for (int k = 0; k < (length - 1); k++)
                {
                    s2.Append((char)buffer[k]);
                }
                s1 = s2.ToString();
                e.raw = buffer;
                while ((n < 255) && (s1.IndexOf(ct) >= 0))
                {
                    i = s1.IndexOf(ct); e.field[n] = s1.Substring(0, i); s1 = s1.Substring(i + 1);
                    n++;
                };
                if (n < 255) e.field[n] = s1;
            };
            e.number_fields = n;
            return (status);
        }
    }
    [Serializable]
    public class TreeViewData
    {
        [XmlAttribute]
        public string Name;
        public string GroupId;
        public int number_children;
        public string NodeType;
        public List<TreeViewData> children = new List<TreeViewData>();

        public TreeViewData()
        {
            number_children = 0; children.Clear();
        }

        public override string ToString()
        {
            return Name;
        }
    }
    public class Utility
    {
        public bool Is_staff;
        public bool Is_student;
        public int year;

        public Utility()
        {

        }

        public async void Test1()
        {
            UserCredential credential;
            using (var stream = new FileStream(@"d:\\client_secret_318674442685-a4ajr3vqnd8tkj4090p3jh83epn21ra5.apps.googleusercontent.com", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { SheetsService.Scope.Spreadsheets },
                    "user", CancellationToken.None, new FileDataStore(@"d:\\test.txt"));
            }

            // Create the service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Apps Script",
            });
            var fred = service.Spreadsheets.Get("10usxblP6yGQTB2VkYeA5xuLUMepf1TuIcyhig4H1ygk").SpreadsheetId;
            fred = fred;

        }

        public DateTime ThisExamSeasonStart()
        {
            DateTime t1 = new DateTime(DateTime.Now.Year, 5, 1);
            if (DateTime.Now.Month > 8)
            {
                //ie this is January sequence
                t1 = new DateTime(DateTime.Now.Year + 1, 1, 1);
            }
            if (DateTime.Now.Month < 2)
            {
                t1 = new DateTime(DateTime.Now.Year, 1, 1);
            }
            return t1;
        }
        public DateTime ThisExamSeasonEnd(DateTime ExamsStart)
        {
            DateTime t1 = new DateTime(ExamsStart.Year, 7, 20);
            if (ExamsStart.Month == 1) t1 = new DateTime(ExamsStart.Year, 3, 1);
            return t1;
        }
        public string ThisSeason(DateTime ExamsStart)
        {
            string s = "";
            if (ExamsStart.Month < 10) s = ExamsStart.Month.ToString();
            if (ExamsStart.Month == 10) s = "A";
            if (ExamsStart.Month == 11) s = "B";
            if (ExamsStart.Month == 12) s = "C";
            return s;
        }
        public string Get_Year(string studentId)
        {
            string s = Get_Form(studentId);
            if (s.StartsWith("1"))
            {
                return s.Substring(0, 2);
            }
            else
            {
                return s.Substring(0, 1);
            }
        }
        public int GetAdmissionNumber(string studentId)
        {
            int i = 0;
            Encode en = new Encode(); string db_connection = en.GetDbConnection();
            string s = "SELECT dbo.tbl_Core_Students.StudentAdmissionNumber  FROM  dbo.tbl_Core_Students WHERE (StudentId='" + studentId + "')";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            i = dr.GetInt32(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return i;
        }
        public int GetAdmissionNumber(Guid PersonId)
        {
            Encode en = new Encode();
            int adno = 0;
            string db_connection = en.GetDbConnection();
            string s = "SELECT StudentAdmissionNumber FROM dbo.tbl_Core_Students ";
            s += " WHERE (StudentPersonId='" + PersonId.ToString() + "')";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            adno = dr.GetInt32(0);
                            Is_student = true;
                        }
                    }
                }
            }
            return adno;
        }
        public string Get_Form(string studentId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT     dbo.tbl_Core_Students.StudentId, dbo.tbl_Core_Groups.GroupCode, dbo.tbl_Core_Courses.CourseCode, ";
            s += "dbo.tbl_Core_Courses.CourseType AS Expr1 ";
            s += "FROM         dbo.tbl_Core_Student_Groups INNER JOIN ";
            s += "dbo.tbl_Core_Students ON dbo.tbl_Core_Student_Groups.StudentId = dbo.tbl_Core_Students.StudentId INNER JOIN ";
            s += "dbo.tbl_Core_Groups ON dbo.tbl_Core_Student_Groups.GroupId = dbo.tbl_Core_Groups.GroupId INNER JOIN ";
            s += "dbo.tbl_Core_Courses ON dbo.tbl_Core_Groups.CourseId = dbo.tbl_Core_Courses.CourseId  ";
            s += "WHERE (dbo.tbl_Core_Student_Groups.StudentId = '" + studentId + "')";
            s += " AND (dbo.tbl_Core_Groups.GroupPrimaryAdministrative = '1')";
            s += " AND  (dbo.tbl_Core_Groups.GroupValidFrom < { fn NOW() }) AND (dbo.tbl_Core_Groups.GroupValidUntil > { fn NOW() })";
            s += " AND  (dbo.tbl_Core_Student_Groups.MemberUntil > { fn NOW() }) AND (dbo.tbl_Core_Student_Groups.MemberFrom < { fn NOW() }) ";
            s += " AND (dbo.tbl_Core_Groups.GroupPrimaryAdministrative = 1) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    s = "";
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            s = dr.GetString(1);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            if (s == "") return s;
            //s may be 7N/Rg  ...  or 7N-Rg   or 7NRg  or 13-6Rg
            s = s.Trim();
            int i = s.IndexOf("Rg"); if (i > 0) s = s.Substring(0, i);//strip Rg
            i = s.IndexOf("/");
            if (i > 0) return s.Substring(0, i);
            i = s.IndexOf("-");
            if (i == (s.Length - 1)) return s.Substring(0, i);//it was 7N-
            if (i == (s.Length - 2)) return s;//it was 13-6
            return s;
        }
        public Guid Get_RoomId_fromCode(string room_code)
        {
            RoomList Rooml1 = new RoomList(); Rooml1.LoadList();
            foreach (SimpleRoom r in Rooml1.m_roomlist)
            {
                if (r.m_roomcode.Trim() == room_code.Trim()) return r.m_RoomID;
            }
            return Guid.Empty;
        }
        public Guid Get_PeriodId_fromcode(string Period_code)
        {
            PeriodList periodl1 = new PeriodList();
            foreach (Period p in periodl1.m_PeriodList)
            {
                if (p.m_periodcode == Period_code) return p.m_PeriodId;
            }
            return Guid.Empty;
        }
        public String Get_StaffID(string StaffCode)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  StaffID FROM tbl_Core_Staff WHERE StaffCode = '" + StaffCode + "'";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    s = "";
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            s = dr.GetSqlGuid(0).ToString();
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return s;
        }
        public string GetRegex_email()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT ValidationRegexRule FROM dbo.tbl_List_ContactTypes ";
            s += "WHERE (Id='4') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            return dr.GetString(0);
                        }
                    }
                }
            }
            return "";
        }
        public String Get_StaffEmailAddress(string StaffCode)
        {
            //first try contacts..
            Utility u = new Utility();

            RegexStringValidator rg1 = new RegexStringValidator(u.GetRegex_email());
            Encode en = new Encode();
            bool valid = false;
            string db_connection = en.GetDbConnection();
            string s = "SELECT  * FROM  qry_User_StaffEmailAddresses WHERE StaffCode = '" + StaffCode + "'";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    s = "";
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read() && !valid)
                        {
                            if (!dr.IsDBNull(0)) s = dr.GetString(3);
                            try
                            {
                                rg1.Validate(s);
                                if (s.ToUpper().Contains("CHALLONERS.COM"))
                                {
                                    // only use company emails....
                                    valid = true;
                                }
                            }
                            catch (ArgumentException e1)
                            {
                                string serror = e1.Message;
                                //what to do with rubbish emails,..
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            if (valid) return s;
            //else try returning the NTusername..
            return "";
            //return Get_StaffNTUsername(StaffCode) + "@challoners.com";
        }
        public string GetEmailAddress(Guid PersonId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT Contactvalue FROM dbo.qry_Contacts ";
            s += " WHERE (PersonId ='" + PersonId.ToString() + "' )";
            s += " AND (ContactType='Email') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            return dr.GetString(0);
                        }
                    }
                }
            }
            return "";
        }

        public String Get_StaffNTUsernameX(string StaffCode)
        {   //depricated May 2106
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  StaffNTUsername FROM tbl_Core_Staff WHERE StaffCode = '" + StaffCode + "'";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    s = "";
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (!dr.IsDBNull(0)) s = dr.GetString(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return s;
        }
        public String Get_StaffCodefromStaffID(Guid StaffId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  StaffCode FROM tbl_Core_Staff WHERE StaffId = '" + StaffId.ToString() + "'";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    s = "";
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            s = dr.GetString(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return s;
        }
        public String Get_StaffCode(Guid PersonId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  StaffCode FROM tbl_Core_Staff WHERE StaffPersonId = '" + PersonId.ToString() + "'";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    s = "";
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            s = dr.GetString(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return s;
        }
        public Guid GetPersonIDX(string user_name, out string staff_code)
        {
            //depricated May 2016
            staff_code = "";
            Guid Id = new Guid(); Id = Guid.Empty;
            if (user_name.IndexOf("\\") > 0)
            {
                staff_code = "";
                string dom = user_name.Split('\\')[0].ToUpper();
                string usr = user_name.Split('\\')[1].ToLower();
                usr = CleanInvertedCommas(usr);
                //could be a member of staff....
                // or a student.... try both separately...
                Encode en = new Encode();
                string db_connection = en.GetDbConnection();
                string s = "SELECT StudentPersonId FROM dbo.tbl_Core_Students ";
                s += " WHERE (StudentNTDomain ='" + dom + "') AND ";
                s += "( StudentNTUsername ='" + usr + "' ) ";
                try
                {
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
                                    Is_student = true;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
                //try staff
                if (!Is_student)
                {
                    s = "SELECT StaffPersonId, StaffCode, StaffId FROM dbo.tbl_Core_Staff ";
                    s += " WHERE (StaffNTDomain ='" + dom + "') AND ";
                    s += "( StaffNTUsername ='" + usr + "' ) ";
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
                                    staff_code = dr.GetString(1);
                                    Is_staff = true;
                                }
                            }
                        }
                    }
                }
            }
            return Id;
        }


        public Guid GetPersonIDX(string user_name)
        {
            //depricated May 2016
            Guid Id = new Guid(); Id = Guid.Empty;
            if (user_name.IndexOf("\\") > 0)
            {
                string dom = user_name.Split('\\')[0].ToUpper();
                string usr = user_name.Split('\\')[1].ToLower();
                usr = CleanInvertedCommas(usr);
                //could be a member of staff....
                // or a student.... try both separately...
                Encode en = new Encode();
                string db_connection = en.GetDbConnection();
                string s = "SELECT StudentPersonId FROM dbo.tbl_Core_Students ";
                s += " WHERE (StudentNTDomain ='" + dom + "') AND ";
                s += "( StudentNTUsername ='" + usr + "' ) ";
                try
                {
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
                                    Is_student = true;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
                //try staff
                if (!Is_student)
                {
                    s = "SELECT StaffPersonId, StaffCode, StaffId FROM dbo.tbl_Core_Staff ";
                    s += " WHERE (StaffNTDomain ='" + dom + "') AND ";
                    s += "( StaffNTUsername ='" + usr + "' ) ";
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
                                    Is_staff = true;
                                }
                            }
                        }
                    }
                }
            }
            return Id;
        }
        public string GetPersonName(Guid PersonId)
        {
            Encode en = new Encode();
            string name = "";
            string db_connection = en.GetDbConnection();
            string s = "SELECT PersonGivenName, PersonSurname FROM dbo.tbl_Core_People ";
            s += " WHERE (PersonId='" + PersonId.ToString() + "')";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            name = dr.GetString(0) + " " + dr.GetString(1);
                        }
                    }
                }
            }
            return name;
        }
        public int GetStudentAdno_fromNTUserNameX(string user_name, out Guid StudentId)
        {
            StudentId = Guid.Empty; int adno = -1; Is_student = false; Is_staff = false;
            if (user_name.IndexOf("\\") > 0)
            {
                string dom = user_name.Split('\\')[0].ToUpper();
                string usr = user_name.Split('\\')[1].ToLower();
                //could be a member of staff....
                // or a student.... try both separately...
                Encode en = new Encode();
                string db_connection = en.GetDbConnection();
                string s = "SELECT StudentId, StudentAdmissionNumber FROM dbo.tbl_Core_Students ";
                s += " WHERE (StudentNTDomain ='" + dom + "') AND ";
                s += "( StudentNTUsername ='" + CleanInvertedCommas(usr) + "' ) ";
                using (SqlConnection cn = new SqlConnection(db_connection))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(s, cn))
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                StudentId = dr.GetGuid(0);
                                adno = dr.GetInt32(1);
                                Is_student = true;
                            }
                        }
                    }
                }
            }
            return adno;
        }
        public int DeleteSanctions(string IncidentId)
        {
            string s = " DELETE  FROM dbo.tbl_Core_Students_Sanctions ";
            s += "WHERE (dbo.tbl_Core_Students_Sanctions.IncidentId='" + IncidentId + "' )";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    return cm.ExecuteNonQuery();
                }
            }
        }
        public void UpdateStudentEmail(string email, string PersonId)
        {
            Encode en = new Encode();
            bool exists = false;
            string db_connection = en.GetDbConnection();
            string s = "SELECT ContactValue FROM tbl_Core_People_Contacts";
            s += " WHERE (PersonId ='" + PersonId + "' )";
            s += " AND (ContactType='4') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            exists = true;
                        }
                    }
                }
                if (exists)
                {
                    s = "UPDATE tbl_Core_People_Contacts SET ContactValue='" + email + "' ";
                    s += " WHERE (ContactType='4') AND (PersonId ='" + PersonId + "' )";
                    en.ExecuteSQL(s);
                }
                else
                {
                    s = "INSERT INTO tbl_Core_People_Contacts ";
                    s += " (PersonId, ContactType, ContactValue, Version ) ";
                    s += "  VALUES ( '" + PersonId + "', '4' , '" + email + "' , '50' )";
                    en.ExecuteSQL(s);
                }
            }


        }
        public void WriteToLogFile(string logfile, string text)
        {

            System.IO.StreamWriter st1 = new System.IO.StreamWriter(logfile, true);
            st1.WriteLine(text); st1.Close();
        }

        public bool ValidateGoogeLogin(string login, out List<Claim> claims)
        {

            Is_staff = false;
            string staff_code = ""; string staff_id = "";
            string person_Id = "";
            Is_student = false;
            string student_id = "";
            string student_adno = "";
            bool PhysicsCanEdit = false;
            bool Is_SchoolOfficial = false;
            bool ExamsUser = false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT StaffId,StaffPersonId,StaffCode FROM dbo.tbl_Core_Staff ";
            s += " WHERE (GoogleAppsLogin ='" + login + "') ";

            string commandText = "SELECT StaffId,StaffPersonId,StaffCode FROM dbo.tbl_Core_Staff WHERE GoogleAppsLogin = @ID;";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(commandText, cn))
                {
                    s = "";
                    cm.Parameters.Add("@ID", System.Data.SqlDbType.VarChar);
                    cm.Parameters["@ID"].Value = login;
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            staff_id = dr.GetGuid(0).ToString();
                            staff_code = dr.GetString(2);
                            person_Id = dr.GetGuid(1).ToString();
                            Is_staff = true;

                        }
                    }
                }
            }
            //if (!Is_staff)
            {
                s = "SELECT StudentId,StudentPersonId,StudentAdmissionNumber FROM dbo.tbl_Core_Students ";
                s += " WHERE (GoogleAppsLogin ='" + login + "') ";
                commandText = "SELECT StudentId,StudentPersonId,StudentAdmissionNumber FROM dbo.tbl_Core_Students WHERE GoogleAppsLogin = @ID;";
                try
                {
                    using (SqlConnection cn = new SqlConnection(db_connection))
                    {
                        cn.Open();
                        using (SqlCommand cm = new SqlCommand(commandText, cn))
                        {
                            cm.Parameters.Add("@ID", System.Data.SqlDbType.VarChar);
                            cm.Parameters["@ID"].Value = login;
                            using (SqlDataReader dr = cm.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    student_id = dr.GetGuid(0).ToString();
                                    person_Id = dr.GetGuid(1).ToString();
                                    student_adno = dr.GetInt32(2).ToString();
                                    Is_student = true;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
            if (Is_staff)
            {
                try
                {
                    PhysicsCanEdit = new Cerval_Configuration("StaffIntranet_PhysicsCanEdit").Value.ToUpper().Contains(staff_code.Trim().ToUpper());
                }
                catch { }
            }
            if (Is_staff)
            {
                try
                {
                    ExamsUser = new Cerval_Configuration("StaffIntranet_ExamsUser").Value.ToUpper().Contains(staff_code.Trim().ToUpper());
                }
                catch { }
            }
            if (Is_student)
            {
                try
                {
                    Is_SchoolOfficial = new Cerval_Configuration("StudentInformation_SchoolOfficials").Value.Contains(login);
                }
                catch { }
            }

#if DEBUG
            //emulate Ben
            //Is_SchoolOfficial = true; Is_student = true; Is_staff = false; student_adno = "6367"; student_id = "9ef73606-10cf-49d5-9b04-26e5681a06bc";
#endif
            //if (person_Id.ToString() == "20744211-d0f0-4e69-af84-020c1023dfda")//CC
            //  {
            //      Is_SchoolOfficial = true; Is_student = true; Is_staff = true; student_adno = "6367"; student_id = "9ef73606-10cf-49d5-9b04-26e5681a06bc";
            //   }
            claims = new List<Claim>();
            Claim t1 = new Claim("staff_code", staff_code); claims.Add(t1);
            Claim t2 = new Claim("staff_id", staff_id.ToString()); claims.Add(t2);
            Claim t3 = new Claim("person_id", person_Id.ToString()); claims.Add(t3);
            Claim t4 = new Claim("student_id", student_id.ToString()); claims.Add(t4);
            Claim t5 = new Claim("student_adno", student_adno.ToString()); claims.Add(t5);
            Claim t6 = new Claim("is_staff", Is_staff.ToString()); claims.Add(t6);
            Claim t7 = new Claim("is_student", Is_student.ToString()); claims.Add(t7);
            Claim t8 = new Claim("PhysicsCanEdit", PhysicsCanEdit.ToString()); claims.Add(t8);
            Claim t9 = new Claim("SchoolOfficial", Is_SchoolOfficial.ToString()); claims.Add(t9);
            Claim t10 = new Claim("ExamsUser", ExamsUser.ToString()); claims.Add(t10);
            return Is_staff || Is_student;
        }

        public string GetsStaffCodefromRequest(HttpRequest Request, out Guid StaffId)
        {
            string staff_code = ""; StaffId = Guid.Empty;
            foreach (Claim c2 in Request.GetOwinContext().Authentication.User.Claims)
            {
                if (c2.Type == "staff_code") staff_code = c2.Value;
                if (c2.Type == "staff_id") StaffId = new Guid(c2.Value);
            }
            return staff_code;
        }

        public string GetsStaffCodefromRequest(HttpRequest Request)
        {
            string staff_code = "";
            foreach (Claim c in Request.GetOwinContext().Authentication.User.Claims)
            {
                if (c.Type == "staff_code") staff_code = c.Value;
            }
            return staff_code;
        }

        public Guid GetsStaffIdfromRequest(HttpRequest Request)
        {
            string s = "";
            foreach (Claim c in Request.GetOwinContext().Authentication.User.Claims)
            {
                if (c.Type == "staff_id") s = c.Value;
            }
            return new Guid(s);
        }

        public Guid GetPersonIdfromRequest(HttpRequest Request)
        {
            string s = "";
            Is_student = false; Is_staff = false;
            foreach (Claim c in Request.GetOwinContext().Authentication.User.Claims)
            {
                if (c.Type == "person_id") s = c.Value;
                if (c.Type == "is_student") Is_student = (c.Value.ToUpper() == "TRUE") ? true : false;
                if (c.Type == "is_staff") Is_staff = (c.Value.ToUpper() == "TRUE") ? true : false;
            }
            return new Guid(s);
        }

        public string GetStaffCodefromContextX(System.Web.HttpContext context)
        {
            //depricated May 2016
            string s = "";
            Guid personID = GetPersonIDX(context.User.Identity.Name, out s);
#if DEBUG
            s = "CC";
#else
#endif
            return s;
        }

        public Guid GetStaffIDfromContextX(System.Web.HttpContext context)
        {
            //depricated May 2016
#if DEBUG
            return new Guid("afe5e91c-9c7c-4bb9-bd5c-b37d552a2254");//cc
#else
#endif

            Guid StaffId = new Guid(); StaffId = Guid.Empty;
            string user_name = context.User.Identity.Name;
            if (user_name.IndexOf("\\") > 0)
            {
                string dom = user_name.Split('\\')[0].ToUpper();
                string usr = user_name.Split('\\')[1].ToLower();
                Encode en = new Encode();
                string db_connection = en.GetDbConnection();
                string s = "SELECT StaffId FROM dbo.tbl_Core_Staff ";
                s += " WHERE (StaffNTDomain ='" + dom + "') AND ";
                s += "( StaffNTUsername ='" + usr + "' ) ";
                using (SqlConnection cn = new SqlConnection(db_connection))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(s, cn))
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                StaffId = dr.GetGuid(0);
                                Is_staff = true;
                            }
                        }
                    }
                }
            }
            return StaffId;
        }

        public bool CheckStaffInConfigGroup(string staffcode, string key)
        {
            //try
            {
                Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_" + key);
                string s1 = c.Value;
                if (!c.valid)//try revert to config file
                {
                    System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
                    s1 = ar.GetValue(key, typeof(string)).ToString();
                }

                //now split staff list into xx,vv,
                char[] separators = new char[1]; separators[0] = ',';
                string[] splits = new string[30];
                splits = s1.Split(separators);
                foreach (string s in splits)
                {
                    if (s.Trim().ToLower() == staffcode.Trim().ToLower()) return true;
                }
            }
            //catch
            //{
            //    return false;
            //}
            return false;
        }
        public bool CheckStaffInConfigGroup(System.Web.HttpContext context, string key)
        {
            //string s = GetStaffCodefromContext(context);
            string s = GetsStaffCodefromRequest(context.Request);
            return CheckStaffInConfigGroup(s, key);
        }

        private class Staff_Number
        {
            public string staff1 = "";
            public int n = 0;
        }

        public string GetTutorForStudent(Guid StudentId, DateTime date)
        {
            PupilPeriodList ppl1 = new PupilPeriodList();
            ppl1.LoadList("StudentID", StudentId.ToString(), true, DateTime.Now);
            //problem of multiple tutors......
            List<Staff_Number> list1 = new List<Staff_Number>();
            bool found = false;
            foreach (ScheduledPeriod p in ppl1)
            {
                if (p.m_groupcode.ToUpper().Contains("RG"))
                {
                    //do we have it?
                    found = false;
                    foreach (Staff_Number sn in list1)
                    {
                        if (sn.staff1 == p.m_staffcode)
                        {
                            sn.n++; found = true;
                        }
                    }
                    if (!found)
                    {
                        Staff_Number sf1 = new Staff_Number();
                        sf1.staff1 = p.m_staffcode;
                        sf1.n = 1;
                        list1.Add(sf1);
                    }

                }

            }
            //find biggest.....
            if (list1.Count == 0) return ("");
            int maxn = 0; string s = "";
            foreach (Staff_Number sn in list1)
            {
                if (sn.n > maxn) { s = sn.staff1; maxn = sn.n; }
            }
            return (s);

        }
        public Guid GetStudentId(Guid PersonId)
        {
            Encode en = new Encode();
            Guid Id = new Guid(); Id = Guid.Empty;
            string db_connection = en.GetDbConnection();
            string s = "SELECT StudentId FROM dbo.tbl_Core_Students ";
            s += " WHERE (StudentPersonId='" + PersonId.ToString() + "')";
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
                            Is_student = true;
                        }
                    }
                }
            }
            return Id;
        }
        public Guid Get_StudentID(string db_field, string db_value)
        {
            Guid g = new Guid();
            g = Guid.Empty;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  StudentId   FROM  tbl_Core_Students  ";
            s += " WHERE (" + db_field + " ='" + db_value + "') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            g = dr.GetGuid(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return g;
        }

        public Guid GetStudentIDfromiSAMS(string iSAMSSchoolId)
        {
            Guid g1 = new Guid(); g1 = Guid.Empty;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  StudentId  FROM  tbl_Core_Students  ";
            s += " WHERE (iSAMStxtSchoolID  ='" + iSAMSSchoolId + "') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            g1 = dr.GetGuid(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return g1;
        }

        public string GetStudentIsamsID(string db_field, string db_value)
        {
            string s1 = "";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT  iSAMStxtSchoolID   FROM  tbl_Core_Students  ";
            s += " WHERE (" + db_field + " ='" + db_value + "') ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0)) s1 = dr.GetString(0); else s1 = "";
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return s1;
        }

        public string CleanInvertedCommas(string s)
        {
            if (s == null) return "";
            if (s.Length == 0) return "";
            int i = 0; i = s.IndexOf("'", i);
            while (i > 0) { i++; s = s.Substring(0, i) + "'" + s.Substring(i); i++; i = s.IndexOf("'", i); }
            return s;

        }
        public DateTime GetResultDate(string year, ref string Title, ref DateTime SetListDate)
        {
            DateTime ResultDate = new DateTime();
            switch (year)
            {
                case "7":
                    ResultDate = new DateTime(DateTime.Now.Year, 6, 1);
                    SetListDate = DateTime.Now;
                    Title = "Summer Exams";
                    break;
                case "8":
                    ResultDate = new DateTime(DateTime.Now.Year, 6, 1);
                    SetListDate = DateTime.Now;
                    Title = "Summer Exams";
                    break;
                case "9":
                    ResultDate = new DateTime(DateTime.Now.Year, 6, 1);
                    SetListDate = DateTime.Now.AddMonths(-2);
                    Title = "Summer Exams";
                    break;
                case "10":
                    ResultDate = new DateTime(DateTime.Now.Year, 6, 1);
                    SetListDate = DateTime.Now;
                    Title = "Summer Exams";
                    break;
                case "11":
                    ResultDate = new DateTime(DateTime.Now.Year, 2, 1);
                    SetListDate = DateTime.Now;
                    Title = "Mock Exams";
                    break;
                case "12":
                    switch (DateTime.Now.Month)
                    {
                        case 10:
                        case 11:
                        case 12:
                            ResultDate = new DateTime(DateTime.Now.Year, 12, 1);
                            Title = "Autumn Tests";
                            break;
                        case 5:
                        case 6:
                        case 7:
                            ResultDate = new DateTime(DateTime.Now.Year, 6, 1);
                            Title = "Summer Exams";
                            break;
                        default:
                            ResultDate = new DateTime(DateTime.Now.Year, 6, 1);
                            Title = "Summer Exams";

                            break;
                    }
                    break;
                case "13":
                    ResultDate = new DateTime(DateTime.Now.Year, 3, 1);
                    SetListDate = DateTime.Now;
                    Title = "Mock Exams";
                    break;
                default: break;
            }
            return ResultDate;
        }

        public void SaveImage(Guid PersonId, string filename)
        {
            string s = " SELECT * FROM dbo.tbl_Core_PeopleImages ";
            s += " WHERE (PersonId = '" + PersonId.ToString() + "' ) ";
            s += " ORDER BY ImageDate DESC";
            try
            {
                Encode en = new Encode();
                FileStream fs1 = new FileStream(@"d:\\temp\\" + filename + ".jpg", FileMode.Create);
                string db_connection = en.GetDbConnection();
                using (SqlConnection cn = new SqlConnection(db_connection))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(s, cn))
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                System.Data.SqlTypes.SqlBinary b1 = new System.Data.SqlTypes.SqlBinary();
                                b1 = dr.GetSqlBinary(2);
                                byte[] b3 = new byte[b1.Length];
                                b3 = (byte[])b1;
                                MemoryStream ms1 = new MemoryStream(b3);
                                Bitmap myImage = new Bitmap(ms1);
                                myImage.Save(fs1, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            else
                            {

                            }
                            dr.Close();
                        }
                    }
                    fs1.Close();
                }
            }
            catch
            { }


        }


    }
    [Serializable]
    public class ValueAddedMethod
    {
        public Guid m_ValueAddedMethodID;
        public string m_ValeAddedDescription;
        public string m_ValueAddedShortName;
        public int m_ValueAddedBaseResultType;
        public int m_ValueAddedOutputResultType;
        public int m_ValueAddedCourseType;
        public bool m_Current;
        public bool m_valid;

        public ValueAddedMethod() { m_valid = false; }
        public ValueAddedMethod(Guid ValueAddedMethodId)
        {
            m_valid = false;
            Load1("WHERE (ValueAddedMethodID='" + ValueAddedMethodId.ToString() + "' )");
        }

        public void Hydrate(SqlDataReader dr)
        {
            m_ValueAddedMethodID = dr.GetGuid(0);
            m_ValeAddedDescription = dr.GetString(1);
            m_ValueAddedShortName = dr.GetString(2);
            m_ValueAddedBaseResultType = dr.GetInt32(3);
            m_ValueAddedOutputResultType = dr.GetInt32(4);
            m_ValueAddedCourseType = dr.GetInt32(5);
            m_Current = dr.GetBoolean(7);
            m_valid = true;

        }
        public void Load1(string query)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  tbl_Core_ValueAddedMethods ";
            s += query;
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read()){Hydrate(dr);}
                    }
                }
            }
        }
    }
    [Serializable]
    public class ValueAddedEquation
    {
        public Guid m_ValueAddedEquationID;
        public Guid m_CourseID;
        public Guid m_ValueAddedMethodID;
        public int m_order;
        public double m_coef0;
        public double m_coef1;
        public double m_coef2 = 0;
        public double m_coef3 = 0;
        public double m_coef4 = 0;
        public DateTime m_ValidUntil;
        public bool m_valid;

        public ValueAddedEquation()
        {
            m_ValueAddedEquationID = Guid.Empty; m_valid = false;
        }

        public ValueAddedEquation(Guid CourseID, Guid ValueAddedMethodID)
        {
            m_valid = false;
            Load1("WHERE (CourseID='" + CourseID.ToString() + "') AND (ValueAddedMethodID='" + ValueAddedMethodID.ToString() + "' )");
        }

        public void Save()
        {
            string s = "";
            if (m_ValueAddedEquationID == Guid.Empty)
            {
                s = "INSERT INTO tbl_Core_ValueAddedEquations (ValueAddedMethodID, CourseID, PolynomialOrder, Coefficient0, Coefficient1, Coefficient2, Coefficient3, Coefficient4, ";
                if (m_ValidUntil != null) s += " ValidforOutputUntil, ";
                s += " Version )";
                s += " VALUES ( '" + m_ValueAddedMethodID.ToString() + "', '";
                s += m_CourseID.ToString() + "', '";
                s += m_order.ToString() + "', '";
                s += m_coef0.ToString() + "',  '"; s += m_coef1.ToString() + "',  '"; s += m_coef2.ToString() + "',  '"; s += m_coef3.ToString() + "',  '";
                s += m_coef4.ToString() + "',";
                if (m_ValidUntil != null) s += " CONVERT(DATETIME, '" + m_ValidUntil.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s += " '1' )";
            }
            else
            {
                //update...
                s = "UPDATE tbl_Core_ValueAddedEquations  ";
                s += " SET ValueAddedMethodID = '" + m_ValueAddedMethodID.ToString() + "' ";
                s += ", CourseID = '" + m_CourseID.ToString() + "' ";
                s += ", PolynomialOrder = '" + m_order.ToString() + "' ";
                s += ", Coefficient0 = '" + m_coef0.ToString() + "' ";
                s += ", Coefficient1 = '" + m_coef1.ToString() + "' ";
                s += ", Coefficient2 = '" + m_coef2.ToString() + "' ";
                s += ", Coefficient3 = '" + m_coef3.ToString() + "' ";
                s += ", Coefficient4 = '" + m_coef4.ToString() + "' ";
                s += ", ValidforOutputUntil = CONVERT(DATETIME, '" + m_ValidUntil.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += " WHERE (ValueAddedEquationID = '" + m_ValueAddedEquationID.ToString() + "' ) ";

            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }
        public void Load1(string query)
        {
            Encode en = new Encode(); m_valid = false;
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  tbl_Core_ValueAddedEquations ";
            s += query;
            s += " ORDER BY ValidforOutputUntil DESC "; //newest first...
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {

                        if (dr.Read()) Hydrate(dr);//only want the newest one...
                    }
                }
                cn.Close();
            }
        }

        public void Hydrate(SqlDataReader dr)
        {
            m_ValueAddedEquationID = dr.GetGuid(0);
            m_ValueAddedMethodID = dr.GetGuid(1);
            m_CourseID = dr.GetGuid(2);
            m_order = dr.GetInt32(3);
            m_coef0 = dr.GetDouble(4);
            m_coef1 = dr.GetDouble(5);
            if (!dr.IsDBNull(6)) m_coef2 = dr.GetDouble(6); else m_coef2 = 0;
            if (!dr.IsDBNull(7)) m_coef3 = dr.GetDouble(7); else m_coef3 = 0;
            if (!dr.IsDBNull(8)) m_coef4 = dr.GetDouble(8); else m_coef4 = 0;
            if (!dr.IsDBNull(9)) { m_ValidUntil = dr.GetDateTime(9); }
            m_valid = true;
        }


    }
    public class ValueAddedEquationList
    {
        public ArrayList _ValueAddedEquationList = new ArrayList();
        public ValueAddedEquationList(Guid CourseID, Guid ValueAddedMethodID)
        {
            _ValueAddedEquationList.Clear();
            LoadList("WHERE (CourseID='" + CourseID.ToString() + "') AND (ValueAddedMethodID='" + ValueAddedMethodID.ToString() + "' )");
        }
        public ValueAddedEquationList(Guid ValueAddedMethodID)
        {
            _ValueAddedEquationList.Clear();
            LoadList("WHERE (ValueAddedMethodID='" + ValueAddedMethodID.ToString() + "' )");
        }

        public void LoadList(string query)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM  tbl_Core_ValueAddedEquations ";
            s += query;
            s += " ORDER BY ValidforOutputUntil DESC "; //newest first...
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ValueAddedEquation vae = new ValueAddedEquation();
                            _ValueAddedEquationList.Add(vae);
                            vae.Hydrate(dr);
                        }
                    }
                }
            }
        }

    }
    public class ValueAddedMethodList
    {
        public List<ValueAddedMethod> _ValueAddedMethodList = new List<ValueAddedMethod>();
        public ValueAddedMethodList()
        {
            _ValueAddedMethodList.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM   tbl_Core_ValueAddedMethods ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ValueAddedMethod v = new ValueAddedMethod();
                            v.Hydrate(dr);_ValueAddedMethodList.Add(v);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public ValueAddedMethod FindVAMethod(Guid StudentID, Guid CourseID, DateTime CourseStartDate)
        {
            Course cse1 = new Course(); cse1.Load(CourseID);
            bool NewYellis = false;
            int ResultType = 0;
            ResultsList rl1 = new ResultsList();
            rl1.LoadListSimple(@"WHERE (StudentId = '" + StudentID.ToString() + "')  AND  (ResultType = '36' ) ");
            if (rl1._results.Count > 0) { NewYellis = true; }
            if (!cse1.GetResultType(ref ResultType, CourseStartDate))
            {
                //throw new System.ArgumentException(CourseStartDate.ToString()+"   :  "+CourseID.ToString(), "original");
                ResultType = 44;
            }
            ValueAddedMethod vam = new ValueAddedMethod();
            foreach (ValueAddedMethod VA_method1 in _ValueAddedMethodList)
            {
                //going to use Yellis for KS4, CATs for KS3 and Alis for KS5..
                if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("YELLIS(") && (cse1.KeyStage == 4) && !NewYellis) { vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID); break; }
                if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("YELLIS_") && (cse1.KeyStage == 4) && NewYellis && (ResultType == 10)) { vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID); break; }
                if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("YELIS_NEW_") && (cse1.KeyStage == 4) && NewYellis && (ResultType == 44)) { vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID); break; }
                if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("AL") && (cse1.KeyStage == 5)) { vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID); break; }
                if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("CATS") && (cse1.KeyStage == 3)) { vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID); break; }
                if (VA_method1.m_ValueAddedShortName.ToUpper().Contains("AL") && (cse1.KeyStage == 0)) vam = new ValueAddedMethod(VA_method1.m_ValueAddedMethodID);//for past studemts
            }
            if (cse1.KeyStage == 3)
            {
                try
                {
                    Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_Target_KS3_VAMethod");
                    vam = new ValueAddedMethod(new Guid(c.Value));
                }
                catch { }
            }
            return vam;
        }

    }
    public class ValueAddedConversion
    {
        public Guid m_ConversionId;
        public int m_ResultType;
        public string m_ResultValue;
        public double m_ResultNumericValue;

        public ValueAddedConversion() { }
    }
    public class ValueAddedConversionList
    {
        public ArrayList m_ValueAddedConversionList = new ArrayList();
        public ValueAddedConversionList()
        {
            m_ValueAddedConversionList.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM   tbl_Core_ValueAddedConversions ORDER BY ResultNumericValue DESC ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ValueAddedConversion v = new ValueAddedConversion();
                            m_ValueAddedConversionList.Add(v);
                            v.m_ConversionId = dr.GetGuid(0);
                            v.m_ResultType = dr.GetInt32(1);
                            v.m_ResultValue = dr.GetString(2);
                            v.m_ResultNumericValue = dr.GetDouble(3);

                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

    }
    public class WarningNoticeRead
    {
        public Guid m_StudentId;
        public DateTime m_DateRead;
        public string m_Season;
        public string m_Year;
        public bool m_valid;

        public WarningNoticeRead()
        {
            m_valid = false;
        }

        public void Save()
        {
            Encode en = new Encode();
            string s = "INSERT INTO dbo.tbl_Exams_WarningNoticeRead ";
            s += " (StudentId, ExamYear, ExamSeason, DateNoticeRead,Version ) ";
            s += "  VALUES ( '" + m_StudentId.ToString() + "', '" + m_Year + "' , '" + m_Season + "' , ";
            s += "CONVERT(DATETIME, '" + m_DateRead.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
            s += " '1' )";
            en.ExecuteSQL(s);
        }


        public WarningNoticeRead(Guid StudentId, string Season, string Year, ref bool valid)
        {
            m_StudentId = StudentId; m_Season = Season; m_Year = Year;
            m_valid = false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT DateNoticeRead FROM dbo.tbl_Exams_WarningNoticeRead ";
            s += "WHERE (StudentId='" + StudentId.ToString() + "') AND (ExamYear = '" + Year + "') AND (ExamSeason = '" + Season + "' )";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            m_DateRead = dr.GetDateTime(0);
                            m_valid = true; valid = true;
                        }
                    }
                }
            }

        }
    }

    #region CateringBalance

    public class Balance
    {
        public Guid Id;
        public Guid PersonId;
        public string source;
        public int type;
        public string description;
        public decimal balance;
        public DateTime lastVerified;

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetGuid(0);
            PersonId = dr.GetGuid(1);
            source = dr.GetString(2);
            type = dr.GetInt32(3);
            description = dr.GetString(4);
            balance = dr.GetDecimal(5);
            lastVerified = dr.GetDateTime(6);
        }
    }

    public class BalanceList
    {
        public ArrayList _balances = new ArrayList();

        public BalanceList()
        {
            _balances.Clear();
        }

        public BalanceList(Guid PersonId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = "SELECT * FROM dbo.tbl_Core_People_Balances";
            s += " WHERE  (PersonId = '" + PersonId.ToString() + "' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Balance b = new Balance();
                            b.Hydrate(dr);
                            _balances.Add(b);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class LineItem
    {
        public int Quantity;
        public string description;
        public decimal cost;
        public DateTime date;
        public bool valid;

        public void Hydrate(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0)) { date = dr.GetDateTime(0); valid = true; } else valid = false; ;
            description = dr.GetString(1);
            cost = dr.GetDecimal(2);
            Quantity = dr.GetInt32(3);
        }
    }

    public class LineItemList
    {
        public ArrayList _balances = new ArrayList();

        public LineItemList()
        {
            _balances.Clear();
        }

        public LineItemList(string adno)
        {
            Encode en = new Encode();
            string db_connection = en.GetGladstoneDBConnection();
            string s = "SELECT dbo.core_invoice.date_opened, dbo.core_lineitem.product_description, dbo.core_lineitem.gross, dbo.core_lineitem.quantity  ";
            s += " FROM  dbo.core_invoice INNER JOIN ";
            s += " dbo.core_lineitem ON dbo.core_invoice.ID = dbo.core_lineitem.p_core_invoice ";
            s += " INNER JOIN  dbo.core_user ON dbo.core_invoice.p_customer_user = dbo.core_user.ID  ";
            s += " WHERE     (dbo.core_user.login = '" + adno + "') ";
            s += " ORDER BY dbo.core_invoice.date_opened DESC ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            LineItem b = new LineItem();
                            b.Hydrate(dr);
                            _balances.Add(b);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class BalanceX
    {
        public int Id;
        public int purse_type;
        public int user;
        public decimal balance;
        public DateTime lastVerified;

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            purse_type = dr.GetInt32(1);
            user = dr.GetInt32(2);
            balance = dr.GetDecimal(3);
        }
    }

    public class BalanceListX
    {
        public ArrayList _balances = new ArrayList();

        public BalanceListX()
        {
            _balances.Clear();
        }

        public BalanceListX(string adno)
        {
            Encode en = new Encode();
            string db_connection = en.GetGladstoneDBConnection();
            string s = "SELECT * FROM dbo.Core_Purse";
            s += " WHERE  (p_core_user = '5' ) ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            BalanceX b = new BalanceX();
                            b.Hydrate(dr);
                            _balances.Add(b);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    #endregion


    #region Examsstuff
    [Serializable]
    public class ExamBaseOption : IComparable
    {
        //missing from normal ExamOption object
        /*
        public Guid m_OptionID;
		public Guid m_SyllabusID;
		public Guid m_ExamBoardID;
		public string m_Season_code;
		public string m_year_Code;
		public string m_Syllabus_Title;
         */
        public string m_OptionEntryCode;
        public string m_Title;
        public string m_Qualification;
        public string m_Level;
        public string m_SyllabusCode;
        //syllabus_title
        public string m_MaximiumMark;
        public string m_Item;
        //public string m_SeriesIdentifier;
        public string m_Process;
        public string m_QCACode;
        public string m_QCANumber;
        public bool m_FeeValid;
        public int m_Fee;
        public string m_file_path;

        public ExamBaseOption()
        {
            m_FeeValid = false;
            m_Fee = 0;
            m_MaximiumMark = "-";
        }

        public int CompareTo(object obj)
        {
            if (obj is ExamBaseOption)
            {
                ExamBaseOption otherOption = (ExamBaseOption)obj;
                return this.ToString().CompareTo(otherOption.ToString());
            }
            else
            {
                throw new ArgumentException("Object is not an ExamBaseOption");
            }
        }

        public bool Load(string line, int JCQ_Version)
        {
            // line is the data line from a option file...
            string s = "";
            if (JCQ_Version > 14) return false;
            if (JCQ_Version == 11)
            {
                //versrion 11......!!!!
                m_OptionEntryCode = line.Substring(2, 6);
                m_SyllabusCode = line.Substring(8, 6);
                m_Qualification = line.Substring(14, 4);
                m_Level = line.Substring(18, 3);
                m_Item = line.Substring(21, 1);
                m_Process = line.Substring(22, 1);
                m_QCACode = line.Substring(23, 4);
                m_QCANumber = line.Substring(27, 8);
                m_Title = line.Substring(35, 36);
                if (line.Substring(71, 1) == "Y")
                {
                    m_FeeValid = true;
                    m_Fee = System.Convert.ToInt32(line.Substring(72, 5));
                }
                s = line.Substring(79, 1);
                if ((s == "U") || (s == "M")) m_MaximiumMark = line.Substring(84, 3);
            }
            if (JCQ_Version == 12)
            {
                //versrion 12
                m_OptionEntryCode = line.Substring(2, 6);
                m_SyllabusCode = line.Substring(8, 6);
                m_Item = line.Substring(21, 1);
                switch (m_Item)
                {
                    case "C":
                        m_Qualification = line.Substring(14, 4);
                        m_Level = line.Substring(18, 3);
                        break;
                    case "U":
                        m_Qualification = line.Substring(22, 4);
                        m_Level = line.Substring(26, 3);
                        break;
                    case "B":
                        //don't know what to do...... take one??
                        m_Qualification = line.Substring(14, 4);
                        m_Level = line.Substring(18, 3);
                        break;
                }

                m_Process = line.Substring(29, 1);
                m_QCACode = line.Substring(30, 4);
                m_QCANumber = line.Substring(34, 8);
                m_Title = line.Substring(42, 36);
                if (line.Substring(78, 1) == "Y")
                {
                    m_FeeValid = true;
                    m_Fee = System.Convert.ToInt32(line.Substring(79, 5));
                }
                s = line.Substring(92, 1);
                if ((s == "U") || (s == "M") || (s == "B") || (s == "C")) m_MaximiumMark = line.Substring(109, 3);
            }
            if (JCQ_Version == 13)
            {
                //version 13
                m_OptionEntryCode = line.Substring(2, 6);
                m_SyllabusCode = line.Substring(8, 6);
                m_Item = line.Substring(21, 1);
                switch (m_Item)
                {
                    case "C":
                        m_Qualification = line.Substring(14, 4);
                        m_Level = line.Substring(18, 3);
                        break;
                    case "U":
                        m_Qualification = line.Substring(22, 4);
                        m_Level = line.Substring(26, 3);
                        break;
                    case "B":
                        //don't know what to do...... take one??
                        m_Qualification = line.Substring(14, 4);
                        m_Level = line.Substring(18, 3);
                        break;
                }

                m_Process = line.Substring(29, 1);
                m_QCACode = line.Substring(30, 4);
                m_QCANumber = line.Substring(34, 8);
                m_Title = line.Substring(42, 36);
                if (line.Substring(78, 1) == "Y")
                {
                    m_FeeValid = true;
                    m_Fee = System.Convert.ToInt32(line.Substring(79, 5));
                }
                s = line.Substring(92, 1);
                if ((s == "U") || (s == "M") || (s == "B") || (s == "C")) m_MaximiumMark = line.Substring(109, 4);
            }

            if (JCQ_Version == 14)
            {
                //version 14
                m_OptionEntryCode = line.Substring(2, 6);
                m_SyllabusCode = line.Substring(8, 6);
                m_Item = line.Substring(21, 1);
                switch (m_Item)
                {
                    case "C":
                        m_Qualification = line.Substring(14, 4);
                        m_Level = line.Substring(18, 3);
                        break;
                    case "U":
                        m_Qualification = line.Substring(22, 4);
                        m_Level = line.Substring(26, 3);
                        break;
                    case "B":
                        //don't know what to do...... take one??
                        m_Qualification = line.Substring(14, 4);
                        m_Level = line.Substring(18, 3);
                        break;
                }

                m_Process = line.Substring(29, 1);
                m_QCACode = line.Substring(30, 4);
                m_QCANumber = line.Substring(34, 8);
                m_Title = line.Substring(42, 36);
                if (line.Substring(78, 1) == "Y")
                {
                    m_FeeValid = true;
                    m_Fee = System.Convert.ToInt32(line.Substring(79, 5));
                }
                s = line.Substring(92, 1);
                if ((s == "U") || (s == "M") || (s == "B") || (s == "C")) m_MaximiumMark = line.Substring(109, 4);
            }
            return true;
        }

        public override string ToString()
        {
            return m_OptionEntryCode + " : " + m_Title;
        }
    }
    public class Exam_Board
    {
        public Guid m_ExamBoardOrganisationId;
        public Guid m_ExamBoardId;
        public string m_LegacyExamBdId;
        public string m_OrganisationName;
        public string m_OrganisationFriendlyName;
        public bool m_valid = false;

        public override string ToString()
        {
            return m_OrganisationName;
        }

        public Exam_Board()
        {
            //normal default
        }
        public Exam_Board(string ExamBd)
        {
            //overloaded to allow for the upload code to work....
            string s = "SELECT     dbo.tbl_Core_ExamBoards.ExamBoardId, dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId, ";
            s += "dbo.tbl_Core_ExamBoards.LegacyExamBdId, dbo.tbl_Core_Organisations.OrganisationName, ";
            s += "dbo.tbl_Core_Organisations.OrganisationFriendlyName ";
            s += "FROM         dbo.tbl_Core_ExamBoards ";
            s += "INNER JOIN dbo.tbl_Core_Organisations ";
            s += "ON dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId = dbo.tbl_Core_Organisations.OrganisationId ";
            s += " WHERE     (LegacyExamBdId = '" + ExamBd + "')";

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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

        public Exam_Board(Guid ExamBdId)
        {
            //overloaded to allow for the upload code to work....
            string s = "SELECT     dbo.tbl_Core_ExamBoards.ExamBoardId, dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId, ";
            s += "dbo.tbl_Core_ExamBoards.LegacyExamBdId, dbo.tbl_Core_Organisations.OrganisationName, ";
            s += "dbo.tbl_Core_Organisations.OrganisationFriendlyName ";
            s += "FROM         dbo.tbl_Core_ExamBoards ";
            s += "INNER JOIN dbo.tbl_Core_Organisations ";
            s += "ON dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId = dbo.tbl_Core_Organisations.OrganisationId ";
            s += " WHERE     (tbl_Core_ExamBoards.ExamBoardId = '" + ExamBdId.ToString() + "')";

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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



        public void Load(string ExamBdFriendlyName)
        {
            //overloaded to allow for the upload code to work....
            string s = "SELECT     dbo.tbl_Core_ExamBoards.ExamBoardId, dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId, ";
            s += "dbo.tbl_Core_ExamBoards.LegacyExamBdId, dbo.tbl_Core_Organisations.OrganisationName, ";
            s += "dbo.tbl_Core_Organisations.OrganisationFriendlyName ";
            s += "FROM         dbo.tbl_Core_ExamBoards ";
            s += "INNER JOIN dbo.tbl_Core_Organisations ";
            s += "ON dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId = dbo.tbl_Core_Organisations.OrganisationId ";
            s += " WHERE     (dbo.tbl_Core_Organisations.OrganisationFriendlyName = '" + ExamBdFriendlyName + "')";

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
        public void Hydrate(SqlDataReader dr)
        {
            m_valid = true;
            if (!dr.IsDBNull(0)) m_ExamBoardId = dr.GetGuid(0); else m_valid = false;
            if (!dr.IsDBNull(1)) m_ExamBoardOrganisationId = dr.GetGuid(1);
            if (!dr.IsDBNull(2)) m_LegacyExamBdId = dr.GetString(2);
            if (!dr.IsDBNull(3)) m_OrganisationName = dr.GetString(3);
            if (!dr.IsDBNull(4)) m_OrganisationFriendlyName = dr.GetString(4);
        }
    }
    public class ExamBoardList
    {
        public ArrayList _ExamBoardList = new ArrayList();
        public ExamBoardList()
        {
            _ExamBoardList.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT     dbo.tbl_Core_ExamBoards.ExamBoardId, ";
            s += "dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId, ";
            s += "dbo.tbl_Core_ExamBoards.LegacyExamBdId, ";
            s += "dbo.tbl_Core_Organisations.OrganisationName, ";
            s += "dbo.tbl_Core_Organisations.OrganisationFriendlyName ";
            s += "FROM         dbo.tbl_Core_ExamBoards ";
            s += "INNER JOIN dbo.tbl_Core_Organisations ";
            s += "ON dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId = dbo.tbl_Core_Organisations.OrganisationId ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Exam_Board eb = new Exam_Board();
                            _ExamBoardList.Add(eb);
                            eb.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    [Serializable]
    public class ExamComponent : IComparable
    {
        public Guid m_ComponentID;
        public string m_ComponentCode;
        public string m_ComponentTitle;
        public Guid m_ExamBoardID;
        public string m_MaximumMark;
        public string m_Teachermarks;
        public string m_Due_Date;
        public string m_Timetabled;
        public DateTime m_TimetableDate;
        public string m_TimetableSession;
        public string m_Time;
        public string m_year;
        public string m_season;
        public bool m_valid;
        public Guid m_OptionID;//note not from sql... has to come from data file
        public string m_OptionTitle;//note not from sql... has to come from data file
        #region Icomparable
        public override string ToString()
        {
            return m_ComponentTitle + "(" + m_ComponentCode + ")";
        }
        public int CompareTo(object obj)
        {
            if (obj is ExamComponent)
            {
                ExamComponent other = (ExamComponent)obj;
                return this.ToString().CompareTo(other.ToString());
            }
            else
            {
                throw new ArgumentException("Object is not an ExamComponent");
            }
        }
        public bool EqualTo(ExamComponent ex2)
        {
            //if both are the same return true
            if (ex2.m_ComponentCode != m_ComponentCode) return false;
            if (ex2.m_ComponentID != m_ComponentID) return false;
            if (ex2.m_ComponentTitle != m_ComponentTitle) return false;
            if (ex2.m_ExamBoardID != m_ExamBoardID) return false;
            //if (ex2.m_Due_Date != m_Due_Date) return false;
            if (ex2.m_MaximumMark != m_MaximumMark) return false;
            if (ex2.m_season != m_season) return false;
            if (ex2.m_Teachermarks != m_Teachermarks) return false;
            //time is a string but represents int value
            try
            {
                if (System.Convert.ToInt16(ex2.m_Time) != System.Convert.ToInt16(m_Time)) return false;
            }
            catch
            {
                return false;
            }

            if (ex2.m_Timetabled != m_Timetabled) return false;
            if (ex2.m_TimetableDate != m_TimetableDate) return false;
            if (ex2.m_TimetableSession != m_TimetableSession) return false;
            if (ex2.m_year != m_year) return false;

            return true;
        }
        #endregion

        public ExamComponent()
        {
            m_valid = false;
        }
        public void Load(string Id)
        {
            Guid g = new Guid(Id);
            Load(g);
        }
        public void Load(Guid ComponentID)
        {
            string s;
            m_valid = false;
            s = "SELECT * FROM  dbo.tbl_Exams_Components  ";
            s += "WHERE  (ComponentID = '" + ComponentID.ToString() + "') ";

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read()) Hydrate(dr);
                        dr.Close();
                    }
                }
            }
            return;
        }
        public void Load(string ComponentCode, string Season, string Year)
        {
            string s;
            m_valid = false;
            s = "SELECT * FROM  dbo.tbl_Exams_Components  ";
            s += "WHERE  (ComponentCode = '" + ComponentCode + "') ";
            s += " AND (SeasonCode = '" + Season + "')";
            s += " AND (YearCode = '" + Year + "')";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read()) Hydrate(dr);
                        dr.Close();
                    }
                }
            }
            return;
        }
        public void Hydrate(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0))
            {
                m_valid = true;
                m_ComponentID = dr.GetGuid(0);
                m_ComponentCode = dr.GetString(1);
                m_ComponentTitle = dr.GetString(2);
                m_ExamBoardID = dr.GetGuid(3);
                m_year = dr.GetString(4);
                m_season = dr.GetString(5);
                if (!dr.IsDBNull(6)) m_Teachermarks = dr.GetString(6);
                if (!dr.IsDBNull(7)) m_MaximumMark = dr.GetInt32(7).ToString();
                if (!dr.IsDBNull(8)) m_Due_Date = dr.GetDateTime(8).ToString();
                if (!dr.IsDBNull(9)) m_Timetabled = dr.GetString(9);
                if (!dr.IsDBNull(10)) m_TimetableDate = dr.GetDateTime(10);
                if (!dr.IsDBNull(11)) m_TimetableSession = dr.GetString(11);
                if (!dr.IsDBNull(12)) m_Time = dr.GetInt32(12).ToString();
            }
        }
        public Guid Find_ComponentID(string ComponentCode, string Examboard, string season, string year)
        {
            string s;
            m_valid = false;
            Guid temp = new Guid();
            temp = Guid.Empty;
            s = "SELECT ComponentID FROM  dbo.tbl_Exams_Components  ";
            s += "WHERE  (ComponentCode = '" + ComponentCode + "') AND (ExamBoardID = '" + Examboard + "') ";
            s += "AND (YearCode = '" + year + "' ) ";
            s += "AND (SeasonCode = '" + season + "') ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0))
                            {
                                temp = dr.GetGuid(0);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            return temp;
        }
        public Guid Find_ComponentID(string ComponentCode, string ComponentTitle, string Examboard, string season, string year)
        {
            string s;
            m_valid = false;
            Guid temp = new Guid();
            temp = Guid.Empty;
            s = "SELECT ComponentID FROM  dbo.tbl_Exams_Components  ";
            s += "WHERE  (ComponentCode = '" + ComponentCode + "') AND (ExamBoardID = '" + Examboard + "') ";
            s += "AND (YearCode = '" + year + "' ) ";
            s += "AND (SeasonCode = '" + season + "') ";
            s += "AND (ComponentTitle = '" + ComponentTitle + "' ) ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0))
                            {
                                temp = dr.GetGuid(0);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            return temp;
        }
        public void Create()
        {
            Encode en = new Encode();
            String s = "";
            DateTime t1 = new DateTime();
            try
            {
                t1 = System.Convert.ToDateTime(m_Due_Date);
            }
            catch
            {
                m_Due_Date = null;//don't try to write it...
            }

            s = " INSERT INTO dbo.tbl_Exams_Components (ComponentCode, ComponentTitle, ";
            s += " ExamBoardID, YearCode, SeasonCode, TeacherMarks, MaximumMark, ";
            if (m_Due_Date != null) s += "DueDate, ";
            s += " Timetabled ";
            if (m_Timetabled == "T") s += ", TimetabledDate, TimetabledSession, TimeAllowed ";
            s += ") VALUES ('" + m_ComponentCode + "' , '" + m_ComponentTitle + "', '";
            s += m_ExamBoardID.ToString() + "', '" + m_year + "', '" + m_season + "', '" + m_Teachermarks + "', '";
            s += m_MaximumMark + "', ";
            if (m_Due_Date != null)
            {
                s += " CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102), ";
            }
            s += " '" + m_Timetabled + "' ";
            if (m_Timetabled == "T")
            {
                s += ", CONVERT(DATETIME,'" + m_TimetableDate.ToString("yyyy-MM-dd HH:mm:ss") + "',102), ";
                s += " '" + m_TimetableSession + "', '" + m_Time + "' ";
            }
            s += ") ";

            en.ExecuteSQL(s);

        }

        public Guid CreateNew()//overloaded for iSAMS sync
        {
            Encode en = new Encode();
            String s = "";
            DateTime t1 = new DateTime();
            try
            {
                t1 = System.Convert.ToDateTime(m_Due_Date);
            }
            catch
            {
                m_Due_Date = null;//don't try to write it...
            }

            Guid g1 = new Guid(); g1 = Guid.NewGuid();

            s = " INSERT INTO dbo.tbl_Exams_Components (ComponentID, ComponentCode, ComponentTitle, ";
            s += " ExamBoardID, YearCode, SeasonCode, TeacherMarks, MaximumMark, ";
            if (m_Due_Date != null) s += "DueDate, ";
            s += " Timetabled ";
            if (m_Timetabled == "T") s += ", TimetabledDate, TimetabledSession, TimeAllowed ";
            s += " ) VALUES ('" + g1.ToString() + "' , '" + m_ComponentCode + "' , '" + m_ComponentTitle + "', '";
            s += m_ExamBoardID.ToString() + "', '" + m_year + "', '" + m_season + "', '" + m_Teachermarks + "', '";
            s += m_MaximumMark + "', ";
            if (m_Due_Date != null)
            {
                s += " CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102), ";
            }
            s += " '" + m_Timetabled + "' ";
            if (m_Timetabled == "T")
            {
                s += ", CONVERT(DATETIME,'" + m_TimetableDate.ToString("yyyy-MM-dd HH:mm:ss") + "',102), ";
                s += " '" + m_TimetableSession + "', '" + m_Time + "' ";
            }
            s += " ) ";

            en.ExecuteSQL(s);
            return g1;
        }


        public void Save()
        {
            Encode en = new Encode();
            String s = "";
            if (m_ComponentID == Guid.Empty) Create();
            if (m_ComponentID == null) Create();
            s = " UPDATE dbo.tbl_Exams_Components SET ComponentCode= '" + m_ComponentCode + "' , ";
            s += " ComponentTitle= '" + m_ComponentTitle + "' , ";
            s += " ExamBoardID = '" + m_ExamBoardID.ToString() + "' , ";
            s += " YearCode = '" + m_year + "' , SeasonCode = '" + m_season + "' , TeacherMarks = '" + m_Teachermarks + "', MaximumMark = '" + m_MaximumMark + "', ";
            if (m_Due_Date != null)
            {
                DateTime t1 = new DateTime(); t1 = System.Convert.ToDateTime(m_Due_Date);
                s += "DueDate = CONVERT(DATETIME,'" + t1.ToString("yyyy-MM-dd HH:mm:ss") + "',102), ";
            }
            s += " Timetabled ='" + m_Timetabled + "'  ";
            if (m_Timetabled == "T")
            {
                s += ", TimetabledDate  = CONVERT(DATETIME,'" + m_TimetableDate.ToString("yyyy-MM-dd HH:mm:ss") + "',102), ";
                s += " TimetabledSession = '" + m_TimetableSession + "' , TimeAllowed ='" + m_Time + "'  ";

            }
            s += " WHERE (ComponentID='" + m_ComponentID.ToString() + "' )";
            en.ExecuteSQL(s);
        }
        public int Delete()
        {
            string s = "";
            Encode en = new Encode();
            s = "DELETE  FROM dbo.tbl_Exams_Link ";
            s += " WHERE (ComponentID='" + m_ComponentID.ToString() + "' )";
            int n = en.Execute_count_SQL(s);
            s = "DELETE  FROM dbo.tbl_Exams_Components ";
            s += " WHERE (ComponentID='" + m_ComponentID.ToString() + "' )";
            n = en.Execute_count_SQL(s);
            return n;
        }
        public bool LoadFromBaseData(string line, int JCQ_Version, string ExamBoardId)
        {
            DateTime DueDate = new DateTime();
            m_ExamBoardID = new Guid(ExamBoardId);
            if (JCQ_Version < 11) return false;
            if (JCQ_Version == 11)
            {
                m_ComponentTitle = line.Substring(14, 36);
                m_MaximumMark = line.Substring(51, 3);
                m_Teachermarks = line.Substring(50, 1);
                int y = 2007; int m = 1; int d = 1; m_Due_Date = null;
                if ((m_Teachermarks == "Y") || (m_Teachermarks == "G") || (m_Teachermarks == "E"))
                {
                    m_Due_Date = line.Substring(55, 6);//ddmmyy   ugh
                    y = 2000 + System.Convert.ToInt32(m_Due_Date.Substring(4, 2));
                    m = System.Convert.ToInt32(m_Due_Date.Substring(2, 2));
                    d = System.Convert.ToInt32(m_Due_Date.Substring(0, 2));
                }
                try
                {
                    DueDate = new DateTime(y, m, d);
                }
                catch (Exception e)
                {
                    string s_error = e.Message;
                    //assume the date from the file is not a valid date (000000 most likely)
                }
                m_Timetabled = line.Substring(61, 1); y = 2007; m = 1; d = 1;
                if (m_Timetabled == "T")
                {
                    string s = line.Substring(62, 6);
                    y = 2000 + System.Convert.ToInt32(s.Substring(4, 2));
                    m = System.Convert.ToInt32(s.Substring(2, 2));
                    d = System.Convert.ToInt32(s.Substring(0, 2));
                    m_TimetableSession = line.Substring(68, 1);
                    m_Time = line.Substring(69, 3);
                }
                try
                {
                    m_TimetableDate = new DateTime(y, m, d);
                }
                catch (Exception e)
                {
                    string s_error = e.Message;
                    //assume timetabled time not valid
                }
                return true;
            }
            if ((JCQ_Version == 12) || (JCQ_Version == 13) || (JCQ_Version == 14))
            {
                m_ComponentCode = line.Substring(2, 12);
                m_ComponentTitle = line.Substring(14, 36);
                m_MaximumMark = line.Substring(51, 3);
                m_Teachermarks = line.Substring(50, 1);
                int y = 2007; int m = 1; int d = 1;
                if ((m_Teachermarks == "Y") || (m_Teachermarks == "G") || (m_Teachermarks == "E"))
                {
                    m_Due_Date = line.Substring(58, 6);//ddmmyy   ugh
                    y = 2000 + System.Convert.ToInt32(m_Due_Date.Substring(4, 2));
                    m = System.Convert.ToInt32(m_Due_Date.Substring(2, 2));
                    d = System.Convert.ToInt32(m_Due_Date.Substring(0, 2));
                }
                try
                {
                    DueDate = new DateTime(y, m, d);
                    m_Due_Date = DueDate.ToShortDateString();
                }
                catch (Exception e)
                {
                    string s_error = e.Message;
                    m_Due_Date = null;
                    //assume the date from the file is not a valid date (000000 most likely)
                }
                m_Timetabled = line.Substring(64, 1); y = 2007; m = 1; d = 1;
                if (m_Timetabled == "T")
                {
                    string s = line.Substring(65, 6);
                    y = 2000 + System.Convert.ToInt32(s.Substring(4, 2));
                    m = System.Convert.ToInt32(s.Substring(2, 2));
                    d = System.Convert.ToInt32(s.Substring(0, 2));
                    m_TimetableSession = line.Substring(71, 1);
                    m_Time = line.Substring(72, 3);
                }
                try
                {
                    m_TimetableDate = new DateTime(y, m, d);
                }
                catch (Exception e)
                {
                    string s_error = e.Message;
                    //assume timetabled time not valid
                }
                return true;
            }
            return false;
        }
    }

    public class ExamCompononent_List
    {
        public ArrayList m_list = new ArrayList();
        public int m_count;


        public ExamCompononent_List()
        {
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_count = 0; m_list.Clear();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamComponent p = new ExamComponent();
                            m_list.Add(p);
                            p.Hydrate(dr);
                            m_count++;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Load(Guid OptionID)
        {
            //get all components for this option
            string s = " SELECT  dbo.tbl_Exams_Components.*  FROM dbo.tbl_Exams_Components ";
            s += " INNER JOIN dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID ";
            s += " INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Options.OptionID  ";
            s += " WHERE (dbo.tbl_Exams_Options.OptionID = '" + OptionID.ToString() + "' )";
            Load1(s);
        }

        public void LoadAllComponents_IncNonTimetabled(string yearcode, string season)
        {   //loads all components where there is an entry,..... (note yearcode = 10 etc
            string s = " SELECT DISTINCT dbo.tbl_Exams_Components.* ,dbo.tbl_Exams_Components.TimetabledDate  ";

            s += " FROM         dbo.tbl_Exams_Components  ";
            s += " WHERE (dbo.tbl_Exams_Components.YearCode = '" + yearcode + "') AND (dbo.tbl_Exams_Components.SeasonCode = '" + season + "') ";
            Load1(s);
        }

        public void LoadAllComponents(string year, string season)
        {   //loads all components where there is an entry,..... (note year =2009....
            string s = " SELECT DISTINCT dbo.tbl_Exams_Components.* ,dbo.tbl_Exams_Components.TimetabledDate  ";
            //s+=" dbo.tbl_Exams_Components.ComponentTitle, dbo.tbl_Exams_Components.TimetabledDate, dbo.tbl_Exams_Components.TimetabledSession, dbo.tbl_Exams_Components.TimeAllowed ";
            s += " FROM         dbo.tbl_Exams_Entries INNER JOIN ";
            s += " dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN ";
            s += " dbo.tbl_Exams_Link ON dbo.tbl_Exams_Options.OptionID = dbo.tbl_Exams_Link.OptionID INNER JOIN ";
            s += " dbo.tbl_Exams_Components ON dbo.tbl_Exams_Link.ComponentID = dbo.tbl_Exams_Components.ComponentID ";
            s += " WHERE (dbo.tbl_Exams_Entries.ExamYear = '" + year + "') AND (dbo.tbl_Exams_Entries.ExamSeason = '" + season + "') AND (dbo.tbl_Exams_Components.Timetabled = 'T') ";
            s += " ORDER BY dbo.tbl_Exams_Components.TimetabledDate  ";
            Load1(s);
        }

        public void LoadAllComponents_NotScheduled(string year, string season)
        {
            string s = "SELECT dbo.tbl_Exams_Components.* FROM dbo.tbl_Exams_Components  ";
            s += "INNER JOIN dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID  ";
            s += "INNER JOIN dbo.tbl_Exams_Entries ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Entries.OptionID ";
            s += "WHERE (dbo.tbl_Exams_Entries.ExamYear = '" + year + "' ) AND (dbo.tbl_Exams_Entries.ExamSeason = '" + season + "' ) AND (dbo.tbl_Exams_Components.Timetabled = 'T')  ";
            s += "EXCEPT  SELECT dbo.tbl_Exams_Components.* FROM dbo.tbl_Exams_Components ";
            s += "INNER JOIN dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID  ";
            s += "INNER JOIN dbo.tbl_Exams_Entries ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Entries.OptionID  ";
            s += "INNER JOIN dbo.tbl_Exams_ScheduledComponents ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_ScheduledComponents.ComponentId AND dbo.tbl_Exams_Entries.StudentID = dbo.tbl_Exams_ScheduledComponents.StudentId ";
            s += "WHERE (dbo.tbl_Exams_Entries.ExamYear = '" + year + "' ) AND (dbo.tbl_Exams_Entries.ExamSeason = '" + season + "' ) AND (dbo.tbl_Exams_Components.Timetabled = 'T')";
            Load1(s);
        }

        public void LoadAllComponentsSeason(string yearcode, string season)
        {
            string s = "SELECT dbo.tbl_Exams_Components.* FROM dbo.tbl_Exams_Components  ";
            s += "  WHERE  (YearCode = '" + yearcode + "') AND(SeasonCode = '" + season + "') AND(Timetabled = 'T') ";
            s += " ORDER BY TimetabledDate  ";
            Load1(s);
        }

        public void LoadAllComponentsSeasonDate(string yearcode, string season, DateTime start, DateTime end)
        {
            //make start at start of day and end at end in case
            start = new DateTime(start.Year, start.Month, start.Day); start = start.AddHours(-1);
            end = new DateTime(end.Year, end.Month, end.Day); end = end.AddHours(20);
            string s1 = " CONVERT(DATETIME, '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s2 = " CONVERT(DATETIME, '" + end.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = "SELECT dbo.tbl_Exams_Components.* FROM dbo.tbl_Exams_Components  ";
            s += "  WHERE  (YearCode = '" + yearcode + "') AND (SeasonCode = '" + season + "') AND (Timetabled = 'T') ";
            s += " AND (TimetabledDate >" + s1 + " ) AND (TimetabledDate <" + s2 + "  ) ";
            s += " ORDER BY TimetabledDate  ";
            Load1(s);
        }



    }
    [DataContract]
    public class ExamComponentResult
    {
        [DataMember]public Guid ComponentResultId;
        [DataMember]public Guid StudentId;
        [DataMember] public Guid ComponentId;
        [DataMember] public Guid OptionId;
        [DataMember] public int ComponentScaledMark = 0;
        [DataMember] public int ComponentUMS = 0;
        [DataMember] public string ComponentStatus = "";
        [DataMember] public int version;
        [DataMember] public bool valid;

        public ExamComponentResult() { valid = false; ComponentResultId = Guid.Empty; }

        public void Load(Guid Id)
        {
            string s;
            s = " SELECT * FROM   dbo.tbl_Exams_ComponentResults ";
            s += " WHERE (ComponentResultId = '" + Id + "') ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection(); valid = false;
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
            }
        }

        public void Load(Guid studentId, Guid optionId, Guid componentId)
        {
            string s;
            s = " SELECT * FROM   dbo.tbl_Exams_ComponentResults ";
            s += " WHERE (ComponentId = '" + componentId.ToString() + "') ";
            s += "AND (StudentId ='" + studentId.ToString() + "')";
            s += "AND (OptionId ='" + optionId.ToString() + "')";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection(); valid = false;
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
            }

        }

        public void Hydrate(SqlDataReader dr)
        {
            ComponentResultId = dr.GetGuid(0); valid = true;
            ComponentId = dr.GetGuid(1);
            StudentId = dr.GetGuid(2);
            OptionId = dr.GetGuid(3);
            if (!dr.IsDBNull(4)) ComponentScaledMark = dr.GetInt32(4);
            if (!dr.IsDBNull(5)) ComponentUMS = dr.GetInt32(5);
            if (!dr.IsDBNull(6)) ComponentStatus = dr.GetString(6);
        }


        public void Save()
        {
            string s = "";
            if (ComponentResultId == Guid.Empty)
            {
                ComponentResultId = Guid.NewGuid();//so we know new id
                // make a new record..   but keep NULL fields NULL
                s = "INSERT INTO dbo.tbl_Exams_ComponentResults (ExamComponentId,StudentId, ComponentId, OptionId";
                if (ComponentScaledMark > 0) { s += ", ComponentScaledMark"; }
                if (ComponentUMS > 0) { s += ", ComponentUMS "; }
                if (ComponentStatus.Length > 0) { s += ", ComponentStatus "; }
                s += " , Version )";
                s += "VALUES ( '" + ComponentResultId.ToString() + "' ,";
                s += "'" + StudentId.ToString() + "' , '" + ComponentId.ToString() + "' , '" + OptionId.ToString() + "'";
                if (ComponentScaledMark > 0) { s += " ,  '" + ComponentScaledMark.ToString() + "'"; }
                if (ComponentUMS > 0) { s += " , '" + ComponentUMS.ToString() + "'"; }
                if (ComponentStatus.Length > 0) { s += " , '" + ComponentStatus + "'"; }
                s += " , '" + version + "' )";
            }
            else
            {
                s = " UPDATE[dbo].[tbl_Exams_ComponentResults] ";
                s += " SET[ComponentId] = '" + ComponentId.ToString() + "' ";
                s += ",[StudentId] = '" + StudentId.ToString() + "' ";
                s += ",[OptionId] = '" + OptionId.ToString() + "' ";
                if (ComponentScaledMark > 0) { s += ",[ComponentScaledMark] = '" + ComponentScaledMark.ToString() + "' "; }
                if (ComponentUMS > 0) { s += ",[ComponentUMS] = '" + ComponentUMS.ToString() + "' "; }
                if (ComponentStatus.Length > 0) { s += ",[ComponentStatus] = '" + ComponentStatus + "' "; }
                s += ",[Version]  = '" + version.ToString() + "' ";
                s += " WHERE ExamComponentId='" + ComponentResultId.ToString() + "'";

            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }
    }

    public class ExamComponentResultList
    {
        public List<ExamComponentResult> m_list = new List<ExamComponentResult>();
        public int m_count;

        public void Load_Student(Guid studentID)
        {
            m_count = 0; m_list.Clear();
            string s = " SELECT  * FROM  dbo.tbl_Exams_ComponentResults WHERE (StudentId = '";
            s += studentID.ToString() + "' )";
            Load1(s);
        }

        public void Load_Option(Guid OptionId)
        {
            m_count = 0; m_list.Clear();
            string s = " SELECT  * FROM  dbo.tbl_Exams_ComponentResults WHERE (OptionId = '";
            s += OptionId.ToString() + "' )";
            Load1(s);
        }
        public int Load_OptionStudent(Guid OptionId, Guid StudentId)  //returns number of results
        {
            m_count = 0; m_list.Clear();
            string s = " SELECT  * FROM  dbo.tbl_Exams_ComponentResults WHERE (OptionId = '";
            s += OptionId.ToString() + "' )  AND (StudentId = '" + StudentId + "' )";
            Load1(s);
            return m_count;
        }

        public int Load_OptionStudent(Guid optionId, Guid studentId, Guid componentId)  //returns number of results
        {
            m_count = 0; m_list.Clear();
            string s = " SELECT  * FROM  dbo.tbl_Exams_ComponentResults WHERE (OptionId = '";
            s += optionId.ToString() + "' )  AND (StudentId = '" + studentId + "' ) ";
            s += " AND (ComponentId='" + componentId.ToString() + "') ";
            Load1(s);
            return m_count;
        }

        private void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamComponentResult r = new ExamComponentResult();
                            m_list.Add(r);
                            r.Hydrate(dr);
                            m_count++;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    [DataContract]
    public class ExamComponentResultFull
    {
        [DataMember]
        public int AdmissionNumber;
        [DataMember]
        public string GivenName;
        [DataMember]
        public string Surname;
        [DataMember]
        public string OptionCode;
        [DataMember]
        public string OptionTitle;
        [DataMember]
        public string ComponentCode;
        [DataMember]
        public string ComponentTitle;
        [DataMember]
        public int ScaledMark = 0;
        [DataMember]
        public int MaxMark = 0;
        [DataMember]
        public bool valid = false;

        public void Hydrate(SqlDataReader dr)
        {
            AdmissionNumber = dr.GetInt32(0); valid = true;
            GivenName = dr.GetString(1);
            Surname = dr.GetString(2);
            OptionCode = dr.GetString(3);
            OptionTitle = dr.GetString(4);
            ComponentCode = dr.GetString(5);
            ComponentTitle = dr.GetString(6);
            if (!dr.IsDBNull(7)) ScaledMark = dr.GetInt32(7);
            if (!dr.IsDBNull(8)) MaxMark = dr.GetInt32(8);

        }
    }
    [DataContract]
    public class ExamComponentResultFullList
    {
        [DataMember]
        public List<ExamComponentResultFull> m_list = new List<ExamComponentResultFull>();

        public string GetQuery(string where)
        {
            string s = " SELECT DISTINCT  dbo.tbl_Core_Students.StudentAdmissionNumber, dbo.tbl_Core_People.PersonGivenName, dbo.tbl_Core_People.PersonSurname, dbo.tbl_Exams_Options.OptionCode, dbo.tbl_Exams_Options.OptionTitle, ";
            s += "     dbo.tbl_Exams_Components.ComponentCode, dbo.tbl_Exams_Components.ComponentTitle, dbo.tbl_Exams_ComponentResults.ComponentScaledMark, dbo.tbl_Exams_Components.MaximumMark ";
            s += " FROM   dbo.tbl_Exams_ComponentResults ";
            s += "   INNER JOIN dbo.tbl_Exams_Components ON dbo.tbl_Exams_ComponentResults.ComponentId = dbo.tbl_Exams_Components.ComponentID ";
            s += " INNER JOIN  dbo.tbl_Core_Students ON dbo.tbl_Exams_ComponentResults.StudentId = dbo.tbl_Core_Students.StudentId ";
            s += "  INNER JOIN dbo.tbl_Core_People ON dbo.tbl_Core_Students.StudentPersonId = dbo.tbl_Core_People.PersonId ";
            s += " INNER JOIN  dbo.tbl_Exams_Options ON dbo.tbl_Exams_ComponentResults.OptionID = dbo.tbl_Exams_Options.OptionID  ";
            s += where;
            return s;
        }


        public void LoadListStudent(int Adno)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_list.Clear();
            string s = GetQuery(" WHERE(dbo.tbl_Core_Students.StudentAdmissionNumber = '" + Adno.ToString() + "')");
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamComponentResultFull r = new ExamComponentResultFull();
                            m_list.Add(r);
                            r.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }

        public void LoadList(string year, string season)
        {
            //get all syllabusses....???
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_list.Clear();
            string s = GetQuery(" WHERE(dbo.tbl_Exams_Options.YearCode = '" + year + "') AND(dbo.tbl_Exams_Options.SeasonCode = '" + season + "')");
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamComponentResultFull r = new ExamComponentResultFull();
                            m_list.Add(r);
                            r.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }



    public class Exam_Entry
    {
        public Guid m_ExamEntryID;
        public Guid m_StudentID;
        public Guid m_OptionID;
        public Guid m_EntryFileID;
        public DateTime m_Date_Created;
        public DateTime m_Date_Entered;
        public bool m_withdrawn;
        public string m_season;
        public string m_year;
        public string m_PredictedGrade;
        public int m_EntryStatus;
        public bool m_valid;



        public Exam_Entry()
        {
            m_EntryFileID = Guid.Empty;
            m_PredictedGrade = "";
            m_EntryStatus = 0;
            m_valid = false;
        }

        public Guid Load(string Exam_EntryID)
        {
            m_ExamEntryID = Guid.Empty;
            string s;
            s = " SELECT * FROM   dbo.tbl_Exams_Entries ";
            s += " WHERE (ExamEntryID = '" + Exam_EntryID + "') ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_valid = false;
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
            }
            return m_ExamEntryID;
        }
        public Guid Load(Guid OptionId, Guid StudentId)
        {
            m_ExamEntryID = Guid.Empty;
            string s;
            s = " SELECT * FROM   dbo.tbl_Exams_Entries ";
            s += " WHERE (OptionID = '" + OptionId.ToString() + "') ";//unique to season
            s += " AND (StudentID = '" + StudentId.ToString() + "') ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_valid = false;
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
            }
            return m_ExamEntryID;
        }

        public Guid Load(string Option, string season, string year, Guid ExamBoardID, Guid StudentID)
        {
            //going to load from db...
            m_ExamEntryID = Guid.Empty;
            string s;
            s = " SELECT dbo.tbl_Exams_Entries.* ";
            s += " FROM   dbo.tbl_Exams_Syllabus ";
            s += " INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Syllabus.SyllabusID = dbo.tbl_Exams_Options.SyllabusID ";
            s += " INNER JOIN dbo.tbl_Core_ExamBoards ON dbo.tbl_Exams_Syllabus.ExamBoardID = dbo.tbl_Core_ExamBoards.ExamBoardId ";
            s += " INNER JOIN dbo.tbl_Exams_Entries ON dbo.tbl_Exams_Options.OptionID = dbo.tbl_Exams_Entries.OptionID ";
            s += " WHERE (dbo.tbl_Exams_Syllabus.ExamBoardID= '" + ExamBoardID.ToString() + "') ";
            s += " AND   (dbo.tbl_Exams_Options.SeasonCode= '" + season + "') ";
            s += " AND  (dbo.tbl_Exams_Options.YearCode= '" + year + "') ";
            s += " AND  (dbo.tbl_Exams_Options.OptionCode = '" + Option + "') ";
            s += " AND  (dbo.tbl_Exams_Entries.StudentID = '" + StudentID.ToString() + "') ";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_valid = false;
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
            }
            return m_ExamEntryID;
        }

        public void Hydrate(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0))
            {
                m_ExamEntryID = dr.GetGuid(0); m_valid = true;
            }
            if (!dr.IsDBNull(1)) m_StudentID = dr.GetGuid(1);
            if (!dr.IsDBNull(2)) m_OptionID = dr.GetGuid(2);
            m_Date_Created = dr.GetDateTime(3);
            if (!dr.IsDBNull(4)) m_Date_Entered = dr.GetDateTime(4);
            if (!dr.IsDBNull(5)) m_EntryFileID = dr.GetGuid(5);
            if (!dr.IsDBNull(6)) m_withdrawn = dr.GetBoolean(6); else m_withdrawn = false;
            m_year = dr.GetString(7);
            m_season = dr.GetString(8);
            if (!dr.IsDBNull(9)) m_PredictedGrade = dr.GetString(9);
            m_EntryStatus = 0;
            if (!dr.IsDBNull(10)) m_EntryStatus = dr.GetInt32(10);
        }

        public void Save()
        {
            string s = "";
            if (m_ExamEntryID == Guid.Empty)
            {
                m_ExamEntryID = new Guid();
                m_ExamEntryID = Guid.NewGuid();//so we know new id
                // make a new record..
                s = "INSERT INTO dbo.tbl_Exams_Entries (ExamEntryID,StudentID, OptionID, DateCreated, ExamYear, ExamSeason, EntryStatus, Version )";
                s += "VALUES ( '" + m_ExamEntryID.ToString() + "' ,";
                s += "'" + m_StudentID.ToString() + "' , '" + m_OptionID.ToString() + "' , CONVERT(DATETIME, '" + m_Date_Created.ToShortDateString() + "', 103) ,'" + m_year + "' , '" + m_season + "' , '" + m_EntryStatus + "' , '5' )";
            }
            else
            {
                s = "UPDATE dbo.tbl_Exams_Entries SET OptionID='" + m_OptionID.ToString() + "', DateCreated=CONVERT(DATETIME, '" + m_Date_Created.ToShortDateString() + "', 103) , ExamYear = '" + m_year + "' , ExamSeason = '" + m_season + "'  ";
                if (m_withdrawn) m_EntryStatus = 5;//should be OK.... but early code used the boolean...
                s += " , EntryStatus='" + m_EntryStatus.ToString() + "' ";
                if (m_withdrawn) s += " , Withdrawn='1' "; else s += " , Withdrawn='0' ";
                if (m_EntryFileID == Guid.Empty) s += ", EntryFileID = NULL  "; else s += ", EntryFileID ='" + m_EntryFileID.ToString() + "' ";
                if (m_PredictedGrade != "") s += ", PredictedGrade = '" + m_PredictedGrade + "' ";
                s += " WHERE (ExamEntryID ='" + m_ExamEntryID.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public void Delete()
        {
            if (m_ExamEntryID != Guid.Empty)
            {
                //need to check that we have no scheduled components for this.....
                ExamCompononent_List ec1 = new ExamCompononent_List();
                ScheduledComponent sch1 = new ScheduledComponent();
                ec1.Load(m_OptionID);
                foreach (ExamComponent ec in ec1.m_list)
                {
                    sch1.Load(ec.m_ComponentID, m_StudentID);
                    if (sch1.m_valid)
                    {
                        sch1.Delete();
                    }
                }
                string s = "DELETE FROM dbo.tbl_Exams_Entries  WHERE (ExamEntryID ='" + m_ExamEntryID.ToString() + "' )";
                Encode en = new Encode();
                en.ExecuteSQL(s);
            }
        }
        public void Withdraw()
        {
            if (m_ExamEntryID != Guid.Empty)
            {
                m_withdrawn = true;
                m_EntryFileID = Guid.Empty;
                m_EntryStatus = 5;
                Save();
                //now need to delete the scheduled components....
                ExamCompononent_List ec1 = new ExamCompononent_List();
                ScheduledComponent sch1 = new ScheduledComponent();
                ec1.Load(m_OptionID);
                foreach (ExamComponent ec in ec1.m_list)
                {
                    sch1.Load(ec.m_ComponentID, m_StudentID);
                    if (sch1.m_valid)
                    {
                        sch1.Delete();
                    }
                }
            }
        }

        public bool CanDelete()
        {
            if ((m_EntryFileID == Guid.Empty) && (!m_withdrawn)) return true;
            return false;
        }

        public bool NeedToSend()
        {
            if ((m_EntryFileID == Guid.Empty)) return true;
            return false;
        }
    }
    public class ExamEntries_List
    {
        public ArrayList m_list = new ArrayList();
        public int m_count;


        public ExamEntries_List()
        {
        }


        public void Load(Guid studentID)
        {
            m_count = 0; m_list.Clear();
            string s = " SELECT  * FROM  tbl_Exams_Entries WHERE (StudentID = '";
            s += studentID.ToString() + "' )";
            Load1(s);
        }

        public void Load_Option(Guid OptionId)
        {
            m_count = 0; m_list.Clear();
            string s = " SELECT  * FROM  tbl_Exams_Entries WHERE (OptionID = '";
            s += OptionId.ToString() + "' )";
            Load1(s);
        }

        public void Load(Guid studentID, string year, string season)
        {
            m_list.Clear(); m_count = 0;
            string s = " SELECT  * FROM  tbl_Exams_Entries WHERE (StudentID = '";
            s += studentID.ToString() + "' )";
            s += " AND (ExamSeason = '" + season + "' ) AND ( ExamYear = '" + year + "' )";
            Load1(s);
        }
        public void Load(Guid studentID, string year, string season, bool sent)
        {
            m_list.Clear(); m_count = 0;
            string s = " SELECT  * FROM  tbl_Exams_Entries WHERE (StudentID = '";
            s += studentID.ToString() + "' )";
            s += " AND (ExamSeason = '" + season + "' ) AND ( ExamYear = '" + year + "' )";
            s += " AND (DateEntered IS "; if (sent) s += "NOT "; s += " NULL)";
            Load1(s);
        }

        public void Load(Guid studentID, string year, string series, bool sent, string ExamBdeId)
        {
            m_list.Clear(); m_count = 0;
            string s = " SELECT   dbo.tbl_Exams_Entries.* FROM  tbl_Exams_Entries ";
            s += " INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID  ";
            s += " WHERE (dbo.tbl_Exams_Options.SeriesIdentifier = '" + series + "' ) AND ( ExamYear = '" + year + "' )";
            s += " AND (StudentID = '" + studentID.ToString() + "' )";
            s += " AND (EntryFileID IS "; if (sent) s += "NOT "; s += " NULL) ";
            s += " AND  (dbo.tbl_Exams_Syllabus.ExamBoardID = '" + ExamBdeId + "')";
            Load1(s);
        }

        public void LoadAllSeries(string year, string series, bool sent, string ExamBdeId)
        {
            m_list.Clear(); m_count = 0;
            string s = " SELECT   dbo.tbl_Exams_Entries.* FROM  tbl_Exams_Entries ";
            s += " INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID  ";
            s += " WHERE ( SeriesIdentifier = '" + series + "' ) AND ( ExamYear = '" + year + "' )";
            s += " AND (EntryFileID IS "; if (sent) s += "NOT "; s += " NULL) ";
            s += " AND  (dbo.tbl_Exams_Syllabus.ExamBoardID = '" + ExamBdeId + "')";
            s += " ORDER BY StudentID ";
            Load1(s);
        }

        public void LoadAllSeries(string year, string series, bool sent, string ExamBdeId, string OrderByField)
        {
            m_list.Clear(); m_count = 0;
            string s = " SELECT   dbo.tbl_Exams_Entries.* FROM  tbl_Exams_Entries ";
            s += " INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID  ";
            s += " WHERE (SeriesIdentifier = '" + series + "' ) AND ( ExamYear = '" + year + "' )";
            s += " AND (EntryFileID IS "; if (sent) s += "NOT "; s += " NULL) ";
            s += " AND  (dbo.tbl_Exams_Syllabus.ExamBoardID = '" + ExamBdeId + "')";
            s += " ORDER BY " + OrderByField;
            Load1(s);
        }

        public void Load_NotScheduled(string year, string season)
        {
            m_count = 0; m_list.Clear();
            string s = "SELECT dbo.tbl_Exams_Entries.* FROM dbo.tbl_Exams_Components";
            s += " INNER JOIN dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID ";
            s += " INNER JOIN dbo.tbl_Exams_Entries ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Entries.OptionID ";
            s += "WHERE (dbo.tbl_Exams_Entries.ExamYear = '" + year + "' ) AND (dbo.tbl_Exams_Entries.ExamSeason = '" + season + "' ) AND (dbo.tbl_Exams_Components.Timetabled = 't') ";
            s += " AND (dbo.tbl_Exams_Entries.Withdrawn = '0' ) ";
            s += " EXCEPT  ";

            s += "SELECT dbo.tbl_Exams_Entries.* FROM dbo.tbl_Exams_Components";
            s += " INNER JOIN dbo.tbl_Exams_Link ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_Link.ComponentID ";
            s += " INNER JOIN dbo.tbl_Exams_Entries ON dbo.tbl_Exams_Link.OptionID = dbo.tbl_Exams_Entries.OptionID ";
            s += " INNER JOIN dbo.tbl_Exams_ScheduledComponents ON dbo.tbl_Exams_Components.ComponentID = dbo.tbl_Exams_ScheduledComponents.ComponentId AND dbo.tbl_Exams_Entries.StudentID = dbo.tbl_Exams_ScheduledComponents.StudentId ";
            s += "WHERE (dbo.tbl_Exams_Entries.ExamYear = '" + year + "' ) AND (dbo.tbl_Exams_Entries.ExamSeason = '" + season + "' ) AND (dbo.tbl_Exams_Components.Timetabled = 't') ";
            s += " AND (dbo.tbl_Exams_Entries.Withdrawn = '0' ) ";
            Load1(s);
        }


        public void LoadAll(string year, string season)
        {
            m_list.Clear(); m_count = 0;
            string s = " SELECT   dbo.tbl_Exams_Entries.* FROM  tbl_Exams_Entries ";
            s += " INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID  ";
            s += " WHERE (ExamSeason = '" + season + "' ) AND ( ExamYear = '" + year + "' )";
            Load1(s);
        }

        public void LoadALL(string year, string season, bool sent)
        {
            m_list.Clear(); m_count = 0;
            string s = " SELECT   dbo.tbl_Exams_Entries.* FROM  tbl_Exams_Entries ";
            s += " INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID  ";
            s += " WHERE (ExamSeason = '" + season + "' ) AND ( ExamYear = '" + year + "' )";

            if (sent) s += "AND  (EntryStatus = '4' ) "; else s += "AND  (EntryStatus <> '4' ) ";
            Load1(s);
        }

        public void LoadAll_OptionOrder(string year, string season)
        {
            m_list.Clear(); m_count = 0;
            string s = " SELECT   dbo.tbl_Exams_Entries.* FROM  tbl_Exams_Entries ";
            s += " INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID  ";
            s += " INNER JOIN dbo.qry_Cerval_Core_Student ON dbo.qry_Cerval_Core_Student.StudentId=dbo.tbl_Exams_Entries.StudentId ";
            s += " WHERE (ExamSeason = '" + season + "' ) AND ( ExamYear = '" + year + "' )";
            s += " ORDER BY dbo.tbl_Exams_Options.OptionCode, dbo.qry_Cerval_Core_Student.PersonSurname, dbo.qry_Cerval_Core_Student.PersonGivenName ";
            Load1(s);
        }

        private void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Exam_Entry p = new Exam_Entry();
                            m_list.Add(p);
                            p.Hydrate(dr);
                            m_count++;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

    }
    public class Exam_File
    {
        public Guid m_EntryFileId;
        public string m_Fileame;
        public DateTime m_DateCreated;
        public DateTime m_DateSent;
        public bool m_DateSentValid;
        public DateTime m_DateAck;
        public bool m_DateAckValid;
        public int m_SequenceNo;
        public Guid m_ExamBoardID;

        public Exam_File()
        {
            m_DateAckValid = false;
            m_DateSentValid = false;
            m_EntryFileId = Guid.Empty;
        }

        public int FindSequencNumber(string ExamBoardID)
        {
            //search for last sequence no....
            int seq = 1;
            string s = "SELECT SequenceNumber FROM dbo.tbl_Exams_EDIFiles  WHERE (ExamBoardId = '" + ExamBoardID + "' )  ORDER BY SequenceNumber DESC ";
            Encode en = new Encode();
            string db = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            seq = dr.GetInt32(0);
                        }
                        dr.Close();
                    }
                }
            }
            return seq;
        }

        public void Load(string filename)
        {
            string s = "SELECT * FROM dbo.tbl_Exams_EDIFiles ";
            s += " WHERE (FileName ='" + filename + "' ) ";
            s += " ORDER BY DateCreated DESC ";
            Encode en = new Encode();
            string db = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            m_EntryFileId = dr.GetGuid(0);
                            m_Fileame = dr.GetString(1);
                            m_DateCreated = dr.GetDateTime(2);
                            if (dr.IsDBNull(3)) { m_DateSentValid = false; } else { m_DateSent = dr.GetDateTime(3); m_DateSentValid = true; };
                            if (dr.IsDBNull(4)) { m_DateAckValid = false; } else { m_DateAck = dr.GetDateTime(4); m_DateAckValid = true; };
                            m_SequenceNo = dr.GetInt32(5);
                            m_ExamBoardID = dr.GetGuid(6);
                        }
                        dr.Close();
                    }
                }
            }

        }


        public void Save()
        {
            string s = "";
            if (m_EntryFileId == Guid.Empty)
            {
                // make a new record..
                s = "INSERT INTO dbo.tbl_Exams_EDIFiles (FileName, DateCreated";
                if (m_DateSentValid) s += ", DateSent";
                if (m_DateAckValid) s += ", DateAcknowledged";
                s += ", SequenceNumber, ExamBoardId, Version )";
                s += "VALUES ( '" + m_Fileame + "' , ";
                s += "CONVERT(DATETIME, '" + m_DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                if (m_DateSentValid) s += "CONVERT(DATETIME, '" + m_DateSent.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                if (m_DateAckValid) s += "CONVERT(DATETIME, '" + m_DateAck.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s += "'" + m_SequenceNo.ToString() + "' , '" + m_ExamBoardID.ToString() + "' , '2' )";
            }
            else
            {
                s = "UPDATE dbo.tbl_Exams_EDIFiles SET FileName ='" + m_Fileame + "', DateCreated=CONVERT(DATETIME, '" + m_DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                if (m_DateSentValid) s += ", DateSent =CONVERT(DATETIME, '" + m_DateSent.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                if (m_DateAckValid) s += ", DateAcknowledged=CONVERT(DATETIME, '" + m_DateAck.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s += ", SequenceNumber = " + m_SequenceNo.ToString() + "' , ExamBoardId='" + m_ExamBoardID.ToString() + "'  Version='2' ";
                s += " WHERE (EntryFileID ='" + m_EntryFileId.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
            if (m_EntryFileId == Guid.Empty)
            {
                s = "SELECT EntryFileId FROM dbo.tbl_Exams_EDIFiles WHERE (FileName = '" + m_Fileame + "' ) AND ( DateCreated = CONVERT(DATETIME, '" + m_DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
                s = en.Execute_one_SQL(s);
                Guid g1 = new Guid(s);
                m_EntryFileId = g1;
            }

        }


    }
    public class Exam_Entry_Status
    {
        public int m_EntryStatusId;
        public string m_EntryStatusCode;
        public string m_EntryStatusDescription;

        public Exam_Entry_Status() { }

        public void Hydrate(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0)) m_EntryStatusId = dr.GetInt32(0);
            if (!dr.IsDBNull(1)) m_EntryStatusCode = dr.GetString(1);
            if (!dr.IsDBNull(2)) m_EntryStatusDescription = dr.GetString(2);
        }
    }
    public class Exam_Entry_Status_List
    {
        public List<Exam_Entry_Status> m_list = new List<Exam_Entry_Status>();
        public Exam_Entry_Status_List()
        {
            //get all values....???
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = " SELECT *  FROM tbl_List_ExamEntriesStatuses  ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Exam_Entry_Status e = new Exam_Entry_Status();
                            e.Hydrate(dr);
                            m_list.Add(e);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class Exam_Entry_CountbyBoard
    {
        public Exam_Board m_ExamBde;
        public string m_ExamEntryStatusId;
        public string m_Season;
        public string m_Year;
        public int m_count;

        public Exam_Entry_CountbyBoard() { m_ExamBde = new Exam_Board(); }
    }

    public class Exam_Entry_CountbyBoardList
    {
        public List<Exam_Entry_CountbyBoard> m_list = new List<Exam_Entry_CountbyBoard>();

        public Exam_Entry_CountbyBoardList()
        {
        }

        public string GetQuery(string Season, string Year, string EntryStatusId, bool IsStatus)
        {
            string s = "SELECT dbo.tbl_Exams_Syllabus.ExamBoardID,   dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId, ";
            s += " dbo.tbl_Core_ExamBoards.LegacyExamBdId, dbo.tbl_Core_Organisations.OrganisationName, ";
            s += " dbo.tbl_Core_Organisations.OrganisationFriendlyName, ";
            s += "  COUNT(dbo.tbl_Exams_Entries.ExamEntryID) AS Expr1, dbo.tbl_Exams_Entries.ExamSeason, dbo.tbl_Exams_Entries.ExamYear ";
            s += " FROM dbo.tbl_Exams_Entries INNER JOIN";
            s += " dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN ";
            s += " dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID INNER JOIN ";
            s += " dbo.tbl_Core_ExamBoards ON dbo.tbl_Exams_Syllabus.ExamBoardID = dbo.tbl_Core_ExamBoards.ExamBoardId INNER JOIN ";
            s += " dbo.tbl_Core_Organisations ON dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId = dbo.tbl_Core_Organisations.OrganisationId ";
            s += " GROUP BY dbo.tbl_Exams_Syllabus.ExamBoardID, dbo.tbl_Core_ExamBoards.LegacyExamBdId, dbo.tbl_Core_Organisations.OrganisationName, dbo.tbl_Core_Organisations.OrganisationFriendlyName, ";
            s += " dbo.tbl_Exams_Entries.EntryStatus, dbo.tbl_Exams_Entries.ExamSeason, dbo.tbl_Exams_Entries.ExamYear, dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId ";
            s += " HAVING  (dbo.tbl_Exams_Entries.EntryStatus ";
            if (IsStatus) s += "= '" + EntryStatusId + "' ) "; else s += "<>'" + EntryStatusId + "' ) ";
            s += "AND(dbo.tbl_Exams_Entries.ExamSeason = '" + Season + "') AND(dbo.tbl_Exams_Entries.ExamYear = '" + Year + "') ";
            return s;
        }

        public string GetQueryNOTSENT(string Season, string Year)
        {
            string s = "SELECT dbo.tbl_Exams_Syllabus.ExamBoardID,   dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId, ";
            s += " dbo.tbl_Core_ExamBoards.LegacyExamBdId, dbo.tbl_Core_Organisations.OrganisationName, ";
            s += " dbo.tbl_Core_Organisations.OrganisationFriendlyName,dbo.tbl_Exams_Entries.EntryStatus, ";
            s += "  COUNT(dbo.tbl_Exams_Entries.ExamEntryID) AS Expr1, dbo.tbl_Exams_Entries.ExamSeason, dbo.tbl_Exams_Entries.ExamYear ";
            s += " FROM dbo.tbl_Exams_Entries INNER JOIN";
            s += " dbo.tbl_Exams_Options ON dbo.tbl_Exams_Entries.OptionID = dbo.tbl_Exams_Options.OptionID INNER JOIN ";
            s += " dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID INNER JOIN ";
            s += " dbo.tbl_Core_ExamBoards ON dbo.tbl_Exams_Syllabus.ExamBoardID = dbo.tbl_Core_ExamBoards.ExamBoardId INNER JOIN ";
            s += " dbo.tbl_Core_Organisations ON dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId = dbo.tbl_Core_Organisations.OrganisationId ";
            s += " GROUP BY dbo.tbl_Exams_Syllabus.ExamBoardID, dbo.tbl_Core_ExamBoards.LegacyExamBdId, dbo.tbl_Core_Organisations.OrganisationName, dbo.tbl_Core_Organisations.OrganisationFriendlyName, ";
            s += " dbo.tbl_Exams_Entries.EntryStatus, dbo.tbl_Exams_Entries.ExamSeason, dbo.tbl_Exams_Entries.ExamYear, dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId, ";
            s += " dbo.tbl_Exams_Entries.EntryFileID ";
            s += " HAVING  (dbo.tbl_Exams_Entries.EntryFileID IS NULL )";
            s += "AND(dbo.tbl_Exams_Entries.ExamSeason = '" + Season + "') AND(dbo.tbl_Exams_Entries.ExamYear = '" + Year + "') ";
            return s;
        }

        public void Load(string Season, string Year, string EntryStatusId, bool IsStatus)
        {
            // Year is 4 digit value...
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = GetQuery(Season, Year, EntryStatusId, IsStatus);
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Exam_Entry_CountbyBoard exs = new Exam_Entry_CountbyBoard();
                            m_list.Add(exs);
                            exs.m_ExamBde.Hydrate(dr);
                            exs.m_count = dr.GetInt32(5);
                            exs.m_ExamEntryStatusId = EntryStatusId;
                            exs.m_Season = Season;
                            exs.m_Year = Year;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void Load_NotSent(string Season, string Year)
        {
            // Year is 4 digit value...
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = GetQueryNOTSENT(Season, Year);
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Exam_Entry_CountbyBoard exs = new Exam_Entry_CountbyBoard();
                            m_list.Add(exs);
                            exs.m_ExamBde.Hydrate(dr);
                            exs.m_count = dr.GetInt32(6);
                            exs.m_ExamEntryStatusId = dr.GetInt32(5).ToString();
                            exs.m_Season = Season;
                            exs.m_Year = Year;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class ExamColumn
    {
        public string name;
        public int count;
    }
    public class ExamRoom
    {
        public Guid m_RoomID;
        public int m_capacity;
        public int no_columns;
        public ExamColumn[] columns;

        public ExamRoom()
        {
            columns = new ExamColumn[41];
            for (int i = 0; i < 41; i++)
            {
                columns[i] = new ExamColumn();
            }
        }

        public void Load(Guid RoomId)
        {
            m_RoomID = RoomId;
            string s = "SELECT * FROM tbl_Exams_RoomLayouts WHERE (RoomId ='" + RoomId.ToString() + "' )";
            s += "   ORDER BY ColumnNumber ";
            Load1(s);
        }

        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        Hydrate(dr);
                    }
                }
                cn.Close();
            }
        }
        public void Hydrate(SqlDataReader dr)
        {
            int j = 0;
            no_columns = 0;
            m_capacity = 0;
            while (dr.Read())
            {
                j = dr.GetInt32(2);
                if (j < 40)
                {
                    columns[j].count = dr.GetInt32(4);
                    m_capacity += columns[j].count; no_columns++;
                    columns[j].name = dr.GetString(3);
                }
            }
            dr.Close();
        }

    }

    [DataContract]
    public class ExamTQMBoundary
    {
        [DataMember] public Guid TQMBoundaryId;
        [DataMember] public Guid OptionId;
        [DataMember] public int Mark;
        [DataMember] public string Grade;
        [DataMember] public string Year; //exam bde year ie 2 digit
        [DataMember] public string Season;
        [DataMember] public int Version;
        [DataMember] public string OptionCode;
        [DataMember] public string OptionTitle;

        public ExamTQMBoundary() { }

        public void Hydrate(SqlDataReader dr)
        {
            TQMBoundaryId = dr.GetGuid(0);
            OptionId = dr.GetGuid(1);
            if (!dr.IsDBNull(2)) Mark = dr.GetInt32(2);
            if (!dr.IsDBNull(3)) Grade = dr.GetString(3);
            Year = dr.GetString(4);
            Season = dr.GetString(5);
            Version = dr.GetInt32(6);
            OptionCode = dr.GetString(7);
            OptionTitle = dr.GetString(8);
        }
        public void Save()
        {
            string s = "";
            if (TQMBoundaryId == Guid.Empty)//new save
            {
                TQMBoundaryId = Guid.NewGuid();//so we know new id
                // make a new record..no NULLS allowed here
                s = "INSERT INTO dbo.tbl_Exams_TQM_Boundaries (TQMBoundariesId,OptionId, Mark,Grade,Year,Season,Version )";
                s += "VALUES ( '" + TQMBoundaryId.ToString() + "' ,";
                s += "'" + OptionId.ToString() + "' , '" + Mark.ToString() + "' , '" + Grade + "'";
                s += " , '" + Year + "' , '" + Season + "','" + Version + "' )";
            }
            else
            {
                s = " UPDATE[dbo].[tbl_Exams_TQM_Boundaries] ";
                s += " SET [OptionId] = '" + OptionId.ToString() + "' ";
                s += ",[Mark] = '" + Mark.ToString() + "' ";
                s += ",[Grade] = '" + Grade + "' ";
                s += ",[Year] = '" + Year + "' ";
                s += ",[Season] = '" + Season + "' ";
                s += ",[Version]  = '" + Version.ToString() + "' ";
                s += " WHERE TQMBoundariesId='" + TQMBoundaryId.ToString() + "'";

            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public void Delete()
        {
            string s = "";
            Encode en = new Encode();
            s = "DELETE  FROM dbo.tbl_Exams_TQM_Boundaries ";
            s += " WHERE (TQMBoundariesId='" + TQMBoundaryId.ToString() + "')";
            int n = en.Execute_count_SQL(s);
            TQMBoundaryId = Guid.Empty;
        }
    }

    [DataContract]
    public class ExamTQMBoundaryList
    {
        [DataMember] public List<ExamTQMBoundary> m_list = new List<ExamTQMBoundary>();

        public ExamTQMBoundaryList() { }

        public void LoadList(Guid optionId, string year, string season)
        {
            //get all syllabusses....???
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_list.Clear();

            string s = " SELECT  dbo.tbl_Exams_TQM_Boundaries.TQMBoundariesId, dbo.tbl_Exams_TQM_Boundaries.OptionId, dbo.tbl_Exams_TQM_Boundaries.Mark, dbo.tbl_Exams_TQM_Boundaries.Grade, dbo.tbl_Exams_TQM_Boundaries.Year, dbo.tbl_Exams_TQM_Boundaries.Season, dbo.tbl_Exams_TQM_Boundaries.Version";
            s += ", dbo.tbl_Exams_Options.OptionCode, dbo.tbl_Exams_Options.OptionTitle ";
            s += " FROM tbl_Exams_TQM_Boundaries INNER JOIN  dbo.tbl_Exams_Options ON dbo.tbl_Exams_TQM_Boundaries.OptionId = dbo.tbl_Exams_Options.OptionID ";
            s += " WHERE (  dbo.tbl_Exams_TQM_Boundaries.Year='" + year + "' ) AND (  dbo.tbl_Exams_TQM_Boundaries.Season='" + season + "'  ) AND ( dbo.tbl_Exams_TQM_Boundaries.OptionId = '" + optionId.ToString() + "' ) ";
            s += " ORDER BY dbo.tbl_Exams_TQM_Boundaries.OptionId, dbo.tbl_Exams_TQM_Boundaries.Mark DESC ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamTQMBoundary e1 = new ExamTQMBoundary();
                            m_list.Add(e1);
                            e1.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }

        public void LoadList(string year, string season)
        {
            //get all syllabusses....???
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_list.Clear();

            string s = " SELECT   dbo.tbl_Exams_TQM_Boundaries.TQMBoundariesId,  dbo.tbl_Exams_TQM_Boundaries.OptionId,  dbo.tbl_Exams_TQM_Boundaries.Mark,  dbo.tbl_Exams_TQM_Boundaries.Grade,  dbo.tbl_Exams_TQM_Boundaries.Year,  dbo.tbl_Exams_TQM_Boundaries.Season,  dbo.tbl_Exams_TQM_Boundaries.Version";
            s += ", dbo.tbl_Exams_Options.OptionCode, dbo.tbl_Exams_Options.OptionTitle ";
            s += " FROM tbl_Exams_TQM_Boundaries INNER JOIN  dbo.tbl_Exams_Options ON dbo.tbl_Exams_TQM_Boundaries.OptionId = dbo.tbl_Exams_Options.OptionID ";
            s += " WHERE ( dbo.tbl_Exams_TQM_Boundaries.Year='" + year + "' ) AND ( dbo.tbl_Exams_TQM_Boundaries.Season='" + season + "'  )  ";
            s += " ORDER BY dbo.tbl_Exams_TQM_Boundaries.OptionId, dbo.tbl_Exams_TQM_Boundaries.Mark DESC ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamTQMBoundary e1 = new ExamTQMBoundary();
                            m_list.Add(e1);
                            e1.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }

    [DataContract]
    public class ExamTQMBoundaryList_AsGrid
    {
        [DataMember] public List<List<string>> m_list = new List<List<string>>();
        public List<List<string>> Load(string year, string season)
        {
            ExamTQMBoundaryList TQM1 = new ExamTQMBoundaryList();
            TQM1.LoadList(year, season);

            string currentOPt = ""; int max_opts = 0;
            List<string> Grades = new List<string>();
            foreach (ExamTQMBoundary e1 in TQM1.m_list)
            {
                if (currentOPt != e1.OptionCode) { max_opts++; currentOPt = e1.OptionCode; }
                if (!Grades.Contains(e1.Grade)) Grades.Add(e1.Grade);
            }
            int max_grades = Grades.Count;
            string[][] a1 = new string[max_opts + 2][];
            for (int i = 0; i < max_opts + 2; i++) { string[] a2 = new string[max_grades + 3]; a1[i] = a2; }

            a1[0][0] = "Option Code"; a1[0][1] = "OptionTitle"; //a1[0][2] = "OptionId";
            for (int i = 0; i < Grades.Count; i++) a1[0][i + 3] = Grades[i];
            currentOPt = ""; int i1 = 1;
            foreach (ExamTQMBoundary e1 in TQM1.m_list)
            {
                if (currentOPt != e1.OptionCode)
                {
                    i1++;
                    a1[i1][0] = e1.OptionCode; a1[i1][1] = e1.OptionTitle; //a1[i1][2] = e1.OptionId.ToString();
                    currentOPt = e1.OptionCode;
                }
                for (int j = 0; j < max_grades; j++)
                {
                    if (a1[0][j] == e1.Grade) { a1[i1][j] = e1.Mark.ToString(); break; }
                }
            }

            //so have array??
            i1 = 0;
            for (int i = 0; i < max_opts; i++)
            {
                List<string> s1 = new List<string>();
                for (int j = 0; j < max_grades; j++)
                {
                    s1.Add(a1[i][j]);
                }
                m_list.Add(s1);
            }
            return m_list;
        }
    }

    [DataContract]
    public class ExamExternalResultsAsGrid
    {
        [DataMember]
        public List<List<string>> m_list = new List<List<string>>();
        public List<List<string>> Load(string year, string season, Guid StudentId)
        {
            List<string> s1 = new List<string>();
            m_list.Clear();
            s1.Add("id");
            s1.Add("Date");
            s1.Add("Description");
            s1.Add("Syllabus");
            s1.Add("Syllabus Title");
            s1.Add("Result");
            s1.Add("Add Info");
            m_list.Add(s1);

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            string s = " SELECT  dbo.tbl_Core_Results.ResultDate, dbo.tbl_List_ResultType.ShortName, dbo.tbl_Exams_Syllabus.SyllabusCode, dbo.tbl_Exams_Syllabus.SyllabusTitle, ";
            s += " dbo.tbl_Core_Results.ResultValue, dbo.tbl_Core_Results.ResultText  FROM  dbo.tbl_List_ResultType ";
            s += "  INNER JOIN dbo.tbl_Core_Results ON dbo.tbl_List_ResultType.Id = dbo.tbl_Core_Results.ResultType ";
            s += " INNER JOIN dbo.tbl_Exams_Syllabus INNER JOIN dbo.tbl_Exams_Options ON dbo.tbl_Exams_Syllabus.SyllabusID = dbo.tbl_Exams_Options.SyllabusID ON dbo.tbl_Core_Results.ExamsOptionId = dbo.tbl_Exams_Options.OptionID ";
            s += " WHERE(dbo.tbl_Exams_Options.YearCode = '" + year + "') AND  ( dbo.tbl_Exams_Options.SeasonCode = '" + season + "') ";
            s += " AND ( dbo.tbl_Core_Results.StudentId = '" + StudentId + "' )";
            s += " ORDER BY dbo.tbl_Core_Results.ResultType ASC ";
            int n = 0;
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            s1 = new List<string>();
                            s1.Add(n.ToString()); n++;
                            s1.Add(dr.GetDateTime(0).ToShortDateString());
                            for (int i = 1; i < 6; i++) s1.Add(dr.GetString(i));
                            m_list.Add(s1);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
            return m_list;
        }
    }



    public class ExamDesk : IComparable
    {
        public string DeskName;
        public int row;
        public int column;

        public override string ToString()
        {
            return DeskName;
        }
        public ExamDesk(string s)
        {
            DeskName = s;
            if (s == "")
            {
                row = 0; column = 0; return;
            }
            //assume it is of the form A10... Z99
            try
            {
                row = System.Convert.ToInt32(s.Substring(1));
                Byte b1 = (Byte)s[0];
                column = (int)(b1 - 64);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Desk name not in correct format");
            }


        }

        public int CompareTo(Object obj)
        {
            if (obj is ExamDesk)
            {
                ExamDesk otherDesk = (ExamDesk)obj;
                if (column != otherDesk.column) return column.CompareTo(otherDesk.column);
                //so if col is odd we go up... if col is even we go down...
                if ((column % 2) == 1) return row.CompareTo(otherDesk.row);
                return otherDesk.row.CompareTo(row);
            }
            else
            {
                throw new ArgumentException("Object is not an ExamDesk Object");
            }
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is ExamDesk))
                return false;
            return (this.CompareTo(obj) == 0);
        }

        public override int GetHashCode()
        {
            char[] c = this.DeskName.ToCharArray();
            return (int)c[0];
        }

        public static bool operator ==(ExamDesk r1, ExamDesk r2)
        {
            return r1.Equals(r2);
        }
        public static bool operator !=(ExamDesk r1, ExamDesk r2)
        {
            return !(r1 == r2);
        }
        public static bool operator <(ExamDesk r1, ExamDesk r2)
        {
            return (r1.CompareTo(r2) < 0);
        }
        public static bool operator >(ExamDesk r1, ExamDesk r2)
        {
            return (r1.CompareTo(r2) > 0);
        }

        public static bool operator >=(ExamDesk r1, ExamDesk r2)
        {
            if (r1 == r2) return true;
            return (r1.CompareTo(r2) > 0);
        }
        public static bool operator <=(ExamDesk r1, ExamDesk r2)
        {
            if (r1 == r2) return true;
            return (r1.CompareTo(r2) < 0);
        }
    }

    [Serializable]
    public class ExamSyllabus : IComparable
    {
        public Guid m_SyllabusId;
        public Guid m_ExamBoardId;
        public string m_Syllabus_Code;
        public string m_Syllabus_Title;
        public DateTime m_ValidFrom;
        public DateTime m_ValidUntil;
        public bool m_valid = false;
        public Guid m_CourseID;
        public string m_ExamBoard_FriendlyName;


        public ExamSyllabus()
        {
        }

        public int CompareTo(object obj)
        {
            if (obj is ExamSyllabus)
            {
                ExamSyllabus other = (ExamSyllabus)obj;
                return this.ToString().CompareTo(other.ToString());
            }
            else
            {
                throw new ArgumentException("Object is not an ExamComponent");
            }
        }

        public bool LoadFromBaseData(string line, int JCQ_Version)
        {
            if (JCQ_Version <= 15)
            {
                string s = line.Substring(0, 2); line = line + "                     ";
                if (line.Substring(0, 2) == "S5")
                {
                    m_Syllabus_Code = line.Substring(2, 6);
                    m_Syllabus_Title = line.Substring(9, 36);
                    return true;
                }
            }

            return false;



        }


        public void Load(string syl_code, string Season, string year, Guid ExamBoardID)
        {
            Load1(" WHERE (ExamBoardID= '" + ExamBoardID.ToString() + "') AND  (SyllabusCode = '" + syl_code + "') ", year, Season);
        }

        public void Load(Guid ExamsSyllabusId)
        {
            Load1(" WHERE ( SyllabusId ='" + ExamsSyllabusId.ToString() + "' )", "", "");
        }

        private void Load1(string where_string, string year, string season)
        {
            //going to load from db...
            string s;
            s = "SELECT * FROM dbo.tbl_Exams_Syllabus ";
            s += where_string;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_valid = false;
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            //now want it to be valid in year,season....
                            m_valid = false;
                            if ((!dr.IsDBNull(4)) && (!dr.IsDBNull(5)) && (year != "") && (season != ""))
                            {
                                m_ValidFrom = dr.GetDateTime(4);
                                m_ValidUntil = dr.GetDateTime(5);
                                int y = System.Convert.ToInt32(year);
                                int m = System.Convert.ToInt32(season, 16);
                                if ((m_ValidFrom.Year <= y) && (m_ValidUntil.Year >= y) && (m_ValidFrom.Month <= m) && (m_ValidUntil.Month >= m))
                                {
                                    m_valid = true;
                                }
                            }
                            else
                            {
                                m_valid = true;
                            }
                            if (m_valid)
                            {
                                m_SyllabusId = dr.GetGuid(0);
                                if (!dr.IsDBNull(1)) m_ExamBoardId = dr.GetGuid(1); else m_valid = false;
                                m_Syllabus_Code = dr.GetString(2);
                                m_Syllabus_Title = dr.GetString(3);
                                if (!dr.IsDBNull(4)) m_ValidFrom = dr.GetDateTime(4);
                                if (!dr.IsDBNull(5)) m_ValidUntil = dr.GetDateTime(5);
                            }
                            m_CourseID = Guid.Empty;
                        }
                        dr.Close();
                    }
                }


            }
            m_CourseID = Guid.Empty;
            if (m_valid)
            {
                s = "SELECT * FROM dbo.tbl_Core_Course_ExamsSyllabus ";
                s += "WHERE (ExamsSyllabusId = '" + m_SyllabusId.ToString() + "') ";
                using (SqlConnection cn = new SqlConnection(db_connection))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(s, cn))
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(1)) m_CourseID = dr.GetGuid(1); else m_CourseID = Guid.Empty;
                            }
                            dr.Close();
                        }
                    }
                }
            }
        }

        public Guid CreateNew(string Version)
        {
            //used by sync to iSAMS stuff
            string s = ""; Guid g1 = new Guid(); g1 = Guid.NewGuid();
            s = "INSERT INTO dbo.tbl_Exams_Syllabus ";
            s += "(SyllabusId, ExamBoardID, SyllabusCode, SyllabusTitle ,Version)";
            s += "VALUES ('" + g1.ToString() + "' , '" + m_ExamBoardId.ToString() + "' , '";
            s += m_Syllabus_Code + "' , '" + m_Syllabus_Title + "' ,'" + Version + "' )";
            Encode en = new Encode();
            en.ExecuteSQL(s);
            return g1;
        }


    }
    public class ExamSyllabus_List
    {
        public ArrayList m_list = new ArrayList();
        public int m_count;


        public ExamSyllabus_List()
        {
        }

        public void Load()
        {
            //get all syllabusses....???
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_count = 0;
            string s = " SELECT dbo.tbl_Exams_Syllabus.SyllabusID, dbo.tbl_Exams_Syllabus.SyllabusCode, dbo.tbl_Exams_Syllabus.SyllabusTitle, dbo.tbl_Core_Organisations.OrganisationFriendlyName, ";
            s += "dbo.tbl_Exams_Syllabus.ExamBoardID, dbo.tbl_Exams_Syllabus.ValidFrom, dbo.tbl_Exams_Syllabus.ValidUntil ";
            s += " FROM dbo.tbl_Core_ExamBoards INNER JOIN ";
            s += " dbo.tbl_Exams_Syllabus ON dbo.tbl_Core_ExamBoards.ExamBoardId = dbo.tbl_Exams_Syllabus.ExamBoardID ";
            s += " INNER JOIN dbo.tbl_Core_Organisations ON dbo.tbl_Core_ExamBoards.ExamBoardOrganisationId = dbo.tbl_Core_Organisations.OrganisationId   ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamSyllabus exs = new ExamSyllabus();
                            m_list.Add(exs);
                            exs.m_SyllabusId = dr.GetGuid(0);
                            exs.m_Syllabus_Code = dr.GetString(1);
                            exs.m_Syllabus_Title = dr.GetString(2);
                            exs.m_ExamBoard_FriendlyName = dr.GetString(3);
                            exs.m_ExamBoardId = dr.GetGuid(4);
                            m_count++;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class ExamOption
    {
        public Guid m_OptionID;
        public Guid m_SyllabusID;
        public string m_Syllabus_Code;
        public string m_Syllabus_Title;
        public Guid m_ExamBoardID;
        public string m_OptionCode;
        public string m_OptionTitle;
        public string m_OptionQualification;
        public string m_OptionLevel;
        public string m_Item;
        public string m_Process;
        public string m_QCACode;
        public string m_QCANumber;
        public string m_fee;
        public string m_OptionMaximumMark;
        public string m_year_Code;
        public string m_Season_code;
        public string m_SeriesIdentifier;
        public bool m_valid = false;
        public int m_feeInt;


        public ExamOption()
        {
        }
        public Guid Value
        {
            get
            {
                return m_OptionID;
            }
        }

        public bool NeedsTeacherMarks()
        {
            ExamCompononent_List ecl1 = new ExamCompononent_List();
            ecl1.Load(m_OptionID);
            foreach (ExamComponent ec1 in ecl1.m_list)
            {
                if (ec1.m_Teachermarks == "Y") return true;
            }
            return false;
        }

        public override string ToString()
        {
            return (m_OptionCode + " : " + m_OptionTitle + " - " + m_Syllabus_Title + ":" + m_OptionID.ToString());
        }

        public void Load(string opt_code, string Season, string year, string ExamBoardCode)
        {
            //ExamBoardCode is the 2 char code (70, 10 etc call LegacyID in db)
            Exam_Board ex1 = new Exam_Board(ExamBoardCode);//get the real guid
            Load(opt_code, Season, year, ex1.m_ExamBoardId);
        }

        public bool Load(string opt_code, string Season, string year, Guid Examboard_ID)
        {
            //going to load from db...
            string s;

            s = " SELECT dbo.tbl_Exams_Options.OptionID, dbo.tbl_Exams_Options.SyllabusID, ";
            s += "dbo.tbl_Core_ExamBoards.ExamBoardId, dbo.tbl_Exams_Options.SeasonCode, ";
            s += "dbo.tbl_Exams_Options.YearCode, dbo.tbl_Exams_Options.OptionCode, ";
            s += "dbo.tbl_Exams_Options.OptionTitle, dbo.tbl_Exams_Options.OptionQualification, ";
            s += "dbo.tbl_Exams_Options.OptionLevel, dbo.tbl_Exams_Syllabus.SyllabusCode, ";
            s += "dbo.tbl_Exams_Syllabus.SyllabusTitle, dbo.tbl_Exams_Options.OptionMaximumMark, ";
            s += " dbo.tbl_Exams_Options.OptionItem , dbo.tbl_Exams_Options.SeriesIdentifier ";
            s += ", dbo.tbl_Exams_Options.OptionFee , dbo.tbl_Exams_Options.OptionProcess, ";
            s += " dbo.tbl_Exams_Options.QCAClassificationCode, dbo.tbl_Exams_Options.QCAAccreditationNumber  ";
            s += "FROM         dbo.tbl_Exams_Options ";
            s += "INNER JOIN dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID ";
            s += "INNER JOIN dbo.tbl_Core_ExamBoards ON dbo.tbl_Exams_Syllabus.ExamBoardID = dbo.tbl_Core_ExamBoards.ExamBoardId ";
            s += "WHERE     (dbo.tbl_Exams_Options.SeasonCode = N'" + Season + "') ";
            s += " AND (dbo.tbl_Exams_Options.OptionCode = N'" + opt_code + "') ";
            s += " AND (dbo.tbl_Exams_Options.YearCode = N'" + year + "') ";
            s += " AND (dbo.tbl_Core_ExamBoards.ExamBoardId = '" + Examboard_ID.ToString() + "')  ";

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_valid = false;
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
            }
            return m_valid;
        }

        public void Load(Guid OptionID)
        {
            //going to load from db...
            string s;

            s = "SELECT * FROM qry_Cerval_Exams_Options  ";
            s += "WHERE (OptionID = '" + OptionID.ToString() + "' ) ";

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_valid = false;
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
            }
        }

        public void Hydrate(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0))
            {
                m_OptionID = dr.GetGuid(0); m_valid = true;
            }
            if (!dr.IsDBNull(1)) m_SyllabusID = dr.GetGuid(1); else m_valid = false;
            if (!dr.IsDBNull(2)) m_ExamBoardID = dr.GetGuid(2); else m_valid = false;
            m_Season_code = dr.GetString(3);
            m_year_Code = dr.GetString(4);
            m_OptionCode = dr.GetString(5);
            m_OptionTitle = dr.GetString(6);
            m_OptionQualification = dr.GetString(7).Trim(); ;
            m_OptionLevel = dr.GetString(8).Trim();
            m_Syllabus_Code = dr.GetString(9);
            m_Syllabus_Title = dr.GetString(10);
            if (!dr.IsDBNull(11)) m_OptionMaximumMark = dr.GetString(11);
            m_Item = dr.GetString(12).Trim();
            if (!dr.IsDBNull(13)) m_SeriesIdentifier = dr.GetString(13);
            m_fee = "0";
            if (!dr.IsDBNull(14)) m_fee = dr.GetString(14);
            try
            {
                m_feeInt = System.Convert.ToInt32(m_fee);
            }
            catch
            {
                m_feeInt = 0;
            }
            if (!dr.IsDBNull(15)) m_Process = dr.GetString(15);
            if (!dr.IsDBNull(16)) m_QCACode = dr.GetString(16);
            if (!dr.IsDBNull(17)) m_QCANumber = dr.GetString(17);
        }

        public int Delete()
        {
            string s = "";
            Encode en = new Encode();
            s = "DELETE  FROM dbo.tbl_Exams_Link ";
            s += " WHERE (OptionID='" + m_OptionID.ToString() + "' )";
            int n = en.Execute_count_SQL(s);
            s = "DELETE  FROM dbo.tbl_Exams_Options ";
            s += " WHERE (OptionID='" + m_OptionID.ToString() + "' )";
            n = en.Execute_count_SQL(s);
            return n;
        }

        public void UpdateMaxMark()
        {
            string s = "";
            Encode en = new Encode();
            s = "UPDATE dbo.tbl_Exams_Options SET OptionMaximumMark = '" + m_OptionMaximumMark.Substring(0, 4) + "'";
            s += " WHERE (OptionID='" + m_OptionID.ToString() + "' )";
            en.ExecuteSQL(s);
            return;
        }

        public Guid CreateNew(string Version)
        {
            //used by sync to iSAMS
            string s = ""; Guid g1 = new Guid(); g1 = Guid.NewGuid();
            s = "INSERT INTO dbo.tbl_Exams_Options  (OptionID, SyllabusID ,OptionCode , OptionTitle, ";
            s += "OptionQualification, OptionLevel, OptionItem, OptionProcess, ";
            s += "QCAClassificationCode, QCAAccreditationNumber, OptionFee, OptionMaximumMark, YearCode, SeasonCode,  SeriesIdentifier, Version )";
            s += "VALUES  ( '" + g1.ToString() + "', '" + m_SyllabusID.ToString() + "' , '";
            s += m_OptionCode + "' , '" + m_OptionTitle + "' , '" + m_OptionQualification + "' , '";
            s += m_OptionLevel + "' , '" + m_Item + "' , '" + m_Process + "' , '";
            s += m_QCACode + "' , '" + m_QCANumber + "' , '";
            s += m_fee.ToString();
            s += "' , '" + m_OptionMaximumMark + "' ,'";
            s += m_year_Code + "' , '" + m_Season_code + "' , '" + m_SeriesIdentifier + "', '" + Version + "' ) ";
            Encode en = new Encode();
            en.ExecuteSQL(s);
            return g1;
        }

    }
    public class ExamOption_List
    {
        public ArrayList m_list = new ArrayList();
        public int m_count;


        public ExamOption_List()
        {
        }

        public void Load(string OptionItem)
        {
            //get all options....???
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_count = 0;
            string s = " SELECT dbo.tbl_Exams_Options.OptionID  ";
            s += " FROM dbo.tbl_Exams_Options  ";
            s += " WHERE (OptionItem = N'";
            s += OptionItem;
            s += "') ";
            s += "  ORDER BY OptionTitle ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamOption exo1 = new ExamOption();
                            m_list.Add(exo1);
                            exo1.Load(dr.GetGuid(0));
                            m_count++;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void Load_Syllabus(Guid SyllabusID, string YearCode, string season)
        {
            string s = "SELECT * FROM qry_Cerval_Exams_Options  ";
            s += " WHERE (SyllabusID = '" + SyllabusID.ToString() + "' ) ";
            s += " AND (YearCode = '" + YearCode + "' ) ";
            s += " AND ( SeasonCode = '" + season + "' ) ";
            Load1(s);
        }

        public void Load(string YearCode, string season)
        {
            string s = "SELECT * FROM qry_Cerval_Exams_Options  ";
            s += " WHERE (YearCode = '" + YearCode + "' ) ";
            s += " AND ( SeasonCode = '" + season + "' ) ";
            s += " ORDER BY OptionCode";
            Load1(s);
        }

        public void Load(string ExamBdeId, string YearCode, string season)
        {
            string s = "SELECT * FROM qry_Cerval_Exams_Options  ";
            s += " WHERE (YearCode = '" + YearCode + "' ) ";
            s += " AND ( SeasonCode = '" + season + "' ) ";
            s += " AND ( ExamBoardID = '" + ExamBdeId.ToString() + "' ) ";
            s += " ORDER BY OptionCode";
            Load1(s);
        }

        public void Load(string ExamBdeId, string YearCode, string season, string OrderBy)
        {
            string s = "SELECT * FROM qry_Cerval_Exams_Options  ";
            s += " WHERE (YearCode = '" + YearCode.Substring(YearCode.Length - 2) + "' ) ";
            s += " AND ( SeasonCode = '" + season + "' ) ";
            s += " AND ( ExamBoardID = '" + ExamBdeId.ToString() + "' ) ";
            s += " ORDER BY " + OrderBy;
            Load1(s);
        }
        private void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_count = 0; m_list.Clear();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ExamOption exo1 = new ExamOption();
                            m_list.Add(exo1);
                            exo1.Hydrate(dr);
                            m_count++;
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }

        public void LoadSimilar(string OptionCode, string ExamBdeId, string YearCode, string season)
        {
            string s = "SELECT * FROM qry_Cerval_Exams_Options  ";
            s += " WHERE (YearCode = '" + YearCode + "' ) ";
            s += " AND ( SeasonCode = '" + season + "' ) ";
            s += " AND ( ExamBoardID = '" + ExamBdeId.ToString() + "' ) ";
            s += " AND  (OptionCode LIKE N'" + OptionCode + "%') ";
            Load1(s);
        }

        public void LoadforComponent(Guid ComponentID)
        {
            string s = "SELECT dbo.qry_Cerval_Exams_Options.* ";
            s += " FROM dbo.tbl_Exams_Link INNER JOIN dbo.qry_Cerval_Exams_Options ON dbo.tbl_Exams_Link.OptionID = dbo.qry_Cerval_Exams_Options.OptionID ";
            s += "WHERE  (dbo.tbl_Exams_Link.ComponentID = '" + ComponentID.ToString() + "')  ";
            Load1(s);
        }
        //dbo.tbl_Core_Course_ExamsSyllabus
        public void LoadforCourse(Guid CourseId)
        {
            string s = "SELECT dbo.qry_Cerval_Exams_Options.* ";
            s += " FROM dbo.tbl_Core_Course_ExamsSyllabus INNER JOIN dbo.qry_Cerval_Exams_Options ON dbo.tbl_Core_Course_ExamsSyllabus.SyllabusID = dbo.qry_Cerval_Exams_Options.SyllabusID ";
            s += "WHERE  (dbo.tbl_Core_Course_ExamsSyllabus.CourseID = '" + CourseId.ToString() + "')  ";
            Load1(s);
        }
        //old version....
        public void LoadforCourseX(Guid CourseId)
        {
            string s = "SELECT dbo.qry_Cerval_Exams_Options.* ";
            s += " FROM tbl_Exams_Options_Courses INNER JOIN dbo.qry_Cerval_Exams_Options ON tbl_Exams_Options_Courses.OptionID = dbo.qry_Cerval_Exams_Options.OptionID ";
            s += "WHERE  (tbl_Exams_Options_Courses.CourseID = '" + CourseId.ToString() + "')  ";
            Load1(s);
        }
        public void LoadforCourse(Guid CourseId, string YearCode)
        {
            string s = "SELECT dbo.qry_Cerval_Exams_Options.* ";
            s += " FROM dbo.tbl_Core_Course_ExamsSyllabus INNER JOIN dbo.qry_Cerval_Exams_Options ON dbo.tbl_Core_Course_ExamsSyllabus.ExamsSyllabusId = dbo.qry_Cerval_Exams_Options.SyllabusID ";
            s += "WHERE  (dbo.tbl_Core_Course_ExamsSyllabus.CourseID = '" + CourseId.ToString() + "')  ";
            s += "AND (dbo.qry_Cerval_Exams_Options.YearCode ='" + YearCode + "')  ";
            Load1(s);
        }

        public void LoadforCourse(Guid CourseId, string YearCode, string Season)
        {
            string s = "SELECT dbo.qry_Cerval_Exams_Options.* ";
            s += " FROM dbo.tbl_Core_Course_ExamsSyllabus INNER JOIN dbo.qry_Cerval_Exams_Options ON dbo.tbl_Core_Course_ExamsSyllabus.ExamsSyllabusId = dbo.qry_Cerval_Exams_Options.SyllabusID ";
            s += "WHERE  (dbo.tbl_Core_Course_ExamsSyllabus.CourseID = '" + CourseId.ToString() + "')  ";
            s += "AND (dbo.qry_Cerval_Exams_Options.YearCode ='" + YearCode + "')  ";
            s += "AND (dbo.qry_Cerval_Exams_Options.SeasonCode ='" + Season + "')  ";
            Load1(s);
        }

    }
    public class Exam_ResitEntry
    {
        public Guid m_Id;
        public Guid m_StudentId;
        public Guid m_OptionId;
        public string m_season;
        public string m_year;
        public DateTime m_DateCreated;
        public DateTime m_DateProcessed;
        public int m_cost;
        public int m_version;
        public bool m_valid;

        public Exam_ResitEntry()
        {
            m_Id = Guid.Empty;
            m_valid = false;
        }

        private void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetGuid(0);
            m_OptionId = dr.GetGuid(1);
            m_StudentId = dr.GetGuid(2);
            m_season = dr.GetString(3);
            m_year = dr.GetString(4);
            m_DateCreated = dr.GetDateTime(5);
            if (!dr.IsDBNull(6)) m_DateProcessed = dr.GetDateTime(6);
            if (!dr.IsDBNull(7)) m_cost = dr.GetInt32(7); else m_cost = 0;
            m_version = dr.GetInt32(8);
            m_valid = true;
        }

        private void Load1(string s)
        {
            m_valid = false;
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
                    }
                }
            }
        }

        public void Load(Guid Id)
        {
            string s = "SELECT * FROM dbo.tbl_Exams_ResitEntries WHERE ( Id ='" + Id.ToString() + "' ) ";
            Load1(s);
        }

        public void Load(Guid StudentId, Guid OptionId)
        {
            string s = " SELECT * FROM dbo.tbl_Exams_ResitEntries ";
            s += "WHERE (StudentId = '" + StudentId.ToString() + "' ) ";
            s += "AND ( OptionId = '" + OptionId.ToString() + "' ) ";
            Load1(s);
        }

        public void Save()
        {
            Encode en = new Encode();
            string s = "";
            if (m_Id != Guid.Empty)
            {
                //so update....
                s = " UPDATE dbo.tbl_Exams_ResitEntries SET ";
                s += " DateProcessed = CONVERT(DATETIME, '" + m_DateProcessed.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                s += ", Cost = '" + m_cost.ToString() + "' ";
                s += " WHERE (Id = '" + m_Id.ToString() + "' ) ";
                en.ExecuteSQL(s);
            }
            else
            {
                s = "INSERT INTO dbo.tbl_Exams_ResitEntries ";
                s += "( StudentId, OptionId, Season, Year, DateCreated, Cost, Version )";
                s += "VALUES ('" + m_StudentId.ToString() + "' , '" + m_OptionId.ToString() + "' , '";
                s += m_season + "' , '" + m_year + "' , ";
                s += " CONVERT(DATETIME, '" + m_DateCreated.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)  , ";
                s += "'" + m_cost.ToString() + "' , '1' ) ";
                en.ExecuteSQL(s);
            }
        }

        public void Delete()
        {
            Encode en = new Encode();
            string s = "";
            if (m_Id != Guid.Empty)
            {
                s = "DELETE FROM dbo.tbl_Exams_ResitEntries WHERE (Id = '" + m_Id.ToString() + "' ) ";
                en.ExecuteSQL(s);
            }
        }


    }
    public class Exam_Series
    {
        public Exam_Series()
        {
        }
        public string series = "";
    }
    public class Exam_SeriesList
    {
        public ArrayList m_list = new ArrayList();

        public Exam_SeriesList(Guid ExamBde, string Year, string season)
        {
            //series list for which we have entries.....
            Encode en = new Encode();
            string s = " SELECT DISTINCT SeriesIdentifier ";
            s += " FROM dbo.tbl_Exams_Options INNER JOIN dbo.tbl_Exams_Syllabus ON dbo.tbl_Exams_Options.SyllabusID = dbo.tbl_Exams_Syllabus.SyllabusID ";
            s += " INNER JOIN dbo.tbl_Core_ExamBoards ON dbo.tbl_Exams_Syllabus.ExamBoardID = dbo.tbl_Core_ExamBoards.ExamBoardId  ";
            s += " INNER JOIN dbo.tbl_Exams_Entries ON dbo.tbl_Exams_Options.OptionID = dbo.tbl_Exams_Entries.OPtionID ";
            s += " WHERE (dbo.tbl_Exams_Options.YearCode = '" + Year + "' ) AND (dbo.tbl_Exams_Options.SeasonCode = '" + season + "' )  ";
            s += " AND ( dbo.tbl_Core_ExamBoards.ExamBoardId ='" + ExamBde.ToString() + "' )   ";

            m_list.Clear();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Exam_Series ex1 = new Exam_Series();
                            m_list.Add(ex1);
                            ex1.series = dr.GetString(0);
                        }
                        dr.Close();
                    }
                }
                cn.Close();

            }

        }
    }
    public class ScheduledComponent
    {
        public Guid m_Id;
        public Guid m_StudentId;
        public Guid m_ComponentId;
        public DateTime m_Date;
        public string m_Year;
        public string m_Season;
        public Guid m_RoomId;
        public bool m_Will_Type;
        public bool m_valid;
        public string m_ComponentCode;
        public int m_TimeAllowed;
        public string m_UCI;
        public string m_Givenname;
        public string m_Surname;
        public string m_Timetabled;
        public DateTime m_TimetabledDate;
        public string m_TimetabledSession;
        public string m_ComponentTitle;
        public string m_Desk;
        public string m_RoomCode;
        public int m_ExamNumber;

        public ScheduledComponent()
        {
            m_Id = Guid.Empty;
            m_RoomId = Guid.Empty;
        }
        public void Load(Guid Id)
        {
            string s;
            m_valid = false; m_Id = Guid.Empty;
            s = "SELECT * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += "WHERE  (ScheduledComponentId = '" + Id.ToString() + "') ";
            Load1(s);
        }
        public void Load(Guid ComponentId, Guid StudentId)
        {
            string s;
            m_valid = false; m_Id = Guid.Empty;
            s = "SELECT * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += "WHERE  (ComponentId = '" + ComponentId.ToString() + "') ";
            s += " AND (StudentId = '" + StudentId.ToString() + "') ";
            Load1(s);
        }
        public void Load1(string s)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
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
            }
            //now going to sort out the roomcodes.... try to only load when we need to...
            SimpleRoom r1 = new SimpleRoom(); r1.Load(m_RoomId.ToString());
            m_RoomCode = r1.m_roomcode;

        }

        public void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetGuid(0);
            m_StudentId = dr.GetGuid(1);
            m_Year = dr.GetString(2);
            m_Season = dr.GetString(3);
            m_ComponentId = dr.GetGuid(4);
            if (!dr.IsDBNull(5)) m_Date = dr.GetDateTime(5);
            if (!dr.IsDBNull(6)) m_RoomId = dr.GetGuid(6);
            m_Will_Type = dr.GetBoolean(7);
            m_ComponentCode = dr.GetString(8);
            m_TimeAllowed = 0; if (!dr.IsDBNull(9)) m_TimeAllowed = dr.GetInt32(9);
            m_UCI = ""; if (!dr.IsDBNull(10)) m_UCI = dr.GetString(10);
            m_Givenname = dr.GetString(11);
            m_Surname = dr.GetString(12);
            m_Timetabled = dr.GetString(13);
            if (!dr.IsDBNull(14)) m_TimetabledDate = dr.GetDateTime(14);
            if (!dr.IsDBNull(15)) m_TimetabledSession = dr.GetString(15);
            m_ComponentTitle = dr.GetString(16);
            if (!dr.IsDBNull(17)) m_Desk = dr.GetString(17); else m_Desk = "";
            m_ExamNumber = dr.GetInt32(18);
            m_valid = true;
            StudentSENList ssen1 = new StudentSENList(m_StudentId.ToString());
            int max_extratime = 0;
            bool has_Special_Access = false;


            foreach (StudentSEN s1 in ssen1.m_List)
            {
                if (s1.m_ExamsExtraTime > max_extratime) max_extratime = s1.m_ExamsExtraTime;
                if (s1.m_ExamsCanType) m_Will_Type = true; else m_Will_Type = false;
                if (s1.m_SpecialAccess) has_Special_Access = true;
            }
            if (max_extratime > 0) m_TimeAllowed = (int)((float)m_TimeAllowed * ((100 + (float)max_extratime) / 100));

            // now check the annoying exeptions file....
            StudentSENExceptionList stsenl1 = new StudentSENExceptionList();
            if (has_Special_Access)
            {
                stsenl1.Load_Student(m_StudentId, m_Year.Substring(2, 2), m_Season);
                if (stsenl1.m_list.Count == 0) return;
                foreach (StudentSENException s in stsenl1.m_list)
                {
                    if (m_ComponentId == s.m_ComponentId)
                    {
                        m_TimeAllowed = (int)(m_TimeAllowed * ((100 + (float)s.m_ExtraTime) / 100));
                        if (s.m_CanType) m_Will_Type = true; else m_Will_Type = false;
                    }
                }
            }


        }

        public void Save()
        {
            Encode en = new Encode();
            if (!m_valid) return;
            string s = "";
            if (m_Id != Guid.Empty)
            {
                //so update....
                s = " UPDATE dbo.tbl_Exams_ScheduledComponents SET DateTime=CONVERT(DATETIME, '" + m_Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                if (m_RoomId == Guid.Empty) s += " , RoomId = NULL "; else s += " , RoomId = '" + m_RoomId.ToString() + "' ";
                if (m_Desk == "") s += " , Desk = NULL "; else s += ", Desk = '" + m_Desk + "' ";
                if (m_Will_Type) s += ", WillType = '1' "; else s += ", WillType = '0' ";
                s += " WHERE (ScheduledComponentId = '" + m_Id.ToString() + "' ) ";
                en.ExecuteSQL(s);
            }
            else
            {
                s = " INSERT INTO dbo.tbl_Exams_ScheduledComponents (StudentId, Year, Season, ComponentId ";
                if (m_Date != null) s += ", DateTime";
                if (m_RoomId != Guid.Empty) s += ", RoomId ";
                s += ", WillType, Desk, version) ";
                s += " VALUES ('" + m_StudentId.ToString() + "' , '" + m_Year + "' , '" + m_Season + "' , '" + m_ComponentId.ToString() + "' ";
                if (m_Date != null) s += ", CONVERT(DATETIME, '" + m_Date.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
                if (m_RoomId != Guid.Empty) s += " , '" + m_RoomId.ToString() + "' ";
                s += ", '"; if (m_Will_Type) s += "1', "; else s += "0', ";
                if (m_Desk == "") s += "  NULL "; else s += "'" + m_Desk + "' ";
                s += " ,'2' )";
                en.ExecuteSQL(s);
            }
        }

        public int Delete()
        {
            string s = "DELETE  FROM tbl_Exams_ScheduledComponents ";
            s += " WHERE (ScheduledComponentID='" + m_Id.ToString() + "' )";
            Encode en = new Encode();
            int n = en.Execute_count_SQL(s);
            return n;
        }

    }
    public class ScheduledComponentList
    {
        public ArrayList m_List = new ArrayList();

        private void LoadList1(string s)
        {
            m_List.Clear();
            RoomList rl = new RoomList(); rl.LoadList();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ScheduledComponent c = new ScheduledComponent();
                            m_List.Add(c);
                            c.Hydrate(dr);
                            if (c.m_RoomId != Guid.Empty)
                            {
                                foreach (SimpleRoom r in rl.m_roomlist)
                                {
                                    if (c.m_RoomId == r.m_RoomID) c.m_RoomCode = r.m_roomcode;
                                }
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void LoadList_Room(DateTime start, DateTime end, Guid RoomId)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (RoomId ='" + RoomId.ToString() + "' )";
            s += "ORDER BY ComponentId";
            LoadList1(s);
        }

        public void LoadList_Room(DateTime start, DateTime end, Guid RoomId, string OrderBy)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (RoomId ='" + RoomId.ToString() + "' )";
            s += OrderBy;
            LoadList1(s);
        }

        public void LoadListAll(string Year, string Season)
        {
            string s = "SELECT   *  FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (Year ='" + Year + "' ) AND (  Season = '" + Season + "' )";
            s += "ORDER BY StudentId, DateTime ";
            LoadList1(s);
        }

        public void LoadListAll(string Year, string Season, string Where, string Orderby)
        {
            string s = "SELECT   *  FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            if (Where == "")
            {
                s += " WHERE (Year ='" + Year + "' ) AND (  Season = '" + Season + "' ) ";
            }
            else
            {
                s += Where;
                s += " AND (Year ='" + Year + "' ) AND (  Season = '" + Season + "' ) ";
            }
            s += Orderby;
            LoadList1(s);
        }


        public void LoadList(DateTime start, DateTime end)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += "ORDER BY StudentId , TimeAllowed  DESC ";
            LoadList1(s);
        }
        public void LoadList_Date(DateTime start, DateTime end)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += "ORDER BY DateTime ,StudentExamNumber ";
            LoadList1(s);
        }

        public void LoadList_orderbyStudentDate(DateTime start, DateTime end)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += "ORDER BY StudentId , DateTime  DESC ";
            LoadList1(s);
        }

        public void LoadList_orderbyStudentDateASC(DateTime start, DateTime end)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += "ORDER BY StudentId , DateTime, TimeAllowed DESC, ComponentCode ";
            LoadList1(s);
        }


        public void LoadList(Guid StudentID, string Year, string Season)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (StudentId ='" + StudentID.ToString() + "' ) ";
            s += " AND (Year ='" + Year + "' ) AND (  Season = '" + Season + "' )";
            s += "ORDER BY DateTime  ";
            LoadList1(s);
        }

        public void LoadList_orderbyComponent(DateTime start, DateTime end)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += "ORDER BY ComponentId ";
            LoadList1(s);
        }


        public void LoadList_orderbyRoom(DateTime start, DateTime end)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += "ORDER BY RoomId ";
            LoadList1(s);
        }

        public void LoadList(DateTime start, DateTime end, string ComponentId)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND ( ComponentId ='" + ComponentId + "' ) ";
            s += "ORDER BY StudentId ";
            LoadList1(s);
        }

        public void LoadList(Guid ComponentId)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE ( ComponentId ='" + ComponentId.ToString() + "' )  ";
            s += "ORDER BY PersonSurname, PersonGivenName ";
            LoadList1(s);
        }


        public void LoadList_Student(DateTime start, DateTime end, string StudentId)
        {
            string s = "SELECT  * FROM dbo.qry_Cerval_Exams_ScheduledComponents ";
            s += " WHERE (DateTime > CONVERT(DATETIME,'" + start.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND (DateTime < CONVERT(DATETIME,'" + end.ToString("yyyy-MM-dd HH:mm:ss") + "',102) ) ";
            s += " AND ( StudentId ='" + StudentId + "' ) ";
            s += "ORDER BY DateTime ";
            LoadList1(s);
        }

        public ScheduledComponent Find_In_List(Guid StudentId)
        {
            if (m_List.Count == 0) return null;
            foreach (ScheduledComponent c in m_List)
            {
                if (c.m_StudentId == StudentId) return c;
            }
            return null;
        }
    }
    public class CanTypeList
    {
        public ArrayList m_List = new ArrayList();
        public CanTypeList()
        {
            Encode en = new Encode();
            string s = "SELECT * FROM dbo.qry_Cerval_Exams_CanType ";
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Guid g = new Guid();
                            g = (Guid)dr.GetSqlGuid(0);
                            m_List.Add(g);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class Pupils_on_Role
    {
        public ArrayList m_list = new ArrayList();
        public int m_count;
        public Pupils_on_Role()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            m_count = 0;
            Utility u = new Utility();
            //System.Configuration.AppSettingsReader ar=new AppSettingsReader();
            //db_connection=ar.GetValue("db",typeof(string)).ToString();
            string s = " SELECT     tbl_Core_Students.StudentId AS Expr1, tbl_Core_Students.StudentAdmissionNumber AS Expr2, ";
            s += "tbl_Core_People.PersonGivenName, tbl_Core_People.PersonSurname, tbl_Core_Students.StudentUPN";
            s += "  FROM tbl_Core_Students INNER JOIN tbl_Core_People ON tbl_Core_Students.StudentPersonId = tbl_Core_People.PersonId";
            s += " WHERE (tbl_Core_Students.StudentIsOnRole = 1)";
            s += " ORDER BY dbo.tbl_Core_People.PersonSurname ASC, dbo.tbl_Core_People.PersonGivenName ASC";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SimplePupil p = new SimplePupil();
                            m_list.Add(p);
                            p.m_StudentId = dr.GetGuid(0);
                            p.m_adno = dr.GetInt32(1);
                            p.m_GivenName = dr.GetString(2);
                            p.m_Surname = dr.GetString(3);
                            if (!dr.IsDBNull(4)) p.m_upn = dr.GetString(4); else p.m_upn = "";
                            m_count++;
                            p.m_valid = false;  // only some fields...
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    public class Organisation
    {
        public Guid m_OrganisationId;
        public string m_OrganisationName;
        public string m_OrganisationFriendlyName;
        public int m_OrganisationTypeID;
        public string m_OrganisationTypeDesc;
        public string m_OrganisationFriendlyNameDesc;
        public bool m_valid = false;
        //now provide constructor
        public Organisation()
        {
            m_OrganisationId = Guid.Empty; m_OrganisationTypeID = 0;//unknown
            m_valid = false;
        }

        public void Hydrate(SqlDataReader dr)
        {
            m_OrganisationId = dr.GetGuid(0);
            m_OrganisationName = dr.GetString(1);
            if (!dr.IsDBNull(2)) m_OrganisationFriendlyName = dr.GetString(2);
            if (!dr.IsDBNull(3)) m_OrganisationTypeID = dr.GetInt32(3);
            m_valid = true;
        }
        public void Load(Guid OrganisationId)
        {
            m_valid = false;
            Encode en = new Encode(); string db_connection = en.GetDbConnection();
            string s = "SELECT   OrganisationId, OrganisationName, OrganisationFriendlyName, OrganisationType  ";
            s += " FROM   dbo.tbl_Core_Organisations  ";
            s += " WHERE ( OrganisationId = '" + OrganisationId.ToString() + "' ) ";
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
            if (m_valid)
            {
                if (m_OrganisationTypeID > 0)
                {
                    s = "SELECT *  FROM   tbl_List_OrganisationTypes  ";
                    s += " WHERE (id='" + m_OrganisationTypeID.ToString() + "' )  ";
                    using (SqlConnection cn = new SqlConnection(db_connection))
                    {
                        cn.Open();
                        using (SqlCommand cm = new SqlCommand(s, cn))
                        {
                            using (SqlDataReader dr = cm.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    m_OrganisationTypeDesc = dr.GetString(1);
                                    if (!dr.IsDBNull(2)) m_OrganisationFriendlyNameDesc = dr.GetString(2);
                                }
                                dr.Close();
                            }
                        }
                        cn.Close();
                    }
                }
            }
        }


        public void Load(string OrganisationFriendlyName)
        {
            m_valid = false;
            Encode en = new Encode(); string db_connection = en.GetDbConnection();
            string s = "SELECT   OrganisationId, OrganisationName, OrganisationFriendlyName, OrganisationType  ";
            s += " FROM         dbo.tbl_Core_Organisations  ";
            s += " WHERE ( OrganisationFriendlyName = '" + OrganisationFriendlyName + "' ) ";
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
            if (m_valid)
            {
                if (m_OrganisationTypeID > 0)
                {
                    s = "SELECT *  FROM   tbl_List_OrganisationTypes  ";
                    s += " WHERE (id='" + m_OrganisationTypeID.ToString() + "' )  ";
                    using (SqlConnection cn = new SqlConnection(db_connection))
                    {
                        cn.Open();
                        using (SqlCommand cm = new SqlCommand(s, cn))
                        {
                            using (SqlDataReader dr = cm.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    m_OrganisationTypeDesc = dr.GetString(1);
                                    if (!dr.IsDBNull(2)) m_OrganisationFriendlyNameDesc = dr.GetString(2);
                                }
                                dr.Close();
                            }
                        }
                        cn.Close();
                    }
                }
            }
        }
        public void Save()
        {
            string s = "";
            if (m_OrganisationId == Guid.Empty)
            {
                m_OrganisationId = Guid.NewGuid();
                // make a new record..
                s = "INSERT INTO dbo.tbl_Core_Organisations (OrganisationId, OrganisationName, OrganisationFriendlyName, OrganisationType, Version ) ";
                s += "VALUES ( '" + m_OrganisationId.ToString() + "' , '" + m_OrganisationName + "' , '" + m_OrganisationFriendlyName + "' ,";
                s += " '" + m_OrganisationTypeID.ToString() + "' , '6' )";
            }
            else
            {
                s = "UPDATE dbo.tbl_Core_Organisations SET OrganisationName ='" + m_OrganisationName + "', OrganisationFriendlyName='" + m_OrganisationFriendlyName + "' , OrganisationType ='" + m_OrganisationTypeID.ToString() + "',  Version ='2' ";
                s += " WHERE (OrganisationId ='" + m_OrganisationId.ToString() + "' )";
            }
            Encode en = new Encode();
            en.ExecuteSQL(s);
        }

        public void Delete()
        {
            if (m_OrganisationId == Guid.Empty) return;

            string s = "DELETE FROM dbo.tbl_Core_Organisations ";
            s += " WHERE (dbo.tbl_Core_Organisations.OrganisationId = '" + m_OrganisationId.ToString() + "') ";
            Encode en = new Encode();
            en.ExecuteSQL(s);

        }

    }
    public class OrganisationList
    {
        public ArrayList _OrganisationList = new ArrayList();
        public OrganisationList(bool load)
        {
            if (load) Load_List();
        }
        public OrganisationList()
        {
            Load_List();
        }
        public void Load_List()
        {
            _OrganisationList.Clear();
            OrganisationTypeList oganTlist1 = new OrganisationTypeList();
            oganTlist1.Load_List();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT   OrganisationId, OrganisationName, OrganisationFriendlyName, OrganisationType  ";
            s += " FROM         dbo.tbl_Core_Organisations  ";
            s += " ORDER BY OrganisationName ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Organisation v = new Organisation();
                            _OrganisationList.Add(v);
                            v.Hydrate(dr);
                            if ((v.m_valid) && (v.m_OrganisationTypeID > 0))
                            {
                                foreach (OganisationType t in oganTlist1._OrganisationTypeList)
                                {
                                    if (t.m_OrganisationID == v.m_OrganisationTypeID)
                                    {
                                        v.m_OrganisationTypeDesc = t.m_OrganisationType;
                                        v.m_OrganisationFriendlyNameDesc = t.m_OrganisationFriendlyNameDescription;
                                    }
                                }
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class OganisationType
    {
        public int m_OrganisationID;
        public string m_OrganisationType;
        public string m_OrganisationFriendlyNameDescription;

        public OganisationType()
        {
        }

        public void Hydrate(SqlDataReader dr)
        {
            m_OrganisationID = dr.GetInt32(0);
            m_OrganisationType = dr.GetString(1);
            if (!dr.IsDBNull(2)) m_OrganisationFriendlyNameDescription = dr.GetString(2);
        }
    }
    public class OrganisationTypeList
    {
        public ArrayList _OrganisationTypeList = new ArrayList();

        public void Load_List()
        {
            _OrganisationTypeList.Clear();
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT *  FROM   tbl_List_OrganisationTypes  ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OganisationType v = new OganisationType();
                            _OrganisationTypeList.Add(v);
                            v.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class OrganisationalUnit
    {
        public Guid m_OrganisationalUnitId;
        public string m_OrganisationalUnitName;
        public Guid m_OrganisationalUnitHead;//is staff id
        public Guid m_OrganisationalUnitParent;
        public int m_Version;
        public bool m_valid;

        public OrganisationalUnit()
        {
            m_OrganisationalUnitId = Guid.Empty;
        }

        public void Hydrate(SqlDataReader dr)
        {
            m_OrganisationalUnitId = dr.GetGuid(0);
            m_OrganisationalUnitName = dr.GetString(1);
            m_OrganisationalUnitHead = dr.GetGuid(2);
            m_OrganisationalUnitParent = dr.GetGuid(3);
            m_valid = true;
        }

        public void Load(string OrganisationalUnitName)
        {
            m_valid = false;
            Encode en = new Encode(); string db_connection = en.GetDbConnection();

            string s = "SELECT   OrganisationalUnitId, OrganisationalUnitName, OrganisationalUnitHead, OrganisationalUnitParent,Version ";
            s += " FROM         dbo.tbl_Core_OrganisationalUnits  ";
            s += " WHERE ( OrganisationalUnitName = '" + OrganisationalUnitName + "' ) ";
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
        public void Load(Guid OrganisationalUnitId)
        {
            m_valid = false;
            Encode en = new Encode(); string db_connection = en.GetDbConnection();

            string s = "SELECT   OrganisationalUnitId, OrganisationalUnitName, OrganisationalUnitHead, OrganisationalUnitParent,Version ";
            s += " FROM         dbo.tbl_Core_OrganisationalUnits  ";
            s += " WHERE ( OrganisationalUnitId = '" + OrganisationalUnitId.ToString() + "' ) ";
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


    }



    public class OrganisationalUnitList
    {
        public ArrayList _OrganisationalUnitList = new ArrayList();
        public OrganisationalUnitList(bool load)
        {
            if (load) Load_List();
        }
        public OrganisationalUnitList()
        {
            Load_List();
        }
        public void Load_List()
        {
            _OrganisationalUnitList.Clear();

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            string s = "SELECT   OrganisationalUnitId, OrganisationalUnitName, OrganisationalUnitHead, OrganisationalUnitParent,Version ";
            s += " FROM         dbo.tbl_Core_OrganisationalUnits  ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            OrganisationalUnit v = new OrganisationalUnit();
                            _OrganisationalUnitList.Add(v);
                            v.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }


    #endregion

    #region ISAMS

    #region TimetableManager



    public class ISAMS_Staff
    {
        public int Id;
        public int PersonId;
        public string PreviousMISId;
        public string Initials;
        public string UserCode;
        public string Title;
        public string FirstName;
        public string Surname;
        public DateTime DoB;
        public string email;

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            if (!dr.IsDBNull(3)) PersonId = dr.GetInt32(3);
            if (!dr.IsDBNull(4)) PreviousMISId = dr.GetString(4);
            if (!dr.IsDBNull(5)) Initials = dr.GetString(5);
            if (!dr.IsDBNull(6)) UserCode = dr.GetString(6);
            if (!dr.IsDBNull(7)) Title = dr.GetString(7);
            if (!dr.IsDBNull(8)) FirstName = dr.GetString(8);
            if (!dr.IsDBNull(10)) Surname = dr.GetString(10);
            if (!dr.IsDBNull(17)) DoB = dr.GetDateTime(17);
            if (!dr.IsDBNull(44)) email = dr.GetString(44);

        }

        public int Load(string StaffUserCode)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT *  FROM TblStaff  WHERE (User_Code = '" + StaffUserCode + "')";
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
            return Id;

        }

        public int LoadStaffInitials(string StaffInitials)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT *  FROM TblStaff  WHERE (Initials = '" + StaffInitials + "')";
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
            return Id;

        }
    }

    public class ISAMS_StaffList
    {
        public List<ISAMS_Staff> m_list = new List<ISAMS_Staff>();
        public ISAMS_StaffList()
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT *  FROM TblStaff ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Staff d = new ISAMS_Staff();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

    }

    public class ISAMS_StaffTeachingList
    {
        public List<ISAMS_Staff> m_list = new List<ISAMS_Staff>();
        public ISAMS_StaffTeachingList()
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT * FROM TblStaff WHERE(TeachingStaff = 1) AND(LeavingDate IS NULL)";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Staff d = new ISAMS_Staff();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }



    public class ISAMS_Room
    {
        public int Id;
        public string Name;
        public string Code;

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            Name = dr.GetString(1);
            Code = dr.GetString(4);
        }
    }

    public class ISAMS_RoomList
    {
        public List<ISAMS_Room> m_list = new List<ISAMS_Room>();

        public ISAMS_RoomList()
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT *  FROM TblSchoolManagementClassrooms ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Room d = new ISAMS_Room();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }


    //TODO  LNA is doing this
    [DataContract]
    public class ISAMS_ExamSeatingPlan
    {
        [DataMember(Order = 1)]  public string SchoolID;
        [DataMember(Order = 2)]  public string candidateName;
        [DataMember(Order = 3)]  public string candidateSurname;
        [DataMember(Order = 4)]  public string candidateNumber;
        [DataMember(Order = 5)]  public string UCI;
        [DataMember(Order = 6)]  public int NCYear;
        [DataMember(Order = 7)]  public string txtForm;
        [DataMember(Order = 8)]  public int CycleID;
        [DataMember(Order = 9)]  public string Programme;
        [DataMember(Order = 10)] public string Subject;
        [DataMember(Order = 11)] public string ComponentCode;
        [DataMember(Order = 12)] public string ComponentTitle;
        [DataMember(Order = 13)] public string OptionCode;
        [DataMember(Order = 14)] public string Qualification;
        [DataMember(Order = 15)] public string StartTimeAsString = "";
        [DataMember(Order = 16)] public string EndTimeAsString = "";
        [DataMember(Order = 17)] public int TimeAllowed;
        [DataMember(Order = 18)] public string TimetabledDateAsString = "";
        [DataMember(Order = 19)] public string GivenSeat;
        [DataMember(Order = 20)] public string RoomCode;
        [DataMember(Order = 21)] public string RoomName;
        [DataMember(Order = 22)] public bool IsTimetabledDateValid = false;
        [DataMember(Order = 23)] public bool IsStartTimeValid = false;
        [DataMember(Order = 24)] public bool IsEndTimeValid = false;
        [DataMember(Order = 25)] public DateTime StartTime;
        [DataMember(Order = 26)] public DateTime EndTime;
        [DataMember(Order = 27)] public DateTime TimetabledDate;

        public string formatString = "d/M/yy";



        public void Hydrate(SqlDataReader dr)
        {
            SchoolID = dr.GetString(0);
            if (!dr.IsDBNull(1)) candidateName = dr.GetString(1); else candidateName = "";
            if (!dr.IsDBNull(2)) candidateSurname = dr.GetString(2); else candidateSurname = "";
            if (!dr.IsDBNull(3)) candidateNumber = dr.GetString(3); else candidateNumber = "";
            if (!dr.IsDBNull(4)) UCI = dr.GetString(4);
            if (!dr.IsDBNull(5)) NCYear = dr.GetInt32(5);
            if (!dr.IsDBNull(6)) txtForm = dr.GetString(6); else txtForm = "";
            if (!dr.IsDBNull(7)) CycleID = dr.GetInt32(7);
            if (!dr.IsDBNull(8)) Programme = dr.GetString(8); else Programme = "";
            if (!dr.IsDBNull(9)) Subject = dr.GetString(9); else Subject = "";
            if (!dr.IsDBNull(10)) ComponentCode = dr.GetString(10); else ComponentCode = "";
            if (!dr.IsDBNull(11)) ComponentTitle = dr.GetString(11); else ComponentTitle = "";
            if (!dr.IsDBNull(12)) OptionCode = dr.GetString(12); else OptionCode = "";
            if (!dr.IsDBNull(13)) Qualification = dr.GetString(13); else Qualification = "";
            if (!dr.IsDBNull(14)) { StartTime = dr.GetDateTime(14); IsStartTimeValid = true; StartTimeAsString = StartTime.ToShortTimeString(); }
            if (!dr.IsDBNull(15)) { EndTime = dr.GetDateTime(15); IsEndTimeValid = true; EndTimeAsString = EndTime.ToShortTimeString(); }
            if (!dr.IsDBNull(16)) TimeAllowed = dr.GetInt32(16);
            if (!dr.IsDBNull(17)) { TimetabledDate = dr.GetDateTime(17); IsTimetabledDateValid = true; TimetabledDateAsString = StartTime.ToString(formatString); }
            if (!dr.IsDBNull(18)) GivenSeat = dr.GetString(18); else GivenSeat = "";
            if (!dr.IsDBNull(19)) RoomCode = dr.GetString(19); else RoomCode = "";
            if (!dr.IsDBNull(20)) RoomName = dr.GetString(20); else RoomName = "";
        }
    }
    [DataContract]
    public class ISAMS_ExamSeatingPlan_List
    {
        public ISAMS_ExamSeatingPlan_List()
        {
        }

        [DataMember]  public List<ISAMS_ExamSeatingPlan> m_list = new List<ISAMS_ExamSeatingPlan>();
        public void Load(int ExamCycle)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT VwExamManagerStudentExams.SchoolID, VwExamManagerStudentExams.CandidateForenames, VwExamManagerStudentExams.CandidateSurname, ";
            s += " VwExamManagerStudentExams.CandidateNumber,  VwExamManagerStudentExams.UCI, VwExamManagerStudentExams.NCYear, ";
            s += " VwExamManagerStudentExams.Form, VwExamManagerStudentExams.CycleID, VwExamManagerStudentExams.Programme, ";
            s += " VwExamManagerStudentExams.Subject, VwExamManagerStudentExams.ComponentCode, VwExamManagerStudentExams.ComponentTitle , ";
            s += " VwExamManagerStudentExams.OptionCode, VwExamManagerStudentExams.Qualification, ";
            s += " VwExamManagerStudentExams.ExamStart,  ";
            s += " VwExamManagerStudentExams.ExamEnd,  ";
            s += " VwExamManagerStudentExams.TimeAllowed, ";
            s += "  VwExamManagerStudentExamSeats.dTimetabledDate, VwExamManagerStudentExamSeats.txtGivenSeat, ";
            s += " TblExamManagerRoomConfigurations.txtRoomCode, UPPER(TblExamManagerRoomConfigurations.txtRoomName) ";
            s += " FROM VwExamManagerStudentExams ";
            s += " INNER JOIN  VwExamManagerStudentExamSeats ON VwExamManagerStudentExams.CycleID = VwExamManagerStudentExamSeats.intCycle ";
            s += " AND VwExamManagerStudentExams.SchoolID = VwExamManagerStudentExamSeats.txtSchoolId ";
            s += " AND VwExamManagerStudentExams.ComponentCode = VwExamManagerStudentExamSeats.txtComponentCode ";
            s += " INNER JOIN TblExamManagerRoomConfigurations ON VwExamManagerStudentExamSeats.intRoomConfigId = TblExamManagerRoomConfigurations.TblExamManagerRoomConfigurationsID ";
            s += " WHERE(VwExamManagerStudentExams.CycleID = '" + ExamCycle.ToString() + "') ";
            s += " ORDER BY VwExamManagerStudentExams.CandidateNumber,VwExamManagerStudentExamSeats.dTimetabledDate, VwExamManagerStudentExams.ExamStart ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_ExamSeatingPlan d = new ISAMS_ExamSeatingPlan();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }



    }

    [Serializable]
    public class ISAMS_ExamComponent
    {
        public int m_ComponentID;
        public string m_ComponentCode;
        public string m_ComponentTitle;
        public string m_ExamBoardCode;
        public string m_MaximumMark;
        public string m_Teachermarks;
        public string m_Due_Date;
        public string m_Timetabled;
        public string m_TimetableDate;
        public string m_TimetableSession;
        public int m_cycle;
        public string m_TimeAllowed;


        public void Load(string ComponentCode, int Cycle, string UABCode)
        {
            string s = " WHERE (dbo.TblExamManagerBaseDataComponents.txtComponentCode='" + ComponentCode + "')";
            s += " AND  (dbo.TblExamManagerBaseDataComponents.intCycle='" + Cycle.ToString() + "')";
            s += " AND  (dbo.TblExamManagerBaseDataComponents.intUABCode='" + UABCode + "')";
            GetQuery(s);
            Load1(GetQuery(s));
        }


        public void Hydrate(SqlDataReader dr)
        {
            m_ComponentID = dr.GetInt32(0);
            m_ComponentCode = dr.GetString(1);
            m_ComponentTitle = dr.GetString(2);
            m_ExamBoardCode = dr.GetString(3);
            if (!dr.IsDBNull(4)) m_MaximumMark = dr.GetString(4);
            if (!dr.IsDBNull(5)) m_Teachermarks = dr.GetString(5);
            if (!dr.IsDBNull(6)) m_Due_Date = dr.GetString(6);
            if (!dr.IsDBNull(7)) m_Timetabled = dr.GetString(7);
            if (!dr.IsDBNull(8)) m_TimetableDate = dr.GetString(8);
            if (!dr.IsDBNull(9)) m_TimetableSession = dr.GetString(9);
            m_cycle = dr.GetInt32(10);
            if (!dr.IsDBNull(11)) m_TimeAllowed = dr.GetString(11); else m_TimeAllowed = "0";

        }


        public string GetQuery(string where)
        {
            string s = " SELECT dbo.TblExamManagerBaseDataComponents.TblExamManagerBaseDataComponentsID, dbo.TblExamManagerBaseDataComponents.txtComponentCode, ";
            s += " dbo.TblExamManagerBaseDataComponents.txtComponentTitle, dbo.TblExamManagerBaseDataComponents.intUABCode, ";
            s += " dbo.TblExamManagerBaseDataComponents.txtMaximumMark, dbo.TblExamManagerBaseDataComponents.txtTeacherMarks, ";
            s += " dbo.TblExamManagerBaseDataComponents.txtDueDate, dbo.TblExamManagerBaseDataComponents.txtTimetabled, ";
            s += " dbo.TblExamManagerBaseDataComponents.txtFormatTimetableDate, dbo.TblExamManagerBaseDataComponents.txtTimetableSession, ";
            s += " dbo.TblExamManagerBaseDataComponents.intCycle  , txtTimeAllowed  ";
            s += " FROM            dbo.TblExamManagerBaseDataComponents INNER JOIN ";
            s += "  dbo.TblExamManagerCycles ON dbo.TblExamManagerBaseDataComponents.intCycle = dbo.TblExamManagerCycles.TblExamManagerCyclesID ";
            s += where;

            return s;
        }

        public void Load1(string q)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(q, cn))
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

    }

    [Serializable]
    [DataContract]
    public class ISAMS_ExamLink
    {
        public int m_Id;
        public int m_cycle;
        public string m_UABCode;
        public string m_OptionCode;
        public string m_ComponentCode;

        public void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetInt32(1);
            m_cycle = dr.GetInt32(2);
            m_UABCode = dr.GetString(3);
            m_OptionCode = dr.GetString(4);
            m_ComponentCode = dr.GetString(5);
        }
    }

    public class ISAMS_ExamLink_List
    {
        public List<ISAMS_ExamLink> m_list = new List<ISAMS_ExamLink>();

        public void LoadList_Component(string ComponentCode, int UABCode, int Cycle)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            m_list.Clear();
            string s = "SELECT * FROM TblExamManagerBaseDataLink WHERE (txtComponentCode='" + ComponentCode + "' )  AND (intCycle='" + Cycle.ToString() + "') ";
            s += " AND ( intUABCode ='" + UABCode.ToString() + "')  ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_ExamLink p = new ISAMS_ExamLink();
                            m_list.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList_Option(string OptionCode, string UABCode, int Cycle)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            m_list.Clear();
            string s = "SELECT * FROM TblExamManagerBaseDataLink WHERE (txtOptionCode='" + OptionCode + "' )  AND (intCycle='" + Cycle.ToString() + "') ";
            s += " AND ( intUABCode ='" + UABCode + "')  ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_ExamLink p = new ISAMS_ExamLink();
                            m_list.Add(p);
                            p.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }


    [Serializable]
    [DataContract]
    public class ISAMS_ExamOption
    {
        [DataMember]public string m_Syllabus_Code;
        [DataMember]public string m_Syllabus_Title;
        [DataMember]public string m_ExamBoardCode;
        [DataMember]public string m_OptionCode;
        [DataMember]public string m_OptionTitle;
        [DataMember]public string m_OptionQualification;
        [DataMember]public string m_OptionLevel;
        [DataMember]public string m_Item;
        [DataMember]public string m_Process;
        [DataMember]public string m_QCACode;
        [DataMember]public string m_QCANumber;
        [DataMember]public string m_fee;
        [DataMember]public string m_OptionMaximumMark;
        [DataMember]public int m_cycle;
        [DataMember]public string m_year_Code;
        [DataMember]public int m_Season_code;
        [DataMember]public string m_SeriesIdentifier;//has to come from base data....  not sure we care.....
        [DataMember]public bool m_valid = false;
        [DataMember]public int m_feeInt;


        public void Hydrate(SqlDataReader dr)
        {
            m_Syllabus_Code = dr.GetString(0);
            m_Syllabus_Title = dr.GetString(1);
            m_ExamBoardCode = dr.GetString(2);
            m_OptionCode = dr.GetString(3);
            m_OptionTitle = dr.GetString(4);
            if (!dr.IsDBNull(5)) m_OptionQualification = dr.GetString(5);
            if (!dr.IsDBNull(6)) m_OptionLevel = dr.GetString(6);
            if (!dr.IsDBNull(7)) m_Item = dr.GetString(7);
            if (!dr.IsDBNull(8)) m_Process = dr.GetString(8);
            if (!dr.IsDBNull(9)) m_QCACode = dr.GetString(9);
            if (!dr.IsDBNull(10)) m_QCANumber = dr.GetString(10);
            if (!dr.IsDBNull(11)) m_fee = dr.GetString(11);
            if (!dr.IsDBNull(12)) m_OptionMaximumMark = dr.GetString(12); else m_OptionMaximumMark = "";
            m_cycle = dr.GetInt32(13);
            m_year_Code = dr.GetString(14);
            m_Season_code = dr.GetInt32(15);
            //m_SeriesIdentifier;//has to come from base data....  not sure we care.....
            m_valid = true;
            if (!dr.IsDBNull(16)) m_feeInt = dr.GetInt32(16);

        }

        public void Load(int cycle, string OptionCode, string UABCode)
        {
            string s = "WHERE ( dbo.TblExamManagerBaseDataOptions.intCycle = '" + cycle.ToString() + "' ) ";
            s += " AND ( dbo.TblExamManagerBaseDataOptions.txtOptionEntryCode = '" + OptionCode + "' )";
            s += " AND ( dbo.TblExamManagerBaseDataOptions.intUABCode = '" + UABCode + "' )";
            Load1(GetQuery(s));
        }

        public void Load1(string q)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(q, cn))
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
        public string GetQuery(string where)
        {
            string s = "SELECT dbo.TblExamManagerBaseDataOptions.txtSyllabusCode, dbo.TblExamManagerBaseDataSyllabus.txtSyllabusName, ";
            s += "dbo.TblExamManagerBaseDataOptions.intUABCode, dbo.TblExamManagerBaseDataOptions.txtOptionEntryCode, dbo.TblExamManagerBaseDataOptions.txtOptionTitle, ";
            s += " dbo.TblExamManagerBaseDataOptions.txtQualification, dbo.TblExamManagerBaseDataOptions.txtLevel, ";
            s += "dbo.TblExamManagerBaseDataOptions.txtItem, dbo.TblExamManagerBaseDataOptions.txtProcess, dbo.TblExamManagerBaseDataOptions.txtClassificationCode, ";
            s += " dbo.TblExamManagerBaseDataOptions.txtAccreditationCode, dbo.TblExamManagerBaseDataOptions.txtFee, dbo.TblExamManagerBaseDataOptions.txtMaxMark, ";
            s += " dbo.TblExamManagerBaseDataOptions.intCycle, dbo.TblExamManagerCycles.intYear, dbo.TblExamManagerCycles.intFormatMonth,  ";
            s += " dbo.TblExamManagerBaseDataOptions.intFee   ";
            s += "  FROM            dbo.TblExamManagerBaseDataOptions INNER JOIN  ";
            s += " dbo.TblExamManagerBaseDataSyllabus ON dbo.TblExamManagerBaseDataOptions.txtSyllabusCode = dbo.TblExamManagerBaseDataSyllabus.txtSyllabusCode INNER JOIN  ";
            s += " dbo.TblExamManagerCycles ON dbo.TblExamManagerBaseDataOptions.intCycle = dbo.TblExamManagerCycles.TblExamManagerCyclesID ";
            s += where;

            return s;
        }
    }

    public class ISAMS_OptionUsed
    {
        public int m_Cycle;
        public string m_Series;
        public string m_OptionCode;
        public string m_UABCode;

        public void Hydrate(SqlDataReader dr)
        {
            m_Cycle = dr.GetInt32(0);
            m_Series = dr.GetString(1);
            m_OptionCode = dr.GetString(2);
            m_UABCode = dr.GetString(3);
        }
    }

    public class ISAMS_OptionsUsed_List
    {
        //get a list of all otpions that have entries for this cycle.
        public List<ISAMS_OptionUsed> m_list = new List<ISAMS_OptionUsed>();
        private void Load1(string query)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(query, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_OptionUsed e = new ISAMS_OptionUsed();
                            m_list.Add(e);
                            e.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void LoadList(int Cycle)
        {
            string s = "SELECT DISTINCT intCycle, txtExaminationSeries, txtModuleCode, intUABCode  FROM TblExamManagerEntries   ";
            s += "WHERE  (intCycle='" + Cycle.ToString() + "') ";
            Load1(s);
        }
    }

    [Serializable]
    public class ISAMS_SimpleExamEntry
    {
        public int m_Id;
        public int m_Cycle;
        public string m_PupilId;
        public string m_Series;
        public string m_OptionCode;
        public string m_UABCode;

        public void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetInt32(0);
            m_Cycle = dr.GetInt32(1);
            m_PupilId = dr.GetString(2);
            m_Series = dr.GetString(4);
            m_OptionCode = dr.GetString(5);
            m_UABCode = dr.GetString(6);
        }
    }

    public class ISAMS_SimpleExamEntry_List
    {
        public List<ISAMS_SimpleExamEntry> m_list = new List<ISAMS_SimpleExamEntry>();
        private string GetQuery(string where)
        {
            string s = "SELECT  TblExamManagerEntriesID, intCycle, txtSchoolID, intCandidateId, txtExaminationSeries, txtModuleCode, intUABCode  ";
            s += " FROM  TblExamManagerEntries  ";
            return (s + where);
        }

        private void Load1(string query)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(query, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_SimpleExamEntry e = new ISAMS_SimpleExamEntry();
                            m_list.Add(e);
                            e.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadList(int Cycle, int UABCode, string PupilId)
        {
            string s = "WHERE  (intCycle='" + Cycle.ToString() + "') ";
            s += " AND ( intUABCode ='" + UABCode.ToString() + "')  ";
            s += " AND ( txtSchoolID ='" + PupilId + "')  ";
            s = GetQuery(s);
            Load1(s);
        }

        public void LoadList(int Cycle, int UABCode)
        {
            string s = "WHERE  (intCycle='" + Cycle.ToString() + "') ";
            s += " AND ( intUABCode ='" + UABCode.ToString() + "')  ";
            s = GetQuery(s);
            Load1(s);
        }

        public void LoadList(int Cycle)
        {
            string s = "WHERE  (intCycle='" + Cycle.ToString() + "') ";
            s = GetQuery(s);
            Load1(s);

        }
    }


    [DataContract]
    public class ISAMS_ExamCycle
    {
        [DataMember]
        public int TblExamManagerCyclesID;
        [DataMember]
        public string txtCentreId;
        [DataMember]
        public int intFormatMonth;
        [DataMember]
        public string intYear;
        [DataMember]
        public int intFormatYear;



        public void Hydrate(SqlDataReader dr)
        {
            TblExamManagerCyclesID = dr.GetInt32(0);
            txtCentreId = dr.GetString(1);
            intFormatMonth = dr.GetInt32(2);
            intYear = dr.GetString(3);
            intFormatYear = dr.GetInt32(4);

        }
    }

    [DataContract]
    public class ISAMS_ExamCycleList
    {
        [DataMember]
        public List<ISAMS_ExamCycle> m_list = new List<ISAMS_ExamCycle>();

        protected string GetQuery()
        {
            string s = " SELECT TblExamManagerCyclesID, txtCentreId, intFormatMonth, intYear, intFormatYear FROM TblExamManagerCycles ";
            return s;
        }

        public void load()
        {
            string s = GetQuery();

            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_ExamCycle d = new ISAMS_ExamCycle();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }


    [Serializable]
    [DataContract]
    public class ISAMS_ExamEntry
    {
        [DataMember] public string PupilId;
        [DataMember] public string OptionCode;
        [DataMember] public string ComponentCode;
        [DataMember] public string OptionTitle;
        [DataMember] public string ComponentTitle;
        [DataMember] public string UABName;
        [DataMember] public string GivenSeat;
        [DataMember] public string RoomName;
        [DataMember] public DateTime dDateTime;
        [DataMember] public string Date;
        [DataMember] public string Time;


        public void Hydrate(SqlDataReader dr)
        {
            PupilId = dr.GetString(0);
            OptionCode = dr.GetString(1);
            ComponentCode = dr.GetString(2);
            OptionTitle = dr.GetString(3);
            ComponentTitle = dr.GetString(4);
            UABName = dr.GetString(5);
            GivenSeat = dr.GetString(6);
            RoomName = dr.GetString(7);
            dDateTime = dr.GetDateTime(8);
            Date = dDateTime.ToShortDateString();
            Time = dDateTime.ToShortTimeString();
        }


    }

    [DataContract]
    public class ISAMS_ExamsEntry_List
    {
        [DataMember]
        public List<ISAMS_ExamEntry> m_list = new List<ISAMS_ExamEntry>();
        [DataMember]
        public ArrayList m_ISAMS_EntriesList = new ArrayList();


        private string GetQuery(string where)
        {
            string s = " SELECT dbo.TblExamManagerEntries.txtSchoolID, dbo.TblExamManagerBaseDataLink.txtOptionCode, dbo.TblExamManagerBaseDataComponents.txtComponentCode, ";
            s += " dbo.TblExamManagerBaseDataOptions.txtOptionTitle, dbo.TblExamManagerBaseDataComponents.txtComponentTitle, dbo.TblExamsManagerUABIdentifiers.txtUABName, ";
            s += " dbo.TblExamManagerSeatingPlanArrangement.txtGivenSeat, dbo.TblExamManagerRoomConfigurations.txtRoomName, dbo.TblExamManagerSeatingPlanExam.dScheduledDateTime ";
            s += "FROM            dbo.TblExamManagerEntries INNER JOIN ";
            s += "  dbo.TblExamManagerBaseDataOptions ON dbo.TblExamManagerEntries.txtModuleCode = dbo.TblExamManagerBaseDataOptions.txtOptionEntryCode AND ";
            s += "  dbo.TblExamManagerEntries.intCycle = dbo.TblExamManagerBaseDataOptions.intCycle INNER JOIN ";
            s += "  dbo.TblExamManagerBaseDataLink ON dbo.TblExamManagerBaseDataOptions.txtOptionEntryCode = dbo.TblExamManagerBaseDataLink.txtOptionCode AND ";
            s += "  dbo.TblExamManagerEntries.intCycle = dbo.TblExamManagerBaseDataLink.intCycle INNER JOIN ";
            s += "  dbo.TblExamManagerBaseDataComponents ON dbo.TblExamManagerBaseDataLink.txtComponentCode = dbo.TblExamManagerBaseDataComponents.txtComponentCode AND ";
            s += "  dbo.TblExamManagerEntries.intCycle = dbo.TblExamManagerBaseDataComponents.intCycle INNER JOIN ";
            s += "  dbo.TblExamsManagerUABIdentifiers ON dbo.TblExamManagerBaseDataOptions.intUABCode = dbo.TblExamsManagerUABIdentifiers.intUABCode INNER JOIN ";
            s += "  dbo.TblExamManagerSeatingPlanExam ON dbo.TblExamManagerBaseDataComponents.txtComponentCode = dbo.TblExamManagerSeatingPlanExam.txtComponentCode INNER JOIN ";
            s += "  dbo.TblExamManagerSeatingPlanExamLink ON dbo.TblExamManagerSeatingPlanExam.TblExamManagerSeatingPlanExamId = dbo.TblExamManagerSeatingPlanExamLink.intExamId INNER JOIN ";
            s += "  dbo.TblExamManagerSeatingPlanArrangement ON dbo.TblExamManagerSeatingPlanExamLink.intArrangementId = dbo.TblExamManagerSeatingPlanArrangement.TblExamManagerSeatingPlanArrangementId AND ";
            s += "  dbo.TblExamManagerEntries.txtSchoolID = dbo.TblExamManagerSeatingPlanArrangement.txtSchoolId INNER JOIN ";
            s += "  dbo.TblExamManagerSeatingPlan ON dbo.TblExamManagerSeatingPlanExam.intPlanId = dbo.TblExamManagerSeatingPlan.TblExamManagerSeatingPlanId AND ";
            s += "  dbo.TblExamManagerSeatingPlanArrangement.intPlanId = dbo.TblExamManagerSeatingPlan.TblExamManagerSeatingPlanId INNER JOIN ";
            s += "  dbo.TblExamManagerRoomConfigurations ON dbo.TblExamManagerSeatingPlan.intRoomConfigId = dbo.TblExamManagerRoomConfigurations.TblExamManagerRoomConfigurationsID   ";
            return s + where;
        }

        private void Load1(string query)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(query, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_ExamEntry e = new ISAMS_ExamEntry();
                            m_list.Add(e);
                            m_ISAMS_EntriesList.Add(e);
                            e.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void Load(string ISAMS_cycle)
        {
            Load1(GetQuery(" WHERE (dbo.TblExamManagerEntries.intCycle = " + ISAMS_cycle + ") "));
        }

        public void Load(string ISAMS_Id, string ISAMS_cycle)
        {
            if (ISAMS_cycle != "0")
            {
                Load1(GetQuery(" WHERE(dbo.TblExamManagerEntries.txtSchoolID = N'" + ISAMS_Id + "') AND(dbo.TblExamManagerEntries.intCycle = " + ISAMS_cycle + ")  ORDER BY dbo.TblExamManagerEntries.intCycle DESC, dbo.TblExamManagerSeatingPlanExam.dScheduledDateTime  "));
            }
            else
            {
                Load1(GetQuery(" WHERE(dbo.TblExamManagerEntries.txtSchoolID = N'" + ISAMS_Id + "')   ORDER BY dbo.TblExamManagerEntries.intCycle DESC, dbo.TblExamManagerSeatingPlanExam.dScheduledDateTime "));// for all cycles
            }
        }
    }


    public class ISAMS_ExamSyllabus
    {
        public int m_Id;
        public int m_cycle;
        public string m_SyllabusCode;
        public string m_SyllabusTitle;
        public string m_UABCode;

        public void Hydrate(SqlDataReader dr)
        {
            m_Id = dr.GetInt32(0);
            m_SyllabusCode = dr.GetString(2);
            m_SyllabusTitle = dr.GetString(3);
            m_UABCode = dr.GetString(6);
            m_cycle = dr.GetInt32(5);
        }

        public void Load(int Cycle, string UABCode, string SyllabusCode)
        {
            string s = " SELECT * FROM            TblExamManagerBaseDataSyllabus ";
            s += " WHERE ( intCycle = '" + Cycle.ToString() + "' ) ";
            s += " AND ( txtSyllabusCode = '" + SyllabusCode + "' )";
            s += " AND ( intUABCode = '" + UABCode + "' )";
            Load1(s);
        }

        public void Load1(string q)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(q, cn))
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

    }



    [DataContract]
    public class ISAMS_ExamStudentAccess
    {
        [DataMember(Order = 1)]
        public string iSAMS_SchoolId; //note is really studentId
        [DataMember(Order = 2)]
        public string candidateNo; //candidate number
        [DataMember(Order = 3)]
        public string candidateName; //candidate forename
        [DataMember(Order = 4)]
        public string candidateSurname; //candidate surename
        [DataMember(Order = 5)]
        public int NCYear;
        [DataMember(Order = 6)]
        public string txtForm;
        [DataMember(Order = 7)]
        public int intCycleId;
        [DataMember(Order = 8)]
        public int intSystemStatus;
        [DataMember(Order = 9)]
        public string ExtraTime = "";
        [DataMember(Order = 10)]
        public string Processor = "";
        [DataMember(Order = 11)]
        public string Rest = "";
        [DataMember(Order = 12)]
        public string Transcript = "";
        [DataMember(Order = 13)]
        public string Dictionary = "";
        [DataMember(Order = 14)]
        public string Prompter = "";
        [DataMember(Order = 15)]
        public string Reader = "";
        [DataMember(Order = 16)]
        public string SeparateInvigilation = "";
        [DataMember(Order = 17)]
        public string Scribe = "";
        [DataMember(Order = 18)]
        public string ColourNaming = "";
        [DataMember(Order = 19)]
        public string examCode;
        [DataMember(Order = 20)]
        public string txtArrangementNotes = "";
        [DataMember(Order = 21)]
        public DateTime startDate;
        [DataMember(Order = 22)]
        public bool IsStartDateValid = false;
        [DataMember(Order = 23)]
        public string StartDateAsString = "";
        [DataMember(Order = 24)]
        public DateTime endDate;
        [DataMember(Order = 25)]
        public bool IsEndDateValid = false;
        [DataMember(Order = 26)]
        public string EndDateAsString = "";



        public void Hydrate(SqlDataReader dr)
        {
            iSAMS_SchoolId = dr.GetString(0);
            candidateNo = dr.GetString(1);
            if (!dr.IsDBNull(2)) candidateName = dr.GetString(2); else candidateName = "";
            if (!dr.IsDBNull(3)) candidateSurname = dr.GetString(3); else candidateSurname = "";
            if (!dr.IsDBNull(4)) NCYear = dr.GetInt32(4);
            if (!dr.IsDBNull(5)) txtForm = dr.GetString(5); else txtForm = "";
            if (!dr.IsDBNull(6)) intCycleId = dr.GetInt32(6);
            if (!dr.IsDBNull(7)) intSystemStatus = dr.GetInt32(7);

            if (!dr.IsDBNull(8)) { if (dr.GetInt32(8) > 0) ExtraTime = dr.GetInt32(8).ToString() + "% Extra Time"; }
            if (!dr.IsDBNull(9)) { if (dr.GetInt32(9) == 1) Processor = "Can Type"; }
            if (!dr.IsDBNull(10)) { if (dr.GetInt32(10) == 1) Rest = "Rest Break"; }
            if (!dr.IsDBNull(11)) { if (dr.GetInt32(11) == 1) Transcript = " Transcript  "; }
            if (!dr.IsDBNull(12)) { if (dr.GetInt32(12) == 1) Dictionary = "Dictionary"; }
            if (!dr.IsDBNull(13)) { if (dr.GetInt32(13) == 1) Prompter = " Prompter  "; }
            if (!dr.IsDBNull(14)) { if (dr.GetInt32(14) == 1) Reader = " Reader"; }
            if (!dr.IsDBNull(15)) { if (dr.GetInt32(15) == 1) SeparateInvigilation = " Separate Invigilation  "; }
            if (!dr.IsDBNull(16)) { if (dr.GetInt32(16) == 1) Scribe = "Scribe"; }
            if (!dr.IsDBNull(17)) { if (dr.GetInt32(17) == 1) ColourNaming = "Colour Naming"; }
            if (!dr.IsDBNull(18)) txtArrangementNotes = dr.GetString(18); else txtArrangementNotes = "";

            if (!dr.IsDBNull(19)) { startDate = dr.GetDateTime(19); IsStartDateValid = true; StartDateAsString = startDate.ToLongDateString(); }
            if (!dr.IsDBNull(20)) { endDate = dr.GetDateTime(20); IsEndDateValid = true; EndDateAsString = endDate.ToLongDateString(); }
            if (!dr.IsDBNull(21)) examCode = dr.GetString(21); else examCode = "";

        }
    }
    [DataContract]
    public class ISAMS_ExamStudentAccessList
    {
        [DataMember]
        public List<ISAMS_ExamStudentAccess> m_list = new List<ISAMS_ExamStudentAccess>();
        protected string GetQuery(string where)
        {
            string s = " SELECT DISTINCT VwExamManagerStudentExams.SchoolID, VwExamManagerStudentExams.CandidateNumber, VwExamManagerStudentExams.CandidateForenames, ";
            s += " VwExamManagerStudentExams.CandidateSurname, TblPupilManagementPupils.intNCYear, TblPupilManagementPupils.txtForm,VwExamManagerStudentExams.CycleID, ";
            s += " TblPupilManagementPupils.intSystemStatus, TblExamManagerCandidateOptions.intExtraTime,TblExamManagerCandidateOptions.intProcessor, TblExamManagerCandidateOptions.intRest, ";
            s += " TblExamManagerCandidateOptions.intTranscript, TblExamManagerCandidateOptions.intDictionary, TblExamManagerCandidateOptions.intPrompter, TblExamManagerCandidateOptions.intReader, ";
            s += " TblExamManagerCandidateOptions.intSeparateInvigilation, TblExamManagerCandidateOptions.intScribe, TblExamManagerCandidateOptions.intColourNaming, ";
            s += " TblExamManagerCandidateOptions.txtArrangementNotes, ";
            s += "TblExamManagerCandidateOptions.dteStartDate, TblExamManagerCandidateOptions.dteEndDate, TblExamManagerCandidateOptions.txtExamCode ";
            s += " FROM VwExamManagerStudentExams ";
            s += " INNER JOIN TblPupilManagementPupils ON VwExamManagerStudentExams.SchoolID = TblPupilManagementPupils.txtSchoolID  ";
            s += " LEFT OUTER JOIN TblExamManagerCandidateOptions ON VwExamManagerStudentExams.SchoolID = TblExamManagerCandidateOptions.txtSchoolID ";
            s += where;
            return s;
        }
        public void load(int CycleId)
        {
            string where = " WHERE (VwExamManagerStudentExams.CycleID = '" + CycleId.ToString() + "')";
            string s = GetQuery(where);
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_ExamStudentAccess d = new ISAMS_ExamStudentAccess();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }



    [Serializable]
    public class ISAMS_Period
    {
        public int Id;
        public int day; //mon = 2  I think could be 2 week tt???
        public int order;//starts at 1 ugh
        public string Name;
        public string ShortName;
        public string StartTime;
        public string EndTime;
        public int Week;  // this is the week no (1 or 2 at present)

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            day = dr.GetInt32(1);
            order = dr.GetInt32(2);
            Name = dr.GetString(3);
            ShortName = dr.GetString(4);
            StartTime = dr.GetString(5);
            EndTime = dr.GetString(6);
            Week = dr.GetInt32(7);
        }
    }
    public class ISAMS_Period_List
    {
        public ArrayList m_ISAMS_PeriodList = new ArrayList();
        public ISAMS_Period_List()
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT dbo.TblTimetableManagerPeriods.TblTimetableManagerPeriodsID, dbo.TblTimetableManagerPeriods.intDay, ";
            s += " dbo.TblTimetableManagerPeriods.intOrder, dbo.TblTimetableManagerPeriods.txtName, ";
            s += " dbo.TblTimetableManagerPeriods.txtShortName, dbo.TblTimetableManagerPeriods.txtStartTime, dbo.TblTimetableManagerPeriods.txtEndTime, ";
            s += " dbo.TblTimetableManagerDays.intWeek  ";
            s += " FROM            dbo.TblTimetableManagerDays INNER JOIN  ";
            s += " dbo.TblTimetableManagerPeriods ON dbo.TblTimetableManagerDays.TblTimetableManagerDaysID = dbo.TblTimetableManagerPeriods.intDay";
            s += "  ORDER BY dbo.TblTimetableManagerPeriods.intOrder";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Period d = new ISAMS_Period();
                            m_ISAMS_PeriodList.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

    }


    public class ISAMS_Timetable
    {
        public int Id;
        public string TimetbaleName;
        public string Description;
        public int StartYear;
        public int EndYear;
        public bool Published;

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            TimetbaleName = dr.GetString(1);
            if (!dr.IsDBNull(2)) Description = dr.GetString(2);
            StartYear = dr.GetInt32(3);
            EndYear = dr.GetInt32(4);
            int i = dr.GetInt32(6);
            Published = (i == 0) ? false : true;
        }

        public int LoadPublishedTimetable()
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT *  FROM TblTimetableManagerTimetables  WHERE (intPublished = 1)";
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
            return Id;
        }
    }

    public class ISAMS_TimeTableSchedule
    {
        public int Id;
        public int TimetableId;
        public string SetCode;
        public int PeriodId;
        public string StaffId; // note seems to be GivenName+SurName+int id...as txt !!!
        public int RoomId;
        public string StaffInitials;

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            TimetableId = dr.GetInt32(1);
            SetCode = dr.GetString(2);
            PeriodId = dr.GetInt32(3);
            StaffId = dr.GetString(4);
            RoomId = dr.GetInt32(5);
            StaffInitials = dr.GetString(11);
        }
    }

    public class ISAMS_TimetableScheduleList
    {
        public List<ISAMS_TimeTableSchedule> m_list = new List<ISAMS_TimeTableSchedule>();

        public void LoadListCurrentTT()
        {
            ISAMS_Timetable tt = new ISAMS_Timetable();
            int TTId = tt.LoadPublishedTimetable();
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT TblTimetableManagerSchedule.* , tblStaff.Initials  FROM TblTimetableManagerSchedule  ";
            s += "INNER JOIN TblStaff ON TblTimetableManagerSchedule.txtTeacher = TblStaff.User_Code ";
            s += " WHERE ( intTimetableID='" + TTId.ToString() + "' )";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_TimeTableSchedule d = new ISAMS_TimeTableSchedule();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }


        }
    }

    public class ISAMS_TimetableScheduledPeriod
    {
        public int Day;
        public string PeriodName;
        public string SetName;
        public string RoomCode;
        public string StaffCode;
        public string PreviousMISId;
        public int Week;

        public void Hydrate(SqlDataReader dr)
        {
            Day = dr.GetInt32(0);
            PeriodName = dr.GetString(1);
            SetName = dr.GetString(2);
            RoomCode = dr.GetString(3);
            StaffCode = dr.GetString(4);
            if (!dr.IsDBNull(5)) PreviousMISId = dr.GetString(5); else PreviousMISId = "";
            Week = dr.GetInt32(6);
        }
    }

    public class ISAMS_TimetableScheduledPeriodList
    {
        public List<ISAMS_TimetableScheduledPeriod> m_list = new List<ISAMS_TimetableScheduledPeriod>();

        public void LoadListCurrentTT()
        {
            ISAMS_Timetable tt = new ISAMS_Timetable();
            int TTId = tt.LoadPublishedTimetable();//gets id of current TT
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT dbo.TblTimetableManagerPeriods.intDay, dbo.TblTimetableManagerPeriods.txtShortName, ";
            s += "dbo.TblTimetableManagerSchedule.txtCode, dbo.TblSchoolManagementClassrooms.txtCode AS Room, dbo.TblStaff.Initials, ";
            s += " dbo.TblStaff.txtPreviousMISStaffID, ";
            s += "dbo.TblTimetableManagerDays.intWeek  ";
            s += " FROM dbo.TblTimetableManagerSchedule INNER JOIN ";
            s += " dbo.TblSchoolManagementClassrooms ON dbo.TblTimetableManagerSchedule.intRoom = dbo.TblSchoolManagementClassrooms.TblSchoolManagementClassroomsID INNER JOIN  ";
            s += " dbo.TblTimetableManagerPeriods ON dbo.TblTimetableManagerSchedule.intPeriod = dbo.TblTimetableManagerPeriods.TblTimetableManagerPeriodsID INNER JOIN  ";
            s += " dbo.TblStaff ON dbo.TblTimetableManagerSchedule.txtTeacher = dbo.TblStaff.User_Code  ";
            s += " INNER JOIN ";
            s += " dbo.TblTimetableManagerDays ON dbo.TblTimetableManagerDays.TblTimetableManagerDaysID = dbo.TblTimetableManagerPeriods.intDay";
            s += " WHERE ( intTimetableID='" + TTId.ToString() + "' )";
            s += "AND  (dbo.TblTimetableManagerDays.intWeek='1') ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_TimetableScheduledPeriod d = new ISAMS_TimetableScheduledPeriod();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }


    public class ISAMS_Calender
    {
        public DateTime Calender_Date;
        public int Calender_Year;
        public int Calender_Week;
        public int Calender_WeekDayNumber;
        public int Timetable_Week;

        public void Hydrate(SqlDataReader dr)
        {
            Calender_Date = dr.GetDateTime(0);
            Calender_Year = dr.GetInt32(1);
            Calender_Week = dr.GetInt32(2);
            Calender_WeekDayNumber = dr.GetInt32(3);
            Timetable_Week = dr.GetInt32(4);

        }
    }

    public class ISAMS_CalenderList
    {
        public List<ISAMS_Calender> m_list = new List<ISAMS_Calender>();

        public void LoadList(DateTime YearStartDate)
        {
            YearStartDate.AddDays(-10); // so we get all of week 0  
            DateTime YearEnd = new DateTime(YearStartDate.Year + 1, 7, 31);
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = "SELECT dbo.TblTimetableManagerCalendar.CalendarDate, dbo.TblTimetableManagerCalendar.CalendarYear, ";
            s += " dbo.TblTimetableManagerCalendar.CalendarWeek, dbo.TblTimetableManagerCalendar.WeekDayNumber, ";
            s += "dbo.TblTimetableManagerWeeksAllocations.intWeek ";
            s += " FROM TblTimetableManagerCalendar  ";
            s += "INNER JOIN  dbo.TblTimetableManagerWeeksAllocations ";
            s += " ON dbo.TblTimetableManagerCalendar.CalendarYear = dbo.TblTimetableManagerWeeksAllocations.intYear AND ";
            s += "  dbo.TblTimetableManagerCalendar.CalendarWeek = dbo.TblTimetableManagerWeeksAllocations.intYearWeek ";
            s += " WHERE(CalendarDate > CONVERT(DATETIME, '" + YearStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
            s += " AND(CalendarDate < CONVERT(DATETIME, '" + YearEnd.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)) ";
            s += " ORDER BY CalendarDate  ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Calender d = new ISAMS_Calender();
                            d.Hydrate(dr);
                            //now only want to add if we haven't got this week....
                            bool found = false;
                            foreach (ISAMS_Calender c in m_list)
                            {
                                if (d.Calender_Week == c.Calender_Week) found = true;
                            }
                            if (!found) m_list.Add(d);


                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }

    public class ISAMS_Set_AssociatedTeacher
    {
        public int Id;
        public int SetId;
        public string Staff_UserCode;

        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            SetId = dr.GetInt32(1);
            Staff_UserCode = dr.GetString(2);
        }
    }


    public class ISAMS_Set_AssociatedTeacher_list
    {
        List<ISAMS_Set_AssociatedTeacher> m_list = new List<ISAMS_Set_AssociatedTeacher>();

        public void Load()
        {
            string s = " SELECT        TblTeachingManagerSetAssociatedTeachersID, intSetID, txtTeacher ";
            s += " FROM            TblTeachingManagerSetAssociatedTeachers ";
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Set_AssociatedTeacher d = new ISAMS_Set_AssociatedTeacher();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }

    }

    [DataContract]
    public class ISAMS_Set
    {
        [DataMember] public int Id;
        [DataMember]
        public int Subject;
        [DataMember]
        public int Year;
        [DataMember]
        public string SetNumber;
        [DataMember]
        public string SetCode;
        [DataMember]
        public string Name;
        [DataMember]
        public string Teacher;
        [DataMember]
        public int StaffId;
        [DataMember]
        public string StaffCode;//ISAMS calls initials
        [DataMember]
        public string StaffFirstName;
        [DataMember]
        public string StaffSurname;
        [DataMember]
        public string StaffEmail;
        [DataMember]
        public int SubjectId;
        [DataMember]
        public string SubjectCode;
        [DataMember]
        public string SubjectName;
        [DataMember]
        public string StaffTitle;



        public void Hydrate(SqlDataReader dr)
        {
            Id = dr.GetInt32(0);
            SetCode = dr.GetString(1);
            Name = dr.GetString(2);
            Year = dr.GetInt32(3);
            SetNumber = dr.GetString(4);
            Teacher = dr.GetString(5);
            StaffId = dr.GetInt32(6);
            StaffCode = dr.GetString(7);
            StaffFirstName = dr.GetString(8);
            StaffSurname = dr.GetString(9);
            StaffEmail = dr.GetString(10);
            SubjectId = dr.GetInt32(11);
            SubjectCode = dr.GetString(12);
            SubjectName = dr.GetString(13);
            StaffTitle = dr.GetString(14);
        }

        public void Hydrate(SqlDataReader dr, int offset)
        {
            Id = dr.GetInt32(offset);
            SetCode = dr.GetString(offset + 1);
            Name = dr.GetString(offset + 2);
            Year = dr.GetInt32(offset + 3);
            SetNumber = dr.GetString(offset + 4);
            Teacher = dr.GetString(offset + 5);
            StaffId = dr.GetInt32(offset + 6);
            StaffCode = dr.GetString(offset + 7);
            StaffFirstName = dr.GetString(offset + 8);
            StaffSurname = dr.GetString(offset + 9);
            StaffEmail = dr.GetString(offset + 10);
            SubjectId = dr.GetInt32(offset + 11);
            SubjectCode = dr.GetString(offset + 12);
            SubjectName = dr.GetString(offset + 13);
            StaffTitle = dr.GetString(offset + 14);
        }
    }

    [DataContract]
    public class ISAMS_Set_List
    {
        [DataMember] public List<ISAMS_Set> m_list = new List<ISAMS_Set>();
        public ISAMS_Set_List()
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = " SELECT dbo.TblTeachingManagerSets.TblTeachingManagerSetsID, dbo.TblTeachingManagerSets.txtSetCode, ";
            s += " dbo.TblTeachingManagerSets.txtName, dbo.TblTeachingManagerSets.intYear, dbo.TblTeachingManagerSets.txtSetNumber, ";
            s += "  dbo.TblTeachingManagerSets.txtTeacher, dbo.TblStaff.TblStaffID, dbo.TblStaff.Initials, ";
            s += " dbo.TblStaff.Firstname, dbo.TblStaff.Surname, dbo.TblStaff.SchoolEmailAddress, ";
            s += " dbo.TblTeachingManagerSubjects.TblTeachingManagerSubjectsID, dbo.TblTeachingManagerSubjects.txtSubjectCode, ";
            s += " dbo.TblTeachingManagerSubjects.txtSubjectName ";
            s += ", dbo.TblStaff.Title ";
            s += " FROM dbo.TblTeachingManagerSets INNER JOIN  ";
            s += " dbo.TblStaff ON dbo.TblTeachingManagerSets.txtTeacher = dbo.TblStaff.User_Code INNER JOIN ";
            s += " dbo.TblTeachingManagerSubjects ON dbo.TblTeachingManagerSets.intSubject = dbo.TblTeachingManagerSubjects.TblTeachingManagerSubjectsID  ";
            s += "ORDER BY txtSetCode";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Set d = new ISAMS_Set();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public ISAMS_Set_List(string SubjectCode, int Year)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = " SELECT dbo.TblTeachingManagerSets.TblTeachingManagerSetsID, dbo.TblTeachingManagerSets.txtSetCode, ";
            s += " dbo.TblTeachingManagerSets.txtName, dbo.TblTeachingManagerSets.intYear, dbo.TblTeachingManagerSets.txtSetNumber, ";
            s += "  dbo.TblTeachingManagerSets.txtTeacher, dbo.TblStaff.TblStaffID, dbo.TblStaff.Initials, ";
            s += " dbo.TblStaff.Firstname, dbo.TblStaff.Surname, dbo.TblStaff.SchoolEmailAddress, ";
            s += " dbo.TblTeachingManagerSubjects.TblTeachingManagerSubjectsID, dbo.TblTeachingManagerSubjects.txtSubjectCode, ";
            s += " dbo.TblTeachingManagerSubjects.txtSubjectName ";
            s += ", dbo.TblStaff.Title ";
            s += " FROM dbo.TblTeachingManagerSets INNER JOIN  ";
            s += " dbo.TblStaff ON dbo.TblTeachingManagerSets.txtTeacher = dbo.TblStaff.User_Code INNER JOIN ";
            s += " dbo.TblTeachingManagerSubjects ON dbo.TblTeachingManagerSets.intSubject = dbo.TblTeachingManagerSubjects.TblTeachingManagerSubjectsID  ";
            s += "WHERE (dbo.TblTeachingManagerSubjects.txtSubjectCode ='" + SubjectCode + "') ";
            if (Year > 6) s += "  AND (dbo.TblTeachingManagerSets.intYear = '" + Year.ToString() + "') ";
            s += "ORDER BY txtSetCode";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Set d = new ISAMS_Set();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }
    [DataContract]
    public class ISAMS_Student_Set
    {
        [DataMember]public ISAMS_Student student;
        [DataMember]public ISAMS_Set set;
        public ISAMS_Student_Set()
        {
            student = new ISAMS_Student();
            set = new ISAMS_Set();
        }
        public void Hydrate(SqlDataReader dr)
        {
            student.Hydrate(dr);
            set.Hydrate(dr, 13);
        }
    }

    [DataContract]
    public class ISAMS_Student_Set_List
    {
        [DataMember] public List<ISAMS_Student_Set> m_list = new List<ISAMS_Student_Set>();

        public void Load(string year, bool OnRole)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();

            string s = " SELECT dbo.TblPupilManagementPupils.txtSchoolID, dbo.TblPupilManagementPupils.txtPreviousMISID, ";
            s += " dbo.TblPupilManagementPupils.txtTitle, dbo.TblPupilManagementPupils.txtForename, ";
            s += " dbo.TblPupilManagementPupils.txtSurname, dbo.TblPupilManagementPupils.txtEmailAddress, dbo.TblPupilManagementPupils.txtForm ";
            s += " ,dbo.TblPupilManagementPupils.txtPreName ";
            s += " , dbo.TblPupilManagementPupils.intAutoSchoolCodeNumericPart,  dbo.TblPupilManagementPupils.intSystemStatus, ";
            s += " dbo.TblPupilManagementPupils.txtGender  , dbo.TblPupilManagementPupils.txtDOB, ";
            s += " dbo.TblPupilManagementPupils.intNCYear, ";
            //next bit is set...
            s += " dbo.TblTeachingManagerSets.TblTeachingManagerSetsID, dbo.TblTeachingManagerSets.txtSetCode, ";
            s += " dbo.TblTeachingManagerSets.txtName, dbo.TblTeachingManagerSets.intYear, dbo.TblTeachingManagerSets.txtSetNumber, ";
            s += "  dbo.TblTeachingManagerSets.txtTeacher, dbo.TblStaff.TblStaffID, dbo.TblStaff.Initials, ";
            s += " dbo.TblStaff.Firstname, dbo.TblStaff.Surname, dbo.TblStaff.SchoolEmailAddress, ";
            s += " dbo.TblTeachingManagerSubjects.TblTeachingManagerSubjectsID, dbo.TblTeachingManagerSubjects.txtSubjectCode, ";
            s += " dbo.TblTeachingManagerSubjects.txtSubjectName ";
            //late additions!
            s += ", dbo.TblStaff.Title ";

            s += " FROM  dbo.TblPupilManagementPupils INNER JOIN  dbo.TblTeachingManagerSetLists ";
            s += " ON dbo.TblPupilManagementPupils.txtSchoolID = dbo.TblTeachingManagerSetLists.txtSchoolID ";
            s += " INNER JOIN  dbo.TblTeachingManagerSets ";
            s += " ON dbo.TblTeachingManagerSets.TblTeachingManagerSetsID = dbo.TblTeachingManagerSetLists.intSetID ";

            s += " INNER JOIN  ";
            s += " dbo.TblStaff ON dbo.TblTeachingManagerSets.txtTeacher = dbo.TblStaff.User_Code INNER JOIN ";
            s += " dbo.TblTeachingManagerSubjects ON dbo.TblTeachingManagerSets.intSubject = dbo.TblTeachingManagerSubjects.TblTeachingManagerSubjectsID  ";
            //s += "WHERE (dbo.TblTeachingManagerSubjects.txtSubjectCode ='" + SubjectCode + "') ";
            s += "  WHERE (dbo.TblTeachingManagerSets.intYear = '" + year + "')  ";
            if (OnRole) { s += " AND (intSystemStatus = 1)  "; } else { }
            s += "ORDER BY dbo .TblPupilManagementPupils.txtSurname , dbo.TblPupilManagementPupils.txtForename, dbo.TblPupilManagementPupils.txtSchoolID, dbo.TblTeachingManagerSets.txtSetCode ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Student_Set d = new ISAMS_Student_Set();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class ISAMS_Sen
    {
        public enum SenFlagType { None, SchoolSupport, SenSupport, Statement, Monitor };
        public int SENRegisterId;
        public string ISAMSPupilId;
        public int SENFlag;// I think this is like serious level  ISAM star colour 4 -monitor  1- grey school support 2 sen support  3 statement/plan
        public string SENNotes;
        public string GoogleNotes;
        /*
        public bool Load_Pupil(string PupilId)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = " SELECT dbo.TblPupilManagementPupils.txtSchoolID, dbo.TblPupilManagementPupils.txtPreviousMISID, ";
            s += " dbo.TblPupilManagementPupils.txtTitle, dbo.TblPupilManagementPupils.txtForename, ";
            s += " dbo.TblPupilManagementPupils.txtSurname, dbo.TblPupilManagementPupils.txtEmailAddress, dbo.TblPupilManagementPupils.txtForm ";
            s += " FROM  dbo.SENManagementRegister INNER JOIN  dbo.TblTeachingManagerSetLists ";
            s += " ON dbo.TblPupilManagementPupils.txtSchoolID = dbo.TblTeachingManagerSetLists.txtSchoolID ";
            s += " WHERE(dbo.TblTeachingManagerSetLists.intSetID = '" + SetId.ToString() + "')  ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Student d = new ISAMS_Student();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
        */


    }

    #endregion

    #region ISAMS_Pupil
    [DataContract]
    public class ISAMS_Student
    {
        [DataMember] public string ISAMS_SchoolId;// they mean pupil Id....
        [DataMember] public int Adno;
        [DataMember] public string Title;
        [DataMember] public string FirstName;
        [DataMember] public string Surname;
        [DataMember] public string PreferedName;
        [DataMember] public string Email;
        [DataMember] public string Form;
        [DataMember] public int SystemStatus; //1=onrole 0 = applicant -1 = leaver]
        [DataMember] public string Gender;
        [DataMember] public DateTime Dob;
        [DataMember] public int NCYear;
        [DataMember] public string GoogleLogin;
        [DataMember] public byte[] SurnameAsBytes;
        [DataMember] public byte[] FirstNameAsBytes;
        [DataMember] public string UCI;
        [DataMember]  public int PreviousMISId;
        [DataMember] public string UPN;
        [DataMember]public DateTime LeavingDate;
        [DataMember]public DateTime JoiningDate;




        public void Hydrate(SqlDataReader dr)
        {
            ISAMS_SchoolId = dr.GetString(0);
            if (!dr.IsDBNull(1)) { Adno = System.Convert.ToInt32(dr.GetString(1)); } else { Adno = 0; };
            if (!dr.IsDBNull(2)) Title = dr.GetString(2);
            if (!dr.IsDBNull(3)) FirstName = dr.GetString(3);
            if (!dr.IsDBNull(4)) Surname = dr.GetString(4);
            if (!dr.IsDBNull(5)) Email = dr.GetString(5);
            if (!dr.IsDBNull(6)) Form = dr.GetString(6);
            if (!dr.IsDBNull(7)) PreferedName = dr.GetString(7);
            //if (!dr.IsDBNull(8) && (Adno == 0)) Adno = dr.GetInt32(8);//other place we store adno???
            if (!dr.IsDBNull(9)) SystemStatus = dr.GetInt32(9);
            if (!dr.IsDBNull(10)) Gender = dr.GetString(10);
            if (!dr.IsDBNull(11)) Dob = dr.GetDateTime(11);
            if (!dr.IsDBNull(12)) NCYear = dr.GetInt32(12);
            if (!dr.IsDBNull(13)) GoogleLogin = dr.GetString(13);
            if (!dr.IsDBNull(14)) UCI = dr.GetString(14);
            if (!dr.IsDBNull(15)) PreviousMISId = System.Convert.ToInt32(dr.GetString(15));
            if (!dr.IsDBNull(16)) UPN = dr.GetString(16);
            if (!dr.IsDBNull(17)) LeavingDate = dr.GetDateTime(17);
            if (!dr.IsDBNull(18)) JoiningDate = dr.GetDateTime(18);

            if (Surname != null) SurnameAsBytes = Encoding.Default.GetBytes(Surname);
            if (FirstName != null) FirstNameAsBytes = Encoding.Default.GetBytes(FirstName);

            //tstEnro;mentDate smalldatetime  txtLeavingDate
        }
    }
    [DataContract]
    public class ISAMS_Student_List
    {
        [DataMember] public List<ISAMS_Student> m_list = new List<ISAMS_Student>();

        protected string GetSelectString()
        {
            string s = " SELECT txtSchoolID,txtSchoolCode , txtTitle, txtForename, txtSurname, ";
            s += " txtEmailAddress, txtForm, txtPreName, intAutoSchoolCodeNumericPart,intSystemStatus ";
            s += ", txtGender ,txtDOB, intNCYear , txtEmailAddress , txtCandidateCode,txtPreviousMISID, txtUPN,  ";
            s += "txtLeavingDate, txtEnrolmentDate ";
            return s;

        }

        public ISAMS_Student_List(string Adno, bool OnRole)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = GetSelectString();
            s += " FROM  dbo.TblPupilManagementPupils ";
            s += " WHERE (txtPreviousMISID = " + Adno + ")  ";


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Student d = new ISAMS_Student();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public ISAMS_Student_List(bool OnRole)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = GetSelectString();
            s += " FROM  dbo.TblPupilManagementPupils ";
            if (OnRole) { s += " WHERE (intSystemStatus = 1)  "; } else { }


            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Student d = new ISAMS_Student();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        protected string GetSelectString2()
        {
            string s = " SELECT dbo.TblPupilManagementPupils.txtSchoolID, dbo.TblPupilManagementPupils.txtSchoolCode, ";
            s += " dbo.TblPupilManagementPupils.txtTitle, dbo.TblPupilManagementPupils.txtForename, ";
            s += " dbo.TblPupilManagementPupils.txtSurname, dbo.TblPupilManagementPupils.txtEmailAddress, dbo.TblPupilManagementPupils.txtForm ";
            s += " ,dbo.TblPupilManagementPupils.txtPreName ";
            s += " , dbo.TblPupilManagementPupils.intAutoSchoolCodeNumericPart,  dbo.TblPupilManagementPupils.intSystemStatus ";
            s += ", dbo.TblPupilManagementPupils.txtGender  , dbo.TblPupilManagementPupils.txtDOB, ";
            s += " dbo.TblPupilManagementPupils.intNCYear , dbo.TblPupilManagementPupils.txtEmailAddress ";
            s += ", dbo.TblPupilManagementPupils.txtCandidateCode ,dbo.TblPupilManagementPupils.txtPreviousMISID,  dbo.TblPupilManagementPupils.txtUPN,  ";
            s += "dbo.TblPupilManagementPupils.txtLeavingDate, dbo.TblPupilManagementPupils.txtEnrolmentDate ";
            s += " FROM  dbo.TblPupilManagementPupils INNER JOIN  dbo.TblTeachingManagerSetLists ";

            s += " ON dbo.TblPupilManagementPupils.txtSchoolID = dbo.TblTeachingManagerSetLists.txtSchoolID ";
            s += " INNER JOIN  dbo.TblTeachingManagerSets ";
            s += " ON dbo.TblTeachingManagerSets.TblTeachingManagerSetsID = dbo.TblTeachingManagerSetLists.intSetID ";
            return s;

        }

        public ISAMS_Student_List(int SetId)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s= GetSelectString2();
            s += " WHERE(dbo.TblTeachingManagerSetLists.intSetID = '" + SetId.ToString() + "')  ";
            s += "ORDER BY dbo.TblPupilManagementPupils.txtSurname , dbo.TblPupilManagementPupils.txtForename ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Student d = new ISAMS_Student();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public ISAMS_Student_List(string SetCode)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = GetSelectString2();
            s += " WHERE(dbo.TblTeachingManagerSets.txtSetCode = '" + SetCode + "')  ";
            s += "ORDER BY dbo.TblPupilManagementPupils.txtSurname , dbo.TblPupilManagementPupils.txtForename ";

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Student d = new ISAMS_Student();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public ISAMS_Student_List() { }

        public void LoadListEmail(string email)
        {
            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();
            string s = GetSelectString();
            s += " FROM  dbo.TblPupilManagementPupils ";
            s += " WHERE (txtEmailAddress = '" + email + "')  ";
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Student d = new ISAMS_Student();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }


    }

    [DataContract]
    public class ISAMS_Address
    {
        [DataMember] public string iSAMS_SchoolId; //note is really studentId
        [DataMember] public string SchoolCode;
        [DataMember] public string txtTitle;
        [DataMember] public string txtForename;
        [DataMember] public string txtSurname;
        [DataMember] public string txtFullName;
        [DataMember] public int intPersonId;
        [DataMember] public string txtContactsTitle;
        [DataMember] public string txtContactsForename;
        [DataMember] public string txtContactsSurname;
        [DataMember] public string txtEmail1;
        [DataMember] public int intSecondaryPersonId;
        [DataMember] public string txtSecondaryTitle;
        [DataMember] public string txtSecondaryForename;
        [DataMember] public string txtSecondarySurname;
        [DataMember] public string txtEmail2;
        [DataMember] public string txtAddressType;
        [DataMember] public string txtRelationType;
        [DataMember] public int intStudentHome;
        [DataMember] int intJustContact;
        [DataMember] string txtSOS;


        public void Hydrate(SqlDataReader dr)
        {
            iSAMS_SchoolId = dr.GetString(0);
            SchoolCode = dr.GetString(1);
            if (!dr.IsDBNull(2)) txtTitle = dr.GetString(2); else txtTitle = "";
            if (!dr.IsDBNull(3)) txtForename = dr.GetString(3); else txtForename = "";
            if (!dr.IsDBNull(4)) txtSurname = dr.GetString(4); else txtSurname = "";
            if (!dr.IsDBNull(5)) txtFullName = dr.GetString(5); else txtFullName = "";
            if (!dr.IsDBNull(6)) intPersonId = dr.GetInt32(6);

            if (!dr.IsDBNull(7)) txtContactsTitle = dr.GetString(7); else txtContactsTitle = "";
            if (!dr.IsDBNull(8)) txtContactsForename = dr.GetString(8); else txtContactsForename = "";
            if (!dr.IsDBNull(9)) txtContactsSurname = dr.GetString(9); else txtContactsSurname = "";
            if (!dr.IsDBNull(10)) txtEmail1 = dr.GetString(10); else txtEmail1 = "";
            if (!dr.IsDBNull(11)) intSecondaryPersonId = dr.GetInt32(11); else intSecondaryPersonId = 0;
            if (!dr.IsDBNull(12)) txtSecondaryTitle = dr.GetString(12); else txtSecondaryTitle = "";
            if (!dr.IsDBNull(13)) txtSecondaryForename = dr.GetString(13); else txtSecondaryForename = "";
            if (!dr.IsDBNull(14)) txtSecondarySurname = dr.GetString(14); else txtSecondarySurname = "";
            if (!dr.IsDBNull(15)) txtEmail2 = dr.GetString(15); else txtEmail2 = "";
            if (!dr.IsDBNull(16)) txtAddressType = dr.GetString(16); else txtAddressType = "";
            if (!dr.IsDBNull(17)) txtRelationType = dr.GetString(17); else txtRelationType = "";
            if (!dr.IsDBNull(18)) intStudentHome = dr.GetInt32(18); else intStudentHome = 0;
            if (!dr.IsDBNull(19)) intJustContact = dr.GetInt32(19); else intJustContact = -1;
            if (!dr.IsDBNull(20)) txtSOS = dr.GetString(20); else txtSOS = "";
        }
    }

    [DataContract]
    public class ISAMS_AddressList
    {
        [DataMember] public List<ISAMS_Address> m_list = new List<ISAMS_Address>();

        public void load()
        {
            string s = GetQuery(" WHERE (dbo.TblPupilManagementPupils.intSystemStatus = 1) ");
            Encode en = new Encode();
            using (SqlConnection cn = new SqlConnection(en.GetISAMSDBConnection()))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Address d = new ISAMS_Address();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        protected string GetQuery(string where)
        {
            string s = " SELECT  dbo.TblPupilManagementPupils.txtSchoolID, dbo.TblPupilManagementPupils.txtSchoolCode , dbo.TblPupilManagementPupils.txtTitle, dbo.TblPupilManagementPupils.txtForename , ";
            s += " dbo.TblPupilManagementPupils.txtSurname , dbo.TblPupilManagementPupils.txtFullName, dbo.TblPupilManagementAddresses.intPersonId , dbo.TblPupilManagementAddresses.txtContactsTitle , ";
            s += "  dbo.TblPupilManagementAddresses.txtContactsForename , dbo.TblPupilManagementAddresses.txtContactsSurname , dbo.TblPupilManagementAddresses.txtEmail1 , ";
            s += "  dbo.TblPupilManagementAddresses.intSecondaryPersonId , dbo.TblPupilManagementAddresses.txtSecondaryTitle , dbo.TblPupilManagementAddresses.txtSecondaryForename , ";
            s += "  dbo.TblPupilManagementAddresses.txtSecondarySurname , dbo.TblPupilManagementAddresses.txtEmail2 , dbo.TblPupilManagementAddresses.txtAddressType, ";
            s += " dbo.TblPupilManagementAddresses.txtRelationType, dbo.TblPupilManagementAddresses.intStudentHome, dbo.TblPupilManagementAddresses.intJustContact, ";
            s += " dbo.TblPupilManagementAddresses.txtSOS ";
            s += " FROM   dbo.TblPupilManagementPupils INNER JOIN ";
            s += " dbo.TblPupilManagementAddressLink ON dbo.TblPupilManagementPupils.txtSchoolID = dbo.TblPupilManagementAddressLink.txtSchoolID INNER JOIN ";
            s += " dbo.TblPupilManagementAddresses ON dbo.TblPupilManagementAddressLink.intAddressID = dbo.TblPupilManagementAddresses.TblPupilManagementAddressesID ";
            s += where;
            s += " ORDER BY dbo.TblPupilManagementAddresses.txtRelationType DESC ";
            return s;
        }

        public void load(int NCYear)
        {
            string where = " WHERE (dbo.TblPupilManagementPupils.intSystemStatus = 1) AND (dbo.TblPupilManagementPupils.intNCYear ='" + NCYear.ToString() + "')";
            string s = GetQuery(where);

            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Address d = new ISAMS_Address();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }
    #endregion

    #endregion

    //lna code.....
    [DataContract]
    public class ISAMS_Set_PTO
    {
        [DataMember(Order = 1)]
        public string SCD; //note is really studentId
        [DataMember(Order = 2)]
        public string SFN;
        [DataMember(Order = 3)]
        public string SSN;
        [DataMember(Order = 4)]
        public string CCD;
        [DataMember(Order = 5)]
        public int CGN;
        [DataMember(Order = 6)]
        public string CLS;
        [DataMember(Order = 7)]
        public string TCD;
        [DataMember(Order = 8)]
        public string TTI;
        [DataMember(Order = 9)]
        public string TFN;
        [DataMember(Order = 10)]
        public string TSN;
        [DataMember(Order = 11)]
        public string TEM;
        [DataMember(Order = 12)]
        public string txtForm;
        [DataMember(Order = 13)]
        public int NCYear;
        [DataMember(Order = 14)]
        public string SchoolCode;


        public void Hydrate(SqlDataReader dr)
        {
            SCD = dr.GetString(0);
            SchoolCode = dr.GetString(1);
            if (!dr.IsDBNull(2)) SFN = dr.GetString(2); else SFN = "";
            if (!dr.IsDBNull(3)) SSN = dr.GetString(3); else SSN = "";
            if (!dr.IsDBNull(4)) CCD = dr.GetString(4); else CCD = "";
            if (!dr.IsDBNull(5)) CGN = dr.GetInt32(5);
            if (!dr.IsDBNull(6)) CLS = dr.GetString(6); else CLS = "";
            if (!dr.IsDBNull(7)) TCD = dr.GetString(7); else TCD = "";
            if (!dr.IsDBNull(8)) TTI = dr.GetString(8); else TTI = "";
            if (!dr.IsDBNull(9)) TFN = dr.GetString(9); else TFN = "";
            if (!dr.IsDBNull(10)) TSN = dr.GetString(10); else TSN = "";
            if (!dr.IsDBNull(11)) TEM = dr.GetString(11); else TEM = "";
            if (!dr.IsDBNull(12)) txtForm = dr.GetString(12); else txtForm = "";
            if (!dr.IsDBNull(13)) NCYear = dr.GetInt32(13);
        }
    }

    [DataContract]
    public class ISAMS_Set_PTOList
    {
        [DataMember]
        public List<ISAMS_Set_PTO> m_list = new List<ISAMS_Set_PTO>();

        protected string GetQuery(string where)
        {
            string s = " SELECT  dbo.TblPupilManagementPupils.txtSchoolID as SCD, dbo.TblPupilManagementPupils.txtSchoolCode , dbo.TblPupilManagementPupils.txtForename as SFN, dbo.TblPupilManagementPupils.txtSurname as SSN, ";
            s += " dbo.TblTeachingManagerSets.txtSetCode AS CCD , dbo.TblTeachingManagerSets.intYear AS CGN ,  dbo.TblTeachingManagerSubjects.txtSubjectName AS CLS ,";
            s += " dbo.TblStaff.Initials AS TCD, dbo.TblStaff.Title AS TTI, dbo.TblStaff.Firstname AS TFN, dbo.TblStaff.Surname AS TSN, dbo.TblStaff.SchoolEmailAddress AS TEM,";
            s += " dbo.TblPupilManagementPupils.txtForm,  dbo.TblPupilManagementPupils.intNCYear ";
            s += " FROM   dbo.TblPupilManagementPupils INNER JOIN ";
            s += " dbo.TblTeachingManagerSetLists ON dbo.TblPupilManagementPupils.txtSchoolID = dbo.TblTeachingManagerSetLists.txtSchoolID INNER JOIN ";
            s += " dbo.TblTeachingManagerSets ON dbo.TblTeachingManagerSetLists.intSetID = dbo.TblTeachingManagerSets.TblTeachingManagerSetsID INNER JOIN ";
            s += " dbo.TblTeachingManagerSubjects ON TblTeachingManagerSets.intSubject = TblTeachingManagerSubjects.TblTeachingManagerSubjectsID INNER JOIN  ";
            s += " dbo.TblStaff ON dbo.TblTeachingManagerSets.txtTeacher = dbo.TblStaff.User_Code ";
            s += where;
            s += " GROUP BY dbo.TblPupilManagementPupils.txtSchoolID, dbo.TblPupilManagementPupils.txtSchoolCode, dbo.TblPupilManagementPupils.txtForename, dbo.TblPupilManagementPupils.txtSurname , ";
            s += " dbo.TblTeachingManagerSets.txtSetCode, dbo.TblTeachingManagerSets.intYear, dbo.TblTeachingManagerSubjects.txtSubjectName, ";
            s += " dbo.TblStaff.Initials, dbo.TblStaff.Title, dbo.TblStaff.Firstname, dbo.TblStaff.Surname, dbo.TblStaff.SchoolEmailAddress, ";
            s += " TblPupilManagementPupils.txtForm,  TblPupilManagementPupils.intNCYear ";
            s += " Union ";
            s += " SELECT TblPupilManagementPupils.txtSchoolID AS SCD, TblPupilManagementPupils.txtSchoolCode,TblPupilManagementPupils.txtForename AS SFN, TblPupilManagementPupils.txtSurname AS SSN, ";
            s += " dbo.TblTeachingManagerSets.txtSetCode AS CCD, TblTeachingManagerSets.intYear AS CGN, TblTeachingManagerSubjects.txtSubjectName AS CLS, ";
            s += " dbo.TblStaff.Initials AS TCD, TblStaff.Title AS TTI, TblStaff.Firstname AS TFN, TblStaff.Surname AS TSN,TblStaff.SchoolEmailAddress AS TEM, ";
            s += " TblPupilManagementPupils.txtForm,  TblPupilManagementPupils.intNCYear ";
            s += " FROM TblTeachingManagerSetAssociatedTeachers INNER JOIN ";
            s += " dbo.TblStaff ON TblTeachingManagerSetAssociatedTeachers.txtTeacher = TblStaff.User_Code INNER JOIN ";
            s += " dbo.TblPupilManagementPupils  INNER JOIN ";
            s += " dbo.TblTeachingManagerSetLists ON TblPupilManagementPupils.txtSchoolID = TblTeachingManagerSetLists.txtSchoolID INNER JOIN ";
            s += " dbo.TblTeachingManagerSets ON TblTeachingManagerSetLists.intSetID = TblTeachingManagerSets.TblTeachingManagerSetsID INNER JOIN ";
            s += " dbo.TblTeachingManagerSubjects ON TblTeachingManagerSets.intSubject = TblTeachingManagerSubjects.TblTeachingManagerSubjectsID  ON  ";
            s += " dbo.TblTeachingManagerSetAssociatedTeachers.intSetID = TblTeachingManagerSets.TblTeachingManagerSetsID ";
            s += where;
            s += " group by TblPupilManagementPupils.txtSchoolID,TblPupilManagementPupils.txtSchoolCode,TblPupilManagementPupils.txtForename,TblPupilManagementPupils.txtSurname,TblTeachingManagerSets.txtSetCode, ";
            s += " TblTeachingManagerSets.intYear,TblTeachingManagerSubjects.txtSubjectName,TblStaff.Initials,TblStaff.Title , TblStaff.Firstname , TblStaff.Surname, ";
            s += " TblStaff.SchoolEmailAddress , TblPupilManagementPupils.txtForm,  TblPupilManagementPupils.intNCYear ";
            return s;
        }

        public void load(int NCYear)
        {
            string where = " WHERE (dbo.TblPupilManagementPupils.intSystemStatus = 1) AND (dbo.TblPupilManagementPupils.intNCYear ='" + NCYear.ToString() + "') AND (NOT(dbo.TblTeachingManagerSubjects.txtSubjectCode IN(N'HS', N'GA')))";
            string s = GetQuery(where);

            Encode en = new Encode();
            string db_connection = en.GetISAMSDBConnection();

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ISAMS_Set_PTO d = new ISAMS_Set_PTO();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

        }
    }




    /// <remarks/>
  
    public class ValueAddedResult
    {
        public Guid Id;
        public Guid VAMethodId;
        public Guid StudentId;
        public Guid GroupId;
        public Guid CourseId;
        public decimal BaseResultValue;
        public decimal OutputResultValue;
        public decimal VAResultValue;
        public System.DateTime DateOutpuResult;
        public DateTime Date;
        public int Version;

        public ValueAddedResult()
        {
            Id = Guid.Empty;
            StudentId = Guid.Empty;
            GroupId = Guid.Empty;
            CourseId = Guid.Empty;

        }

        public void Hydrate(SqlDataReader dr)
        {
            int i = 0;
            Id = dr.GetGuid(i); i++;
            VAMethodId = dr.GetGuid(i); i++;
            if (!dr.IsDBNull(i)) { StudentId = dr.GetGuid(i);} i++;
            if (!dr.IsDBNull(i)) { GroupId   = dr.GetGuid(i);} i++;
            if (!dr.IsDBNull(i)) { CourseId  = dr.GetGuid(i);} i++;
            BaseResultValue = dr.GetDecimal(i);i++;
            OutputResultValue = dr.GetDecimal(i); i++;
            VAResultValue = dr.GetDecimal(i);i++;
            DateOutpuResult = dr.GetDateTime(i);i++;
            Date = dr.GetDateTime(i);i++;
            Version = dr.GetInt32(i);
        }

        public void Update()
        {
            //if id exists then update - else create...
            string s = "";
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
                s = "INSERT INTO tbl_Core_ValueAddedResults (Id,VAMethodId,";
                if (StudentId != Guid.Empty) { s += " StudentId,"; }
                if (GroupId != Guid.Empty) { s += " GroupId,"; }
                if (CourseId != Guid.Empty) { s += "CourseId,"; }
                s += " BaseResultValue,OutputResultValue,VAResultValue,DateOutpuResult,DateofEvaluation,Version )";
                s += "VALUES ('" + Id.ToString() + "', '" + VAMethodId.ToString() + "' , '";
                if (StudentId != Guid.Empty) { s += StudentId.ToString() + "' , '"; }
                if (GroupId != Guid.Empty) { s += GroupId.ToString() + "' , '"; }
                if (CourseId != Guid.Empty) { s += CourseId.ToString() + "' , '"; }
                s += BaseResultValue.ToString() + "' , '" + OutputResultValue.ToString() + "' , '" + VAResultValue.ToString() + "' , CONVERT(DATETIME, '" + DateOutpuResult.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , CONVERT(DATETIME, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '" + Version + "'  )";
            }
            else
            {
                s = "UPDATE tbl_Core_ValueAddedResults SET VAMethodId='" + VAMethodId.ToString() + "' , ";
                if (StudentId != Guid.Empty) { s += "StudentId='" + StudentId.ToString() + "' ,"; }
                if (GroupId != Guid.Empty) { s += "GroupId='" + GroupId.ToString() + "' , "; }
                if (CourseId != Guid.Empty) { s += "CourseId= '" + CourseId.ToString() + "', ";}
                s += "BaseResultValue= '" + BaseResultValue.ToString() + "' , ";
                s += "OutputResultValue= '" + OutputResultValue.ToString() + "' , ";
                s += "VAResultValue='" + VAResultValue.ToString() + "' , ";
                s += "DateOutpuResult=CONVERT(DATETIME, '" + DateOutpuResult.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s += "DateofEvaluation=CONVERT(DATETIME, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) , ";
                s+=" Version='" + Version + "' ";
                s += " WHERE (ID ='" + Id.ToString() + "' ) ";
            }
            Encode en = new Encode(); en.ExecuteSQL(s);
        }
    }


    public class VAResultsList
    {
        public List<ValueAddedResult> m_list = new List<ValueAddedResult>();

        public void Load()
        {   Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                string s = "SELECT * FROM tbl_Core_ValueAddedResults";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ValueAddedResult d = new ValueAddedResult();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
        public void LoadCourse(Guid VAMethod)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                string s = "SELECT * FROM tbl_Core_ValueAddedResults INNER JOIN tbl_Core_Courses ON tbl_Core_ValueAddedResults.CourseId  = tbl_Core_Courses.CourseId ";
                s += "    WHERE (VAMethodId ='" + VAMethod.ToString() + "') AND (StudentId IS NULL) AND (GroupId IS NULL)";
                s += " ORDER BY tbl_Core_Courses.CourseName ";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ValueAddedResult d = new ValueAddedResult();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }

        public void LoadGroup(Guid VAMethod, Guid CourseId)
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                string s = "SELECT * FROM tbl_Core_ValueAddedResults INNER JOIN tbl_Core_Groups ON tbl_Core_ValueAddedResults.GroupId  = tbl_Core_Groups.GroupId ";
                s += "    WHERE (VAMethodId ='" + VAMethod.ToString() + "') AND (StudentId IS NULL) AND (tbl_Core_ValueAddedResults.CourseId ='" + CourseId.ToString()+"')";
                s += " ORDER BY tbl_Core_Groups.GroupName ";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ValueAddedResult d = new ValueAddedResult();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class VAModel
    {
        public Guid Id;
        public string Name;
        public Guid VAMethodId;
        public int BaseDataAggregationId;
        public int ResultsYear;
        public string Notes;
        public bool Valid;
        public bool Display;

        public VAModel() { Id = Guid.Empty; }

        public void Hydrate(SqlDataReader dr)
        {
            int i = 0;
            Id = dr.GetGuid(i);i++;
            Name = dr.GetString(i); i++;
            if (!dr.IsDBNull(i)) { VAMethodId = dr.GetGuid(i); }else {VAMethodId = Guid.Empty; } i++;
            if (!dr.IsDBNull(i)) { BaseDataAggregationId = dr.GetInt32(i); } else { BaseDataAggregationId = 0; }; i++;
            if (!dr.IsDBNull(i)) { ResultsYear = dr.GetInt32(i); } i++;
            if (!dr.IsDBNull(i)) { Notes = dr.GetString(i); i++; } else { Notes = ""; i++; }
            Valid = dr.GetBoolean(i); i++;
            Display = dr.GetBoolean(i);       
        }

        public void Update()
        {
            //if id exists then update - else create...
            string s = "";
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
                s = "INSERT INTO tbl_Core_ValueAddedModels (Id,ImplementationName,VAMethodId,BaseDataAggregationId,ResultsYear,Description,Valid,Display )";
                s += "VALUES ('" + Id.ToString() + "', '" + Name + "' , '" + VAMethodId.ToString() + "' , '" + BaseDataAggregationId.ToString()+"' , '"+ ResultsYear.ToString() + "' , '" + Notes + "' ,'"+Valid+"' , '"+Display+"'  )";
            }
            else
            {
                s = "UPDATE dtbl_Core_ValueAddedModels SET ImplementationName='" + Name + "' , VAMethodId='"+ VAMethodId.ToString() + "' , BaseDataAggregationId='"+BaseDataAggregationId.ToString()+"' ,  ResultsYear='" + ResultsYear.ToString()+"',  Description='"+Notes+"' , valid='"+Valid+"' , Display='"+Display+"'";
                s += " WHERE (Id ='" + Id.ToString() + "' )";
            }
            Encode en = new Encode(); en.ExecuteSQL(s);
        }
    }
    public class VAModelList
    {
        public List<VAModel> m_list = new List<VAModel>();
        public void Load()
        {

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                string s = "SELECT * FROM tbl_Core_ValueAddedModels";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            VAModel d = new VAModel();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    public class VABaseDataAggregation
    {
        public int Id;
        public string Name;
        public string Description;
        public string Equation;
        public VABaseDataAggregation() { Id = 0; }
        public void Hydrate(SqlDataReader dr)
        {
            int i = 0;
            Id = dr.GetInt32(i); i++;
            Name = dr.GetString(i); i++;
            if (!dr.IsDBNull(i)) { Description = dr.GetString(i); }else {Description = "";} i++;
            if (!dr.IsDBNull(i)) { Equation = dr.GetString(i); } else { Equation = ""; }
        }
        public void Update()
        {
            //if id exists then update - else create...
            string s = "";
            if (Id == 0)
            {
                s = "INSERT INTO tbl_List_VABaseDataAggrehation (Name,Description,Equation)";
                s += "VALUES ('" + Name + "' , '" + Description + "' , '" + Equation +  "'  )";
            }
            else
            {
                s = "UPDATE dtbl_Core_ValueAddedModels SET Name='" + Name + "' ,Description='" + Description + "' , Equation='" + Equation + "'";
                s += " WHERE (Id ='" + Id.ToString() + "' )";
            }
            Encode en = new Encode(); en.ExecuteSQL(s);
        }
    }
    public class VABaseDataAggregationList
    {
        public VABaseDataAggregationList()
        {
            Load();
        }
        public List<VABaseDataAggregation> m_list = new List<VABaseDataAggregation>();
        public void Load()
        {
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                string s = "SELECT * FROM tbl_List_VABaseDataAggregation";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            VABaseDataAggregation d = new VABaseDataAggregation();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }

    [DataContract]
    public class dFEProgressItem
    {

        public Guid Id;
        public Guid studentID;
        public bool disadvantaged;
        public bool english_first_language;
        public string sEN;
        public bool nonMobile;
        public int ethnicOrigin;
        public int attainment8;
        public double averageAttainment8ScoreForSimilarPupils;
        public float progress8AdjustedScore;
        public float progress8UnadjustedScore;
        public bool grade5InEnglishmaths;
        public bool eBacc;
        public int eN_DoubleweightedA8score;
        public float averageAttainment8ScoreEnglishForSimilarPupils;
        public float en_Progressscore;
        public int mA_DoubleweightedA8score;
        public float averageAttainment8ScoreMathsForSimilarPupils;
        public float mAProgressscore;
        public int eBaccSlot1Score;
        public int eBaccSlot2Score;
        public int eBaccSlot3Score;
        public int eBaccA8Score;
        public float averageAttainment8ScoreEBACCForSimilarPupils;
        public float eBacc_ProgressScore;
        public int openSlot1Score;
        public int openSlot2Score;
        public int openSlot3Score;
        public int gCSEonlyScore;
        public int nonGCSEOnlyScore;
        public int openA8Score;
        public float averageAttainment8ScoreOpenSlotsForSimilarPupils;
        public float openA8_ProgressScore;
        public bool englishEntry;
        public bool eN_PillarArachieved;
        public bool mathsEntry;
        public bool mA_PillarAchieved;
        public bool scienceEntry;
        public bool sc_PillarAchieved;
        public bool humanitiesEntry;
        public bool hu_PillarAchieved;
        public bool languageEntry;
        public bool lang_PillarAchieved;

        

        public void Hydrate(SqlDataReader dr)
        {
            int i = 0;
            Id = dr.GetGuid(i); i++;
            studentID = dr.GetGuid(i); i++;
            disadvantaged = dr.GetBoolean(i); i++;
            english_first_language = dr.GetBoolean(i); i++;
            sEN = dr.GetString(i); i++;
            nonMobile = dr.GetBoolean(i); i++;
            ethnicOrigin = dr.GetInt32(i); i++;
            attainment8 = dr.GetInt32(i); i++;
            try {
                averageAttainment8ScoreForSimilarPupils = dr.GetInt32(i); i++;
            }catch (Exception e)
            {
                i = 9;
            }
            try
            {
                progress8AdjustedScore = (float)dr.GetDecimal(i); i++;
            }
            catch (Exception e)
            {
                i = 9;
            }
            try
            {
                progress8UnadjustedScore = (float)dr.GetDecimal(i); i++;
            }
            catch (Exception e)
            {
                i = 9;
            }

        }
    }

    [DataContract]
    public class dFEProgressList
    {

        [DataMember]
        public List<dFEProgressItem> m_list = new List<dFEProgressItem>();

        public void Load()
        {

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();

            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                string s = "SELECT * FROM dFE_ProgressData";
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            dFEProgressItem d = new dFEProgressItem();
                            m_list.Add(d);
                            d.Hydrate(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
        }
    }



        


}