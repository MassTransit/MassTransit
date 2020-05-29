namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Primitives;


    public interface IServiceBusHostConfigurator
    {
        /// <summary>
        /// Sets the TokenProvider for the host
        /// </summary>
        ITokenProvider TokenProvider { set; }

        /// <summary>
        /// Sets the operation timeout for the messaging factory
        /// </summary>
        TimeSpan OperationTimeout { set; }

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
        /// Sets the messaging protocol to use with the messaging factory, defaults to AMQP.
        /// </summary>
        TransportType TransportType { set; }
    }
}
