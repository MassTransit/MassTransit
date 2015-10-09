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
namespace MassTransit
{
    using System;


    /// <summary>
    /// Published (or sent, if part of a request/response conversation) when a fault occurs during message
    /// processing
    /// </summary>
    public interface Fault
    {
        /// <summary>
        /// Identifies the fault that was generated
        /// </summary>
        Guid FaultId { get; }

        /// <summary>
        /// The messageId that faulted
        /// </summary>
        Guid? FaultedMessageId { get; }

        /// <summary>
        /// When the fault was produced
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The exception information that occurred
        /// </summary>
        ExceptionInfo[] Exceptions { get; }

        /// <summary>
        /// The host information was the fault occurred
        /// </summary>
        HostInfo Host { get; }
    }


    /// <summary>
    /// A faulted message, published when a message consumer fails to process the message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Fault<out T> :
        Fault
    {
        /// <summary>
        /// The message that faulted
        /// </summary>
        T Message { get; }
    }
}