namespace MassTransit.Azure.Table
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Microsoft.Azure.Cosmos.Table;
    using Registration;
    using Saga;


    public class AzureTableSagaRepositoryConfigurator<TSaga> :
        IAzureTableSagaRepositoryConfigurator<TSaga>,
        ISpecification
    {
        Func<IConfigurationServiceProvider, CloudTable> _connectionFactory;

        public AzureTableSagaRepositoryConfigurator()
        {
        }

        /// <summary>
        /// Supply factory for retrieving the Cloud Table.
        /// </summary>
        /// <param name="connectionFactory"></param>
        public void ConnectionFactory(Func<CloudTable> connectionFactory)
        {
            _connectionFactory = provider => connectionFactory();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_connectionFactory == null)
                yield return this.Failure("ConnectionFactory", "must be specified");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, IVersionedSaga
        {
            configurator.RegisterSingleInstance(_connectionFactory);
            configurator.RegisterSagaRepository<T, DatabaseContext<T>, SagaConsumeContextFactory<DatabaseContext<T>, T>,
                AzureTableSagaRepositoryContextFactory<T>>();
        }
    }
}
