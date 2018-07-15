<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="CalendarForm.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.CalendarForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/TableStyles.css" rel="stylesheet" type="text/css" />
    <!--[if(gte IE 5.5)&(lte IE 8)]>
    <script type ="text/jscript" src=" ../code/js/selectivizr-min.js"></script>
    <script type="text/jscript"  src="../code/js/DOMAssistantCompressed-2.8.js"></script>
    <![endif]-->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
    
     <div id="Servercontentleft" runat="server" style="float:left; width:615px;  " >

         <div id="servercontent"  runat="server" style="float:left;" ></div>
        <asp:Calendar ID="Calendar1" runat="server" BackColor="White" style="float:left; " 
        BorderColor="Black" BorderStyle="Solid" CellSpacing="1" Font-Names="Verdana" 
        Font-Size="9pt" ForeColor="Black" Height="275px" NextPrevFormat="ShortMonth" 
        ondayrender="Calendar1_DayRender" 
        onselectionchanged="Calendar1_SelectionChanged" Width="612px" 
        onvisiblemonthchanged="Calendar1_VisibleMonthChanged">
        <SelectedDayStyle BackColor="#BBBBBB" ForeColor="White" />
        <TodayDayStyle BackColor="#DDDDDD" ForeColor="Blue" />
        <OtherMonthDayStyle ForeColor="#999999" />
        <DayStyle BackColor="#DDDDDD" />
        <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="White" />
        <DayHeaderStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" 
            Height="8pt" />
        <TitleStyle BackColor="#084c8d" BorderStyle="Solid" Font-Bold="True" 
            Font-Size="12pt" ForeColor="White" Height="12pt" />
    </asp:Calendar>
    <br />

    </div>
    
    <div id="ServerContentright" runat="server" style="float:right; width:170px; " >
    
        <asp:RadioButtonList ID="RadioButtonList2" runat="server" Width="120px" 
            style="margin-right: 0px" Font-Size="X-Small" BorderStyle="Ridge" AutoPostBack="True"
            onselectedindexchanged="RadioButtonList2_SelectedIndexChanged"  >
        <asp:ListItem Selected="True">Display</asp:ListItem>
        <asp:ListItem>Search</asp:ListItem>
    </asp:RadioButtonList>
        <asp:Label ID="Label3" runat="server" Text="Categories to Display" ></asp:Label>
        <asp:CheckBoxList ID="CheckBoxList_Categories" runat="server" Width="120px"
            style="margin-right: 0px" Font-Size="X-Small" BorderStyle="Ridge" 
            AutoPostBack="True" 
            onselectedindexchanged="CheckBoxList_Categories_SelectedIndexChanged">
        <asp:ListItem Selected="True">Events</asp:ListItem>
        <asp:ListItem>Routine</asp:ListItem>
        <asp:ListItem>Sports Fixtures</asp:ListItem>
        <asp:ListItem>Meeting</asp:ListItem>
        <asp:ListItem>Visit</asp:ListItem>
        <asp:ListItem>All</asp:ListItem>
        </asp:CheckBoxList>
        <div id="SearchStuff"  runat="server" visible="false" >
        <br /><asp:Label ID="Label1" runat="server" Text="Search String" ></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server" width="100px" ></asp:TextBox><br />
        <asp:Button ID="Button_SearchForward" runat="server"  
            Text="Search Forward" onclick="Button_SearchForward_Click" /><br /><br />
        <asp:Label ID="Label2" runat="server" Text="Months to search"></asp:Label>
        <asp:TextBox ID="TextBox_monthstosearch" runat="server"  Text ="6" width ="20px" ></asp:TextBox> <br />
        </div>  
        </div>
  
    
    
</asp:Content>
