namespace MassTransit
{
    using System;
    using AzureCosmos;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public static class CosmosRepositoryConfigurationExtensions
    {
        /// <summary>
        /// Configures the Cosmos Saga Repository
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> CosmosRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<ICosmosSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new CosmosSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The CosmosDb saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Configures the Cosmos Saga Repository.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The account endpoint of the database</param>
        /// <param name="authKeyOrResourceToken">The authentication key or resource token for the database</param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> CosmosRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            string accountEndpoint, string authKeyOrResourceToken, Action<ICosmosSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new CosmosSagaRepositoryConfigurator<TSaga>
            {
                EndpointUri = accountEndpoint,
                Key = authKeyOrResourceToken
            };

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The CosmosDb saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the Cosmos saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The endpointUri of the database</param>
        /// <param name="key">The authentication key of the database</param>
        /// <param name="configure"></param>
        public static void SetCosmosSagaRepositoryProvider(this IRegistrationConfigurator configurator, string accountEndpoint, string key,
            Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new CosmosSagaRepositoryRegistrationProvider(x =>
            {
                x.EndpointUri = accountEndpoint;
                x.Key = key;

                configure?.Invoke(x);
            }));
        }

        /// <summary>
        /// Use the Cosmos saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetCosmosSagaRepositoryProvider(this IRegistrationConfigurator configurator, Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new CosmosSagaRepositoryRegistrationProvider(configure));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="accountEndpoint">The account endpoint of the database</param>
        /// <param name="authKeyOrResourceToken">The authentication key or resource token for the database</param>
        public static IServiceCollection AddCosmosClientFactory(this IServiceCollection collection, string accountEndpoint, string authKeyOrResourceToken)
        {
            return collection.AddSingleton<ICosmosClientFactory>(provider => new StaticCosmosClientFactory(accountEndpoint, authKeyOrResourceToken));
        }
    }
}
