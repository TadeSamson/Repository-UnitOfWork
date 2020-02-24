using SampleArchitecture.Infrastructure.Implementation.MongoRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SampleArchitecture.Infrastructure.Utils
{
    public static class ConfigurationManager
    {
        static MongoConfig mongoConfig;
        static ConfigurationManager()
        {
            //var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            //settings = builder.Build();

        }

        public static MongoConfig GetMongoConfig()
        {
           
            //MongoConfig mongoConfig = new MongoConfig();
            //settings.Bind("MongoSettings",mongoConfig);
            //mongoConfig.Username = WebUtility.UrlEncode(mongoConfig.Username);
            //mongoConfig.Password = WebUtility.UrlEncode(mongoConfig.Password);
            return mongoConfig;
        }

        public static void SetMongoConfig(MongoConfig config)
        {
            mongoConfig = config;
            if (mongoConfig != null)
            {
                mongoConfig.Username = WebUtility.UrlEncode(mongoConfig.Username);
                mongoConfig.Password = WebUtility.UrlEncode(mongoConfig.Password);
            }
        }
    }
}
