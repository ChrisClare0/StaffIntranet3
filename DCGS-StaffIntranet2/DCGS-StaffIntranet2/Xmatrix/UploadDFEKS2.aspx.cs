using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Xmatrix
{
    public partial class UploadDFEKS2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public HtmlDocument doc = null;
        public  void getdom()
        {
            if (this.Site != null)
            {
                doc = (HtmlDocument)this.Site.GetService(typeof(HtmlDocument));
                doc.GetElementById("TextInputx").InnerText = "fcytcy";
            }
        }


        [WebMethod]
        public static string Process(string s)
        {
            string Name = "Hello Rohatash Kumar";
            dFEProgressList l1 = new dFEProgressList();
            l1.Load();

            return Name;
        }

        [WebMethod]
        public static string check(string[][] d)
        {

            string s = d[0][0];

            return s;
        }
    }
}