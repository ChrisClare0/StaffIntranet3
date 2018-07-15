<%@ Page Language="C#" AutoEventWireup="true"   MasterPageFile="~/styles/StudentInformation.Master" CodeBehind="ExamResults.aspx.cs" Inherits="StudentInformation.ExamResults" %>
<%@ Register Assembly="Cerval_Library" Namespace="Cerval_Library" TagPrefix="cc1" %>


    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Exam Results</title>
    </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <div id="content">
        <div id="Server_Heading" runat="server" > </div><br />
            <cc1:ResultGrid id="Resultgrid1" runat="server"/>
            <div id="VIdatadiv" runat="server"></div>
            <cc1:ResultGrid id="Resultgrid2" runat="server"/>
            <cc1:ResultGrid id="Resultgrid3" runat="server"/>
            <div id="servercontent" runat="server"></div>           
        </div>
</asp:Content>