using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleArchitecture.Core.Interfaces
{
    public interface ICacheService
    {
        #region Sychronous
        bool Add<T>(string key, T value, bool overwrite = true);
        bool AddList(Dictionary<string, object> storeObjects, bool overwrite = true);

        bool KeyExists(string key);
        string Getstring(string key);

        T GetObject<T>(string key);

        bool RemoveIfExists(string key);
        #endregion
        #region Asychronous
        Task<bool> AddAsync<T>(string key, T value, bool overwrite = true);
        Task<bool> AddListAsync(Dictionary<string, object> storeObjects, bool overwrite = true);

        Task<bool> KeyExistsAsync(string key);
        Task<string> GetstringAsync(string key);

        Task<T> GetObjectAsync<T>(string key);
        Task<bool> RemoveIfExistsAsync(string key);
        #endregion
    }
}
