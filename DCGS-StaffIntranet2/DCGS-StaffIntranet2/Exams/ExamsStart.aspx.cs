using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class ExamsStart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                //testing
                text1.InnerHtml = Server.MapPath("BaseData");
                //set up the season and year
                TextBox_Year.Text = new Cerval_Configuration("StaffIntranet_ExamsStartYear").Value;
                string s = new Cerval_Configuration("StaffIntranet_ExamsStartSeason").Value;
                DropDownList_Season.SelectedIndex = 1;
                try { DropDownList_Season.SelectedValue = s; } catch { };
                Session["Year"] = TextBox_Year.Text;
                Session["Season"] = DropDownList_Season.SelectedValue;
#if DEBUG
                Button1.Visible = true;
#endif

            }
            else
            {
                // is post back so we have changed something..
                //check that the Year is a valid year
                int y = 0;bool valid = false;
                try
                {
                    y = System.Convert.ToInt32(TextBox_Year.Text);
                    if ((y > 2000) && (y < 2050)) valid = true;
                }
                catch
                {
                }

                if (valid)
                {
                    Label_Error.Visible = false;
                    Session["Year"] = TextBox_Year.Text;
                    Session["Season"] = DropDownList_Season.SelectedValue;
                }
                else
                {
                    // input not a valid year...  reset display....
                    Label Label_Year = (Label)Master.FindControl("Label_Year");
                    if (Label_Year != null)
                    {
                        TextBox_Year.Text = Label_Year.Text;
                    }
                    Label_Error.Visible = true;

                }
            }
        }

        protected void Button_Save_Click(object sender, EventArgs e)
        {
            Cerval_Configuration c1 = new Cerval_Configuration("StaffIntranet_ExamsStartYear");
            Cerval_Configuration c2 = new Cerval_Configuration("StaffIntranet_ExamsStartSeason");
            c1.Value = TextBox_Year.Text;
            c2.Value = DropDownList_Season.SelectedValue;
            c1.Save();
            c2.Save();

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DateTime t0 = new DateTime();t0 = new DateTime(2017, 3, 2);
            Server.Transfer(@"../exams/TimetableDetailEdit.aspx?Date=" + t0.ToShortDateString() + "&Session=AM");
        }
    }
}