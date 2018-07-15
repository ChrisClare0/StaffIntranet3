<%@ Page Title="" Language="C#" MasterPageFile="~/styles/StaffIntranet.Master" AutoEventWireup="true" CodeBehind="EditMessageGroup.aspx.cs" Inherits="DCGS_Staff_Intranet2.content.EditMessageGroup" %>
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
    	            cache: false,
    	            dataType: "jsonp",
    	            converters: { "json jsond": function(msg) { return msg.hasOwnProperty('d') ? msg.d : msg; } },
    	            statusCode: { 500: function() { alert("Server Error 500") }, 404: function() { alert("Page Not Found 404"); } },
    	            failure: function(msg) { alert(msg); }
    	        });
    	        if ($('#pupillist').is(":empty")) {
    	            $.ajax({
    	                url: "https://internal.challoners.com/x/staff2/service.asmx/GetStudentList",
    	                data: { year: JSON.stringify("7") },
    	                error: function(XHR, msg, OBJ) { alert("Failed to load StudentList " + OBJ); },
    	                success: function(msg) { $('#pupillist').html($.parseJSON(msg.d)); }
    	            });
    	        }
    	        
    	        LoadCurrentNames();

    	        function SelectYearList_onSelect() {
    	            $.ajax
    	            ({
    	                url: "https://internal.challoners.com/x/staff2/service.asmx/GetStudentList",
    	                data: { year: JSON.stringify($('#YearList ').val()) },
    	                error: function(XHR, msg, OBJ) { alert("Failed to load StudentList " + OBJ); },
    	                success: function(msg) { $('#pupillist').html($.parseJSON(msg.d)); }
    	            });
    	        }
    	        $('#YearList').bind('click.edit', SelectYearList_onSelect);
    	        $('#AddNames').bind('click.edit', AddNames_onClick);
    	        $('#Button_Clear').bind('click.edit', Button_ClearClick);
    	        $('#Button_RemoveNames').bind('click.edit', RemoveNamesClick);
    	        $('#Button_Save').bind('click.edit', Button_SaveClick);

    	        function RemoveNamesClick() { $("#selectedNames option:selected").remove(); }
    	        function AddNames_onClick() { $("#pupillist option:selected").each(function() { $("<option value=" + $(this).val() + ">" + $(this).text() + "</option>").appendTo("#selectedNames"); }); }
    	        function Button_ClearClick() { $("#selectedNames").html(""); }
    	        function Button_SaveClick() {Save()}
    	        
    	        function Save() {
    	            var theform = document.forms[0];
    	            var i=0;
    	            $("#selectedNames option").each(function() {i++;});
    	            if(i==0){alert("No names selected..");return;}
    	            theform.__EVENTDATA.value="Students:";//to pas data back to server...
    	            $("#selectedNames option").each(function() {theform.__EVENTDATA.value+=$(this).val()+","});   
    	            <%= PostBackStr_MakeGroup %>;
    	            LoadCurrentNames();
    	        }
    	        
    	        function LoadCurrentNames()
    	        {
    	        var g = getUrlVars()["GroupId"];
    	        $('#selectedNames').html("");
    	        $('#selectedNames').load('EditMessageGroup.aspx','Ajaxcall=2&GroupID='+g);
    	        g = getUrlVars()["GroupName"];
    	        $('#Heading').html("Current Members for "+g+":");
    	        
    	        }
                function getUrlVars()
                    {
                    var vars = [], hash;
                    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
                    for(var i = 0; i < hashes.length; i++)
                    {hash = hashes[i].split('=');vars.push(hash[0]);vars[hash[0]] = hash[1];}
                    return vars;
                    }

    	    })
	        </script>
    <style type="text/css">
        #Button_RemoveNames
        {
            width: 148px;
        }
        #Button_Clear
        {
            width: 86px;
        }
        #selectedNames {
            width: 292px;
        }
        #pupillist {
            width: 270px;
            height: 94px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">




<div  >
<input type="hidden"  runat="server" id = "GroupId" name="GroupId" />

<p id="Heading">Current Group members for </p>
<select id="selectedNames"  multiple='multiple' name="D2"   ></select>
<input  type="button" id="Button_RemoveNames" value="Remove Selected" />
<input  type="button" id="Button_Clear" value="Clear List" />
<input  type="button" id="Button_Save" value="Save" />
<br /><br />
Select Students to add to group:<br /><br />

<select id="YearList" name="Dyear" >
<option value="7">Year 7</option>
<option value="8">Year 8</option>
<option value="9">Year 9</option>
<option value="10">Year 10</option>
<option value="11">Year 11</option>
<option value="12">Year 12</option>
<option value="13">Year 13</option>
</select><br />
<select id="pupillist"  multiple='multiple' name="D1"></select>
<input  type="button" id="AddNames" value="Add Names" /><br />
</div>

<br />
</asp:Content>
