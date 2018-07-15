<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/styles/PhysicsBooking.Master" CodeBehind="PhysicsBookingForm.aspx.cs" Inherits="PhysicsBookings.content.Booking.PhysicsBookingForm" %>

<%@ Register Assembly="Cerval_Library" Namespace="Cerval_Library" TagPrefix="cc1" %>



    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


</asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
        <div>
        <cc1:TimetableControl ID="TimetableControl1" runat="server" />
    </div>
    <div>
        <asp:Label ID="Label_ActAs" runat="server" Visible="false" Text="Act as......"></asp:Label>
        <asp:DropDownList ID="DropDownList_staff" runat="server" Visible="false" 
            onselectedindexchanged="DropDownList_staff_SelectedIndexChanged" 
            AutoPostBack="True">
        </asp:DropDownList>
        
        </div>
    </asp:Content>

