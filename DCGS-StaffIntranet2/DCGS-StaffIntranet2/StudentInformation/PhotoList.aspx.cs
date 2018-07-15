using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.Drawing;
using Cerval_Library;

namespace StudentInformation
{
    public partial class PhotoList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string dir = Request.QueryString["Dir"];
                string path = "";
                Guid PersonID = new Guid(); Utility u1 = new Utility();
                string s2 = "";
                PersonID = u1.GetPersonIDX(Context.User.Identity.Name.ToString(), out s2);
                //PersonID = u1.GetPersonIDX(@"CHALLONERS\Richard.Baldwin");//development
#if DEBUG

                PersonID = u1.GetPersonIDX(@"CHALLONERS\Nicholas.Brabbs", out s2);//development
                PersonID = u1.GetPersonIDX(@"CHALLONERS\Sean.Tadman", out s2);//development
                //PersonID = u1.GetPersonIDX(@"CHALLONERS\Richard.Baldwin");//development

#endif
                path = Server.MapPath(@"images\Morocco_2013\") + dir;
                servercontent.InnerHtml = "<h3>Welcome " + u1.GetPersonName(PersonID) + "</h3>";
                Table table1 = new Table();
                servercontent.Controls.Add(table1);
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] fi = di.GetFiles();
                //log visit...??

                //StreamWriter sw1 = new StreamWriter(Server.MapPath( "log.txt"), true);
                
                //sw1.WriteLine(u1.GetPersonName(PersonID) + DateTime.Now.ToLongDateString()+" " +DateTime.Now.ToShortTimeString());
                //sw1.Close();
                foreach (FileInfo fiTemp in fi)
                {
                    if (!fiTemp.Name.StartsWith("_"))
                    {
                        TableRow tr1 = new TableRow(); table1.Rows.Add(tr1);
                        TableCell cell1 = new TableCell(); tr1.Cells.Add(cell1);
                        cell1.Text = "<a href = \"" + "images/Morocco_2013/" + dir + "/" + fiTemp.Name + "\">" + fiTemp.Name + "</a>";
                        /*
                        if(fiTemp.Name.Contains("wmv"))
                        {
                            cell1.Text = "<a href = \"" + "CC_IMAGES\\RidgeWay\\" + dir + "\\" + fiTemp.Name + "\">" + fiTemp.Name + "</a>";
                            //cell1.Text = "<a href = \"" + "D:\\beta-challoners\\StudentInformation\\images\\RidgeWay\\" + dir + "\\" + fiTemp.Name + "\">" + fiTemp.Name + "</a>";
                        }
                         */
                        TableCell cell2 = new TableCell(); tr1.Cells.Add(cell2);

                        //System.Drawing.Image.GetThumbnailImageAbort myCallback =new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
                        //Bitmap myBitmap = new Bitmap(fiTemp.FullName);
                        //System.Drawing.Image myThumbnail = myBitmap.GetThumbnailImage(140, 100, null, IntPtr.Zero);
                        //File.Move(s1, s2);


                        System.Web.UI.WebControls.Image im1 = new System.Web.UI.WebControls.Image();
                        
                        //myThumbnail.Save(path + "\\_X_" + fiTemp.Name);
                        
                        cell2.Controls.Add(im1);
                        //im1.ImageUrl = "../images/RidgeWay/cc_photos/_X_IMG_2603.jpg";
                        im1.ImageUrl = "images/Morocco_2013/" + dir + "/_X_" + fiTemp.Name;
                    }
                }

            }
        }
    }
}
