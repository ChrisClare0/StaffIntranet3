using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet
{
    public partial class DisplayMessages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
            else
            {
            }
        }

        protected void SetCellStyle(TableCell c1, Table tb1)
        {
            c1.BorderStyle = BorderStyle.Solid;
            c1.BorderWidth = tb1.BorderWidth;
            c1.BorderColor = tb1.BorderColor;
        }

        private void DisplayGroupMsg(Guid GroupId)
        {
            Group gr1 = new Group();
            gr1.Load(GroupId);
            DisplayGroupMSgG(gr1);
        }

        private void DisplayGroupMsg(string GroupCode)
        {
            Group gr1 = new Group();
            gr1.Load(GroupCode, DateTime.Now);
            DisplayGroupMSgG(gr1);
        }

        private void DisplayGroupMSgG(Group gr1)
        {
            Table1.Visible = true;
            Table tb1 = (Table)form1.FindControl("table1");tb1.Width = 400;tb1.EnableViewState = true;
            tb1.Controls.Clear();
            TableRow r0 = new TableRow();tb1.Controls.Add(r0);
            TableCell c01 = new TableCell(); r0.Controls.Add(c01); SetCellStyle(c01, tb1);c01.Text = "For";
            TableCell c02 = new TableCell(); r0.Controls.Add(c02); SetCellStyle(c02, tb1);c02.Text = "From";
            TableCell c03 = new TableCell(); r0.Controls.Add(c03); SetCellStyle(c03, tb1);c03.Text = "Message";
            TableCell c04 = new TableCell(); r0.Controls.Add(c04); SetCellStyle(c04, tb1);c04.Text = "Deliv'd";
            bool display = false;
            MessageGroupList mlg1 = new MessageGroupList();mlg1.LoadList(gr1._GroupID);
            foreach (MessageGroup m1 in mlg1.m_list)
            {
                display = true;
                if (m1._Message.ValidFrom > DateTime.Now) display = false;
                if (m1._Message.ValidUntil < DateTime.Now) display = false;
                //so is valid....
                if (!CheckBox1.Checked) display = !m1.Delivered;//only display old msgs if checked....
                if (display)
                {
                    TableRow r1 = new TableRow();
                    AddMsgRow(tb1, r1, m1.Id.ToString(), m1._Message, gr1._GroupCode, true,!m1.Delivered,m1.DateDelivered);
                }
            }
            MessageStudentList ml1 = new MessageStudentList();
            ml1.LoadList_Group(gr1._GroupID, DateTime.Now);
            SimplePupil pupil1 = new SimplePupil();
            foreach (MessageStudent m1 in ml1.m_list)
            {
                display = true;
                if (m1._Message.ValidFrom > DateTime.Now) display = false;
                if (m1._Message.ValidUntil < DateTime.Now) display = false;
                if (!CheckBox1.Checked) display = !m1.Delivered;//only display old msgs if checked....
                if (display)
                {
                    pupil1.Load(m1.StudentId);
                    TableRow r1 = new TableRow();
                    AddMsgRow(tb1, r1, m1.Id.ToString(), m1._Message, pupil1.m_GivenName + " " + pupil1.m_Surname, false,!m1.Delivered, m1.DateDelivered);
                }
            }
        }

        void AddMsgRow(Table tb1,TableRow r1, string Id,Message msg,string msgTo, bool msgGroup, bool current,DateTime Datedelivered)
        {
            Utility u = new Utility();
            tb1.Controls.Add(r1);
            r1.BorderStyle = BorderStyle.Solid;
            r1.BorderColor = tb1.BorderColor;
            r1.BorderWidth = tb1.BorderWidth;

            TableCell c1 = new TableCell(); r1.Controls.Add(c1); SetCellStyle(c1, tb1); c1.Text = msgTo;
            TableCell c2 = new TableCell(); r1.Controls.Add(c2); SetCellStyle(c2, tb1);
            if (msg.StaffId != Guid.Empty)
            {
                c2.Text = u.Get_StaffCodefromStaffID(msg.StaffId);
            }
            else
            {
                c2.Text = "School Captain";

            }

            TableCell c3 = new TableCell(); r1.Controls.Add(c3); SetCellStyle(c3, tb1); c3.Text = msg.Msg;
            //if (msg.DocumentURL != "") c3.Text += "<a href=\"" + msg.DocumentURL + "\">:Link</a>";

            if (current)
            {
                CheckBox cb1 = new CheckBox(); cb1.ID = Id; cb1.AutoPostBack = true;
                cb1.CheckedChanged += new EventHandler(cb1_CheckedChanged);
                TableCell c4 = new TableCell(); r1.Controls.Add(c4); SetCellStyle(c4, tb1); c4.Controls.Add(cb1);
                if (!msgGroup)
                    cb1.ToolTip = "Delivered to Individual";
                else
                    cb1.ToolTip = "Delivered to Group";//used to distinguish groups and students
            }
            else
            {
                TableCell c4 = new TableCell(); r1.Controls.Add(c4); SetCellStyle(c4, tb1); c4.Text = Datedelivered.ToShortDateString();
            }
        }

        void cb1_CheckedChanged(object sender, EventArgs e)
        {
            //should be a checkBox
            //try
            //Response.Write("We are here!!!");
            {
                CheckBox c = (CheckBox)sender;
                Table tb1= (Table) c.Parent.Parent.Parent;
                TableRow tr1 = (TableRow)c.Parent.Parent;
                if (c.Checked)
                {
                    string s = c.ToolTip;
                    Guid g = new Guid(c.ID);
                    if (s.ToUpper().Contains("GROUP"))
                    {
                        MessageGroup m = new MessageGroup();
                        m.Load(g);
                        m.DateDelivered = DateTime.Now;
                        m.Delivered = true;
                        m.Save();
                    }
                    else
                    {
                        MessageStudent m = new MessageStudent();
                        m.Load(g);
                        m.DateDelivered = DateTime.Now.AddMinutes(-5);
                        m.Delivered = true;
                        m.Save();
                    }
                }
                tb1.Controls.Remove(tr1);
            }
            //going to redraw table..
            //DisplayGroupMsg(GroupCode.Value);
            //catch { }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
        #endregion
        private void InitializeComponent()
        {
            Table1.Width = 200;
            string s = Request.QueryString["Type"];
            string s1 = "";
            if (s == "Group")
            {
                s1 = Request.QueryString["GroupId"];
                if (s1 != null)
                {
                    Guid g = new Guid(s1);
                    DisplayGroupMsg(g);
                }
                s1 = Request.QueryString["GroupCode"];
                DisplayGroupMsg(s1);
                GroupCode.Value = s1;
            }
            if (s == "Student")
            {
                s1 = Request.QueryString["StudentId"];
            }

        }



        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            DisplayGroupMsg(GroupCode.Value);
            //Response.Write("We are here!!!");
        }

    }
}
