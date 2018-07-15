<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/styles/PhysicsBooking.Master" CodeBehind="BookingList.aspx.cs" Inherits="PhysicsBookings.content.Booking.BookingList" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../styles/TableStyles.css" rel="stylesheet" type="text/css" />

</asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
    <div  id="servercontent" runat="server"></div><br /> 
    <asp:Table ID="Table1" runat="server" BorderStyle="Ridge"  class= "ResultsTbl">
    </asp:Table>
    <br />
        <asp:Label ID="Label1" runat="server" Text="Notes for this Booking:"></asp:Label>
        <br /> 
        <asp:TextBox ID="TextBox_Notes" runat="server" 
            Width="528px" TextMode="MultiLine"></asp:TextBox>
        <asp:Button ID="Button_SaveNotes" runat="server" 
            onclick="Button_SaveNotes_Click" Text="Save" Height="25px" />
        <asp:Button ID="Button_delete" runat="server" Text="Delete this Booking" 
            Height="25px" onclick="Button_delete_Click" Width="157px" />
            <br />
            <div id="PreviousBookingdiv" visible="false">  </div>
            
   <asp:Label ID="Label2" runat="server" Text="Previous Booking for this group:"></asp:Label>         
        <asp:TextBox ID="TextBox_last" runat="server" Height="44px" 
            TextMode="MultiLine" Width="239px"></asp:TextBox>   
        <asp:Button ID="Button_AddLast" runat="server" Text="Add these Items" 
            Height="25px" onclick="Button_AddLast_Click" />
        </div>
        <br />
        <hr />Add Items from Equipment List
    <br />
        <asp:DropDownList ID="DropDownList_Items" runat="server" Height="25px" 
            Width="309px">
        </asp:DropDownList>
    <asp:Button ID="Button_Add" runat="server" Text="Add Item" onclick="Button1_Click" 
            Height="25px" Width="133px" /><br />
        <hr />Add Experiment

        <br />
            <asp:DropDownList ID="DropDownList_Expt" runat="server" Width="309px" 
            AutoPostBack="True" 
            onselectedindexchanged="DropDownList_Expt_SelectedIndexChanged" 
            Height="25px">
            </asp:DropDownList>
        <asp:Button ID="Button_AddExperiment" runat="server" Text="Add Experiment" 
            Height="25px" Width="133px" onclick="Button_AddExperiment_Click" />
            
            
            <br />
            <asp:RadioButtonList ID="RadioButtonList1" runat="server" 
            AutoPostBack="True" 
            onselectedindexchanged="RadioButtonList1_SelectedIndexChanged" 
            RepeatDirection="Horizontal">
                <asp:ListItem Value="3">KS3</asp:ListItem>
                <asp:ListItem Value="4">KS4</asp:ListItem>
                <asp:ListItem Value="5">KS5</asp:ListItem>
                <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
        </asp:RadioButtonList>


             <br />
             <asp:Label ID="Label_List" runat="server" Text="Items:"></asp:Label><br />
             <asp:TextBox ID="TextBox_List" runat="server" Height="67px" TextMode="MultiLine" 
                     Width="302px"></asp:TextBox>
             <br />
             <div  id="servercontent1" runat="server">
             
             </div><br /> 

    </asp:Content>



