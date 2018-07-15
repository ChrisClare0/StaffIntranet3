<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="UploadEntriesByCourse.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.UploadEntriesByCourse" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:TextBox ID="TextBox3_Intro" runat="server"  Visible="false" TextMode="MultiLine" BorderStyle="None" Height="110px" Width="807px">This routine will enter all the students in the selected year
studying the selected course for the selected Options on the selected date.

The &quot;Test Entry&quot; Button will show how many (and which) students will beentered before you comit this.
</asp:TextBox><br />
    <asp:Label ID="Label1" runat="server" Text="Select Year:"></asp:Label> 
    &nbsp;&nbsp;&nbsp; 
    <asp:DropDownList ID="DropDownList_Year" runat="server" Height="20px" Width="82px" OnSelectedIndexChanged="DropDownList_Year_SelectedIndexChanged" >
        <asp:ListItem Value="13" Selected="True">Year 13</asp:ListItem>
        <asp:ListItem Value="12">Year 12</asp:ListItem>
        <asp:ListItem Value="11">Year 11</asp:ListItem>
        <asp:ListItem Value="10">Year 10</asp:ListItem>
        <asp:ListItem Value="9">Year 9</asp:ListItem>
        <asp:ListItem Value="8">Year 8</asp:ListItem>
        <asp:ListItem Value="7">Year 7</asp:ListItem>
    </asp:DropDownList>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label2" runat="server" Text="Select Course:"></asp:Label>
    &nbsp;&nbsp;&nbsp;
    <asp:DropDownList ID="DropDownList_Course" runat="server" Height="19px" Width="147px"  DataSourceID="SqlDataSource_cses" AutoPostBack="True" OnSelectedIndexChanged="DropDownList_Course_SelectedIndexChanged" ></asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
    <asp:Label ID="Label4" runat="server" Text="Date:"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp; <asp:TextBox ID="TextBox_Date" runat="server" Width="71px" AutoPostBack="True" OnTextChanged="TextBox_Date_TextChanged"></asp:TextBox>
   

    <br /><br />
    <asp:Label ID="Label3" runat="server" Text="Select Option"></asp:Label>(s)<br />
    <asp:ListBox ID="ListBox_Options" runat="server"   SelectionMode="Multiple" Height="192px" Width="394px"  DataSourceID="SqlDataSource_opts" AutoPostBack="True" OnSelectedIndexChanged="ListBox_Options_SelectedIndexChanged"  ></asp:ListBox>
    <asp:Label ID="Label5" runat="server" Text="Or Type code:"></asp:Label>
    &nbsp;
    <asp:TextBox ID="TextBox_FindOption" runat="server" Width="148px"></asp:TextBox>
    &nbsp;&nbsp;&nbsp;
    <asp:Button ID="Button_Opt_Find" runat="server" Text="Find" OnClick="Button_Opt_Find_Click" />
    <br /><br />

    <asp:Button ID="ButtonTest" runat="server" Text="Test Entry..."  Visible="false" OnClick="ButtonTest_Click"/>
    <br />
    <asp:TextBox ID="TextBox1" runat="server"  Visible="false" Width="798px"></asp:TextBox><br />
    <asp:TextBox ID="TextBox2" runat="server"  Visible="false" Width="333px" Height="147px" TextMode="MultiLine"></asp:TextBox><br />


        <asp:Button ID="Button_Enter" runat="server" Text="Proceed to Enter these.."  Visible="false" OnClick="Button_Enter_Click"/>
     <asp:SqlDataSource ID="SqlDataSource_cses" runat="server"></asp:SqlDataSource>

     <asp:SqlDataSource ID="SqlDataSource_opts" runat="server"></asp:SqlDataSource>

</asp:Content>
