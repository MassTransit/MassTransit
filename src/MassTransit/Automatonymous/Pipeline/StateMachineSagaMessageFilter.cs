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
namespace Automatonymous.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.Logging;
    using MassTransit.Metadata;
    using MassTransit.Saga;
    using MassTransit.Saga.Pipeline.Filters;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TInstance">The consumer type</typeparam>
    /// <typeparam name="TData">The message type</typeparam>
    public class StateMachineSagaMessageFilter<TInstance, TData> :
        ISagaMessageFilter<TInstance, TData>
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly SagaStateMachine<TInstance> _machine;

        public StateMachineSagaMessageFilter(SagaStateMachine<TInstance> machine, Event<TData> @event)
        {
            _machine = machine;
            _event = @event;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("automatonymous");
            scope.Set(new
            {
                Event = _event.Name,
                DataType = TypeMetadataCache<TData>.ShortName,
                InstanceType = TypeMetadataCache<TInstance>.ShortName
            });

            List<State<TInstance>> states = _machine.States.Cast<State<TInstance>>()
                .Where(x => x.Events.Contains(_event)).ToList();
            if (states.Any())
            {
                scope.Add("states", states.Select(x => x.Name).ToArray());
            }

            _machine.Probe(context);
        }

        public async Task Send(SagaConsumeContext<TInstance, TData> context, IPipe<SagaConsumeContext<TInstance, TData>> next)
        {
            var eventContext = new StateMachineEventContextProxy<TInstance, TData>(context, _machine, context.Saga, _event, context.Message);

            State<TInstance> currentState = await _machine.Accessor.Get(eventContext).ConfigureAwait(false);

            var activity = LogContext.IfEnabled(OperationName.Saga.RaiseEvent)?.StartActivity(new {BeginState = currentState.Name});
            try
            {
                await _machine.RaiseEvent(eventContext).ConfigureAwait(false);

                if (_machine.IsCompleted(context.Saga))
                    await context.SetCompleted().ConfigureAwait(false);
            }
            catch (UnhandledEventException ex)
            {
                throw new NotAcceptedStateMachineException(typeof(TInstance), typeof(TData), context.CorrelationId ?? Guid.Empty, currentState.Name, ex);
            }
            finally
            {
                activity?.Stop(new {EndState = (await _machine.Accessor.Get(eventContext).ConfigureAwait(false)).Name});
            }
        }
    }
}
