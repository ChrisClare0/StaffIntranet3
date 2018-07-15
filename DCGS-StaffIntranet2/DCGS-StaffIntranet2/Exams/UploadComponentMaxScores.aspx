<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="UploadComponentMaxScores.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.UploadComponentMaxScores" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div><br />Please select the Exam Board...
<asp:DropDownList ID="DropDownList_ExamBoards" runat="server" AutoPostBack="true" Height="20px"  Width="200px" OnSelectedIndexChanged="DropDownList_ExamBoards_SelectedIndexChanged" ></asp:DropDownList>
    </div>
<div><br />and Option code.... 

    <asp:DropDownList ID="DropDownList_Options" runat="server" AutoPostBack="true" Height="20px"  Width="427px" OnSelectedIndexChanged="DropDownList_Options_SelectedIndexChanged"></asp:DropDownList>
    </div>
    <div><br />and Component code.... 

    <asp:DropDownList ID="DropDownList_Component" runat="server" AutoPostBack="true" Height="20px"  Width="427px" OnSelectedIndexChanged="DropDownList_Components_SelectedIndexChanged"></asp:DropDownList>
    </div>
    <br />
    <div  >
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label><asp:TextBox ID="TextBoxValue" runat="server" Width="50"></asp:TextBox>
        <asp:Button ID="ButtonUpdate" runat="server" Text="Update" OnClick="ButtonUpdate_Click" />

    </div>
</asp:Content>
