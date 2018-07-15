using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet.DataUtilities
{
    public partial class SanctionsNotCompleted : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Encode en = new Encode();
            SqlDataSource1.ConnectionString = en.GetCervalConnectionString();
            SqlDataSource1.SelectCommand = "SELECT [StudentAdmissionNumber], [PersonGivenName], [PersonSurname], [SanctionName], [SanctionDate], [IncidentDate], [SanctionId], [IncidentText], [WorkSet], [GroupCode], [StaffCode], [Count] FROM [cc_development_SanctionsNotCompleted]";
            GridView1.RowCommand += new GridViewCommandEventHandler(GridView1_RowCommand);
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        void GridView1_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            //calls here for any command- including a sort
            if (e.CommandName == "Complete")
            {
                int row = Convert.ToInt32(e.CommandArgument);
                GridViewRow row1 = GridView1.Rows[row];
                string s = Server.HtmlDecode(row1.Cells[12].Text);
                //this is the sanctionid
                StudentSanction san1 = new StudentSanction();
                san1.Load(s);
                san1.MarkAsComplete();
                GridView1.DataBind();
            }
            if (e.CommandName == "ReSchedule")
            {
                int row = Convert.ToInt32(e.CommandArgument);
                GridViewRow row1 = GridView1.Rows[row];
                string s = Server.HtmlDecode(row1.Cells[12].Text);
                //this is the sanctionid
                StudentSanction san1 = new StudentSanction();
                san1.Load(s);
                san1.SanctionDate = san1.SanctionDate.AddDays(7);
                san1.Save();
                GridView1.DataBind();

            }

        }

        protected void SqlDataSource1_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {

        }
    }
}
