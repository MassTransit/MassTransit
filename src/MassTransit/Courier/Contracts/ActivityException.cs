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
namespace MassTransit.Courier.Contracts
{
    using System;


    /// <summary>
    /// Capture the exception information thrown by an activity
    /// </summary>
    public interface ActivityException
    {
        /// <summary>
        /// The tracking number of the activity that threw the exception
        /// </summary>
        Guid ActivityTrackingNumber { get; }

        /// <summary>
        /// The point in time when the exception occurred
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The time from when the routing slip was created until the exception occurred
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The name of the activity that caused the exception
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The host where the activity was executed
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// The exception details
        /// </summary>
        ExceptionInfo ExceptionInfo { get; }
    }
}