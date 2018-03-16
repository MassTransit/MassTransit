// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Pipeline;
    using Topology;


    /// <summary>
    /// An Azure ServiceBus Host, which caches the messaging factory and namespace manager
    /// </summary>
    public interface IServiceBusHost :
        IHost
    {
        ServiceBusHostSettings Settings { get; }

        /// <summary>
        /// The base path used for queues on this host
        /// </summary>
        string BasePath { get; }

        /// <summary>
        /// Returns the topology of the service bus host
        /// </summary>
        new IServiceBusHostTopology Topology { get; }

        /// <summary>
        /// The default messaging factory cache, could be AMQP or NET-TCP, depending upon configuration
        /// </summary>
        IMessagingFactoryCache MessagingFactoryCache { get; }

        /// <summary>
        /// The messaging factory cache for NET-TCP (may be the same as above, depending upon configuration)
        /// </summary>
        IMessagingFactoryCache NetMessagingFactoryCache { get; }

        /// <summary>
        /// The namespace cache for operating on the service bus namespace (management)
        /// </summary>
        INamespaceCache NamespaceCache { get; }

        /// <summary>
        /// The retry policy shared by transports communicating with the host. Should be
        /// used for all operations against Azure.
        /// </summary>
        IRetryPolicy RetryPolicy { get; }

        /// <summary>
        /// Create a temporary receive endpoint on the host, with a separate handle for stopping/removing the endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IServiceBusReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Create a receive endpoint on the host, with a separate handle for stopping/removing the endpoint
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Create a subscription endpoint on the host, which can be stopped independently from the bus
        /// </summary>
        /// <typeparam name="T">The topic message type</typeparam>
        /// <param name="subscriptionName">The subscription name for this endpoint</param>
        /// <param name="configure">Configuration callback for the endpoint</param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName,Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class;

        /// <summary>
        /// Create a subscription endpoint on the host, which can be stopped independently from the bus
        /// </summary>
        /// <param name="subscriptionName">The subscription name for this endpoint</param>
        /// <param name="topicName">The topic name to subscribe for this endpoint</param>
        /// <param name="configure">Configuration callback for the endpoint</param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null);
    }
}