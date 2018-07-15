<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateEmail.aspx.cs" Inherits="StudentInformation.UpdateEmail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body bgcolor="#e8e8e8">
    <form id="form1" runat="server">
    <div><p align="center">&nbsp;<p align="center"><asp:Label ID="Label3" runat="server" 
            Text="Update My Email" Font-Names="Arial" Font-Size="Large"></asp:Label></div>
        <asp:Label ID="Label1" runat="server" Text="Current Email:" 
        Font-Names="Arial"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TextBox_old" runat="server" ReadOnly="True" Width="426px" 
        Font-Names="Arial"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="New Email    :" 
        Font-Names="Arial"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TextBox_new" runat="server" Width="426px" 
        Font-Names="Arial"></asp:TextBox>
        <br />
        <asp:Label ID="Label4" runat="server" Text="Label" Visible="False"></asp:Label>
        <br />
        <p align="center"><asp:Button ID="Button_Submit" runat="server" Text="Submit" 
                onclick="Button_Submit_Click"  /></p>
        <br />
    </form>
</body>
</html>
