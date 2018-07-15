using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.Exams
{
    public partial class iSAMSPullBaseData : System.Web.UI.Page
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
                Label_Banner.Text = "DCGS Exams : Year = " + (string)(Session["Year"]) + ", Season =" + (string)(Session["Season"]) + "     Transferring BaseData to Server.";
            }
        }

        protected void Button_PullBaseData_Click(object sender, EventArgs e)
        {
            // so first we are going to get a list of all options used...

            int c = 0;
            c = System.Convert.ToInt32(TextBox_CycleNumber.Text);
            ISAMS_OptionsUsed_List list1 = new ISAMS_OptionsUsed_List();
            list1.LoadList(c);

            string s ="";
            foreach (ISAMS_OptionUsed o in list1.m_list)
            {
                s += o.m_OptionCode;

                //need the Cerval ExamBde Guid
                Exam_Board CExamBoard = new Exam_Board(o.m_UABCode);

                // so load option / link /components
                ISAMS_ExamOption Opt = new ISAMS_ExamOption();
                Opt.Load(c, o.m_OptionCode, o.m_UABCode);
                s +="  :  "+ Opt.m_OptionTitle;
                ISAMS_ExamSyllabus Syl = new ISAMS_ExamSyllabus();
                Syl.Load(Opt.m_cycle, Opt.m_ExamBoardCode, Opt.m_Syllabus_Code);
                ISAMS_ExamLink_List Link = new ISAMS_ExamLink_List();
                Link.LoadList_Option(Opt.m_OptionCode, Opt.m_ExamBoardCode, Opt.m_cycle);

                //so try to find the Ceral Syllabus
                ExamSyllabus CSyllabus = new ExamSyllabus();
                CSyllabus.Load(Syl.m_SyllabusCode, SeasonCode.ToString(), YearCode.ToString(), CExamBoard.m_ExamBoardId);
                if (!CSyllabus.m_valid)
                {
                    CSyllabus.m_ExamBoardId = CExamBoard.m_ExamBoardId;
                    CSyllabus.m_Syllabus_Code = Syl.m_SyllabusCode;
                    CSyllabus.m_Syllabus_Title = Syl.m_SyllabusTitle;

                    CSyllabus.m_SyllabusId = CSyllabus.CreateNew("22");
                }

                //now try to find the option

                ExamOption COption = new ExamOption();
                COption.Load(Opt.m_OptionCode, SeasonCode.ToString(), YearCode.ToString(), Opt.m_ExamBoardCode);
                if (COption.m_valid)
                {
                    //option found...
                }
                else
                {
                    //option not found need to create...
                    COption.m_SyllabusID = CSyllabus.m_SyllabusId;
                    COption.m_OptionCode = Opt.m_OptionCode;
                    COption.m_OptionTitle = Opt.m_OptionTitle;
                    COption.m_OptionQualification = Opt.m_OptionQualification;
                    COption.m_OptionLevel = Opt.m_OptionLevel;
                    COption.m_Item = Opt.m_Item;
                    COption.m_Process = Opt.m_Process;
                    COption.m_QCACode = Opt.m_QCACode;
                    COption.m_QCANumber = Opt.m_QCANumber;
                    COption.m_fee = Opt.m_fee;
                    COption.m_OptionMaximumMark = Opt.m_OptionMaximumMark;
                    COption.m_year_Code = YearCode.ToString();
                    COption.m_Season_code = SeasonCode.ToString();
                    COption.m_SeriesIdentifier = Opt.m_SeriesIdentifier;

                    COption.m_OptionID = COption.CreateNew("22");
                }


                foreach (ISAMS_ExamLink el1 in Link.m_list)
                {
                    ISAMS_ExamComponent  ec1 = new ISAMS_ExamComponent();
                    ec1.Load(el1.m_ComponentCode,Opt.m_cycle,Opt.m_ExamBoardCode);
                    s += "  &  " + ec1.m_ComponentTitle;
                    //so should be able to insert into Cerval



                    //OK so now the components...
                    ExamComponent CComp = new ExamComponent();
                    CComp.Load(ec1.m_ComponentCode, SeasonCode.ToString(), YearCode.ToString());
                    if (!CComp.m_valid)
                    {
                        //so need to create
                        CComp.m_ComponentCode = ec1.m_ComponentCode;
                        CComp.m_ComponentTitle = ec1.m_ComponentTitle;
                        CComp.m_ExamBoardID = CExamBoard.m_ExamBoardId;
                        CComp.m_year = YearCode.ToString();
                        CComp.m_season = SeasonCode.ToString();
                        CComp.m_Teachermarks = ec1.m_Teachermarks;
                        CComp.m_MaximumMark = ec1.m_MaximumMark;
                        CComp.m_Due_Date = ec1.m_Due_Date;
                        CComp.m_Timetabled = ec1.m_Timetabled;

                        CComp.m_TimetableSession = ec1.m_TimetableSession;
                        CComp.m_Time = ec1.m_TimeAllowed;
                        DateTime t1 = new DateTime();t1 = System.Convert.ToDateTime(ec1.m_TimetableDate);
                        CComp.m_TimetableDate = t1;

                        CComp.m_ComponentID= CComp.CreateNew();
                    }

                    ExamLinkComponent_List l2 = new ExamLinkComponent_List();
                    l2.LoadList_OptionComponent(COption.m_OptionID, CComp.m_ComponentID);
                    if (l2.m_list.Count == 0)
                    {
                        //need to make the link
                        ExamLinkComponent Clink = new ExamLinkComponent();
                        Clink.m_OptionId = COption.m_OptionID;
                        Clink.m_ComponentId = CComp.m_ComponentID;
                        Clink.CreateNew("22");
                    }


                }
                s+= Environment.NewLine;
            }
            TextBox_test.Text = s;
            
        }

        protected void Button_PullEntries_Click(object sender, EventArgs e)
        {
            ISAMS_SimpleExamEntry_List IEntries = new ISAMS_SimpleExamEntry_List();

            int c = 0;
            c = System.Convert.ToInt32(TextBox_CycleNumber.Text);
            IEntries.LoadList(c);

            foreach (ISAMS_SimpleExamEntry Ien in IEntries.m_list)
            {
                //need to find Guids for Pupil, OPtion etc...
                Utility u = new Utility();
                Guid SId = new Guid();
                SId = u.GetStudentIDfromiSAMS(Ien.m_PupilId);
                Exam_Board eb1 = new Exam_Board(Ien.m_UABCode);
                ExamOption eo1 = new ExamOption();
                eo1.Load(Ien.m_OptionCode,SeasonCode.ToString(),YearCode.ToString(), Ien.m_UABCode.ToString());

                Exam_Entry Ex1 = new Exam_Entry();
                Ex1.Load(Ien.m_OptionCode, SeasonCode.ToString(), YearCode.ToString(),eb1.m_ExamBoardId, SId);
                if (!Ex1.m_valid)
                {
                    Ex1.m_Date_Created = DateTime.Now;
                    Ex1.m_OptionID = eo1.m_OptionID;
                    Ex1.m_season = SeasonCode.ToString();
                    Ex1.m_StudentID = SId;
                    Ex1.m_withdrawn = false;
                    Ex1.m_year = Year.ToString();
                    Ex1.m_EntryStatus = 7;

                    Ex1.Save();
                }
                else
                {
                    bool f = Ex1.m_valid;
                }






            }
        }
    }
}