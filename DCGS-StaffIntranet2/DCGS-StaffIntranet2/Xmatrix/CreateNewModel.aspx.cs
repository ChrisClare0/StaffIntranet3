using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Xmatrix
{
    public partial class CreatNewModel : System.Web.UI.Page
    {
        protected ValueAddedMethod VaM = new ValueAddedMethod();
        protected ValueAddedMethodList ml1 = new ValueAddedMethodList();
        protected VAModel VAmodel = new VAModel();
        protected VABaseDataAggregationList ml2 = new VABaseDataAggregationList();
        protected VABaseDataAggregation VABDA = new VABaseDataAggregation();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (ValueAddedMethod vm in ml1._ValueAddedMethodList)
                {
                    ListItem l = new ListItem(vm.m_ValeAddedDescription, vm.m_ValueAddedMethodID.ToString());
                    DropDownList_VAmethods.Items.Add(l);
                }
                VaM = ml1._ValueAddedMethodList[DropDownList_VAmethods.SelectedIndex];
                DropDownList_VAmethods_SelectedIndexChanged(sender, e);

                foreach (VABaseDataAggregation vm in ml2.m_list)
                {
                    ListItem l = new ListItem(vm.Name, vm.Id.ToString());
                    DropDownList_AggregationMethods.Items.Add(l);
                }
                VABDA = ml2.m_list[DropDownList_AggregationMethods.SelectedIndex];

            }
        }

        protected void DropDownList_VAmethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            VaM = (ValueAddedMethod)ml1._ValueAddedMethodList[DropDownList_VAmethods.SelectedIndex];
            ResultsList rl1 = new ResultsList();
            rl1.LoadListSimple("WHERE ResultType=" + VaM.m_ValueAddedOutputResultType + "  ORDER BY dbo.tbl_Core_Results.ResultDate  DESC ");
            DateTime d1 = new DateTime();d1 = DateTime.Now;
            bool found = false;
            if (rl1._results.Count > 0)
            {
                Result r = new Result(); r = (Result)rl1._results[0];
                d1 = (DateTime)r.Date;found = true;
            }

            int y = d1.Year; ListItem l = new ListItem();
            if (!found) { l = new ListItem("No results found!","0"); DropDownList_Cohort.Items.Clear(); DropDownList_Cohort.Items.Add(l); DropDownList_Cohort.Enabled = false; }
            else {
                switch (VaM.m_ValueAddedOutputResultType)
                {
                    case 9://A2
                        DropDownList_Cohort.Items.Clear();
                        l = new ListItem("A-level Results" + y.ToString(), y.ToString());
                        DropDownList_Cohort.Items.Add(l); y--;
                        l = new ListItem("A-level Results" + y.ToString(), y.ToString());
                        DropDownList_Cohort.Items.Add(l); y--;
                        l = new ListItem("A-level Results" + y.ToString(), y.ToString());
                        DropDownList_Cohort.Items.Add(l); y--;
                        break;
                    case 10://GCSE
                        DropDownList_Cohort.Items.Clear();
                        l = new ListItem("GCSE Results" + y.ToString(), y.ToString());
                        DropDownList_Cohort.Items.Add(l); y--;
                        l = new ListItem("GCSE Results" + y.ToString(), y.ToString());
                        DropDownList_Cohort.Items.Add(l); y--;
                        l = new ListItem("GCSE Results" + y.ToString(), y.ToString());
                        DropDownList_Cohort.Items.Add(l); y--;
                        break;

                    default:
                        break;
                }
                DropDownList_Cohort.Enabled = true;
            }
            DropDownList_Cohort.Enabled = true;
            //find dates....... 

            string s = VaM.m_ValeAddedDescription;
        }

        protected void DropDownList_Cohort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void Button_Create_Click(object sender, EventArgs e)
        {
            //make sure we have a model name and a VA method and a valid cohort
            if (TextBox_ModelName.Text == "") { AlertBox.Visible = true;AlertText.Text = "You must set a Model name";  return; }
            if (DropDownList_Cohort.SelectedValue == "0") { return; }
            //create
            VaM = ml1._ValueAddedMethodList[DropDownList_VAmethods.SelectedIndex];
            VAModel v = new VAModel();
            v.VAMethodId= VaM.m_ValueAddedMethodID;
            v.Name = TextBox_ModelName.Text;
            v.Notes = TextBox_Description.Text;
            v.ResultsYear = System.Convert.ToInt32(DropDownList_Cohort.SelectedValue);
            v.Valid = false;//unless we have data....
            v.Display = false; if (RadioButtonList_Display.SelectedValue == "true") v.Display = true;
            v.Update();
            VAmodel = v;

        }


        protected void OK_Click2(object sender, EventArgs e)
        {
            //now to do the work
            //load students results for this set
            VaM = (ValueAddedMethod)ml1._ValueAddedMethodList[DropDownList_VAmethods.SelectedIndex];
            ResultsList rl1 = new ResultsList();
            rl1.LoadListSimple("WHERE ResultType=" + VaM.m_ValueAddedOutputResultType + "  ORDER BY dbo.tbl_Core_Results.ResultDate  DESC ");

            ResultsList rl0 = new ResultsList();
            rl0.LoadListSimple("WHERE ResultType=" + VaM.m_ValueAddedBaseResultType + "  ORDER BY dbo.tbl_Core_Results.ResultDate  DESC ");
            ResultsList rl2 = new ResultsList();
            CourseList cl1 = new CourseList(5);
            foreach (Result r in rl1._results)
            {
                if (r.Date.Year == VAmodel.ResultsYear)
                {
                    rl2._results.Add(r);
                }
            }

            foreach (Course c in cl1._courses)
            {
                ValueAddedEquation veqn = new ValueAddedEquation(c._CourseID, VaM.m_ValueAddedMethodID);
                foreach (Result r in rl2._results)
                {
                    if (r.CourseID == c._CourseID)
                    {
                        foreach(Result r2 in rl0._results)
                        {
                            if (r.StudentID == r2.StudentID)
                            {
                                //match!!!


                                //Double d = veqn.m_coef0+veqn.m_coef1      
                            }
                        }
                    }
                }

            }



            //ValueAddedEquation vaEqn = new ValueAddedEquation()


            //load VA Equation

            //Proces

        }


        protected void OK_Click(object sender, EventArgs e)
        {
            AlertBox.Visible = false;
        }
    }
}