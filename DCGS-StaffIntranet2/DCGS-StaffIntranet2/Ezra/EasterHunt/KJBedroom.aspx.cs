using System;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public partial class KJBedroom : System.Web.UI.Page
    {
        protected string Id = "5";
        protected string Name = "KJBedroom";
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

        protected void Button5_Click(object sender, EventArgs e)
        {
            Response.Redirect("Stairs.aspx");
        }
    }
}