namespace MassTransit.Azure.Table
{
    using System;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;
    using Registration;
    using Saga;


    public interface IAzureTableSagaRepositoryConfigurator<TSaga> :
        IAzureTableSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Use a factory method to create the key formatter
        /// </summary>
        /// <param name="formatterFactory"></param>
        void KeyFormatter(Func<ISagaKeyFormatter<TSaga>> formatterFactory);
    }


    public interface IAzureTableSagaRepositoryConfigurator
    {
        /// <summary>
        /// Use a simple factory method to create the connection
        /// </summary>
        /// <param name="connectionFactory"></param>
        void ConnectionFactory(Func<CloudTable> connectionFactory);

        /// <summary>
        /// Supply factory for retrieving the Cloud Table.
        /// </summary>
        /// <param name="connectionFactory"></param>
        void ConnectionFactory(Func<IConfigurationServiceProvider, CloudTable> connectionFactory);
    }
}
