using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleArchitecture.Core.Implementations;

namespace SampleArchitecture.Core.Interfaces
{
    public interface IBlobStorage
    {
        string GetDefaultContainer();
        string PutBlob(Blob blob);
        string CopyBlob(string targetPath, string destinationPath);
        bool DeleteBlob(string path);
        Blob GetBlob(string path);
        string GetContainerFromPath(string path);
        string GetFilenameFromPath(string path);
        Task<string> PutBlobAsync(Blob blob);
        Task<string> CopyBlobAsync(string targetPath, string destinationPath);
        Task<bool> DeleteBlobAsync(string path);
        Task<Blob> GetBlobAsync(string path);


    }
}
