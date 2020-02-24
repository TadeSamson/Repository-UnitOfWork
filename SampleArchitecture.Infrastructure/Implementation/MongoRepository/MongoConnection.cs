using SampleArchitecture.Infrastructure.Utils;
//using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace SampleArchitecture.Infrastructure.Implementation.MongoRepository
{
    public  class MongoConnection
    {
        IMongoDatabase mongoDatabase;
        private static MongoConnection instance; 
        private MongoConnection()
        {
            MongoConfig mongoConfig = ConfigurationManager.GetMongoConfig();
            String connectionString = $"mongodb://{mongoConfig.Username}:{mongoConfig.Password}@{mongoConfig.Server}:{mongoConfig.Port}/{mongoConfig.DatabaseName}";
            mongoDatabase = new MongoClient(connectionString).GetDatabase(mongoConfig.DatabaseName);
        }

        public static MongoConnection GetInstance()
        {
                if (instance == null)
                    instance = new MongoConnection();
                return instance;
        }
        


        public  IMongoDatabase Database 
        {
            get { return mongoDatabase; }
        }





    }
}
