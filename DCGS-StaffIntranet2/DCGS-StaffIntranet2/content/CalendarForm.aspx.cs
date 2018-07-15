using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Net;

using System.IO;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class CalendarForm : System.Web.UI.Page
    {

        CalendarEventList EventList = new CalendarEventList();
        string path = "";

        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
        path=@"u:\beta-challoners\DCGS.UI.TV\content\calendar_staff";
#else
        path=@"d:\beta-challoners\DCGS.UI.TV\content\calendar_staff";
#endif
            if (!IsPostBack)
            {
#if DEBUG
                string s = "";string url = "https://www.google.com/calendar/feeds/challoners.org_feci78hr5nl5dils4thcg4lot4%40group.calendar.google.com/private-85fb073b5e78ff584fd6d3172a2f1a27/basic";
                using(WebClient client = new WebClient()) 
                {s = client.DownloadString(url);}
                XmlDocument doc = new XmlDocument();
                XmlTextReader x1 = new XmlTextReader(url);
                doc.Load(url);
                string s1 = ""; 
                XmlNodeList nodes1 = doc.DocumentElement.SelectNodes("entry");
                XmlNodeList nodes4 = doc.DocumentElement.ChildNodes;
                foreach (XmlNode node in nodes4)
                {
                    if (node.Name == "entry")
                    {
                        XmlNodeList nodes5 = node.ChildNodes; s1 += "</br>";
                        foreach (XmlNode n1 in nodes5)
                        {
                            s = n1.Name;
                            if (n1.Name == "summary")
                            {
                                s1 += n1.InnerText;
                            }
                            if (n1.Name == "title")
                            {
                                s1 += n1.InnerText;
                            }
                        }
                    }

                }
#else
#endif

                Response.Redirect("../content/Calendar_new.aspx");  //to new Google calander
                EventList.LoadXML_months(path, DateTime.Now.Month, 1, DateTime.Now.Year);
                ViewState.Add("EventList", EventList);
                ViewState.Add("path", path);
                //Label_Heading.Text = "Click on a day to display detail";
            }
        }

        private bool Display_CalendarItem(CalendarEvent ev)
        {
            if (CheckBoxList_Categories.Items.FindByText("All").Selected) return true;
            if (CheckBoxList_Categories.Items.FindByText("Events").Selected && (ev.Categories.Contains("Event"))) return true;
            if (CheckBoxList_Categories.Items.FindByText("Sports Fixtures").Selected && (ev.Categories.Contains("Match"))) return true;
            if (CheckBoxList_Categories.Items.FindByText("Routine").Selected && (ev.Categories.Contains("Routine"))) return true;
            if (CheckBoxList_Categories.Items.FindByText("Meeting").Selected && (ev.Categories.Contains("Meeting"))) return true;
            if (CheckBoxList_Categories.Items.FindByText("Visit").Selected && (ev.Categories.Contains("Visit"))) return true;
            return false;
        }

        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            EventList = (CalendarEventList )ViewState["EventList"];
            path = (string)ViewState["path"];
            if (EventList == null) EventList.LoadXML_months(path, DateTime.Now.Month, 1, DateTime.Now.Year);
            DateTime t = e.Day.Date;
            e.Day.IsSelectable = false;
            foreach (CalendarEvent ev in EventList.EventList)
            {
                if ((ev.date.Day == t.Day)&&(ev.date.Month==t.Month)&&Display_CalendarItem(ev))
                {
                    string s = ev.title; if (s.Length > 14) s = s.Substring(0, 14)+"...";
                    e.Day.IsSelectable = true;
                    e.Cell.ForeColor = System.Drawing.Color.Black;
                    Label l1 = new Label(); 
                    l1.Text = "<BR>"+s ;
                    l1.Font.Size = FontUnit.XSmall;
                    e.Cell.Controls.Add(l1);
                    if(ev.Categories.Contains("Event"))l1.ForeColor = System.Drawing.Color.Red;
                    if (ev.Categories.Contains("Meeting")) l1.ForeColor = System.Drawing.Color.Green;
                    if (ev.Categories.Contains("Match")) l1.ForeColor = System.Drawing.Color.Chocolate;
                    if (ev.Categories.Contains("Visit")) l1.ForeColor = System.Drawing.Color.Blue;
                }
            }
        }

        private string DisplayEventsAsTable(List<CalendarEvent> list,bool ShowDate)
        {
            if (list.Count == 0) return "";
            string s = "<br><table  border style=\"font-size:smaller; \" class=\"EventsTable\" >";
            s += "<tr>";
            if (ShowDate) s += "<td>Date</td>";
            s+="<td>Start</td><td>End</td><td>Title</td><td>Location</td><td>Description</td></tr>";
            foreach (CalendarEvent ev in list)
            {
                        //add this event...
                        s += "<tr><td>";
                        if (ShowDate) s += ev.date.ToShortDateString() + "</td><td>";
                        if (ev.start != null) s += ev.start.ToString();
                        s += "</td><td>";
                        if (ev.end != null) s += ev.end.ToString();
                        s += "</td><td>";
                        if (ev.title != null) s += ev.title;
                        s += "</td><td>";
                        if (ev.location != null) s += ev.location.ToString();
                        s += "</td><td>";
                        if (ev.description != null) s += ev.description;
                        s += "</td></tr>";
            }
            s += "</table>";
            return s;
        }

        private void UpdateTable()
        {
            if (RadioButtonList2.Items.FindByText("Display").Selected)
            {
                List<CalendarEvent> EventFound = new List<CalendarEvent>();
                DateTime t1 = Calendar1.SelectedDate;
                //Label_Heading.Text = "Events for " + t1.ToLongDateString();
                EventList = (CalendarEventList)ViewState["EventList"];
                foreach (CalendarEvent ev in EventList.EventList)
                {
                    if ((ev.date.Month == t1.Month) && (ev.date.Day == t1.Day))
                    {
                        if (Display_CalendarItem(ev))
                        {
                            EventFound.Add(ev);
                        }
                    }
                }
                servercontent.InnerHtml = DisplayEventsAsTable(EventFound,false);
            }
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateTable();
        }

        protected void Calendar1_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            EventList.EventList.Clear();
            ViewState.Remove("EventList");
            path = (string)ViewState["path"];
            EventList.LoadXML_months(path, e.NewDate.Month, 1, e.NewDate.Year);
            ViewState.Add("EventList", EventList);
        }

        protected void CheckBoxList_Categories_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calendar1.SelectedDate = Calendar1.SelectedDate;
            UpdateTable();

        }

        protected void RadioButtonList2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if(RadioButtonList2.Items.FindByText("Display").Selected)
            {
                Calendar1.Visible = true;
                CheckBoxList_Categories.Visible = true;
                SearchStuff.Visible = false;
                //Label_Heading.Text = "Click on a day to display detail";
                Label3.Visible = true;
            }
            if (RadioButtonList2.Items.FindByText("Search").Selected)
            {
                Calendar1.Visible = false;
                CheckBoxList_Categories.Visible = false;
                SearchStuff.Visible = true;
                servercontent.InnerHtml = "";
               // Label_Heading.Text= "Enter search string";
                Label3.Visible = false;
            }
        }

        protected void Button_SearchForward_Click(object sender, EventArgs e)
        {
            string s = TextBox1.Text.ToLower();
            List<CalendarEvent> EventFound = new List<CalendarEvent>();
            int n = 6;
            try
            {
                n = System.Convert.ToInt32(TextBox_monthstosearch.Text);
            }
            catch
            {
            }
            path = (string)ViewState["path"];
            EventList.LoadXML_months(path, DateTime.Now.Month, n, DateTime.Now.Year);
            foreach (CalendarEvent ev in EventList.EventList)
            {
                if (ev.title.ToLower().Contains(s))
                {
                    EventFound.Add(ev);
                }
            }
            servercontent.InnerHtml = DisplayEventsAsTable(EventFound,true);
            //now display;

           


        }
    }
}
