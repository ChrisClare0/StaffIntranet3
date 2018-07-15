<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="BaseDataCheckComponents.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.BaseDataCheckComponents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:Label ID="Label_Message" runat="server" Text="Label"></asp:Label>
    <asp:Button ID="Button_Check" runat="server" Text="Check Components" OnClick="Button_Check_Click" />
</asp:Content>
