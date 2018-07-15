<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="TimetableDetailEdit.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.TimetableDetailEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="Panel1" runat="server">
    <asp:Label ID="Label_Date" runat="server" Text="Label"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label3" runat="server" Text="Restrict list by Admission Number: "></asp:Label> 
        <asp:TextBox ID="TextBox_Adno" runat="server" Width="73px"></asp:TextBox>
        <asp:Button ID="Button_Restrict" runat="server" Text="Restrict List" OnClick="Button_Restrict_Click" />
         &nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button_Clear" runat="server" Text="Clear List"  OnClick="Button_Clear_Click" Width="66px"/>
                 &nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button_Return" runat="server" Text="Return to Summary" OnClick="Button_Return_Click"   />

    <asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource1" AllowSorting="True" OnRowDataBound="GridView1_RowDataBound"  OnRowCommand="GridView1_RowCommand"  >
        <HeaderStyle BackColor="#BBBBBB" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="EditRow"
                         runat="server" 
                        CommandName="xx2" 
                        CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" 
                        Text="Edit" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </asp:Panel>

    <asp:Panel ID="Panel2" runat="server" Visible="false">

        <asp:Label ID="Label_EditName" runat="server" Text="Name"></asp:Label>          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label1" runat="server" Text="Desk:"></asp:Label>
        <asp:TextBox ID="TextBox_Desk" runat="server" Width="116px"></asp:TextBox> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label2" runat="server" Text="Room:"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="DropDownList_Rooms" runat="server"
            DataSourceID="SqlDataSource2"  DataTextField="Name" 
        DataValueField="Id" 
            ></asp:DropDownList>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
         <asp:Label ID="Label4" runat="server" Text="Start Time:"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TextBox_StartTime" runat="server" Width="116px"></asp:TextBox> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        <asp:Button ID="Button_EditSave" runat="server" Text="Save" OnClick="Button_EditSave_Click" Width="84px" />&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button_Cancel" runat="server" Text="Cancel" Onclick="Button_Cancel_Click" Width="84px" />
        <br /> <br/>
        Note that there is no error checking here ! Make sure the desk exists and is free! 
    </asp:Panel>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server"></asp:SqlDataSource>
    </asp:Content>