<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="ProcessCommand.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.ProcessCommand" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script language="javascript" type="text/javascript">
 function OpenProgressWindow(reqid)
 {
     window.showModalDialog("../Exams/ProcessCommandResultPage.aspx?RequestId=" + reqid
 ,"Progress  Window","dialogHeight:200px; dialogWidth:380px");
 }
 </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div>
 <asp:Button ID="btnLongRuningTask"  runat="server" Text="Long Runing Task" OnClick="btnLongRuningTask_Click"
 />
 </div>
</asp:Content>
