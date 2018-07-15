<%@ Page Language="C#"  AutoEventWireup="true" CodeBehind="home_test.aspx.cs" Inherits="DCGS_Staff_Intranet2.home_test" %>

<!DOCTYPE html>

<html lang="en">

<head runat="server">
    <title>DCGS Staff Intranet Signin</title>
    <!--
    <meta name="google-signin-scope" content="profile email">
    <meta name="google-signin-client_id" content="1071688065998-c6mkpr28h0lu4ts5n1f2givml865t0jv.apps.googleusercontent.com">
    <script src="https://apis.google.com/js/platform.js" async defer></script>
        -->
        <!-- <link href="../admin/styles/default1.css" rel="stylesheet" type="text/css" />
    <style>
        .container1 {

	    background-color: #ffffff;
	    position:relative;
        align-items:center;
    margin: 0;
    font:bold;
    background: yellow;
    position: absolute;
    top: 140%;
    left: 50%;
    margin-right: -50%;
    transform: translate(-50%, -50%) }
    </style>
    -->
</head>
<body>
    <form id="form1" runat="server">

        <div id="container">
            <div id="banner">

            <!--
        <div align="center">


            <div class="g-signin2" data-onsuccess="onSignIn" data-theme="dark"></div>
        </div>
        <script>
            function onSignIn(googleUser) {

                var profile = googleUser.getBasicProfile();
                console.log("ID: " + profile.getId());
                console.log('Full Name: ' + profile.getName());
                console.log('Given Name: ' + profile.getGivenName());
                console.log('Family Name: ' + profile.getFamilyName());
                console.log("Image URL: " + profile.getImageUrl());
                console.log("Email: " + profile.getEmail());
                document.getElementById("demo").innerHTML = profile.getEmail();
                document.getElementById("Text1").value = profile.getName();
                // The ID token
                var id_token = googleUser.getAuthResponse().id_token;
                console.log("ID Token: " + id_token);
                window.location = "home_x.aspx?token=" + id_token;

            };
        </script>
        <a href="#" onclick="signOut();">Sign out</a>
        <br />
        <br />
        <script>
            function signOut() {
                var auth2 = gapi.auth2.getAuthInstance();
                auth2.signOut().then(function () {
                    console.log('User signed out.');
                });
            }
        </script>
        -->               
                <br /><br /><br /> <br /><br /><br />
            <div align="center" class="container1">
                <div align="center">
                    <br /><br />
                    <asp:Label ID="Label1" runat="server" Text="Click below to login using your Google (@challoners.org) account." Font-Bold="true"></asp:Label>
                    <br />
                    <br />

                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Intranet Login"  Font-Bold="true" align="centre" Height="40px" Width="174px" />

                </div>
            </div>
                            </div>
            </div>
              <br />
    </form>
</body>
</html>
