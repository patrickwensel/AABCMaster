using System;
using System.Collections.Generic;
using System.Linq;

using ds = DocuSign.eSign;

namespace Dymeng.DocuSign
{

    public class DocuSignException : Exception
    {
        public DocuSignException() : base() { }
        public DocuSignException(string message) : base(message) { }
        public DocuSignException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class AuthConfig
    {
        public string UserID { get; set; }
        public string OAuthBasePath { get; set; }
        public string IntegratorKey { get; set; }
        public string PrivateKeyPath { get; set; }
        public string Host { get; set; }
    }

    public class ClientConfig
    {
        public AuthConfig AuthConfig { get; set; }
        public string AccountID { get; set; }
        public string SignerID { get; set; }
        public string SignerName { get; set; }
        public string SignerEmail { get; set; }
        public string DocumentFilename { get; set; }
        public byte[] DocumentBytes { get; set; }
        public string ReturnURL { get; set; }
        public string EmailSubject { get; set; }
    }

    public class DocuSignClient
    {

        public AuthConfig AuthConfig { get; set; }


        private string AuthenticateJWT() {
            
            const int EXPIRE_HOURS = 1;
            
            if (AuthConfig == null) {
                throw new DocuSignException("Auth config not initialized");
            }

            string accountID = string.Empty;

            ds.Client.ApiClient apiClient = new ds.Client.ApiClient(AuthConfig.Host);
            apiClient.ConfigureJwtAuthorizationFlow(
                AuthConfig.IntegratorKey, AuthConfig.UserID, AuthConfig.OAuthBasePath,
                AuthConfig.PrivateKeyPath, EXPIRE_HOURS);
            
            ds.Api.AuthenticationApi authApi = new ds.Api.AuthenticationApi(apiClient.Configuration);
            ds.Model.LoginInformation loginInfo = authApi.Login();

            // find the default account for this user
            var account = loginInfo.LoginAccounts?.Where(x => x.IsDefault == "true").FirstOrDefault();

            if (account == null) {
                throw new DocuSignException("No accounts found or unable to determine default account");
            }

            return account.AccountId;
        }




        public Models.Envelope GetSignatureRedirect(ClientConfig config) {

            // verify auth info
            if (string.IsNullOrWhiteSpace(config.AccountID)) {
                AuthConfig = config.AuthConfig;
                config.AccountID = AuthenticateJWT();
            }

            ds.Model.EnvelopeDefinition def = new ds.Model.EnvelopeDefinition();
            def.EmailSubject = config.EmailSubject;
            
            // Add a document to the envelope
            ds.Model.Document doc = new ds.Model.Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(config.DocumentBytes);
            doc.Name = config.DocumentFilename;
            doc.DocumentId = "1";

            def.Documents = new List<ds.Model.Document>() { doc };

            // Add a recipient to sign the documeent
            ds.Model.Signer signer = new ds.Model.Signer();
            signer.Email = config.SignerEmail;
            signer.Name = config.SignerName;
            signer.RecipientId = "2";
            signer.ClientUserId = config.SignerID; // must set |clientUserId| to embed the recipient!

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer.Tabs = new ds.Model.Tabs();
            signer.Tabs.SignHereTabs = new List<ds.Model.SignHere>();
            ds.Model.SignHere signHere = new ds.Model.SignHere();
            signHere.DocumentId = "1";            
            signHere.RecipientId = "1";
            signHere.AnchorXOffset = "0";
            signHere.AnchorYOffset = "0";
            signHere.AnchorUnits = "inches";
            signHere.AnchorString = "SIGN_HERE_PLEASE";
            signer.Tabs.SignHereTabs.Add(signHere);

            def.Recipients = new ds.Model.Recipients();
            def.Recipients.Signers = new List<ds.Model.Signer>();
            def.Recipients.Signers.Add(signer);

            // set envelope status to "sent" to immediately send the signature request
            def.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            ds.Api.EnvelopesApi envelopesApi = new ds.Api.EnvelopesApi();
            ds.Model.EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(config.AccountID, def);

            ds.Model.RecipientViewRequest viewOptions = new ds.Model.RecipientViewRequest()
            {
                ReturnUrl = config.ReturnURL,
                ClientUserId = config.SignerID,  // must match clientUserId set in step #2!
                AuthenticationMethod = "password",
                UserName = def.Recipients.Signers[0].Name,
                Email = def.Recipients.Signers[0].Email
            };

            // create the recipient view (aka signing URL)
            ds.Model.ViewUrl recipientView = envelopesApi.CreateRecipientView(
                config.AccountID, 
                envelopeSummary.EnvelopeId, 
                viewOptions);

            return new Models.Envelope() {
                EnvelopeID = envelopeSummary.EnvelopeId,
                RedirectURL = recipientView.Url
            };
            
        }
        
        public bool GetDocumentAndStatus(string envelopeID, string folderPath) {

            var accountID = AuthenticateJWT();
            var envelopesApi = new ds.Api.EnvelopesApi();
            var envelope = envelopesApi.GetEnvelope(accountID, envelopeID);

            if (envelope.Status == "completed") {
                // Download the docs
                var docsList = envelopesApi.ListDocuments(accountID, envelopeID);
                int docCount = docsList.EnvelopeDocuments.Count;
                string filePath = null;
                System.IO.FileStream fs = null;

                for (int i = 0; i < docCount; i++) {
                    // GetDocument() API call returns a MemoryStream
                    System.IO.MemoryStream docStream = (System.IO.MemoryStream)envelopesApi.GetDocument(accountID, envelopeID, docsList.EnvelopeDocuments[i].DocumentId);
                    // save the document to local file system
                    System.IO.Directory.CreateDirectory(folderPath);
                    filePath = folderPath + "\\" + System.IO.Path.GetRandomFileName() + ".pdf";
                    fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                    docStream.Seek(0, System.IO.SeekOrigin.Begin);
                    docStream.CopyTo(fs);
                    fs.Close();
                    Console.WriteLine("Envelope Document {0} has been downloaded to:  {1}", i, filePath);
                }

                return true;
            } else {
                return false;
            }

        }










        //public string LegacyAuthRequest(
        //    string signerID, 
        //    string signerCommonName, 
        //    string signerEmail, 
        //    string documentFileName, 
        //    byte[] document,
        //    string returnUrl)
        //{
        //    string username = "info@dymeng.com";
        //    string password = "sdzfgwdfgsdfghs";
        //    string integratorKey = "f3b3522a-51d1-412d-8fc8-095994bf3c9e";

        //    // initialize client for desired environment (for production change to www)
        //    ds.Client.ApiClient apiClient = new ds.Client.ApiClient("https://demo.docusign.net/restapi");
        //    ds.Client.Configuration.Default.ApiClient = apiClient;

        //    // configure 'X-DocuSign-Authentication' header
        //    string authHeader = "{\"Username\":\"" + username + "\", \"Password\":\"" + password + "\", \"IntegratorKey\":\"" + integratorKey + "\"}";
        //    ds.Client.Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

        //    // we will retrieve this from the login API call
        //    string accountId = null;

        //    /////////////////////////////////////////////////////////////////
        //    // STEP 1: LOGIN API        
        //    /////////////////////////////////////////////////////////////////

        //    // login call is available in the authentication api 
        //    ds.Api.AuthenticationApi authApi = new ds.Api.AuthenticationApi();
        //    ds.Model.LoginInformation loginInfo = authApi.Login();

        //    // parse the first account ID that is returned (user might belong to multiple accounts)
        //    accountId = loginInfo.LoginAccounts[0].AccountId;

        //    // Update ApiClient with the new base url from login call
        //    string[] separatingStrings = { "/v2" };
        //    apiClient = new ds.Client.ApiClient(loginInfo.LoginAccounts[0].BaseUrl.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)[0]);


        //    /////////////////////////////////////////////////////////////////
        //    // Hopefully try to get embedded signing working here.       
        //    /////////////////////////////////////////////////////////////////

        //    ds.Model.EnvelopeDefinition envDef = new ds.Model.EnvelopeDefinition();
        //    envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc";

        //    // Add a document to the envelope
        //    ds.Model.Document doc = new ds.Model.Document();
        //    doc.DocumentBase64 = System.Convert.ToBase64String(document);
        //    doc.Name = documentFileName;
        //    doc.DocumentId = "1";

        //    envDef.Documents = new List<ds.Model.Document>();
        //    envDef.Documents.Add(doc);

        //    // Add a recipient to sign the documeent
        //    ds.Model.Signer signer = new ds.Model.Signer();
        //    signer.Email = signerEmail;
        //    signer.Name = signerCommonName;
        //    signer.RecipientId = "2";
        //    signer.ClientUserId = signerID; // must set |clientUserId| to embed the recipient!

        //    // Create a |SignHere| tab somewhere on the document for the recipient to sign
        //    signer.Tabs = new ds.Model.Tabs();
        //    signer.Tabs.SignHereTabs = new List<ds.Model.SignHere>();
        //    ds.Model.SignHere signHere = new ds.Model.SignHere();
        //    signHere.DocumentId = "1";
        //    signHere.PageNumber = "1";
        //    signHere.RecipientId = "1";
        //    signHere.XPosition = "100";
        //    signHere.YPosition = "80";
        //    signer.Tabs.SignHereTabs.Add(signHere);

        //    envDef.Recipients = new ds.Model.Recipients();
        //    envDef.Recipients.Signers = new List<ds.Model.Signer>();
        //    envDef.Recipients.Signers.Add(signer);

        //    // set envelope status to "sent" to immediately send the signature request
        //    envDef.Status = "sent";

        //    // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
        //    ds.Api.EnvelopesApi envelopesApi = new ds.Api.EnvelopesApi();
        //    ds.Model.EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

        //    ds.Model.RecipientViewRequest viewOptions = new ds.Model.RecipientViewRequest()
        //    {
        //        ReturnUrl = returnUrl + "/" + envelopeSummary.EnvelopeId,
        //        ClientUserId = signerID,  // must match clientUserId set in step #2!
        //        AuthenticationMethod = "password",
        //        UserName = envDef.Recipients.Signers[0].Name,
        //        Email = envDef.Recipients.Signers[0].Email
        //    };

        //    // create the recipient view (aka signing URL)
        //    ds.Model.ViewUrl recipientView = envelopesApi.CreateRecipientView(accountId, envelopeSummary.EnvelopeId, viewOptions);

        //    return recipientView.Url;
        //}







    }
}
