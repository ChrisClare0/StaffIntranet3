<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="TimetableRooms.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.TimetableRooms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="Label_message" runat="server" Text="" Font-Size="Large" ></asp:Label><br />
    <asp:Label ID="Label_Date" runat="server" Text="" Font-Size="Large"  ></asp:Label><br />
    <asp:GridView ID="GridView_RoomSummary" runat="server" DataSourceID="SqlDataSource3" >
        <HeaderStyle BackColor="#BBBBBB" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="EditRow"
                         runat="server" 
                        CommandName="EditRow" 
                        CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" 
                        Text="Edit Row" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>

        </asp:GridView>


    <div id="left1" style="float:left; width:332px; " >

    <asp:Panel id="Panel_left1" HorizontalAlign="Left"  runat="server" Width="322px" >
    <asp:Label ID="Label_RoomsAllcation" runat="server" Text="" ></asp:Label>
    <asp:ListBox ID="ListBox_Rooms" runat="server" OnSelectedIndexChanged="ListBox_Rooms_SelectedIndexChanged"  
        DataSourceID="SqlDataSource2" AutoPostBack="True" DataTextField="Name" 
        DataValueField="Id" Height="426px" SelectionMode="Multiple" 
        Width="253px">

    </asp:ListBox>
    </asp:Panel>
    </div>

    <div id="right1" style="float:right; width:470px; height: 441px;" >
    <asp:Panel id="Panel_right1" HorizontalAlign="Left"  runat="server" style="margin-left: 0px" >
 
        <asp:Label ID="Label_Rules" runat="server" Text="Allocate Rooms first THEN choose rules (if any) THEN hit ASSIGN." ></asp:Label>
        <br />
        <asp:Label ID="Label_Rules1" runat="server" Text="Rule 1." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule1Source" runat="server" Width="120px" ></asp:DropDownList>
        <asp:Label ID="Label_Rules11" runat="server" Text="To......."></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule1Room" runat="server"  Width="100px" ></asp:DropDownList>        
        <asp:CheckBox ID="CheckBox1" runat="server" Checked="True" Text="Exclusive" TextAlign="Left" />
        <br />
        <asp:Label ID="Label_Rules2" runat="server" Text="Rule 2." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule2Source" runat="server"  Width="120px" ></asp:DropDownList>
        <asp:Label ID="Label_Rules21" runat="server" Text="To......." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule2Room" runat="server"  Width="100px"  ></asp:DropDownList>
        <asp:CheckBox ID="CheckBox2" runat="server" Checked="True" Text="Exclusive" TextAlign="Left" />
        <br />
        <asp:Label ID="Label_Rules3" runat="server" Text="Rule 3." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule3Source" runat="server"  Width="120px" ></asp:DropDownList>
        <asp:Label ID="Label_Rules31" runat="server" Text="To......." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule3Room" runat="server"  Width="100px"  ></asp:DropDownList>
        <asp:CheckBox ID="CheckBox3" runat="server" Checked="True" Text="Exclusive" TextAlign="Left" />
        <br />
        <asp:Label ID="Label_Rules4" runat="server" Text="Rule 4." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule4Source" runat="server"  Width="120px" ></asp:DropDownList>
        <asp:Label ID="Label2" runat="server" Text="To......." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule4Room" runat="server"  Width="100px"  ></asp:DropDownList>
        <asp:CheckBox ID="CheckBox4" runat="server" Checked="True" Text="Exclusive" TextAlign="Left" />
        <br />
        <asp:Label ID="Label_Rules5" runat="server" Text="Rule 5." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule5Source" runat="server"  Width="120px" ></asp:DropDownList>
        <asp:Label ID="Label3" runat="server" Text="To......." ></asp:Label>
        <asp:DropDownList ID="DropDownList_Rule5Room" runat="server"  Width="100px"  ></asp:DropDownList>
        <asp:CheckBox ID="CheckBox5" runat="server" Checked="True" Text="Exclusive" TextAlign="Left" />
        <br /><br />
        <asp:Button ID="Button_Assign" runat="server" Text="Assign Rooms" OnClick="Button_Assign_Click" />
                <br />
        <br />
        
        <br />
        <br />

        </asp:Panel>

    </div style="float:left">
            <div style="height: 169px">
            <br />
            <asp:Button ID="Button_GotoDesks" runat="server" OnClick="Button_GotoDesks_Click" Text="View Desks" />
        </div>
    <br /><br />
    <br /><br /><br /><br />

    <br /><br /><br /><br />

    <asp:SqlDataSource ID="SqlDataSource2" runat="server"></asp:SqlDataSource>
     <asp:SqlDataSource ID="SqlDataSource3" runat="server"></asp:SqlDataSource>
</asp:Content>
