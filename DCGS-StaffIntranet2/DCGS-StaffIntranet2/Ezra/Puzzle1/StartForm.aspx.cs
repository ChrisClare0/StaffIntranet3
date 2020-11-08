using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Ezra.Puzzle1
{
    public partial class StartForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //set up table           
                for (int i = 0; i < 6; i++)
                {
                    TableRow r = new TableRow();
                    Table1.Rows.Add(r);
                    for (int j = 0; j < 6; j++)
                    {
                        TableCell c = new TableCell();
                        r.Cells.Add(c);
                        c.Text = i.ToString() + j.ToString();
                    }
                }
            }
        }
    }
}