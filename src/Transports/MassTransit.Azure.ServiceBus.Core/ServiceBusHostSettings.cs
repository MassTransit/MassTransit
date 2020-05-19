namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Primitives;


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
        /// The token provider to access the namespace
        /// </summary>
        ITokenProvider TokenProvider { get; }

        /// <summary>
        /// The operation timeout for timing out operations
        /// </summary>
        TimeSpan OperationTimeout { get; }

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
        /// The type of transport to use AMQP or NetMessaging
        /// </summary>
        TransportType TransportType { get; }
    }
}
