using System;
using System.Web.UI.WebControls;
using System.IO;

namespace StudentInformation
{
    public partial class Emulator : System.Web.UI.Page
    {
        public processor micro = new processor();
        public string Processor_Type;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EnableViewState = true;
                ViewState.Add("micro", micro);
                Processor_Type=DropDownList_Processor.SelectedValue;
            }
            micro = (processor)ViewState["micro"];
            try
            {
                micro.inport = System.Convert.ToInt16(Text1.Value, 16);
            }
            catch
            {
                micro.inport = 0;
            }
            Hidden_htmlbox1.Value = micro.inport.ToString();
            try
            {
                micro.adc = (int)(255 * System.Convert.ToDouble(Analog_Input.Text) / 5);
            }
            catch
            {
                TextBox_Info.Text = "!Warning Analogue input not a number"; return;
            }
            if (micro.adc > 255) micro.adc = 255; if (micro.adc < 0) micro.adc = 0;
        }
        protected void Input_changed(object sender, EventArgs e)
        {}
        protected bool Assemble()
        {
            //must already have been parsed...
            int[] mem = new int[255];
            string label = ""; string op = ""; int Rd = 0; int Rs = 0; string data = "";
            string s = ""; int n = 0;
            for (int i = 0; i < micro.maxcodelines; i++)
            {
                s = micro.memory[i];
                if (!DecodeLine(ref s, ref label, ref op, ref Rs, ref Rd, ref data)) return false;
                switch (op)
                {
                    case "": break;
                    case "MOVI": mem[i] = 0x0100 + System.Convert.ToInt16(data, 16); break;
                    case "MOV": mem[i] = 0x0200 + Rd + Rs * 16; break;
                    case "ADD": mem[i] = 0x0300 + Rd + Rs * 16; break;
                    case "SUB": mem[i] = 0x0400 + Rd + Rs * 16; break;
                    case "AND": mem[i] = 0x0500 + Rd + Rs * 16; break;
                    case "EOR": mem[i] = 0x0600 + Rd + Rs * 16; break;
                    case "INC": mem[i] = 0x0700 + Rd; break;
                    case "DEC": mem[i] = 0x0800 + Rd; break;
                    case "IN": mem[i] = 0x0900 + Rd; break;
                    case "OUT": mem[i] = 0x0A00 + Rd; break;
                    case "JP": mem[i] = 0x0B00 + FindLabel(data); break;
                    case "JZ": mem[i] = 0x0C00 + FindLabel(data); break;
                    case "JNZ": mem[i] = 0x0D00 + FindLabel(data); break;
                    case "RCALL": mem[i] = 0x0E00 + FindLabel(data); break;
                    case "RET": mem[i] = 0x0F00; break;
                    case "SHL": mem[i] = 0x1000 + Rd; break;
                    case "SHR": mem[i] = 0x1100 + Rd; break;
                    case "EQUB": mem[i] = System.Convert.ToInt16(data, 16); break;
                    case "BYTE": mem[i] = System.Convert.ToInt16(data, 16); break;
                }
                n = i;
            } n++;
            s = "";
            for (int i = 0; i < n; i++)
            {
                s += i.ToString() + "-" + mem[i].ToString() + ":";
            }
            Hidden_htmlbox3.Value = s; return true;
        }
        protected bool GetRegister(ref string s, ref int r)
        {
            if (!s.StartsWith("S"))
            {
                s = "Register must be in format Sn";
                return false;
            }
            try
            {
                //fro tris try to parse 2 digits
                r = System.Convert.ToInt32(s.Substring(1, 1));
                try
                {
                int temp=System.Convert.ToInt32(s.Substring(2, 1));
                r = r * 10 + temp;
                }
                catch
                {
                }
            }
            catch 
            {
                s = "Invalid Register number " + s[1]; return false;
            }
            if ((r < 0) || (r > 20))
            {
                s = "Invalid Register number " + s[1]; return false;
            }
            return true;
        }
        protected bool DecodeLine(ref string line, ref string label, ref string op, ref int Rs, ref int Rd, ref string data)
        {
            line = line.ToUpper();
            int n = 0, k = 0;
            char[] c1 = new char[2]; c1[0] = (char)0x09; c1[1] = ' ';
            //strip comments
            n = line.IndexOf(";");
            if (n != -1) { line = line.Substring(0, n); }
            n = line.IndexOfAny(c1);
            if (line.Length == 0)
            {
                //comment only
                op = ""; return true;//done!
            }
            if (n != 0)
            {
                //we have a label
                k = line.IndexOf(":");
                if (k < 1)
                {
                    //no colon so..
                    line = "Syntax Error:  labels must end with :"; return false;
                }
                label = line.Substring(0, k);
                line = line.Substring(k + 1);
            }
            line = line.Trim(c1);// strip the separators

            //now have opcode.....
            n = line.IndexOfAny(c1);//end of opcode
            if (n == -1) op = line;
            else
            {
                op = line.Substring(0, n);
                line = line.Substring(n + 1);
                line = line.Trim(c1);
                data = line;
            }

            switch (op)
            {
                case "":
                    break;
                case "MOVI":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = data.Substring(data.IndexOf(",") + 1);
                    break;
                case "MOV":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = data.Substring(data.IndexOf(",") + 1);
                    if (!GetRegister(ref data, ref Rs)) { line = data; return false; }
                    data = "";
                    break;
                case "ADD":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = data.Substring(data.IndexOf(",") + 1);
                    if (!GetRegister(ref data, ref Rs)) { line = data; return false; }
                    data = "";
                    break;
                case "SUB":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = data.Substring(data.IndexOf(",") + 1);
                    if (!GetRegister(ref data, ref Rs)) { line = data; return false; }
                    data = "";
                    break;
                case "AND":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = data.Substring(data.IndexOf(",") + 1);
                    if (!GetRegister(ref data, ref Rs)) { line = data; return false; }
                    data = "";
                    break;
                case "EOR":
                    //todo... how to exor
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = data.Substring(data.IndexOf(",") + 1);
                    if (!GetRegister(ref data, ref Rs)) { line = data; return false; }
                    data = "";
                    break;
                case "INC":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = "";
                    break;
                case "DEC":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = "";
                    break;
                case "IN":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = "";
                    break;
                case "OUT":
                    data = data.Substring(data.IndexOf(",") + 1);
                    if (!GetRegister(ref data, ref Rs)) { line = data; return false; }
                    data = "";
                    break;
                case "JP":
                    break;
                case "JZ":
                    break;
                case "JNZ":
                    break;
                case "RCALL":
                    break;
                case "RET":
                    break;
                case "SHL":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = "";
                    break;
                case "SHR":
                    if (!GetRegister(ref data, ref Rd)) { line = data; return false; }
                    data = "";
                    break;
                //emulator only
                case "EQUB": break;
                case "BYTE": break;


                default:
                    line = "Invalid Op code" + op; return false; break;
            }
            return true;
        }
        protected void UpdateView()
        {
            TextBox t1;string s;
            System.Drawing.Color black = System.Drawing.Color.Black;
            System.Drawing.Color red = System.Drawing.Color.Red;
            for (int i = 0; i < 8; i++)
            {
                s=System.Convert.ToString(micro.GetR(i), 16);

                t1 = (TextBox)FindControl("S" + i.ToString());
                t1.ForeColor = black;
                if (t1.Text != s) t1.ForeColor = red;
                t1.Text = s;
            }
            s = System.Convert.ToString(micro.outport, 16);
            TextBoxOut.ForeColor = black;
            if (TextBoxOut.Text != s) TextBoxOut.ForeColor = red;
            TextBoxOut.Text = s;
            Hidden_htmlbox2.Value = s;
            int j = 7; int x = 0; int v = micro.outport;
            System.Web.UI.HtmlControls.HtmlInputRadioButton radio1;
            while (j >= 0)
            {
                x = v - (1 << j);
                radio1 = (System.Web.UI.HtmlControls.HtmlInputRadioButton)FindControl("Out_" + j.ToString());
                radio1.Checked=(x>=0)?true:false;
                v = (x >= 0) ? x : v;
                j--;
            } 
            
            Clock_Out.Text = (((double)micro.clock) / 1000).ToString();
            Z.ForeColor = black;
            s=micro.CurrentZ.ToString();
            if (Z.Text != s) Z.ForeColor = red;
            Z.Text = s;
        }
        protected int FindLabel(string s)
        {
            for (int i = 0; i < micro.maxlabels; i++)
            {
                if (micro.labels[i] == s) return micro.labelvalues[i];
            }
            TextBox_Info.Text = "Error! Label " + s + " not found!";
            return -1;
        }
        protected void Reset()
        {
            micro.reset();
            ViewState.Add("micro", micro);
            TextBox_Info.Text = micro.memory[0];
            UpdateView();
        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        protected void Button_Help_Click(object sender, EventArgs e)
        {
            if (Text_Help.Visible)
            {
                Text_Help.Visible = false;
                Button_Help.Text = "Show Help";
                return;
            }
            Button_Help.Text = "Hide Help";
            string path = "";
            path = Server.MapPath(@"App_Data/EmulatorHelp.txt");
            Text_Help.Visible = true; Text_Help.Text = "";
            string s = @"..\App_Data\EmulatorHelp.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                while ((s = sr.ReadLine()) != null)
                {
                    Text_Help.Text += s+"\n";
                }
            }

        }
        protected void Button_AssemblePIC(object sender, EventArgs e)
        {
            if (micro.maxcodelines == 0) { TextBox_Info.Text = "No code!"; return; }
            micro = (processor)ViewState["micro"];
            Processor_Type = DropDownList_Processor.SelectedValue;
            PIC_output.Text = "";
            string line = "";
            string path = "";
            path = Server.MapPath(@"App_Data/setup_"+Processor_Type+".txt");
            string s = @"..\App_Data\EmulatorHelp.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                while ((s = sr.ReadLine()) != null)
                {
                    PIC_output.Text += s + "\n";
                }
            }

            s = ProgramText.Text;//get text
            string s1 = ""; int n = 0;
            int i = 0;
            micro.maxcodelines = 0; micro.maxlabels = 0;
            while (s.Length > 0)
            {
                line = s; i = s.IndexOf((char)0x0d);
                if (i >= 0)
                {
                    //multi line.. so split it..
                    line = s.Substring(0, i); s = s.Substring(i + 2);
                }
                else s = "";
                s1 = line;//preserve orig line
                if (!ProcessLine(ref line))
                {//we have an error...
                    TextBox_Info.Text = s1 + "at line " + n.ToString() + "   :" + line; return;
                }
                //no error so add the code
                PIC_output.Text += line;
                n++;
            }
            PIC_output.Text += "    END";
            PIC_output.Visible = true;
            Cross_Assemble_Label.Visible = true;
            return;
        }
        protected void Button_Parse_Click(object sender, EventArgs e)
        {
            //parse text
            string s = ProgramText.Text;//get text
            PIC_output.Text = "";
            string line = ""; string s1 = "";int n=0;
            string label = ""; int Rd = 0; int Rs = 0; string data = ""; string op = "";
            int i = 0;
            micro.maxcodelines = 0; micro.maxlabels = 0;
            while (s.Length > 0)
            {
                line = s; i = s.IndexOf((char)0x0d);
                if (i >= 0)
                {
                    //multi line.. so split it..
                    line = s.Substring(0, i); s = s.Substring(i + 2);
                }
                else s = "";
                label = ""; op = "";
                s1 = line;//preserve orig line
                if (!DecodeLine(ref s1, ref label, ref op, ref Rs, ref Rd, ref data))
                {//we have an error...
                    TextBox_Info.Text = s1 + "at line "+n.ToString() +"   :"+line; return;

                }
                //no error so add the code
                if (label == "TABLE")
                {
                    micro.table = micro.maxcodelines;
                }
                if (label != "")
                {
                    micro.labels[micro.maxlabels] = label;
                    micro.labelvalues[micro.maxlabels] = micro.maxcodelines;
                    micro.maxlabels++;
                }
                if ((op == "BYTE") || (op == "EQUB"))//emulator only
                {
                    micro.memory[micro.maxcodelines] = data;
                    micro.maxcodelines++;
                    op = "";
                }
                if (op != "")
                {
                    micro.memory[micro.maxcodelines] = line;
                    micro.maxcodelines++;
                }
                n++;
            }
            Reset(); Reset();
            Button_Step.Enabled = true;
            Button_Reset.Enabled = true;
            Button_PIC.Enabled = true;
            //Assemble();
        }
        protected void Button_Step_Click(object sender, EventArgs e)
        {
            //need to get the pc line of code........
            if (micro.maxcodelines == 0) { TextBox_Info.Text = "No code!"; return; }
            micro = (processor)ViewState["micro"];
            string line = micro.memory[micro.CurrentPC];
            string label = ""; int Rd = 0; int Rs = 0; string data = ""; string op = "";
            if (!DecodeLine(ref line, ref label, ref op, ref Rs, ref Rd, ref data))
            {
                TextBox_Info.Text = "No such line"; return;
            }
            int imm = 0;
            if ((data == "READTABLE") && (op == "RCALL")) { op = "READTABLE"; data = ""; }
            if ((data == "READADC") && (op == "RCALL")) { op = "READADC"; data = ""; }
            if ((data == "WAIT1MS") && (op == "RCALL")) { op = "WAIT1MS"; data = ""; }
            if (data != "")
            {
                if ((op == "JP") || (op == "JZ") || (op == "JNZ"))
                {
                    imm = FindLabel(data);//is it a label....
                }
                else
                {
                    try
                    {
                        imm = System.Convert.ToInt32(data, 16);
                    }
                    catch
                    {
                        imm = FindLabel(data);
                    }
                }
            }

            if (imm == -1)return;
            int d1 = (int)imm;
            try
            {
                double x = System.Convert.ToDouble(Analog_Input.Text);
                micro.adc = (int)(255 * (x / 5));
            }
            catch
            {
                micro.adc = 0;
            }
            micro.inport = System.Convert.ToInt16(Text1.Value);
            if (!micro.execute(op, Rd, Rs, d1, ref line)) TextBox_Info.Text = "Error! " + line;
            UpdateView();
            TextBox_Info.Text = micro.memory[micro.CurrentPC];
        }
        protected bool ProcessLine(ref string line)
        {
            char t1 = (char)0x09;string t = ""; t += t1;
            line = line.ToUpper();
            string label = "";
            string op = "";
            string data = ""; int Rs = 0; int Rd = 0;
            if (!DecodeLine(ref line, ref label, ref op, ref Rs, ref Rd, ref data))
            {
                return false;
            }
            line = "";
            if (op == "BLANK") return true;
            if (op == "") return true;
            if (label == "TABLE")
            { line += t + "ORG 0x400" + t + " \r\n" + label + " \r\n"+t; }
            else { line += label + (char)0x09; }
            

            switch (op)
            {
                case "":
                    line += "nop";
                    break;
                case "MOVI":
                    line += "movlw " + t + "0x"+data+"\n";
                    line += t + "movwf" + t + "R" + Rd.ToString()+"\n";
                    break;
                case "MOV":
                    line += "movf" + t + "R" + Rs.ToString() + ",0 " + "\n";
                    line += t + "movwf" + t + "R" + Rd.ToString() + "\n";
                    break;
                case "ADD":
                    line += "movf" + t + "R" + Rs.ToString() + ",0 " + "\n";
                    line += t + "addwf" + t + "R" + Rd.ToString() + ",1 " + "\n"; 
                    break;
                case "SUB":
                    line += "movf" + t + "R" + Rs.ToString() + ",0 " + "\n";
                    line += t + "subwf" + t + "R" + Rd.ToString() + ",1 " + "\n";
                    break;
                case "AND":
                    line += "movf" + t + "R" + Rs.ToString() + ",0 " + "\n";
                    line += t + "andwf" + t + "R" + Rd.ToString() + ",1 " + "\n"; 
                    break;
                case "EOR":
                    line += "movf" + t + "R" + Rs.ToString() + ",0 " + "\n";
                    line += t + "xorwf" + t + "R" + Rd.ToString() + ",1 " + "\n"; 
                    break;
                case "INC":
                    line += "incf" + t + "R" + Rd.ToString() + ",1 " + "\n"; 
                    break;
                case "DEC":
                    line += "decf" + t + "R" + Rd.ToString() + ",1 " + "\n"; 
                    break;
                case "IN":
                    //read from portB
                    line += "movf" + t + "PORTB,0" + "\n";
                    line += t + "movwf" + t + "R" + Rd.ToString() + "\n"; 
                    break;
                case "OUT":
                    line += "movf" + t + "R" + Rs.ToString() + ",0" + "\n";
                    line += t + "movwf" + t + "PORTC" + "\n"; 
                    break;
                case "JP":
                    line += "goto" + t + data + "\n"; 
                    break;
                case "JZ":
                    line += "btfsc" + t + "STATUS,2" + "\n";
                    line += t + "goto" + t + data + "\n"; 
                    break;
                case "JNZ":
                    line += "btfss" + t + "STATUS,2" + "\n";
                    line += t + "goto" + t + data + "\n"; 
                    break;
                case "RCALL":
                    line += "call" + t + data + "\n"; 
                    break;
                case "RET":
                    line += "return" + "\n";
                    break;
                case "SHL":
                    //need to clear C first
                    line += "bcf" + t + "STATUS,0" + "\n";
                    line += t + "rlf" + t + "R" + Rd.ToString() + ",1" + "\n"; 
                    break;
                case "SHR":
                    //need to clear C first
                    line += "bcf" + t + "STATUS,0" + "\n";
                    line += t + "rrf" + t + "R" + Rd.ToString() + ",1" + "\n";
                    break;
                case "EQUB":
                    line += "db" + t + "0x00, 0x"+data + "\r\n";
                    break;
                case "BYTE":
                    line += "db" + t +"0x00, 0x"+ data + "\r\n";
                    break;
                default:
                    break;
            }
            return true;
        }
        protected void DropDownList_Processor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cross_Assemble_Label.InnerHtml = ">Assembler output for " + DropDownList_Processor.SelectedValue;
        }

    }

    [Serializable]
    public class processor
    {
        private int pc = 0;
        public int CurrentPC
        {
            get
            {
                return pc;
            }
        }
        private byte[] r = new byte[20];
        private int[] stack = new int[8];
        private int sp = 0;
        private int Z;
        public int CurrentZ
        {
            get
            {
                return Z;
            }
        }
        public int table = -1;
        public int inport = 0;
        public int outport = 0;
        public int adc = 0;
        public string[] memory = new string[255*4];//ben's mod
        public int maxcodelines = 0;
        public string[] labels = new string[255];
        public int[] labelvalues = new int[255];
        public int maxlabels = 0;
        public int clock = 0;

        public processor()
        {
            reset();
        }
        public int GetR(int i) { return r[i]; }
        public void reset() 
        { 
            pc = 0; 
            outport = 0; 
            for (int i = 0; i < 8; i++) r[i] = 0; 
            clock = 0; 
        }
        private void push(int d)
        {
            stack[sp] = d; sp++;
        }
        private int pull()
        {
            sp--; return stack[sp];
        }
        private void setZ(int Rd)
        {
            if (r[Rd] == 0) Z = 1; else Z = 0;
        }
        //public bool execute(string op, int Rd, int Rs, byte data, ref string error)//ben's mod
        public bool execute(string op, int Rd, int Rs, int data, ref string error)//ben's mod
        {
            pc++; error = ""; clock++;
            switch (op)
            {
                case "": break;
                case "MOVI": r[Rd] = (byte)data; setZ(Rd); break;
                case "MOV": r[Rd] = r[Rs]; setZ(Rd); break;
                case "ADD": r[Rd] = (byte)(r[Rd] + r[Rs]); setZ(Rd); break;
                case "SUB": r[Rd] = (byte)(r[Rd] - r[Rs]); setZ(Rd); break;
                case "AND": r[Rd] = (byte)(r[Rd] & r[Rs]); setZ(Rd); break;
                case "EOR": r[Rd] = (byte)(r[Rd] ^ r[Rs]); setZ(Rd); break;
                case "SHL": r[Rd] = (byte)(r[Rd] << 1); setZ(Rd); break;
                case "SHR": r[Rd] = (byte)(r[Rd] >> 1); setZ(Rd); break;
                case "INC": r[Rd]++; setZ(Rd); break;
                case "DEC": r[Rd]--; setZ(Rd); break;
                case "JP": pc = data; break;
                case "JZ": if (Z == 1) pc = data; break;
                case "JNZ": if (Z == 0) pc = data; break;
                case "RCALL": push(pc); pc = data; break;
                case "RET": pc = pull(); break;
                case "IN": r[Rd] = (byte)inport; break;
                case "OUT": outport = r[Rs]; break;
                case "READTABLE":
                    if (table < 0) { error = "Error - Table not setup!"; return false; }
                    try
                    {
                        r[0] = (byte)System.Convert.ToInt16(memory[table + r[7]], 16);
                    }
                    catch
                    {
                        error = "Bad Lookup table value address:" + r[7].ToString() + "value:" + memory[r[7]].ToString(); return false;
                    }
                    break;

                case "READADC":  
                    r[0] = (byte)adc; 
                    break;
                case "WAIT1MS": clock += 999; break;

                default: error = "Unknown Op Code " + op; return false; break;
            }
            return true;
        }
    }
}
