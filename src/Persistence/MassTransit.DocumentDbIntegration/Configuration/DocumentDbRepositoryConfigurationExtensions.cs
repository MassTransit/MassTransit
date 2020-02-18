namespace MassTransit.DocumentDbIntegration
{
    using System;
    using Configuration;
    using Configurators;


    public static class DocumentDbRepositoryConfigurationExtensions
    {
        /// <summary>
        /// Configures the DocumentDb Saga Repository
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> DocumentDbRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<IDocumentDbSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, IVersionedSaga
        {
            var repositoryConfigurator = new DocumentDbSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(repositoryConfigurator);

            BusConfigurationResult.CompileResults(repositoryConfigurator.Validate());

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Configures the DocumentDb Saga Repository.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="endpointUri">The endpointUri of the database</param>
        /// <param name="key">The authentication key of the database</param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> DocumentDbRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Uri endpointUri, string key, Action<IDocumentDbSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, IVersionedSaga
        {
            var repositoryConfigurator = new DocumentDbSagaRepositoryConfigurator<TSaga>
            {
                EndpointUri = endpointUri,
                Key = key
            };

            configure?.Invoke(repositoryConfigurator);

            BusConfigurationResult.CompileResults(repositoryConfigurator.Validate());

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }
    }
}
