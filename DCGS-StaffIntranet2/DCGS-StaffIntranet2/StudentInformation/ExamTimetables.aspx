<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/styles/StudentInformation.Master"  CodeBehind="ExamTimetables.aspx.cs" Inherits="StudentInformation.ExamTimetables" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Exam Timetables</title>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div id="servercontent" runat="server"></div>
         <div><a  href=../content/ExaminationFAQ.pdf>Examination FAQs here</a></div>
    </asp:Content>