using System;
using System.Web.UI.WebControls;


namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public partial class Kitchen : System.Web.UI.Page

    {
        protected string Id = "1";
        protected string Name = "Kitchen";
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
            Response.Redirect("DiningRoom.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Hall.aspx");
        }
    }
}