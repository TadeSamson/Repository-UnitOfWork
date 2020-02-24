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
    public abstract class BlogPostRepositoryBase:UnitOfWorkRepositoryBase<BlogPost>
    {
        public BlogPostRepositoryBase(IUnitOfWork work)
            : base(work)
        {

        }
  
        #region Asynchronous
        /// <summary>
        /// Gets the by blog identifier asynchronous.
        /// </summary>
        /// <param name="partitionKey">The partition key is the parent blog id</param>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        public abstract Task<List<BlogPost>> GetByBTagIdAsync(string tagId, GetOption<BlogPost> option);
        
        #endregion
    }
}
