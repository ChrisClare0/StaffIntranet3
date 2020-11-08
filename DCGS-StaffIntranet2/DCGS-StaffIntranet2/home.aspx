<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="DCGS_Staff_Intranet2.home" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body style="width: 294px">
    <form id="form1" runat="server">
    <div></div>


        <asp:RadioButtonList ID="RadioButtonList1" runat="server">
            <asp:ListItem Selected="True" Value="0">Cerval Intranet</asp:ListItem>
            <asp:ListItem Value="1">4Matrix Site</asp:ListItem>
            <asp:ListItem Value="2">Physics Equipment Booking</asp:ListItem>
        </asp:RadioButtonList>

        <a href="home.aspx">home.aspx</a>
    </form>
</body>
</html>
