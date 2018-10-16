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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Context for delaying message redelivery using a delayed RabbitMQ exchange. Requires the plug-in
    /// https://github.com/rabbitmq/rabbitmq-delayed-message-exchange
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class DelayedExchangeMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly IMessageScheduler _scheduler;

        public DelayedExchangeMessageRedeliveryContext(ConsumeContext<TMessage> context, IMessageScheduler scheduler)
        {
            _context = context;
            _scheduler = scheduler;
        }

        Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
        {
            Action<ConsumeContext, SendContext> combinedCallback = UpdateDeliveryContext + callback;

            return _scheduler.ScheduleSend(_context.ReceiveContext.InputAddress, delay, _context.Message,
                _context.CreateCopyContextPipe(combinedCallback));
        }

        static void UpdateDeliveryContext(ConsumeContext context, SendContext sendContext)
        {
            sendContext.Headers.Set(MessageHeaders.RedeliveryCount, context.GetRedeliveryCount() + 1);
        }
    }
}