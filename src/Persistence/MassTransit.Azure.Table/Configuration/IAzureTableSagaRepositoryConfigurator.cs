namespace MassTransit
{
    using System;
    using AzureTable;
    using Microsoft.Azure.Cosmos.Table;


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
        void ConnectionFactory(Func<IServiceProvider, CloudTable> connectionFactory);
    }
}
