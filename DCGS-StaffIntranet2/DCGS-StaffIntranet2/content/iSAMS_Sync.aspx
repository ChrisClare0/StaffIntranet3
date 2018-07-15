<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="iSAMS_Sync.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.iSAMS_Sync" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>This is in development - use with caution!</h2>
    
        <asp:Button ID="Button_sync" runat="server"  Height="33px" OnClick="Button_synClick" Text="Sync iSAMSGroups" Width="163px" />
    <asp:Label ID="EndDate" runat="server" Text="End Date  (dd/mm/yyyy)"></asp:Label><asp:TextBox ID="TextBox_EndDate" runat="server" Width="146px"  Text="31/7/2018"></asp:TextBox>
    <asp:Label ID="Label1" runat="server" Text="year"></asp:Label><asp:TextBox ID="TextBox_Year" runat="server" Width="96px"  Text="10"></asp:TextBox>
    <br /><br />
    <asp:TextBox ID="TextBox_Out" runat="server" TextMode="MultiLine" Height="240px" Width="825px"></asp:TextBox>
</asp:Content>
