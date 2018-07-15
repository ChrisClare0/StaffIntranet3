<%@ Page Title="" Language="C#" MasterPageFile="~/styles/MasterPage1.master" AutoEventWireup="true" CodeBehind="DataEntry1.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.DataEntry1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderLeft" runat="server">
    <asp:DropDownList ID="DDList_Sets" runat="server" Height="16px" Width="219px">
    </asp:DropDownList>
    <br />
    <br />
    <asp:Button ID="ButtonWriteSS" runat="server" OnClick="ButtonWriteSS_Click" Text="Creat Google Sheet" />
    <br />
    <br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderRight" runat="server">
</asp:Content>
