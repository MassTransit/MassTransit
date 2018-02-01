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
namespace MassTransit.ActiveMqTransport.Topology
{
    using Entities;


    public interface IBrokerTopologyBuilder
    {
        /// <summary>
        /// Declares an exchange
        /// </summary>
        /// <param name="name">The exchange name</param>
        /// <param name="durable">A durable exchange survives a broker restart</param>
        /// <param name="autoDelete">Automatically delete if the broker connection is closed</param>
        /// <returns>An entity handle used to reference the exchange in subsequent calls</returns>
        TopicHandle CreateTopic(string name, bool durable, bool autoDelete);

        /// <summary>
        /// Declares a queue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="durable"></param>
        /// <param name="autoDelete"></param>
        /// <returns></returns>
        QueueHandle CreateQueue(string name, bool durable, bool autoDelete);

        /// <summary>
        /// Binds an exchange to a queue, with the specified routing key and arguments
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="queue"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        ConsumerHandle BindConsumer(TopicHandle topic, QueueHandle queue, string selector);
    }
}