using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.IO;

namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public partial class EasterHunt : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string[] sList = { "", "", "", "", "", "", "", "", "" };
                GetCurrentClue(ref sList);
                LabelCurrent.Text = "        "+sList[5];

            }
        }

        protected string GetCurrentClue(ref string[] result)
        {
            Encode en = new Encode();
            FileStream fs = new FileStream(en.GetFilePath("Ez_Hunt.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs);
            string s = sr.ReadLine();//header
            string[] spearator = { "," };
            s = sr.ReadLine();//current hunt Id and current clue Id
            result = s.Split(spearator, 20, StringSplitOptions.RemoveEmptyEntries);
            string HuntId = result[0]; string currentClueId = result[1];
            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                result = s.Split(spearator, 20, StringSplitOptions.RemoveEmptyEntries);
                if ((result[0] == HuntId) && (result[1] == currentClueId)) 
                {
                    sr.Close();
                    fs.Close();
                    return currentClueId;

                }
            }
            sr.Close();
            fs.Close();
            return currentClueId;
        }


        protected void AdvanceCurrentClue()
        {
            Encode en = new Encode();
            FileStream fs1 = new FileStream(en.GetFilePath("Ez_Hunt_temp.txt"), FileMode.Open, FileAccess.ReadWrite);
            FileStream fs = new FileStream(en.GetFilePath("Ez_Hunt.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamWriter srw = new StreamWriter(fs1);
            StreamReader sr = new StreamReader(fs);
            srw.WriteLine(sr.ReadLine());//header
            string[] spearator = { "," };
            string s = sr.ReadLine();//current hunt Id and current clue Id
            string[] result = s.Split(spearator, 20, StringSplitOptions.RemoveEmptyEntries);
            int n = System.Convert.ToInt32(result[1]); n++;
            s = result[0] + "," + n.ToString() + ",0,0,0,,0";
            srw.WriteLine(s);
            while (!sr.EndOfStream) { srw.WriteLine(sr.ReadLine()); }
            //HuntId,ClueId,RoomId,Position-x,Position-y,Clue,SuccessCode
            sr.Close();
            srw.Close();
            fs1.Close();
            fs.Close();

            //now copy temp to real
            fs1 = new FileStream(en.GetFilePath("Ez_Hunt.txt"), FileMode.Open, FileAccess.ReadWrite);
            fs = new FileStream(en.GetFilePath("Ez_Hunt_temp.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
            srw = new StreamWriter(fs1);
            sr = new StreamReader(fs);
            while (!sr.EndOfStream){srw.WriteLine(sr.ReadLine());}
            sr.Close();
            srw.Close();
            fs1.Close();
            fs.Close();
            return;
        }



        protected void Button1_Click(object sender, EventArgs e)
        {
            //submit a code
            string s = TextBox1.Text;
            string[] sList = { "", "", "", "", "", "", "", "", "" };
            string current = GetCurrentClue(ref sList);

            if ((sList[6] == "0") && (s == "0")) { Response.Redirect("finish.aspx"); }
            if (sList[6] == s)
            {
                //success!
                //need to update txt file..
                AdvanceCurrentClue();
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
        }
    }
}