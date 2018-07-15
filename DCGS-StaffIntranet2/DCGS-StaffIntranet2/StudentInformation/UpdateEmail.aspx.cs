using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Cerval_Library;

namespace StudentInformation
{
    public partial class UpdateEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Utility u1 = new Utility();
                Guid PersonID = u1.GetPersonIdfromRequest(Request);
#if DEBUG
                u1.Is_student = true; PersonID = u1.GetPersonIDX(@"CHALLONERS\ryhs.aitchison");//development             
#endif
                if ((PersonID != Guid.Empty) && (u1.Is_student))
                {
                    string s = u1.GetEmailAddress(PersonID);
                    TextBox_old.Text = s;
                    Label4.Text = PersonID.ToString();
                }
            }
        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            Utility u1 = new Utility();
            RegexStringValidator rg1 = new RegexStringValidator(u1.GetRegex_email());
            try
            {
                rg1.Validate(TextBox_new.Text);
                u1.UpdateStudentEmail(TextBox_new.Text, Label4.Text);
                Server.Transfer("StartForm.aspx");
            }
            catch (ArgumentException e1)
            {
                string s1 = e1.Message;
                Label3.Text = "Invalid address - please re-type";
            }     
        }
    }
}
