<%@ Page Title="" Language="C#"  AutoEventWireup="true" MasterPageFile="~/styles/PhysicsBooking.Master" CodeBehind="PhysicsExperimentEdit.aspx.cs" Inherits="PhysicsBookings.content.Booking.PhysicsExperimentEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<h4>Experiment:</h4>
    <asp:DropDownList ID="DropDownList_Expt" runat="server" AutoPostBack="True" 
        onselectedindexchanged="DropDownList_Expt_SelectedIndexChanged" 
        Height="24px" Width="220px"></asp:DropDownList>   
         
    
    <h4>Current Equipment:</h4>
    <asp:Table ID="Table1" runat="server" BorderStyle="Ridge"  class= "ResultsTbl"></asp:Table>
    <div  id="servercontent" runat="server"></div>
    <hr />
    <h4>Add Equipment:</h4>
    <asp:DropDownList ID="DropDownList_Equipment" runat="server" Height="25px" 
        Width="224px"></asp:DropDownList>    <asp:Button ID="Button_Add" runat="server" Text="Add" onclick="Button_Add_Click" />

    <br /><hr />
    <br /> <h4>Equipment Details:</h4>
    <asp:Label ID="Label1" runat="server" Text="Experiment Code:" Width="250px"></asp:Label>
    <asp:TextBox ID="TextBox_Code" runat="server"></asp:TextBox>
    <br />
    <asp:Label ID="Label2" runat="server" Text="Experiment Key Stage:" Width="250px"></asp:Label>
    <asp:TextBox ID="TextBox_KeyStage" runat="server"></asp:TextBox>
    <br />
    
    <asp:Label ID="Label3" runat="server" Text="Experiment Description:" Width="250px" ></asp:Label>
    <asp:TextBox ID="TextBox_Desc" runat="server"></asp:TextBox>
    <br />
    
    <asp:Label ID="Label4" runat="server" Text="Specification Reference:"  Width="250px"></asp:Label>
    <asp:TextBox ID="TextBox_SpecRef" runat="server"></asp:TextBox>
     <br />
    <asp:Label ID="Label5" runat="server" Text="Experiment Topic:" Width="250px" ></asp:Label>
    <asp:TextBox ID="TextBox_Topic" runat="server"></asp:TextBox>
    <br />
    
    <asp:Label ID="Label6" runat="server" Text="Experiment Notes:"  Width="250px"></asp:Label>
    <asp:TextBox ID="TextBox_Notes" runat="server" TextMode="MultiLine"></asp:TextBox>
    <br />
    <br />
    <asp:Button ID="Button_Update0" runat="server" Text="Update Details" 
        onclick="Button_Update_Click" />
    
    <asp:Button ID="Button_Update" runat="server" Text="Delete" 
        onclick="Button_Delete_Click" />
    
    <br />
    <hr />

<asp:Button ID="Button_New" runat="server" Text="Create New" 
        onclick="Button_CreateNew" />
    <asp:TextBox ID="TextBox_New" runat="server" Text></asp:TextBox>

</asp:Content>
