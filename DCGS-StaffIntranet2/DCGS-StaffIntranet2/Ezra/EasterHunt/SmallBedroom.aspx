<%@ Page Language="C#" MasterPageFile="EasterHunt.Master" AutoEventWireup="true" CodeBehind="SmallBedroom.aspx.cs" Inherits="DCGS_Staff_Intranet2.Ezra.EasterHunt.SmallBedroom" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Room" runat="server"  class="SmallBedroom_Background">
        <asp:Button ID="Button1" runat="server" Text="Go to Upstairs Landing" Style="position: absolute; top: 949px; left: 670px; height: 23px;" OnClick="Button1_Click" />

    </div>

</asp:Content>
