<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="BaseDataView.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.BaseDataView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <br />
    Select the Exam Board Base Data from the list below. This list only shows those available on-line. 
    If the Board you wish to view does not appear here you will need to download Base Data files from the board
    and then transfer them to this server.  Use the menu option "Upload" for this.
    <br />

    <asp:DropDownList ID="DropDownList_ExamBoards" runat="server" AutoPostBack="true" Height="20px" OnSelectedIndexChanged="DropDownList_ExamBoards_SelectedIndexChanged" Width="427px"></asp:DropDownList>
<br />Then select an option to view:<br />
    <asp:DropDownList ID="DropDownList_data" runat="server" AutoPostBack="true" Height="27px" Width="431px" OnSelectedIndexChanged="DropDownList_data_SelectedIndexChanged" ></asp:DropDownList>
    <br />
     <br />
    <asp:TextBox ID="TextBox1" runat="server" Height="272px" TextMode="MultiLine" Width="420px"></asp:TextBox>
    <br />
    <br />
    <asp:Button ID="Button_Upload" runat="server" Text="Upload Selected to Cerval" OnClick="Button_Upload_Click" />
    <br />
    <br />
</asp:Content>

