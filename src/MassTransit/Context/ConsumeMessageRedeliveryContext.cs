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


    public class ConsumeMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly MessageSchedulerContext _scheduler;

        public ConsumeMessageRedeliveryContext(ConsumeContext<TMessage> context, MessageSchedulerContext scheduler)
        {
            _scheduler = scheduler;
            _context = context;
        }

        Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay)
        {
            return _scheduler.ScheduleSend(_context.Message, delay, _context.CreateCopyContextPipe(GetScheduledMessageHeaders));
        }

        static IEnumerable<Tuple<string, object>> GetScheduledMessageHeaders(ConsumeContext context)
        {
            Uri inputAddress = context.ReceiveContext.InputAddress ?? context.DestinationAddress;
            if (inputAddress != null)
                yield return Tuple.Create<string, object>("MT-Scheduling-DeliveredAddress", inputAddress.ToString());
        }
    }
}