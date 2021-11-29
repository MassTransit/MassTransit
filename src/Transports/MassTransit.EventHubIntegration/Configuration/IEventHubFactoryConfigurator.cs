namespace MassTransit
{
    using System;
    using Azure.Core;
    using Azure.Messaging.EventHubs.Producer;
    using Azure.Storage;
    using Azure.Storage.Blobs;


    public interface IEventHubFactoryConfigurator :
        IRiderFactoryConfigurator,
        ISendObserverConnector,
        ISendPipelineConfigurator
    {
        /// <summary>
        /// Configure EventHub namespace using connection string
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to use for connecting to the Event Hubs namespace; it is expected that the Event Hub name and the shared key properties are contained in
        /// this connection
        /// string
        /// </param>
        void Host(string connectionString);

        /// <summary>
        /// Configure EventHub namespace using fullyQualifiedNamespace + tokenCredential
        /// </summary>
        /// <param name="fullyQualifiedNamespace">
        /// The fully qualified Event Hubs namespace to connect to.  This is likely to be similar to <c>{yournamespace}.servicebus.windows.net</c>.
        /// </param>
        /// <param name="tokenCredential">
        /// The Azure identity credential to use for authorization.  Access controls may be specified by the Event Hubs namespace or the requested Event Hub, depending on
        /// Azure configuration.
        /// </param>
        void Host(string fullyQualifiedNamespace, TokenCredential tokenCredential);

        /// <summary>
        /// Configure Blob storage using connection string
        /// </summary>
        /// <param name="connectionString">
        /// A connection string includes the authentication information
        /// required for your application to access data in an Azure Storage
        /// account at runtime.
        /// For more information, <see href="https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string" />.
        /// </param>
        /// <param name="configure"></param>
        void Storage(string connectionString, Action<BlobClientOptions> configure = null);

        /// <summary>
        /// </summary>
        /// <param name="containerUri">
        /// A <see cref="Uri" /> referencing the blob container that includes the
        /// name of the account and the name of the container.
        /// </param>
        /// <param name="configure"></param>
        void Storage(Uri containerUri, Action<BlobClientOptions> configure = null);

        /// <summary>
        /// </summary>
        /// <param name="containerUri">
        /// A <see cref="Uri" /> referencing the blob container that includes the
        /// name of the account and the name of the container.
        /// </param>
        /// <param name="credential">The token credential used to sign requests.</param>
        /// <param name="configure"></param>
        void Storage(Uri containerUri, TokenCredential credential, Action<BlobClientOptions> configure = null);

        /// <summary>
        /// </summary>
        /// <param name="containerUri">
        /// A <see cref="Uri" /> referencing the blob container that includes the
        /// name of the account and the name of the container.
        /// </param>
        /// <param name="credential">The shared key credential used to sign requests.</param>
        /// <param name="configure"></param>
        void Storage(Uri containerUri, StorageSharedKeyCredential credential, Action<BlobClientOptions> configure = null);

        /// <summary>
        /// Subscribe to EventHub messages
        /// </summary>
        /// <param name="eventHubName">EventHub Name</param>
        /// <param name="consumerGroup">Consumer Group</param>
        /// <param name="configure"></param>
        void ReceiveEndpoint(string eventHubName, string consumerGroup, Action<IEventHubReceiveEndpointConfigurator> configure);

        /// <summary>
        /// Sets the outbound message serializer
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="isSerializer"></param>
        void AddSerializer(ISerializerFactory factory, bool isSerializer = true);

        /// <summary>
        /// Configure Producer options
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureProducerOptions(Action<EventHubProducerClientOptions> configure);
    }
}
