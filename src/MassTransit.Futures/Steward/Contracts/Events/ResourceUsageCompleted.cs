// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Steward.Contracts.Events
{
    using System;


    /// <summary>
    /// When a resource is accessed successfully, this event should be published to enable
    /// discovery of resources and when resources are successfully accessed
    /// </summary>
    public interface ResourceUsageCompleted
    {
        /// <summary>
        /// A unique identifier for the event published
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// The DispatchId (if present) being executed when the resource access failed
        /// </summary>
        Guid DispatchId { get; }

        /// <summary>
        /// The timestamp for when the resource access failed
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The duration of the resource usage
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The resource which was being accessed when the failure occurred
        /// </summary>
        Uri Resource { get; }
    }
}