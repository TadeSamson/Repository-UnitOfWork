using System.Collections.Generic;
using System.Threading.Tasks;
using SampleArchitecture.Core.Utils;

namespace SampleArchitecture.Core.Interfaces
{
    /// <summary>
    /// The base Repository Interface. Consumers who necessary care less about Transactional consistency can just implement this and be okay. 
    /// </summary>
    public interface  IRepository<T>  where T:class,IEntity
    {


        #region asynchronous api

        /// <summary>
        /// Gets all entities associated with this repository.
        /// </summary>
        /// <returns>
        /// <c> <see cref="IEnumerable<T>"/> </c> where T is of type EntityBase
        /// </returns>
         Task<List<T>> GetEntitiesAsync(GetOption<T> option = null);
 

        /// <summary>
        /// Gets the entity asynchronous.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns>
        ///   <c>entity T of type <see cref="EntityBase" /></c>
        /// </returns>
        /// 
        Task<T> GetEntityAsync(string entityId, GetOption<T> option = null);

        /// <summary>
        /// Persists added entity asynchronous.
        /// </summary>
        /// <param name="entity">The added entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        Task<CrudResult<string>> PersistAddEntityAsync(T entity);


        /// <summary>
        /// Persists updated entity asynchronous.
        /// </summary>
        /// <param name="entity">The updated entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        Task<CrudResult<bool>> PersistUpdateEntityAsync( T entity);


        /// <summary>
        /// Persists deleted entity asynchronous.
        /// </summary>
        /// <param name="entity">The deleted entity to persist</param>
        /// <returns><c>True</c> if persisted, <c>False</c> otherwise</returns>
        Task<CrudResult<bool>> PersistDeleteEntityAsync(T entity);
        #endregion




    }
}
