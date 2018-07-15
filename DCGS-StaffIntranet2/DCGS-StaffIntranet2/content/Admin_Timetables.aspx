<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="Admin_Timetables.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.Admin_Timetables" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h4>Select staff from list. Use Ctrl for mulitipe selections</h4>
    <asp:ListBox ID="ListBox_staff" runat="server" AutoPostBack="True" 
        onselectedindexchanged="ListBox_staff_SelectedIndexChanged" 
        SelectionMode="Multiple" Height="210px" Width="267px"></asp:ListBox>
<div ID="fred" runat="server">
</div>
</asp:Content>
