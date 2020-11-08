using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Xmatrix
{
    public partial class XmatrixStartPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                VAModelList l1 = new VAModelList();
                l1.Load();
                foreach (VAModel m in l1.m_list)
                {
                    DropDownList_Methods.Items.Add(new ListItem(m.Name,m.Id.ToString()));

                }

            }

        }
    }
}