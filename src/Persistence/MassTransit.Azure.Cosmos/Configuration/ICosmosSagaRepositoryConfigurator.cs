#nullable enable
namespace MassTransit
{
    using System;
    using System.Text.Json;
    using Azure.Core;
    using Microsoft.Azure.Cosmos;


    public interface ICosmosSagaRepositoryConfigurator
    {
        string AccountEndpoint { set; }

        string AuthKeyOrResourceToken { set; }

        string ConnectionString { set; }

        string CollectionId { set; }

        string DatabaseId { set; }

        [Obsolete("Use AccountEndpoint")]
        string EndpointUri { set; }

        [Obsolete("Use AuthKeyOrResourceToken")]
        string Key { set; }

        TokenCredential TokenCredential { set; }

        /// <summary>
        /// Set the JSON naming policy, which defaults to CamelCase, to something else, or NULL to use the default PascalCase.
        /// </summary>
        JsonNamingPolicy? PropertyNamingPolicy { set; }

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
        /// Configure the LinqSerializerOptions, these are configured by default is using System.Text.Json, but provided in case they are needed
        /// </summary>
        /// <param name="cfg"></param>
        void ConfigureLinqSerializerOptions(Action<CosmosLinqSerializerOptions> cfg);

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

        /// <summary>
        /// Use the previous Newtonsoft JSON serializer, instead of the new System.Text.Json serializer
        /// </summary>
        void UseNewtonsoftJson();
    }


    public interface ICosmosSagaRepositoryConfigurator<TSaga> :
        ICosmosSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
    }
}
