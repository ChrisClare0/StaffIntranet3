using System;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public partial class Hall : System.Web.UI.Page
    {
        protected string Id = "3";
        protected string Name = "Hall";
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

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Lounge.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Kitchen.aspx");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Response.Redirect("Stairs.aspx");
        }

        protected void Button_out_Click(object sender, EventArgs e)
        {
            Response.Redirect("NOTYET.aspx");
        }

        protected void Button_study_Click(object sender, EventArgs e)
        {
            Response.Redirect("NOTYET.aspx");
        }
  
    }
}