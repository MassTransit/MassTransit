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
    using System.Threading.Tasks;
    using Pipeline;
    using Scheduling;


    public interface MessageSchedulerContext
    {
        Task<ScheduledMessage<T>> ScheduleSend<T>(T message, TimeSpan deliveryDelay, IPipe<SendContext> sendPipe)
            where T : class;

        Task<ScheduledMessage<T>> ScheduleSend<T>(T message, DateTime deliveryTime, IPipe<SendContext> sendPipe)
            where T : class;

        /// <summary>
        /// Cancel a scheduled message by the scheduled message (uses the TokenId)
        /// </summary>
        /// <param name="message">The schedule message reference</param>
        Task CancelScheduledSend<T>(ScheduledMessage<T> message)
            where T : class;

        /// <summary>
        /// Cancel a scheduled message by TokenId
        /// </summary>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        Task CancelScheduledSend(Guid tokenId);
    }
}