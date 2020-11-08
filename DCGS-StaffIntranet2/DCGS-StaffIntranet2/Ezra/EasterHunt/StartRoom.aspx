<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartRoom.aspx.cs" Inherits="DCGS_Staff_Intranet2.Ezra.EasterHunt.StartRoom" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../StyleSheet1.css" media="screen" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #TextArea1 {
            width: 978px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="bannerEz" class="test1"><br /><br /><br /><br /><br /><br /><br />So Ezra,<br />
        <br />
        This is my attmept at doing an on-line, interactive Easter Egg Hunt/Treasure Hunt.&nbsp; It has taken me a while to do it, and so this first one is quite simple to make sure it works.<br />
        <br />
        The idea is like a normal treasure hunt, where you will have to find clues and each clue will point to the next one. When you think you know where it is, hover the mouse over that part of the picture and if you are right a code ( a number like 245) will appear, which you have to type in before you can look for the next clue.<br />
        <br />
        The first clue is &quot;<span style="color: rgb(0, 0, 0); font-family: &quot;Comic Sans MS&quot;; font-size: x-large; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; text-decoration-style: initial; text-decoration-color: initial; display: inline !important; float: none;">Inside Pop&#39;s present&quot;. You will start in the room where this is to make it easy for the first one... but who knows where the rest are !!!&nbsp;&nbsp; GOOD LUCK<br />
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="START"  CssClass="test1" />


        <br /><br /><br />
         <asp:Button ID="Button2" runat="server" Text="RESET"  CssClass="test1" OnClick="Button2_Click" />

        </span>
    </div>
    </form>
</body>
</html>
