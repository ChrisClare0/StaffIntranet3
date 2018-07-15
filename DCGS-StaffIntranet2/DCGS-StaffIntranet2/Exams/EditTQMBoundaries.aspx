<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="EditTQMBoundaries.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.EditTQMBoundaries" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div><br />Please select the Exam Board...
<asp:DropDownList ID="DropDownList_ExamBoards" runat="server" AutoPostBack="true" Height="20px"  Width="200px" OnSelectedIndexChanged="DropDownList_ExamBoards_SelectedIndexChanged" ></asp:DropDownList>
    </div>
<div><br />and Option code.... 

    <asp:DropDownList ID="DropDownList_Options" runat="server" AutoPostBack="true" Height="20px"  Width="427px" OnSelectedIndexChanged="DropDownList_Options_SelectedIndexChanged"></asp:DropDownList>
    </div>
    <div>
        <br /><br />
        <div id ="Current_Data"  runat="server"></div>
        <br />
        <div    style="position: absolute;width: 300px;left: 40%;" >
        <asp:DropDownList ID="DropDownList_Grades" runat="server" ></asp:DropDownList>
        <asp:TextBox ID="TextBox_Value" runat="server" Width="64px"></asp:TextBox>
        <asp:Button ID="Button_Update" runat="server" Text="Update" OnClick="Button_Update_Click" />
            <br/>
            <asp:Button ID="Button_Delete" runat="server" Text="Delete TQM for this Option" OnClick="Button_Delete_Click"  />
            </div>

    </div>

</asp:Content>
