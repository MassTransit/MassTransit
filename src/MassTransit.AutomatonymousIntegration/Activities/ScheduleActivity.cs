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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.Pipeline;
    using MassTransit.Scheduling;


    public class ScheduleActivity<TInstance, TMessage> :
        Activity<TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly EventMessageFactory<TInstance, TMessage> _messageFactory;
        readonly Schedule<TInstance> _schedule;
        readonly IPipe<SendContext> _sendPipe;

        public ScheduleActivity(EventMessageFactory<TInstance, TMessage> messageFactory, Schedule<TInstance> schedule)
        {
            _messageFactory = messageFactory;
            _schedule = schedule;

            _sendPipe = Pipe.Empty<SendContext>();
        }

        public ScheduleActivity(EventMessageFactory<TInstance, TMessage> messageFactory, Schedule<TInstance> schedule,
            Action<SendContext> contextCallback)
        {
            _messageFactory = messageFactory;
            _schedule = schedule;

            _sendPipe = Pipe.Execute(contextCallback);
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        async Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context);

            await next.Execute(context);
        }

        async Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await Execute(context);

            await next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            await next.Faulted(context);
        }

        async Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            await next.Faulted(context);
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            ConsumeContext consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ContextException("The consume context could not be retrieved.");

            var consumeEventContext = new AutomatonymousConsumeEventContext<TInstance>(context, consumeContext);

            TMessage message = _messageFactory(consumeEventContext);

            MessageSchedulerContext schedulerContext;
            if (!consumeContext.TryGetPayload(out schedulerContext))
                throw new ContextException("The scheduler context could not be retrieved.");

            var scheduledMessage = await schedulerContext.ScheduleSend(message, _schedule.Delay, _sendPipe);

            var previousTokenId = _schedule.GetTokenId(context.Instance);
            if (previousTokenId.HasValue)
            {
                await schedulerContext.CancelScheduledSend(previousTokenId.Value);
            }

            _schedule?.SetTokenId(context.Instance, scheduledMessage.TokenId);
        }
    }


    public class ScheduleActivity<TInstance, TData, TMessage> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TMessage : class
    {
        readonly EventMessageFactory<TInstance, TData, TMessage> _messageFactory;
        readonly Schedule<TInstance, TMessage> _schedule;
        readonly IPipe<SendContext> _sendPipe;

        public ScheduleActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory, Schedule<TInstance, TMessage> schedule,
            Action<SendContext> contextCallback)
        {
            _messageFactory = messageFactory;
            _schedule = schedule;

            _sendPipe = Pipe.Execute(contextCallback);
        }

        public ScheduleActivity(EventMessageFactory<TInstance, TData, TMessage> messageFactory, Schedule<TInstance, TMessage> schedule)
        {
            _messageFactory = messageFactory;
            _schedule = schedule;

            _sendPipe = Pipe.Empty<SendContext>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeContext<TData> consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ContextException("The consume context could not be retrieved.");

            var consumeEventContext = new AutomatonymousConsumeEventContext<TInstance, TData>(context, consumeContext);

            TMessage message = _messageFactory(consumeEventContext);

            MessageSchedulerContext schedulerContext;
            if (!consumeContext.TryGetPayload(out schedulerContext))
                throw new ContextException("The scheduler context could not be retrieved.");

            var scheduledMessage = await schedulerContext.ScheduleSend(message, _schedule.Delay, _sendPipe);

            var previousTokenId = _schedule.GetTokenId(context.Instance);
            if (previousTokenId.HasValue)
            {
                await schedulerContext.CancelScheduledSend(previousTokenId.Value);
            }

            _schedule?.SetTokenId(context.Instance, scheduledMessage.TokenId);

            await next.Execute(context);
        }

        async Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context,
            Behavior<TInstance, TData> next)
        {
            await next.Faulted(context);
        }
    }
}