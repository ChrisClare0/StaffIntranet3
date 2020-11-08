<%@ Page Title="" Language="C#" MasterPageFile="~/Ezra/EasterHunt/EasterHunt.Master" AutoEventWireup="true" CodeBehind="DiningRoom.aspx.cs" Inherits="DCGS_Staff_Intranet2.Ezra.EasterHunt.DiningRoom" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Room" runat="server"  class="DiningRoom_Background">
        <asp:Button ID="Button1" runat="server" Text="Go to Kitchen" Style="position: absolute; top: 213px; left: 8px; height: 23px;" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="Go to Lounge" Style="position: absolute; top: 601px; left: 408px; height: 23px;" OnClick="Button2_Click" />
        <asp:Button ID="Button3" runat="server" Text="Go to Outside" Style="position: absolute; top: 330px; left: 1072px; height: 23px;" OnClick="Button3_Click" />

    </div>

</asp:Content>
