using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace FBIUCRDemo.BusinessLogic
{
    public class StorageClient
    {
        public string GetBlobLink(StringBuilder csvData)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["UCRStorage"].ToString());
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference("ucrcsv");
            blobContainer.CreateIfNotExists();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference("UCRDataCsv_" + DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_') + ".csv");

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1);
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            string csvLink = blob.Uri + sasBlobToken;

            var newblob = new CloudBlockBlob(new Uri(csvLink));
            byte[] byteArray = Encoding.UTF8.GetBytes(csvData.ToString());
            MemoryStream stream = new MemoryStream(byteArray);
            blob.UploadFromStream(stream);

            return csvLink;
        }

        public string GetBlobLink(String jsonData)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["UCRStorage"].ToString());
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference("ucrcsv");
            blobContainer.CreateIfNotExists();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference("UCRDataJson_" + DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_') + ".json");

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1);
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            string jsonLink = blob.Uri + sasBlobToken;

            var newblob = new CloudBlockBlob(new Uri(jsonLink));
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonData.ToString());
            MemoryStream stream = new MemoryStream(byteArray);
            blob.UploadFromStream(stream);

            return jsonLink;
        }

        public static void LogError(Exception ex)
        {
            StringBuilder sbError = new StringBuilder("Message").AppendLine().AppendLine().AppendLine(ex.Message).AppendLine().AppendLine().AppendLine("************************************************************************").AppendLine().AppendLine();
            sbError.AppendLine(ex.StackTrace).AppendLine().AppendLine().AppendLine("************************************************************************").AppendLine().AppendLine();
            sbError.AppendLine("Source").AppendLine().AppendLine().AppendLine(ex.Source).AppendLine().AppendLine().AppendLine("************************************************************************").AppendLine().AppendLine();
            if(ex.InnerException != null)
            {
                sbError.AppendLine("InnerException").AppendLine().AppendLine().AppendLine(ex.InnerException.Message).AppendLine().AppendLine().AppendLine("************************************************************************").AppendLine().AppendLine();
                sbError.AppendLine(ex.InnerException.StackTrace);
            }
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["UCRStorage"].ToString());
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference("ucrlog");
            blobContainer.CreateIfNotExists();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_') + ".txt");

            var newblob = new CloudBlockBlob(blob.Uri);
            byte[] byteArray = Encoding.UTF8.GetBytes(sbError.ToString());
            MemoryStream stream = new MemoryStream(byteArray);
            blob.UploadFromStream(stream);//.PutBlock(id, stream, null);
        }
    }
}