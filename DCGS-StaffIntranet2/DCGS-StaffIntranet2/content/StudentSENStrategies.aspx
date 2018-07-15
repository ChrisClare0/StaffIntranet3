<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="StudentSENStrategies.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.StudentSENStrategies" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="Servercontentleft" runat="server" style="float:left; width:615px; " >

        <div id="content0" runat="server" ></div>
    <asp:TextBox ID="TextBox1" runat="server"  Visible="false" TextMode="MultiLine" 
            Height="59px" Width="609px"></asp:TextBox>
        <br />
        <asp:Button ID="CreateNewButton" runat="server" Text="Add Strategy" OnClick="CreateNewButton_Click" />
        <asp:Button ID="Button_Save"     runat="server" Text="Save" Visible="false" onclick="Button_Save_Click" />
        <asp:Button ID="Button_Cancel"   runat="server" Text="Cancel" Visible="false" onclick="Button_Cancel_Click" />
        <asp:Button ID="Button_Delete"   runat="server" Text="Delete" Visible="false" 
            onclick="Button_Delete_Click" />
</div>

</asp:Content>
