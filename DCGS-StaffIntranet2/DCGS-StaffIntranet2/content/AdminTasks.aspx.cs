using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;
namespace DCGS_Staff_Intranet2.content
{
    public partial class AdminTasks : System.Web.UI.Page
    {
        public string Type//mark as null or empty if new...
        {
            get { return ((ViewState["Type"] == null) ? "" : (string)ViewState["Type"]); }
            set { ViewState["Type"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Type = Request.QueryString["Type"];    
                StatsControl1.type = StatsControl.StatsType.School;
                Display_List.Items.FindByValue("Population").Selected = true;
                RadioButtonList2.Visible = Display_List.Items.FindByValue("Population").Selected;
                RadioButtonList3.Visible = Display_List.Items.FindByValue("Permissions").Selected;
                Utility u = new Utility();
                Display_List.Items.FindByValue("SEN").Enabled = (u.CheckStaffInConfigGroup(Context, "SEN-MANAGERS") ? true : false);

                if (Type == "Medical")
                {
                    Display_List.Items.FindByValue("Medical").Selected = true;
                    MedicalList();
                }
                //if Type=MedicalEdit we don't want display list
                if (Type == "MedicalEdit")
                {
                    string studid=Request.QueryString["StudentId"];
                    MedicalEditControl1.StudentId = new Guid(studid);
                    Display_List.Visible = false;
                    MedicalEditControl1.Visible = true;
                    MedicalEditControl1.CallBackPage = "../content/AdminTasks.aspx";
                    Button_UpdateMedicalNotes.Visible = true;
                    Button_CancelUpdateMedicalNotes.Visible = true;
                }

#if DEBUG
                Button_complexemail.Visible = true;
                string staff_code = u.GetsStaffCodefromRequest(Request).Trim();
                if (!u.Is_staff)
                {
                    if ((staff_code == "CC") || (staff_code == "ENI"))
                    {
                        Button_complexemail.Visible = true;
                    }
                    if ((staff_code == "CC") || (staff_code == "DCO"))
                    {
                        DCO_div.Visible = true;                       
                    }
                }
#else

                //string staff_code = u.GetStaffCodefromContext(Context);
                string staff_code = u.GetsStaffCodefromRequest(Request).Trim();
                if (!u.Is_staff)
                {
                    if ((staff_code == "CC") || (staff_code == "ENI"))
                    {
                        Button_complexemail.Visible = true;
                    }
                    if ((staff_code == "CC") || (staff_code == "DCO"))
                    {
                        DCO_div.Visible = true;                       
                    }
                }

#endif
            }
        }
        protected bool CompareFormName(string s1, string s2)
        {
            //true if s1>s2
            int i1, i2;
            if (s1.StartsWith("1")) i1 = System.Convert.ToInt32(s1.Substring(0, 2)); else i1 = System.Convert.ToInt32(s1.Substring(0, 1));
            if (s2.StartsWith("1")) i2 = System.Convert.ToInt32(s2.Substring(0, 2)); else i2 = System.Convert.ToInt32(s2.Substring(0, 1));
            if (i1 < i2) return false;
            if (i1 > i2) return true;
            //ok so same year so we can compare strings
            if (string.Compare(s1, s2) > 0) return true;
            return false;
        }
        protected void Display_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            content0.InnerHtml = "";
            SENListControl1.Visible  = Display_List.Items.FindByValue("SEN").Selected;       
            StatsControl1.Visible    = Display_List.Items.FindByValue("Population").Selected;
            RadioButtonList2.Visible = Display_List.Items.FindByValue("Population").Selected;
            RadioButtonList3.Visible = Display_List.Items.FindByValue("Permissions").Selected;
            if (Display_List.Items.FindByValue("SEN").Selected)
            {
                TextBox_mask.Visible = true;
                Display_List.Visible = false;
            }
            if (Display_List.Items.FindByValue("Permissions").Selected)
            {
                PermissiontoLeave(RadioButtonList3.SelectedValue);
            }
            if (Display_List.Items.FindByValue("Sanctions").Selected)
            {
                Server.Transfer("UtilitySanctionsNotCompleted.aspx");
            }
            if (Display_List.Items.FindByValue("Medical").Selected)
            {
                MedicalList();
            }
            if (Display_List.Items.FindByValue("AC1").Selected)
            {
                AC1List();
            }
        }
        protected void RadioButtonList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList2.Items.FindByValue("School").Selected) StatsControl1.type = StatsControl.StatsType.School;
            if (RadioButtonList2.Items.FindByValue("ByYear").Selected) StatsControl1.type = StatsControl.StatsType.Year;
            if (RadioButtonList2.Items.FindByValue("ByForm").Selected) StatsControl1.type = StatsControl.StatsType.Form;
            if (RadioButtonList2.Items.FindByValue("Not in Form Group").Selected) StatsControl1.type = StatsControl.StatsType.NotOnRole;
        }
        protected void RadioButtonList3_SelectedIndexChanged(object sender, EventArgs e)
        {
            PermissiontoLeave(RadioButtonList3.SelectedValue);
        }
        private   void PermissiontoLeave(string stype)
        {
            int i = System.Convert.ToInt32(stype);
            bool title_found = false;
            content0.InnerHtml = "";
            SimpleStudentList sl = new SimpleStudentList("");
            StudentConsent sc1 = new StudentConsent();
            string s = "";
            string s2 = "<p style=\"font-size:small \"> <table align =center  border  style=\"font-size:x-small \">";
            s2 += "<tr><td>Surname</td><td>FirstName</td><td>Year</td><td>Form</td><td>";
            sc1.Load( ((SimplePupil)sl._studentlist[0]).m_StudentId.ToString(),i);
            s="</td></tr>";
            foreach (SimplePupil p1 in sl)
            {
                s+="<tr><td>" + p1.m_Surname + "</td><td>" + p1.m_GivenName + "</td><td>" + p1.m_year.ToString() + "</td><td>" + p1.m_form + "</td>";
                if (sc1.Load(p1.m_StudentId.ToString(), i))
                {
                    s+="<td>Permission granted " + sc1.m_ConsentFrom.ToShortDateString() + "</td>";
                    if (!title_found) { s2 += sc1.m_ConsentDescription; title_found = true; }
                }
                else
                {
                    s+="<td>NONE</td>";
                }
                s+="</tr>";
            }
            s+="</table></p>";
            content0.InnerHtml = s2+s;
        }
        private void AC1List()
        {
            SimpleStudentList sstl1 = new SimpleStudentList("");
            int no_on_role = sstl1._studentlist.Count;
            double p = new double();p=0;
            string s = "<p style=\"font-size:small \"> <table align =center  border  style=\"font-size:x-small \">";
            s += "<tr><td>Item</td><td>Number</td><td>Percentage</td><td>.....</td></tr>";
            Utility u = new Utility();
            int n = 0; int n1 = 0;
            s += "<tr><td>On Role</td><td>" + no_on_role.ToString() + "</td><td>100</td><td></td></tr>";
            sstl1.LoadList_FreeMealsOnly();
            n = sstl1._studentlist.Count;
            p=100*((double)n/(double)no_on_role);
            s += "<tr><td>";
            s += (u.CheckStaffInConfigGroup(Context, "AdminAccess") ? "<A HREF=\"PlainResponseForm.aspx?Type=FSM\">FSM</A>" : "FSM");
            s+="</td><td>" + n.ToString() + "</td><td>"+p.ToString()+"</td><td></td></tr>";

            n = 0;
            foreach (SimplePupil p1 in sstl1._studentlist)
            {
                if (p1.m_year < 12) n++;
            }
            
            p = 100 * ((double)n / (double)no_on_role);
            s += "<tr><td>FSM Y7-11</td><td>" + n.ToString() + "</td><td>" + p.ToString() + "</td><td></td></tr>";

            sstl1.LoadHMFList(); n = sstl1._studentlist.Count;
            p = 100 * ((double)n / (double)no_on_role);
            s += "<tr><td>HMF</td><td>" + n.ToString() + "</td><td>" + p.ToString() + "</td><td></td></tr>";

        


            sstl1.LoadList_SENOnly(3);
            n = sstl1._studentlist.Count; n1 = 0;
            foreach (SimplePupil p1 in sstl1._studentlist){if (p1.m_year < 12) n1++;}
            p = 100 * ((double)n / (double)no_on_role);
            s += "<tr><td>Shool Action</td><td>" + n.ToString() + "</td><td>" + p.ToString() + "</td><td></td></tr>";

            sstl1.LoadList_SENOnly(4);
            foreach (SimplePupil p1 in sstl1._studentlist) { if (p1.m_year < 12) n1++; }
            n += sstl1._studentlist.Count; 
            p = 100 * ((double)sstl1._studentlist.Count / (double)no_on_role);
            s += "<tr><td>Shool Action+</td><td>" + sstl1._studentlist.Count.ToString() + "</td><td>" + p.ToString() + "</td><td></td></tr>";

            p = 100 * ((double)n / (double)no_on_role);
            s += "<tr><td>SA and SA+</td><td>" + n.ToString() + "</td><td>" + p.ToString() + "</td><td></td></tr>";
            
            p = 100 * ((double)n1 / (double)no_on_role);
            s += "<tr><td>SA and SA+in Y7-11</td><td>" + n1.ToString() + "</td><td>" + p.ToString() + "</td><td></td></tr>";
            

            s += "</table></p>";
            content0.InnerHtml = s;

        }
        private void MedicalList()
        {
            SimpleStudentList sstl1 = new SimpleStudentList("");
            string s = "<p style=\"font-size:small \"> <table align =center  border  style=\"font-size:x-small \">";
            s += "<tr><td>Surname</td><td>FirstName</td><td>Medical</td><td>Edit</td></tr>";
            foreach (SimplePupil p1 in sstl1)
            {
                StudentMedical sm1 = new StudentMedical();
                sm1.Load(p1.m_StudentId.ToString());
                s += "<tr><td>" + p1.m_Surname + "</td><td>" + p1.m_GivenName + "</td>";
                s += "<td>" + p1.m_adno.ToString() + "</td>";
                s += "<td>" + sm1.m_MedicalNotes + "</td>";
                s +="<td><A HREF=\"";
                s += "../content/AdminTasks.aspx";
                s += "?Type=MedicalEdit&StudentId=" + p1.m_StudentId.ToString();
                s += "\">Edit</A></td>";
                s += "</tr>";
            }
            s += "</table></p>";
            content0.InnerHtml = s;
        }

        protected void TextBox_mask_TextChanged(object sender, EventArgs e)
        {
            //HMM... SO SELECT by surname...
            SENListControl1.SurnameMask = TextBox_mask.Text;
        }

        protected void Button_CancelUpdateMedicalNotes_Click(object sender, EventArgs e)
        {
            MedicalEditControl1.Visible = false;
            Button_CancelUpdateMedicalNotes.Visible = false;
            Button_UpdateMedicalNotes.Visible = false;
            Response.Redirect("../content/AdminTasks.aspx?Type=Medical");
        }

        protected void Button_UpdateMedicalNotes_Click(object sender, EventArgs e)
        {
            MedicalEditControl1.UpdateEdit();
            MedicalEditControl1.Visible = false;
            Button_CancelUpdateMedicalNotes.Visible = false;
            Button_UpdateMedicalNotes.Visible = false;
            Response.Redirect("../content/AdminTasks.aspx?Type=Medical");
        }

        protected void Button_complexemail_Click(object sender, EventArgs e)
        {
            Server.Transfer("../content/ComplexEmail.aspx");
        }

        protected void Button_DCO_Click(object sender, EventArgs e)
        {

        }
    }
}
