namespace MassTransit
{
    using System;
    using Azure;
    using Azure.Core;
    using Azure.Messaging.ServiceBus;


    public interface IServiceBusHostConfigurator
    {
        /// <summary>
        /// The named key to use to connect to the Service Bus
        /// </summary>
        /// <remarks>
        /// This property cannot be used if <see cref="SasCredential"/> or <see cref="TokenCredential"/>
        /// is being used.
        /// </remarks>
        AzureNamedKeyCredential NamedKeyCredential { set; }

        /// <summary>
        /// The shared access signature to use to connect to the Service Bus
        /// </summary>
        /// <remarks>
        /// This property cannot be used if <see cref="NamedKeyCredential"/> or <see cref="TokenCredential"/>
        /// is being used.
        /// </remarks>
        AzureSasCredential SasCredential { set; }

        /// <summary>
        /// The token credential to use to connect to the Service Bus
        /// </summary>
        /// <remarks>
        /// This property cannot be used if <see cref="SasCredential"/> or <see cref="NamedKeyCredential"/>
        /// is being used.
        /// </remarks>
        TokenCredential TokenCredential { set; }

        /// <summary>
        /// The connection string to use to connect to the Service Bus
        /// </summary>
        /// <remarks>
        /// If a credential is not part of the connection string, one of the other authentication
        /// methods needs to be used. <see cref="NamedKeyCredential"/> or <see cref="SasCredential"/>
        /// or <see cref="TokenCredential"/>
        /// </remarks>
        string ConnectionString { set; }

        /// <summary>
        /// The minimum back off interval for the exponential retry policy
        /// </summary>
        TimeSpan RetryMinBackoff { set; }

        /// <summary>
        /// The maximum back off interval for the exponential retry policy
        /// </summary>
        TimeSpan RetryMaxBackoff { set; }

        /// <summary>
        /// The retry limit for service bus operations
        /// </summary>
        int RetryLimit { set; }

        /// <summary>
        /// Sets the messaging protocol to use with the messaging factory, defaults to AMQP TCP.
        /// </summary>
        ServiceBusTransportType TransportType { set; }
    }
}
