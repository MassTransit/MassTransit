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
namespace MassTransit
{
    using System;


    /// <summary>
    ///     Produced when a message is sent to an endpoint
    /// </summary>
    public interface MessageSendCompleted
    {
        /// <summary>
        ///     The unique identifier of the message that was created and used on the transport
        /// </summary>
        Guid MessageId { get; }

        /// <summary>
        ///     The timestamp when the message began being sent (on the transport, does not include time spent preparing the
        ///     message context
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        ///     The time taken between the time the message was sent and the time it was acknowledged by the transport
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        ///     The destination address of the message
        /// </summary>
        IEndpointAddress Address { get; }
    }
}