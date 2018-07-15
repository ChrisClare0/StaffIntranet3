<%@ Page Language="C#" AutoEventWireup="true"   MasterPageFile="~/styles/StudentInformation.Master"  CodeBehind="ExamModuleRetakes.aspx.cs" Inherits="StudentInformation.ExamModuleRetakes" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Student Information - Start</title>
    </asp:Content>


    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div id="content" runat="server">

        <asp:Label ID="Label_title" runat="server" Font-Names="Arial" Font-Size="Large" 
            Text="Please check the modules you wish to re-take. When complete, click 'Print'"></asp:Label>
    <br />
    <asp:Label ID="Label1" runat="server" Font-Names="Arial" 
        Text="Total Cost so far (£)   :  "></asp:Label>
    
    <asp:TextBox ID="TextBox_cost" runat="server" Font-Names="Arial" 
        Enabled="False"></asp:TextBox>
    <asp:Button ID="Button_submit" runat="server" Text="Print" Width="108px" 
        onclick="Button_submit_Click" Font-Names="Arial" Font-Size="Medium" />
    
    <asp:Table ID="Table1" runat="server" BorderStyle="Ridge">
    </asp:Table>
            <div id="servercontent" runat="server"></div>
            </div>


        </asp:Content>

