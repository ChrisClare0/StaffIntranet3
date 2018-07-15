<%@ Page Title="" Language="C#" MasterPageFile="~/styles/PhysicsBooking.Master" AutoEventWireup="true" CodeBehind="DisplayBooking.aspx.cs" Inherits="PhysicsBookings.content.Booking.DisplayBooking" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <div  id="servercontent" runat="server"></div><br /> 
        
        
        
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
            Text="Delete Booking" Width="143px" />
        
        
        
</asp:Content>
