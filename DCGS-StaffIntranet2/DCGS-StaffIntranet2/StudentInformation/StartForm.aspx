<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/styles/StudentInformation.Master" CodeBehind="StartForm.aspx.cs" Inherits="StudentInformation.StartForm" %>


    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Student Information - Start</title>
    </asp:Content>


    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
       
        <div id="servercontent" runat="server"></div>
            <table style="width:50%;" runat="server" id="table1" visible="false">
                    <tr>
                        <td>CC's photos</td><td><a href="PhotoList.aspx?Dir=CC_PHOTOS">CC</a></td>
                    </tr>

            </table>

        </asp:Content>


