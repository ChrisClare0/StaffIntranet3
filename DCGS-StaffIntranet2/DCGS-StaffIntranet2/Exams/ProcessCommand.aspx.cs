using System;
using System.Threading;
using System.Web.UI;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class ProcessCommand : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected Guid requestId;

        protected void btnLongRuningTask_Click(object sender, EventArgs e)
        {
            requestId = Guid.NewGuid();

            // Start the long running task on one thread
            ParameterizedThreadStart parameterizedThreadStart = new ParameterizedThreadStart(LongRuningTask);

            Thread thread = new Thread(parameterizedThreadStart);

            thread.Start();

            // Show Modal Progress Window
            //Page.ClientScript.RegisterStartupScript(this.GetType(),"OpenWindow", "OpenProgressWindow('" + requestId.ToString() + "');", true);
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('../Exams/ProcessCommandResultPage.aspx?RequestId=" + requestId.ToString() + "');", true);
        }

        private void LongRuningTask(object data)
        {
            //  simulate long running task – your main logic should   go here
            ThreadResult.Progress = 0;ThreadResult.UpdateString = "Starting...";
            for(int i = 0;i<100;i++)
            {
                ThreadResult.Progress++;
                double d1 = ThreadResult.Progress;
                double d2 = 100.0;
                ThreadResult.UpdateString = "Progress at " + (100 * d1 / d2).ToString() + "%";
                Thread.Sleep(4000);

            }


            // Add ThreadResult -- when this
            // line executes it  means task has been
            // completed
            ThreadResult.Add(requestId.ToString(), "Item Processed Successfully."); // you  can add your result in second parameter.
        }
    }
}