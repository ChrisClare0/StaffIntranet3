<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamWarnings.aspx.cs" Inherits="StudentInformation.ExamWarnings" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
 <link href="../styles/Style1.css" rel="stylesheet" title="compact" type="text/css" />
    <script language="javascript" type="text/javascript">
<!--
        function Pop1() {
            window.open('WarningtoCandidates.pdf', 'Document1');
            document.getElementById("Button4").disabled = true;
            document.getElementById("Button2").disabled = false;
        }
        function Pop2() {
            window.open('Informationforcandidates.pdf', 'Document2');
            document.getElementById("Button1").disabled = false;
            document.getElementById("Button2").disabled = true;
            document.getElementById("Checkbox1").visible = true;
            document.getElementById("Label1").visible = true;
        }
//-->
</script>
    <title>Exam Warning Notices</title>
    <style type="text/css">

        #Button4
        {
            width: 423px;
        }
        #Button2
        {
            width: 423px;
        }
        #Button1
        {
            width: 423px;
        }
    </style>
</head>
<body >
    <form id="form1" runat="server">
        <div id="wrapper">
        <div id="branding"><h2>&nbsp;Student Information</h2></div>
        <div id="content"><h2>
    Please open and read these two documents. They are the legal notices from JCQ regarding exams.
    Please then confirm that you have read them.</h2>


    <input id="Button4" type="button" value="Read the JCQ Document"  onclick="Pop1()"/><br />
            <br />
    <input id="Button2" disabled="disabled"   type="button" value="Read the information to Candidates sheet" onclick="Pop2()"/> 
    <br />
            <br />
    <input id="Button1" disabled="disabled"  runat="server"  type="submit" value="I have read and understand the JCQ notices"/>
    <p>
    </p>
        </div>
            </div>
    </form>

</body>
</html>
