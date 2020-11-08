<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="EasterHunt.Master" CodeBehind="Kitchen.aspx.cs" Inherits="DCGS_Staff_Intranet2.Ezra.EasterHunt.Kitchen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Room" runat="server"  class="Kitchen_Background">
 
        <asp:Button ID="Button6" runat="server" Text="Go to Hall" Style="position: absolute; top: 223px; left: 104px; height: 23px;" OnClick="Button2_Click" />
 
        <asp:Button ID="Button5" runat="server" Text="Go to Dining Room" Style="position: absolute; top: 938px; left: 401px; height: 23px;" OnClick="Button1_Click" />
   </div>
        


</asp:Content>
