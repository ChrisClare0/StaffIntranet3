<%@ Page Language="C#"  MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="PlainResponseForm.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.PlainResponseForm" %>

<%@ Register Assembly="Cerval_Library" Namespace="Cerval_Library" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="../styles/TableStyles.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Server_Content" runat="server"></div>
    <cc1:GroupIncidentControl ID="GroupIncidentControl1" runat="server"  GroupCode="" visible ="false"/>
    <cc1:GroupListControl ID="GroupListControl1" runat="server"  visible ="false" />
    <cc1:GroupStudentDevelopmentControl ID="GroupStudentDevelopmentControl" runat="server" GroupCode="" visible="false" />
</asp:Content>

