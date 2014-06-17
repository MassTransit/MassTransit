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
namespace MassTransit.Transports.RabbitMq
{
    using System.Collections.Generic;


    public interface ReceiveConsumerSettings
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

        bool Durable { get; }

        bool Exclusive { get; }


        bool AutoDelete { get; }

        IDictionary<string, object> QueueArguments { get; }
        IDictionary<string, object> ExchangeArguments { get; }

        bool PurgeOnReceive { get; }

        string ExchangeType { get; }
    }
}