// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    public interface ErrorQueueSettings
    {
        /// <summary>
        /// The exchange name to bind to the queue as the default exchange
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// The queue name of the error queue
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// True if messages should be persisted to disk for the queue
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the queue/exchange should automatically be deleted
        /// </summary>
        bool AutoDelete { get; }
    }


    public class RabbitMqErrorQueueSettings :
        ErrorQueueSettings
    {
        public string ExchangeName { get; set; }

        public string QueueName { get; set; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }
    }
}