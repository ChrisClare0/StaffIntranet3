using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Cerval_Library;


namespace DCGS_Staff_Intranet2
{
    public partial class ComplexEmail : System.Web.UI.Page
    {
        //going to have to save the data in the viewstate... ugh
        string message = "";
        Dictionary<string, string> msglist = new Dictionary<string, string>();
        //will store email address and isert field
        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
           
#else

#endif

        }

        protected void Button_Check_Click(object sender, EventArgs e)
        {
            if (TextBox_emailSubject.Text == "")
            {
                TextBox_Message.Text = "You must give a subject"; return;
            }
            if (!FileUpload_Message.HasFile)
            {
                TextBox_Message.Text = "You must select a message file"; return;
            }
            if (!FileUpload_Recipients.HasFile)
            {
                TextBox_Message.Text = "You must select a recipients file"; return;
            }
            //so we have the two files... let's run through...
            System.IO.StreamReader sr = new System.IO.StreamReader(FileUpload_Message.FileContent);
            int n=0;
            message = sr.ReadToEnd();
            ViewState.Add("msg", message);
            bool has_insert = false;
            if (message.Contains("<message>"))
            {
                has_insert = true;
                n = message.IndexOf("<message>");
            }
            sr.Close();
            System.IO.StreamReader sr1 = new System.IO.StreamReader(FileUpload_Recipients.FileContent);


            Cerval_Library.TextReader tr = new Cerval_Library.TextReader();
            Cerval_Library.TextRecord t1 = new TextRecord();

            tr.ReadTextLine(sr1.BaseStream, ref t1);
            //we expect t1 to have Email  and Text columns
            if (t1.field[0].ToUpper() != "EMAIL")
            {
                TextBox_Message.Text = "Recipient file must have an Email field. "; return;
            }

            if (has_insert&&(t1.field[1].ToUpper() != "TEXT"))
            {
                TextBox_Message.Text = "Recipient file must have a TEXT field as message has a <message> marker. "; return;
            }

            tr.ReadTextLine(sr1.BaseStream, ref t1);
            string s= message;
            //generate first message
            if(has_insert)
            {
                s = message.Substring(0, n);
                s += t1.field[1];
                s += message.Substring(n + 9);
            }
            else
            {
            }
            msglist.Add(t1.field[0], t1.field[1]);
            SampleText.InnerHtml = "Text to be emailed is:<br/><br/>"+s;
            TextBox1.Text += t1.field[0]+Environment.NewLine;
            while (tr.ReadTextLine(sr1.BaseStream, ref t1) == Cerval_Library.TextReader.READ_LINE_STATUS.VALID)
            {
                if(!msglist.ContainsKey(t1.field[0]))//ignore duplicates...
                {
                    TextBox1.Text += t1.field[0] + Environment.NewLine;
                    msglist.Add(t1.field[0], t1.field[1]);
                }
            }

            Button_Send.Visible = true;
            ViewState.Add("list", msglist);
        }

        protected void Button_Send_Click(object sender, EventArgs e)
        {
            ///so we are going to do it...
            message = (string)ViewState["msg"];
            msglist = (Dictionary<string, string>)ViewState["list"];
            bool has_insert = false; int n = 0; string s = ""; int no_sent = 0;
            MailHelper m = new MailHelper();

            if (message.Contains("<message>"))
            {
                has_insert = true;
                n = message.IndexOf("<message>");
            }

            foreach ( KeyValuePair<string, string> f in msglist)
            {
                s = message;
                if (has_insert)
                {
                    s = message.Substring(0, n);
                    s += f.Value;
                    s += message.Substring(n + 9);
                }
                string email_address = f.Key;
                //m.SendMail("cc@challoners.com", email_address, "", s, "", TextBox_emailSubject.Text);
                m.SendMail("cc@challoners.com", email_address, @"C:\Users\cc\Desktop\pto\Year10letter.pdf", s, "", TextBox_emailSubject.Text);
                no_sent++;
            }
            TextBox_Message.Text = "Sent " + no_sent.ToString() + " emails";
        }


    }
}
