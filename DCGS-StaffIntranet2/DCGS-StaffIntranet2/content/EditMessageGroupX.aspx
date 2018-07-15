<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="EditMessageGroupX.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.EditMessageGroupX" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content1" runat="server" ></div>
    <asp:ListBox ID="ListBox_SelectedNames" runat="server" SelectionMode="Multiple" Width="299px" Height="101px"></asp:ListBox>
    <asp:Button ID="Button_RemoveSelected" runat="server" Text="Remove Selected"  onclick="Button_RemoveSelected_Click" />
    <asp:Button ID="Button_Clear" runat="server" Text="Clear List" onclick="Button_Clear_Click" />
    <asp:Button ID="Button_Save" runat="server" Text="Save" onclick="Button_Save_Click" />
    <br /><br />
    Select Students to add to Group:<br />
    <asp:DropDownList ID="DropDownList_Years" runat="server" AutoPostBack="True" 
        onselectedindexchanged="DropDownList_Years_SelectedIndexChanged">
        <asp:ListItem Value="7">Year 7</asp:ListItem>
        <asp:ListItem Value="8">Year 8</asp:ListItem>
        <asp:ListItem Value="9">Year 9</asp:ListItem>
        <asp:ListItem Value="10">Year 10</asp:ListItem>
        <asp:ListItem Value="11">Year 11</asp:ListItem>
        <asp:ListItem Value="12">Year 12</asp:ListItem>
        <asp:ListItem Value="13">Year 13</asp:ListItem>
    </asp:DropDownList> <br />
    <asp:ListBox ID="ListBox_Names" runat="server" SelectionMode="Multiple" Height="90px" Width="298px"></asp:ListBox>
    <asp:Button ID="Button_Add" runat="server" Text="Add Names" onclick="Button_Add_Click" />  

</asp:Content>
