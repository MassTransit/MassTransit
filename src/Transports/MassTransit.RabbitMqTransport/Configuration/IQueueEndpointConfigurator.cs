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
namespace MassTransit.RabbitMqTransport
{
    public interface IQueueEndpointConfigurator :
        IQueueConfigurator
    {
        /// <summary>
        /// Specify the maximum number of concurrent messages that are consumed
        /// </summary>
        /// <value>The limit</value>
        ushort PrefetchCount { set; }

        /// <summary>
        /// Purge the messages from an existing queue on startup (note that upon reconnection to the server
        /// the queue will not be purged again, only when the service is restarted).
        /// </summary>
        bool PurgeOnStartup { set; }

        /// <summary>
        /// Sets the priority of the consumer (optional, no default value specified)
        /// </summary>
        int ConsumerPriority { set; }

        /// <summary>
        /// Should the consumer have exclusive access to the queue
        /// </summary>
        bool ExclusiveConsumer { set; }
    }
}