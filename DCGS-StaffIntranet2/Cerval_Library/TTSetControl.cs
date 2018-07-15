using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cerval_Library
{
    [Serializable]
    public partial class TTSetControl : Control
    {
        public TTData.TT_period m_period;
        public Color m_color;
        public Color m_DropColor;
        

        public TTSetControl(TTData.TT_period period)
        {
            m_period = period;
            m_color = System.Drawing.Color.Gray;
            m_DropColor = System.Drawing.Color.BlueViolet;
            AllowDrop = false;
            DragDrop += new DragEventHandler(TTSetControl_DragDrop);
            DragEnter += new DragEventHandler(TTSetControl_DragEnter);
            InitializeComponent();
        }



        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics; string s = "";
            Font drawFont = new Font("Arial", 8);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            float x = 0; float y = 0; StringFormat drawFormat = new StringFormat();
            Pen p = new Pen(System.Drawing.Color.DarkGray, 3.0F);
            g.DrawRectangle(p, 0, 0, Width-1, Height-1);
            g.FillRectangle(new SolidBrush(m_color), 0, 0, Width - 1, Height - 1);
            g.DrawString(m_period.SetName, drawFont, drawBrush, x, y, drawFormat);
            s = m_period.StaffCode;
            if (m_period.Covered_StaffCode != "") s += "(" + m_period.Covered_StaffCode + ")";
            g.DrawString(s, drawFont, drawBrush, x, y + Height / 3, drawFormat);
            g.DrawString(m_period.RoomCode, drawFont, drawBrush, x, y + 2*Height / 3, drawFormat);
            base.OnPaint(pe);
        }

        void TTSetControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        void TTSetControl_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                TTSetControl tt1 = (TTSetControl)e.Data.GetData(DataFormats.Serializable);
                m_color = m_DropColor;
                m_period.Covered_StaffCode = m_period.StaffCode;
                m_period.Covered_StaffId = m_period.StaffId;
                m_period.StaffCode = tt1.m_period.StaffCode;
                m_period.StaffId = tt1.m_period.StaffId;
                AllowDrop = false;
                Invalidate();
            }
            catch
            {

            }

        }

    }


    public partial class GCSEResultsGrid : Control
    {
        private ResultsList rl1 = new ResultsList();
        public List<Result> Results=  new List<Result>();
        private List<Result> ResultsHalf = new List<Result>();
        private List<Result> ResultsDouble = new List<Result>();
        private List<int> resultsvalue = new List<int>();
        public string[,] GCSEsubs = new string[6, 20];// [0,n] for A* [1,n] for A etc
        private Guid m_studentID;

        public Guid StudentID { get { return (m_studentID); } set { m_studentID = value; } }

        public GCSEResultsGrid()
        {
        }

        public int Load() //returns points score from best 8
        {
            rl1._results.Clear(); Results.Clear(); ResultsDouble.Clear(); ResultsHalf.Clear();
            resultsvalue.Clear();
            rl1.LoadList("dbo.tbl_Core_Students.StudentId", m_studentID.ToString());
            foreach (Result r in rl1._results)
            {
                if ((r.Resulttype == 10)||(r.Resulttype == 35))
                {
                    Results.Add(r);//if gcse
                    switch (r.Value.Trim())
                    {
                        case "A*": resultsvalue.Add(58); break;
                        case "A": resultsvalue.Add(52); break;
                        case "B": resultsvalue.Add(46); break;
                        case "C": resultsvalue.Add(40); break;
                        case "D": resultsvalue.Add(34); break;
                        case "E": resultsvalue.Add(28); break;
                        default: break;
                    }
                }
                if (r.Resulttype == 13)
                {
                    ResultsHalf.Add(r);//if half gcse
                    switch (r.Value.Trim())
                    {
                        case "A*": resultsvalue.Add(29); break;
                        case "A": resultsvalue.Add(26); break;
                        case "B": resultsvalue.Add(23); break;
                        case "C": resultsvalue.Add(20); break;
                        case "D": resultsvalue.Add(17); break;
                        case "E": resultsvalue.Add(14); break;
                        default: break;
                    }
                }
                if (r.Resulttype == 24)
                {
                    ResultsDouble.Add(r);//if double gcse  only count first grade
                    switch (r.Value.Substring(0,1))
                    {
                        case "*": resultsvalue.Add(58); break;
                        case "A": resultsvalue.Add(52);  break;
                        case "B": resultsvalue.Add(46);  break;
                        case "C": resultsvalue.Add(40); break;
                        case "D": resultsvalue.Add(34);  break;
                        case "E": resultsvalue.Add(28);  break;
                        default: break;
                    }

                    switch (r.Value.Substring(1, 1))
                    {
                        case "*": resultsvalue.Add(58); break;
                        case "A": resultsvalue.Add(52); break;
                        case "B": resultsvalue.Add(46); break;
                        case "C": resultsvalue.Add(40); break;
                        case "D": resultsvalue.Add(34); break;
                        case "E": resultsvalue.Add(28); break;
                        default: break;
                    }




                }
            }

            resultsvalue.Sort();//lowest to highest...
            int i = resultsvalue.Count-1; int v = 0;
            for (int j = 8; j > 0; j--)
            {
                if (i >= 0)
                {
                    v += resultsvalue[i]; i--;
                }
            }
            return v;
        }


        public GCSEResultsGrid(Guid StudentID)
        {
            m_studentID = StudentID;
            rl1.LoadList("StudentId", m_studentID.ToString());
            foreach (Result r in rl1._results)
            {
                if (r.Resulttype == 10)
                {
                    Results.Add(r);//if gcse
                }
                if (r.Resulttype == 13) ResultsHalf.Add(r);//if half gcse
                if (r.Resulttype == 24) ResultsDouble.Add(r);//if double gcse
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics; string s = "";
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            float x = 0; float y = 0; StringFormat drawFormat = new StringFormat();
            Pen p = new Pen(System.Drawing.Color.Blue, 1.5F);
            //draw grid
            float xs = (Width-10) / 6; float ys = Height - 10;
            for(int i=0; i<7;i++)g.DrawLine(p,xs*i+5,5,xs*i+5,ys-5);
            g.DrawLine(p, 5, 5, Width - 7, 5);
            g.DrawLine(p, 5, ys-5, Width - 7, ys-5);
            x = 5; y = 5;
            g.DrawString("A*", drawFont, drawBrush, x, y); x += xs;
            g.DrawString("A", drawFont, drawBrush, x, y); x += xs;
            g.DrawString("B", drawFont, drawBrush, x, y); x += xs;
            g.DrawString("C", drawFont, drawBrush, x, y); x += xs;
            g.DrawString("D", drawFont, drawBrush, x, y); x += xs;
            g.DrawString("E/u", drawFont, drawBrush, x, y);
            ys=5+20;
            int ns = 2, na = 2, nb = 2, nc = 2, nd = 2, ne = 2;
            x = 0; y = 0; xs = (Width - 10) / 6;
            g.DrawLine(p, 5, ys, Width - 7, ys);
            foreach (Result r in Results)
            {
                s = r.Code.Trim();
                if (s == "SD")
                {
                    if (r.OptionTitle.Trim().Length > 4)
                    {
                        s = r.OptionTitle.Trim(); s = s.Substring(0, 2);
                    }
                }
                switch (r.Value.Trim())
                {
                    case "A*": x = 5; y = ns * 20; ns++; GCSEsubs[0, ns-3] = s; break;
                    case "A": x = xs + 5; y = na * 20; na++; GCSEsubs[1, na-3] = s; break;
                    case "B": x = 2 * xs + 5; y = nb * 20; nb++; GCSEsubs[2, nb-3] = s; break;
                    case "C": x = 3 * xs + 5; y = nc * 20; nc++; GCSEsubs[3, nc-3] = s; break;
                    case "D": x = 4 * xs + 5; y = nd * 20; nd++; GCSEsubs[4, nd-3] = s; break;
                    default: x = 5 * xs + 5; y = ne * 20; ne++; GCSEsubs[5, ne-3] = s; break;
                }
                if (x > 0)
                {

                    g.DrawString(s, drawFont, drawBrush, x, y);
                }
            }

            foreach (Result r in ResultsHalf)
            {

                int n=0;
                n=(ns>n)?ns:n;
                n=(na>n)?na:n;
                n=(nb>n)?nb:n;
                n=(nc>n)?nc:n;
                n=(nd>n)?nd:n;
                n=(ne>n)?ne:n;

                y = n * 20;
                //if we have any... draw a line..
                g.DrawLine(p, 5, y, Width - 7, y);
                s = "Half Subjects";
                g.DrawString(s, drawFont, drawBrush, 5, y); y += 20;
                g.DrawLine(p, 5, y, Width - 7, y);
                n++;
                ns = n; na = n; nb = n; nc = n; nd = n; ne = n;
                break;
            }

            foreach (Result r in ResultsHalf)
            {
                s = r.Code.Trim();
                if (s == "SD")
                {
                    if (r.OptionTitle.Trim().Length > 4)
                    {
                        s = r.OptionTitle.Trim(); s = s.Substring(0, 2);
                    }
                }
                switch (r.Value.Trim())
                {
                    case "A*": x = xs + 5; y = ns * 20; ns++; break;
                    case "A": x = 2 * xs + 5; y = na * 20; na++; break;
                    case "B": x = 3 * xs + 5; y = nb * 20; nb++; break;
                    case "C": x = 4 * xs + 5; y = nc * 20; nc++; break;
                    case "D": x = 5 * xs + 5; y = nd * 20; nd++; break;
                    default: x = 6 * xs + 5; y = ne * 20; ne++; break;
                }
                if (x > 0)
                {
                    g.DrawString(s, drawFont, drawBrush, x, y);
                }
            }


            foreach (Result r in ResultsDouble)
            {
                int n = 0;
                n = (ns > n) ? ns : n;
                n = (na > n) ? na : n;
                n = (nb > n) ? nb : n;
                n = (nc > n) ? nc : n;
                n = (nd > n) ? nd : n;
                n = (ne > n) ? ne : n;

                y = n * 20;
                //if we have any... draw a line..
                g.DrawLine(p, 5, y, Width - 7, y);
                s = "Double Award Subjects";
                g.DrawString(s, drawFont, drawBrush, 5, y); y += 20;
                g.DrawLine(p, 5, y, Width - 7, y);
                n++;
                ns = n; na = n; nb = n; nc = n; nd = n; ne = n;
                break;
            }

            foreach (Result r in ResultsDouble)
            {
                s = r.Code.Trim();
                if (s == "SD")
                {
                    if (r.OptionTitle.Trim().Length > 4)
                    {
                        s = r.OptionTitle.Trim(); s = s.Substring(0, 2);
                    }
                }
                switch (r.Value.Substring(0,1))
                {
                    case "*": x =  5; y = ns * 20; ns++; break;
                    case "A": x = xs + 5; y = na * 20; na++; break;
                    case "B": x = 2 * xs + 5; y = nb * 20; nb++; break;
                    case "C": x = 3 * xs + 5; y = nc * 20; nc++; break;
                    case "D": x = 4 * xs + 5; y = nd * 20; nd++; break;
                    default: x = 5 * xs + 5; y = ne * 20; ne++; break;
                }
                if (x > 0)
                {
                    g.DrawString(s, drawFont, drawBrush, x, y);
                }
            }

            base.OnPaint(pe);
        }
    }
}
