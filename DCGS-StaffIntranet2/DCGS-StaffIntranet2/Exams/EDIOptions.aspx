<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="EDIOptions.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.EDIOptions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">





</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label1" runat="server" Text="Label                          "></asp:Label>
 
    <br/><br />
    
     <asp:TextBox ID="TextBox1" runat="server" Height="146px" Width="799px"></asp:TextBox>
    <br />
       <asp:Button ID="Button1" runat="server" Text="Button" Width="155px" OnClick="Button1_Click" />
    
</asp:Content>
