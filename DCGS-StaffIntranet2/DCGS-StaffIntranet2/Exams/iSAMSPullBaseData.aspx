<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="iSAMSPullBaseData.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.iSAMSPullBaseData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label3" runat="server" Text="iSAMS Cycle Number:     "></asp:Label>
    <asp:TextBox ID="TextBox_CycleNumber" runat="server">10</asp:TextBox><br />
    <asp:Button ID="Button_PullBaseData" runat="server" Text="Get Used Base Data for this cycle" OnClick="Button_PullBaseData_Click" />
    <asp:Button ID="Button_PullEntries" runat="server" Text="Copy Exam Entries for this cycle" Width="248px" OnClick="Button_PullEntries_Click" />
    <asp:Button ID="Button1" runat="server" Enabled="False" OnClick="Button1_Click" Text="CleanUp" Width="153px" />
    <br /><br />
    <asp:TextBox ID="TextBox_test" runat="server" Height="186px" TextMode="MultiLine" Width="828px"></asp:TextBox>
</asp:Content>
