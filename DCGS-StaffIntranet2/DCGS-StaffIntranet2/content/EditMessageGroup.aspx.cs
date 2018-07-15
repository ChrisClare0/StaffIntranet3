using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class EditMessageGroup : System.Web.UI.Page
    {
        protected string PostBackStr_MakeGroup;
        protected string GroupIDS;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            PostBackStr_MakeGroup = Page.ClientScript.GetPostBackEventReference(this, "MakeGroup");

            string s = Request.Params.Get("GroupID");
            if (s != null)
            {
                GroupIDS = s;
                //we are coming in from CreateGroup....  so Update the ID box
                GroupId.Value = s;         
            }
            
            s = Request.Params.Get("Ajaxcall");
            if (s == "2")
            {
                GroupIDS = Request.Params.Get("GroupID");
                //need to find group members for this...
                PupilGroupList pgl1 = new PupilGroupList();
                pgl1.AddToList(new Guid(GroupId.Value), DateTime.Now);
                foreach (SimplePupil p in pgl1.m_pupilllist)
                {
                    Response.Write("<option value=\"" + p.m_StudentId + "\">" +p.m_GivenName+" "+p.m_Surname + "</option>");
                }
                Response.End(); return;
            }
            if (IsPostBack)
            {
                s = Request.Params.Get("__EVENTARGUMENT");
                if (s == "MakeGroup") ProcessMakeGroup(Request.Params.Get("__EVENTDATA"));
            }
        }

        private void ProcessMakeGroup(string p1)
        {
            string[] parameters = new string[20];
            string[] splitchar = new string[2];
            string[] students = new string[200];
            //parameters are students%GroupName
            splitchar[0]="%";
            int no_students = 0;
            parameters = p1.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
            splitchar[0] = ":";
            splitchar[1] = ",";
            students = parameters[0].Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
            //so we have a list....how many?
            no_students = students.Length;students[0]="";
            //get current group members
            StudentGroupMembershipList sgl1 = new StudentGroupMembershipList();
            Guid g = new Guid(GroupIDS);
            sgl1.LoadList_Group(g, DateTime.Now);
            //now if not in list delete sgm or if in list delete from list;
            bool found = false;
            foreach (StudentGroupMembership sg in sgl1.m_list)
            {
                found = false;
                for (int i = 0; i < no_students; i++)
                {
                    if (sg.m_Studentid.ToString() == students[i])
                    {
                        found = true;students[i] = "";
                    }
                }
                if (!found)sg.Delete();
            }
            //now make new ones....
            int y = DateTime.Now.Year;
            if (DateTime.Now.Month > 7) y++;
            string id="";
            for (int i = 0; i < no_students; i++)
            {
                if (students[i] != "")
                {
                    id=students[i];
                    StudentGroupMembership sg = new StudentGroupMembership();
                    sg.m_Groupid = new Guid(GroupIDS);
                    sg.m_Studentid =  new Guid(students[i]);
                    sg.m_ValidTo = new DateTime(y, 7, 31);
                    sg.m_ValidFrom = DateTime.Now.AddDays(-1);
                    sg.Save();
                    //remove duplicates....
                    for (int j = 0; j < no_students; j++)
                    {
                        if (students[j] == id) students[j] = "";
                    }    
                }
            }


        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}