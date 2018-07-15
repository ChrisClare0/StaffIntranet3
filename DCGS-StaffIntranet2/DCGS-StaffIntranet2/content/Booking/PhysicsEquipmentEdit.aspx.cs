using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace PhysicsBookings.content.Booking
{
    public partial class PhysicsEquipmentEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetupEquipmentList();
            }
            else
            {
                PhysicsEquipmentItem PhEq = (PhysicsEquipmentItem)ViewState["PhysicsEquipmentItem"];
                //Add_Controls(b2);
            }
        }
        private void SetupEquipmentList()
        {
            PhysicsEquipmentItemList list1 = new PhysicsEquipmentItemList();
            list1.LoadList_All();
            foreach (PhysicsEquipmentItem i in list1.m_list)
            {
                ListItem l = new ListItem(i.EquipmentItemCode, i.EquipmentItemId.ToString());
                DropDownList_Equipment.Items.Add(l);
            }
            PhysicsEquipmentItem i1 = new PhysicsEquipmentItem();
            i1.Load(new Guid(DropDownList_Equipment.SelectedValue));
            ViewState.Add("PhysicsEquipmentItem", i1);
            SetupEquipmentDetail(i1);
        }

        private void SetupEquipmentDetail(PhysicsEquipmentItem item)
        {
            TextBox_Code.Text = item.EquipmentItemCode;
            TextBox_Desc.Text = item.EquipmentItemDescription;
            TextBox_Location.Text = item.EquipmentItemLocation;
            TextBox_Supplier.Text = item.EquipmentItemSupplierCode;
            PhysicsExperimentList ExList = new PhysicsExperimentList();
            ExList.Load_Equipment(item); string s = "";
            foreach (PhysicsExperiment e in ExList.m_list)
            {
                s += e.ExperimentCode + Environment.NewLine;
            }
            TextBox_ExptList.Text = s;
            if (ExList.m_list.Count == 0) Button_Delete.Enabled = true; else Button_Delete.Enabled = false;

        }

        protected void DropDownList_Equipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhysicsEquipmentItem i = new PhysicsEquipmentItem();
            i.Load(new Guid(DropDownList_Equipment.SelectedValue));
            SetupEquipmentDetail(i);
            ViewState.Add("PhysicsEquipmentItem", i);
            PhysicsExperimentList ExList = new PhysicsExperimentList();
            ExList.Load_Equipment(i); string s = "";
            foreach (PhysicsExperiment e1 in ExList.m_list)
            {
                s += e1.ExperimentCode + Environment.NewLine;
            }
            TextBox_ExptList.Text = s;
        }

        protected void Button_Update_Click(object sender, EventArgs e)
        {
            PhysicsEquipmentItem i = new PhysicsEquipmentItem();
            i.Load(new Guid(DropDownList_Equipment.SelectedValue));
            i.EquipmentItemCode = TextBox_Code.Text;
            i.EquipmentItemDescription = TextBox_Desc.Text;
            i.EquipmentItemLocation = TextBox_Location.Text;
            i.EquipmentItemSupplierCode = TextBox_Supplier.Text;
            i.Save();
        }

        protected void Button_New_Click(object sender, EventArgs e)
        {
            PhysicsEquipmentItem i = new PhysicsEquipmentItem();
            i.EquipmentItemCode = TextBox_New.Text;
            i.Save();
        }

        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            PhysicsEquipmentItem i = new PhysicsEquipmentItem();
            i.Load(new Guid(DropDownList_Equipment.SelectedValue));
            i.Delete();
        }


 
    }
}
