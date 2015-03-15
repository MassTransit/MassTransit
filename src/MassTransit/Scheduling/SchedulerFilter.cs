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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pipeline;


    /// <summary>
    /// Adds the scheduler to the consume context, so that it can be used for message redelivery
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class SchedulerFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly SchedulerContext _scheduler;

        public SchedulerFilter(SchedulerContext scheduler)
        {
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            _scheduler = scheduler;
        }

        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            context.GetOrAddPayload<SchedulerContext<TMessage>>(() => new ConsumeSchedulerContext(_scheduler, context));
            context.GetOrAddPayload(() => _scheduler);

            await next.Send(context);
        }

        bool IFilter<ConsumeContext<TMessage>>.Visit(IPipeVisitor visitor)
        {
            return visitor.Visit(this);
        }


        class ConsumeSchedulerContext :
            SchedulerContext<TMessage>
        {
            readonly ConsumeContext<TMessage> _context;
            readonly SchedulerContext _scheduler;

            public ConsumeSchedulerContext(SchedulerContext scheduler, ConsumeContext<TMessage> context)
            {
                _scheduler = scheduler;
                _context = context;
            }

            Task SchedulerContext<TMessage>.ScheduleRedelivery(TimeSpan delay)
            {
                return _scheduler.ScheduleSend(_context.Message, delay, _context.CreateCopyContextPipe<TMessage>(GetSchedulingHeaders));
            }

            Task SchedulerContext.ScheduleSend<T>(T message, TimeSpan deliveryDelay, IPipe<SendContext<T>> sendPipe)
            {
                return _scheduler.ScheduleSend(message, deliveryDelay, sendPipe);
            }

            Task SchedulerContext.ScheduleSend<T>(T message, DateTime deliveryTime, IPipe<SendContext<T>> sendPipe)
            {
                return _scheduler.ScheduleSend(message, deliveryTime, sendPipe);
            }

            static IEnumerable<Tuple<string, object>> GetSchedulingHeaders(ConsumeContext context)
            {
                Uri inputAddress = context.ReceiveContext.InputAddress ?? context.DestinationAddress;
                if (inputAddress != null)
                    yield return Tuple.Create<string, object>("MT-Scheduling-DeliveredAddress", inputAddress.ToString());
            }
        }
    }
}