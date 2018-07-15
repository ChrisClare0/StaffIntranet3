<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="BaseDataCreateDCGS.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.BaseDataCreateDCGS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Starting_information" runat="server">
        <h2>Information:</h2>
        This routine create a Base Data file for internal exams in a spreadsheet.<br />
        Create a list file of the required exams with the comumn headings as in the text box below. The first line has to be:<br />
        Subject / Option / Component / Component Code Date / Length / AM/PM<br />
        Then copy this information into the text box. The "Parse" button will check the validity of this information and if OK you can then "Create" a new
        BaseData file. This will also create the Option Codes (which will just be truncated versions of the Subjects).<br/> 
        If it is not valid, edit in the spreadsheet and re-paste.<br />
        Note that the Students see the Components Title on their student timetables.<br />
        Subject, Option and Component Titles can be up to 30 characters, but try to limit if possible.<br />
        Note that Length is in minutes, and Date should be in format dd/mm/yyyy.<br /><br />
         <asp:Button ID="Button_Start" runat="server" Text="Continue" Width="85px" OnClick="Button_Start_Click" />
    </div>   
    <asp:Label ID="Label1" runat="server" Text="Label" Visible="false"></asp:Label><br />
    <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"  Visible="false" Width="128px" />
    <asp:Button ID="Button_Upload" runat="server" Text="Upload" OnClick="Button_Upload_Click"  Visible="false" Width="128px" />
    <br />


    <asp:TextBox ID="TextBox1" runat="server" Height="122px" Width="810px" TextMode="MultiLine" Wrap="False"  ToolTip="Paste information in this format.... Core    Core     Maths Paper1    Ma1     02/01/2017      60     AM">Subject	Option	Component  Code 	Date	Length	AM/PM</asp:TextBox>


    <asp:Table ID="Table1" runat="server" Width="77px" CssClass="DataTable" Visible="false" >
    </asp:Table>


</asp:Content>
