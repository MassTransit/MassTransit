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
namespace MassTransit.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ScheduleMessageContextPipe<T> :
        IPipe<SendContext<T>>,
        IPipe<SendContext<ScheduleMessage<T>>>
        where T : class
    {
        readonly IPipe<SendContext<T>> _pipe;
        SendContext _context;

        Guid? _scheduledMessageId;

        protected ScheduleMessageContextPipe()
        {
        }

        public ScheduleMessageContextPipe(IPipe<SendContext<T>> pipe)
        {
            _pipe = pipe;
        }

        public Guid? ScheduledMessageId
        {
            get => _context?.ScheduledMessageId ?? _scheduledMessageId;
            set => _scheduledMessageId = value;
        }

        public virtual async Task Send(SendContext<ScheduleMessage<T>> context)
        {
            _context = context;

            _context.ScheduledMessageId = _scheduledMessageId;

            if (_pipe.IsNotEmpty())
            {
                SendContext<T> proxy = context.CreateProxy(context.Message.Payload);

                await _pipe.Send(proxy).ConfigureAwait(false);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public virtual async Task Send(SendContext<T> context)
        {
            _context = context;

            _context.ScheduledMessageId = _scheduledMessageId;

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);
        }
    }


    public class ScheduleMessageContextPipe :
        IPipe<SendContext>
    {
        readonly IPipe<SendContext> _pipe;
        SendContext _context;

        Guid? _scheduledMessageId;

        protected ScheduleMessageContextPipe()
        {
        }

        public ScheduleMessageContextPipe(IPipe<SendContext> pipe)
        {
            _pipe = pipe;
        }

        public Guid? ScheduledMessageId
        {
            get => _context?.ScheduledMessageId ?? _scheduledMessageId;
            set => _scheduledMessageId = value;
        }

        public virtual async Task Send(SendContext context)
        {
            _context = context;

            _context.ScheduledMessageId = _scheduledMessageId;

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }
    }
}
