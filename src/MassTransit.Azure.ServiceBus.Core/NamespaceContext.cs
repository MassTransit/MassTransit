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
namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    /// <summary>
    /// A service bus namespace which has the appropriate messaging factories available
    /// </summary>
    public interface NamespaceContext :
        PipeContext
    {
        /// <summary>
        /// The address of the service bus namespace, including any scope specified at host configuration
        /// </summary>
        Uri ServiceAddress { get; }

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
        /// <param name="rule"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription subscriptionDescription, RuleDescription rule, Filter filter);

        /// <summary>
        /// Delete a subscription from the topic
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        Task DeleteTopicSubscription(SubscriptionDescription description);
    }
}