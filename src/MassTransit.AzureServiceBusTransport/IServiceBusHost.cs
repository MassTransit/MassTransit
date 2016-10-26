// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Transports;
    using Util;


    /// <summary>
    /// An Azure ServiceBus Host, which caches the messaging factory and namespace manager
    /// </summary>
    public interface IServiceBusHost :
        IHost
    {
        ServiceBusHostSettings Settings { get; }

        Task<MessagingFactory> MessagingFactory { get; }

        /// <summary>
        /// Session-based messages with state require the use of a net-tcp style client
        /// </summary>
        Task<MessagingFactory> SessionMessagingFactory { get; }

        NamespaceManager NamespaceManager { get; }

        NamespaceManager RootNamespaceManager { get; }

        IMessageNameFormatter MessageNameFormatter { get; }

        /// <summary>
        /// The supervisor for the host, which indicates when it's being stopped
        /// </summary>
        ITaskSupervisor Supervisor { get; }

        /// <summary>
        /// The retry policy shared by transports communicating with the host. Should be
        /// used for all operations against Azure.
        /// </summary>
        IRetryPolicy RetryPolicy { get; }

        string GetQueuePath(QueueDescription queueDescription);

        Task<TopicDescription> CreateTopic(TopicDescription topicDescription);

        Task<QueueDescription> CreateQueue(QueueDescription queueDescription);

        Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription description);

        /// <summary>
        /// Create a temporary receive endpoint on the host, with a separate handle for stopping/removing the endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task<HostReceiveEndpointHandle> ConnectReceiveEndpoint(Action<IServiceBusReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Create a receive endpoint on the host, with a separate handle for stopping/removing the endpoint
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task<HostReceiveEndpointHandle> ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null);

        Task<HostReceiveEndpointHandle> ConnectSubscriptionEndpoint<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class;

        Task<HostReceiveEndpointHandle> ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null);
    }
}