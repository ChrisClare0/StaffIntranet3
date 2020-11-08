<%@ Page Language="C#"  MasterPageFile="EasterHunt.Master" AutoEventWireup="true" CodeBehind="Lounge.aspx.cs" Inherits="DCGS_Staff_Intranet2.Ezra.EasterHunt.Lounge" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="EasterHuntDRoom" runat="server"  class="Lounge_Background">
        <asp:Button ID="Button1" runat="server" Text="Go to Dining Room" Style="position: absolute; top: 806px; left: 538px; height: 23px;" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="Go to Hall" Style="position: absolute; top: 687px; left: 1107px; height: 23px;" OnClick="Button2_Click" />

    </div>

</asp:Content>



