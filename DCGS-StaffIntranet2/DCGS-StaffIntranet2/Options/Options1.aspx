<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Options.Master" AutoEventWireup="true" CodeBehind="Options1.aspx.cs" Inherits="DCGS_Staff_Intranet2.Options.Options1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="false" Width="589px" /><br />
    <asp:Button ID="Button_Display" runat="server" Text="Display" OnClick="Button_Display_Click" />
    <asp:Button ID="Button_Upload" runat="server" Text="Process" OnClick="Button_Process_Click" Height="28px" Width="126px" />


    <br />
    <br />
    
    <asp:TextBox ID="TextBox1" runat="server" Height="292px" TextMode="MultiLine" Width="782px" Wrap="False"></asp:TextBox>


</asp:Content>
