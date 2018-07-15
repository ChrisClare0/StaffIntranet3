using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class StaffMessages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Utility u = new Utility();
            //string staff = u.GetStaffCodefromContext(Context);
            string staff = u.GetsStaffCodefromRequest(Request);
            Guid g1 = new Guid(u.Get_StaffID(staff));
            MessageList ml1 = new MessageList();
            MessageGroupList mgl1 = new MessageGroupList();
            Group group1 = new Group();
            PupilDetails pupil1 = new PupilDetails();
            MessageStudentList msl1 = new MessageStudentList();
            ml1.LoadList_Staff(g1,10);
            bool first = true;
            string s = "<h4>Last 10 messages sent from "+staff+"</h4>";
            s += "<br><table  border  class=\"EventsTable\"  >";
            s += "<tr><th>Valid From</th><th>Message</th><th>To</th><th>Delivered</th></tr>";
            foreach (Message m1 in ml1.m_list)
            {
                first = true;
                s += "<tr><td>"+m1.ValidFrom.ToShortDateString()+"</td><td>"+m1.Msg+"</td>";
                //fi nd groups
                mgl1.LoadList_Message(m1.Id);
                foreach(MessageGroup mg1 in mgl1.m_list)
                {
                    group1.Load(mg1.GroupId);
                    if (!first) s += "<tr><td></td><td></td>";
                    s += "<td>" + group1._GroupCode + "</td>";
                    if (mg1.Delivered) s += "<td>" + mg1.DateDelivered + "</td>";
                    s += "</tr>";
                    first = false;
                }
                msl1.LoadList_Message(m1.Id);
                foreach (MessageStudent ms1 in msl1.m_list)
                {
                    pupil1.Load(ms1.StudentId.ToString());
                    if (!first) s += "<tr><td></td><td></td>";
                    s += "<td>" + pupil1.m_GivenName+" "+pupil1.m_Surname+ "</td>";
                    if (ms1.Delivered) s += "<td>" + ms1.DateDelivered.ToString()  + "</td>";
                    s += "</tr>";
                    first = false;
                }

            }
            s += "</table>";
            content0.InnerHtml = s;

        }
    }
}
