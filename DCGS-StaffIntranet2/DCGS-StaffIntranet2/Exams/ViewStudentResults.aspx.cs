using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class ViewStudentResults : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SimpleStudentList sl1 = new SimpleStudentList();
            sl1.LoadFull();
            foreach (SimplePupil p in sl1._studentlist)
            {
                ListItem l = new ListItem(p.m_GivenName + p.m_Surname, p.m_adno.ToString());
                DropDownList1.Items.Add(l);

            }
        }
    }
}