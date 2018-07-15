<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="PupilChoice.aspx.cs" Inherits="DCGS_Staff_Intranet2.PupilChoice" %>
<%@ Register assembly="DCGS-Staff-Intranet2" namespace="DCGS_Staff_Intranet2" tagprefix="cc1" %>
<%@ Register assembly="Cerval_Library" namespace="Cerval_Library" tagprefix="cc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/TableStyles.css" rel="stylesheet" type="text/css" />
    <!--[if(gte IE 5.5)&(lte IE 8)]>
    <script type ="text/jscript" src=" ../code/js/selectivizr-min.js"></script>
    <script type="text/jscript"  src="../code/js/DOMAssistantCompressed-2.8.js"></script>
    <![endif]-->

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
        
    <div id="Servercontentleft" runat="server" style="float:left; width:615px; " >
        <asp:TextBox ID="TextBox_Registrations" runat="server" TextMode="MultiLine" Visible="false" Height="65px" Width="601px"></asp:TextBox>
    <cc2:EmailStaffControl ID="EmailStaffControl1" runat="server" visible="false"/>
    <cc2:StudentDetailControl ID="StudentDetail1" runat="server"></cc2:StudentDetailControl>
    <cc2:TimetableControl ID="TimetableControl1" visible="false" runat="server" />
    <cc2:ResultGrid ID="ResultGrid1" runat="server" visible="false" StudentId="6dbfac38-d7fa-4e6d-901e-a1eabe63a6d1" />
    <cc2:IncidentControl ID="IncidentConrol1" visible="false" runat="server"   />
    <cc2:StudentDevelopmentControl ID="StudentDevelopmentControl1" visible="false" runat="server"   />
    <cc2:EditIncidentControl ID="e1" visible="false" runat="server"  />
    <cc2:EditStudentDevelopmentControl ID="e2" visible="false" runat="server"  />
    <cc2:MedicalDetailsControl ID="MedicalDetailsControl1" Visible="false" runat="server" />
    <cc2:ReportCommentControl ID = "ReportCommentControl1" Visible ="false" runat="server" />
    <asp:TextBox ID="TextBox_EditMedical" Visible="false" runat="server"  Height="73px" Width="609px" TextMode="MultiLine"></asp:TextBox><br />
    <asp:Button ID="Button_SaveMedical" runat="server" Visible="false"  Text="Save Medical Note" onclick="Button_SaveMedical_Click" />
    <asp:Button ID="Button_CreateNewIncident" runat="server" Visible="false"  Text="CreateNewIncident" onclick="Button_NewIncident_Click" />
    <asp:Button ID="Button_EditIncident" runat="server" Visible="false"  Text="EditIncident" onclick="Button_EditIncident_Click" />
    <asp:Button ID="Button_CreateNewStudentDevelopment" runat="server" Visible="false"  Text="CreateNewStudentDevelopment" onclick="Button_NewStudentDevelopment_Click" />
    <asp:Button ID="Button_EditStudentDevelopment" runat="server" Visible="false"  Text="EditStudentDevelopment" onclick="Button_EditStudentDevelopment_Click" />
</div>
<div>


</div>


<div id="ServerContentright" runat="server" style="float:right; width:170px; " >
<asp:Label ID="Label_Year" runat="server" Text="Year 7" Width="160px" style ="float:left "></asp:Label><br />
<asp:TextBox ID="TextBox_mask" runat="server" AutoPostBack="True" 
        ToolTip="Type a mask and press return" Visible="False" 
        ontextchanged="TextBox_mask_TextChanged"></asp:TextBox>
<asp:ListBox ID="NameList" runat="server" Height="158px" style ="float:left; " 
        Width="166px" AutoPostBack="True" 
        onselectedindexchanged="NameList_SelectedIndexChanged" Font-Size="X-Small"></asp:ListBox>
<asp:RadioButtonList ID="RadioButtonList_reports" runat="server"
    AutoPostBack="True" 
        BorderStyle="Ridge" CausesValidation="True" Font-Size="X-Small" 
        Width="169px" visible="false" 
        onselectedindexchanged="RadioButtonList_reports_SelectedIndexChanged">
        <asp:ListItem  Selected="True" Text="FURTCOMM" Value="2" >Report Comments</asp:ListItem>
        <asp:ListItem Text="PNTSIMPR" Value="1">Points for Improvement</asp:ListItem>
    </asp:RadioButtonList>        
    <asp:RadioButtonList ID="Display_List" runat="server"
    AutoPostBack="True" 
        BorderStyle="Ridge" CausesValidation="True" Font-Size="X-Small" 
        Width="169px" onselectedindexchanged="Display_List_SelectedIndexChanged" >
        <asp:ListItem Selected="True" >Details</asp:ListItem>
        <asp:ListItem>Timetable</asp:ListItem>
        <asp:ListItem>Module Results</asp:ListItem>
        <asp:ListItem>External Results</asp:ListItem>
        <asp:ListItem>Internal Results</asp:ListItem>
        <asp:ListItem>Academic Profile</asp:ListItem>
        <asp:ListItem>Incident Log</asp:ListItem>
        <asp:ListItem>StudentDevelopment Log</asp:ListItem>
        <asp:ListItem>Exams</asp:ListItem>
        <asp:ListItem>ReportComments</asp:ListItem>
        <asp:ListItem>Recent Registrations</asp:ListItem>
        <asp:ListItem Enabled="false" >Medical</asp:ListItem>
    </asp:RadioButtonList>    
</div>

</asp:Content>


