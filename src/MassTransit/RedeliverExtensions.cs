// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;


    public static class RedeliverExtensions
    {
        /// <summary>
        /// Redeliver uses the message scheduler to deliver the message to the queue at a future
        /// time. The delivery count is incremented. Moreover, if you give custom callback action, it perform before sending message to queueu.
        /// A message scheduler must be configured on the bus for redelivery to be enabled.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The consume context of the message</param>
        /// <param name="delay">The delay before the message is delivered. It may take longer to receive the message if the queue is not empty.</param>
        /// <param name="callback">Operation which is executed before the message is delivered.</param>
        /// <returns></returns>
        public static Task Redeliver<T>(this ConsumeContext<T> context, TimeSpan delay, Action<ConsumeContext, SendContext> callback = null)
            where T : class
        {
            if (!context.TryGetPayload(out MessageSchedulerContext schedulerContext))
                throw new MessageException(typeof(T), "No scheduler context was available to redeliver the message");

            MessageRedeliveryContext redeliverContext = new ScheduleMessageRedeliveryContext<T>(context, schedulerContext);

            return redeliverContext.ScheduleRedelivery(delay, callback);
        }
    }
}