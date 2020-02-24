using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using SampleArchitecture.Core.Interfaces;

namespace SampleArchitecture.Core.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        Dictionary<IEntity, UnitOfWorkRepositoryBase<IEntity>> deletedEntities = new Dictionary<IEntity, UnitOfWorkRepositoryBase<IEntity>>();
        Dictionary<IEntity, UnitOfWorkRepositoryBase<IEntity>> updatedEntities = new Dictionary<IEntity, UnitOfWorkRepositoryBase<IEntity>>();
        Dictionary<IEntity, UnitOfWorkRepositoryBase<IEntity>> addedEntities = new Dictionary<IEntity, UnitOfWorkRepositoryBase<IEntity>>();

        private static UnitOfWork instance = null;
        public static UnitOfWork Instance
        {
            get
            {
                if (instance == null)
                    instance = new UnitOfWork();
                return instance;
            }
        }

        private UnitOfWork()
        {

        }

        public void RegisterDelete(IEntity entity, UnitOfWorkRepositoryBase<IEntity> entityRepository)
        {
            this.deletedEntities.Add(entity, entityRepository);
        }


        public void RegisterUpdate(IEntity entity, UnitOfWorkRepositoryBase<IEntity> entityRepository)
        {

            this.updatedEntities.Add(entity, entityRepository);
        }

        public void RegisterAdd(IEntity entity, UnitOfWorkRepositoryBase<IEntity> entityRepository)
        {
            this.addedEntities.Add(entity, entityRepository);
        }


        public Task<bool> CommitChangesAsync()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            List<Task> taskList = new List<Task>();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (IEntity entity in this.deletedEntities.Keys)
                    {
                        taskList.Add(this.deletedEntities[entity].UnitOfWorkDeleteAsync(entity));
                    }
                    foreach (IEntity entity in this.updatedEntities.Keys)
                    {
                        taskList.Add(this.updatedEntities[entity].UnitOfWorkUpdateAsync(entity));
                    }

                    foreach (IEntity entity in this.addedEntities.Keys)
                    {
                        taskList.Add(this.addedEntities[entity].UnitOfWorkAddAsync(entity));

                    }

                    Task.WaitAll(taskList.ToArray());
                    scope.Complete();
                    this.deletedEntities.Clear();
                    this.addedEntities.Clear();
                    this.updatedEntities.Clear();
                    tcs.SetResult(true);

                }
                catch (Exception ex)
                {
                    this.deletedEntities.Clear();
                    this.addedEntities.Clear();
                    this.updatedEntities.Clear();
                    tcs.SetException(ex);
                }

            }
            return tcs.Task;

        }


    }
}
