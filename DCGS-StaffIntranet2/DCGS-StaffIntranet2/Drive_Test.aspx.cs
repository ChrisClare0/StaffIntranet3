using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Google.Apis.Auth.OAuth2;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Services;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Auth.OAuth2.Flows;
using System.IO;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading;

namespace DCGS_Staff_Intranet2
{
    public partial class Drive_Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var serviceAccountEmail = "cc@challoners.org";
                var certificate = new X509Certificate2(@"D:\users\chris\testcert.cer", "notasecret", X509KeyStorageFlags.Exportable);


                IAuthorizationCodeFlow flow =
    new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
    {
        ClientSecrets = GetClientConfiguration().Secrets,
        Scopes = new string[] { DriveService.Scope.DriveFile }
    });


                //TokenResponse response = flow.ExchangeCodeForTokenAsync("", code, "postmessage", CancellationToken.None).Result; // response.accessToken now should be populated

              

                UserCredential credential1 = new UserCredential(null, "cc@challoners.org", null);
                ServiceAccountCredential credential = new ServiceAccountCredential(
                           new ServiceAccountCredential.Initializer(serviceAccountEmail)
                           {
                               Scopes = new[] { DriveService.Scope.DriveReadonly }
                           }.FromCertificate(certificate));

                // Create the service.

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential1,
                    ApplicationName = "Drive API Sample",
                });

                FilesResource.ListRequest request = service.Files.List();
                FileList files = request.Execute();
                string s = "";
                foreach (Google.Apis.Drive.v2.Data.File item in files.Items)
                {
                    s = item.Title;
                    s = s;
                }
            }

        }
        /// <summary>Retrieves the Client Configuration from the server path.</summary>
        /// <returns>Client secrets that can be used for API calls.</returns>
        public static GoogleClientSecrets GetClientConfiguration()
        {
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                return GoogleClientSecrets.Load(stream);
            }
        }

        /// <summary>
        /// Gets a Google+ service object for making authorized API calls to Google+ endpoints.
        /// </summary>
        /// <param name="credentials">The OAuth 2 credentials to use to authorize the client.</param>
        /// <returns>
        /// A <see cref="PlusService">PlusService</see>"/> object for API calls authorized to the
        /// user who corresponds with the credentials.
        /// </returns>
        public static DriveService GetDriveService(TokenResponse credentials)
        {
            IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = GetClientConfiguration().Secrets,
                    Scopes = new string[] { DriveService.Scope.DriveFile }
                });

            UserCredential credential = new UserCredential(flow, "me", credentials);

            return new DriveService(
                new Google.Apis.Services.BaseClientService.Initializer()
                {
                    ApplicationName = "Haikunamatata",
                    HttpClientInitializer = credential
                });
        }
    }
}