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
namespace MassTransit.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;
    using Scheduling;


    public class ConsumeMessageSchedulerContext :
        MessageSchedulerContext
    {
        readonly ConsumeContext _consumeContext;
        readonly IMessageScheduler _scheduler;

        public ConsumeMessageSchedulerContext(ConsumeContext consumeContext, IMessageScheduler scheduler)
        {
            if (consumeContext == null)
                throw new ArgumentNullException(nameof(consumeContext));
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));

            _consumeContext = consumeContext;
            _scheduler = scheduler;
        }

        Task IMessageScheduler.CancelScheduledSend(Guid tokenId)
        {
            return _scheduler.CancelScheduledSend(tokenId);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(_consumeContext.ReceiveContext.InputAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(_consumeContext.ReceiveContext.InputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(_consumeContext.ReceiveContext.InputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(_consumeContext.ReceiveContext.InputAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(_consumeContext.ReceiveContext.InputAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(_consumeContext.ReceiveContext.InputAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(_consumeContext.ReceiveContext.InputAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend<T>(_consumeContext.ReceiveContext.InputAddress, scheduledTime, values, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend(_consumeContext.ReceiveContext.InputAddress, scheduledTime, values, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _scheduler.ScheduleSend<T>(_consumeContext.ReceiveContext.InputAddress, scheduledTime, values, pipe, cancellationToken);
        }
    }
}