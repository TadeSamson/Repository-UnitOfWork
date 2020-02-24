//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Paytithe.Core.Implementations;
//using Paytithe.Core.Interface;
//using Paytithe.Infrastructure.Utils;

//namespace Paytithe.Infrastructure.Implementation.BlobStorage
//{
//    public class FolderBlobStorageHelper : IBlobStorage
//    {
//        public string PutBlob(Blob blob)
//        {
//            try
//            {
//                var basePath = System.Web.HttpContext.Current.Server.MapPath("~/files/");
//                if (!Directory.Exists(basePath))
//                    Directory.CreateDirectory(basePath);
//                string[] folders = blob.Container.Split('/');
//                foreach (string folder in folders)
//                {
//                    basePath = basePath + folder + "/";
//                    if (!Directory.Exists(basePath))
//                        Directory.CreateDirectory(basePath);
//                }
//                basePath = basePath + blob.Filename;
//                if (System.IO.File.Exists(blob.Container))
//                    System.IO.File.Delete(blob.Container);
//                using (System.IO.FileStream fs = new System.IO.FileStream(basePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
//                {
//                    using (MemoryStream ms = new MemoryStream(blob.Bytes))
//                    {
//                        ms.WriteTo(fs);
//                    }
//                }
//                basePath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/files/" + blob.Container + "/" + blob.Filename;
//                return basePath;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        public bool DeleteBlob(string path)
//        {
//            if (System.IO.File.Exists(path))
//                System.IO.File.Delete(path);
//            return true;
//        }

//        public string CopyBlob(string targetPath, string destinationPath)
//        {
//            targetPath = HttpContext.Current.Server.MapPath("~/" + targetPath);
//            destinationPath = HttpContext.Current.Server.MapPath("~/" + destinationPath);
//            if (System.IO.File.Exists(targetPath))
//            {
//                System.IO.File.Copy(targetPath, destinationPath, true);
//            }
//            return destinationPath;
//        }



//        public Task<string> PutBlobAsync(Blob blob)
//        {
//            throw new NotImplementedException("use the synchronous method instead");
//        }

//        public Blob GetBlob(string path)
//        {
//            Blob blob = new Blob();
//            path = HttpContext.Current.Server.MapPath("~/" + path);
//            if (System.IO.File.Exists(path))
//            {
//                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
//                {

//                    fs.Read(blob.Bytes, 0, (int)fs.Length);
//                    blob.Filename = path.SplitAsBlobPath().Value;
//                    blob.Container = path.SplitAsBlobPath().Key;
//                }
//            }

//            return blob;
//        }

//        public Task<Blob> GetBlobAsync(string path)
//        {
//            throw new NotImplementedException("use the synchronous method instead");
//        }

//        public Task<string> CopyBlobAsync(string targetPath, string destinationPath)
//        {
//            throw new NotImplementedException("use the synchronous method instead");
//        }

//        public Task<bool> DeleteBlobAsync(string path)
//        {
//            throw new NotImplementedException("use the synchronous method instead");
//        }
//    }
//}
