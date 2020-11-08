using System;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public partial class Lounge : System.Web.UI.Page
    {
        protected string Id = "2";
        protected string Name = "Lounge";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HuntClass h1 = new HuntClass();
                Panel p = new Panel();
                if (h1.GetClue(Id, ref p))
                {
                    EasterHuntDRoom.Controls.Add(p);
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Hall.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("DiningRoom.aspx");
        }
    }
}