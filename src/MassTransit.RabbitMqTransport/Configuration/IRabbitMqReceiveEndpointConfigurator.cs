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
namespace MassTransit
{
    /// <summary>
    /// Configure a receiving RabbitMQ endpoint
    /// </summary>
    public interface IRabbitMqReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Specify the maximum number of concurrent messages that are consumed
        /// </summary>
        /// <value>The limit</value>
        ushort PrefetchCount { set; }

        /// <summary>
        /// Specify the queue should be durable (survives broker restart) or in-memory
        /// </summary>
        /// <param name="durable">True for a durable queue, False for an in-memory queue</param>
        void Durable(bool durable = true);

        /// <summary>
        /// Specify that the queue is exclusive to this process and cannot be accessed by other processes
        /// at the same time.
        /// </summary>
        /// <param name="exclusive">True for exclusive, otherwise false</param>
        void Exclusive(bool exclusive = true);

        /// <summary>
        /// Specify that the queue (and the exchange of the same name) should be created as auto-delete
        /// </summary>
        /// <param name="autoDelete"></param>
        void AutoDelete(bool autoDelete = true);

        /// <summary>
        /// Specify the exchange type for the endpoint
        /// </summary>
        /// <param name="exchangeType"></param>
        void ExchangeType(string exchangeType);

        /// <summary>
        /// Purge the messages from an existing queue on startup (note that upon reconnection to the server
        /// the queue will not be purged again, only when the service is restarted).
        /// </summary>
        /// <param name="purgeOnStartup"></param>
        void PurgeOnStartup(bool purgeOnStartup = true);

        /// <summary>
        /// Set a queue argument passed to the broker on queue declaration
        /// </summary>
        /// <param name="key">The argument key</param>
        /// <param name="value">The argument value</param>
        void SetQueueArgument(string key, object value);

        /// <summary>
        /// Set an exchange argument passed to the broker on queue declaration
        /// </summary>
        /// <param name="key">The argument key</param>
        /// <param name="value">The argument value</param>
        void SetExchangeArgument(string key, object value);
    }
}