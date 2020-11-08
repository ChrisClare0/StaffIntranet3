<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Xmatrix.Master" AutoEventWireup="true" CodeBehind="CreateNewModel.aspx.cs" Inherits="DCGS_Staff_Intranet2.Xmatrix.CreatNewModel " %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">



    <style type="text/css">
        .auto-style1 {
            height: 26px;
        }
        .auto-style2 {
            height: 29px;
        }
        .auto-style3 {
            width: 99%;
        }
    </style>



</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="auto-style3">
        <tr>
            <td class="auto-style2"><asp:Label ID="Label3" runat="server" Text="Label">Give a name for this model</asp:Label>:</td>
            <td class="auto-style2"><asp:TextBox ID="TextBox_ModelName" runat="server" Width="462px"  Height="16px"></asp:TextBox></td>
        </tr>
        <tr>
             <td><asp:Label ID="Label5" runat="server" Text="Label">Give a full description</asp:Label>:</td>
            <td><asp:TextBox ID="TextBox_Description" runat="server" Width="466px"  Height="52px" TextMode="MultiLine"></asp:TextBox></td>

        </tr>
        <tr>
            <td > <asp:Label ID="Label1" runat="server" Text="Label">Select VA Method</asp:Label>:</td>
            <td> <asp:DropDownList ID="DropDownList_VAmethods" runat="server" Width="469px" Height="45px" OnSelectedIndexChanged="DropDownList_VAmethods_SelectedIndexChanged" AutoPostBack="True" ></asp:DropDownList></td>
        </tr>
        <tr>
            <td class="auto-style1">Select Cohort for this:</td>
            <td class="auto-style1"><asp:DropDownList ID="DropDownList_Cohort" runat="server" Height="45px" Width="470px" OnSelectedIndexChanged="DropDownList_Cohort_SelectedIndexChanged" >
                <asp:ListItem Value="1">one</asp:ListItem>
                <asp:ListItem>two</asp:ListItem>
                </asp:DropDownList>

            </td>
        </tr>
        <tr>
            <td>Select Aggregation Method:</td>
            <td>
                <asp:DropDownList ID="DropDownList_AggregationMethods" runat="server"  Height="23px" Width="470px"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>Enable for Display:</td>
            <td>
                <asp:RadioButtonList ID="RadioButtonList_Display" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True">False</asp:ListItem>
                    <asp:ListItem>True</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td></td>
            <td><asp:Button ID="Button_Create" runat="server" Text="Create Implementation" OnClick="Button_Create_Click" PostBackUrl="~/Xmatrix/CreateNewModel.aspx" />
                </td>
        </tr>
        <tr>
            <td>

            </td>
            <td>
                <asp:Button ID="Button_Calculate" runat="server" Text="Calculate VA for Model" OnClick="OK_Click2" PostBackUrl="~/Xmatrix/CreateNewModel.aspx" />
            </td>
        </tr>
    </table>
    <br />
     <br />

    <asp:Panel ID="AlertBox" runat="server" Visible="false" BorderWidth="5px" BorderStyle="Solid">
           <table style="width: 80%;">
               <tr><td><asp:TextBox ID="AlertText" runat="server" Width="737px" BorderStyle="None" Height="29px" ></asp:TextBox></td> </tr>
       <tr><td></td> <td> <asp:Button ID="OK" runat="server" Text="OK" CssClass="center"  OnClick="OK_Click"  /></td> <td></td></tr>
              </table>
    </asp:Panel>
   <br />
    <asp:Chart ID="Chart2" runat="server">
        <Series>
            <asp:Series Name="Series1">
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1">
            </asp:ChartArea>
        </ChartAreas>
    </asp:Chart>
    <br /><br />
</asp:Content>

