namespace MassTransit.Azure.Cosmos
{
    using System;
    using Microsoft.Azure.Cosmos;
    using Registration;
    using Saga.CollectionIdFormatters;


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
        void SetCollectionIdFormatter(ICollectionIdFormatter collectionIdFormatter);

        /// <summary>
        /// Use CollectionId formatter
        /// </summary>
        /// <param name="collectionIdFormatterFactory"></param>
        void SetCollectionIdFormatter(Func<IConfigurationServiceProvider, ICollectionIdFormatter> collectionIdFormatterFactory);

        /// <summary>
        /// Configure the ItemRequestOptions
        /// </summary>
        void ConfigureItemRequestOptions(Action<ItemRequestOptions> cfg);

        /// <summary>
        /// Configure the QueryRequestOptions
        /// </summary>
        void ConfigureQueryRequestOptions(Action<QueryRequestOptions> cfg);
    }


    public interface ICosmosSagaRepositoryConfigurator<TSaga> :
        ICosmosSagaRepositoryConfigurator
    {
    }
}
