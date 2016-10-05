// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.InMemory
{
    /// <summary>
    /// Allows access to real-time metrics on the in-memory transport
    /// </summary>
    public interface IInMemoryTransport
    {
        /// <summary>
        /// The number of messages currently being delivered to consumers.
        /// </summary>
        int PendingDeliveryCount { get; }

        /// <summary>
        /// The total number of messages which have been or are currently being delivered to consumers.
        /// </summary>
        long DeliveryCount { get; }

        /// <summary>
        /// The highest number of message being concurrently delivered to consumers.
        /// </summary>
        int MaxPendingDeliveryCount { get; }

        /// <summary>
        /// The depth of the queue, including messages currently being delivered, on the transport
        /// </summary>
        int QueueDepth { get; }
    }
}