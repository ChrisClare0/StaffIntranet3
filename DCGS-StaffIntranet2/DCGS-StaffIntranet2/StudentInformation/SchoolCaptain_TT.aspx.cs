using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.StudentInformation
{
    public partial class SchoolCaptain_TT : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Build_NameList("12");
                string studentID = NameList.SelectedItem.Value;
                TimetableControl1.BaseId = new Guid(studentID);
                TimetableControl1.Visible = true;
                TimetableControl1.type = TT_writer.TimetableType.Student;
            }
            }
        private void Build_NameList(string year)
        {

                DateTime d = new DateTime(); d = DateTime.Now;
                Label_Year.Text = "Year " + year;
                StudentYearList yl1 = new StudentYearList(NameList, year + "Year", d);
            if (NameList.Items.Count == 0)
            {
                Label_Year.Text = "Year " + year + " (Next Year)";
                yl1.StudentYearList_Load(NameList, year + "Year", d);
            }
            if (NameList.Items.Count > 0) NameList.Items[0].Selected = true;
        }

        protected void NameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string studentID = NameList.SelectedItem.Value;
            TimetableControl1.BaseId = new Guid(studentID);
        }

        protected void YearList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = YearList.SelectedItem.Value;
            Build_NameList(s);
            string studentID = NameList.SelectedItem.Value;
            TimetableControl1.BaseId = new Guid(studentID);
        }
    }
}