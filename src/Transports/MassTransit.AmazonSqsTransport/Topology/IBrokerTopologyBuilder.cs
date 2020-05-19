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
namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Collections.Generic;
    using Entities;


    public interface IBrokerTopologyBuilder
    {
        /// <summary>
        /// Declares an exchange
        /// </summary>
        /// <param name="name">The topic name</param>
        /// <param name="durable">A durable topic survives a broker restart</param>
        /// <param name="autoDelete">Automatically delete if the broker connection is closed</param>
        /// <param name="topicAttributes"></param>
        /// <param name="topicSubscriptionAttributes"></param>
        /// <param name="tags"></param>
        /// <returns>An entity handle used to reference the topic in subsequent calls</returns>
        TopicHandle CreateTopic(string name, bool durable, bool autoDelete, IDictionary<string, object> topicAttributes = null, IDictionary<string, object> topicSubscriptionAttributes = null, IDictionary<string, string> tags = null);

        /// <summary>
        /// Declares a queue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="durable"></param>
        /// <param name="autoDelete"></param>
        /// <param name="queueAttributes"></param>
        /// <param name="queueSubscriptionAttributes"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        QueueHandle CreateQueue(string name, bool durable, bool autoDelete, IDictionary<string, object> queueAttributes = null, IDictionary<string, object> queueSubscriptionAttributes = null, IDictionary<string, string> tags = null);

        /// <summary>
        /// Create a subscription on a topic to a queue
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue);

        /// <summary>
        /// Create a subscription on a topic to another topic
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination);
    }
}
