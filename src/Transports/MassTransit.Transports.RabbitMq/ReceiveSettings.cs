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


    /// <summary>
    /// Specify the receive settings for a receive transport
    /// </summary>
    public interface ReceiveSettings
    {
        /// <summary>
        /// The queue name to receive from
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// The exchange name to bind to the queue as the default exchange
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// The number of unacknowledged messages to allow to be processed concurrently
        /// </summary>
        ushort PrefetchCount { get; }

        /// <summary>
        /// True if messages should be persisted to disk for the queue
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the queue receive should be exclusive and not shared
        /// </summary>
        bool Exclusive { get; }

        /// <summary>
        /// True if the queue/exchange should automatically be deleted
        /// </summary>
        bool AutoDelete { get; }

        /// <summary>
        /// Arguments passed to QueueDeclare
        /// </summary>
        IDictionary<string, object> QueueArguments { get; }

        /// <summary>
        /// Arguments passed to exchange-declare
        /// </summary>
        IDictionary<string, object> ExchangeArguments { get; }

        /// <summary>
        /// If True, and a queue name is specified, if the queue exists and has messages, they are purged at startup
        /// If the connection is reset, messages are not purged until the service is reset
        /// </summary>
        bool PurgeOnStartup { get; }

        /// <summary>
        /// The RabbitMQ exchange type
        /// </summary>
        string ExchangeType { get; }
    }
}