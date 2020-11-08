<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="testform.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.testform"   %>

<%@ Register Assembly="Cerval_Library" Namespace="Cerval_Library" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Various testing things///</h2>
    <br /><br />

    <p>

        <asp:Button ID="Button1" runat="server" Height="33px" OnClick="Button1_Click" Text="test1" Width="156px" style="margin-top: 3px" />
        <br /><br />
        <asp:Button ID="Button_test3" runat="server" Height="33px" OnClick="Button_test3_Click" Text="Remake Cerval Year group" Width="159px" style="margin-right: 2px; margin-top: 0px" />
        <asp:Label ID="Label1" runat="server" Text="YearGroup (eg 9  )"></asp:Label><asp:TextBox ID="TextBox_YearGroup" runat="server"></asp:TextBox>
        <br /><br />
        <asp:Button ID="Button2" runat="server" Height="33px" OnClick="ButtonDeleteLeavers_Click" style="margin-top: 0px" Text="Take Leavers off role" Width="182px" />

        <br />


    </p>
    <p>
                &nbsp;</p>
    <p>
        <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label><br />
        <asp:TextBox ID="TextBox1" runat="server"  TextMode="MultiLine" Height="82px" Width="751px"></asp:TextBox>
    </p>
    <br />

    <div id="servercontent" runat="server"></div>

<div id="div1" runat="server">
</div>
         
         
         
</asp:Content>
