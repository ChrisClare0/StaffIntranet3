<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="TimetableDesks.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.TimetableDesks" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <asp:Label ID="Label_message" runat="server" Text=""  Font-Size="Larger" Visible ="false" ></asp:Label><br />
    <br />
    <asp:Panel ID="Panel1" runat="server" BorderStyle="Inset" Height="34px">
        <asp:Button ID="Button_AssignDesks" runat="server" Text="Assign Desks" OnClick="Button_AssignDesks_Click" Width="217px" />

        <asp:CheckBox ID="CheckBox1" runat="server" TextAlign="Left" Text="Try to leave blank columns if space" />

    </asp:Panel>

    <br />

    <asp:Panel ID="Panel_Desks" runat="server" BorderStyle="Inset"  >
        <asp:Label ID="Label1" runat="server" Text="Room:    "></asp:Label>
        <asp:DropDownList ID="DropDownListDeskRooms" runat="server" Height="16px" 
            OnSelectedIndexChanged="DropDownListDeskRooms_SelectedIndexChanged" 
            Width="109px"   AutoPostBack="true">

        </asp:DropDownList>
        <asp:Label ID="Label_DeskDate" runat="server" Text="Date.....">
        </asp:Label>

        <asp:DropDownList ID="DropDownListDeskTimes" runat="server"   
            AutoPostBack="true" 
            OnSelectedIndexChanged="DropDownListDeskTimes_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:Button ID="ButtonDesk_Next" runat="server" Text="Next Change" OnClick="ButtonDesk_Next_Click" />

        <asp:Table ID="TableDeskView" runat="server" EnableViewState="False">
        </asp:Table>
        <br />
        <br />

    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server" Height="55px">
        <asp:Button ID="Button_GotoRooms" runat="server" OnClick="Button_GotoRooms_Click" Text="View RoomsALlocation" Width="153px" />
        <asp:Button ID="Button_GotoDaySummary" runat="server"  Text="Return to Day Summary" Width="153px" OnClick="Button_GotoDaySummary_Click" />
        <br />
        <asp:Button ID="ButtonDesk_Print" runat="server" Text="Print" Enabled="False" Width="61px" BorderStyle="Inset" />
        <asp:Button ID="ButtonDesk_PrintAll" runat="server" Text="Print All" Enabled="False" Width="91px" />
    </asp:Panel>

</asp:Content>
