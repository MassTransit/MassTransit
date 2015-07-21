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
namespace Automatonymous.Pipeline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contexts;
    using MassTransit;
    using MassTransit.Logging;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Saga.Pipeline.Filters;
    using MassTransit.Util;


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
        static readonly ILog _log = Logger.Get<StateMachineSagaMessageFilter<TInstance, TData>>();
        readonly Event<TData> _event;
        readonly SagaStateMachine<TInstance> _stateMachine;

        public StateMachineSagaMessageFilter(SagaStateMachine<TInstance> stateMachine, Event<TData> @event)
        {
            _stateMachine = stateMachine;
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
        }

        public async Task Send(SagaConsumeContext<TInstance, TData> context, IPipe<SagaConsumeContext<TInstance, TData>> next)
        {
            var eventContext = new StateMachineEventContext<TInstance, TData>(_stateMachine, context.Saga, _event, context.Message,
                context.CancellationToken);

            eventContext.GetOrAddPayload(() => context);
            eventContext.GetOrAddPayload(() => (ConsumeContext<TData>)context);
            eventContext.GetOrAddPayload(() => (ConsumeContext)context);

            State<TInstance> currentState = await _stateMachine.InstanceStateAccessor.Get(eventContext).ConfigureAwait(false);

            IEnumerable<Event> nextEvents = _stateMachine.NextEvents(currentState);
            if (nextEvents.Contains(_event))
            {
                await _stateMachine.RaiseEvent(eventContext).ConfigureAwait(false);

                if (_stateMachine.IsCompleted(context.Saga))
                    await context.SetCompleted();
            }
            else
            {
                _log.DebugFormat("{0} {1} in {2} does not accept {3}", _stateMachine.GetType().Name, context.Saga.CorrelationId,
                    currentState.Name, _event.Name);
            }
        }
    }
}