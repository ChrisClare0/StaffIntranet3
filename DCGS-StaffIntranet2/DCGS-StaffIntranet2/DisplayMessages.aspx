<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayMessages.aspx.cs" Inherits="DCGS_Staff_Intranet.DisplayMessages" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Display Student Messages</title>
<link href="TableStyles.css" rel="stylesheet" type="text/css" />
</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <input type="hidden" runat="server" id="GroupCode" />
    <asp:CheckBox ID="CheckBox1"  Text="Show messages delivered but still valid"  runat="server" 
            oncheckedchanged="CheckBox1_CheckedChanged" AutoPostBack="true"/>


    </div>
    <asp:Table ID="Table1"  runat="server" BorderStyle="Solid"  class ="ResultsTbl">
    </asp:Table>
    </form>

</body>
</html>
