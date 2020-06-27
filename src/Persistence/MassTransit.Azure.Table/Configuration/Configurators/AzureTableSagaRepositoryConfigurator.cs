namespace MassTransit.Azure.Table.Configurators
{
    using System;
    using System.Collections.Generic;
    using Contexts;
    using GreenPipes;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;
    using Registration;
    using Saga;


    public class AzureTableSagaRepositoryConfigurator<TSaga> :
        IAzureTableSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        Func<IConfigurationServiceProvider, CloudTable> _connectionFactory;

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
