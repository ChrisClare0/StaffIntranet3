using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class StudentSENStrategies : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string s = ""; bool edit = false; string s1 = "";
                try
                {
                    Utility u = new Utility();
                    edit = u.CheckStaffInConfigGroup(Context, "SEN-MANAGERS");
                }
                catch (Exception e1)
                {
                    s = e1.Message;
                }

                string type = Request.QueryString["Type"];
                string studentID = Request.QueryString["Id"];
                string studentFullName = Request.QueryString["Name"];
                string photo = Request.QueryString["Photo"];
                string senId = Request.QueryString["StudentSENId"];
                ViewState.Add("senId", senId);
                StudentSEN sen1 = new StudentSEN(); sen1.Load(senId);
                s = "<p align=\"center\">" + studentFullName;
                s += "<br>Special Needs Strategies</p>";
                s += "<p align=\"center\"> <img src=\"" + photo + "\" width = \"110\" height=\"140\"></p>";
                string NewString = "../content/StudentSENStrategies.aspx?Type=New&Id=" + studentID + "&Name=" + studentFullName + "&Photo=" + photo+"&StudentSENId=" + sen1.m_SENid.ToString();;
                ViewState.Add("NewString", NewString);
                if (type == "Display")
                {

                    StudentSENList senlist1 = new StudentSENList(studentID);
                    SENTypeList sentypes1 = new SENTypeList();
                    foreach (SENType sent1 in sentypes1._List)
                    {
                        if (sen1.m_SenType == sent1.id) Response.Write(sent1.SENtype + " :   ");
                    }
                    s += sen1.m_SenDescription + "<br><br>";
                    if (sen1.m_strategies.m_List.Count > 0)
                    {
                        foreach (StudentSENStrategy senst1 in sen1.m_strategies.m_List)
                        {
                            if (edit)
                            {
                                s1 = "../content/StudentSENStrategies.aspx?Type=Edit&";
                                s1 += "&Id=" + studentID.ToString() + "&Name=" + studentFullName;
                                s1 += "&Photo=" + photo + "&StudentSENId=" + sen1.m_SENid.ToString();
                                s1 += "&StudentSENStrategy=" + senst1.Id.ToString();
                                s += "<A HREF=\"" + s1 + "\">Strategy: </A>";
                            }
                            else
                            {
                                s += "Strategy: ";
                            }
                            s += senst1.Strategy_Value + "<br>";
                        }
                    }
                    content0.InnerHtml = s;
                }
                if (type == "Edit")
                {
                    s += "<br>";
                    content0.InnerHtml = s;
                    TextBox1.Visible = true; Button_Save.Visible = true; 
                    CreateNewButton.Visible = false; Button_Cancel.Visible = true;
                    Button_Delete.Visible = true;
                    string StudentSENStrategyId = Request.QueryString["StudentSENStrategy"];
                    ViewState.Add("StudentSENStrategyId", StudentSENStrategyId);
                    StudentSENStrategy sss1 = new StudentSENStrategy(); sss1.Load(StudentSENStrategyId);
                    TextBox1.Text = sss1.Strategy_Value;
                    string CancelString = "../content/StudentSENStrategies.aspx?Type=Display&Id=" +studentID+ "&Name=" + studentFullName+"&Photo="+photo+"&StudentSENId=" + sen1.m_SENid.ToString();
                    ViewState.Add("Cancel_String", CancelString);
                }
                if (type == "New")
                {
                    s += "<br>";
                    content0.InnerHtml = s;
                    TextBox1.Visible = true; Button_Save.Visible = true;
                    CreateNewButton.Visible = false; Button_Cancel.Visible = true;
                    Button_Delete.Visible = false;
                    TextBox1.Text = "";
                    string CancelString = "../content/StudentSENStrategies.aspx?Type=Display&Id=" + studentID + "&Name=" + studentFullName + "&Photo=" + photo + "&StudentSENId=" + sen1.m_SENid.ToString();
                    ViewState.Add("Cancel_String", CancelString);
                }
            }
        }
        protected void CreateNewButton_Click(object sender, EventArgs e)
        {
            Server.Transfer((string)ViewState["NewString"]);
        }
        protected void Button_Save_Click(object sender, EventArgs e)
        {
            string StudentSENStrategyId = (string)ViewState["StudentSENStrategyId"];
            StudentSENStrategy sss1 = new StudentSENStrategy(); 
            if (StudentSENStrategyId != null)
            {
                //edit
                sss1.Load(StudentSENStrategyId);
                sss1.Strategy_Value = TextBox1.Text;
                sss1.Update();
                Server.Transfer((string)ViewState["Cancel_String"]);
            }
            else
            {
                //new
                string senId = (string)ViewState["senId"];
                sss1.Strategy_Value = TextBox1.Text;
                sss1.Create(senId);
                Server.Transfer((string)ViewState["Cancel_String"]);
            }
        }
        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            Server.Transfer((string)ViewState["Cancel_String"]);
        }
        protected void Button_Delete_Click(object sender, EventArgs e)
        {
            string StudentSENStrategyId = (string)ViewState["StudentSENStrategyId"];
            StudentSENStrategy sss1 = new StudentSENStrategy(); sss1.Load(StudentSENStrategyId);
            sss1.Strategy_Value = TextBox1.Text;
            sss1.Delete();
            Server.Transfer((string)ViewState["Cancel_String"]);
        }
    }
}
