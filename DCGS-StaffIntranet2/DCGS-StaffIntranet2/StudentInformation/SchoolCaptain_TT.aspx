<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StudentInformation.Master" AutoEventWireup="true" CodeBehind="SchoolCaptain_TT.aspx.cs" Inherits="DCGS_Staff_Intranet2.StudentInformation.SchoolCaptain_TT" %>
<%@ Register assembly="Cerval_Library" namespace="Cerval_Library" tagprefix="cc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label_Year" runat="server" Text="Year 7" Width="160px" style ="float:left "></asp:Label><br />
        <asp:ListBox ID="YearList" runat="server" Height="40px" style ="float:left; margin-bottom: 0px;" 
        Width="40px" AutoPostBack="True"  Font-Size="X-Small" OnSelectedIndexChanged="YearList_SelectedIndexChanged" >
            <asp:ListItem>12</asp:ListItem>
            <asp:ListItem>13</asp:ListItem>
    </asp:ListBox>
    <asp:ListBox ID="NameList" runat="server" Height="158px" style ="float:left; " 
        Width="166px" AutoPostBack="True"  Font-Size="X-Small" OnSelectedIndexChanged="NameList_SelectedIndexChanged"></asp:ListBox>
    <cc2:TimetableControl ID="TimetableControl1" visible="false" runat="server" style ="float:left" />
</asp:Content>
