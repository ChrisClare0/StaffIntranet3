<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamsSubjectList.aspx.cs" Inherits="DCGS_Staff_Intranet.StudentEntriesEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<HEAD>
		<title>Student Edit Exams Entries</title>
		<meta content="True" name="vs_showGrid"/>
</HEAD>
<body bgcolor="#e8e8e8">
    <form id="form2" runat="server">
        <div>
        <asp:Label ID="Label1" runat="server" Font-Names="Arial" Text="Year :" 
                Font-Size="Small"  Height="21px"></asp:Label>
        <asp:DropDownList ID="DropDownList_ExamYear" runat="server" 
                        DataSourceID="ExamsYears_sql" DataTextField="ExamYear" 
                        DataValueField="ExamYear" Font-Names="Arial" 
                        onselectedindexchanged="DropDownList_ExamYear_SelectedIndexChanged" 
                        AutoPostBack="True" style="margin-bottom: 0px" Height="23px" 
                Width="96px" Font-Size="Small">
                    </asp:DropDownList>
        &nbsp;
        <asp:Label ID="Label2" runat="server" Font-Names="Arial" Text="Season : " 
                Font-Size="Small" Height="21px"></asp:Label>
        <asp:DropDownList ID="DropDownList1" runat="server" 
                        onselectedindexchanged="DropDownList1_SelectedIndexChanged1" 
                        AutoPostBack="True" style="margin-bottom: 0px" Height="21px">
                        <asp:ListItem Value="1">January</asp:ListItem>
                        <asp:ListItem Value="3">March</asp:ListItem>
                         <asp:ListItem Value="5">May(Y9/10)</asp:ListItem>
                        <asp:ListItem Value="6">June</asp:ListItem>
                        <asp:ListItem Value="B">November</asp:ListItem>
                    </asp:DropDownList>
         &nbsp;
         <asp:Label ID="Label4" runat="server" Font-Names="Arial" Text="Level:" 
                Font-Size="Small"  Height="21px"></asp:Label>
            <asp:DropDownList ID="DropDownList3" runat="server" AutoPostBack="True" 
                onselectedindexchanged="DropDownList3_SelectedIndexChanged">
                <asp:ListItem>GCSE</asp:ListItem>
                <asp:ListItem Value="GCE">A-Level</asp:ListItem>
                <asp:ListItem Value="INTR">Internal</asp:ListItem>
                <asp:ListItem>FSMQ</asp:ListItem>
            </asp:DropDownList>
         <asp:Label ID="Label5" runat="server" Font-Names="Arial" Text="Type:" 
                Font-Size="Small" Height="21px" Width="32px"></asp:Label>
            <asp:DropDownList ID="DropDownList4" runat="server" AutoPostBack="True" 
                onselectedindexchanged="DropDownList4_SelectedIndexChanged">
                <asp:ListItem Value="C">Certification</asp:ListItem>
                <asp:ListItem Value="U">Module</asp:ListItem>
            </asp:DropDownList>
&nbsp; <asp:Label ID="Label3" runat="server" Font-Names="Arial" Text="Module : " 
                Font-Size="Small" Height="21px"></asp:Label>
         <asp:DropDownList ID="DropDownList2" runat="server" 
                        DataSourceID="ExamsEntries_Modules_sql" DataTextField="OptionTitle" 
                        DataValueField="OptionCode" 
                        onselectedindexchanged="DropDownList2_SelectedIndexChanged" 
                        AutoPostBack="True" ondatabound="DropDownList2_DataBound" 
                Height="20px" Width="254px">
          </asp:DropDownList>
          &nbsp;&nbsp;&nbsp;
          <asp:Button ID="Button_Excel" runat="server" OnClick="Button_Excel_Click" Text="Copy to Excel"
                        Width="107px" Height="21px" />
          <asp:Button ID="Button_ShowTimetable" runat="server" Height="21px" 
                        onclick="Button_ShowTimetable_Click" Text="Timetable" />
          <p align="center" >
                        <asp:Label ID="Label_Title" runat="server" Font-Bold="True" 
                        Font-Names="Arial" Font-Size="Large" Width="736px" Height="21px" ></asp:Label> 
                <p align="center">
			    <asp:GridView ID="GridView1" runat="server" AllowSorting="True"
            AutoGenerateColumns="False" CellPadding="4" DataKeyNames="ExamEntryID" DataSourceID="ExamsEntries_sql"
            Width="915px" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" 
                        Font-Bold="False" Font-Size="Medium" Height="188px" ForeColor="#333333" 
                        GridLines="None">
            <FooterStyle BackColor="#1C5E55" ForeColor="White" Font-Bold="True" />
            <Columns>
                <asp:BoundField DataField="ExamEntryID" HeaderText="Id" ReadOnly="True" SortExpression="ExamEntryID" >
                    <ItemStyle ForeColor= "LightGray" Font-Size="XX-Small" Wrap="True"  Width="100" />
                </asp:BoundField>
                <asp:BoundField DataField="StudentExamNumber" HeaderText="ExamNumber" SortExpression="StudentExamNumber" />
                <asp:BoundField DataField="PersonGivenName" HeaderText="GivenName" SortExpression="PersonGivenName" />
                <asp:BoundField DataField="PersonSurname" HeaderText="Surname" SortExpression="PersonSurname" />    
                <asp:BoundField DataField="OptionCode" HeaderText="Code" SortExpression="OptionCode" />
                <asp:BoundField DataField="OptionTitle" HeaderText="Title" SortExpression="OptionTitle" />
                <asp:BoundField DataField="DateEntered" HeaderText="Entered" SortExpression="DateEntered"  />
                
                <asp:BoundField DataField="PredictedGrade" HeaderText="PredictedGrade" 
                    SortExpression="PredictedGrade"  />
                <asp:ButtonField ButtonType="Button" CausesValidation="True" 
                    CommandName="Edit_Grade" Text="Edit" />
                
            </Columns>
            <RowStyle BackColor="#E3EAEB" />
            <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#7C6F57" />
                    <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
                    <br />
                    &nbsp;
                    </p>
                <p align=center style="width: 835px">
			        <asp:Label ID="Label_Title_for_Edit" runat="server" Font-Names="Arial" 
                        Text="Title for Edit"></asp:Label>
                </p>
                <p align=center style="width: 835px">
			        &nbsp;<asp:ListBox ID="ListBox_PredictedGrades" runat="server" Height="127px">
                    </asp:ListBox>
&nbsp;&nbsp;
                    <asp:Button ID="Button_Save_Prediction" runat="server" 
                        onclick="Button_Save_Prediction_Click" Text="Save New Prediction" 
                        Width="143px" />
&nbsp;<asp:Button ID="Button_Edit_Cancel" runat="server" onclick="Button_Edit_Cancel_Click" 
                        Text="Cancel" />
                    <asp:Label ID="Label_EntryID_for_edit" runat="server" 
                        Text="Hidden Label 4 Entry ID"></asp:Label>
                </p>

        <asp:SqlDataSource ID="ExamsEntries_sql" runat="server" ConnectionString=""
            
                    SelectCommand="SELECT  [ExamEntryID] , [PersonGivenName] , [PersonSurname] ,[StudentExamNumber],[OptionCode],[OptionTitle], [DateCreated], [DateEntered] [PredictedGrade] FROM [qry_Cerval_Exams_Entries]  WHERE (OptionCode ='4751' ) ORDER BY [PersonSurname] DESC" 
                    
                    
                    ProviderName="<%$ ConnectionStrings:CervalConnectionString.ProviderName %>">
        </asp:SqlDataSource>
                <asp:SqlDataSource ID="ExamsYears_sql" runat="server" 
                    ConnectionString="" 
                    SelectCommand="SELECT DISTINCT [ExamYear] FROM [qry_Cerval_Exams_Entries]">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="ExamsEntries_Modules_sql" runat="server" 
                    ConnectionString="" 
                    
                    SelectCommand="SELECT DISTINCT [OptionCode], [OptionTitle] FROM [qry_Cerval_Exams_Entries] ORDER BY [OptionTitle]" 
                    onselecting="ExamsEntries_Modules_sql_Selecting">
                </asp:SqlDataSource>
            <asp:Label ID="Label_OptionCode" runat="server" Text="Label" Visible="true" 
                    Width="75px"></asp:Label>
            <asp:Label ID="Label_ExamYear" runat="server" Text="Label" Visible="False" Width="67px"></asp:Label>&nbsp;
            <asp:Label ID="Label_ExamSeason" runat="server" Text="Label" Visible="False" Width="102px"></asp:Label>
                <asp:Label ID="Label_type" runat="server" Text="Label-type" Visible="False"></asp:Label>
                <asp:Label ID="Label_StudentID" runat="server" Text="Label-studentID" 
                    Visible="False"></asp:Label>
                <asp:Label ID="Label_name" runat="server" Text="Label-name" Visible="False"></asp:Label>
			    <asp:Label ID="Label_staffCode" runat="server" Text="StaffCode" Visible="False"></asp:Label>
			<br>
			<br>
			<br>
        </div>
    </form>
</body>

</html>
