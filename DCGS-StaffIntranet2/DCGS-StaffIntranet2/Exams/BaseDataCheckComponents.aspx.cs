using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class BaseDataCheckComponents : System.Web.UI.Page
    {

        #region variables
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Uploading from Text File";
            }
        }


        protected bool CheckOptions(Exam_Board eb)
        {
            ExamFiles ef = new Cerval_Library.ExamFiles();
            ExamOption_List examOl1 = new ExamOption_List();
            examOl1.Load(eb.m_ExamBoardId.ToString(), YearCode.ToString(), SeasonCode.ToString());
            System.Collections.ArrayList ExamOptionsList = new System.Collections.ArrayList();
            ExamOptionsList = ef.ExamsOptionsFromBaseData(eb, SeasonCode.ToString(),YearCode.ToString());
            int m1 = 0; int m2 = 0;
            foreach (ExamOption examo1 in examOl1.m_list)
            {
                foreach (ExamBaseOption eb1 in ExamOptionsList)
                {
                    if (eb1.m_OptionEntryCode == examo1.m_OptionCode)
                    {
                        m1 = 0; m2 = 0;
                        try { m1 = System.Convert.ToInt32(eb1.m_MaximiumMark); }
                        catch { }
                        try { m2 = System.Convert.ToInt32(examo1.m_OptionMaximumMark); }
                        catch { }
                        try
                        {
                            if (m1 != m2)
                            {
                                examo1.m_OptionMaximumMark = eb1.m_MaximiumMark;
                                examo1.UpdateMaxMark();
                            }
                            //ought to check we have all the components for this...
                            string component_file = ef.path+"c" + eb1.m_file_path.Substring(1, eb1.m_file_path.Length - 1);
                            string link_file = ef.path+ "l" + eb1.m_file_path.Substring(1, eb1.m_file_path.Length - 1);
                            ef.Install_Components(examo1.m_OptionCode, link_file, component_file, eb, SeasonCode.ToString()  , YearCode.ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return true;
        }

        protected bool CheckComponents(Exam_Board eb)
        {
            ExamFiles ef = new Cerval_Library.ExamFiles();
            List<ExamComponent> BaseComponentList = new List<ExamComponent>();
            ExamCompononent_List ecl1 = new ExamCompononent_List();
            ecl1.LoadAllComponents_IncNonTimetabled(YearCode.ToString(), SeasonCode.ToString());
            BaseComponentList = ef.ExamsComponentsFromBaseData(eb, SeasonCode.ToString(), YearCode.ToString());
            foreach (ExamComponent c in ecl1.m_list)
            {
                foreach (ExamComponent c1 in BaseComponentList)
                {
                    if (c.m_ComponentCode == c1.m_ComponentCode)
                    {
                        c.m_Teachermarks = c1.m_Teachermarks;
                        c.m_Due_Date = c1.m_Due_Date;
                        c.m_ExamBoardID = c1.m_ExamBoardID;
                        c.m_MaximumMark = c1.m_MaximumMark;
                        c.m_Teachermarks = c1.m_Teachermarks;
                        c.m_ComponentTitle = c1.m_ComponentTitle;
                        c.m_Timetabled = c1.m_Timetabled;
                        c.m_TimetableSession = c1.m_TimetableSession;
                        c.m_Time = c1.m_Time;
                        c.m_TimetableDate = c1.m_TimetableDate;
                        c.Save();
                    }
                }
            }
            return true; ;
        }

        protected void Button_Check_Click(object sender, EventArgs e)
        {
            //going to check components
            Label_Message.Text = "This just checks the time allocations for all Components for an exam board against ";
            Label_Message.Text+=" the base data and corrects if wrong! (they shouldn't be)";

            ExamBoardList eblist1 = new ExamBoardList();
            foreach (Exam_Board eb in eblist1._ExamBoardList)
            {
                CheckOptions(eb);
                CheckComponents(eb);
            }
           

        }
    }
}