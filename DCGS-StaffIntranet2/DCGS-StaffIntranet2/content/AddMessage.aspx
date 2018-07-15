<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="AddMessage.aspx.cs" Inherits="DCGS_Staff_Intranet2.AddMessage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Servercontentleft" runat="server" style="float:left; width:615px; " >
        Messages will be displayed on the next registration access. They remain visible 
        unless cleared by staff for 1 week.<br />
        <br />
<asp:Label ID="Label_header2" runat="server" Text="Message:" Width="141px" Font-Names="Arial"></asp:Label>
(messages limited to 75 chars)
<br />
<asp:TextBox ID="TextBox1" runat="server" Height="24px" Width="601px" MaxLength="75"></asp:TextBox>
<br />

<asp:Label ID="Label3" runat="server" Text="Add Link URL:" Width="110px" style="float:left; " Font-Names="Arial" Font-Size="X-Small"></asp:Label>
<asp:TextBox ID="TextBox_URL" runat="server" Height="24px" Width="490px" style="float:right; " Font-Size="X-Small" MaxLength="75">  </asp:TextBox>
       <br /><br /> 
        <asp:Label ID="Label_header4" runat="server" Text="Message Valid from:" Width="231px" style="float:left; "
               Font-Names="Arial"></asp:Label>
               <asp:Label ID="Label_header1" runat="server" style="float:right; " Text="Send to:" Width="133px" 
               Font-Names="Arial"></asp:Label>
           <br />

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

           <asp:TextBox ID="TextBox_textTo" runat="server" Height="192px" 
            ReadOnly="True" TextMode="MultiLine" style="float:right; "
               Width="173px" ontextchanged="TextBox_textTo_TextChanged"></asp:TextBox>
<br /><br /><br /><br /><br /><br /><br /><br />
<br /><p>
           <asp:Button ID="Button_SendMessage" runat="server"
               Text="Send Message" Visible="True" onclick="Button_SendMessage_Click" 
               Width="131px" />
</p>

    <br />
           <asp:Label ID="Label_MessageType" runat="server" Text="Group" 
        Visible="False"></asp:Label>
       <br />

       <asp:TextBox ID="TextBox_GroupList" runat="server" Height="60px" 
        Width="261px" Visible="False"></asp:TextBox>
       <asp:TextBox ID="TextBox_StudentList" runat="server" Height="60px" 
           Width="261px" Visible="False"></asp:TextBox>

    <br />
</div>

<div id="servercontent" runat="server"></div>

<div id="ServerContentright" runat="server" style="float:right; width:170px; " >
    Select Recipients<br /><br />
<asp:Label ID="Label_YearList" runat="server" Text="Year:" Width="50px" Visible="False" style="margin-bottom: 0px" Font-Names="Arial"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;
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
               <asp:ListItem>My Groups</asp:ListItem>
               <asp:ListItem Value="14">Other</asp:ListItem>
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

</asp:Content>

