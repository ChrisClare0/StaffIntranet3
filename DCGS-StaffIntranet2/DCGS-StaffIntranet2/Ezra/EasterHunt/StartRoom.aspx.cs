using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.IO;

namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public partial class StartRoom : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
      
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("DiningRoom.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            HuntClass h1 = new HuntClass();
            h1.ResetClue("1");
        }
    }
}