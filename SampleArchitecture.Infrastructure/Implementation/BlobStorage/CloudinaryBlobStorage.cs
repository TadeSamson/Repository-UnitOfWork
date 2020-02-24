using LearnPM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LearnPM.Core.Implementations;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace SampleArchitecture.Infrastructure.Implementation.BlobStorage
{

    public class CloudinaryOptions
    {
        public CloudinaryOptions(String name, String key, String secret,String rootFolder)
        {
            this.Name = name;
            this.Key = key;
            this.Secret = secret;
        }
        public String Name { get; set; }
        public String Key { get; set; }
        public String Secret { get; set; }
        public String RootFolder { get; set; }
    }
    public class CloudinaryBlobStorage: IBlobStorage
    {
        Cloudinary cloudinary;
        CloudinaryOptions cloudinarySettings;
        public CloudinaryBlobStorage(CloudinaryOptions settings)
        {
            Account account = new Account(settings.Name, settings.Key, settings.Secret);
            cloudinary = new Cloudinary(account);
            this.cloudinarySettings = settings;
        }

        public string CopyBlob(string targetPath, string destinationPath)
        {
            throw new NotImplementedException();
        }

        public Task<string> CopyBlobAsync(string targetPath, string destinationPath)
        {
            throw new NotImplementedException();
        }

        public bool DeleteBlob(string path)
        {
            String publicId = path.Substring(path.LastIndexOf('/')+1);
            publicId = publicId.Substring(0, publicId.LastIndexOf('.'));
            publicId = $"{GetDefaultContainer().ToLower()}/{ publicId}";
            DeletionParams deletionParams = new DeletionParams(publicId);
            DeletionResult result= cloudinary.Destroy(deletionParams);
            return result.Result=="ok";
        }

        public Task<bool> DeleteBlobAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Blob GetBlob(string path)
        {
            String publicId = GetFilenameFromPath(path);
            GetResourceParams resourceParams = new GetResourceParams(publicId);
            GetResourceResult result= cloudinary.GetResource(resourceParams);
            WebClient webClient = new WebClient();
            Blob blob = new Blob();
            blob.Bytes=webClient.DownloadData(result.Url);
            return blob;
        }

        public Task<Blob> GetBlobAsync(string path)
        {
            throw new NotImplementedException();
        }

        public string GetContainerFromPath(string path)
        {
            throw new NotImplementedException();
        }

        public string GetFilenameFromPath(string path)
        {
            throw new NotImplementedException();
        }

        public string PutBlob(Blob blob)
        {
            String path = null;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(blob.Bytes);
                ms.Position = 0;
                FileDescription fileDescription = new FileDescription(blob.Filename, ms);
                RawUploadParams rawUploadParams = new RawUploadParams() { File= fileDescription,  Folder=blob.Container, Async="false" };
                RawUploadResult result= cloudinary.Upload(rawUploadParams);
                path = result.Uri.AbsoluteUri;
            }

            return path;

        }

        public Task<string> PutBlobAsync(Blob blob)
        {
            throw new NotImplementedException();
        }

        public String GetDefaultContainer()
        {
            return cloudinarySettings.RootFolder;
        }

    }
}
