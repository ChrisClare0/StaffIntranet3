using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using Cerval_Library;
using System.Drawing;

namespace DCGS_Staff_Intranet2.Xmatrix
{
    public partial class chartForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                VAResultsList va1 = new VAResultsList();
                va1.LoadCourse(new Guid("cf07a783-1a83-49cd-af92-4623c88c15eb"));

                //va1.LoadGroup(new Guid("cf07a783-1a83-49cd-af92-4623c88c15eb"), new Guid("1904046f-4532-4a82-9091-1ce43c2c6489"));
                CourseList cl1 = new CourseList(0);
                GroupList gl1 = new GroupList();gl1.LoadList(new DateTime(2018, 4, 1), GroupList.GroupListOrder.GroupName);
                Chart8.Titles.Add("A-level VA from Alis model");
                Chart8.Titles[0].Font = new Font("Verdana", 16, System.Drawing.FontStyle.Bold);
                Chart8.ChartAreas["ChartArea1"].AxisX.Interval = 1;
                LabelStyle ls1 = new LabelStyle();
                ls1.Font = new Font("Verdana", 16, System.Drawing.FontStyle.Bold); ls1.Angle = 90;
                Chart8.ChartAreas["ChartArea1"].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None;
                // Chart8.ChartAreas["ChartArea1"].AxisX.TextOrientation = TextOrientation.Rotated90;
                ls1.ForeColor = Color.Red;
                Chart8.ChartAreas["ChartArea1"].AxisX.LabelStyle = ls1;

                Series s1 = new Series("test");
                s1.XValueType = ChartValueType.String;
                s1.ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;
                s1.YValueType = new ChartValueType();
                s1.YValueType = System.Web.UI.DataVisualization.Charting.ChartValueType.Int32;
                Chart8.Series.Add(s1);
                Chart8.Series["test"]["PixelPointWidth"] = "40";
                Chart8.Series["test"].IsXValueIndexed = true;

                Chart8.Series["test"].Font = new Font("Arial", 12, System.Drawing.FontStyle.Bold);
                int i = 0;
                foreach (ValueAddedResult r in va1.m_list)
                {
                    if (r.DateOutpuResult.Year == 2018)
                    {
                        string s = "X";
                        //find course
                        foreach (Course c in cl1._courses)
                       {
                          if (c._CourseID == r.CourseId) { s = c.CourseName; break; }
                        }

                        //fid Group
                        //foreach(Group g in gl1._groups)
                        //{
                        //    if (g._GroupID == r.GroupId) { s = g._GroupName; break; }

                      //  }
                        DataPoint d = new DataPoint();

                        d.AxisLabel = s;
                        d.SetValueY(r.OutputResultValue - r.VAResultValue);
                        d.Label = "#VALY";
                        s1.Points.Add(d); 
                    }
                    i++;

                }


                // s1.MarkerSize = 30;
                // s1.MarkerStyle = System.Web.UI.DataVisualization.Charting.MarkerStyle.Cross;
                // s1.BorderWidth = 5;
            }
        }
    }
}