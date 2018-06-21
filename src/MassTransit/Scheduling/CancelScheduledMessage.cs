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
namespace MassTransit.Scheduling
{
    using System;


    public interface CancelScheduledMessage
    {
        /// <summary>
        /// The cancel scheduled message correlationId
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// The date/time this message was created
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The token of the scheduled message
        /// </summary>
        Guid TokenId { get; }
    }
}