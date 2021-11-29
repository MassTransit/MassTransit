namespace MassTransit
{
    using System;
    using Configuration;
    using MongoDB.Driver;


    public static class MongoDbSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a MongoDB saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> MongoDbRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<IMongoDbSagaRepositoryConfigurator<TSaga>> configure = null)
            where TSaga : class, ISagaVersion
        {
            var mongoDbConfigurator = new MongoDbSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(mongoDbConfigurator);

            mongoDbConfigurator.Validate().ThrowIfContainsFailure("The MongoDB saga repository configuration is invalid:");

            configurator.Repository(mongoDbConfigurator.Register);

            return configurator;
        }

        /// <summary>
        /// Adds a MongoDB saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> MongoDbRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            string connectionString, Action<IMongoDbSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISagaVersion
        {
            return configurator.MongoDbRepository(cfg =>
            {
                cfg.Connection = connectionString;
                configure?.Invoke(cfg);
            });
        }

        /// <summary>
        /// Adds a MongoDB saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="database"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> MongoDbRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            IMongoDatabase database, Action<IMongoDbSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISagaVersion
        {
            return configurator.MongoDbRepository(cfg =>
            {
                cfg.Database(database);
                configure?.Invoke(cfg);
            });
        }

        /// <summary>
        /// Use the MongoDB saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetMongoDbSagaRepositoryProvider(this IRegistrationConfigurator configurator, Action<IMongoDbSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new MongoDbSagaRepositoryRegistrationProvider(configure));
        }

        /// <summary>
        /// Use the MongoDB saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">The connection string for the MongoDB database</param>
        /// <param name="configure"></param>
        public static void SetMongoDbSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString,
            Action<IMongoDbSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new MongoDbSagaRepositoryRegistrationProvider(r =>
            {
                r.Connection = connectionString;

                configure?.Invoke(r);
            }));
        }

        /// <summary>
        /// Use the MongoDB saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="database">A ready to use MongoDB database</param>
        /// <param name="configure"></param>
        public static void SetMongoDbSagaRepositoryProvider(this IRegistrationConfigurator configurator, IMongoDatabase database,
            Action<IMongoDbSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new MongoDbSagaRepositoryRegistrationProvider(r =>
            {
                r.Database(database);

                configure?.Invoke(r);
            }));
        }
    }
}
