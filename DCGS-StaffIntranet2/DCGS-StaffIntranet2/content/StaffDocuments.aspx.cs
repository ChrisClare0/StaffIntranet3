using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class StaffDocuments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //get the parameter....  the current directory level ..may be null
   
                string start_directory = Request.QueryString["Path"];
                string s = Table1.TemplateSourceDirectory;
                Cerval_Configuration c1 = new Cerval_Configuration("StaffIntranet_StaffDocuments");
                string s1 = c1.Value;
                if (!c1.valid)//try revert to config file
                {
                    System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
                    s1 = ar.GetValue("StaffDocuments", typeof(string)).ToString();
                }
                //s1 = @"c:\users\cc\documents";
                if (start_directory != null) s1 = s1 + "\\" + start_directory;
                s = s1;
                DirectoryInfo di = new DirectoryInfo(s);
                DirectoryInfo[] dilist = di.GetDirectories();
                foreach (DirectoryInfo di1 in dilist)
                {
                    if (!di1.Name.StartsWith("_"))
                    {
                        TableRow r = new TableRow();
                        s = di1.Name;
                        if (start_directory != null) s = start_directory + "\\" + s;
                        TableCell c = new TableCell();
                        c.Controls.Add(new LiteralControl("FOLDER:- <A href=\"" + Table1.TemplateSourceDirectory + "/StaffDocuments.aspx?Path=" + s + "\">" + s + "</A>"));
                        r.Cells.Add(c);
                        Table1.Rows.Add(r);
                    }
                }
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo fiTemp in fi)
                {
                    AddFileToTable(fiTemp,false,start_directory);
                }

                Table1.Font.Name = "Arial";
                Table1.CellPadding = 5;
                Table1.CellSpacing = 0;
                Table1.ToolTip = "List of directories and files to downlaod";
            }
        }

        private void AddFileToTable(FileInfo fi, bool addfoldercol, string start_directory)
        {
            TableRow r = new TableRow();
            TableCell c = new TableCell();
            string s = "test-docs\\";
            if (start_directory != null)
            {
                s += start_directory + "\\";
            }

            s +=fi.Name;
            c.Controls.Add(new LiteralControl("<A href=\"" + s + "\">" + fi.Name + "</A>"));
            r.Cells.Add(c);
            Table1.Rows.Add(r);
            if (addfoldercol)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(start_directory));
                r.Cells.Add(c);
            }
        }

        protected void TextBox_mask_TextChanged(object sender, EventArgs e)
        {
            //do search...
            //string s1 = @"c:\users\cc\documents";


            Cerval_Configuration c1 = new Cerval_Configuration("StaffIntranet_StaffDocuments");
            string s1 = c1.Value;
            if (!c1.valid)//try revert to config file
            {
                System.Configuration.AppSettingsReader ar = new System.Configuration.AppSettingsReader();
                s1 = ar.GetValue("StaffDocuments", typeof(string)).ToString();
            }


            Table1.Rows.Clear();
            TableRow r = new TableRow();
            TableCell c = new TableCell();
            c.Controls.Add(new LiteralControl("FileName"));
            r.Cells.Add(c);
            Table1.Rows.Add(r);
            c = new TableCell();
            c.Controls.Add(new LiteralControl("In Folder"));
            r.Cells.Add(c);
            SearchDirectory(s1, TextBox_mask.Text+"*");
            

        }

        private void SearchDirectory(string dir, string searchpattern)
        {

            DirectoryInfo di = new DirectoryInfo(dir);
            try
            {
                FileInfo[] fi = di.GetFiles(searchpattern);
                foreach (FileInfo fiTemp in fi)
                {
                    AddFileToTable(fiTemp,true,dir);
                }
                DirectoryInfo[] dilist = di.GetDirectories();
                foreach (DirectoryInfo di1 in dilist)
                {
                    SearchDirectory(di1.FullName, searchpattern);
                }
            }
            catch 
            {
                //Response.Write(e1.Message + "<br>");
            }

        }

    }
}
