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
using Cerval_Library;

namespace DCGS_Staff_Intranet2
{
    public partial class PredictedGrades : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                System.Collections.Specialized.NameValueCollection coll = Request.QueryString;
                if (coll.Count > 0)
                {

                    GroupListBox.Visible = true;
                    Label1.Visible = true;
                    String[] arr = coll.GetValues(0);
                    
                    if (coll.Keys[0]=="Set")
                    {
                        TextBox1.Text = arr[0];//remember set name
                        GroupListBox.Visible = false;
                        ListSet(arr[0]);
                    }
                    else
                    {
                        //is a boy...
                        TextBox2.Text = arr[0];//remember  ID
                        EnterResult(arr[0]);
                        arr = coll.GetValues(2);
                        TextBox1.Text = arr[0];
                        arr = coll.GetValues(3);
                        TextBox3.Text=arr[0];
                    }
                }
                UpDateGroupsListBox();
            }	
        }
        private void EnterResult(string ID)
        {
            SimplePupil p1 = new SimplePupil();
            p1.Load(ID);
            Label_Student.Text ="Predicted Grade for "+ p1.m_GivenName + " " + p1.m_Surname;
            Button_Astar.Visible = true;
            Button_A.Visible = true; Button_B.Visible = true;
            Button_C.Visible = true; Button_D.Visible = true;
            Button_E.Visible = true; Button_U.Visible = true;
            Label1.Visible = false; GroupListBox.Visible = false;
            Label_Student.Visible = true; Button1.Visible = false;
        }

        private void ListSet(string set)
        {
            string s = "";
            PupilGroupList pgl = new PupilGroupList();
            ResultsList rl1 = new ResultsList();
            pgl.AddToList(set, DateTime.Now);
            Response.Write("<TABLE BORDER>");
            string grade = "";
            //need to find course type....
            CourseList cl1 = new CourseList(5);
            foreach (Course c in cl1._courses)
            {
                if (c.CourseCode == set.Substring(2, 2).ToUpper()) TextBox3.Text = c._CourseID.ToString();
            }
            foreach (SimplePupil sp in pgl.m_pupilllist)
            {
                grade = "";
                rl1.m_parameters = 2; rl1.m_db_field2 = "ResultType"; rl1.m_value2 = "6";
                rl1.LoadList("dbo.tbl_Core_Students.StudentId", sp.m_StudentId.ToString());
                foreach (Result r in rl1._results)
                {
                    if (r.Code == set.Substring(2, 2).ToUpper())
                    {
                        grade=r.Value;
                    }
                }
                s = "PredictedGrades.aspx";
                s += "?ID=" + sp.m_StudentId.ToString() + "&Name=" + sp.m_GivenName + " " + sp.m_Surname;
                s += "& set=" + TextBox1.Text;
                s += "& course=" + TextBox3.Text;
                Response.Write("<TR><TD><A HREF=\""+s+"\">"+sp.m_GivenName+" "+sp.m_Surname+"</A></TD><TD>");

                Response.Write(grade+"</td></TR>");

            }
            Response.Write("</TABLE>");
            Button_A.Visible = false; Button_B.Visible = false;
            Button_C.Visible = false; Button_D.Visible = false;
            Button_E.Visible = false; Button_U.Visible = false;
            Label1.Visible = false; GroupListBox.Visible = false;
            Label_Student.Visible = false; Button1.Visible = false;
        }

        private void UpDateGroupsListBox()
        {
            GroupList gr = new GroupList("");
            GroupListBox.Items.Clear();
            PupilPeriodList ppl = new PupilPeriodList();
            string struser = Context.User.Identity.Name;
            //struser = @"challoners\cc";
            Utility u = new Utility();
            string s = "";
            //Guid personID = u.GetPersonID(struser,out s);
            Guid personID = u.GetPersonIdfromRequest(Request);
            s = u.GetsStaffCodefromRequest(Request);
            if (personID != Guid.Empty) struser = s ;
            //struser = "CC";
            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_Predicted_Grade_Type");
            s = c.Value;
            if (!c.valid)//try revert to config file
            {
                System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
                s = ar.GetValue("Predicted Grade Type", typeof(string)).ToString();
            }

            //if s =SL then do the lockdown mode for SL
            //if s = staff then do sets for staff
            //if s = ""... ignore...
            if (s == "SL")
            {
                this.Label1.Text = "Select Set. Only Subject Leaders can now change values.";
                //this code to list sets of a subject...
                if ((struser.Trim() == "CC") || (struser.Trim() == "DCO")) struser = "";//allow all sets
                GroupList_SL grsl = new GroupList_SL(struser);
                foreach (Group g in grsl._groups)
                {
                    if ((g._StartDate < DateTime.Now) && (g._EndDate > DateTime.Now) && (g._GroupCode.StartsWith("13")))
                    {
                        ListItem Item = new ListItem(g._GroupCode, g._GroupID.ToString());
                        GroupListBox.Items.Add(Item);
                    }
                }
            }

            if (s == "STAFF")
            {
                this.Label1.Text = "Select Set ";
                //this code lists sets the user is timetabled to teach....
                ppl.LoadList("StaffCode", struser, false, DateTime.Now);
                bool add = false;
                foreach (Group g in gr._groups)
                {
                    add = false;
                    if ((g._StartDate < DateTime.Now) && (g._EndDate > DateTime.Now))
                    {
                        foreach (ScheduledPeriod p in ppl.m_pupilTTlist)
                        {
                            if ((p.m_groupcode == g._GroupCode) && (g._GroupCode.StartsWith("13"))) add = true;
                        }
                        if (add)
                        {
                            ListItem Item = new ListItem(g._GroupCode, g._GroupID.ToString());
                            GroupListBox.Items.Add(Item);
                        }
                    }
                }
            }

            GroupListBox.SelectedIndex = 0;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string s = "Set="+GroupListBox.SelectedItem.Text;
            Server.Transfer("../content/PredictedGrades.aspx?" + s);
        }
        protected void ButtonAstar_Click(object sender, EventArgs e)
        {
            Update("A*");
        }

        protected void ButtonA_Click(object sender, EventArgs e)
        {
            Update("A");
        }
        protected void ButtonB_Click(object sender, EventArgs e)
        {
            Update("B");
        }
        protected void ButtonC_Click(object sender, EventArgs e)
        {
            Update("C");
        }
        protected void ButtonD_Click(object sender, EventArgs e)
        {
            Update("D");
        }
        protected void ButtonE_Click(object sender, EventArgs e)
        {
            Update("E");
        }
        protected void ButtonU_Click(object sender, EventArgs e)
        {
            Update("U");
        }
        protected void Update(string grade)
        {
            SimplePupil p1 = new SimplePupil();
            p1.Load(TextBox2.Text);
            bool found = false;
            string s;
            string s1 = "";
            ResultsList rl1 = new ResultsList();
            rl1.m_parameters = 2; rl1.m_db_field2 = "ResultType"; rl1.m_value2 = "6";
            rl1.LoadList("dbo.tbl_Core_Students.StudentId", p1.m_StudentId.ToString());
            foreach (Result r in rl1._results)
            {
                if (r.CourseID.ToString()==TextBox3.Text)
                {
                    // we have a value...
                    //updtae 
                    found = true;
                    s = "UPDATE tbl_Core_Results SET ResultValue='" + grade + "' ";
                    s += "WHERE ResultID = '" + r.ResultID.ToString() +"'";
                    Encode en1 = new Encode();
                    en1.ExecuteSQL(s);

                    s1 = "Prediction for " + p1.m_GivenName + " " + p1.m_Surname + "(" + p1.m_adno + ")  for ";
                    s1 += r.Coursename + " changed from " + r.Value + "  to  " + grade +".   Change made by ";
                    s1 += Context.User.Identity.Name + " on   " + DateTime.Now.ToLongDateString();
                    s1 += "   at  " + DateTime.Now.ToShortTimeString();
                }
            }
            if (!found)
            {
                s= "INSERT INTO dbo.tbl_Core_Results (ResultType, StudentID, ResultValue, ResultDate, ResultText, ";
                s += "CourseId  )";
                s += " VALUES ( '6', '";
                s += p1.m_StudentId.ToString() + "', '";
                s += grade + "', ";
                DateTime d0 = new DateTime();
                d0 = DateTime.Now;//going to normalise to 17th september in this year....
                DateTime d1 = new DateTime(d0.Year, 9, 17);
                s += " CONVERT(DATETIME, '" + d1.ToString("yyyy-MM-dd HH:mm:ss") + "', 102), '";
                s += " ',  '";
                s += TextBox3.Text + "' )";
                Encode en1 = new Encode();
                en1.ExecuteSQL(s);
                s1 = "New Prediction for " + p1.m_GivenName + " " + p1.m_Surname + "(" + p1.m_adno + ")  for ";
                s1 +=  TextBox1.Text +"    Grade =  " + grade + ".   Change made by ";
                s1 += Context.User.Identity.Name + " on   " + DateTime.Now.ToLongDateString();
                s1 += "   at  " + DateTime.Now.ToShortTimeString();
            }
            MailHelper mail1 = new MailHelper();
            Utility u = new Utility();
            //string from_address = u.GetStaffCodefromContext(Context);
            string from_address = u.GetsStaffCodefromRequest(Request);
            from_address = u.Get_StaffEmailAddress(from_address);
            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_Predicted_Grade_Notification");
            s = c.Value;
            if (!c.valid)//try revert to config file
            {
                System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
                s = ar.GetValue("Predicted Grade Type", typeof(string)).ToString();
            }


            if (s != "")
            {
                mail1.SendMail(from_address, s, null, s1, "cc@challoners.com", "A2 Prediction changed - sent of behalf of :"+from_address);
            }
            Server.Transfer("../content/PredictedGrades.aspx?Set=" + TextBox1.Text);
        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
