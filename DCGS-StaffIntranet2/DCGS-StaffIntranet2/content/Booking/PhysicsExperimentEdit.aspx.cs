using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace PhysicsBookings.content.Booking
{
    public partial class PhysicsExperimentEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetupExptList();
                SetupEquipmentList();       
            }
            else
            {
                PhysicsExperiment b2 = (PhysicsExperiment)ViewState["PhysicsExperiment"];
                Add_Controls(b2);
            }

        }
        void SetupEquipmentList()
        {
            PhysicsEquipmentItemList list1 = new PhysicsEquipmentItemList();
            list1.LoadList_All();
            foreach (PhysicsEquipmentItem p in list1.m_list)
            {
                ListItem l1 = new ListItem(p.EquipmentItemCode, p.EquipmentItemId.ToString());
                DropDownList_Equipment.Items.Add(l1);
            }
        }
        void SetupExptList()
        {
            DropDownList_Expt.Items.Clear();
            PhysicsExperimentList PhExList1 = new PhysicsExperimentList(); PhExList1.Load_All();
            foreach (PhysicsExperiment p in PhExList1.m_list)
            {
                ListItem l = new ListItem(p.ExperimentCode + " - " + p.ExperimentDescription + "(" + p.topic + ")", p.Id.ToString());
                DropDownList_Expt.Items.Add(l);
            }     
            Update_ExptDetails();
        }
        void Add_Controls(PhysicsExperiment phex1)
        {
            Table tb1 = Table1;
            Table1.Visible = true;
            tb1.EnableViewState = true;
            tb1.Controls.Clear();
            if (phex1.Id == Guid.Empty) return;
            TableRow r0 = new TableRow();
            tb1.Controls.Add(r0);
            TableCell c01 = new TableCell(); r0.Controls.Add(c01); SetCellStyle(c01, tb1);
            TableCell c02 = new TableCell(); r0.Controls.Add(c02); SetCellStyle(c02, tb1);
            TableCell c03 = new TableCell(); r0.Controls.Add(c03); SetCellStyle(c03, tb1);

            c01.Text = "Equipment Code"; c02.Text = "Location"; c03.Text = "Delete";

            foreach (PhysicsEquipmentItem p in phex1.EquipmentList.m_list)
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
                b1.ID = p.Experiment_EquipmentId.ToString();
                b1.Text = "Remove";
                b1.Click+=new EventHandler(b1_Click);
                c3.Controls.Add(b1);
                
            }
        }
        void b1_Click(object sender, EventArgs e)
        {
            PhysicsExperiment PhEx = (PhysicsExperiment)ViewState["PhysicsExperiment"];
            Button b1 = (Button)sender;
            PhEx.RemoveEquipmentItem(new Guid(b1.ID));
            Update_ExptDetails();
        }
        private void Update_ExptDetails()
        {
            //populate the equipment list table...
            if (DropDownList_Expt.Items.Count > 0)
            {
                PhysicsExperiment PhEx = new PhysicsExperiment();
                PhEx.Id = new Guid(DropDownList_Expt.SelectedValue);
                PhEx.Load();
                ViewState.Add("PhysicsExperiment",PhEx);
                Add_Controls(PhEx);
                servercontent.InnerHtml = "";
                if (PhEx.notes.Length > 1)
                {
                    servercontent.InnerHtml = "<A HREF=\"DisplayBookingNotes.aspx?Id=" + PhEx.Id.ToString() + " \">Experiemnt has notes...</A>";
                }
                TextBox_Code.Text = PhEx.ExperimentCode;
                TextBox_Desc.Text = PhEx.ExperimentDescription;
                TextBox_Notes.Text = PhEx.notes;
                TextBox_SpecRef.Text = PhEx.SpecificationReference;
                TextBox_Topic.Text = PhEx.topic;
                TextBox_KeyStage.Text = PhEx.KeyStage.ToString();
                

            }
            
        }
        protected void SetCellStyle(TableCell c1, Table tb1)
        {
            c1.BorderStyle = BorderStyle.Inset;
            c1.BorderWidth = tb1.BorderWidth;
            c1.BorderColor = tb1.BorderColor;
        }
        protected void Button_Add_Click(object sender, EventArgs e)
        {
            PhysicsExperiment PhEx = (PhysicsExperiment)ViewState["PhysicsExperiment"];
            //if (b2.BookingId == Guid.Empty) b2.Save();//to get id
            //b2.Notes = TextBox_Notes.Text;b2.Save();
            ViewState.Add("PhysicsExperiment", PhEx);
            string s = DropDownList_Equipment.SelectedValue;
            Guid ItemId = new Guid(s);
            PhEx.AddEquipmentItem(ItemId);
            Add_Controls(PhEx);
        }
        protected void DropDownList_Expt_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update_ExptDetails();
        }
        protected void Button_CreateNew(object sender, EventArgs e)
        {
            PhysicsExperiment PhEx = new PhysicsExperiment();
            PhEx.ExperimentCode = TextBox_New.Text;
            PhEx.Save();
            SetupExptList();
        }

        protected void Button_Update_Click(object sender, EventArgs e)
        {
            PhysicsExperiment PhEx = new PhysicsExperiment();
            PhEx.Id = new Guid(DropDownList_Expt.SelectedValue);
            PhEx.Load();
            PhEx.ExperimentCode = TextBox_Code.Text;
            PhEx.ExperimentDescription = TextBox_Desc.Text;
            PhEx.notes = TextBox_Notes.Text;
            PhEx.SpecificationReference = TextBox_SpecRef.Text;
            PhEx.topic = TextBox_Topic.Text;
            PhEx.KeyStage = System.Convert.ToInt32(TextBox_KeyStage.Text);
            PhEx.Save();
        }
        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            PhysicsExperiment PhEx = new PhysicsExperiment();
            PhEx.Id = new Guid(DropDownList_Expt.SelectedValue);
            PhEx.Delete();
            SetupExptList();
        }

    }
}
