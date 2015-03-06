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
namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Linq;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.Saga.Policies;


    public class MassTransitEventCorrelation<TInstance, TData> :
        EventCorrelation<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Func<ConsumeContext<TData>, Guid> _correlationIdSelector;
        readonly Event<TData> _event;
        readonly Func<ISagaRepository<TInstance>, ISagaPolicy<TInstance, TData>, ISagaLocator<TData>> _locatorFactory;
        readonly SagaStateMachine<TInstance> _machine;
        Lazy<ISagaPolicy<TInstance, TData>> _policy;

        public MassTransitEventCorrelation(Event<TData> @event, Func<ConsumeContext<TData>, Guid> correlationIdSelector, SagaStateMachine<TInstance> machine,
            Func<ISagaRepository<TInstance>, ISagaPolicy<TInstance, TData>, ISagaLocator<TData>> locatorFactory)
        {
            _event = @event;
            _correlationIdSelector = correlationIdSelector;
            _machine = machine;
            _locatorFactory = locatorFactory;
            _policy = new Lazy<ISagaPolicy<TInstance, TData>>(GetSagaPolicy);
        }

        public Event Event
        {
            get { return _event; }
        }

        Event<TData> EventCorrelation<TInstance, TData>.Event
        {
            get { return _event; }
        }

        public Type DataType
        {
            get { return typeof(TData); }
        }

        public ISagaLocator<TData> GetSagaLocator(ISagaRepository<TInstance> sagaRepository)
        {
            return _locatorFactory(sagaRepository, _policy.Value);
        }

        Guid EventCorrelation<TInstance, TData>.GetCorrelationId(ConsumeContext<TData> context)
        {
            Guid correlationId = _correlationIdSelector(context);
            if (correlationId == Guid.Empty)
                throw new SagaException("The message CorrelationId is empty", typeof(TInstance), typeof(TData), correlationId);

            return correlationId;
        }

        ISagaPolicy<TInstance, TData> EventCorrelation<TInstance, TData>.Policy
        {
            get { return _policy.Value; }
        }

        ISagaPolicy<TInstance, TData> GetSagaPolicy()
        {
            State[] states = _machine.States
                .Where(state => _machine.NextEvents(state).Contains(_event))
                .ToArray();

            bool includesInitial = states.Any(x => x.Name.Equals(_machine.Initial.Name));
            bool includesOther = states.Any(x => !x.Name.Equals(_machine.Initial.Name));


            if (includesInitial && includesOther)
                return new CreateOrUseExistingSagaPolicy<TInstance, TData>(_correlationIdSelector, _machine.IsCompleted);

            if (includesInitial)
                return new InitiatingSagaPolicy<TInstance, TData>(_correlationIdSelector, _machine.IsCompleted);

            return new ExistingOrIgnoreSagaPolicy<TInstance, TData>(_machine.IsCompleted);
        }
    }
}