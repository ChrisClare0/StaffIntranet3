<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="ComplexEmail.aspx.cs" Inherits="DCGS_Staff_Intranet2.ComplexEmail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>
        This allows you to email a group of parents, with a message, and an idividual 
        insert to each person. The message is in an external file, and the addresses and 
        individual text should be in a tab spearated file with headings: &quot;Email&quot;, &quot;Text&quot;.</p>
    <p>
        File Location for Message:  <asp:FileUpload ID="FileUpload_Message" runat="server" />
    </p>
    <p>
        Tab Separated file of recipients:  
        <asp:FileUpload ID="FileUpload_Recipients" 
            runat="server" />
    </p>
    <p>
        Subject for email:&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TextBox_emailSubject" runat="server" Width="371px"></asp:TextBox>
    </p>
    <p>
        <asp:TextBox ID="TextBox_Message" runat="server" Width="643px"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="Button_Check" runat="server" Text="Check" 
            onclick="Button_Check_Click" />
        <asp:Button ID="Button_Send" runat="server" Text="Send" 
            onclick="Button_Send_Click" Visible="False" />
    </p>
    <div id="SampleText" runat="server">

    </div>
    <div>
    <br /><br />
    This will be emailled to:<br />
&nbsp;<asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" 
        Width="442px" Height="223px"></asp:TextBox>
</div>
    
</asp:Content>
