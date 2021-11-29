namespace MassTransit
{
    using System;
    using Azure;
    using Azure.Core;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;


    /// <summary>
    /// The host settings used to configure the service bus connection
    /// </summary>
    public interface ServiceBusHostSettings
    {
        /// <summary>
        /// The address of the service bus namespace (and accompanying service scope)
        /// </summary>
        Uri ServiceUri { get; }

        /// <summary>
        /// A custom client that will be used instead of one defined by the settings provided here.
        /// </summary>
        ServiceBusClient ServiceBusClient { get; }

        /// <summary>
        /// A custom administration client that will be used instead of one defined by the settings provided here.
        /// </summary>
        ServiceBusAdministrationClient ServiceBusAdministrationClient { get; }

        /// <summary>
        /// The named key to use to connect to the Service Bus
        /// </summary>
        /// <remarks>
        /// This property cannot be used if <see cref="SasCredential" /> or <see cref="TokenCredential" />
        /// is being used.
        /// </remarks>
        AzureNamedKeyCredential NamedKeyCredential { get; }

        /// <summary>
        /// The shared access signature to use to connect to the Service Bus
        /// </summary>
        /// <remarks>
        /// This property cannot be used if <see cref="NamedKeyCredential" /> or <see cref="TokenCredential" />
        /// is being used.
        /// </remarks>
        AzureSasCredential SasCredential { get; }

        /// <summary>
        /// The token credential to use to connect to the Service Bus
        /// </summary>
        /// <remarks>
        /// This property cannot be used if <see cref="SasCredential" /> or <see cref="NamedKeyCredential" />
        /// is being used.
        /// </remarks>
        TokenCredential TokenCredential { get; }

        /// <summary>
        /// The connection string to use to connect to the Service Bus
        /// </summary>
        /// <remarks>
        /// If a credential is not part of the connection string, one of the other authentication
        /// methods needs to be used. <see cref="NamedKeyCredential" /> or <see cref="SasCredential" />
        /// or <see cref="TokenCredential" />
        /// </remarks>
        string ConnectionString { get; }

        /// <summary>
        /// The minimum back off interval for the exponential retry policy
        /// </summary>
        TimeSpan RetryMinBackoff { get; }

        /// <summary>
        /// The maximum back off interval for the exponential retry policy
        /// </summary>
        TimeSpan RetryMaxBackoff { get; }

        /// <summary>
        /// The retry limit for service bus operations
        /// </summary>
        int RetryLimit { get; }

        /// <summary>
        /// The type of transport to use AMQP TCP or AMQP Websockets
        /// </summary>
        ServiceBusTransportType TransportType { get; }
    }
}
