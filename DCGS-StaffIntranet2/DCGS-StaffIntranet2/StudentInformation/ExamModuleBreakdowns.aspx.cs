using System;
using System.IO;
using Cerval_Library;


namespace StudentInformation
{
    public partial class ExamModuleBreakdowns : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Count >= 1)
                {
                    string type = Request.QueryString["Type"];
                    if (type == "Student")
                    {
                        string Paper = Request.QueryString["Paper"];
                        string Date = Request.QueryString["Date"];
                        string Id = Request.QueryString["Id"];
                        string path = Request.QueryString["path"];
                        PupilDetails pupil1 = new PupilDetails(Id);
                        try
                        {
                            //construct filename.....  
                            string fname = path;
                            //date is 11/03/2012 etc.....
                            fname += Date.Substring(6, 4) + "_";
                            //for month want to go back to either 1 or 6.....
                            int m = System.Convert.ToInt16(Date.Substring(3, 2));
                            if (m < 5) fname += "01_"; else fname += "06_";
                            fname += Paper.Trim(); fname += ".txt";


                            //going to open the file

                            Cerval_Library.TextReader tr1 = new Cerval_Library.TextReader();

                            {
                                FileStream f1 = new FileStream(fname, FileMode.Open);
                                Cerval_Library.TextRecord tx1 = new Cerval_Library.TextRecord();
                                tr1.ReadTextLine(f1, ref  tx1);
                                //field 3 onwards are q nos..
                                string[] questions = new string[50];
                                for (int i = 3; i < tx1.number_fields + 1; i++)
                                {
                                    questions[i] = tx1.field[i];
                                }
                                tr1.ReadTextLine(f1, ref  tx1);
                                //field 3 onwards are max marks..
                                string[] max = new string[50];
                                for (int i = 3; i < tx1.number_fields + 1; i++)
                                {
                                    max[i] = tx1.field[i];
                                }

                                //row 3 has descriptions...
                                tr1.ReadTextLine(f1, ref  tx1);
                                //field 3 onwards are max marks..
                                string[] desc = new string[50];
                                for (int i = 3; i < tx1.number_fields + 1; i++)
                                {
                                    desc[i] = tx1.field[i];
                                }

                                while (tr1.ReadTextLine(f1, ref  tx1) == Cerval_Library.TextReader.READ_LINE_STATUS.VALID)
                                {
                                    if (tx1.field[1].Trim() == pupil1.m_adno.ToString())
                                    {
                                        string s = tx1.field[0];//this is uci
                                        s = "<h3>Paper Breakdowns for " + pupil1.m_GivenName + " " + pupil1.m_Surname + " for" + Paper + "</h3><br /><TABLE BORDER   class=\"ResultsTbl\" style = \"font-size:small ;  \">";
                                        s += "<tr><th>Question</th><th>Max Mark</th><th>Your Mark</th><th>Question Description</th></tr>";
                                        for (int i = 3; i < tx1.number_fields + 1; i++)
                                        {
                                            s += "<tr><td>" + questions[i] + "</td><td>" + max[i] + "</td><td>" + tx1.field[i] + "</td><td>" + desc[i] + "</td></tr>";
                                        }
                                        s += "</table>";

                                        s += "<br/><h3>Please  note these are raw not UMS marks.</h3>";
                                        content4.InnerHtml = s;
                                        f1.Close();
                                        break;
                                    }
                                }
                                f1.Close();
                            }
                        }
                        catch (Exception e1)
                        {
                            //assume file not found....
                           content4.InnerText = "<h3> No data found for this paper</h3></br> ("+e1.Message+")";

                        }

                    }
                }

            }
        }
    }
}
