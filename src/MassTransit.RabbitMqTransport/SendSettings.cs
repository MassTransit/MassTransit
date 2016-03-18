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
namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Generic;
    using Topology;


    public interface SendSettings
    {
        /// <summary>
        /// The exchange name to bind to the queue as the default exchange
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// True if messages should be persisted to disk for the queue
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the queue/exchange should automatically be deleted
        /// </summary>
        bool AutoDelete { get; }

        /// <summary>
        /// Arguments passed to exchange-declare
        /// </summary>
        IDictionary<string, object> ExchangeArguments { get; }

        /// <summary>
        /// The RabbitMQ exchange type
        /// </summary>
        string ExchangeType { get; }

        /// <summary>
        /// True if the exchange should be bound to a queue on send (for error queues, etc.)
        /// </summary>
        bool BindToQueue { get; }

        /// <summary>
        /// The queue to create/bind to the exchange
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// Arguments passed to QueueDeclare
        /// </summary>
        IDictionary<string, object> QueueArguments { get; }

        /// <summary>
        /// The exchange bindings for the send
        /// </summary>
        IEnumerable<ExchangeBindingSettings> ExchangeBindings { get; } 
    }
}