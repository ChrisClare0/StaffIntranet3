<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="StartForm.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.StartForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <link href="../styles/TableStyles.css" rel="stylesheet" type="text/css" />

    


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div  id="servercontent" runat="server"></div>
<div id="Calendar"  runat="server">
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Testing" Width="116px" />
    </div>

</asp:Content>
