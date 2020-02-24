using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampleArchitecture.Core.Interfaces;
using SampleArchitecture.Core.Utils;
using System.Transactions;

namespace SampleArchitecture.Core.Implementations
{
    /// <summary>
    /// the base repository class which would allow <see cref="IUnitOfWork" persist <see cref="EntityBase"/> objects/>
    /// </summary>
    /// <remarks>The class is abstract rather than being an interface so as to abstract the persisted layer 
    /// away from the <see cref="RepositoryBase"/>e and thereby force all persists to take place through <see cref="IUnitOfWork"/>. 
    /// making this an interface will force it members to be publicly accessible  </remarks>
    public abstract class UnitOfWorkRepositoryBase<T> :  IEnlistmentNotification where T:class,IEntity
    {
        Boolean enlisted;
        Dictionary<IEntity, EnlistmentOperations> transactionElements = new Dictionary<IEntity, EnlistmentOperations>();

        private IUnitOfWork unitOfWork;
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
        /// </summary>
        /// <param name="_unitOfWork">The _unit of work that will be associated with this repository.</param>
        /// <exception cref="System.Exception">IUnitOfWork instance is require for class initialization</exception>
        public UnitOfWorkRepositoryBase(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork ?? throw new Exception("IUnitOfWork instance is require for class initialization");
        }

        #region Delegating the implementation of IRepository methods to Derive classes of UniOfWorkRepositoryBase

        public abstract Task<List<T>> GetEntitiesAsync(GetOption<T> option = null);
        public abstract Task<T> GetEntityAsync(string entityId, GetOption<T> option = null);

        protected abstract Task<CrudResult<string>> PersistAddEntityAsync(T entity);

        protected abstract Task<CrudResult<bool>> PersistUpdateEntityAsync(T entity);

        protected abstract Task<CrudResult<bool>> PersistDeleteEntityAsync(T entity);
        #endregion



        #region support methods to allow consumer pass transactional entity to unifOfWork through Repository

        /// <summary>
        /// Pushes an entity of type <see cref="IEntity"/> into <see cref="IUnitOfWork"/> for deletion after call to it CommitChanges method.
        /// </summary>
        /// <param name="entity">The entity to Add.</param>
        public void QueueForRemove(T entity)
        {
            this.unitOfWork.RegisterDelete(entity, this as UnitOfWorkRepositoryBase<IEntity>);
        }


        /// <summary>
        /// Pushes an entity of type <see cref="IEntity"/> into <see cref="IUnitOfWork"/> for update after call to it <c> CommitChanges()</c> method.
        /// </summary>
        /// <param name="entity">The entity to Update.</param>
        public void QueueForUpdate(T entity)
        {
            this.unitOfWork.RegisterUpdate(entity, this as UnitOfWorkRepositoryBase<IEntity>);
        }


        /// <summary>
        /// Pushes an entity of type <see cref="IEntity"/> into a <see cref="IUnitOfWork"/>  for persistence after call to it <c> CommitChanges()</c> method.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        public void QueueForAdd(T entity)
        {
            this.unitOfWork.RegisterAdd(entity, this as UnitOfWorkRepositoryBase<IEntity>);
        }

        #endregion



        #region these methods are called directly by <see cref="IUnitOfWork"/> which in turn calls the necessary TransactionalMethod.


        /// <summary>
        /// Prepares an entity to take part in a unitOfWork add operation.
        /// It does nothing than call virtual TransactionalAddAsync that knows how to perform an ACID Add
        /// <remark> Will be called by UnitofWork implementation. This introduction of this method necessary to make TransactionalAddAsync only accessible to only the 
        /// derive class of UnifOfWorkRepositoryBase. Not introducing it will force the access modifier of the necessary TransactionMethod to become public
        /// which intentionally, should not be exposed to consumer.</remark> 
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>
        ///   <c>True</c> 
        internal Task<CrudResult<String>> UnitOfWorkAddAsync(IEntity entity)
        {
            return this.TransactionalAddAsync(entity);
        }


        /// <summary>
        /// Prepares an entity to take part in a unitOfWork update operation. 
        /// Will be called by UnitofWork implementation
        /// It does nothing than call virtual TransactionalUpdateAsync that knows how to perform an ACID update
        /// <remark>Will be called by UnitofWork implementation. This introduction of this method necessary to make TransactionalAddAsync only accessible to only the 
        /// derive class of UnifOfWorkRepositoryBase. Not introducing it will force the access modifier of the necessary TransactionMethod to become public
        /// which intentionally, should not be exposed to consumer. </remark>
        /// </summary>
        /// <param name="entity">The updated entity to persist</param>
        /// <returns>
        ///   <c>True</c> if prepared, <c>False</c> otherwise
        /// </returns>
        internal Task<CrudResult<bool>> UnitOfWorkUpdateAsync(IEntity entity)
        {
            return this.TransactionalUpdateAsync(entity);
        }




        /// <summary>
        /// Prepares an entity to take part in a unitOfWork delete operation
        /// /// Will be called by UnitofWork implementation
        /// It does nothing than call virtual TransactionalDeleteAsync that knows how to perform an ACID delete
        /// <remark>Will be called by UnitofWork implementation. This introduction of this method necessary to make TransactionalAddAsync only accessible to only the 
        /// derive class of UnifOfWorkRepositoryBase. Not introducing it will force the access modifier of the necessary TransactionMethod to become public
        /// which intentionally, should not be exposed to consumer. </remark>
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <returns>
        ///   <c>True</c> 
        internal Task<CrudResult<bool>> UnitOfWorkDeleteAsync(IEntity entity)
        {
            return this.TransactionalDeleteAsync(entity);
        }


        #endregion



        #region TransactionManager Implementation


        void enlistForTransaction()
        {
            if (this.enlisted == false)
            {
                Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);
                this.enlisted = true;
            }
        }

        public void Commit(Enlistment enlistment)
        {
            enlistment.Done();
        }
        public void InDoubt(Enlistment enlistment)
        {
            Rollback(enlistment);
            //enlistment.Done();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }



        public void Rollback(Enlistment enlistment)
        {
            foreach (IEntity entity in transactionElements.Keys)
            {
                EnlistmentOperations operation = transactionElements[entity];
                switch (operation)
                {
                    case EnlistmentOperations.Add: PersistDeleteEntityAsync(entity as T); break;
                    case EnlistmentOperations.Delete: PersistAddEntityAsync(entity as T); break;
                    case EnlistmentOperations.Update: PersistUpdateEntityAsync(entity as T); break;
                }
            }
            enlistment.Done();
        }

        #endregion



        #region Methods in charge of Transaction operations that can rollback. Can be overriden to behave differently by derive classes

        /// <summary>
        /// Performs Add operation that can be rollback
        /// </summary>
        /// <param name="entity"></param>
        /// <returns><c>True</c> if operation success, <c>False</c> otherwise</returns>
        protected virtual Task<CrudResult<string>> TransactionalAddAsync(IEntity entity)
        {
            if (Transaction.Current != null)
            {
                enlistForTransaction();
                transactionElements.Add(entity, EnlistmentOperations.Add);
                return this.PersistAddEntityAsync(entity as T);
            }
            else
                return this.PersistAddEntityAsync(entity as T);
        }


        /// <summary>
        /// Performs Update operation that can be rollback
        /// </summary>
        /// <param name="entity"></param>
        /// <returns><c>True</c> if operation success, <c>False</c> otherwise</returns>
        protected virtual Task<CrudResult<bool>> TransactionalUpdateAsync(IEntity entity)
        {
            if (Transaction.Current != null)
            {
                enlistForTransaction();
                GetEntityAsync(entity.Id).ContinueWith((resultTask) =>
                {
                    T original = resultTask.Result as T;
                    transactionElements.Add(original, EnlistmentOperations.Update);
                });

                return this.PersistUpdateEntityAsync(entity as T);
            }
            else
                return this.PersistUpdateEntityAsync(entity as T);
        }


        /// <summary>
        /// Performs Delete operation that can be rollback
        /// </summary>
        /// <param name="entity"></param>
        /// <returns><c>True</c> if operation success, <c>False</c> otherwise</returns>
        protected virtual Task<CrudResult<bool>> TransactionalDeleteAsync(IEntity entity)
        {
            if (Transaction.Current != null)
            {
                enlistForTransaction();
                GetEntityAsync(entity.Id).ContinueWith((resultTask) =>
                {
                    T original = resultTask.Result as T;
                    transactionElements.Add(original, EnlistmentOperations.Delete);
                });
                return this.PersistDeleteEntityAsync(entity as T);
            }
            else
                return this.PersistDeleteEntityAsync(entity as T);
        }




        #endregion








    }





}