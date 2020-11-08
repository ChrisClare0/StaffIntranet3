<%@ Page Title="" Language="C#" MasterPageFile="~/styles/Xmatrix.Master" AutoEventWireup="true" CodeBehind="UploadDFEKS2.aspx.cs" Inherits="DCGS_Staff_Intranet2.Xmatrix.UploadDFEKS2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
    <script src="http://canvasjs.com/assets/script/canvasjs.min.js"></script>
    <style>
        .loader {
  border: 16px solid #f3f3f3;
  --
  border-radius: 50%;
  border-top: 16px solid #3498db;
  width: 120px;
  height: 120px;
  -webkit-animation: spin 2s linear infinite; /* Safari */
  animation: spin 2s linear infinite;

}
          .center {
  position: absolute;
  left: 400px;
}
          @-webkit-keyframes spin {
  0% { -webkit-transform: rotate(0deg); }
  100% { -webkit-transform: rotate(360deg); }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

    </style>
    <script>
        //stuff to do the ajax call to get Google sheets
        var Data = []; var d1 = []; d1[0] = "test"; Data.push(d1);
        function test() {
            alert("clicked");
        var url = SetupURL("1JzmV2nyZY-uoBhjYSMDGYHg5TEuSw5VOc_WL7y8k-rg", 1, 1, 3, 0, "callback1", "ImportdFE")
        callGoogleScript(url);
        var ss = "";
        ss=PageMethods.Process("test");
        while (ss == "") {

            document.getElementById("Text1").textContent = "wait";
        }
            
        document.getElementById("Text1").textContent = "done";
        }

        var promiseresult="not yet";


        function Test2() {
            alert("in test2");
            promiseresult = PageMethods.Process("3");

        }


        function LoadData() {
            var s= document.getElementById("TextInput1").value;
            var url = SetupURL(s, 2, 1, 1, 0, "callbackload", "ImportdFE");
            callGoogleScript(url);
        }

    function SetupURL(SheetId, row, column, opt, data, callback, sheetname) {
        var url = "https://script.google.com/a/challoners.org/macros/s/AKfycbwoXgr32pNpjaLzN6suZfSTF-Fkxpr52flkYtX6os_wmO3MhnMT/exec?";
        url += "callback=" + callback;
        url += "&SheetName=" + sheetname;
        url += "&SheetId=" + SheetId;
        url += "&Row=" + row;
        url += "&Column=" + column;
        url += "&Option=" + opt;
        url += "&Data=" + data;
        
        //alert(url);
        return url;
    }

    function callbackload(e){
        //alert("in callback");
        if (e.Status == "1") {
            for (var j = 0; j < 2; j++) {
                //alert(e.Data[j]);
            }
            Data = e.Data;
            document.getElementById("TextInputx").value = Data[1][2];
            //alert("done in callback");
        }
        else { alert("api call failed"); }
        document.getElementById("Loader1").hidden = "hidden";
    }

    // Make an AJAX call to Google Script
    function callGoogleScript(url) {
        document.getElementById("Loader1").hidden = "";
        var request = $.ajax({
            crossDomain: true,
            url: url,
            method: "GET",
            dataType: "jsonp"
        });
    }

    function callback1(e) {
        alert("in callback");
        if (e.Status == "1") {
            for (var j = 0; j < 4; j++) {
                alert(e.Data[j]);
            }
        }
        else { alert("api call failed"); }
    }

</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"  >
           <div class="center">
            <div class="loader" id="Loader1" style="float:right"  hidden="hidden"></div>

        </div>

    <label for='TextInput1' style='position:absolute; top: 30px; left: 58px; width: 291px; height: 20px;'>ID of Google sheet DFE download</label>
    <input id='TextInput1' type='text' style='position:absolute; top: 30px; left: 355px; width: 346px; height: 20px;' value='1JzmV2nyZY-uoBhjYSMDGYHg5TEuSw5VOc_WL7y8k-rg' />

    <label for='TextInput2' style='position:absolute; top: 60px; left: 58px; width: 291px; height: 20px;'>Sheet Name to upload</label>
    <input id='TextInput2' type='text' style='position:absolute; top: 60px; left: 355px; width: 346px; height: 20px; ' value="ImportdFE" />
    
    <label for='TextInput3' style='position:absolute; top: 60px; left: 58px; width: 291px; height: 20px;'>Sheet Name to upload</label>
    <input id='TextInput3' type='text' style='position:absolute; top: 60px; left: 355px; width: 346px; height: 20px; ' value="ImportdFE" />
     <br />


    <input id='TextInputx' type='text' style='position:absolute; top: 355px; left: 157px; width: 346px; height: 20px;' />

    <input id="Button1" type="button" value="Load Data to check" onclick="LoadData()"  style='position:absolute; top: 98px; left: 59px;'/>&nbsp;


    <br /><br /><br /><br /><br />


    <br /><br />
    ><br /><br /><br />
    <input id="Button2" type="button" value="Testttttt" onclick="Test2()"  style='position:absolute; top: 261px; left: 53px;'/><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />
     <asp:scriptmanager enablepagemethods="true" id="scpt" runat="server"> </asp:scriptmanager>
</asp:Content>
