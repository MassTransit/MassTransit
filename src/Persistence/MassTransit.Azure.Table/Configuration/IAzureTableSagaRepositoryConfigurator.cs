namespace MassTransit.Azure.Table
{
    using System;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;


    public interface IAzureTableSagaRepositoryConfigurator<TSaga> :
        IAzureTableSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
    }


    public interface IAzureTableSagaRepositoryConfigurator
    {
        /// <summary>
        /// Use a simple factory method to create the connection
        /// </summary>
        /// <param name="connectionFactory"></param>
        void ConnectionFactory(Func<CloudTable> connectionFactory);
    }
}
