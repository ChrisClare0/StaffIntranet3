<%@ Page Title="" Language="C#" MasterPageFile="~/styles/PhysicsBooking.Master" AutoEventWireup="true" CodeBehind="BookingSummary.aspx.cs" Inherits="PhysicsBookings.content.Booking.BookingSummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div  id="servercontent" runat="server"></div><br /> 
   
   
    <asp:Calendar ID="Calendar1" runat="server" 
            onselectionchanged="Calendar1_SelectionChanged"></asp:Calendar>
</asp:Content>
