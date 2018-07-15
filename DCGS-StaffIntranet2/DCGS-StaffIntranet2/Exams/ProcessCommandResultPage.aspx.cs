using System;
using System.Collections.Generic;
using System.Linq;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class ProcessCommandResultPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string requestId = Request.QueryString["RequestId"];

            if (!string.IsNullOrEmpty(requestId))
            {
                // if we found requestid means thread is processed
                if (ThreadResult.Contains(requestId))
                {
                    // get value
                    string result = (string)ThreadResult.Get(requestId);

                    // show message
                    lblResult.Text = result;

                    lblProcessing.Visible = false;
                    imgProgress.Visible = false;
                    lblResult.Visible = true;
                    btnClose.Visible = true;
                    // Remove value from HashTable
                    ThreadResult.Remove(requestId);

                }
                else
                {
                    lblProcessing.Text = "Update....." + ThreadResult.Progress.ToString()+"   :   "+ThreadResult.UpdateString;
                    // keep refreshing progress window
                    Response.AddHeader("refresh", "2");
                }
            }
        }
    }
}