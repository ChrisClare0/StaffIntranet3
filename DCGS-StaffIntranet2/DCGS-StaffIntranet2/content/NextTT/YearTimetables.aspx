<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="YearTimetables.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.NextTT.YearTimetables" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Year Timetables</title>
        <link href="../../styles/TableStyles.css" media="screen" rel="stylesheet" type="text/css" />
         <link href="../../styles/Styles1.css" media="screen" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
              <div id="HeaderDiv" runat="server" >
                   <div id="Information" runat="server"></div>
                 <asp:Button ID="ButtonGenerateTT" runat="server"  Text="Display" onclick="ButtonGenerateTT_Click"  />
         </div>
         <div id="content0" runat="server" ></div>
         <div>
                 <asp:ListBox ID="ListBox_year" runat="server" Height="235px" 
            SelectionMode="Multiple" Width="132px"></asp:ListBox>
            </div>
    </form>
</body>
</html>
