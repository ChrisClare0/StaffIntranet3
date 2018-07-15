<%@ Page Language="C#" AutoEventWireup="true"   MasterPageFile="~/styles/StudentInformation.Master" CodeBehind="PupilTimetable.aspx.cs" Inherits="StudentInformation.PupilTimetable" %>


    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
		<title>Pupil Timetable</title>
    </asp:Content>
	
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content">
        
    <div id="servercontent" runat="server"></div>
    <asp:Button ID="Button1" runat="server" Text="Switch to current Date"  onclick="Button1_Click" />
    <asp:Label ID="Label1" runat="server" Text="This added for Jamie W" Font-Size="X-Small"></asp:Label>
    </div>
</asp:Content>
