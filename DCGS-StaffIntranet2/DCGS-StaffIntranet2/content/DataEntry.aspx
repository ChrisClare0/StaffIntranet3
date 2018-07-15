<%@ Page Language="C#" MasterPageFile="~/styles/MasterPage1.master"  AutoEventWireup="true" CodeBehind="DataEntry.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.DataEntry" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderLeft" runat="server">

<div id="content0" runat="server" >

    <div id="content10" >
        <asp:Label ID="Label1" runat="server" Text="No sets found" Height="28px" Width="600px"></asp:Label>
        <br />
        <div id="content100" runat="server"></div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Submit" /><br />
        <br />
        <asp:Table ID="Table1" runat="server" BorderStyle="Solid" BorderWidth="2px" EnableViewState="False"
            Width="491px" BorderColor="Silver">
        </asp:Table>
        </div>
        <asp:TextBox ID="TextBox1" runat="server" Visible="False" Width="111px"></asp:TextBox>
        <asp:TextBox ID="TextBox2" runat="server" Visible="False" Width="80px"></asp:TextBox>
        <asp:TextBox ID="TextBox3" runat="server" Visible="False" Width="80px"></asp:TextBox>
        <asp:TextBox ID="TextBox4" runat="server" Visible="False" Width="80px"></asp:TextBox>
    <br />
    <asp:FileUpload ID="FileUpload_picker" runat="server" Visible="False" />
    <asp:Button ID="Button_FileUpload" runat="server" Text="Upload File" 
        Height="24px" onclick="Button_FileUpload_Click" style="margin-right: 0px" Visible="False"  />
    <br />
</div>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderRight" runat="server">

    <asp:DropDownList ID="DropDownList_Sets" runat="server" AutoPostBack="True" 
        Height="22px" onselectedindexchanged="DropDownList_Sets_SelectedIndexChanged" 
        Width="149px">
    </asp:DropDownList>
    <br /><br />
    <div id="Div1" runat="server" >
    If you wish to up load from a text file for this set click below:
    </div>
    <br /><br />
    <asp:Button ID="Button_UseFile" runat="server" Text="Use a file" 
        onclick="Button2_Click" Enabled="True" />

</asp:Content>

