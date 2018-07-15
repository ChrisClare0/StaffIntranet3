<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessCommandResultPage.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.ProcessCommandResultPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Result Page</title>
<base target="_self" />
<style type="text/css">
.buttonCss
{
color:White;
background-color:Black;
font-weight:bold;
}
</style>
</head>
<body>
<form id="form1" runat="server">
<div>
<asp:Label ID="lblProcessing" runat="server" Text="We are processing your request. Please don't close this browser." />
<br />
<center>
<img id="imgProgress" runat="server" src="../images/loading.GIF" width="200" height="200" />
</allign>
<asp:Label ID="lblResult" runat="server" Visible="false" Font-Bold="true" />
<br />
<center>
<asp:Button ID="btnClose" runat="server" Visible="false" Text="Close" OnClientClick="window.close();" CssClass="buttonCss" />
</center>
</div>
</form>
</body>
</html>
