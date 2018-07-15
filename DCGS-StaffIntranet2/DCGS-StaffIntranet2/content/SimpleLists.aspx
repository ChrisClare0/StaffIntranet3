<%@ Page Title="" Language="C#" MasterPageFile="~/styles/MasterPage1.master" AutoEventWireup="true" CodeBehind="SimpleLists.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.SimpleLists" %>

<%@ Register Assembly="Cerval_Library" Namespace="Cerval_Library" TagPrefix="cc1" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderLeft" runat="server">

    <cc1:GroupListControl ID="GroupListControl1" runat="server" />
    <asp:Button ID="Button_Email" runat="server" Text="Email Parents" Visible="false"        Width="153px" OnClick="Button_Email_Click" />

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolderRight"  runat="server">
    <asp:Label ID="Label_Groups" runat="server" Text="Year 7" Width="160px" style ="float:left "></asp:Label>
<br />
    <div id="ListDate">
    <label><small>List Date:</small></label>
    <asp:TextBox ID="TextBox_ListDate" runat="server" AutoPostBack="True" 
        ToolTip="Change Date for lists" Visible="True" width="85px"
        ontextchanged="TextBox_ListDate_TextChanged"></asp:TextBox> 
    </div>
<asp:TextBox ID="TextBox_mask" runat="server" AutoPostBack="True" 
        ToolTip="Type a mask and press return" Visible="False" 
        ontextchanged="TextBox_mask_TextChanged"></asp:TextBox>
<asp:ListBox ID="GroupListBox" runat="server" Height="158px" style ="float:left; " 
        Width="166px" AutoPostBack="True" 
        onselectedindexchanged="GroupListBox_SelectedIndexChanged" 
        Font-Size="X-Small" SelectionMode="Multiple"></asp:ListBox>
 <asp:CheckBoxList ID="Display_List" runat="server"
    AutoPostBack="True" 
        BorderStyle="Ridge" CausesValidation="True" Font-Size="X-Small" 
        Width="169px" onselectedindexchanged="Display_List_SelectedIndexChanged" >
    </asp:CheckBoxList>

    <div id="HyperLinks" runat="server" ></div>
    </asp:Content>
    
