<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="EasterHunt.Master" CodeBehind="Stairs.aspx.cs" Inherits="DCGS_Staff_Intranet2.Ezra.EasterHunt.Stairs" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Room" runat="server"  class="Upstairs_Background">
        <asp:Button ID="Button6" runat="server" Text="Go to Pops Bedroom"  style="position:absolute; top: 389px; left: 1187px; height: 23px; " OnClick="Button2_Click" />
        <asp:Button ID="Button7" runat="server" Text="Go to Small Bedroom"  style="position:absolute; top: 260px; left: 473px; height: 23px; " OnClick="Button3_Click"  />
        <asp:Button ID="Button8" runat="server" Text="Go to Kat's Bedroom"  style="position:absolute; top: 544px; left: 27px; height: 23px; " OnClick="Button4_Click"  />
        <asp:Button ID="Button9" runat="server" Text="Go to Spare Bedroom"  style="position:absolute; top: 243px; left: 788px; height: 23px; " OnClick="Button5_Click"  />
        <asp:Button ID="Button1" runat="server" Text="Go to Bathroom"  style="position:absolute; top: 208px; left: 292px; height: 23px;" OnClick="Button1_Click1" />
        <asp:Button ID="Button2" runat="server" Text="Go Downstairs"  style="position:absolute; top: 935px; left: 533px; height: 23px;" OnClick="Button2_Click1" />
    </div>

</asp:Content>

