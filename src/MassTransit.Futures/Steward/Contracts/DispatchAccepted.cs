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
namespace MassTransit.Steward.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Published when a command or request is accepted and will be sent to the service
    /// </summary>
    public interface DispatchAccepted
    {
        /// <summary>
        /// uniquely identify the event
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// The timestamp at which at the command was forwarded for execution
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The command that was forwarded
        /// </summary>
        Guid DispatchId { get; }

        /// <summary>
        /// The timestamp at which the command execution was requested
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// A unique URI identifying the resource(s) being accessed by the command
        /// </summary>
        Uri[] Resources { get; }

        /// <summary>
        /// The message types implemented by the command message
        /// </summary>
        string[] DispatchTypes { get; }

        /// <summary>
        /// The destination where the command was sent for execution
        /// </summary>
        Uri Destination { get; }
    }
}