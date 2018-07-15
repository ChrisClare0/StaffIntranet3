<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="CreateMessageGroupX.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.CreateMessageGroupX" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

Please select the group to edit:<br />
    <asp:DropDownList ID="DropDownList_groups" runat="server" Height="16px" 
        Width="223px">


    </asp:DropDownList>
    <asp:Button ID="Button_Edit" runat="server" Text="Edit Group Members" 
        onclick="Button_Edit_Click" />
    <asp:Button ID="Button_Delete" runat="server" Text="Delete Group" 
        onclick="Button_Delete_Click" />

<br /> <br />
To create a new private group, please enter a group name (3-30 characters).<br />
<asp:TextBox 
        ID="TextBox_GroupName" MaxLength=30 runat="server"></asp:TextBox>
    <asp:Button ID="Button_MakeGroup" runat="server" Text="Make New Group" 
        onclick="Button_MakeGroup_Click" />
&nbsp;


</asp:Content>
