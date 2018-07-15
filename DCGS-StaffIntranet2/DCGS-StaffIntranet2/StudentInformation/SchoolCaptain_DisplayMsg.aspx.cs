using System;
using Cerval_Library;

namespace StudentInformation
{
    public partial class SchoolCaptain_DisplayMsg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Utility u = new Utility();

                Guid g1 =u.GetPersonIdfromRequest(Request);
                MessageList ml1 = new MessageList();
                MessageGroupList mgl1 = new MessageGroupList();
                Group group1 = new Group();
                PupilDetails pupil1 = new PupilDetails();
                MessageStudentList msl1 = new MessageStudentList();
                PersonDetails p2 = new PersonDetails(g1.ToString());
                ml1.LoadList_Person(g1,20);
                bool first = true;
                string s = "<h4>Last 20 messages sent from " + p2.m_GivenName+" "+p2.m_Surname + "</h4>";
                s += "<br><table  border  class=\"EventsTable\"  >";
                s += "<tr><th>Valid From</th><th>Message</th><th>To</th><th>Delivered</th></tr>";
                foreach (Message m1 in ml1.m_list)
                {
                    first = true;
                    s += "<tr><td>" + m1.ValidFrom.ToShortDateString() + "</td><td>" + m1.Msg + "</td>";
                    //fi nd groups
                    mgl1.LoadList_Message(m1.Id);
                    foreach (MessageGroup mg1 in mgl1.m_list)
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
                        s += "<td>" + pupil1.m_GivenName + " " + pupil1.m_Surname + "</td>";
                        if (ms1.Delivered) s += "<td>" + ms1.DateDelivered.ToString() + "</td>";
                        s += "</tr>";
                        first = false;
                    }

                }
                s += "</table>";
                content0.InnerHtml = s;
            }
        }
    }
}