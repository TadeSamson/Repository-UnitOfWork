using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleArchitecture.Core.Implementations;
using SampleArchitecture.Core.Interfaces;
using SampleArchitecture.Core.Models;
using SampleArchitecture.Core.Utils;

namespace SampleArchitecture.Core.Implementations.EntityRepositories
{
    public abstract class TagRepositoryBase:UnitOfWorkRepositoryBase<Tag>
    {
        public TagRepositoryBase(IUnitOfWork work)
            : base(work)
        {

        }

        #region Synchronous
        public abstract Tag GetTagByName(string partitionKey, string name);
       // public abstract User GetUser(string partitionKey, string tagId);

        #endregion
        #region Asynchronous
        public abstract Task<Tag> GetTagByNameAsync(string partitionKey, string name);
       // public abstract Task<User> GetUserAsync(string partitionKey, string tagId);
        
        #endregion
    }
}
