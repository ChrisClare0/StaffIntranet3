using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace PhysicsBookings.content.Booking
{
    public partial class BookingList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PhysicsBooking phb1 = new PhysicsBooking();
                string s = Request.Params.Get("BookingId");
                if (s != null) phb1.Load(s);
                else
                {
                    Utility u = new Utility();
                    phb1.DayId = System.Convert.ToInt16(Request.Params.Get("Day"));
                    phb1.StaffId = new Guid(u.Get_StaffID(Request.Params.Get("Staff")));
                    phb1.Date = System.Convert.ToDateTime(Request.Params.Get("Date"));//this is week beginning...
                    phb1.Date = phb1.Date.AddDays(phb1.DayId);
                    phb1.PeriodId = u.Get_PeriodId_fromcode(Request.Params.Get("Period"));
                    phb1.RoomId = u.Get_RoomId_fromCode(Request.Params.Get("Room"));
                    Group g1 = new Group(); g1.Load(Request.Params.Get("Group"), phb1.Date);
                    phb1.GroupId = g1._GroupID;
                    s = Request.Params.Get("Group");
                    if (s.StartsWith("10") || s.StartsWith("11")) RadioButtonList1.SelectedIndex = 1;
                    if (s.StartsWith("12") || s.StartsWith("13")) RadioButtonList1.SelectedIndex = 2;
                    if (s.StartsWith("7") || s.StartsWith("8") || s.StartsWith("9")) RadioButtonList1.SelectedIndex = 0;
                }
                DayList dlist1 = new DayList();
                s = dlist1.FindDayName_fromId(phb1.DayId);
                servercontent.InnerHtml = "<h3>Booking for " + s + " Period " + Request.Params.Get("Period") + " for Group " + Request.Params.Get("Group") + " and Room " + Request.Params.Get("Room") + "</h3>";
                if (phb1.FindBooking()) { Add_Controls(phb1); TextBox_Notes.Text = phb1.Notes; }
                ViewState.Add("PhysicsBooking", phb1);
                PhysicsEquipmentItemList list1 = new PhysicsEquipmentItemList();
                list1.LoadList_All();
                foreach (PhysicsEquipmentItem p in list1.m_list)
                {
                    ListItem l = new ListItem(p.EquipmentItemCode, p.EquipmentItemId.ToString());
                    DropDownList_Items.Items.Add(l);
                }
                UpdateExptList();
                //find previous booking???
                Label2.Visible = false; TextBox_last.Visible = false; Button_AddLast.Visible = false;
                PhysicsBooking phb2 = new PhysicsBooking();//for last booking for this group
                PhysicsBookingList list2 = new PhysicsBookingList();
                list2.LoadList_Group(phb1.GroupId);//these are ordered with lates first...
                foreach (PhysicsBooking b in list2.m_list)
                {
                    if (b.Date < phb1.Date)
                    {
                        Label2.Visible = true; TextBox_last.Visible = true; Button_AddLast.Visible = true;
                        PhysicsEquipmentItemList list3 = new PhysicsEquipmentItemList();
                        list3.LoadList(b); TextBox_last.Text = "";
                        foreach (PhysicsEquipmentItem i in list3.m_list) TextBox_last.Text += i.EquipmentItemCode + Environment.NewLine;
                        ViewState.Add("LastBooking", b);
                        break;
                    }
                }         
            }
            else
            {
                PhysicsBooking b2 = (PhysicsBooking)ViewState["PhysicsBooking"];
                Add_Controls(b2);
            }
        }
        protected void UpdateExptList()
        {
            DropDownList_Expt.Items.Clear();
            string s = RadioButtonList1.SelectedValue;
            int i = System.Convert.ToInt32(s);
            PhysicsExperimentList PhExList1 = new PhysicsExperimentList(); PhExList1.Load_All();
            foreach (PhysicsExperiment p in PhExList1.m_list)
            {
                if ((p.KeyStage == i) || (i == 0))
                {
                    ListItem l = new ListItem(p.ExperimentCode + " - " + p.ExperimentDescription + "(" + p.topic + ")", p.Id.ToString());
                    DropDownList_Expt.Items.Add(l);
                }
            }
            Update_ExptDetails();
        }
        protected void Add_Controls(PhysicsBooking phb1)
        {
            Table tb1 = Table1;
            Table1.Visible = true;
            tb1.EnableViewState = true;
            tb1.Controls.Clear();
            if (phb1.BookingId == Guid.Empty) return;
            TableRow r0 = new TableRow();
            tb1.Controls.Add(r0);
            TableCell c01 = new TableCell(); r0.Controls.Add(c01); SetCellStyle(c01, tb1);
            TableCell c02 = new TableCell(); r0.Controls.Add(c02); SetCellStyle(c02, tb1);
            TableCell c03 = new TableCell(); r0.Controls.Add(c03); SetCellStyle(c03, tb1);

            c01.Text = "Equipment"; c02.Text = "Location"; c03.Text = "Edit";
            PhysicsEquipmentItemList ItemList1 = new PhysicsEquipmentItemList();
            ItemList1.LoadList(phb1);//output as table

            foreach (PhysicsEquipmentItem p in ItemList1.m_list)
            {
                TableRow r1 = new TableRow();
                tb1.Controls.Add(r1);
                r1.BorderStyle = BorderStyle.Solid;
                r1.BorderColor = tb1.BorderColor;
                r1.BorderWidth = tb1.BorderWidth;
                TableCell c1 = new TableCell(); r1.Controls.Add(c1); SetCellStyle(c1, tb1);
                TableCell c2 = new TableCell(); r1.Controls.Add(c2); SetCellStyle(c2, tb1);
                TableCell c3 = new TableCell(); r1.Controls.Add(c3); SetCellStyle(c3, tb1);
                c1.Text = p.EquipmentItemCode; c2.Text = p.EquipmentItemLocation;
                Button b1 = new Button();
                b1.ID = p.Bookings_EquipmentId.ToString();
                c3.Controls.Add(b1);
                b1.Click += new EventHandler(b1_Click);
                b1.Text = "Remove";
            }
        }
        protected void b1_Click(object sender, EventArgs e)
        {
            //this is remove the item.....
            Button b1 = (Button)sender;
            string s = b1.ID;//is Bookings_EquipmentID
            PhysicsEquipmentBooking pb1 = new PhysicsEquipmentBooking();
            pb1.Id = new Guid(s);
            pb1.Delete_by_Id();
            PhysicsBooking b2 = (PhysicsBooking)ViewState["PhysicsBooking"];
            Add_Controls(b2);
        }
        protected void SetCellStyle(TableCell c1, Table tb1)
        {
            c1.BorderStyle = BorderStyle.Solid;
            c1.BorderWidth = tb1.BorderWidth;
            c1.BorderColor = tb1.BorderColor;
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            //add item...

            PhysicsEquipmentBooking b1 = new PhysicsEquipmentBooking();
            PhysicsBooking b2= (PhysicsBooking)ViewState["PhysicsBooking"];
                                   
            if (b2.BookingId == Guid.Empty) b2.Save();//to get id
            b2.Notes = TextBox_Notes.Text;b2.Save();

            ViewState.Remove("PhysicsBooking");
            ViewState.Add("PhysicsBooking", b2);
            b1.BookingId = b2.BookingId ;
            string s = DropDownList_Items.SelectedValue; PhysicsBooking clash = new PhysicsBooking();
            Guid b3 = new Guid(s);
            if (Check_Item_Available(b3, b2, ref clash))
            {
                b1.EquipmentId = b3;
                b1.Save();
                Add_Controls(b2);
            }
            else
            {
                Response.Redirect("BookingFailure.aspx?BookingId="+b2.BookingId.ToString()+"&EquipmentCode="+DropDownList_Items.SelectedItem.Text+"&ClashedBookingId="+clash.BookingId.ToString());
            }


        }
        protected void Button_SaveNotes_Click(object sender, EventArgs e)
        {
            PhysicsBooking b2 = (PhysicsBooking)ViewState["PhysicsBooking"];
            b2.Notes = TextBox_Notes.Text;
            b2.Save();
            ViewState.Remove("PhysicsBooking");
            ViewState.Add("PhysicsBooking", b2);
        }
        protected void Button_AddExperiment_Click(object sender, EventArgs e)
        {
            string not_booked = "";
            PhysicsExperiment PhEx = new PhysicsExperiment();
            PhEx.Id = new Guid(DropDownList_Expt.SelectedValue);
            PhEx.Load();
            PhysicsBooking b2 = (PhysicsBooking)ViewState["PhysicsBooking"];
            if (b2.BookingId == Guid.Empty) b2.Save();//to get id
            b2.Notes ="[Expt: "+PhEx.ExperimentCode+PhEx.ExperimentDescription +"]"+TextBox_Notes.Text; b2.Save(); 
            PhysicsBooking clash = new PhysicsBooking();
            // now to add the Equipment to the booking.....
            foreach (PhysicsEquipmentItem ei in PhEx.EquipmentList.m_list)
            {
                PhysicsEquipmentBooking b1 = new PhysicsEquipmentBooking();
                if (Check_Item_Available(ei.EquipmentItemId, b2, ref clash))
                {
                    b1.BookingId = b2.BookingId;
                    b1.EquipmentId = ei.EquipmentItemId;
                    b1.Save();
                }
                else
                {
                    not_booked += ei.EquipmentItemCode + "  , ";
                }
            }
            ViewState.Remove("PhysicsBooking");
            ViewState.Add("PhysicsBooking", b2);
            if(not_booked.Length>0)
                Response.Redirect("BookingFailure.aspx?BookingId=" + b2.BookingId.ToString() + "&EquipmentCode=" + not_booked + "&ClashedBookingId=" + clash.BookingId.ToString());



            Add_Controls(b2);
        }
        protected void Update_ExptDetails()
        {
            //populate the details box....
            if (DropDownList_Expt.Items.Count > 0)
            {
                PhysicsExperiment PhEx = new PhysicsExperiment();
                PhEx.Id = new Guid(DropDownList_Expt.SelectedValue);
                PhEx.Load(); TextBox_List.Text = "";
                
                foreach (PhysicsEquipmentItem ei in PhEx.EquipmentList.m_list)
                {
                    TextBox_List.Text += ei.EquipmentItemCode + Environment.NewLine;
                }
                servercontent1.InnerHtml = "";
                if (PhEx.notes.Length > 1)
                {
                    servercontent1.InnerHtml = "<A HREF=\"DisplayBookingNotes.aspx?Id="+PhEx.Id.ToString()+" \">Experiemnt has notes...</A>";
                }
            }
        }
        protected void DropDownList_Expt_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update_ExptDetails();
        }
        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateExptList();
        }
        protected void Button_AddLast_Click(object sender, EventArgs e)
        {
            //add items from previous list...
            string not_booked = "";
            PhysicsBooking b_last = (PhysicsBooking)ViewState["LastBooking"];
            if (b_last.BookingId == Guid.Empty) return;
            PhysicsEquipmentBooking b1 = new PhysicsEquipmentBooking();
            PhysicsBooking b2 = (PhysicsBooking)ViewState["PhysicsBooking"];
            if (b2.BookingId == Guid.Empty) b2.Save();//to get id
            b2.Notes = TextBox_Notes.Text; b2.Save();
            ViewState.Remove("PhysicsBooking");
            ViewState.Add("PhysicsBooking", b2);
            PhysicsBooking clash = new PhysicsBooking();
            PhysicsEquipmentItemList list2 = new PhysicsEquipmentItemList();
            list2.LoadList(b_last);
            foreach (PhysicsEquipmentItem i in list2.m_list)
            {
                if (Check_Item_Available(i.EquipmentItemId, b2,ref clash))
                {
                    b1.BookingId = b2.BookingId;
                    b1.EquipmentId = i.EquipmentItemId;
                    b1.Save();
                }
                else
                {
                    not_booked += i.EquipmentItemCode + "  , ";
                }
            }
            if (not_booked.Length > 0)
                Response.Redirect("BookingFailure.aspx?BookingId=" + b2.BookingId.ToString() + "&EquipmentCode=" + not_booked + "&ClashedBookingId=" + clash.BookingId.ToString());

            Add_Controls(b2);
        }
        protected bool Check_Item_Available(Guid EquipmentItemId, PhysicsBooking phb1,ref PhysicsBooking bx)
        {
            PhysicsBookingList list1 = new PhysicsBookingList();
            list1.LoadList_Date(phb1.Date);
            foreach (PhysicsBooking b in list1.m_list)
            {
                PhysicsEquipmentItemList list2 = new PhysicsEquipmentItemList();
                list2.LoadList(b);
                foreach (PhysicsEquipmentItem i in list2.m_list)
                {
                    if (i.EquipmentItemId == EquipmentItemId)
                    {
                        if (b.PeriodId == phb1.PeriodId)
                        {
                            bx = b;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected void Button_delete_Click(object sender, EventArgs e)
        {
            PhysicsBooking b2 = (PhysicsBooking)ViewState["PhysicsBooking"];
            b2.Delete();
            Add_Controls(b2); TextBox_Notes.Text = "";
        }
    }
}
