// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Scheduling;


    public static class CancelScheduledSendExtensions
    {
        /// <summary>
        /// Cancel a scheduled message 
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="scheduler">The message scheduler</param>
        /// <param name="message">The </param>
        /// <returns></returns>
        public static Task CancelScheduledSend<T>(this IMessageScheduler scheduler, ScheduledMessage<T> message)
            where T : class
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return scheduler.CancelScheduledSend(message.Destination, message.TokenId);
        }

        /// <summary>
        /// Cancel a scheduled message 
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message scheduler</param>
        /// <param name="message">The </param>
        /// <returns></returns>
        public static Task CancelScheduledSend<T>(this ConsumeContext context, ScheduledMessage<T> message)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var scheduler = context.GetPayload<MessageSchedulerContext>();

            return scheduler.CancelScheduledSend(message.Destination, message.TokenId);
        }
    }
}