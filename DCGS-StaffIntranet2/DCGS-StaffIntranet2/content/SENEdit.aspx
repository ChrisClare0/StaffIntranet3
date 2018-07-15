<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/styles/MasterPage1.master"  CodeBehind="SENEdit.aspx.cs"  Inherits="DCGS_Staff_Intranet2.content.SENEdit" %>
<%@ Register assembly="Cerval_Library" namespace="Cerval_Library" tagprefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderLeft" runat="server">
    <div>
    <cc1:SENEditControl ID="SENEdit1" visible ="false" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderRight" runat="server">
</asp:Content>

