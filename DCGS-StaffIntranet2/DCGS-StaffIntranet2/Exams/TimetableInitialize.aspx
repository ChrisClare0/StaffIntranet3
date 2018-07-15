<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="TimetableInitialize.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.TimetableInitialize" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel ID="Panel1" runat="server" >
    Be sure you want to do this... this will remove all rooms/desk allcocations!
    <br /><br />

    <asp:Button ID="Button_Initialize" runat="server" Text="Initialize TimeTable" OnClick="Button_Initialize_Click" />
    <asp:Button ID="Button_Clear" runat="server" Text="Clear TimeTable" OnClick="Button_Clear_Click"  />
    <asp:Button ID="Button_Update" runat="server" Text="Update TimeTable" OnClick="Button_Update_Click"  />
    </asp:Panel>



        <asp:Panel ID="Panel2" runat="server" Visible="false" BackColor="#FF9966" BorderStyle="Inset" Width="803px">
        <asp:TextBox ID="TextBox_Warning" runat="server" Font-Size="Large" Text="Are you sure you want to Initialise the Timetable? This will reset all rooms/desks etc.." Width="797px"  BackColor="#FF9966" Height="69px" ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
        <br />
        <asp:Button ID="Button_Cancel" runat="server" Text="Cancel" Width="91px" OnClick="Button_Cancel_Click" />
        <asp:Button ID="Button_OK" runat="server" Text="OK"  Width="85px" OnClick="Button_OK_Click" />
    </asp:Panel>


    <asp:Panel ID="Panel3" runat="server" Visible="false" >
        <asp:Label ID="Label_Panel3" runat="server" Text="Label"></asp:Label><br /><br />
        <asp:Table ID="Table_Summary" runat="server">
        <asp:TableRow runat="server" >
            <asp:TableCell runat="server">Date</asp:TableCell>
            <asp:TableCell runat="server">Monday</asp:TableCell>
            <asp:TableCell runat="server">Tuesday</asp:TableCell>
            <asp:TableCell runat="server">Wednesday</asp:TableCell>
            <asp:TableCell runat="server">Thursday</asp:TableCell>
            <asp:TableCell runat="server">Friday</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br /><br />



    </asp:Panel>
</asp:Content>
