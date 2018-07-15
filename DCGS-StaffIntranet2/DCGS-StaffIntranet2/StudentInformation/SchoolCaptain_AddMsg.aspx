<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StudentInformation.Master" AutoEventWireup="true" CodeBehind="SchoolCaptain_AddMsg.aspx.cs" Inherits="StudentInformation.SchoolCaptain_AddMsg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div id="servercontent" runat="server"></div>


        <div id="Servercontentleft" runat="server" style="float:left; width:769px; margin-left:30px " >
        Messages will be displayed on the next registration access. They remain visible 
        unless delivered for 1 week.<br />
        <br />
<asp:Label ID="Label_header2" runat="server" Text="Message:" Width="141px" Font-Names="Arial"></asp:Label>
(messages limited to 75 chars)
<br />
<asp:TextBox ID="TextBox1" runat="server" Height="24px" Width="601px" MaxLength="85"></asp:TextBox>
<br />

       <br /><br /> 

<asp:Label ID="Label_header1" runat="server" style="float:right; " Text="Sending to:" Width="173px"  Font-Names="Arial">
</asp:Label><br />
            <asp:Label ID="Label_MessageType" runat="server" Text="Group" Visible="False"></asp:Label>
<asp:TextBox ID="TextBox_textTo" runat="server" Height="192px" ReadOnly="True" TextMode="MultiLine" style="float:right; "
Width="173px" ontextchanged="TextBox_textTo_TextChanged"></asp:TextBox>

 
<div id="ServerContentright" runat="server" style="float:left; width:220px; margin-left:30px" >
    Select Recipients<br /><br />
<asp:Label ID="Label_YearList" runat="server" Text="Year:" Width="50px" Visible="False" style="margin-bottom: 0px" Font-Names="Arial"></asp:Label>
<asp:DropDownList ID="DropDownList_Year" runat="server" AutoPostBack="True" 
               onselectedindexchanged="DropDownList_Year_SelectedIndexChanged" 
        Height="25px" Width="80px">
               <asp:ListItem>7</asp:ListItem>
               <asp:ListItem>8</asp:ListItem>
               <asp:ListItem>9</asp:ListItem>
               <asp:ListItem>10</asp:ListItem>
               <asp:ListItem>11</asp:ListItem>
               <asp:ListItem>12</asp:ListItem>
               <asp:ListItem>13</asp:ListItem>
           </asp:DropDownList>

           <br />
    <br />
           <asp:Button ID="Button_groups" runat="server" onclick="Button_groups_Click" 
               Text="Select Individuals" Visible="True" Width="155px" />


    <br />

           <asp:ListBox ID="ListBox_staff" runat="server" Width="152px" 
               SelectionMode="Multiple" Height="140px"></asp:ListBox>

<br />
           <asp:Button ID="Button_Add" runat="server" onclick="Button_Add_Click" 
               Text="Add to Recipient List" Visible="True" Width="162px" />


           <asp:Button ID="Button_Clear" runat="server" 
               Text="Clear Recipient List" onclick="Button_Clear_Click" 
        Width="161px" />
<br />
</div>

<asp:Label ID="Label_header4" runat="server" Text="Message Valid from:" Width="231px" style="float:left; "
               Font-Names="Arial"></asp:Label>

<asp:Calendar ID="Calendar2" runat="server" BackColor="White" 
        BorderColor="#999999" CellPadding="4" style="float:left; "
        DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" 
        ForeColor="Black" Height="180px" Width="200px">
        <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
        <SelectorStyle BackColor="#CCCCCC" />
        <WeekendDayStyle BackColor="#FFFFCC" />
        <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
        <OtherMonthDayStyle ForeColor="#808080" />
        <NextPrevStyle VerticalAlign="Bottom" />
        <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" />
        <TitleStyle BackColor="#999999" BorderColor="Black" 
            Font-Bold="True" />
    </asp:Calendar>


<br /><p>
           <asp:Button ID="Button_SendMessage" runat="server"
               Text="Send Message" Visible="True" onclick="Button_SendMessage_Click" 
               Width="131px" />
</p>
    <br />
           
       <br />

       <asp:TextBox ID="TextBox_GroupList" runat="server" Height="60px" 
        Width="261px" Visible="False"></asp:TextBox>
       <asp:TextBox ID="TextBox_StudentList" runat="server" Height="60px" 
           Width="261px" Visible="False"></asp:TextBox>

    <br />
</div>



</asp:Content>
