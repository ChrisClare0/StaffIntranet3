<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Exams.Master" AutoEventWireup="true" CodeBehind="BaseDataSetupRetakes.aspx.cs" Inherits="DCGS_Staff_Intranet2.Exams.BaseDataSetupRetakes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


<style type="text/css">
    .modal
    {
        position: fixed;
        top: 0;
        left: 0;
        background-color: black;
        z-index: 99;
        opacity: 0.8;
        filter: alpha(opacity=80);
        -moz-opacity: 0.8;
        min-height: 100%;
        width: 100%;
    }
    .loading
    {
        font-family: Arial;
        font-size: 10pt;
        border: 5px solid #67CFF5;
        width: 200px;
        height: 100px;
        display: none;
        position: fixed;
        background-color: White;
        z-index: 999;
    }
</style>


    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script type="text/javascript">
    function ShowProgress() {
        setTimeout(function () {
            var modal = $('<div />');
            modal.addClass("modal");
            $('body').append(modal);
            var loading = $(".loading");
            loading.show();
            var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
            var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
            loading.css({ top: top, left: left });
        }, 200);
    }
    $('form').live("submit", function () {
        ShowProgress();
    });
</script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

This routine will search the BaseData that has been uploaded to this server (via the Base Data Upload Option) 
    and upload those options to Cerval where students took them in the previous June season
    AND there is no coursework component.  Once this is done you will be given an option to allow retakes in those Options where couresework is required.
    <br />
    <br />    <asp:Button ID="Button_Run" runat="server" Text="Run Routine" OnClick="Button_Run_Click" /><br /><br />

    <asp:Label ID="Label_Result" runat="server" Text=""></asp:Label><br />
    <asp:Label ID="Label_Error" runat="server" Text=""></asp:Label> <br /><br /> 

    <asp:Label ID="LabelDropDown" runat="server" Text=""></asp:Label>
    <asp:DropDownList ID="DropDownList1" runat="server"  visible="false" ></asp:DropDownList><asp:Button ID="Button_Upload" runat="server" Text="Upload Selected" visible="false" OnClick="Button_Upload_Click"/>
    <br /><br />

    <div class="loading" align="center">
    Processing.... Please wait.<br />
    <br />
    <img src="../images/loading.gif" alt="" />
</div>

</asp:Content>

