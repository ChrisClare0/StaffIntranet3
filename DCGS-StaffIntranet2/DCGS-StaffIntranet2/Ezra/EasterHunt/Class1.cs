using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI.WebControls;

namespace DCGS_Staff_Intranet2.Ezra.EasterHunt
{
    public class HuntClass
    {
        public bool GetClue(string RoomId, ref Panel p)
        {

            FileStream fs = new FileStream(GetFilePath("Ez_Hunt.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs);
            string s = sr.ReadLine();//header
            string[] result = { };
            string code = "";
            string[] spearator = { "," };
            s = sr.ReadLine();//current hunt Id and current clue Id
            result = s.Split(spearator, 20, StringSplitOptions.RemoveEmptyEntries);
            string HuntId = result[0]; string currentClueId = result[1];
            int n = System.Convert.ToInt32(currentClueId); n++;
            string nextClueId = n.ToString();
            while ((!sr.EndOfStream) && (result[0] != "X"))
            {
                s = sr.ReadLine();
                result = s.Split(spearator, 20, StringSplitOptions.RemoveEmptyEntries);
                if (result[1] == currentClueId) code = result[6];
                if ((result[0] == HuntId) && (result[2] == RoomId) && (result[1] == nextClueId)) //[1] is clueId
                {
                    sr.Close();
                    fs.Close();
                    p = new Panel();
                    p.ToolTip = "Clue Found Here!  :Found Code is " + code;
                    p.Style.Add("position", "absolute; top: " + result[4] + "px; left: " + result[3] + "px; height: 100px; width: 90px;");
                    BorderStyle b = new BorderStyle(); b = BorderStyle.Double;
                    //p.BorderWidth = 4;p.BorderStyle = b;
                    //Button bb = new Button(); bb.Text = result[1]; p.Controls.Add(bb);

                    return true;
                }
            }
            sr.Close();
            fs.Close();
            return false;
        }

        public string GetFilePath(string filename)
        {
            string s = "";
            s = HostingEnvironment.MapPath(@"/App_Data/" + filename);//OK for web apps
            if (s == null)
            {
                s = @"../App_Data/" + filename;
            }
            return s;
        }

        public void ResetClue(string HuntId)
        {

            FileStream fs1 = new FileStream(GetFilePath("Ez_Hunt_temp.txt"), FileMode.Open, FileAccess.ReadWrite);
            FileStream fs = new FileStream(GetFilePath("Ez_Hunt.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamWriter srw = new StreamWriter(fs1);
            StreamReader sr = new StreamReader(fs);
            srw.WriteLine(sr.ReadLine());//header
            string[] spearator = { "," };
            string s = sr.ReadLine();//current hunt Id and current clue Id
            string[] result = s.Split(spearator, 20, StringSplitOptions.RemoveEmptyEntries);
            s = HuntId + ",0,0,0,0,,0";
            srw.WriteLine(s);
            while (!sr.EndOfStream) { srw.WriteLine(sr.ReadLine()); }
            sr.Close();
            srw.Close();
            fs1.Close();
            fs.Close();

            //now copy temp to real
            fs1 = new FileStream(GetFilePath("Ez_Hunt.txt"), FileMode.Open, FileAccess.ReadWrite);
            fs = new FileStream(GetFilePath("Ez_Hunt_temp.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
            srw = new StreamWriter(fs1);
            sr = new StreamReader(fs);
            while (!sr.EndOfStream) { srw.WriteLine(sr.ReadLine()); }
            sr.Close();
            srw.Close();
            fs1.Close();
            fs.Close();
            return;
        }
    }
}