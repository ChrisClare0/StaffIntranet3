<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="EDICreateEntryFile.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.EDICreateEntryFile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label_message" runat="server" Text="" Font-Size="Large"></asp:Label>
    <br />
    <asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource1" OnRowCommand="GridView1_RowCommand"  >
        <HeaderStyle BackColor="#CCCCCC" />

        <Columns>
        <asp:TemplateField>
  <ItemTemplate>
    <asp:Button ID="Process" runat="server" 
      CommandName="Process" 
CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
      Text="Process" />
  </ItemTemplate> 
</asp:TemplateField>
            </Columns>

     </asp:GridView>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
</asp:Content>
