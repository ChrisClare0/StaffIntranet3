using System;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public partial class Stairs : System.Web.UI.Page
    {
        protected string Id = "4";
        protected string Name = "Upstairs";
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
            Response.Redirect("Hall.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("GPBedroom.aspx");
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            Response.Redirect("SpareBedroom.aspx");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Response.Redirect("SmallBedroom.aspx");
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Response.Redirect("KJBedroom.aspx");
        }

        protected void Button2_Click1(object sender, EventArgs e)
        {
            Response.Redirect("Hall.aspx");
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
            Response.Redirect("Bathroom.aspx");
        }
    }
}