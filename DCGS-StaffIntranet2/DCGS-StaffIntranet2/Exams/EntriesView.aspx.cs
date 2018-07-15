using System;
using System.Data;
using System.Data.SqlClient;
using Cerval_Library;
using System.Collections.Generic;
using System.Web.UI.WebControls;


namespace DCGS_Staff_Intranet2.Exams
{
    public partial class EntriesView : System.Web.UI.Page
    {
        #region variables

        public string DisplayType
        {
            get { return (((String)ViewState["Type"] == null) ? String.Empty : (String)ViewState["Type"]); }
            set { ViewState["Type"] = value; }
        }
        public string StudentAdno
        {
            get { return (((String)ViewState["StudentAdno"] == null) ? String.Empty : (String)ViewState["StudentAdno"]); }
            set { ViewState["StudentAdno"] = value; }
        }
        public string CourseId
        {
            get { return (((String)ViewState["CourseId"] == null) ? String.Empty : (String)ViewState["CourseId"]); }
            set { ViewState["CourseId"] = value; }
        }

        public int Year
        {   //this is the year in 2016 format
            get { try { return (int)ViewState["Year"]; } catch { return 0; } }
            set { ViewState["Year"] = value; }
        }
        public int YearCode
        {   //this is the year in 16 format
            get { try { return (int)ViewState["YearCode"]; } catch { return 0; } }
            set { ViewState["YearCode"] = value; }
        }
        public int SeasonCode
        {
            get { try { return (int)ViewState["SeasonCode"]; } catch { return 0; } }
            set { ViewState["SeasonCode"] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetMasterValues();
                DropDownList1.Visible = false;
                GridView2.Visible = false;
                GridView1.Visible = false;
                ListBox1.Visible = false;
                TextBox1.Visible = false;
                Label_Select3.Text = "";
                if (Request.QueryString.Count >= 1)
                {
                    DisplayType = Request.QueryString["Type"];
                }
                switch (DisplayType)
                {
                    case "Student":
                        Label_Select1.Text = "Select Year:";
                        Label_Select3.Text = "OR type Admission Number:";
                        DropDownList1.Visible = true;
                        DropDownList1.Items.Add("14");
                        DropDownList1.Items.Add("13");
                        DropDownList1.Items.Add("12");
                        DropDownList1.Items.Add("11");
                        DropDownList1.Items.Add("10");
                        DropDownList1.SelectedIndex = 1;
                        DropDownList1.Visible = true;
                        ListBox1.Visible = true;
                        TextBox1.Visible = true;
                        GridView2.Visible = true;
                    
                        StudentYearList styl1 = new StudentYearList(ListBox1, DropDownList1.SelectedValue+"Year", DateTime.Now);

                        break;
                    case "Option":
                        Label_Select1.Text = "Select Option:";
                        DropDownList1.Visible = true;
                        GridView1.Visible = true;
                        DropDownList1.Items.Clear();
                        ExamOption_List exol1 = new ExamOption_List();exol1.Load(YearCode.ToString(), SeasonCode.ToString());
                        foreach (ExamOption exo in exol1.m_list)
                        {
                            DropDownList1.Items.Add(new ListItem(exo.m_OptionCode, exo.m_OptionID.ToString()));
                        }
                        DropDownList1.SelectedIndex = -1;
                        break;
                    case "Subject":
                        Label_Select1.Text = "Select Subject:";
                        DropDownList1.Visible = true;
                        GridView1.Visible = true;
                        ListItem l1 = new ListItem("All","0");
                        DropDownList1.Items.Add(l1);
                        CourseList cl1 = new CourseList(0);
                        foreach (Course c in cl1._courses)
                        {
                            if (c.KeyStage == 5)
                            {
                                ListItem l = new ListItem(c.CourseName+"( A-level)", c._CourseID.ToString());
                                DropDownList1.Items.Add(l);
                            }
                            if (c.KeyStage == 4)
                            {
                                ListItem l = new ListItem(c.CourseName + "( GCSE)", c._CourseID.ToString());
                                DropDownList1.Items.Add(l);
                            }
                        }
                        DropDownList1.SelectedIndex = -1;
                        break;
                    case "Status":
                        Label_Select1.Text = "Select Status:";
                        DropDownList1.Visible = true;
                        GridView1.Visible = true;
                        Exam_Entry_Status_List el1 = new Exam_Entry_Status_List();
                        foreach (Exam_Entry_Status e1 in el1.m_list)
                        {
                            ListItem l2 = new ListItem(e1.m_EntryStatusDescription, e1.m_EntryStatusCode);
                            DropDownList1.Items.Add(l2);

                        }
                        //going to add extra line for Student resits...
                        ListItem l3 = new ListItem("Resit Request waiting to be processed","RESITS");
                        DropDownList1.Items.Add(l3);
                        DropDownList1.SelectedIndex = -1;

                        break;
                    default:
                        break;
                }
            }
        }

        private void GetMasterValues()
        {
            SeasonCode = System.Convert.ToInt32((string)(Session["Season"]));
            Year = System.Convert.ToInt32((string)(Session["Year"]));
            YearCode = Year % 100;
            Label Label_Banner = (Label)Master.FindControl("Label_Banner");
            if (Label_Banner != null)
            {
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]);
            }
        }

        private DataSet GetData(string Query)
        {
            Encode en = new Encode();
            DataSet ds = new DataSet();
            try
            {
                SqlConnection connection = new SqlConnection(en.GetDbConnection());
                SqlDataAdapter adapter = new SqlDataAdapter(Query, connection);
                adapter.Fill(ds);
            }
            catch
            {
            }
            return ds;
        }


        private string GetQueryStringX()
        {
            string s = "SELECT ExamEntryID, StudentAdmissionNumber, PersonGivenName, PersonSurname, StudentExamNumber, OptionCode, OptionTitle, SyllabusTitle, DateCreated, DateEntered, Withdrawn, ExamYear, ExamSeason,EntryStatusCode  FROM dbo.qry_Cerval_Exams_Entries WHERE (ExamYear='" + Year.ToString() + "') AND ( ExamSeason='" + SeasonCode.ToString() + "' ) ";
            //if (checkBox_notsent.Checked) s += " AND   (EntryFileID IS  NULL) ";
            //if (checkBox_all.Checked) s += " AND (ExamBoardID <> '436ff234-0457-430a-b1e2-b08758ff30ef') ";
            return s;
        }
        private string GetWhereString()
        {
            string s = " WHERE (ExamYear='" + Year.ToString() + "') AND ( ExamSeason='" + SeasonCode.ToString() + "' ) ";
            switch (DisplayType)
            {
                case "Student":
                    s += " AND StudentAdmissionNumber ='" + StudentAdno + "' ";
                    break;
                case "Option":
                    s += " AND OptionCode = '" + DropDownList1.SelectedItem.Text + "' ";
                    break;
                case "Subject":
                    //need to work out which options....
                    if (CourseId == "0")
                    {//so load all options...
                        s = "HAVING (ExamYear = '" + Year.ToString() + "') AND(ExamSeason = '" + SeasonCode.ToString() + "')";return s;
                    }
                    ExamOption_List el1 = new ExamOption_List();
                    el1.LoadforCourse(new Guid(CourseId),YearCode.ToString(),SeasonCode.ToString());
                    s = "";
                    bool first = true;
                    foreach (ExamOption e in el1.m_list)
                    {
                        if (first) { first = false; s = " HAVING "; } else { s += " OR "; }
                        s+= " (OptionCode = '"+e.m_OptionCode + "' ) AND (ExamYear = '"+Year.ToString()+ "' ) AND (ExamSeason = '"+SeasonCode.ToString()+"')";
                    }
                    if(first)
                    { //we didn't have any options...
                        s = " HAVING (OptionCode = 'ZZZZZZ' )";  //null return!
                    }
                    break;
                case "Status":
                    s += " AND (EntryStatusCode  ='" + DropDownList1.SelectedItem.Value + "' ) ";
                    break;
            }
            return s;
        }
        private string GetQueryString()
        {
            string s = "";
            switch (DisplayType)
            {
                case "Student":
                    s = "SELECT  ExamEntryID AS ID, OptionCode, OptionTitle AS Title, SyllabusTitle AS Syllabus,  Withdrawn,EntryStatusCode AS Status";
                    s += " FROM dbo.qry_Cerval_Exams_Entries ";
                    s += GetWhereString();
                    s += " ORDER BY OptionCode ";
                    break;
                case "Option":
                    s = "SELECT StudentExamNumber AS CandNo, PersonGivenName AS GivenName, PersonSurname AS Surname,  Withdrawn,EntryStatusCode AS Status";
                    s += " FROM dbo.qry_Cerval_Exams_Entries ";
                    s += GetWhereString();
                    s += " ORDER BY StudentExamNumber ";
                    break;
                case "Subject":
                    s = " SELECT OptionCode,OptionTitle AS Title, SyllabusTitle AS Syllabus, COUNT(ExamEntryID) AS Number_of_Entries ";
                    s += " FROM   dbo.qry_Cerval_Exams_Entries ";
                    s += " GROUP  BY OptionCode,OptionTitle, SyllabusTitle, ExamYear, ExamSeason ";
                    s += GetWhereString();
                    s += " ORDER BY OptionCode ";
                    break;
                case "Status":
                    if (DropDownList1.SelectedValue == "RESITS")
                    {
                        s = " SELECT OptionCode, StudentAdmissionNumber AS CandNo, PersonGivenName AS GivenName, PersonSurname AS Surname  ";
                        s += " FROM  qry_Cerval_Exams_ResitEntries";
                        s += " ORDER BY PersonSurname ";
                    }
                    else
                    {
                        s = " SELECT OptionCode, StudentExamNumber AS CandNo, PersonGivenName AS GivenName, PersonSurname AS Surname  ";
                        s += " FROM dbo.qry_Cerval_Exams_Entries ";
                        s += GetWhereString();
                        s += " ORDER BY OptionCode ";
                    }
                    break;
            }
            return s;
        }


        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Encode en = new Encode();
            switch (DisplayType)
            {
                case "Student":
                    StudentYearList styl1 = new StudentYearList(ListBox1, DropDownList1.SelectedValue + "Year", DateTime.Now);
                    if (DropDownList1.SelectedValue == "14")
                    {
                        styl1.Load_NotOnRole_ExamEntries(ListBox1, Year.ToString(), SeasonCode.ToString());
                    }
                    ListBox1.SelectedIndex = -1;
                    break;
                case "Option":
                    SqlDataSource1.SelectCommand = GetQueryString();
                    SqlDataSource1.ConnectionString = en.GetDbConnection();
                    SqlDataSource1.DataBind();                  
                    string s="SELECT COUNT (ExamEntryID)  FROM dbo.qry_Cerval_Exams_Entries " + GetWhereString();
                    Label_Grid.Text = "Entries for " + DropDownList1.SelectedItem.Text + "  Total Number =" + en.Execute_one_SQL(s);
                    break;
                case "Subject":
                    CourseId = DropDownList1.SelectedValue;
                    SqlDataSource1.SelectCommand = GetQueryString();
                    SqlDataSource1.ConnectionString = en.GetDbConnection();
                    SqlDataSource1.DataBind();
                    Label_Grid.Text = "";
                    if (SqlDataSource1.SelectCommand.Contains("ZZZZZZ"))
                    {
                        Label_Grid.Text = "None found";
                    }
                    //the other option for none is that there are options found for this seasonby no entries... have to run the querry...
                    int n = 0;
                    using (SqlConnection cn = new SqlConnection(en.GetDbConnection()))
                    {
                        cn.Open();
                        using (SqlCommand cm = new SqlCommand(SqlDataSource1.SelectCommand, cn))
                        {
                            using (SqlDataReader dr = cm.ExecuteReader())
                            {
                                while (dr.Read()) n++;
                                dr.Close();
                            }
                        }
                        cn.Close();
                    }
                    if (n == 0) { Label_Grid.Text = "None found"; }
                    break;
                case "Status":
                    SqlDataSource1.SelectCommand = GetQueryString();
                    SqlDataSource1.ConnectionString = en.GetDbConnection();
                    SqlDataSource1.DataBind();
                    if(DropDownList1.SelectedValue=="RESITS")
                    {
                        string s1 = "SELECT COUNT (OptionCode)  FROM qry_Cerval_Exams_ResitEntries ";
                        Label_Grid.Text = "Entries for Status =  " + DropDownList1.SelectedItem.Text + "  Total Number =" + en.Execute_one_SQL(s1);
                    }
                    else { 
                    string s1 = "SELECT COUNT (ExamEntryID)  FROM dbo.qry_Cerval_Exams_Entries " + GetWhereString();
                    Label_Grid.Text = "Entries for Status =  " + DropDownList1.SelectedItem.Text + "  Total Number =" + en.Execute_one_SQL(s1);
                    }
                    break;



            }
        }


        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            switch (DisplayType)
            {
                case "Student":        
                    //so new Adno
                    int i = 0;
                    try
                    {
                        i = System.Convert.ToInt32(TextBox1.Text);
                        StudentAdno = i.ToString();
                        SimplePupil p = new SimplePupil(); p.Load_Left(i);
                        Encode en = new Encode();
                        SqlDataSource2.SelectCommand = GetQueryString();
                        SqlDataSource2.ConnectionString = en.GetDbConnection();
                        SqlDataSource2.DataBind();
                        string s = "SELECT COUNT (ExamEntryID)  FROM dbo.qry_Cerval_Exams_Entries " + GetWhereString();
                        Label_Grid.Text = "Entries for " + p.m_GivenName+" "+p.m_Surname+" ("+p.m_adno.ToString()+") ....  Total Entries =" + en.Execute_one_SQL(s);
                    }
                    catch { }
                    break;
            }
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            SqlDataSource1.SelectCommand = GetQueryString();
            Encode en = new Encode();
            SqlDataSource1.ConnectionString = en.GetDbConnection();
            SqlDataSource1.DataBind();
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            SqlDataSource1.SelectCommand = GetQueryString();
            Encode en = new Encode();
            SqlDataSource1.ConnectionString = en.GetDbConnection();
            SqlDataSource1.DataBind();
        }

        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Utility u = new Utility();
            int i = u.GetAdmissionNumber(ListBox1.SelectedValue);
            StudentAdno = i.ToString();
            SimplePupil p = new SimplePupil(); p.Load_Left(i);
            Encode en = new Encode();
            SqlDataSource2.SelectCommand = GetQueryString();
            SqlDataSource2.ConnectionString = en.GetDbConnection();
            SqlDataSource2.DataBind();
            string s = "SELECT COUNT (ExamEntryID)  FROM dbo.qry_Cerval_Exams_Entries " + GetWhereString();
            Label_Grid.Text = "Entries for " + p.m_GivenName + " " + p.m_Surname + " (" + StudentAdno + ") ....  Total Entries =" + en.Execute_one_SQL(s);
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView2.Rows[index];
                if(row.BackColor == System.Drawing.Color.DarkGray)
                {
                    row.BackColor = System.Drawing.Color.White;
                }
                else
                {
                    row.BackColor = System.Drawing.Color.DarkGray;
                    Button_Delete.Visible = true;
                }
            }
            Button_Delete.Visible = false;
            foreach (GridViewRow r in GridView2.Rows)
            {
                if (r.BackColor == System.Drawing.Color.DarkGray)
                {
                    Button_Delete.Visible = true;
                }
            }


        }
        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            SqlDataSource2.SelectCommand = GetQueryString();
            Encode en = new Encode();
            SqlDataSource2.ConnectionString = en.GetDbConnection();
            SqlDataSource2.DataBind();
        }

        protected void GridView2_Sorting(object sender, GridViewSortEventArgs e)
        {
            SqlDataSource2.SelectCommand = GetQueryString();
            Encode en = new Encode();
            SqlDataSource2.ConnectionString = en.GetDbConnection();
            SqlDataSource2.DataBind();
        }


        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            Panel2.Visible = true;
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
        }

        protected void Button_OK_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
            //do it
            string s = "";
            foreach (GridViewRow r in GridView2.Rows)
            {
                if (r.BackColor == System.Drawing.Color.DarkGray)
                {
                    s = r.Cells[0].Text;
                    Exam_Entry ex1 = new Exam_Entry();
                    ex1.Load(s);
                    if (ex1.CanDelete())
                    {
                        s = "delete";
                        ex1.Delete();
                    }
                    else
                    {
                        s = "withdraw";
                        ex1.Withdraw();
                    }
                }
            }
            SqlDataSource2.SelectCommand = GetQueryString();
            Encode en = new Encode();
            SqlDataSource2.ConnectionString = en.GetDbConnection();
            SqlDataSource2.DataBind();
            Button_Delete.Visible = false;
        }
    }
}