using System;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class iSAMS_Sync : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void ResetAllStudentGroupVersion(int version)
        {
            Encode en = new Encode();
            string d = "CONVERT(DATETIME, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 102)";
            string s = " UPDATE dbo.tbl_Core_Student_Groups SET Version='" + version.ToString() + "'  WHERE ";
            s += " (MemberFrom < " + d + " ) AND (MemberUntil > " + d + " )  ";
            en.ExecuteSQL(s);
        }

        protected void Button_synClick(object sender, EventArgs e)
        {
            //sync iSAMSGroups
            //first going to mark all current student_group memberships as version 10
            //then will be able to end all those not found in iSAMS
            //ResetAllStudentGroupVersion(10); //reset all current sgms to ver 10
            DateTime EndDate = new DateTime();
            EndDate = System.Convert.ToDateTime(TextBox_EndDate.Text);
            int year = System.Convert.ToInt32(TextBox_Year.Text);
            Utility u = new Utility();
            string errors = ""; bool found = false;
            ISAMS_Set_List Isetl1 = new ISAMS_Set_List();
            foreach (ISAMS_Set Is in Isetl1.m_list)
            {
                //for each set...
                if (Is.Year == year)
                {
                    ISAMS_Student_List Isl1 = new ISAMS_Student_List(Is.Id);
                    StudentGroupMembershipList sgml1 = new StudentGroupMembershipList();
                    Group g1 = new Group();
                    g1.Load(Is.SetCode, DateTime.Now);
                    if (!g1._valid)
                    {
                        //ought to try to make the group
                        errors += "Group not found in Cerval" + Is.SetCode + Environment.NewLine;
                    }
                    else
                    {
                        sgml1.LoadList_Group(g1._GroupID, DateTime.Now);
                        foreach (ISAMS_Student iSS1 in Isl1.m_list)
                        {
                            found = false;
                            foreach (StudentGroupMembership sgm1 in sgml1.m_list)
                            {
                                string iSAMSPupilId = u.GetStudentIsamsID("StudentId", sgm1.m_Studentid.ToString());
                                if (iSAMSPupilId == "") { errors += "Student not found " + sgm1.m_Studentid.ToString() + Environment.NewLine; }
                                if (iSS1.ISAMS_SchoolId == iSAMSPupilId)
                                {
                                    found = true;
                                    sgm1.ResetVersion(3);// it is OK
                                    break;
                                }
                            }
                            if (!found)
                            {
                                //need to add to sgml
                                StudentGroupMembership sgm2 = new StudentGroupMembership();
                                //need to find ID for this student for the iSAMSId
                                Guid g2 = u.GetStudentIDfromiSAMS(iSS1.ISAMS_SchoolId);
                                if (g2 == Guid.Empty) { errors += " Student not found in Cerval for iSAMSID " + iSS1.ISAMS_SchoolId + Environment.NewLine; }
                                else
                                {
                                    sgm2.m_ValidFrom = DateTime.Now.AddHours(-10);
                                    sgm2.m_ValidTo = EndDate;
                                    sgm2.m_Studentid = g2;
                                    sgm2.m_Groupid = g1._GroupID;
                                    //version=4
                                    sgm2.Save();
                                }
                            }
                            errors += "Cmpleted set " + Is.SetCode + "  at   " + DateTime.Now.ToLongTimeString() + Environment.NewLine;
                        }
                        //so that iSAMS set is done....
                    }
                    errors += "Cmpleted set " + Is.SetCode + "  at   " + DateTime.Now.ToLongTimeString() + Environment.NewLine;
                }

            }
            //done iSAMS sets...
            // so those at version 10 need closing down......


            TextBox_Out.Text = errors;

            Encode en = new Encode();
            string d = " CONVERT(DATETIME, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 102) ";
            string s = " UPDATE dbo.tbl_Core_Student_Groups SET MemberUntil = " + d + "  WHERE Version ='10' ";
            //en.ExecuteSQL(s);

        }
    }
}