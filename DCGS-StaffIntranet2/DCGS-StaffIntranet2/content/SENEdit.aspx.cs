using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class SENEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Utility u = new Utility();
                //string staff_code = u.GetStaffCodefromContext(Context);
                string staff_code = u.GetsStaffCodefromRequest(Request);
                Cerval_Configuration c2 = new Cerval_Configuration("StaffIntranet_SEN-MANAGERS");
                string s_sen = ""; bool display = false;
                s_sen = c2.Value;
                if (!c2.valid)//try revert to config file
                {
                    System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
                    s_sen = ar.GetValue("SEN-MANAGERS", typeof(string)).ToString();
                }
                staff_code = staff_code.Trim();
                char[] c1 = new char[1]; c1[0] = ','; s_sen = s_sen.ToUpper();
                string[] s_array = new string[10]; s_array = s_sen.Split(c1);


                if (s_array.Contains(staff_code.ToUpper()))
                {
                    SENEdit1.SenId = new Guid(Request.QueryString["SENID"]);
                    SENEdit1.StudentId = new Guid(Request.QueryString["StudentID"]);
                    SENEdit1.UpdateControls();
                    SENEdit1.Visible = true;
                }
                else
                {
                    Response.Write("<br/><h3>You do not have permission to edit SEN data.</h3><br/>" + s_sen + "<br/>" + staff_code + "<br/>" + s_array[4].ToString());
                }
            }
        }
    }
}