<%@ Page Title="Exams Start Page" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="ExamsStart.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.ExamsStart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>
    <asp:Label ID="Label4" CssClass="NormalText" runat="server" Text="Season:"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="DropDownList_Season" CssClass="NormalText" runat="server" AutoPostBack="True">
    <asp:ListItem Value="1">January</asp:ListItem>
    <asp:ListItem Value="6">June</asp:ListItem>
    <asp:ListItem Value="B">November</asp:ListItem>
    <asp:ListItem Value="2">February</asp:ListItem>
    <asp:ListItem Value="3">March</asp:ListItem>
    <asp:ListItem Value="4">April</asp:ListItem>
    <asp:ListItem Value="5">May</asp:ListItem>
    <asp:ListItem Value="7">July</asp:ListItem>
    <asp:ListItem Value="8">August</asp:ListItem>
    <asp:ListItem Value="9">September</asp:ListItem>
    <asp:ListItem Value="A">October</asp:ListItem>
    <asp:ListItem Value="C">December</asp:ListItem>
</asp:DropDownList>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label5" CssClass="NormalText" runat="server" Text="Year:"></asp:Label>
        &nbsp;&nbsp;
        <asp:TextBox ID="TextBox_Year" runat="server"  CssClass="NormalText" ToolTip="The Exam year eg 2016" AutoPostBack="True" Width="77px"></asp:TextBox>
                &nbsp;&nbsp;
            <asp:Button ID="Button_Save" runat="server" Text="Save as Default Values on startup" Width="223px" OnClick="Button_Save_Click" />
    </p>
    <p>
       <asp:Label ID="Label_Error" runat="server" Text="You need to enter a correct Year in the range 2000 - 2050" Visible ="false" Font-Bold="true" BackColor ="Red"></asp:Label>
    </p>

    <div id="text1"  runat="server" >

    </div>


    <asp:Button ID="Button1" runat="server" Text="Test" OnClick="Button1_Click" Width="110px" Visible="false" />
    </asp:Content>

