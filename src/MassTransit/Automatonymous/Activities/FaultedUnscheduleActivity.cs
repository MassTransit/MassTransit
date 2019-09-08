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
    using GreenPipes;
    using MassTransit;


    public class FaultedUnscheduleActivity<TInstance> :
        Activity<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly Schedule<TInstance> _schedule;

        public FaultedUnscheduleActivity(Schedule<TInstance> schedule)
        {
            _schedule = schedule;
        }

        public void Accept(StateMachineVisitor inspector)
        {
            inspector.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("unschedule-faulted");
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            return next.Execute(context);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            return next.Execute(context);
        }

        async Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            await Faulted(context).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            await Faulted(context).ConfigureAwait(false);

            await next.Faulted(context).ConfigureAwait(false);
        }

        async Task Faulted(BehaviorContext<TInstance> context)
        {
            ConsumeContext consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ContextException("The consume context could not be retrieved.");

            MessageSchedulerContext schedulerContext;
            if (!consumeContext.TryGetPayload(out schedulerContext))
                throw new ContextException("The scheduler context could not be retrieved.");

            Guid? previousTokenId = _schedule.GetTokenId(context.Instance);
            if (previousTokenId.HasValue)
            {
                await schedulerContext.CancelScheduledSend(consumeContext.ReceiveContext.InputAddress, previousTokenId.Value).ConfigureAwait(false);

                _schedule.SetTokenId(context.Instance, default(Guid?));
            }
        }
    }
}