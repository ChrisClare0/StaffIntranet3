using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class BaseDataUpload : System.Web.UI.Page
    {
        #region variables

        public int Year
        {   //this is the year in 2016 format
            get { try { return (int)ViewState["Year"]; } catch { return 0; } }
            set { ViewState["Year"] = value; }
        }
        public int YearCode
        {   //this is the year in 16 format
            get { try { return (int)ViewState["YearCode"]; } catch { return 0; } }
            set { ViewState["YearCode"] = value; }
        }
        public int SeasonCode
        {
            get { try { return (int)ViewState["SeasonCode"]; } catch { return 0; } }
            set { ViewState["SeasonCode"] = value; }
        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetMasterValues();
            }
        }

        private void GetMasterValues()
        {
            SeasonCode = System.Convert.ToInt32((string)(Session["Season"]));
            Year = System.Convert.ToInt32((string)(Session["Year"]));
            YearCode = Year % 100;
            Label Label_Banner = (Label)Master.FindControl("Label_Banner");
            if (Label_Banner != null)
            {
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Transferring BaseData to Server.";
            }
        }

        protected void Button_Upload_Click(object sender, EventArgs e)
        {
            bool success = false;string s1 = "";int n = 0;
            try
            {
                HttpFileCollection hfc = Request.Files;
                for (int i = 0; i < hfc.Count; i++)
                {
                    HttpPostedFile hpf = hfc[i];
                    if (hpf.ContentLength > 0)
                    {
                        string s = Server.MapPath("BaseData")+"\\"+System.IO.Path.GetFileName(hpf.FileName);
                        hpf.SaveAs(s);
                        n++;
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                s1 = ex.Message;
            }
            if (success)
            {
                Label_end.Text = "Successfully uploadad " + n.ToString() + " files.";
            }
            else
            {
                Label_end.Text = "Error uploading files:" + s1;
            }
        }
    }
}