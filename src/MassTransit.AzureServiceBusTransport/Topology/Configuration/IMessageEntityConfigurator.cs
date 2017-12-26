// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Topology.Configuration
{
    using System;


    public interface IMessageEntityConfigurator :
        IEntityConfigurator
    {
        /// <summary>
        /// The entity path
        /// </summary>
        string Path { get; }

        /// <summary>
        /// How long of a window to use to detect duplicate messages
        /// </summary>
        TimeSpan? DuplicateDetectionHistoryTimeWindow { set; }

        /// <summary>
        /// If express is enabled, messages are not persisted to durable storage
        /// </summary>
        /// <value>True for a durable queue, False for an in-memory queue</value>
        bool? EnableExpress { set; }

        /// <summary>
        /// Sets a value that indicates whether the queue to be partitioned across multiple message brokers is enabled
        /// </summary>
        bool? EnablePartitioning { set; }

        /// <summary>
        /// Sets a value that indicates whether the message is anonymous accessible.
        /// </summary>
        bool? IsAnonymousAccessible { set; }

        /// <summary>
        /// Sets the maximum size of the queue in megabytes, which is the size of memory allocated for the queue
        /// </summary>
        long? MaxSizeInMegabytes { set; }

        /// <summary>
        /// Sets the value indicating if this queue requires duplicate detection.
        /// </summary>
        bool? RequiresDuplicateDetection { set; }

        /// <summary>
        /// Sets a value that indicates whether the queue supports ordering.
        /// </summary>
        bool? SupportOrdering { set; }

        /// <summary>
        /// Enable duplicate detection on the queue, specifying the time window
        /// </summary>
        /// <param name="historyTimeWindow">The time window for duplicate history</param>
        void EnableDuplicateDetection(TimeSpan historyTimeWindow);
    }
}