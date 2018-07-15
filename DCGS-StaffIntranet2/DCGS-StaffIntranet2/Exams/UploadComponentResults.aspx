<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="UploadComponentResults.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.UploadComponentResults" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <asp:Label ID="Label_head" runat="server" Text=""></asp:Label><br />

    <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="false" Width="589px" />
    <br />

    <asp:Button ID="Button_Display" runat="server" Text="Display File"  Height="28px" Width="126px" OnClick="Button_Display_Click" />
    <asp:Button ID="Button_Process" runat="server" Text="Process File"  Visible="false" Height="28px" Width="126px" OnClick="Button_Process_Click" />
    <br /><br /> 

    <asp:Label ID="Label_Text" runat="server" Text=""></asp:Label>
    <asp:TextBox ID="TextBox_Warning" visible="false" runat="server"  TextMode="MultiLine" Width="725px" Wrap="False" Height="53px"></asp:TextBox><BR />

    <asp:TextBox ID="TextBox1" runat="server" Height="292px" TextMode="MultiLine" Width="725px" Wrap="False"></asp:TextBox><BR />

</asp:Content>
