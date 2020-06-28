namespace MassTransit.Azure.Cosmos
{
    using System;
    using Configuration;
    using Configurators;
    using MassTransit.Saga;


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

            BusConfigurationResult.CompileResults(repositoryConfigurator.Validate());

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Configures the Cosmos Saga Repository.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The endpointUri of the database</param>
        /// <param name="key">The authentication key of the database</param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> CosmosRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            string accountEndpoint, string key, Action<ICosmosSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new CosmosSagaRepositoryConfigurator<TSaga>
            {
                EndpointUri = accountEndpoint,
                Key = key
            };

            configure?.Invoke(repositoryConfigurator);

            BusConfigurationResult.CompileResults(repositoryConfigurator.Validate());

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }
    }
}
