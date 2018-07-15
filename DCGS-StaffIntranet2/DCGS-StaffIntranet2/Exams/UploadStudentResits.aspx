<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="UploadStudentResits.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.UploadStudentResits" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <asp:Label ID="Label3" runat="server" Text="Enter Student Admission Number:"></asp:Label>   
    <asp:TextBox ID="TextBox_StudentAdno" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
    <br />
     <br />
    <asp:Label ID="Label_Grid" runat="server" Text=""></asp:Label><br /><br /> 
    <asp:GridView ID="GridView1" runat="server"  DataSourceID="SqlDataSource1"  CssClass="ExamsTbl" AutoGenerateSelectButton="True" OnSelectedIndexChanging="GridView1_SelectedIndexChanging" OnDataBound="GridView1_DataBound" Height="81px">
    </asp:GridView><br />
    <asp:Label ID="Label_Error" runat="server" Text=""></asp:Label>
    <br /> 
    <asp:Label ID="Label_cost" runat="server" Text=""></asp:Label>
    <br />  <br />
        
        <asp:Button ID="Button_Upload" runat="server" Text="Upload Selected Row" OnClick="Button_Upload_Click" />
        
        <br />
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" DataSourceMode ="DataSet"></asp:SqlDataSource>


    <br />



</asp:Content>
