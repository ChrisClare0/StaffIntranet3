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
    <div style="height: 309px; width: 837px">
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
    <br /><br/>
    <asp:Button ID="Button1" runat="server" Text="Upload from file" OnClick="Button1_Click" Width="137px" />

    <asp:FileUpload ID="FileUpload1" runat="server" Width="511px" />
    <br />
    <br />
    <asp:Label ID="Label_Text" runat="server" Text="File is tab separted text file and has: Board/Option/Grade/Mark"></asp:Label>
    <br />
    <asp:TextBox ID="TextBox1" runat="server" Height="101px" TextMode="MultiLine" Width="737px"></asp:TextBox>
    <br />

</asp:Content>
