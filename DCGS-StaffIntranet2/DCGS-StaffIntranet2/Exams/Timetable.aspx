<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="Timetable.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.Timetable" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        
        <script language="javascript" type="text/javascript">
<!--
            function myFunction() {
                var x;
                if (confirm("Press a button!") == true) {
                    x = "You pressed OK!";
                } else {
                    x = "You pressed Cancel!";
                }
                document.getElementById("demo").innerHTML = x;
                PageMethods.Name();
            }

             function check2() {
     document.getElementById("<%=btncheck.ClientID%>").click(); 
             }

            function CallMethod() {
                PageMethods.ToUpper("hello","fred", OnSuccessCallback, OnFailureCallback);
            }

            function OnSuccessCallback(res) {
                alert('Success');
            }

            function OnFailureCallback() {
                alert('Error');
            }
            function OnSuccess1(res) {
                alert(res);
                PageMethods.UpdateTT("2017", "3", OnSuccessCallback, OnFailureCallback);
               
            }
            function CallInitialise() {
                if (confirm("Please confirm that you wish to delete the current Timetable for this Season") == true) {
                    alert('1');
                    PageMethods.InitializeTT("2017", "3", OnSuccess1, OnFailureCallback);
                    alert('end');
                } else {}
            }

            //-->
            </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <button onclick="  CallInitialise()  ">Initialise TimeTable</button>
    <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"  Visible="false"/>
    <asp:scriptmanager enablepagemethods="true" id="scpt" runat="server"> </asp:scriptmanager>
    <asp:Button ID="btncheck" runat="server" Text="Verify" OnClick="check1" />

    <p id="demo"></p>

    <br />
    <br />
    <asp:GridView ID="GridView1" runat="server"   DataSourceID ="SqlDataSource1" OnRowCommand="GridView1_RowCommand"  > 
        <HeaderStyle BackColor="#CCCCCC" />

        <Columns>
        <asp:TemplateField>
  <ItemTemplate>
    <asp:Button ID="EditDay" runat="server" 
      CommandName="EditDay" 
CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
      Text="Edit Day" />
  </ItemTemplate> 
</asp:TemplateField>
            </Columns>






    </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
</asp:Content>
