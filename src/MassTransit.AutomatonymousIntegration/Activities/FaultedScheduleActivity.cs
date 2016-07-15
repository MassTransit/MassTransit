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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.Scheduling;


    public class FaultedScheduleActivity<TInstance, TData, TException, TMessage> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TException : Exception
        where TMessage : class
    {
        readonly ScheduleDelayProvider<TInstance, TData, TException> _delayProvider;
        readonly EventExceptionMessageFactory<TInstance, TData, TException, TMessage> _messageFactory;
        readonly Schedule<TInstance, TMessage> _schedule;
        readonly IPipe<SendContext> _sendPipe;

        public FaultedScheduleActivity(EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Schedule<TInstance, TMessage> schedule,
            Action<SendContext> contextCallback, ScheduleDelayProvider<TInstance, TData, TException> delayProvider)
        {
            _messageFactory = messageFactory;
            _schedule = schedule;
            _delayProvider = delayProvider;

            _sendPipe = Pipe.Execute(contextCallback);
        }

        public FaultedScheduleActivity(EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Schedule<TInstance, TMessage> schedule, ScheduleDelayProvider<TInstance, TData, TException> delayProvider)
        {
            _messageFactory = messageFactory;
            _schedule = schedule;
            _delayProvider = delayProvider;

            _sendPipe = Pipe.Empty<SendContext>();
        }

        void Visitable.Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance, TData>.Faulted<T>(BehaviorExceptionContext<TInstance, TData, T> context,
            Behavior<TInstance, TData> next)
        {
            ConsumeExceptionEventContext<TInstance, TData, TException> exceptionContext;
            if (context.TryGetExceptionContext(out exceptionContext))
            {
                MessageSchedulerContext schedulerContext;
                if (!((ConsumeContext)exceptionContext).TryGetPayload(out schedulerContext))
                    throw new ContextException("The scheduler context could not be retrieved.");

                var message = _messageFactory(exceptionContext);

                var delay = _delayProvider(exceptionContext);

                ScheduledMessage<TMessage> scheduledMessage = await schedulerContext.ScheduleSend(delay, message, _sendPipe).ConfigureAwait(false);

                Guid? previousTokenId = _schedule.GetTokenId(context.Instance);
                if (previousTokenId.HasValue)
                {
                    await schedulerContext.CancelScheduledSend(previousTokenId.Value).ConfigureAwait(false);
                }

                _schedule?.SetTokenId(context.Instance, scheduledMessage.TokenId);
            }

            await next.Faulted(context).ConfigureAwait(false);
        }
    }
}