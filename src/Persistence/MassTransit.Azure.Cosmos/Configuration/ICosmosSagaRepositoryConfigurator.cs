using Microsoft.Azure.Cosmos;
using System;

namespace MassTransit.Azure.Cosmos
{
    public interface ICosmosSagaRepositoryConfigurator<TSaga>
        where TSaga : class, IVersionedSaga
    {
        string EndpointUri { set; }
        string Key { set; }

        string DatabaseId { set; }

        string CollectionId { set; }

        /// <summary>
        /// Configure the ConnectionString to use the Cosmos Emulator
        /// </summary>
        void ConfigureEmulator();

        /// <summary>
        /// Configure the ItemRequestOptions
        /// </summary>
        void ConfigureItemRequestOptions(Action<ItemRequestOptions> cfg);

        /// <summary>
        /// Configure the QueryRequestOptions
        /// </summary>
        void ConfigureQueryRequestOptions(Action<QueryRequestOptions> cfg);

        /// <summary>
        /// Configure the PartitionKey Expression
        /// </summary>
        void AddPartitionKeyExpression(Func<TSaga, PartitionKey> expression);
    }
}
