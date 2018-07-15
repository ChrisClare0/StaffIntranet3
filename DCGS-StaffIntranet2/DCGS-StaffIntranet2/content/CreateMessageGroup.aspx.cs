using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class CreateMessageGroup : System.Web.UI.Page
    {
        protected string PostBackStr_MakeGroup;
        protected void Page_Load(object sender, EventArgs e)
        {
            PostBackStr_MakeGroup = Page.ClientScript.GetPostBackEventReference(this, "MakeGroup");
            string s = Request.Params.Get("Ajaxcall");
            if (s == "1")
            {
                GroupList gl1 = new GroupList();
                Utility u = new Utility();

                //string staff_code = u.GetStaffCodefromContext(Context);
                string staff_code = u.GetsStaffCodefromRequest(Request);
                gl1.LoadList_StaffPrivateGroups(staff_code.Trim(), GroupList.GroupListOrder.GroupName);
                foreach (Group g in gl1._groups)
                {
                    string s2 = g._GroupCode;
                    s2 += g._GroupName;
                    Response.Write("<option value=\"" + g._GroupID.ToString() + "\">" + g._GroupName + "</option>");
                }
                Response.End(); return;
            }
            if (s == "3")
            {
                s = Request.Params.Get("GroupId");
                Guid g1 = new Guid(s);
                Group gp1 = new Group(); gp1.Load(g1);
                gp1.Delete();
                GroupList gl1 = new GroupList();
                Utility u = new Utility();
                //string staff_code = u.GetStaffCodefromContext(this.Context);
                string staff_code = u.GetsStaffCodefromRequest(Request);
                gl1.LoadList_StaffPrivateGroups(staff_code.Trim(), GroupList.GroupListOrder.GroupName);
                foreach (Group g in gl1._groups)
                {
                    string s2 = g._GroupCode;
                    s2 += g._GroupName;
                    Response.Write("<option value=\"" + g._GroupID.ToString() + "\">" + g._GroupName + "</option>");
                }
                Response.End(); return;
            }

            if (IsPostBack)
            {
                s = Request.Params.Get("__EVENTARGUMENT");
                if (s == "MakeGroup") ProcessMakeGroup(Request.Params.Get("__EVENTDATA"));
            }
        }

        protected void ProcessMakeGroup(string p)
        {
            string s = p;
            string[] parameters = new string[3];
            string[] splitchar = new string[2];
            //parameters are students%GroupName
            splitchar[0] = "%";
            parameters = p.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
            splitchar[0] = ":";splitchar[1] = ",";
            parameters = parameters[0].Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
            s = parameters[1];//has group name....

            Utility u = new Utility();
            //string s1=u.GetStaffCodefromContext(Context);
            string s1 = u.GetsStaffCodefromRequest(Request);
            Guid g = new Guid(u.Get_StaffID(s1));

            GroupList gl1 = new GroupList();
            gl1.LoadList_StaffPrivateGroups(s1, GroupList.GroupListOrder.GroupName);

            string code = s1 +"_"+ (gl1._groups.Count + 1).ToString();

            Group g1 = new Group();
            g1._CourseID = Cerval_Globals.newdawnCse;
            int y = DateTime.Now.Year;
            if (DateTime.Now.Month > 7) y++;
            g1._EndDate = new DateTime(y, 7, 31);
            g1._GroupCode = code;
            g1._GroupManagedBy = g;
            g1._GroupName = s;
            g1._GroupPrimaryAdministrative = false;
            g1._StartDate = DateTime.Now.AddDays(-2);
            g1.Save();

            //now need to insert into New Dawn stuff....

            Group g2 = new Group(); g2.Load("StaffGps", DateTime.Now);
            if (g2._valid)
            {
                GroupLink gll1 = new GroupLink();
                gll1.ParentId = g2._GroupID;
                gll1.ChildId = g1._GroupID;
                gll1.Save();
            }


        }

    }
}
