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
namespace MassTransit.Steward.Contracts
{
    using System;


    /// <summary>
    /// When access to a resource fails, this event should be published. This event
    /// may be correlated to a message.
    /// </summary>
    public interface ResourceUsageFailed
    {
        /// <summary>
        /// A unique identifier for the event published
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// The CommandId (if present) being executed when the resource access failed
        /// </summary>
        Guid DispatchId { get; }

        /// <summary>
        /// The timestamp for when the resource access failed
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The duration the resource access was attempted
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The resource which was being accessed when the failure occurred
        /// </summary>
        Uri Resource { get; }

        /// <summary>
        /// A code associated with the reason why the resource usage faulted
        /// </summary>
        int ReasonCode { get; }

        /// <summary>
        /// A description of the reason
        /// </summary>
        string ReasonText { get; }
    }
}