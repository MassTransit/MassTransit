// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier.Contracts
{
    using System;


    /// <summary>
    /// Message contract for storing activity log data
    /// </summary>
    public interface ActivityLog
    {
        /// <summary>
        /// The tracking number for completion of the activity
        /// </summary>
        Guid ExecutionId { get; }

        /// <summary>
        /// The name of the activity that was completed
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The timestamp when the activity started
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The duration of the activity execution
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The host that executed the activity
        /// </summary>
        HostInfo Host { get; }
    }
}