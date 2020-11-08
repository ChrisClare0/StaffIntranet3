using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public partial class GPBedroom : System.Web.UI.Page
    {
        protected string Id = "9";
        protected string Name = "PopsBedroom";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HuntClass h1 = new HuntClass();
                Panel p = new Panel();
                if (h1.GetClue(Id, ref p))
                {
                    Room.Controls.Add(p);
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Stairs.aspx");
        }
    }
}