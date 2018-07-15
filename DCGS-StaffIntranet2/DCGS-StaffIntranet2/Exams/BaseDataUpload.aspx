<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="BaseDataUpload.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.BaseDataUpload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <asp:Label ID="Label1" runat="server" Text="Browse to the BaseData files you wish to upload:"></asp:Label><br />
    <asp:Label ID="Label2" runat="server" Text="You will need toselect S,O,C,L and possibly D files for each season and board."></asp:Label><br />
    <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="True" />
    <br />
    <asp:Button ID="Button_Upload" runat="server" Text="Upload Selected Files" OnClick="Button_Upload_Click"  />
    <br /><br />
    <asp:Label ID="Label_end" runat="server" Text=""></asp:Label>
</asp:Content>

