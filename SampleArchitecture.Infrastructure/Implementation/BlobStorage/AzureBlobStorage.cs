//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Blob;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using LearnPM.Core.Implementations;
//using LearnPM.Core.Interface;
//using SampleArchitecture.Infrastructure.Utils;

//namespace Paytithe.Infrastructure.Implementation.BlobStorage
//{
//    public class AzureBlobStorage : IBlobStorage
//    {

//        static CloudBlobClient blobClient;
//        static AzureBlobStorage()
//        {
//            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(new AppSettingsReader().GetValue("BlobStorageConnectionString", typeof(string)) as string);
//            blobClient = storageAccount.CreateCloudBlobClient();
//        }


//        #region Synchronous
//        public bool CreateContainerIfNotExist(CloudBlobContainer container)
//        {

//            bool status = container.CreateIfNotExists();
//            if (status == true)
//            {
//                container.SetPermissionsAsync(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
//            }
//            return status;
//        }

//        public string PutBlob(Blob blob)
//        {
//            CloudBlobContainer container = blobClient.GetContainerReference(blob.Container);
//            CreateContainerIfNotExist(container);
//            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blob.Filename);
//            blockBlob.UploadFromByteArray(blob.Bytes, 0, blob.Bytes.Length);
//            return blockBlob.Uri.AbsoluteUri;
//        }

//        public Blob GetBlob(string path)
//        {
//            Blob blob = new Blob();
//            CloudBlobContainer blobContainer = blobClient.GetContainerReference(path.SplitAsBlobPath().Key);
//            if (blobContainer.Exists())
//            {
//                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(path.SplitAsBlobPath().Value);
//                // blockBlob.BeginDownloadToByteArray(blob.Bytes, 0, (obj) => { callback.Invoke(); }, null);
//                blockBlob.DownloadRangeToByteArray(blob.Bytes, 0, null, null);
//            }

//            return blob;
//        }
//        public string CopyBlob(string targetPath, string destinationPath)
//        {

//            CloudBlobContainer targetContainer = blobClient.GetContainerReference(targetPath.SplitAsBlobPath().Key);
//            if (!targetContainer.Exists())
//                return "";
//            CloudBlockBlob targetBlockBlob = targetContainer.GetBlockBlobReference(targetPath.SplitAsBlobPath().Value);
//            if (!targetBlockBlob.Exists())
//                return "";

//            CloudBlobContainer destinationContainer = blobClient.GetContainerReference(destinationPath.SplitAsBlobPath().Key);
//            CreateContainerIfNotExist(destinationContainer);
//            CloudBlockBlob destinationBlockBlob = destinationContainer.GetBlockBlobReference(destinationPath.SplitAsBlobPath().Value);
//            string path = destinationBlockBlob.StartCopy(targetBlockBlob);
//            return path;
//        }

//        /// <summary>
//        /// Deletes the specify Blob (not available. Use the Asynchronous counterpart instead).
//        /// </summary>
//        /// <param name="path">The path.</param>
//        /// <returns></returns>
//        /// <exception cref="System.NotImplementedException"> not available. use the Asynchronous method instead</exception>
//        public bool DeleteBlob(string path)
//        {
//            bool status = false;
//            try
//            {
//                CloudBlobContainer container = blobClient.GetContainerReference(path.SplitAsBlobPath().Key);
//                if (container.Exists())
//                {
                   
//                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(path.SplitAsBlobPath().Value);
//                    status = blockBlob.DeleteIfExists();
//                }

//                return status;
//            }
//            catch
//            {
//                return status;
//            }
//        }

//        public string GetContainerFromPath(string path)
//        {
//            if (path == null)
//                return "others";
//            string container = path.SplitAsBlobPath().Key;
//            return container;
//        }

//        public string GetFilenameFromPath(string path)
//        {
//            string container = path.Substring(0, path.LastIndexOf('/'));
//            string filename = path.Substring(path.LastIndexOf('/') + 1);
//            return filename;
//        } 
//        #endregion
//        #region Asynchronous
//        public async Task<bool> CreateContainerIfNotExistAsync(CloudBlobContainer container)
//        {

//            bool status = await container.CreateIfNotExistsAsync().ContinueWith<bool>(result =>
//            {
//                if (result.Result == true)
//                    container.SetPermissionsAsync(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
//                return result.Result;
//            });

//            return status;
//        }



//        public async Task<string> PutBlobAsync(Blob blob)
//        {
//            CloudBlobContainer container = blobClient.GetContainerReference(blob.Container);
//            await CreateContainerIfNotExistAsync(container);
//            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blob.Filename);
//            await blockBlob.UploadFromByteArrayAsync(blob.Bytes, 0, blob.Bytes.Length);
//            return blockBlob.Uri.AbsoluteUri;
//        }

//        public async Task<bool> DeleteBlobAsync(string path)
//        {
//            try
//            {
//                bool status = false;
//                CloudBlobContainer container = blobClient.GetContainerReference(path.SplitAsBlobPath().Key);
//                if (container.Exists())
//                {
//                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(path.SplitAsBlobPath().Value);
//                    status = await blockBlob.DeleteIfExistsAsync();
//                }

//                return status;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Gets the BLOB asynchronous (not available. Use the Synchronous counterpart instead).
//        /// </summary>
//        /// <param name="path">The path.</param>
//        /// <returns></returns>
//        /// <exception cref="System.NotImplementedException">use the synchronous method instead</exception>
//        public Task<Blob> GetBlobAsync(string path)
//        {
//            Blob blob = new Blob();
//            CloudBlobContainer blobContainer = blobClient.GetContainerReference(path.SplitAsBlobPath().Key);
//            if (blobContainer.Exists())
//            {
//                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(path.SplitAsBlobPath().Value);
//                // blockBlob.BeginDownloadToByteArray(blob.Bytes, 0, (obj) => { callback.Invoke(); }, null);
//                return blockBlob.DownloadRangeToByteArrayAsync(blob.Bytes, 0, null, null).ContinueWith(result =>
//                {
//                    return blob;
//                });
//            }

//            else
//                return new Task<Blob>(() => blob);
//        }

//        /// <summary>
//        /// Copies the BLOB asynchronous (not available. Use the Synchronous counterpart instead).
//        /// </summary>
//        /// <param name="targetPath">The target path.</param>
//        /// <param name="destinationPath">The destination path.</param>
//        /// <returns></returns>
//        /// <exception cref="System.NotImplementedException">use the synchronous method instead</exception>
//        public Task<string> CopyBlobAsync(string targetPath, string destinationPath)
//        {
//            throw new NotImplementedException();
//            //CloudBlobContainer targetContainer = blobClient.GetContainerReference(targetPath.SplitAsBlobPath().Key);
//            //if (!targetContainer.ExistsAsync())
//            //    return new Task<string>(() => "");
//            //CloudBlockBlob targetBlockBlob = targetContainer.GetBlockBlobReference(targetPath.SplitAsBlobPath().Value);
//            //if (!targetBlockBlob.Exists())
//            //    return "";

//            //CloudBlobContainer destinationContainer = blobClient.GetContainerReference(destinationPath.SplitAsBlobPath().Key);
//            //CreateContainerIfNotExist(destinationContainer);
//            //CloudBlockBlob destinationBlockBlob = destinationContainer.GetBlockBlobReference(destinationPath.SplitAsBlobPath().Value);
//            //string path = destinationBlockBlob.StartCopy(targetBlockBlob);
//            //return path;
//        } 
//        #endregion


      
//    }
//}
