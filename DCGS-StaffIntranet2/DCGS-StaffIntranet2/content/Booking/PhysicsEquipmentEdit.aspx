<%@ Page Title="" Language="C#" MasterPageFile="~/styles/PhysicsBooking.Master" AutoEventWireup="true" CodeBehind="PhysicsEquipmentEdit.aspx.cs" Inherits="PhysicsBookings.content.Booking.PhysicsEquipmentEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:DropDownList ID="DropDownList_Equipment" runat="server" Height="26px" 
        AutoPostBack="True" 
        onselectedindexchanged="DropDownList_Equipment_SelectedIndexChanged" 
        Width="253px"></asp:DropDownList>
    <div  id="servercontent" runat="server"></div>
    <asp:Label ID="Label1" runat="server" Text="Equipment:" Width="250px"></asp:Label>
    <asp:TextBox ID="TextBox_Code" runat="server"></asp:TextBox>
    <br />
    <asp:Label ID="Label2" runat="server" Text="Equipment Location:" Width="250px"></asp:Label>
    <asp:TextBox ID="TextBox_Location" runat="server"></asp:TextBox>
    <br />
    
    <asp:Label ID="Label3" runat="server" Text="Equipment Description:" Width="250px" ></asp:Label>
    <asp:TextBox ID="TextBox_Desc" runat="server"></asp:TextBox>
    <br />
    
    <asp:Label ID="Label4" runat="server" Text="Equipment Supplier:"  Width="250px"></asp:Label>
    <asp:TextBox ID="TextBox_Supplier" runat="server"></asp:TextBox>
    
    <br />
    <br />
    <asp:Button ID="Button_Update0" runat="server" Text="Update" 
        onclick="Button_Update_Click" Width="128px" />
        
    <asp:Button ID="Button_Delete" runat="server" Text="Delete" 
        onclick="Button_Delete_Click" Width="128px" Enabled="False" />
        
     <br />
     <hr />
    <asp:Label ID="Label5" runat="server" Text="Used in Experiments:"  Width="250px"></asp:Label><br />
    <asp:TextBox ID="TextBox_ExptList" runat="server" ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
    <br /><hr />
    
    <asp:Label ID="Label6" runat="server" Text="Equipment Code:" Width="250px"></asp:Label><asp:TextBox ID="TextBox_New" runat="server" Text></asp:TextBox>
    <asp:Button ID="Button_New" runat="server" Text="Create New" 
        onclick="Button_New_Click"  /><br />

</asp:Content>
