

<%@ Page Title="EntriesView" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="EntriesView.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.EntriesView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel ID="Panel1" runat="server">


    <asp:Label ID="Label_Select1" runat="server" Text="Select Year:"></asp:Label>
    <asp:DropDownList ID="DropDownList1" runat="server" Height="16px" Width="280px" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label_Select3" runat="server" Text="Or Type Admission Number:"></asp:Label>
    <asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox><br />
    <asp:ListBox ID="ListBox1" runat="server" AutoPostBack="True" Width="228px" Visible ="true" Height="114px" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged" style="margin-right: 28px"></asp:ListBox><br />

    <asp:Label ID="Label_Grid" runat="server" Text=""></asp:Label> <br /><br />
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
        AllowSorting="True" DataSourceID="SqlDataSource1" OnPageIndexChanged="GridView1_PageIndexChanged" 
        OnPageIndexChanging="GridView1_PageIndexChanging" OnSelectedIndexChanging="GridView1_SelectedIndexChanging" 
        OnSorting="GridView1_Sorting"  
        CssClass="ExamsTbl" OnSelectedIndexChanged="GridView1_SelectedIndexChanged"  >
        <HeaderStyle BackColor="#CCCCCC" />

     </asp:GridView>

    <asp:GridView ID="GridView2" runat="server" AllowPaging="True" 
        AllowSorting="True" DataSourceID="SqlDataSource2" 
        OnPageIndexChanging="GridView2_PageIndexChanging" 
        OnSorting="GridView2_Sorting" AutoGenerateColumns="false" 
        CssClass="ExamsTbl" OnRowCommand="GridView2_RowCommand" >
        <HeaderStyle BackColor="#CCCCCC" />
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID"  HeaderStyle-HorizontalAlign="Left"   />
                <asp:BoundField DataField="OptionCode" HeaderText="Option Code"  HeaderStyle-HorizontalAlign="Left" SortExpression="OptionCode"   />
                <asp:BoundField DataField="Title" HeaderText="Title"  HeaderStyle-HorizontalAlign="Left"  SortExpression="Title" />
                <asp:BoundField DataField="Syllabus" HeaderText="Syllabus"  HeaderStyle-HorizontalAlign="Left"   SortExpression="Syllabus" />
            <asp:CheckBoxField DataField="Withdrawn" HeaderText="Withdranw" SortExpression="Withdrawn"  />
                <asp:BoundField DataField="Status" HeaderText="Status"  HeaderStyle-HorizontalAlign="Left"  SortExpression="Status" />

                <asp:TemplateField >
                    <ItemTemplate>
                        <asp:Button ID="btnEditRow" 
                            runat="server" 
                            CommandName="Select" 
                            CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" 
                            Text="Select" 
                             />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

     </asp:GridView>

    <asp:Button ID="Button_Delete" runat="server" Text="Delete Selected" visible ="false"  OnClick="Button_Delete_Click"/>
        <br />
        </asp:Panel>
    <asp:Panel ID="Panel2" runat="server" Visible="false" BackColor="#FF9966" BorderStyle="Inset" Width="557px">
        <asp:TextBox ID="TextBox2" runat="server" Text="Are you sure you want to delete?" Width="275px"></asp:TextBox>
        <br />
        <asp:Button ID="Button_Cancel" runat="server" Text="Cancel" OnClick="Button_Cancel_Click" Width="91px" />
        <asp:Button ID="Button_OK" runat="server" Text="OK" OnClick="Button_OK_Click" Width="85px" />
    </asp:Panel>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" DataSourceMode ="DataSet" ></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" DataSourceMode ="DataSet" ></asp:SqlDataSource>

</asp:Content>


