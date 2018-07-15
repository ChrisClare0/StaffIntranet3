<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UtilitySanctionsNotCompleted.aspx.cs" Inherits="DCGS_Staff_Intranet.DataUtilities.SanctionsNotCompleted" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body bgcolor=#e8e8e8>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Arial" Text="Sanctions Outstanding"
            Width="466px"></asp:Label>
        <br />
        <br />
        <asp:GridView ID="GridView1" runat="server" BackColor="White" BorderColor="#DEDFDE"
            BorderStyle="None" BorderWidth="1px" CellPadding="4" DataSourceID="SqlDataSource1"
            Height="161px" Width="989px" AllowPaging="True" AllowSorting="True" 
            AutoGenerateColumns="False" ForeColor="Black" GridLines="Vertical" 
            onselectedindexchanged="GridView1_SelectedIndexChanged">
            <FooterStyle BackColor="#CCCC99" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <Columns>
                <asp:BoundField DataField="SanctionId" ReadOnly="True" 
                    SortExpression="SanctionId" Visible="False">
                    <ItemStyle Font-Size="XX-Small" Width="1px" />
                </asp:BoundField>
                <asp:BoundField DataField="PersonGivenName" SortExpression="PersonGivenName" 
                    HeaderText="FirstName">
                    <ItemStyle Width="20px" />
                </asp:BoundField>
                <asp:BoundField DataField="PersonSurname" HeaderText="Surname" 
                    SortExpression="PersonSurname" />
                <asp:BoundField DataField="SanctionName" HeaderText="Sanction" 
                    SortExpression="SanctionName" />
                <asp:BoundField DataField="SanctionDate" HeaderText="Date" 
                    SortExpression="SanctionDate">
                    <ItemStyle Font-Size="Small" />
                </asp:BoundField>
                <asp:BoundField DataField="IncidentText" HeaderText="Incident" 
                    SortExpression="IncidentText" />
                <asp:BoundField DataField="WorkSet" HeaderText="WorkSet" 
                    SortExpression="WorkSet" />
                <asp:BoundField DataField="GroupCode" HeaderText="Form" 
                    SortExpression="GroupCode" />
                <asp:BoundField DataField="StaffCode" HeaderText="Staff" 
                    SortExpression="StaffCode" />
                <asp:BoundField DataField="Count" HeaderText="Count" SortExpression="Count" />
                <asp:ButtonField ButtonType="Button" CommandName="Complete" HeaderText="Done" 
                    Text="Done" />
                <asp:ButtonField ButtonType="Button" CommandName="ReSchedule" HeaderText="Done" 
                    Text="Delay 1wk" />
                <asp:BoundField DataField="SanctionId" HeaderText="SanctionId" ReadOnly="True" 
                    SortExpression="SanctionId">
                    <ItemStyle Font-Size="XX-Small" ForeColor="#FFFF99" />
                </asp:BoundField>
            </Columns>
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
        &nbsp;
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString=""
            
            SelectCommand="SELECT [StudentAdmissionNumber], [PersonGivenName], [PersonSurname], [SanctionName], [SanctionDate], [IncidentDate], [SanctionId], [IncidentText], [WorkSet], [GroupCode], [StaffCode], [Count] FROM [cc_development_SanctionsNotCompleted]" 
            onselecting="SqlDataSource1_Selecting">
        </asp:SqlDataSource>
    
    </div>
    </form>
</body>
</html>
