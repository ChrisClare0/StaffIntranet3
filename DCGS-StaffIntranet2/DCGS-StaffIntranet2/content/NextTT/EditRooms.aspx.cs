using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content
{
    public partial class EditRooms : System.Web.UI.Page
    {
        static int max_rows = 30;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Utility u = new Utility();
                string staff_code = u.GetsStaffCodefromRequest(Request);
                string file_name = Server.MapPath(@"~/App_Data/tt_data.ttd");

                Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_TTD_TimetablePlanDate");
                DateTime date1 = new DateTime();
                try { date1 = System.Convert.ToDateTime(c.Value); }
                catch { date1 = DateTime.Now; }

                TTData tt1 = new TTData();
                bool UseTTD = true;
                c = new Cerval_Configuration("StaffIntranet_UseOldTTDfile");
                if (c.Value.ToUpper() == "TRUE")
                {
                    tt1.Load(file_name, date1, date1, false, false, "");
                }
                else
                {
                    tt1.Load_DB(date1); //load from database
                    UseTTD = false;
                }
                //string[] rooms = new string[max_rows];//room names
                //string[] rooms_t = new string[max_rows];//room names
                List<string> rooms = new List<string>();
                List<string> subjects = new List<string>();
                int no_rooms = 0; int no_rows = 0;
                //string[] subjects = new string[10];
                int no_subjects = 0;
                string[,] data = new string[30, max_rows]; //has the display data
                long[,] pointers = new long[30, max_rows];//has the file ppointers for TTD
                Guid[,] ScheduledPeriodIds = new Guid[30, max_rows];//has period ids for db use


                string[] setupS = new string[20];
                string[] staffS = new string[20];
                string[] subS = new string[20];

                char[] c1 = new char[1]; c1[0] = ',';
                char[] c2= new char[1]; c2[0] = ';';
                char[] c3 = new char[1]; c3[0] = ':';
                staff_code = staff_code.Trim().ToUpper();
                no_rooms = 0; bool found = false;
    
                
                c = new Cerval_Configuration("StaffIntranet_TTD_EditRoomsList");
                //list has staffcode;room1,room2,room3;subject1,sub2:
                string[] initialS = new string[20];
                string[] temp1 = new string[3]; 
                initialS = c.Value.Split(c3);
                for (int i = 0; i < 9; i++)
                {
                    try
                    {
                        temp1 = initialS[i].Split(c2);
                        setupS[i] = temp1[1];
                        staffS[i] = temp1[0];
                        subS[i] = temp1[2];
                    }
                    catch { };
                }


                /*
                setupS[0] = "P1,P2,P3,P4,P5"; staffS[0] = "LBI"; subS[0] = "PH,ET";
                setupS[1] = "P1,P2,P3,P4,P5,B1,B2,B3,B4,Q9,C1,C2,C3"; staffS[1] = "ABR"; subS[1] = "BI,PH,CH";
                setupS[2] = "C1,C2,C3"; staffS[2] = "JHO"; subS[2] = "CH";
                setupS[3] = "21,22,23,24,31,32,33,34,41"; staffS[3] = "CH"; subS[3] = "MA";
                setupS[4] = "N11,N12,N13,N14"; staffS[4] = "AAB"; subS[4] = "GE";
                setupS[5] = "11,12,13,14,L4,L5,Q6"; staffS[5] = "EWA"; subS[5] = "FR,GM,SP";
                setupS[6] = "N1,N2,N3,N4,Q7,Q8,Q13,ST11"; staffS[6] = "FRO"; subS[6] = "EN,EL,TH";
                setupS[7] = "P1,P2,P3,P4,P5,B1,B2,B3,B4,Q9,C1,C2,C3"; staffS[7] = "JA"; subS[7] = "BI,CH,PH";//testing
                setupS[8] = "W12,W13,W1,W2,W3,W11,N11,N12,N13,N14,A8,A9"; staffS[8] = "GWS"; subS[8] = "RS,GE,HI";
                setupS[9] = "P1"; staffS[9] = "CC"; subS[9] = "RS,GE,HI";//testing
                //find the staff
                */
  
                staff_code = staff_code.Trim().ToUpper();
                for (int i = 0; i < 10; i++)
                {
                    if (staffS[i]==staff_code)
                    {
                        string [] rooms1 = setupS[i].Split(c1); found = true;
                        foreach (string s1 in rooms1) rooms.Add(s1);
                        string[] subject1 = subS[i].Split(c1);
                        foreach (string s1 in subject1) subjects.Add(s1);
                        break;
                    }
                }
                if (!found)
                {
                    return;
                }

                no_rooms = rooms.Count; no_rows = no_rooms;
                no_subjects = subjects.Count;
                foreach (string s1 in rooms)
                {
                    ListItem l = new ListItem(s1, rooms.IndexOf(s1).ToString());
                    DropDownList_room1.Items.Add(l);
                    DropDownList_room2.Items.Add(l);
                }
                int p1 = 0;
                foreach (TTData.TT_period t in tt1.periodlist1.m_list)
                {
                    if (rooms.Contains(t.RoomCode.Trim().ToUpper()))
                    {
                        int r = rooms.IndexOf(t.RoomCode.Trim().ToUpper());
                        try
                        {
                            p1 = System.Convert.ToInt16(t.PeriodCode);
                        }
                        catch
                        {
                            p1 = 0;//reg periods etc
                        }
                        if (p1 > 0)
                        {
                            data[t.DayNo * 5 + p1, r] = t.SetName + "<br>" + t.StaffCode;
                            pointers[t.DayNo * 5 + p1, r] = t.File_Position;// only one of these lines works!
                            ScheduledPeriodIds[t.DayNo * 5 + p1, r] = t.ScheduledPeriodId;
                        }
                    }
                    else
                    {
                        foreach (string s1 in subjects)
                        {
                            if (t.SetName.Trim().ToUpper().Contains(s1))
                            {
                                try
                                {
                                    p1 = System.Convert.ToInt16(t.PeriodCode);
                                }
                                catch
                                {
                                    p1 = 0;//reg periods etc
                                }
                                if (p1 > 0)
                                {
                                    // now to add room to the drop downs....
                                    ListItem l = DropDownList_room1.Items.FindByText(t.RoomCode.Trim().ToUpper());
                                    if (l == null)
                                    {
                                        //need to add it...
                                        l = new ListItem(t.RoomCode.Trim().ToUpper(), no_rooms.ToString());
                                        DropDownList_room1.Items.Add(l);
                                        DropDownList_room2.Items.Add(l);

                                        data[t.DayNo * 5 + p1, no_rooms] = t.SetName + "<br>" + t.StaffCode;
                                        pointers[t.DayNo * 5 + p1, no_rooms] = t.File_Position;
                                        ScheduledPeriodIds[t.DayNo * 5 + p1, no_rooms] = t.ScheduledPeriodId;
                                        rooms.Add(t.RoomCode.Trim().ToUpper());
                                        no_rooms++;
                                    }
                                    else
                                    {
                                        int r1 = DropDownList_room1.Items.IndexOf(l);
                                        data[t.DayNo * 5 + p1, r1] = t.SetName + "<br>" + t.StaffCode;
                                        pointers[t.DayNo * 5 + p1, r1] = t.File_Position;
                                        ScheduledPeriodIds[t.DayNo * 5 + p1, r1] = t.ScheduledPeriodId;
                                    }

                                }
                            }
                        }
                    }

                }

                string[] rooms_t = new string[no_rooms];
                rooms_t = rooms.ToArray();
                content0.InnerHtml = GenerateTimetable(data, rooms_t, no_rooms);

                //to serialise we need to put data /pointers into 1D arrays...
                string[] data1 = new string[30*max_rows];
                for (int r = 0; r < max_rows; r++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        data1[i + r * 30] = data[i, r];
                    }
                }
                ViewState.Add("data", data1);
                ViewState.Add("UseTTD",UseTTD);
                if (!UseTTD)
                {
                    Guid[] ScheduledPeriodIds1 = new Guid[30 * max_rows];
                    for (int r = 0; r < max_rows; r++)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            ScheduledPeriodIds1[i + r * 30] = ScheduledPeriodIds[i, r];
                        }
                    }
                    ViewState.Add("ScheduledPeriodIds", ScheduledPeriodIds1);
                }
                else
                {
                    long[] pointers1 = new long[30 * max_rows];
                    for (int r = 0; r < max_rows; r++)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            pointers1[i + r * 30] = pointers[i, r];
                        }
                    }
                    ViewState.Add("pointers", pointers1);
                }
                ViewState.Add("rooms", rooms_t);
                ViewState.Add("no_rooms", no_rooms);
                ViewState.Add("no_rows", no_rows);
                ViewState.Add("UseTTd", UseTTD);
            }
        }

        protected string GenerateTimetable(string[,] data,string[] rooms, int no_rooms)
        {
            string s = "<p  ><TABLE BORDER  class= \"TimetableTable\"   ><tr><TD>Room</td>";
            //only going to do mon-fri  period 1,2,3,4,5
            string[] days = new string[5]; days[0] = "MO "; days[1] = "TU "; days[2] = "WE "; days[3] = "TH "; days[4] = "FR ";
            for (int d = 0; d < 5; d++)
            {
                for (int p = 1; p < 6; p++)
                {
                    s += "<td>" + days[d]+p.ToString() + "</td>";
                }
            }
            s += "</tr>";
            for (int r = 0; r < no_rooms; r++)
            {
                s += "<tr>";
                s += "<td>" + rooms[r] + "</td>";
                data[0, 0] = rooms[r];
                for (int d = 0; d < 5; d++)
                {
                    for (int p = 1; p < 6; p++)
                    {
                        s += "<td>" + data[p + d * 5, r] + "</td>";
                    }
                }
                s += "</tr>";
            }
            s += "</table></p>";
            return s;
        }



        protected void Button_swop_Click(object sender, EventArgs e)
        {
            string[] rooms = new string[max_rows];//room names
            int no_rooms = 0; int no_rows = 0;
            string[,] data = new string[30, max_rows]; //has the display data
            string[] data1 = new string[30 * max_rows];

            long[,] pointers = new long[30, max_rows];//has the file ppointers...
            long[] pointers1 = new long[30 * max_rows];
            Guid[,] ScheduledPeriodIds = new Guid[30, max_rows];
            Guid[] ScheduledPeriodIds1 = new Guid[30 * max_rows];
            data1 = (string[]) ViewState["data"];
            for (int r = 0; r < max_rows; r++)
            {
                for (int i = 0; i < 30; i++)
                {
                     data[i, r]=data1[i + r * 30];
                }
            }
            bool UseTTD = (bool)ViewState["UseTTD"];
            if (UseTTD)
            {
                pointers1 = (long[])ViewState["pointers"];
                for (int r = 0; r < max_rows; r++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        pointers[i, r] = pointers1[i + r * 30];
                    }
                }
            }
            else
            {
                ScheduledPeriodIds1 = (Guid[])ViewState["ScheduledPeriodIds"];
                for (int r = 0; r < max_rows; r++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        ScheduledPeriodIds[i, r] = ScheduledPeriodIds1[i + r * 30];
                    }
                }
            }
            no_rooms = (int)ViewState["no_rooms"];
            rooms=(string[])ViewState["rooms"];
            no_rows = (int)ViewState["no_rows"];//note rows from no_rooms to no_rows are non std

            int d = 0; int p = 0;
            d = DropDownList_day.SelectedIndex;
            p = DropDownList_period.SelectedIndex+1;

            string temps = ""; long tempp = 0; Guid tempId;
            ListItem l = DropDownList_room1.SelectedItem;
            int r1 = System.Convert.ToInt16(l.Value);
            l = DropDownList_room2.SelectedItem;
            int r2 = System.Convert.ToInt16(l.Value);

            if ((d > no_rooms) || (p > no_rooms))
            {
                //complex because one or both are non std
                //need to find the r
            }
            else
            {
                //simple row swops....
                temps = data[p + d * 5, r1]; 
                data[p + d * 5, r1] = data[p + d * 5, r2]; 
                data[p + d * 5, r2] = temps;
                if (UseTTD)
                {
                    tempp = pointers[p + d * 5, r1];
                    pointers[p + d * 5, r1] = pointers[p + d * 5, r2];
                    pointers[p + d * 5, r2] = tempp;
                }
                else
                {
                    tempId = ScheduledPeriodIds[p + d * 5, r1];
                    ScheduledPeriodIds[p + d * 5, r1] = ScheduledPeriodIds[p + d * 5, r2];
                    ScheduledPeriodIds[p + d * 5, r2] = tempId; ;
                }
                content0.InnerHtml = GenerateTimetable(data, rooms, no_rooms);
            }

            for (int r = 0; r < max_rows; r++)
            {
                for (int i = 0; i < 30; i++)
                {
                    data1[i + r * 30] = data[i, r];
                }
            }

            ViewState.Add("data", data1);

            if (UseTTD)
            {
                for (int r = 0; r < max_rows; r++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        pointers1[i + r * 30] = pointers[i, r];
                    }
                }
                ViewState.Add("pointers", pointers1);
            }
            else
            {
                for (int r = 0; r < max_rows; r++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        ScheduledPeriodIds1[i + r * 30] = ScheduledPeriodIds[i, r];
                    }
                }
                ViewState.Add("ScheduledPeriodIds", ScheduledPeriodIds1);
            }
            ViewState.Add("rooms", rooms);
            ViewState.Add("no_rooms", no_rooms);

        }

        protected void Button_savechanges_Click(object sender, EventArgs e)
        {
            // oh deary me... recover the viewstate
            string[] rooms = new string[max_rows];//room names
            int no_rooms = 0;
            string[,] data = new string[30, max_rows]; //has the display data
            string[] data1 = new string[30 * max_rows];

            long[,] pointers = new long[30, max_rows];//has the file ppointers...
            long[] pointers1 = new long[30 * max_rows];

            Guid[,] ScheduledPeriodIds = new Guid[30, max_rows];
            Guid[] ScheduledPeriodIds1 = new Guid[30 * max_rows];

            data1 = (string[])ViewState["data"];
            for (int r = 0; r < max_rows; r++)
            {
                for (int i = 0; i < 30; i++)
                {
                    data[i, r] = data1[i + r * 30];
                }
            }
            bool UseTTD = (bool)ViewState["UseTTD"];
            if (UseTTD)
            {
                pointers1 = (long[])ViewState["pointers"];
                for (int r = 0; r < max_rows; r++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        pointers[i, r] = pointers1[i + r * 30];
                    }
                }
            }
            else
            {
                ScheduledPeriodIds1 = (Guid[])ViewState["ScheduledPeriodIds"];
                for (int r = 0; r < max_rows; r++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        ScheduledPeriodIds[i, r] = ScheduledPeriodIds1[i + r * 30];
                    }
                }

            }
            no_rooms = (int)ViewState["no_rooms"];
            rooms = (string[])ViewState["rooms"];
            string file_name = Server.MapPath(@"~/App_Data/tt_data.ttd");

            Cerval_Configuration c = new Cerval_Configuration("StaffIntranet_TTD_TimetablePlanDate");
            DateTime date1 = new DateTime();
            try { date1 = System.Convert.ToDateTime(c.Value); }
            catch { date1 = DateTime.Now; }

            TTData tt1 = new TTData();
            if (UseTTD) { tt1.Load(file_name, date1, date1, false, false, ""); }
            else { tt1.Load_DB(date1); } //load from database

            int[] room_ttcode = new int[30];
            for (int i = 0; i < 30; i++) room_ttcode[i] = -1;
            for(int r =0 ; r<no_rooms;r++)
            {
                for (int r1 = 0; r1 < tt1.room_count; r1++)
                {
                    try
                    {
                        if (tt1.RoomCodes[r1].Trim() == rooms[r].Trim())
                        {
                            //so the code here is r1...
                            room_ttcode[r] = r1;// so for row r the integer index is r1 now in room_ttcode[r]
                        }
                    }
                    catch
                    {// to catch nulls in  room_ttcode[]
                    }
                }
            }


            //now all we have to do is go through the file and update the rooms....
            if (UseTTD)
            {
                FileStream fs = new FileStream(file_name, FileMode.Open, FileAccess.ReadWrite);
                for (int r = 0; r < max_rows; r++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        if (pointers[i, r] > 0)
                        {
                            if(room_ttcode[r]>=0)
                            {
                                fs.Position = pointers[i, r] + 9;
                                fs.WriteByte((byte)room_ttcode[r]);
                            }
                        }
                    }
                }
                fs.Close();
            }
            else
            {
                for (int r = 0; r < max_rows; r++)  //for each row ie each room
                {
                    for (int i = 0; i < 30; i++)   //for each period
                    {
                        if (ScheduledPeriodIds[i, r] !=Guid.Empty)  //there is a set here
                        {    
                            foreach (TTData.TT_period p in tt1.periodlist1.m_list) //find the TT_period
                            {
                                if (p.ScheduledPeriodId == ScheduledPeriodIds[i, r])
                                {
                                    // which room id for this row??
                                    if (room_ttcode[r] >= 0) //ie we found a  ref...
                                    {
                                        Guid new_room = tt1.RoomList1[room_ttcode[r]];
                                        if (p.RoomId != new_room)
                                        {
                                            ScheduledPeriod p1 = new ScheduledPeriod();
                                            p1.Load(p.ScheduledPeriodId);
                                            p1.m_RoomId = new_room;
                                            p1.UpdateRoom();
                                        }
                                    }
                                    break;
                                }
                            }    
                        }
                    }
                }
            }

        }
    }
}
