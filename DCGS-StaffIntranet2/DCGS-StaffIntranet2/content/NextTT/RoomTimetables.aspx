<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoomTimetables.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.NextTT.RoomTimetables" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Sectional Timetables</title>
        <link href="../../styles/TableStyles.css" media="screen" rel="stylesheet" type="text/css" /> 
        <link href="../../styles/Styles1.css" media="screen" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
             <div id="HeaderDiv" runat="server" ><h2>Room Timetables</h2><br />Select the staff for the display (hold ctrl for multiple selections) and then press &quot;Display&quot;.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                 <asp:Button ID="ButtonGenerateTT" runat="server" 
            onclick="ButtonGenerateTT_Click" Text="Display" />
         </div>
         <div id="content0" runat="server" ></div>
    <div>
    
        <asp:RadioButtonList ID="RadioButtonList1" runat="server" 
            onselectedindexchanged="RadioButtonList1_SelectedIndexChanged" 
            AutoPostBack="True">
            <asp:ListItem Value="Room">Select Rooms</asp:ListItem>
            <asp:ListItem Value="Subjects">Select Subjects</asp:ListItem>
        </asp:RadioButtonList>
    
    
        <asp:ListBox ID="ListBox_staff" runat="server" Height="235px" 
            SelectionMode="Multiple" Width="132px"></asp:ListBox>

    
    </div>
    </form>
</body>
</html>
