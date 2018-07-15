<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="CreateMessageGroup.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.CreateMessageGroup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../styles/TableStyles.css" rel="stylesheet" type="text/css" />
     <!--[if(gte IE 5.5)&(lte IE 8)]>
    <script type ="text/jscript" src=" ../code/js/selectivizr-min.js"></script>
    <script type="text/jscript"  src="../code/js/DOMAssistantCompressed-2.8.js"></script>
    <![endif]-->
    <script type ="text/jscript" src=" ../code/js/jquery-1.6.2.min.js"></script>
    
    	<script language="javascript" type="text/javascript">
    	    $(document).ready(function() {
    	        $.support.cors = true;
    	        $.ajaxSetup({
    	            type: "GET",
    	            contentType: "application/json; charset=utf-8",
    	            data: "{}",
    	            dataType: "jsonp",
    	            converters: { "json jsond": function(msg) { return msg.hasOwnProperty('d') ? msg.d : msg; } },
    	            statusCode: { 500: function() { alert("Server Error 500") }, 404: function() { alert("Page Not Found 404"); } },
    	            failure: function(msg) { alert(msg); }
    	        });
    	        $('#Button_MakeGroup').bind('click.edit', Button_MakeGroupClick);
    	        $('#Button_SelectGroup').bind('click.edit', Button_SelectGroupClick);
    	        $('#Button_DeleteGroup').bind('click.edit', Button_DeleteGroupClick);

    	        $('#Previous_Groups_List').load('CreateMessageGroup.aspx','Ajaxcall=1');

    	        
    	           function Button_SelectGroupClick()
    	           {
    	           var g = $('#Previous_Groups_List').val();
    	           var t = $('#Previous_Groups_List option:selected').text();
    	           window.location="editmessageGroup.aspx?GroupId="+g+"&GroupName='"+t+"'";;

    	           }
    	           
    	           function Button_DeleteGroupClick()
    	           {
    	           var g = $('#Previous_Groups_List').val();
    	           var t = $('#Previous_Groups_List option:selected').text();
    	           $('#Previous_Groups_List').load('CreateMessageGroup.aspx','Ajaxcall=3&GroupId='+g);
    	           }

    	    
    	    function Button_MakeGroupClick() {
    	            var theform = document.forms[0];
    	            var g = $('#Group_name').attr("value");
    	            if(g.length<3){alert("Group Name must be given (3-20 characters");return;}
    	            theform.__EVENTDATA.value+="%GroupName:";
    	            theform.__EVENTDATA.value+=$('#Group_name').attr("value");     
    	            <%= PostBackStr_MakeGroup %>;
    	        }
    	    })
	        </script>

    <style type="text/css">
        #Previous_Groups_List {
            width: 262px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<p>Please Select the Group to Edit:<br /></p>

<select id="Previous_Groups_List" name="PreviousGroup" >
</select>
<input  type="button" id="Button_SelectGroup" value="Edit Group Members" />&nbsp;
<input  type="button" id="Button_DeleteGroup" value="Delete Group" /><br /><br /><br />To create a new private group please enter a group name (3-30 characters) below:<br />

<input type="text" id="Group_name" maxlength="30"  />
<input  type="button" id="Button_MakeGroup" value="Make New Group"/>



</asp:Content>
