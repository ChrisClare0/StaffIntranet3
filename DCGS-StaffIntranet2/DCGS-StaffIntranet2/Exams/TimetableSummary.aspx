<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="TimetableSummary.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.TimetableSummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Table ID="Table_Summary" runat="server">
        <asp:TableRow runat="server" >
            <asp:TableCell runat="server">Date</asp:TableCell>
            <asp:TableCell runat="server">Monday</asp:TableCell>
            <asp:TableCell runat="server">Tuesday</asp:TableCell>
            <asp:TableCell runat="server">Wednesday</asp:TableCell>
            <asp:TableCell runat="server">Thursday</asp:TableCell>
            <asp:TableCell runat="server">Friday</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br /><br />
    <asp:Label ID="Label_Date" runat="server" Text="" Font-Size="Larger"></asp:Label>
    <br />
    <asp:Panel ID="Panel_DayView" runat="server" Visible="false">
    <asp:GridView ID="GridView_DayView" runat="server" DataSourceID="SqlDataSource1" OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView_RowDataBound">
        <HeaderStyle BackColor="#BBBBBB" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="EditRow"
                         runat="server" 
                        CommandName="EditRow" 
                        CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" 
                        Text="Edit Start Time" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
        
        <asp:Label ID="Label_message" runat="server" Text="" Visible ="false" ></asp:Label>
        <asp:Button ID="Button1" runat="server" Text="Resolve clashes" OnClick="CheckClashes_Click"  /> <br /><br />

        <table class="EventsTable">
            <tr>
                <td></td>
                <td>AM</td>
                <td>PM</td>
            </tr>
            <tr>
                <td>Rooms</td>
                <td><asp:Button ID="Button_RoomsAM" runat="server" Text="View Rooms AM"  OnClick="Button_RoomsAM_Click"/> </td>
                <td><asp:Button ID="Button_RoomsPM" runat="server" Text="View Rooms PM"  OnClick="Button_RoomsPM_Click" /> </td>
            </tr>
            <tr>
                <td>Desks</td>
                <td><asp:Button ID="Button_DesksAM" runat="server" Text="View Desks AM"   OnClick="Button_DesksAM_Click"/> </td>
                <td><asp:Button ID="Button_DesksPM" runat="server" Text="View Desks PM"  OnClick="Button_DesksPM_Click"/> </td>
            </tr>
            <tr>
                <td>Detils</td>
                <td><asp:Button ID="Button_PupilsAM" runat="server" Text="View Student's Details AM" OnClick="Button_PupilsAM_Click" /> </td>

                <td><asp:Button ID="Button_PupilsPM" runat="server" Text="View Student's Details PM" OnClick="Button_PupilsPM_Click" /></td>

            </tr>

        </table>

    </asp:Panel>
    

    <br />
    

        


    <br />
    <asp:Label ID="Label_NewStartTime" runat="server" Text="Type New Start Time:" Visible="false"></asp:Label>
    <asp:TextBox ID="TextBox_NewStartTime" runat="server" Visible="false"></asp:TextBox>
    <asp:Button ID="ButtonNewStartTime" runat="server" Text="UpdateStartTime" Visible="false" OnClick="ButtonNewStartTime_Click" />


     <asp:Label ID="Label_EditComponentID" runat="server" Text="" Visible="false"></asp:Label>
    <asp:Label ID="Label_EditTime" runat="server" Text="" Visible="false"></asp:Label>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>

</asp:Content>
