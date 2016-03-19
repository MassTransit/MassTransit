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
namespace MassTransit.AzureServiceBusTransport.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    /// <summary>
    /// Sets the message endqueue time when sending the message, and invokes
    /// any developer-specified pipes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceBusScheduleMessagePipe<T> :
        IPipe<SendContext<T>>,
        IPipe<PublishContext<T>>
        where T : class
    {
        readonly IPipe<SendContext<T>> _pipe;
        readonly DateTime _scheduledTime;
        readonly IPipe<SendContext> _sendPipe;

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime)
        {
            _scheduledTime = scheduledTime;

            MessageId = NewId.NextGuid();
        }

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime, IPipe<SendContext<T>> pipe)
            : this(scheduledTime)
        {
            _pipe = pipe;
        }

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime, IPipe<SendContext> pipe)
            : this(scheduledTime)
        {
            _sendPipe = pipe;
        }

        public Guid MessageId { get; }

        public async Task Send(PublishContext<T> context)
        {
            context.MessageId = MessageId;
            context.SetScheduledEnqueueTime(_scheduledTime);

            if (_pipe != null)
                await _pipe.Send(context).ConfigureAwait(false);

            if (_sendPipe != null)
                await _sendPipe.Send(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
            _sendPipe?.Probe(context);
        }

        public async Task Send(SendContext<T> context)
        {
            context.MessageId = MessageId;
            context.SetScheduledEnqueueTime(_scheduledTime);

            if (_pipe != null)
                await _pipe.Send(context).ConfigureAwait(false);

            if (_sendPipe != null)
                await _sendPipe.Send(context).ConfigureAwait(false);
        }
    }


    /// <summary>
    /// Sets the message endqueue time when sending the message, and invokes
    /// any developer-specified pipes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceBusScheduleMessagePipe :
        IPipe<SendContext>,
        IPipe<PublishContext>
    {
        readonly DateTime _scheduledTime;
        readonly IPipe<SendContext> _sendPipe;

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime)
        {
            _scheduledTime = scheduledTime;

            MessageId = NewId.NextGuid();
        }

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime, IPipe<SendContext> pipe)
            : this(scheduledTime)
        {
            _sendPipe = pipe;
        }

        public Guid MessageId { get; }

        public async Task Send(PublishContext context)
        {
            context.MessageId = MessageId;
            context.SetScheduledEnqueueTime(_scheduledTime);

            if (_sendPipe != null)
                await _sendPipe.Send(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _sendPipe?.Probe(context);
        }

        public async Task Send(SendContext context)
        {
            context.MessageId = MessageId;
            context.SetScheduledEnqueueTime(_scheduledTime);

            if (_sendPipe != null)
                await _sendPipe.Send(context).ConfigureAwait(false);
        }
    }
}