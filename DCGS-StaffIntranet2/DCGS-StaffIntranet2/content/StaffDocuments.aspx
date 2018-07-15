<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="StaffDocuments.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.StaffDocuments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label1" runat="server" Text="Search Files"></asp:Label>
    <asp:TextBox ID="TextBox_mask" runat="server" 
        ToolTip="Enter Search string and press return" AutoPostBack="True" 
        ontextchanged="TextBox_mask_TextChanged" ></asp:TextBox>
    <br /><asp:Label ID="Label2" runat="server" Text="Click on the files below to open or download them."></asp:Label>
    <asp:Table id="Table1" runat="server" Width="688px" GridLines="Both" HorizontalAlign="Left"
				Font-Size="smaller" BorderWidth="2px" BorderStyle="Double" ></asp:Table>
    <br />
    <br />

</asp:Content>
