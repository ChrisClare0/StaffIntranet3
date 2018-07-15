<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRooms.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.EditRooms" EnableSessionState="False" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="../../styles/TableStyles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
 


    
    <div  id = "controls">
        Day:&nbsp;&nbsp;         <asp:DropDownList ID="DropDownList_day" runat="server">
            <asp:ListItem Value="0">Mon</asp:ListItem>
            <asp:ListItem Value="1">Tues</asp:ListItem>
            <asp:ListItem Value="2">Wed</asp:ListItem>
            <asp:ListItem Value="3">Thurs</asp:ListItem>
            <asp:ListItem Value="4">Fri</asp:ListItem>
        </asp:DropDownList>
&nbsp;&nbsp;&nbsp; Period:&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="DropDownList_period" runat="server">
            <asp:ListItem Value="1">Period 1</asp:ListItem>
            <asp:ListItem Value="2">Period 2</asp:ListItem>
            <asp:ListItem Value="3">Period 3</asp:ListItem>
            <asp:ListItem Value="4">Period 4</asp:ListItem>
            <asp:ListItem Value="5">Period 5</asp:ListItem>
        </asp:DropDownList>
&nbsp;&nbsp;&nbsp; Swop:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="DropDownList_room1" runat="server">
        </asp:DropDownList>
        &nbsp;&nbsp;&nbsp; with&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="DropDownList_room2" runat="server">
        </asp:DropDownList>
&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button_swop" runat="server" onclick="Button_swop_Click" 
            Text="Swop" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button_savechanges" runat="server" 
            onclick="Button_savechanges_Click" Text="Save Changes!" Width="138px" />
    
    </div>
       
    <div id="content0" runat="server" ></div>
    <div id="controls2">
    
    

    
    </div>
    
    
    </form>
</body>
</html>
