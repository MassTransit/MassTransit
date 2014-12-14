// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Saga.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Extensions;
    using MassTransit;
    using MassTransit.Logging;
    using MassTransit.Saga;
    using MassTransit.Saga.Pipeline;


    public class CorrelatedStateMachineMessageSink<TInstance, TMessage> :
        SagaMessageSinkBase<TInstance, TMessage>
        where TMessage : class, CorrelatedBy<Guid>
        where TInstance : class, SagaStateMachineInstance
    {
        static readonly ILog _log = Logger.Get<CorrelatedStateMachineMessageSink<TInstance, TMessage>>();

        public CorrelatedStateMachineMessageSink(StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> repository, ISagaPolicy<TInstance, TMessage> policy, Event<TMessage> @event)
            : base(repository, policy, new CorrelationIdSagaLocator<TMessage>(), (s, c) => GetHandlers(stateMachine, s, c, @event))
        {
        }

        static IEnumerable<Action<IConsumeContext<TMessage>>> GetHandlers(StateMachine<TInstance> stateMachine,
            TInstance instance, IConsumeContext<TMessage> context, Event<TMessage> @event)
        {
            State<TInstance> currentState = stateMachine.InstanceStateAccessor.Get(instance);
            IEnumerable<Event> nextEvents = stateMachine.NextEvents(currentState);
            if (nextEvents.Contains(@event))
            {
                yield return x =>
                    {
                        instance.Bus = context.Bus;


                        context.BaseContext.NotifyConsume(context, typeof(TInstance).ToShortTypeName(),
                            instance.CorrelationId.ToString());

                        using (x.CreateScope())
                            stateMachine.RaiseEvent(instance, @event, x.Message);
                    };
            }
            else
            {
                _log.DebugFormat("{0} {1} in {2} does not accept {3}", stateMachine.GetType().Name, instance.CorrelationId,
                    currentState.Name, @event.Name);
            }
        }
    }
}