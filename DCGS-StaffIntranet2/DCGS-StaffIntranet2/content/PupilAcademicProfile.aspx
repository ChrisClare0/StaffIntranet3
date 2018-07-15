<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="PupilAcademicProfile.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.PupilAcademicProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<link href="../styles/TableStyles.css" rel="stylesheet" type="text/css" />
     <!--[if(gte IE 5.5)&(lte IE 8)]>
    <script type ="text/jscript" src=" ../code/js/selectivizr-min.js"></script>
    <script type="text/jscript"  src="../code/js/DOMAssistantCompressed-2.8.js"></script>
    <![endif]-->
    	    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div id="Servercontentleft" runat="server" style="float:left; width:615px; " >
<div id="content0" runat="server" ></div>
<div class="tableAdditionalText">
<br />
Notes:<br />
The current grade is the last attainment profile grade converted to the appropriate external scale (At A-level this is A=120, at KS3 and 4 it is either old GCSE points 52=A etc or new points (1-9)). 
The Target grade is derived from the mean CATS score for KS3, from Yelis test for KS4, and from Mean GCSE score for KS5.
The colouring here is red if 0.5 grade below and bright red if 1 grade or more points below. Similarly green for +half a grade and bright green for + whole grade or more.
The Commitment grades are coloured as green for >3 and bright green for >4, and red <2 and bright red for <1.
The last exam marks are similarly coloured top 15% bright green, top30% green, bottom 30% red, bottom 15% bright red.
</div><br />
<div id="content1" runat="server" ></div>



</div>
</asp:Content>
