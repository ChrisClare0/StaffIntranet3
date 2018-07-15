using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Options
{
    public partial class Options1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button_Process_Click(object sender, EventArgs e)
        {

        }

        protected void Button_Display_Click(object sender, EventArgs e)
        {
            if (!FileUpload1.HasFile) return;

            /*
            TextFileType ftype1 = TextFileType.Unknown;

            string s = Server.MapPath("BaseData") + "\\" + FileUpload1.FileName;

            Cerval_Library.TextReader TxtRd1 = new Cerval_Library.TextReader();
            TextRecord t = new TextRecord(); int l = 0; int n = 0;
            string s1 = ""; char ct = (char)0x09;

            while (TxtRd1.ReadTextLine(FileUpload1.FileContent, ref t) == Cerval_Library.TextReader.READ_LINE_STATUS.VALID)
            {
                l = s1.Length;
                for (int i = 0; i < t.number_fields; i++)
                {
                    if (t.field[i].Length == 0) break;
                    else s1 += t.field[i] + ct;
                }
                if (s1.Length > l) s1 += Environment.NewLine;
                if (n == 0)
                {
                    //this is first line so has headers....
                    if (t.field[0].ToUpper().Trim() == "SET") ftype1 = TextFileType.Sets;
                    if (t.field[0].ToUpper().Trim() == "ADNO") ftype1 = TextFileType.Students;
                    if (t.field[0].ToUpper().Trim() == "ADMISSION NUMBER") ftype1 = TextFileType.Students;
                    switch (ftype1)
                    {
                        case TextFileType.Unknown:
                            Label_Text.Text = "First column must be either 'Set' or 'adno' or 'Admission Number'";
                            break;
                        case TextFileType.Students:
                            Label_Text.Text = "Assuming columns are: Admission Number,Surname, GivenName,Board,Syllabus,Option...";
                            break;
                        case TextFileType.Sets:
                            Label_Text.Text = "Assuming columns are: Set, Board, Syllabus, Option.......";
                            break;
                        default:
                            break;
                    }
                }
            }
            ViewState["TextFileType"] = ftype1;
            TextBox1.Text = s1;
            Button_CheckTime.Visible = false;
            Button_Upload.Visible = false;
            Button_Process.Visible = true;
            */
        }
    }
}