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
    using Util;


    /// <summary>
    /// A service bus namespace which has the appropropriate messaging factories available
    /// </summary>
    public interface NamespaceContext :
        PipeContext,
        IReceiveObserver,
        IReceiveTransportObserver
    {
        /// <summary>
        /// The messaging factory initialized for the service bus
        /// </summary>
        Task<MessagingFactory> MessagingFactory { get; }

        /// <summary>
        /// The messaging factory initialized for the service bus
        /// </summary>
        Task<MessagingFactory> SessionMessagingFactory { get; }

        /// <summary>
        /// The namespace manager for the service bus
        /// </summary>
        NamespaceManager NamespaceManager { get; }

        /// <summary>
        /// The address of the service bus namespace, including any scope specified at host configuration
        /// </summary>
        Uri ServiceAddress { get; }

        /// <summary>
        /// return the path of the queue for this connection
        /// </summary>
        /// <param name="queueDescription"></param>
        /// <returns></returns>
        string GetQueuePath(QueueDescription queueDescription);

        /// <summary>
        /// Create a queue in the host namespace (which is scoped to the full ServiceUri)
        /// </summary>
        /// <param name="queueDescription"></param>
        /// <returns></returns>
        Task<QueueDescription> CreateQueue(QueueDescription queueDescription);

        /// <summary>
        /// Create a topic in the root namespace
        /// </summary>
        /// <param name="topicDescription"></param>
        /// <returns></returns>
        Task<TopicDescription> CreateTopic(TopicDescription topicDescription);

        /// <summary>
        /// Create a topic subscription
        /// </summary>
        /// <param name="subscriptionDescription"></param>
        /// <returns></returns>
        Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription subscriptionDescription);


        /// <summary>
        /// Create topic subscription in the root namespace
        /// </summary>
        /// <param name="subscriptionName"></param>
        /// <param name="topicPath"></param>
        /// <param name="queuePath"></param>
        /// <param name="queueDescription"></param>
        /// <returns></returns>
        Task<SubscriptionDescription> CreateTopicSubscription(string subscriptionName, string topicPath, string queuePath, QueueDescription queueDescription);


        /// <summary>
        /// Creates a scope that has it's own participants that can be coordinated
        /// </summary>
        /// <returns></returns>
        ITaskScope CreateScope(string tag);

        /// <summary>
        /// Delete a subscription from the topic
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        Task DeleteTopicSubscription(SubscriptionDescription description);
    }
}