<%@ Page Title="" Language="C#" MasterPageFile="~/styles/MasterPage1.master" AutoEventWireup="true" CodeBehind="StaffInfo.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.StaffInfo" %>
<%@ Register assembly="Cerval_Library" namespace="Cerval_Library" tagprefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderLeft" runat="server">

<div id="content0" runat="server" >

</div>
<cc1:timetablecontrol ID="TimetableControl1" visible="false" runat="server" />
<cc1:GroupIncidentControl ID="GroupIncidentControl1" runat="server"  GroupCode="" visible ="false"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderRight" runat="server">
    <asp:Label ID="Label_Year" runat="server" Text="Staff" Width="160px" style ="float:left; " Font-Size="Small" ></asp:Label>
<asp:TextBox ID="TextBox_mask" runat="server" AutoPostBack="True" 
        ToolTip="Type a mask and press return" Visible="False" Font-Size="Small" 
        ontextchanged="TextBox_mask_TextChanged" ></asp:TextBox><asp:ListBox ID="NameList" runat="server" Height="158px" style ="float:left; " 
        Width="166px" AutoPostBack="True" ToolTip="Select a member of staff" 
        onselectedindexchanged="NameList_SelectedIndexChanged" Font-Size="X-Small"></asp:ListBox>

    <asp:RadioButtonList ID="Display_List" runat="server"
    AutoPostBack="True" 
        BorderStyle="Ridge" CausesValidation="True" Font-Size="X-Small" 
        Width="169px" onselectedindexchanged="Display_List_SelectedIndexChanged" >
        <asp:ListItem Selected="True">Timetable</asp:ListItem>
        <asp:ListItem  Enabled="false">Details</asp:ListItem>
        <asp:ListItem  Enabled="false">Incidents Authored by</asp:ListItem>
    </asp:RadioButtonList><br />
        <asp:RadioButtonList ID="RadioButtonList_StaffType" runat="server"
    AutoPostBack="True" 
        BorderStyle="Ridge" CausesValidation="True" Font-Size="X-Small" 
        Width="169px" 
        onselectedindexchanged="RadioButtonList_StaffType_SelectedIndexChanged" >
        <asp:ListItem Selected="True" Value="staff">Teaching Staff</asp:ListItem>
        <asp:ListItem  Enabled="True" Value="Currentstaff">Current staff</asp:ListItem>
        <asp:ListItem  Enabled="True" Value="Allstaff">All staff</asp:ListItem>
        </asp:RadioButtonList>
        </asp:Content>