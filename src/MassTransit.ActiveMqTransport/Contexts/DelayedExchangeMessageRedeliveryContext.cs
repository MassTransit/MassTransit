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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Context for delaying message redelivery using a delayed ActiveMQ messages.
    /// http://activemq.apache.org/delay-and-schedule-message-delivery
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class DelayedExchangeMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> context;
        readonly IMessageScheduler scheduler;

        public DelayedExchangeMessageRedeliveryContext(ConsumeContext<TMessage> context, IMessageScheduler scheduler)
        {
            this.context = context;
            this.scheduler = scheduler;
        }

        Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
        {
            var combinedCallback = UpdateDeliveryContext + callback;

            return this.scheduler.ScheduleSend(this.context.ReceiveContext.InputAddress, delay, this.context.Message, this.context.CreateCopyContextPipe(combinedCallback));
        }

        static void UpdateDeliveryContext(ConsumeContext context, SendContext sendContext)
        {
            sendContext.Headers.Set(MessageHeaders.RedeliveryCount, context.GetRedeliveryCount() + 1);
        }
    }
}
