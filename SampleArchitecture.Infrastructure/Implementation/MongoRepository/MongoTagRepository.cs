using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SampleArchitecture.Core.Implementations.EntityRepositories;
using SampleArchitecture.Core.Interfaces;
using SampleArchitecture.Core.Models;
using SampleArchitecture.Core.Utils;
using SampleArchitecture.Infrastructure.Utils;

namespace SampleArchitecture.Infrastructure.Implementation.MongoRepository
{
    public class MongoTagRepository : TagRepositoryBase
    {
        IMongoDatabase mongoDatabase;
        string tableName = typeof(Core.Models.Tag).Name;
        public MongoTagRepository(IUnitOfWork _work) : base(_work)
        {

            mongoDatabase = MongoConnection.GetInstance().Database;
        }



        #region UnitOfWorkRepositoryBase
        public override Task<List<Core.Models.Tag>> GetEntitiesAsync(GetOption<Core.Models.Tag> option = null)
        {
            GetOptionToDocumentQuery<Core.Models.Tag> documentQuery = option.ToDocumentQuery();
            return mongoDatabase.GetCollection<Core.Models.Tag>(this.tableName).Find(documentQuery.FilterDefinition)
                .Project(documentQuery.ProjectionDefinition)
                .Sort(documentQuery.SortDefinition)
                .Skip(documentQuery.PaginationDefinition.skip)
                .Limit(documentQuery.PaginationDefinition.limit).ToListAsync();
        }

        public override Task<Core.Models.Tag> GetEntityAsync(string entityId, GetOption<Core.Models.Tag> option = null)
        {
            option = option ?? new GetOption<Core.Models.Tag>();
            option.SearchOption = new SearchOption<Core.Models.Tag>() { Expression = new PropertySearchExpression() { Operator = "=", Property = nameof(IEntity.Id), Value = entityId.ToString() } };
            GetOptionToDocumentQuery<Core.Models.Tag> documentQuery = option.ToDocumentQuery();
            return mongoDatabase.GetCollection<Core.Models.Tag>(this.tableName).Find(documentQuery.FilterDefinition).Limit(1).Project(documentQuery.ProjectionDefinition).SingleOrDefaultAsync();

        }

   

        protected override Task<CrudResult<string>> PersistAddEntityAsync(Core.Models.Tag entity)
        {

            TaskCompletionSource<CrudResult<string>> tcs = new TaskCompletionSource<CrudResult<string>>();
            mongoDatabase.GetCollection<Core.Models.Tag>(this.tableName).InsertOneAsync(entity).ContinueWith(resultTask => {
                if (resultTask.IsFaulted)
                {
                    MongoCommandException mongoException = resultTask.Exception?.InnerException as MongoCommandException;
                    if (mongoException != null && mongoException.CodeName == "OperationNotSupportedInTransaction")
                    {
                        try
                        {
                            mongoDatabase.CreateCollection(tableName);
                            List<CreateIndexModel<Core.Models.Tag>> indexModelList = new List<CreateIndexModel<Core.Models.Tag>>();



                            ////adding userId as Index
                            var userIdIndexKey = Builders<Core.Models.Tag>.IndexKeys.Ascending(new StringFieldDefinition<Core.Models.Tag>($"{nameof(Core.Models.Tag.UserId)}"));
                            var userIdIndexModel = new CreateIndexModel<Core.Models.Tag>(userIdIndexKey);
                            indexModelList.Add(userIdIndexModel);

                            ////adding name as Index
                            var nameIndexKey = Builders<Core.Models.Tag>.IndexKeys.Ascending(new StringFieldDefinition<Core.Models.Tag>($"{nameof(Core.Models.Tag.Name)}"));
                            var nameIndexModel = new CreateIndexModel<Core.Models.Tag>(nameIndexKey);
                            indexModelList.Add(nameIndexModel);


                            //adding all the indexes. 
                            mongoDatabase.GetCollection<Core.Models.Tag>(this.tableName).Indexes.CreateMany(indexModelList);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }

                    tcs.SetException(resultTask.Exception);

                }
                else
                    tcs.SetResult(CrudResult<String>.Success(entity.Id));

            });
            return tcs.Task;
        }

        protected override Task<CrudResult<bool>> PersistDeleteEntityAsync(Core.Models.Tag entity)
        {
            return mongoDatabase.GetCollection<Core.Models.Tag>(this.tableName).DeleteOneAsync(user => user.Id == entity.Id).ContinueWith(resultTask =>
            {
                if (!resultTask.IsFaulted)
                    return CrudResult<bool>.Success(true);
                throw resultTask.Exception;
            });
        }

        protected override Task<CrudResult<bool>> PersistUpdateEntityAsync(Core.Models.Tag entity)
        {
            return mongoDatabase.GetCollection<Core.Models.Tag>(this.tableName).ReplaceOneAsync(iEntity => iEntity.Id == entity.Id, entity).ContinueWith(resultTask =>
            {
                if (!resultTask.IsFaulted)
                    return CrudResult<bool>.Success(true);
                throw resultTask.Exception;
            });
        }

        #endregion



        #region TagRepositoryBase Implementation
        public override Core.Models.Tag GetTagByName(string partitionKey, string name)
        {
            throw new NotImplementedException();
        }

        public override Task<Core.Models.Tag> GetTagByNameAsync(string partitionKey, string name)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
