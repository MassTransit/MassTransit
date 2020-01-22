namespace MassTransit.MongoDbIntegration
{
    using System;
    using Configurators;
    using MassTransit.Configurators;
    using MongoDB.Driver;
    using Saga;


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
            where TSaga : class, IVersionedSaga
        {
            var mongoDbConfigurator = new MongoDbSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(mongoDbConfigurator);

            BusConfigurationResult.CompileResults(mongoDbConfigurator.Validate());

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
            where TSaga : class, IVersionedSaga =>
            configurator.MongoDbRepository(cfg =>
            {
                cfg.Connection = connectionString;
                configure?.Invoke(cfg);
            });

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
            where TSaga : class, IVersionedSaga =>
            configurator.MongoDbRepository(cfg =>
            {
                cfg.Database(database);
                configure?.Invoke(cfg);
            });
    }
}
