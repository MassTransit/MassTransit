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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using MassTransit.Topology;


    public interface IRabbitMqMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IRabbitMqMessagePublishTopology<TMessage>,
        IRabbitMqMessagePublishTopologyConfigurator
        where TMessage : class
    {
        /// <summary>
        /// Specifies the alternate exchange for the published message exchange, which is where messages are sent if no
        /// queues receive the message.
        /// </summary>
        string AlternateExchange { set; }

        /// <summary>
        /// Bind an exchange to a queue
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        void BindQueue(string exchangeName, string queueName, Action<IQueueBindingConfigurator> configure = null);

        /// <summary>
        /// Bind an alternate exchange/queue for the published message type
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        void BindAlterateExchangeQueue(string exchangeName, string queueName = null, Action<IQueueBindingConfigurator> configure = null);
    }


    public interface IRabbitMqMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator,
        IExchangeConfigurator
    {
    }
}