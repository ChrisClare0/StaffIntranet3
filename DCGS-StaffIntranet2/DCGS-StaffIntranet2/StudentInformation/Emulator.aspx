<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Emulator.aspx.cs" Inherits="StudentInformation.Emulator" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <link href="../styles/Emulator_style.css" rel="stylesheet" title="compact" type="text/css" />
    <title>DCGS OCR-Emulator</title>
    <script language="javascript" type="text/javascript">
<!--


function setup()
{
    var v = document.getElementById('Hidden_htmlbox1').value;
    var x; 
    var i=7;
    while(i>=0)
    {
    x=v-(1<<i);
    document.getElementById('In_' + i).checked= (x>=0)? 'checked':'';
    v=(x>=0)?x:v;
    i=i-1;
    }
    
    v = document.getElementById('Hidden_htmlbox2').value;
    i=7;
    while(i>=0)
    {
    x=v-(1<<i);
    //document.getElementById('Out_' + i).checked=(x>=0)?'true':'false';
    v=(x>=0)?x:v;
    i=i-1;
    }  
}



function RadioInput(i) {
    //document.getElementById('In_'+ i).checked=document.getElementById('In_' + i).checked=='false' ? 'true' : 'false';
    //now need to update text... if input.....
    var i=0;var lsb=0;var msb=0;var v1='';var v2='';
    while(i<4)
    {
        lsb=document.getElementById('In_' + i).checked=='' ? lsb : lsb+(1<<i);
        i=i+1;  
    }
    (lsb==0 )?v1='0':v1=v1;
    (lsb==1 )?v1='1':v1=v1;
    (lsb==2 )?v1='2':v1=v1;
    (lsb==3 )?v1='3':v1=v1;
    (lsb==4 )?v1='4':v1=v1;
    (lsb==5 )?v1='5':v1=v1;
    (lsb==6 )?v1='6':v1=v1;
    (lsb==7 )?v1='7':v1=v1;
    (lsb==8 )?v1='8':v1=v1;
    (lsb==9 )?v1='9':v1=v1;
    (lsb==10)?v1='a' : v1=v1;
    (lsb==11)?v1='b' : v1=v1;
    (lsb==12)?v1='c' : v1=v1;
    (lsb==13)?v1='d' : v1=v1;
    (lsb==14)?v1='e' : v1=v1;
    (lsb==15)?v1='f' : v1=v1;
       
    while(i<8)
    {
        msb=document.getElementById('In_'+ i).checked=='' ? msb : msb+(1<<(i-4));
        i=i+1;  
    }
    v2=msb;
    (msb==0)?v2='0':v2=v2;
    (msb==10)?v2='a' : v1=v1;
    (msb==11)?v2='b' : v1=v1;
    (msb==12)?v2='c' : v1=v1;
    (msb==13)?v2='d' : v1=v1;
    (msb==14)?v2='e' : v1=v1;
    (msb==15)?v2='f' : v1=v1;
    
    document.getElementById('Text1').value=v2+v1;
}

//-->
</script>
</head>
<body>
    <form id="form1" runat="server" >    
    <div id="wrapper">
    <div id="branding">
        <h2><span>DCGS OCR-Microprocessor Emulator</span></h2>
    </div>
        <asp:TextBox ID="Text_Help"  runat="server" TextMode="MultiLine" Visible=false 
            Width="1000px" ></asp:TextBox>
    <div id="processor">
    <h3>Processor</h3>
<h5><br />
S0<asp:TextBox ID="S0" runat="server" CssClass="test"></asp:TextBox><br/>
S1<asp:TextBox ID="S1" runat="server" CssClass="test" ></asp:TextBox><br/>
S2<asp:TextBox ID="S2" runat="server" CssClass="test" ></asp:TextBox><br/>
S3<asp:TextBox ID="S3" runat="server" CssClass="test" ></asp:TextBox><br/>
S4<asp:TextBox ID="S4" runat="server" CssClass="test" ></asp:TextBox><br/>
S5<asp:TextBox ID="S5" runat="server" CssClass="test" ></asp:TextBox><br/>
S6<asp:TextBox ID="S6" runat="server" CssClass="test" ></asp:TextBox><br/>
S7<asp:TextBox ID="S7" runat="server" CssClass="test" ></asp:TextBox><br />
Z-flag  <asp:TextBox ID="Z" runat="server" Width="20px"></asp:TextBox><br /><br />
Time<asp:TextBox ID="Clock_Out" runat="server" CssClass="test" Text="0.000" ReadOnly=true></asp:TextBox></h5>

    <asp:Button ID="Button_Reset" runat="server" Text="Reset" Enabled=false onclick="Button_Reset_Click" /><br />
    <asp:Button ID="Button_Step"  runat="server" Text="Step" Enabled=false onclick="Button_Step_Click"  />
    </div>    
    <div id="content">
    <div id="code">
    <h3>Code Window</h3>
    <button id=test1 onclick="click1"></button>
    <asp:TextBox ID="TextBox_Info" runat="server" >No code loaded - paste/type code into code window</asp:TextBox>
    <asp:Button ID="Button_Parse" runat="server" Text="Read Code" onclick="Button_Parse_Click"/>
    <asp:Button ID="Button_Help"  runat="server" Text="Show Help" onclick="Button_Help_Click"  />
    <asp:Button ID="Button_PIC"  runat="server" Enabled="false" Text="Cross_Assemble" onclick="Button_AssemblePIC"  />
    <asp:TextBox ID="ProgramText"  runat="server" CssClass="codetext" Text="Code goes here...." TextMode="MultiLine" ontextchanged="Input_changed"></asp:TextBox>
<div>
    <asp:Label ID="Label_SelectProc" runat="server" Text="Select Processor    "></asp:Label>
        <asp:DropDownList ID="DropDownList_Processor" runat="server" 
        onselectedindexchanged="DropDownList_Processor_SelectedIndexChanged" >
            <asp:ListItem>16F876</asp:ListItem>
            <asp:ListItem>16F73</asp:ListItem>
        </asp:DropDownList>
        
        </div>
        <h4>
            <label id="Cross_Assemble_Label" visible=false runat=server>Assembler output for 16F876</label></h4>
    <asp:TextBox ID="PIC_output"   runat="server" CssClass="codetext" TextMode="MultiLine" Visible=false ></asp:TextBox>
    </div>
    
    <div id="ports">
    <h3>Ports</h3>
    <div id="input1" class="port1">
    <h4>Input Port:<input id="Text1" type="text" readonly="readonly" runat="server" value="00" class="test"/></h4><h5>
    b7<input id="In_7" onclick="RadioInput(7)" type="checkbox" />
    b6<input id="In_6" onclick="RadioInput(6)" type="checkbox" />
    b5<input id="In_5" onclick="RadioInput(5)" type="checkbox"/>
    b4<input id="In_4" onclick="RadioInput(4)" type="checkbox" />
    b3<input id="In_3" onclick="RadioInput(3)" type="checkbox"/>
    b2<input id="In_2" onclick="RadioInput(2)" type="checkbox" />
    b1<input id="In_1" onclick="RadioInput(1)" type="checkbox" />
    b0<input id="In_0" onclick="RadioInput(0)" type="checkbox" />
    </h5>
    </div>
    
    <div id="OutputPort"  class="port1">
    <h4>Output Port:<asp:TextBox ID="TextBoxOut" runat="server" CssClass="test"></asp:TextBox></h4>
    <h5>     
    b7<input id="Out_7"  type="radio"  runat="server"/>
    b6<input id="Out_6"  type="radio" runat="server"/>
    b5<input id="Out_5"  type="radio" runat="server"/>
    b4<input id="Out_4"  type="radio" runat="server" />
    b3<input id="Out_3"  type="radio" runat="server"/>
    b2<input id="Out_2"  type="radio" runat="server" />
    b1<input id="Out_1"  type="radio" runat="server"/>
    b0<input id="Out_0"  type="radio" runat="server" />
    </h5> 
    </div>
    <h4>Analog Voltage<asp:TextBox ID="Analog_Input" runat="server" Text="0.0" CssClass="test"></asp:TextBox>V</h4>
    

    </div>
    </div>
    <div id="footer">
    provided by CC: - comments, suggestions and errors to me please.
    </div>
    </div>
    
        <asp:TextBox ID="Hidden_1" runat="server" ReadOnly="True" Visible=false></asp:TextBox>
        <input id="Hidden_htmlbox1" type="hidden" runat=server />
        <input id="Hidden_htmlbox2" type="hidden" runat="server" value="0" />
        <input id="Hidden_htmlbox3" type="hidden" runat=server />
    </form>
<script language="javascript" type="text/javascript">
<!--
setup();//to re-setup bits on input port after re-post from server... uses Hidden_htmlbox1 to pass value back..
//-->
</script>
</body>
</html>