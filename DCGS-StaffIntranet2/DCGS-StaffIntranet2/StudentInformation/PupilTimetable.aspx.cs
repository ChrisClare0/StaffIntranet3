using System;
using Cerval_Library;

namespace StudentInformation
{
	public partial class PupilTimetable : System.Web.UI.Page
	{
		public string studentID;
        public Guid PersonID;
        public System.Web.UI.HtmlControls.HtmlGenericControl servercontent;
        public System.Web.UI.WebControls.Label Label1;
        public System.Web.UI.WebControls.Button Button1;

		protected void Page_Load(object sender, EventArgs e)
		{
			if(!IsPostBack)
			{
                string s = "";
                DateTime t1 = new DateTime(2011, 9, 8);
                t1 = DateTime.Now; servercontent.InnerHtml = "";
                Cerval_Configurations config = new Cerval_Configurations();
                config.Find_Key("StudentInformation_TimeTableDisplayDate", out s);
                Button1.Visible = false; Label1.Visible = false;
                if (s != "")
                {
                    try 
                    {
                        t1 = System.Convert.ToDateTime(s);
                        Button1.Visible = true;
                        Label1.Visible = true;
                    }
                    catch { servercontent.InnerHtml = "Warning:StudentInformation_TimeTableDisplayDate invlaid</br."; }
                }  
                DisplayTT(t1);
			}
		}

        protected void DisplayTT(DateTime t1)
        {
            studentID = "";
            Utility u1 = new Utility();
            PersonID = u1.GetPersonIdfromRequest(Request);

#if DEBUG
            u1.Is_student = true;u1.Is_staff = false;
#endif
            
            if ((PersonID != Guid.Empty) && (u1.Is_student))
                {
                    studentID = u1.GetStudentId(PersonID).ToString();
                    PupilPeriodList ppl = new PupilPeriodList();          
                    ppl.LoadList("StudentId", studentID, true,t1);
                    TT_writer ttw = new TT_writer();
                    servercontent.InnerHtml += "<h2>Timetable for " + u1.GetPersonName(PersonID) + " as at "+t1.ToShortDateString()+"</h2>";
                    servercontent.InnerHtml += ttw.OutputTT_string("", false, TT_writer.TimetableType.Student, ref ppl, t1)+"<br/>";
                }
                else
                {
                    Response.Write("");
                    servercontent.InnerHtml = "<h2>No data to display</h2>";
                }

        }

		private void RenderHeader(object sender, System.EventArgs e)
		{
            
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

        protected void Button1_Click(object sender, EventArgs e)
        {
            string s = "";
            DateTime t1 = new DateTime(2011, 9, 8);
            t1 = DateTime.Now; servercontent.InnerHtml = "";
            Cerval_Configurations config = new Cerval_Configurations();
            config.Find_Key("StudentInformation_TimeTableDisplayDate", out s);
            if (s != "")
            {
                try { t1 = System.Convert.ToDateTime(s); }
                catch { servercontent.InnerHtml = "Warning:StudentInformation_TimeTableDisplayDate invlaid</br."; }
            }  

            if(Button1.Text.Contains("current"))
            {
                DisplayTT(DateTime.Now); 
                Button1.Text = "Switch to forward Date ("+t1.ToShortDateString()+")"; 
                return;
            }
            if(Button1.Text.Contains("forward"))
            {
                DisplayTT(t1);
                Button1.Text = "Switch to current Date";
            }

        }
	}
}
