<%@ Page Language="C#" MasterPageFile="EasterHunt.Master" AutoEventWireup="true" CodeBehind="Hall.aspx.cs" Inherits="DCGS_Staff_Intranet2.Ezra.EasterHunt.Hall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Room" runat="server"  class="Hall_Background">
        <asp:Button ID="Button5" runat="server" Text="Go to Kitchen" Style="position: absolute; top: 664px; left: 871px; height: 23px;" OnClick="Button1_Click" />
        <asp:Button ID="Button6" runat="server" Text="Go to Lounge" Style="position: absolute; top: 1232px; left: 608px; height: 23px;" OnClick="Button2_Click" />
        <asp:Button ID="Button7" runat="server" Text="Go to Upstairs" Style="position: absolute; top: 1016px; left: 19px; height: 23px;" OnClick="Button3_Click" />
        <asp:Button ID="Button_out" runat="server" Text="Go Outside" Style="position: absolute; top: 366px; left: 539px; height: 23px;" OnClick="Button_out_Click"  />
        <asp:Button ID="Button_study" runat="server" Text="Go to Study" Style="position: absolute; top: 578px; left: 236px; " OnClick="Button_study_Click"  />
    </div>

</asp:Content>
