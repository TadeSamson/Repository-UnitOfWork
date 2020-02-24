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
    public class MongoBlogpostRepository : BlogPostRepositoryBase
    {
        IMongoDatabase mongoDatabase;
        string tableName = typeof(BlogPost).Name;
        public MongoBlogpostRepository(IUnitOfWork _work) : base(_work) {

            mongoDatabase = MongoConnection.GetInstance().Database;
        }
      



        #region UnitOfWorkRepositoryBase Implementation
        public override Task<List<BlogPost>> GetEntitiesAsync(GetOption<BlogPost> option = null)
        {
            GetOptionToDocumentQuery<BlogPost> documentQuery = option.ToDocumentQuery();
            return mongoDatabase.GetCollection<BlogPost>(this.tableName).Find(documentQuery.FilterDefinition)
                .Project(documentQuery.ProjectionDefinition)
                .Sort(documentQuery.SortDefinition)
                .Skip(documentQuery.PaginationDefinition.skip)
                .Limit(documentQuery.PaginationDefinition.limit).ToListAsync();
        }

        public override Task<BlogPost> GetEntityAsync(string entityId, GetOption<BlogPost> option = null)
        {
            option = option ?? new GetOption<BlogPost>();
            option.SearchOption = new SearchOption<BlogPost>() { Expression = new PropertySearchExpression() { Operator = "=", Property = nameof(IEntity.Id), Value = entityId.ToString() } };
            GetOptionToDocumentQuery<BlogPost> documentQuery = option.ToDocumentQuery();
            return mongoDatabase.GetCollection<BlogPost>(this.tableName).Find(documentQuery.FilterDefinition).Limit(1).Project(documentQuery.ProjectionDefinition).SingleOrDefaultAsync();

        }

        protected override Task<CrudResult<string>> PersistAddEntityAsync(BlogPost entity)
        {

            TaskCompletionSource<CrudResult<string>> tcs = new TaskCompletionSource<CrudResult<string>>();
            mongoDatabase.GetCollection<BlogPost>(this.tableName).InsertOneAsync(entity).ContinueWith(resultTask => {
                if (resultTask.IsFaulted)
                {
                    MongoCommandException mongoException = resultTask.Exception?.InnerException as MongoCommandException;
                    if (mongoException != null && mongoException.CodeName == "OperationNotSupportedInTransaction")
                    {
                        try
                        {
                            mongoDatabase.CreateCollection(tableName);
                            List<CreateIndexModel<BlogPost>> indexModelList = new List<CreateIndexModel<BlogPost>>();

                            ////adding userId as Index
                            var userIdIndexKey = Builders<BlogPost>.IndexKeys.Ascending(new StringFieldDefinition<BlogPost>($"{nameof(BlogPost.UserId)}"));
                            var userIdIndexModel = new CreateIndexModel<BlogPost>(userIdIndexKey);
                            indexModelList.Add(userIdIndexModel);


                            ////adding tagId  as Index
                            var tagIdIndexKey = Builders<BlogPost>.IndexKeys.Ascending(new StringFieldDefinition<BlogPost>($"{nameof(BlogPost.BlogPostTagIds)}"));
                            var tagIdIndexModel = new CreateIndexModel<BlogPost>(tagIdIndexKey);
                            indexModelList.Add(tagIdIndexModel);

                            //adding all the indexes. 
                            mongoDatabase.GetCollection<BlogPost>(this.tableName).Indexes.CreateMany(indexModelList);
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

        protected override Task<CrudResult<bool>> PersistDeleteEntityAsync(BlogPost entity)
        {
            return mongoDatabase.GetCollection<BlogPost>(this.tableName).DeleteOneAsync(user => user.Id == entity.Id).ContinueWith(resultTask =>
            {
                if (!resultTask.IsFaulted)
                    return CrudResult<bool>.Success(true);
                throw resultTask.Exception;
            });
        }

        protected override Task<CrudResult<bool>> PersistUpdateEntityAsync(BlogPost entity)
        {
            return mongoDatabase.GetCollection<BlogPost>(this.tableName).ReplaceOneAsync(iEntity => iEntity.Id == entity.Id, entity).ContinueWith(resultTask =>
            {
                if (!resultTask.IsFaulted)
                    return CrudResult<bool>.Success(true);
                throw resultTask.Exception;
            });
        }

        #endregion


        #region BlogpostRepository Implementation
        public override Task<List<BlogPost>> GetByBTagIdAsync(string tagId, GetOption<BlogPost> option)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
