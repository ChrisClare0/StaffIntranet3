using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class ListAPI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ListJsonHandler x = new ListJsonHandler();
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            x.ProcessRequest(Context);
            Response.End();
        }
    }
}