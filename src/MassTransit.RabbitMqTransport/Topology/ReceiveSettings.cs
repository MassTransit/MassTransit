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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Specify the receive settings for a receive transport
    /// </summary>
    public interface ReceiveSettings :
        EntitySettings
    {
        /// <summary>
        /// The queue name to receive from
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// The number of unacknowledged messages to allow to be processed concurrently
        /// </summary>
        ushort PrefetchCount { get; }

        /// <summary>
        /// True if the queue receive should be exclusive and not shared
        /// </summary>
        bool Exclusive { get; }

        /// <summary>
        /// Arguments passed to QueueDeclare
        /// </summary>
        IDictionary<string, object> QueueArguments { get; }

        string RoutingKey { get; }

        IDictionary<string, object> BindingArguments { get; }

        /// <summary>
        /// If True, and a queue name is specified, if the queue exists and has messages, they are purged at startup
        /// If the connection is reset, messages are not purged until the service is reset
        /// </summary>
        bool PurgeOnStartup { get; }

        /// <summary>
        /// Arguments passed to the basicConsume
        /// </summary>
        IDictionary<string, object> ConsumeArguments { get; }

        /// <summary>
        /// Should the consumer have exclusive access to the queue
        /// </summary>
        bool ExclusiveConsumer { get; }

        /// <summary>
        /// When the queue should expire
        /// </summary>
        TimeSpan? QueueExpiration { get; }

        /// <summary>
        /// If false, deploys only exchange, without queue
        /// </summary>
        bool BindQueue { get; }

        /// <summary>
        /// Get the input address for the transport on the specified host
        /// </summary>
        Uri GetInputAddress(Uri hostAddress);
    }
}