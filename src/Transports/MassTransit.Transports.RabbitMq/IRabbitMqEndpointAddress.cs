// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq
{
    using System.Collections.Generic;
    using RabbitMQ.Client;


    public interface IRabbitMqEndpointAddress :
        IEndpointAddress
    {
        ConnectionFactory ConnectionFactory { get; }
        string Name { get; }

        /// <summary>
        /// The prefetch count for consumers
        /// </summary>
        ushort PrefetchCount { get; }

        /// <summary>
        /// If bound to a queue, the queue should be exclusive
        /// </summary>
        bool Exclusive { get; }

        /// <summary>
        /// If bound to a queue, the queue should be durable
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// If bound to a queue, the queue should automatically be deleted when connection closed
        /// </summary>
        bool AutoDelete { get; }

        IRabbitMqEndpointAddress ForQueue(string name);

        IDictionary<string,object> QueueArguments();
    }
}