// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AzureServiceBusTransport
{
    using System;
#if !NETCORE
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.ServiceBus.Messaging.Amqp;
#else
    using Microsoft.Azure.ServiceBus;
#endif

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
        TokenProvider TokenProvider { get; }

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

        /// <summary>
        /// The AMQP settings
        /// </summary>
        AmqpTransportSettings AmqpTransportSettings { get; }

        /// <summary>
        /// The net messaging settings
        /// </summary>
        NetMessagingTransportSettings NetMessagingTransportSettings { get; }
    }
}
