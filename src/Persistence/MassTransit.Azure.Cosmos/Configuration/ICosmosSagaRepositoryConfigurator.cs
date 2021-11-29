namespace MassTransit
{
    using System;
    using Microsoft.Azure.Cosmos;


    public interface ICosmosSagaRepositoryConfigurator
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
        /// Use CollectionId formatter
        /// </summary>
        /// <param name="collectionIdFormatter"></param>
        void SetCollectionIdFormatter(ICosmosCollectionIdFormatter collectionIdFormatter);

        /// <summary>
        /// Use CollectionId formatter
        /// </summary>
        /// <param name="collectionIdFormatterFactory"></param>
        void SetCollectionIdFormatter(Func<IServiceProvider, ICosmosCollectionIdFormatter> collectionIdFormatterFactory);

        /// <summary>
        /// Configure the ItemRequestOptions
        /// </summary>
        void ConfigureItemRequestOptions(Action<ItemRequestOptions> cfg);

        /// <summary>
        /// Configure the QueryRequestOptions
        /// </summary>
        void ConfigureQueryRequestOptions(Action<QueryRequestOptions> cfg);

        /// <summary>
        /// Triggers the creation of <see cref="CosmosClient" />s through the <see cref="ICosmosClientFactory" />. When
        /// the client factory is used, the Endpoint and Key will be ignored and the creation of the clients will only
        /// be done using the factory.
        /// </summary>
        /// <remarks>
        /// An instance of <see cref="ICosmosClientFactory" /> must be added to the dependency injection container.
        /// </remarks>
        /// <param name="clientName">The name of the <see cref="CosmosClient" /> that will be used</param>
        void UseClientFactory(string clientName);
    }


    public interface ICosmosSagaRepositoryConfigurator<TSaga> :
        ICosmosSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
    }
}
