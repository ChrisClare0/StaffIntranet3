<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="UploadTextFile.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.UploadTextFile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <asp:Label ID="Label1" runat="server" Text=""></asp:Label><br />
    <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="false" Width="589px" /><br />
    <asp:Button ID="Button_Upload" runat="server" Text="Display File" OnClick="Button_Upload_Click" Height="28px" Width="126px" />
    <asp:Button ID="Button_CheckTime" runat="server" Text="Continue"  Visible="false" Height="28px" Width="126px" OnClick="Button_CheckTime_Click" />
    <asp:Button ID="Button_Process" runat="server" Text="Process File" OnClick="Button_Process_Click" Visible="false" Height="28px" Width="126px" />
    <br /><br /> 
    <asp:Label ID="Label_Text" runat="server" Text=""></asp:Label>
    <asp:TextBox ID="TextBox_Warning" visible="false" runat="server"  TextMode="MultiLine" Width="725px" Wrap="False" Height="53px"></asp:TextBox><BR />

    <asp:TextBox ID="TextBox1" runat="server" Height="292px" TextMode="MultiLine" Width="725px" Wrap="False"></asp:TextBox><BR />
        </asp:Content>

