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
namespace MassTransit.AzureServiceBusTransport
{
    using System;


    /// <summary>
    /// Configure an Azure Service Bus receive endpoint
    /// </summary>
    public interface IServiceBusReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Specify the queue should be durable (survives broker restart) or in-memory
        /// </summary>
        /// <value>True for a durable queue, False for an in-memory queue</value>
        bool EnableExpress { set; }

        /// <summary>
        /// Specify the lock duration for messages read from the queue
        /// </summary>
        /// <value></value>
        TimeSpan LockDuration { set; }

        /// <summary>
        /// Move messages to the dead letter queue on expiration (time to live exceeded)
        /// </summary>
        bool EnableDeadLetteringOnMessageExpiration { set; }

        /// <summary>
        /// Set the default message time to live in the queue
        /// </summary>
        TimeSpan DefaultMessageTimeToLive { set; }

        /// <summary>
        /// Specify the maximum number of concurrent messages that are consumed
        /// </summary>
        /// <value>The limit</value>
        int PrefetchCount { set; }

        /// <summary>
        /// Purge the messages from an existing queue on startup (note that upon reconnection to the server
        /// the queue will not be purged again, only when the service is restarted).
        /// </summary>
        /// <value></value>
        bool PurgeOnStartup { set; }

        /// <summary>
        /// The path of the queue that's being configured
        /// </summary>
        Uri QueuePath { get; }

        /// <summary>
        /// Enable duplicate detection on the queue, specifying the time window
        /// </summary>
        /// <param name="historyTimeWindow">The time window for duplicate history</param>
        void EnableDuplicateDetection(TimeSpan historyTimeWindow);
    }
}