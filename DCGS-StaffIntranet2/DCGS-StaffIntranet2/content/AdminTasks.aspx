<%@ Page Title="" Language="C#" MasterPageFile="~/styles/MasterPage1.master" AutoEventWireup="true" CodeBehind="AdminTasks.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.AdminTasks" %>
<%@ Register assembly="Cerval_Library" namespace="Cerval_Library" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderLeft" runat="server">
<p style="font-size:small "  > 
<cc1:MedicalEditControl ID="MedicalEditControl1" runat="server" Visible="false" />
</p>
    <p style="font-size:small "  > 

<cc1:SENListControl ID="SENListControl1" visible ="false" runat="server" />
        <p>
        <asp:Button ID="Button_UpdateMedicalNotes" runat="server" 
            Text="Update Medical Notes" visible ="false" 
            onclick="Button_UpdateMedicalNotes_Click"/>
        <asp:Button ID="Button_CancelUpdateMedicalNotes" runat="server" 
            Text="Exit without updating"  Visible="false" 
            onclick="Button_CancelUpdateMedicalNotes_Click"/>
        
<cc1:StatsControl ID="StatsControl1" runat="server" Visible="true"  />

</p>
<div id="content0" runat="server" >
    <asp:Button ID="Button_complexemail" runat="server" 
        onclick="Button_complexemail_Click" Text="Complex Email" Visible="False" 
        Width="133px" />

    <div id="DCO_div"  runat="server" visible="false" >
        <a href="https://script.google.com/macros/s/AKfycbzluKuo-4rwv3q0d9-aFwqYeGNTh6FaLbogl-H21OkfzuOAuY0/exec">DCO Spy</a>
    </div>
        <asp:Button ID="Button_DCO" runat="server" 
         Text="DCO Spy" Visible="False" 
        Width="133px" OnClick="Button_DCO_Click" />
    <br />
    </div>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderRight" runat="server">
<asp:TextBox ID="TextBox_mask" runat="server" AutoPostBack="True" 
        ToolTip="Type a mask and press return" Visible="False" 
        ontextchanged="TextBox_mask_TextChanged" ></asp:TextBox>
<asp:RadioButtonList ID="Display_List" runat="server"
    AutoPostBack="True" 
        BorderStyle="Ridge" CausesValidation="True" Font-Size="X-Small" 
        Width="169px" onselectedindexchanged="Display_List_SelectedIndexChanged" >
        <asp:ListItem Selected="True">Population</asp:ListItem>
        <asp:ListItem >SEN</asp:ListItem>  
        <asp:ListItem >Sanctions</asp:ListItem> 
        <asp:ListItem >Permissions</asp:ListItem>
        <asp:ListItem >Medical</asp:ListItem>
        <asp:ListItem >StudentDevelopment</asp:ListItem>
        <asp:ListItem >AC1</asp:ListItem>
    </asp:RadioButtonList>
    
    <asp:RadioButtonList ID="RadioButtonList2" runat="server"
    AutoPostBack="True" visible = "false" 
        BorderStyle="Ridge" CausesValidation="True" Font-Size="X-Small" 
        Width="169px" onselectedindexchanged="RadioButtonList2_SelectedIndexChanged" >
        <asp:ListItem Selected="True">School</asp:ListItem>
        <asp:ListItem >ByYear</asp:ListItem> 
        <asp:ListItem >ByForm</asp:ListItem>
        <asp:ListItem >Not in Form Group</asp:ListItem>  
    </asp:RadioButtonList>
        <asp:RadioButtonList ID="RadioButtonList3" runat="server"
    AutoPostBack="True" visible = "false" 
        BorderStyle="Ridge" CausesValidation="True" Font-Size="X-Small" 
        Width="169px" onselectedindexchanged="RadioButtonList3_SelectedIndexChanged" >
        <asp:ListItem Value="1" Selected="True">ToLeave</asp:ListItem>
        <asp:ListItem Value="2" >MinorMedical</asp:ListItem>
        <asp:ListItem Value="3" >LocalVisits</asp:ListItem>
        <asp:ListItem Value="4" >Medical</asp:ListItem>
        <asp:ListItem Value="5" >Connextions</asp:ListItem>
    </asp:RadioButtonList>
</asp:Content>
