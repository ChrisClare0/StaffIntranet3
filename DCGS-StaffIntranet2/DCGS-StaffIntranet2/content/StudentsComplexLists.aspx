<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="StudentsComplexLists.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.StudentsComplexLists" %>
<%@ Register assembly="Cerval_Library" namespace="Cerval_Library" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <cc1:ComplexStudentList ID="ComplexStudentList1" runat="server" />


</asp:Content>
