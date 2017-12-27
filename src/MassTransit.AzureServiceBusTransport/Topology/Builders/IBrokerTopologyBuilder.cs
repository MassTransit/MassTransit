// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Topology.Builders
{
    using Entities;
    using Microsoft.ServiceBus.Messaging;


    public interface IBrokerTopologyBuilder
    {
        /// <summary>
        /// Creates a topic
        /// </summary>
        /// <param name="topicDescription">The immutable topic description</param>
        /// <returns>An entity handle used to reference the exchange in subsequent calls</returns>
        TopicHandle CreateTopic(TopicDescription topicDescription);

        /// <summary>
        /// Creates a subscription
        /// </summary>
        /// <param name="topic">The source exchange</param>
        /// <param name="subscriptionDescription"></param>
        /// <returns>An entity handle used to reference the binding in subsequent calls</returns>
        SubscriptionHandle CreateSubscription(TopicHandle topic, SubscriptionDescription subscriptionDescription);

        /// <summary>
        /// Creates a subscription which forwards to a different topic
        /// </summary>
        /// <param name="source">The source topic</param>
        /// <param name="destination">The destination topic</param>
        /// <param name="subscriptionDescription"></param>
        /// <returns>An entity handle used to reference the binding in subsequent calls</returns>
        TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SubscriptionDescription subscriptionDescription);

        /// <summary>
        /// Creates a queue
        /// </summary>
        /// <param name="queueDescription"></param>
        /// <returns></returns>
        QueueHandle CreateQueue(QueueDescription queueDescription);

        /// <summary>
        /// Creates a subscription which forwards to a queue
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="subscriptionDescription"></param>
        /// <returns></returns>
        QueueSubscriptionHandle CreateQueueSubscription(TopicHandle exchange, QueueHandle queue, SubscriptionDescription subscriptionDescription);
    }
}