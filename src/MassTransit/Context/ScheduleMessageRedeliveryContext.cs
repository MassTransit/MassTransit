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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// Used to schedule message redelivery using the message scheduler
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class ScheduleMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly MessageSchedulerContext _scheduler;

        public ScheduleMessageRedeliveryContext(ConsumeContext<TMessage> context, MessageSchedulerContext scheduler)
        {
            _scheduler = scheduler;
            _context = context;
        }

        Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
        {
            Action<ConsumeContext, SendContext> combinedAction = AddMessageHeaderAction + callback;
            return _scheduler.ScheduleSend(delay, _context.Message, _context.CreateCopyContextPipe(combinedAction));
        }

        void AddMessageHeaderAction(ConsumeContext consumeContext, SendContext sendContext)
        {
            foreach (KeyValuePair<string, object> additionalHeader in GetScheduledMessageHeaders(consumeContext))
                sendContext.Headers.Set(additionalHeader.Key, additionalHeader.Value);
        }

        static IEnumerable<KeyValuePair<string, object>> GetScheduledMessageHeaders(ConsumeContext context)
        {
            Uri inputAddress = context.ReceiveContext.InputAddress ?? context.DestinationAddress;
            if (inputAddress != null)
                yield return new KeyValuePair<string, object>(MessageHeaders.DeliveredAddress, inputAddress.ToString());

            int? previousDeliveryCount = context.Headers.Get(MessageHeaders.RedeliveryCount, default(int?));
            if (!previousDeliveryCount.HasValue)
                previousDeliveryCount = 0;

            yield return new KeyValuePair<string, object>(MessageHeaders.RedeliveryCount, previousDeliveryCount.Value + 1);
        }
    }
}