using System;
using System.Configuration;
using System.Web.UI.WebControls;
using Cerval_Library;


namespace StudentInformation
{
    public partial class ExamModuleRetakes : System.Web.UI.Page
    {
        public Guid PersonID = new Guid();
        ResultsList resultlist1 = new ResultsList();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsRetakeActive())
            {
                servercontent.InnerHtml="<H3><center>No re-take entries are possible at this time.</center></H3>"; 
                Label1.Visible=false;TextBox_cost.Visible=false;
                Button_submit.Visible=false;
                Label_title.Visible = false;
                return;
            }

            Utility u1 = new Utility();
            PersonID = u1.GetPersonIdfromRequest(Request);
#if DEBUG
            //u1.Is_student = true; PersonID = u1.GetPersonIDX(@"CHALLONERS\michael.warren");//development
            //u1.Is_student = true;PersonID = u1.GetPersonIDX(@"CHALLONERS\Henry.Anning");//development             
#endif
            //u1.Is_student = true; PersonID = u1.GetPersonIDX(@"CHALLONERS\Benjamin.Smith1");//development
            if ((PersonID == Guid.Empty))
            {
                Response.Write("<center> No Data to display</center>"); return;
            }
            
            if (!IsPostBack)
            {
                ViewState.Add("PersonID", PersonID);
                ResultsList rl = new ResultsList();
                rl.LoadList("StudentPersonId", PersonID.ToString());
                resultlist1._results.Clear();
                ExamOption exo1 = new ExamOption();

                TextBox_cost.Text = "0";
                //first time we load ... get the results data...
                foreach (Result res1 in rl._results)
                {
                    if (res1.OptionItem == "U")
                    {
                        Result r1 = new Result();
                        r1 = res1; r1.Valid = false;            
                        exo1.Load(res1.OptionId);
                        exo1.Load(exo1.m_OptionCode, RetakeSeason(), RetakeYear(), exo1.m_ExamBoardID);
                        r1.Valid = exo1.m_valid;
                        //if (exo1.NeedsTeacherMarks()) r1.Valid = false;//try to disable csewrk modules..??
                        resultlist1._results.Add(r1);
                        r1.OptionId = exo1.m_OptionID;//update optionID!!!!
                    }
                }
                ViewState.Add("ResultList",resultlist1);    
            }
            resultlist1= (ResultsList)ViewState["ResultList"];
            int cost = AddControls(resultlist1,!IsPostBack);
            if (!IsPostBack)
            {
                double x = (double)cost; 
                x = x / 100;
                TextBox_cost.Text = x.ToString();
                ViewState.Add("cost", cost);
            }
        }

        protected bool IsRetakeActive(){return (new Cerval_Configuration("StudentInformation_RetakeActive").Value.ToUpper() == "TRUE") ? true : false;}
        protected string RetakeYear(){ return new Cerval_Configuration("StudentInformation_RetakeYear").Value;}
        protected string RetakeSeason(){return new Cerval_Configuration("StudentInformation_RetakeSeason").Value; }
        protected string RetakeMessage() {return new Cerval_Configuration("StudentInformation_RetakeMessage").Value; }
        protected int RetakeCoverCost() { return System.Convert.ToInt32(new Cerval_Configuration("StudentInformation_RetakeCoverCost").Value); }
        protected void SetCellStyle(TableCell c1, Table tb1){c1.BorderWidth = tb1.BorderWidth;}

        protected int AddControls(ResultsList rl,bool setup)
        {
            int cost=0;
            Guid[] listID = new Guid[100];
            Exam_ResitEntry ExamResit1 = new Exam_ResitEntry();
            Exam_Entry entry1 = new Exam_Entry();
            Table1.Visible = true;
            Table tb1 = (Table)content.FindControl("table1");
            tb1.Width = 700;
            tb1.EnableViewState = true;
            TableRow r0 = new TableRow();
            tb1.Controls.Add(r0);
            TableCell c01 = new TableCell(); r0.Controls.Add(c01); SetCellStyle(c01, tb1);
            TableCell c02 = new TableCell(); r0.Controls.Add(c02); SetCellStyle(c02, tb1);
            TableCell c03 = new TableCell(); r0.Controls.Add(c03); SetCellStyle(c03, tb1);
            TableCell c04 = new TableCell(); r0.Controls.Add(c04); SetCellStyle(c04, tb1);
            TableCell c05 = new TableCell(); r0.Controls.Add(c05); SetCellStyle(c05, tb1);
            c01.Text = "Code"; c02.Text = "Title"; c03.Text = "Result"; c04.Text = "Date Taken"; c05.Text = "Retake ?";

            foreach (Result res1 in rl._results)
            {
                if (res1.OptionItem == "U")
                {//we have a module result
                    TableRow r1 = new TableRow(); 
                    tb1.Controls.Add(r1);
                    r1.BorderStyle = BorderStyle.Solid;
                    r1.BorderColor = tb1.BorderColor;
                    r1.BorderWidth = tb1.BorderWidth;
                    TableCell c1 = new TableCell(); r1.Controls.Add(c1); SetCellStyle(c1, tb1);
                    TableCell c2 = new TableCell(); r1.Controls.Add(c2); SetCellStyle(c2, tb1);
                    TableCell c3 = new TableCell(); r1.Controls.Add(c3); SetCellStyle(c3, tb1);
                    TableCell c4 = new TableCell(); r1.Controls.Add(c4); SetCellStyle(c4, tb1);
                    TableCell c5 = new TableCell(); r1.Controls.Add(c5); SetCellStyle(c5, tb1);


                    if ((res1.Valid) && ((CheckBox)content.FindControl(res1.OptionId.ToString()) == null))
                    {
                        entry1.Load(res1.OptionId, res1.StudentID);
                        //now if already entered we don't want to offer.....
                        if (entry1.m_valid)
                        {
                            c5.Text = "Entered";
                        }
                        else
                        {
                            CheckBox cb1 = new CheckBox();
                            cb1.ID = res1.OptionId.ToString();
                            c5.Controls.Add(cb1);
                            //decide if we already have an entry....
                            if (setup)
                            {
                                ExamResit1.Load(res1.StudentID, res1.OptionId);
                                if (ExamResit1.m_valid)
                                {
                                    cb1.Checked = true;
                                    ExamOption exo1 = new ExamOption();
                                    exo1.Load(res1.OptionId);
                                    cost += exo1.m_feeInt + RetakeCoverCost();
                                }
                            }
                            cb1.CheckedChanged += new EventHandler(cb_CheckedChanged);
                            cb1.AutoPostBack = true;
                        }
                    }
                    else
                    {
                        c5.Text = "Not available";
                    }
                    c1.Text = res1.OptionCode;
                    c2.Text = res1.OptionTitle;
                    c3.Text = res1.Value;
                    c4.Text = res1.Date.ToShortDateString();
                }
            }
            return cost;
        }

        protected void cb_CheckedChanged(object sender, EventArgs e)
        {
            string s = sender.ToString();
            CheckBox cb1 = (CheckBox)sender;
            string Option_id = cb1.ID;
            Guid g1 = new Guid(Option_id);
            //if it has been checked.... increase cost.....
            ExamOption exo1 = new ExamOption(); exo1.Load(g1);
            int cover_cost = RetakeCoverCost();
            int cost = (int)ViewState["cost"];
            if (cb1.Checked)
            {
                cost += exo1.m_feeInt + cover_cost;
            }
            else
            {
                cost -= (exo1.m_feeInt + cover_cost);
            }
            ViewState.Add("cost", cost);
            double x = (double)cost; x = x / 100;
            TextBox_cost.Text = x.ToString();
        }
    /*
        private void RenderModuleResultLine(HttpResponse Response, Result r1,bool retake)
        {
            string s = "";
            ExamOption exo1 = new ExamOption();
            Response.Write("<TR>");
            s = r1.Date.ToShortDateString(); Response.Write("<TD>" + s + "</TD>");
            s = r1.Coursename; Response.Write("<TD>" + s + "</TD>");
            s = r1.OptionTitle; Response.Write("<TD>" + s + "</TD>");
            s = r1.OptionCode; Response.Write("<TD>" + s + "</TD>");
            s = r1.Value; Response.Write("<TD>" + s + "</TD>");
            s = r1.OptionMaximumMark; Response.Write("<TD>" + s + "</TD>");

            s = "";
            if (r1.Resulttype == 27)//UMS mark for GCSE
            {
                try
                {
                    double v1 = System.Convert.ToDouble(r1.Value);
                    double v2 = System.Convert.ToDouble(r1.OptionMaximumMark);
                    v1 = v1 / v2;
                    if (v1 >= 0.9) s = "a*";
                    if ((v1 < 0.9) && (v1 >= 0.8)) s = "a";
                    if ((v1 < 0.8) && (v1 >= 0.7)) s = "b";
                    if ((v1 < 0.7) && (v1 >= 0.6)) s = "c";
                    if ((v1 < 0.6) && (v1 >= 0.5)) s = "d";
                    if ((v1 < 0.5) && (v1 >= 0.4)) s = "e";
                    if ((v1 < 0.4)) s = "u";
                }
                catch (Exception exc1)
                {
                    s = exc1.Message;
                }
            }
            if (r1.Resulttype == 11)//GCE module mark
            {
                try
                {
                    double v1 = System.Convert.ToDouble(r1.Value);
                    double v2 = System.Convert.ToDouble(r1.OptionMaximumMark);
                    v1 = v1 / v2;
                    if (v1 >= 0.8) s = "a";
                    if ((v1 < 0.8) && (v1 >= 0.7)) s = "b";
                    if ((v1 < 0.7) && (v1 >= 0.6)) s = "c";
                    if ((v1 < 0.6) && (v1 >= 0.5)) s = "d";
                    if ((v1 < 0.5) && (v1 >= 0.4)) s = "e";
                    if ((v1 < 0.4)) s = "u";
                }
                catch (Exception exc1)
                {
                    s = exc1.Message;
                }
            }

            Response.Write("<TD>" + s + "</TD>");
            exo1.Load(r1.OptionId);
            Exam_Board exb1 = new Exam_Board(exo1.m_ExamBoardID);
            Response.Write("<TD>" + exb1.m_OrganisationFriendlyName + "</TD>");
            if (retake)
            {
                CheckBox cb1 = new CheckBox();
                cb1.ID = "CheckBox-" + r1.OptionId.ToString() ;
                Response.Write("<TD><asp:CheckBox ID=\"" + cb1.ID.ToString() + "\"  runat=\"server\" /></TD>");
            }
            Response.Write("</TR>");
        }

        private void RenderModuleResultTableHeader(HttpResponse Response, Guid PersonId,bool retake)
        {
            Utility u1 = new Utility();
            Response.Write("<H1><center> Module Results for " + u1.GetPersonName(PersonID) + "</H1></center><BR>");
            Response.Write("<BR><p  align=\"center\"><TABLE BORDER><TR>");
            Response.Write("<TD>Date</TD>");
            Response.Write("<TD>Subject</TD>");
            Response.Write("<TD>Option Title</TD>");
            Response.Write("<TD>Option Code</TD>");
            Response.Write("<TD>Result</TD>");
            Response.Write("<TD>Max Mark</TD>");
            Response.Write("<TD>Grade</TD>");
            Response.Write("<TD>Exam Board</TD>");
            if (retake) Response.Write("<TD>Retake ?</TD>");
            Response.Write("</TR>");
        }
        */
        protected void Button_submit_Click(object sender, EventArgs e)
        {
            Guid PersonID = (Guid)ViewState["PersonID"];
            Table1.Visible = false;
            Label1.Visible = false;
            TextBox_cost.Visible = false;
            Button_submit.Visible = false;
            Label_title.Visible = false;
            ExamOption exo1 = new ExamOption();
            ExamCompononent_List ecl1 = new ExamCompononent_List();
            string dates = "";
            double cost = 0;
            double total = 0;
            Utility u1 = new Utility();
            Guid[] list1 = new Guid[100]; int no_list1 = 0; bool in_list1 = false;
            PupilDetails pupil1 = new PupilDetails(u1.GetStudentId(PersonID).ToString());

            int cover_cost = RetakeCoverCost();
            string season = RetakeSeason();
            string year = RetakeYear();
            string s1 = RetakeMessage();
            string s2 = "";

            s2 = "<H2><center>Entry Requests for " + u1.GetPersonName(PersonID) + "  " + u1.Get_Form(pupil1.m_StudentId.ToString()) + "(" + pupil1.m_examNo.ToString() + ")</H2></center><BR>";
            s2 += "<H3><center>You should print this page and take it with your cheque ";
            s2 += "made out to DCGS to the Exams Office.</H3></center><BR><H3><center>" + s1 + "</H3></center><BR>";
 

            s2 += "<BR><p  align=\"center\"><TABLE BORDER><TR><TD>Option Code</TD><TD>Option Title</TD>";
            s2 += "<TD>Exam Board</TD><TD>Date</TD><TD>Cost</TD><TD>Comment</TD></TR>";

            resultlist1 = (ResultsList)ViewState["ResultList"];
            string s = "";
            CheckBox cb1 = new CheckBox();

            foreach (Result r1 in resultlist1._results)
            {
                s = r1.OptionId.ToString(); cb1 = null;
                cb1 = (CheckBox)content.FindControl(s);
                if (cb1 != null)
                {
                    if (cb1.Checked)
                    {
                        exo1.Load(r1.OptionId);
                        ecl1.m_list.Clear();
                        ecl1.Load(r1.OptionId);
                        Exam_ResitEntry exres1 = new Exam_ResitEntry();
                        exres1.Load(r1.StudentID, r1.OptionId);
                        in_list1 = false;
                        for (int i = 0; i < no_list1; i++)
                        {
                            if (list1[i] == r1.OptionId) in_list1 = true;
                        }

                        if (in_list1)
                        {
                            //already entered......
                        }
                        else
                        {
                            s2 += "<TR><TD>" + r1.OptionCode + "</TD><TD>" + r1.OptionTitle + "</TD>";
                            Exam_Board exb1 = new Exam_Board(exo1.m_ExamBoardID);
                            s2 += "<TD>" + exb1.m_OrganisationFriendlyName + "</TD>";
                            dates = "";
                            foreach (ExamComponent c1 in ecl1.m_list)
                            {
                                if (c1.m_Timetabled == "T")  dates += c1.m_TimetableDate.ToShortDateString();
                            }
                            s2+="<TD>" + dates + "</TD>";
                            cost = (double)exo1.m_feeInt + cover_cost;
                            exres1.Load(r1.StudentID, r1.OptionId);
                            if (!exres1.m_valid)
                            {
                                exres1.m_cost = (int)cost;
                                exres1.m_DateCreated = DateTime.Now;
                                exres1.m_OptionId = r1.OptionId;//has been updated when we loaded it to be the new one..
                                exres1.m_season = season;
                                exres1.m_year = year;
                                exres1.m_StudentId = u1.GetStudentId(PersonID);
                                exres1.m_version = 1;
                                exres1.Save();
                            }
                            cost = cost / 100;
                            total += cost;
                            s2 += "<TD>" + cost.ToString() + "</TD>";
                            if (exo1.NeedsTeacherMarks())
                            {
                                s2+="<TD>Warning - Needs CW</TD></TR>";
                            }
                            else
                            {
                                s2+="<TD></TD></TR>";
                            }
                            list1[no_list1] = r1.OptionId; no_list1++;
                        }
                    }
                    else
                    {
                        Exam_ResitEntry exres1 = new Exam_ResitEntry();
                        exres1.Load(r1.StudentID, r1.OptionId);
                        if (exres1.m_valid)  exres1.Delete();//need to delete it!!!;
                    }
                }
            }
            s2+="</TABLE> <H3><center>Total Cost : £" + total.ToString() + "</H3></center><H3><center>Dates are provisional!</H3></center>";
            servercontent.InnerHtml = s2;
        }
    }
}
