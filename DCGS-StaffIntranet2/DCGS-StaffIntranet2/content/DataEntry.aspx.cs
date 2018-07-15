using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class DataEntry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                SetupCombo();
                AddControls(true);
            }
            else
            {
                AddControls(false);
            }

        }
        protected void SetupCombo()
        {
            DropDownList_Sets.Items.Clear();
            //get sets for this user..
            Utility u = new Utility();
            //string struser = Context.User.Identity.Name;
            //string staff_code = "";
            //Guid personID = u.GetPersonID(struser, out staff_code);
            Guid personID = u.GetPersonIdfromRequest(Request);
            string staff_code = u.GetsStaffCodefromRequest(Request);
#if DEBUG
            staff_code = "CC";
#endif

            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_Exam_Results_Years");
            string s = c.Value;
            if (!c.valid)//revert to config file
            {
                System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
                s = ar.GetValue("Exam_Results_Years", typeof(string)).ToString();
            }
            //s contains the years we want to process as comma sep list
            string[] s1 = new string[1]; s1[0] = ",";
            string[] s2 = new string[10];
            s2 = s.Split(s1, StringSplitOptions.None);

            //staff_code = "JA";
            GroupList gl1 = new GroupList(); 
            gl1.LoadStaff(staff_code, DateTime.Now, GroupList.GroupListOrder.GroupName);
            foreach (Group g in gl1._groups)
            {
                foreach (string s3 in s2)
                {
                    if(g._GroupCode.StartsWith(s3))
                    {
                        ListItem i = new ListItem(g._GroupCode, g._GroupID.ToString());
                        DropDownList_Sets.Items.Add(i);
                    }
                }
                //if (g._GroupCode.StartsWith("12"))
                //if (g._GroupCode.StartsWith("10") || g._GroupCode.StartsWith("7") || g._GroupCode.StartsWith("8") || g._GroupCode.StartsWith("9"))
                //if (g._GroupCode.StartsWith("11"))
                //{
                  //  ListItem i = new ListItem(g._GroupCode, g._GroupID.ToString());
                   // DropDownList_Sets.Items.Add(i);
                //}

            }
            if (DropDownList_Sets.Items.Count > 0) DropDownList_Sets.SelectedIndex = 0;
        }
        protected override void LoadViewState(object savedState)
        {
            LoadViewState(savedState);
            AddControls(false);
        }
        protected void AddControls(bool init)
        {
            if (DropDownList_Sets.SelectedIndex < 0)
            {
                return;
            }
            TextBox1.Text = DropDownList_Sets.SelectedItem.Text;
            string set = TextBox1.Text;
            Table tb1 = (Table)content0.FindControl("table1");
            PupilGroupList pgl = new PupilGroupList();
            ResultsList rl1 = new ResultsList();


            string grade = ""; string cse = "";
            int Key_stage = 0; string year = "";
            //need to find course type....
            if (set.StartsWith("1"))
            {
                //10,11,12,13
                if (set.StartsWith("10") || set.StartsWith("11")) Key_stage = 4; else Key_stage = 5;
                cse = set.Substring(2, 2).ToUpper(); year = set.Substring(0, 2);
            }
            else
            {
                Key_stage = 3;
                cse = set.Substring(3, 2).ToUpper(); year = set.Substring(0, 1);
            }
            if (init)
            {
                CourseList cl1 = new CourseList(Key_stage);
                foreach (Course c in cl1._courses)
                {
                    if (c.CourseCode == cse)
                    {
                        TextBox3.Text = c._CourseID.ToString();
                    }
                }
            }
            //now this is for collecting internal results.....
            string s2 = "";
            DateTime SetListDate = new DateTime();
            DateTime ResultDate = new DateTime();
            Utility u = new Utility();
            ResultDate = u.GetResultDate(year, ref s2, ref SetListDate);
            
            TextBox2.Text = ResultDate.ToShortDateString();


            TextBox4.Text = s2;
            pgl.AddToList(set, SetListDate);
            Label1.Text = "Results for "+set+" for "+s2+". Use tab to go to next record."; Label1.Visible = true;


            foreach (SimplePupil sp in pgl.m_pupilllist)
            {
                TableRow r1 = new TableRow();tb1.Controls.Add(r1);
                r1.BorderStyle = BorderStyle.Solid;
                r1.BorderColor = tb1.BorderColor;
                r1.BorderWidth = tb1.BorderWidth;
                TableCell c1 = new TableCell(); r1.Controls.Add(c1);
                TableCell c2 = new TableCell(); r1.Controls.Add(c2);
                TextBox txb1 = new TextBox();c2.Controls.Add(txb1);
                c1.BorderStyle = tb1.BorderStyle;
                c1.BorderWidth = tb1.BorderWidth;
                c1.BorderColor = tb1.BorderColor;
                c2.BorderStyle = tb1.BorderStyle;
                c2.BorderWidth = tb1.BorderWidth;
                c2.BorderColor = tb1.BorderColor;
                c2.Width = 40;
                txb1.TextChanged += new EventHandler(txt);
                txb1.Width = 30;
                txb1.ID = sp.m_StudentId.ToString();
                c1.Text = sp.m_GivenName + " " + sp.m_Surname;
                if (set.StartsWith("1")) cse = set.Substring(2, 2).ToUpper(); else cse = set.Substring(3, 2).ToUpper();
                if (init)
                {
                    grade = "";
                    rl1.m_parameters = 3; rl1.m_db_field2 = "ResultType"; rl1.m_value2 = "5";
                    rl1.m_db_extraquery = " AND (dbo.tbl_Core_Results.ResultDate >= CONVERT(DATETIME, '" + ResultDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
                    rl1.LoadList("dbo.tbl_Core_Students.StudentId", sp.m_StudentId.ToString());
                    foreach (Result r in rl1._results)
                    {
                        if (r.Code == cse)
                        {
                            grade = r.Value;
                        }
                    }
                    txb1.Text = grade;        
                }
            }
        }

        protected void submit(object sender, EventArgs e)
        {
            string s = sender.ToString();
            s.ToUpper();
        }
        protected void SaveResult(string stud_id,string grade, string CourseID)
        {
            Utility u = new Utility();
            string s = "";
            bool found = false;
            ResultsList rl1 = new ResultsList();

            DateTime ResultDate = new DateTime();
            ResultDate = System.Convert.ToDateTime(TextBox2.Text);
            rl1.m_parameters = 3; rl1.m_db_field2 = "ResultType"; rl1.m_value2 = "5";
            rl1.m_db_extraquery = " AND (dbo.tbl_Core_Results.ResultDate >= CONVERT(DATETIME, '" + ResultDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) )";
            rl1.LoadList("dbo.tbl_Core_Students.StudentId", stud_id);
            foreach (Result r in rl1._results)
            {
                if (r.CourseID.ToString() == TextBox3.Text)
                {
                    // we have a value...
                    //update 
                    found = true;
                    s = "UPDATE tbl_Core_Results SET ResultValue='" + grade + "' ";
                    s += "WHERE ResultID = '" + r.ResultID.ToString() + "'";
                    Encode en1 = new Encode();
                    en1.ExecuteSQL(s);
                }
            }
            if (!found)
            {
                string struser = Context.User.Identity.Name;
                string staff_code = "";
                //Guid personID = u.GetPersonID(struser, out staff_code);
                Guid personID = u.GetPersonIdfromRequest(Request);
                staff_code = u.GetsStaffCodefromRequest(Request);
                SimplePupil pupil1 = new SimplePupil(); pupil1.Load(stud_id.ToString());
                if (personID != Guid.Empty) struser = u.Get_StaffCode(personID);
#if DEBUG
                struser = "CC";
#endif
                s = DateTime.Now.ToShortDateString() + "," + DateTime.Now.ToShortTimeString() + "," + TextBox1.Text + "," + struser + "," + pupil1.m_GivenName + "," + pupil1.m_Surname + "," + pupil1.m_adno.ToString() + "," + grade;
                //u.WriteToLogFile("resultslog.txt", s);
                s = "INSERT INTO dbo.tbl_Core_Results (ResultType, StudentID, ResultValue, ResultDate, ResultText, ";
                s += "CourseId  )";
                s += " VALUES ( '5', '";
                s += stud_id + "', '";
                s += grade + "', ";
                s += " CONVERT(DATETIME, '" + ResultDate.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '";
                s += " ',  '";
                s += CourseID + "' )";
                Encode en1 = new Encode();
                en1.ExecuteSQL(s);

            }
        }
        protected void txt(object sender, EventArgs e)
        {
            TextBox txtbox = (TextBox) sender;
            string grade = txtbox.Text.Trim();
            //SO WE HAVE A CALL WITH STUDENT ID AS TEXT BOX ID
            string stud_id = txtbox.ID;
            SaveResult(stud_id, grade, TextBox3.Text);
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            string s = sender.ToString();
            s.ToUpper();
            Server.Transfer("StartForm.aspx");
        }
        protected void DropDownList_Sets_SelectedIndexChanged(object sender, EventArgs e)
        {
            Table1.Controls.Clear();
            AddControls(true);
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            if (Button_UseFile.Text == "Grid Entry")
            {
                Table1.Visible = true;
                Button1.Visible = true;
                FileUpload_picker.Visible = false;
                Button_FileUpload.Visible = false;
                Label1.Text = "";
                Button_UseFile.Text = "Submit by File";
                Div1.InnerText="If you wish to up load from a text file for this set click below:";
                Table1.Controls.Clear();
                content100.InnerHtml = "";
                AddControls(true);
            }
            else
            {
                //file upload.
                Table1.Visible = false;
                Button1.Visible = false;
                Button_FileUpload.Visible = true;
                FileUpload_picker.Visible = true;
                Label1.Text = "Results for " + TextBox1.Text + " for " + TextBox4.Text+"."; Label1.Visible = true;
                //Label1.Text = " Please prepare and select the file. It must be a tab spearated output from an Excel sheet. It must have four columns. In order: FirstName, Surname, Admission Number, Result(%). ";
                content100.InnerHtml= " Please prepare and select the file. <br />";
                content100.InnerHtml += "It must be a tab spearated output from an Excel sheet.<br />";
                content100.InnerHtml += "(From Excel: Save as ..other format.. Text(Tab delimeted)) <br />";
                content100.InnerHtml+="It must have four columns. <br />";
                content100.InnerHtml+="In order: FirstName, Surname, Admission Number, Result(%). ";
                Button_UseFile.Text = "Grid Entry";
                Div1.InnerText="If you wish to revert to entering data via a grid click below:";
            }
        }
        protected void ProcessYearFile(int Key_stage)
        {
            string s = "DataUpload_YearFile_" + DateTime.Now.ToLongTimeString() + ".txt";
            s = s.Replace(":", "-");
            string path = Server.MapPath(@"../App_Data/") + s;
            //String username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            FileUpload_picker.SaveAs(path);
            Cerval_Library.TextReader text1 = new Cerval_Library.TextReader();
            Cerval_Library.TextRecord t = new TextRecord();
            FileStream f = new FileStream(path, FileMode.Open);
            string s2 = "";
            string[] Course_Name = new string[20];
            string[] Course_ID = new string[20];
            text1.ReadTextLine(f, ref t);//header row
            s = t.field[2]; int n = t.number_fields;
            SimplePupil pupil1 = new SimplePupil();
            if ((s != "Adno") || (n < 4))
            {
                f.Close(); return;
            }

            CourseList cl1 = new CourseList(Key_stage);

            for (int i = 3; i < n; i++)
            {
                Course_Name[i] = t.field[i];
                foreach (Course c in cl1._courses)
                {
                    if (c.CourseCode == Course_Name[i])
                    {
                        Course_ID[i] = c._CourseID.ToString();
                    }
                }
            }

            while (text1.ReadTextLine(f, ref t) == Cerval_Library.TextReader.READ_LINE_STATUS.VALID)
            {
                //if all goes well col 2 is adno....
                s = t.field[2];
                for (int i = 3; i < n; i++)
                {
                    s2 = t.field[i];//result
                    try
                    {
                        pupil1.Load(System.Convert.ToInt32(s));

                            try
                            {
                                int r = System.Convert.ToInt32(s2);
                                if ((r > 0) && (r < 101))
                                    SaveResult(pupil1.m_StudentId.ToString(), s2,Course_ID[i]);
                            }
                            catch
                            {
                            }
                    }
                    catch
                    {
                    }
                }

            }
            f.Close();
        }
        protected void Button_FileUpload_Click(object sender, EventArgs e)
        {
            SimplePupil pupil1 = new SimplePupil();
            if (FileUpload_picker.FileName.ToUpper().Contains("WHOLEYEAR"))//should be WholeYear10 or etc
            {
                try
                {
                    string s1 = FileUpload_picker.FileName.Substring(9);
                    if (s1.StartsWith("1")) s1 = s1.Substring(0, 2); else s1 = s1.Substring(0, 1);
                    int year = System.Convert.ToInt32(s1);
                    int k = 4;
                    if (year < 10) k = 3;
                    if (year > 11) k = 5;
                    //need to set up the date for result
                    string s2 = "";
                    DateTime SetListDate = new DateTime();
                    Utility u = new Utility();
                    TextBox2.Text = u.GetResultDate(s1, ref s2, ref SetListDate).ToShortDateString();
                    ProcessYearFile(k); 
                    return;
                }
                catch
                {
                }
            }
            string s = "DataUpload_" +TextBox1.Text+"_"+  DateTime.Now.ToLongTimeString() + ".txt";
            s=s.Replace(":", "-");
            string path = Server.MapPath(@"../App_Data/")+s;
            //String username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            FileUpload_picker.SaveAs(path);
            Stream mystream = FileUpload_picker.FileContent;
            byte[] buffer = new byte[mystream.Length];
            mystream.Read(buffer, 0, (int)mystream.Length);

            Cerval_Library.TextReader text1 = new Cerval_Library.TextReader();
            Cerval_Library.TextRecord t = new TextRecord();
            FileStream f = new FileStream(path, FileMode.Open);
            while (text1.ReadTextLine(f,ref t) == Cerval_Library.TextReader.READ_LINE_STATUS.VALID)
            {
                //if all goes well col 2 is adno....
                s = t.field[2];
                try
                {
                    pupil1.Load(System.Convert.ToInt32(s));
                    TextBox tst1 = (TextBox) content0.FindControl(pupil1.m_StudentId.ToString());
                    if (tst1 != null)
                    {
                        //so we have the adno of a student in the list...
                        s = t.field[3];//and a grade
                        try
                        {
                            int r = System.Convert.ToInt32(s);
                            if((r>0)&&(r<101))
                                SaveResult(pupil1.m_StudentId.ToString(), s, TextBox3.Text);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }

            }
            f.Close();
            //File.Delete(path);
            Table1.Controls.Clear();
            AddControls(true);
        }
    }
}
