using Microsoft.PowerBI.Api.Beta;
using Microsoft.PowerBI.Api.Beta.Models;
using Microsoft.PowerBI.Security;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FBIUCRDemo.BusinessLogic
{
    public class PowerBIHelper
    {
        public static void UploadReports()
        {
            try
            {
                string workspaceCollection = ConfigurationManager.AppSettings["powerbiWorkspaceCollection"];
                string accessKey = ConfigurationManager.AppSettings["powerbiAccessKey"];
                string apiUrl = ConfigurationManager.AppSettings["powerbiApiUrl"];

                var workspace = PowerBIHelper.CreateWorkspace(workspaceCollection, accessKey, apiUrl);
                string workspaceId = workspace.WorkspaceId;
                System.Web.HttpContext.Current.Application["WorkspaceID"] = workspace.WorkspaceId;

                string reportsPath = HttpContext.Current.Server.MapPath("~/ReportFiles");
                
                var files = Directory.GetFiles(reportsPath);
                foreach (string file in files)
                {
                    var temp = file.Substring(file.LastIndexOf('\\') + 1);
                    var datasetName = file.ToLower().Contains("dashboard") ? "UCRDashboard" : temp.Substring(0, temp.Length - 5);
                    var filePath = file;
                    var import = ImportPbix(workspaceCollection, workspaceId, datasetName, filePath, accessKey, apiUrl);
                }
            }
            catch (Exception ex)
            {
                StorageClient.LogError(ex);
            }
        }

        static Workspace CreateWorkspace(string workspaceCollectionName, string accessKey, string apiUrl)
        {
            // Create a provision token required to create a new workspace within your collection
            var provisionToken = PowerBIToken.CreateProvisionToken(workspaceCollectionName);
            using (var client = CreateClient(provisionToken, accessKey, apiUrl))
            {
                // Create a new workspace witin the specified collection
                return client.Workspaces.PostWorkspace(workspaceCollectionName);
            }
        }

        static IPowerBIClient CreateClient(PowerBIToken token, string accessKey, string apiUrl)
        {
            WorkspaceCollectionKeys accessKeys = new WorkspaceCollectionKeys()
            {
                Key1 = accessKey
            };

            // Generate a JWT token used when accessing the REST APIs
            var jwt = token.Generate(accessKeys.Key1);

            // Create a token credentials with "AppToken" type
            var credentials = new TokenCredentials(jwt, "AppToken");

            // Instantiate your Power BI client passing in the required credentials
            var client = new PowerBIClient(credentials);

            // Override the api endpoint base URL.  Default value is https://api.powerbi.com
            client.BaseUri = new Uri(apiUrl);

            return client;
        }

        static Import ImportPbix(string workspaceCollectionName, string workspaceId, string datasetName, string filePath, string accessKey, string apiUrl)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                // Create a dev token for import
                var devToken = PowerBIToken.CreateDevToken(workspaceCollectionName, workspaceId);
                using (var client = CreateClient(devToken, accessKey, apiUrl))
                {

                    // Import PBIX file from the file stream
                    var import = client.Imports.PostImportWithFile(workspaceCollectionName, workspaceId, fileStream, datasetName);
                    return import;
                }
            }
        }
    }

    public class WorkspaceCollectionKeys
    {
        [JsonProperty(PropertyName = "key1")]
        public string Key1 { get; set; }

        [JsonProperty(PropertyName = "key2")]
        public string Key2 { get; set; }
    }
}