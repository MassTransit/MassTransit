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
namespace MassTransit.RabbitMqTransport.Configuration
{
    /// <summary>
    /// Configures a queue/exchange pair in RabbitMQ
    /// </summary>
    public interface IQueueConfigurator :
        IExchangeConfigurator
    {
        /// <summary>
        /// Specify the maximum number of concurrent messages that are consumed
        /// </summary>
        /// <value>The limit</value>
        ushort PrefetchCount { set; }

        /// <summary>
        /// Specify that the queue is exclusive to this process and cannot be accessed by other processes
        /// at the same time.
        /// </summary>
        bool Exclusive { set; }

        /// <summary>
        /// Purge the messages from an existing queue on startup (note that upon reconnection to the server
        /// the queue will not be purged again, only when the service is restarted).
        /// </summary>
        bool PurgeOnStartup { set; }

        /// <summary>
        /// Sets the queue to be lazy (using less memory)
        /// </summary>
        bool Lazy { set; }

        /// <summary>
        /// Set a queue argument passed to the broker on queue declaration
        /// </summary>
        /// <param name="key">The argument key</param>
        /// <param name="value">The argument value</param>
        void SetQueueArgument(string key, object value);

        /// <summary>
        /// Enable the message priority for the queue, specifying the maximum priority available
        /// </summary>
        /// <param name="maxPriority"></param>
        void EnablePriority(byte maxPriority);
    }
}