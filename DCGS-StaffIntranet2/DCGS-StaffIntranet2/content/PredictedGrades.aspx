<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PredictedGrades.aspx.cs" Inherits="DCGS_Staff_Intranet2.PredictedGrades" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
<script language="javascript" type="text/javascript">
<!--


// -->
</script>
</head>
<body bgcolor=#e8e8e8>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Select Set - Changes now only allowed by Subject Leaders" Width="493px" Font-Names="Arial" Height="20px"></asp:Label>
        <br />
        <asp:Label ID="Label_Student" runat="server" Font-Names="Arial" Height="21px" Text="Student"
            Width="491px" Visible="False"></asp:Label><br />
        &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;<asp:ListBox ID="GroupListBox" runat="server" Height="114px" Width="119px" Font-Names="Arial"></asp:ListBox>&nbsp;&nbsp;<br />
        &nbsp;
                <asp:Button ID="Button_Astar" runat="server" Height="24px" OnClick="ButtonAstar_Click" Text="A*" Width="31px" Visible="False" />
            <asp:Button ID="Button_A" runat="server" Height="24px" OnClick="ButtonA_Click" Text="A"
            Width="31px" Visible="False" />
        <asp:Button ID="Button_B" runat="server" Height="24px" OnClick="ButtonB_Click" Text="B"
            Width="31px" Visible="False" />
        <asp:Button ID="Button_C" runat="server" Height="24px" OnClick="ButtonC_Click" Text="C"
            Width="31px" Visible="False" />
        <asp:Button ID="Button_D" runat="server" Height="24px" OnClick="ButtonD_Click" Text="D"
            Width="31px" Visible="False" />
        <asp:Button ID="Button_E" runat="server" Height="24px" OnClick="ButtonE_Click" Text="E"
            Width="31px" Visible="False" />
        <asp:Button ID="Button_U" runat="server" Height="24px" OnClick="ButtonU_Click" Text="U"
            Width="31px" Visible="False" />
<br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Go" Width="42px" />
        <br />
        <br />
        <br />
        <br />
        <asp:TextBox ID="TextBox2" runat="server" Visible="False" Width="297px" OnTextChanged="TextBox2_TextChanged"></asp:TextBox><br />
        <asp:TextBox ID="TextBox1" runat="server" Visible="False" Width="289px"></asp:TextBox>
        <br />
        <asp:TextBox ID="TextBox3" runat="server" Visible="False" Width="332px"></asp:TextBox></div>
    </form>
</body>
</html>
