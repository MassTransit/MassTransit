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
namespace MassTransit.Steward.Core
{
    using System;


    /// <summary>
    /// The context around a dispatch being processed
    /// </summary>
    public interface DispatchContext :
        ConsumeContext
    {
        /// <summary>
        /// Identifies the dispatch
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
        /// The destination where the command message should be sent for execution
        /// </summary>
        Uri Destination { get; }

        /// <summary>
        /// The message body to be serialized
        /// </summary>
        string Body { get; }

        /// <summary>
        /// Accept the dispatch for processing
        /// </summary>
        DispatchResult Accept();

        /// <summary>
        /// Delay the dispatch
        /// </summary>
        /// <param name="timeSpan">The duration of the delay</param>
        /// <param name="reason">The reason for the delay</param>
        DispatchResult Delay(TimeSpan timeSpan, string reason);

        /// <summary>
        /// Discard the dispatch without any further communication other than Discarded
        /// </summary>
        /// <param name="reason"></param>
        DispatchResult Discard(string reason);

        /// <summary>
        /// Refuse the dispatch due to a security violation
        /// </summary>
        /// <param name="reason"></param>
        DispatchResult Refuse(string reason);

        /// <summary>
        /// Decline the dispatch due to a resource or policy violation
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        DispatchResult Reject(string reason);

        /// <summary>
        /// Request a typed version of the dispatch including the message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The output parameter for the resulting context</param>
        /// <returns>True if the message type is available, otherwise false</returns>
        bool TryGetContext<T>(out MessageDispatchContext<T> context)
            where T : class;
    }
}