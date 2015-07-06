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
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Saga.Policies;


    public class MassTransitEventCorrelation<TInstance, TData> :
        EventCorrelation<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly SagaStateMachine<TInstance> _machine;
        readonly IFilter<ConsumeContext<TData>> _messageFilter;
        readonly IPipe<ConsumeContext<TData>> _missingPipe;
        readonly Lazy<ISagaPolicy<TInstance, TData>> _policy;
        readonly SagaFilterFactory<TInstance, TData> _sagaFilterFactory;

        public MassTransitEventCorrelation(SagaStateMachine<TInstance> machine, Event<TData> @event, SagaFilterFactory<TInstance, TData> sagaFilterFactory,
            IFilter<ConsumeContext<TData>> messageFilter, IPipe<ConsumeContext<TData>> missingPipe)
        {
            _event = @event;
            _sagaFilterFactory = sagaFilterFactory;
            _messageFilter = messageFilter;
            _missingPipe = missingPipe;
            _machine = machine;

            _policy = new Lazy<ISagaPolicy<TInstance, TData>>(GetSagaPolicy);
        }

        public SagaFilterFactory<TInstance, TData> FilterFactory
        {
            get { return _sagaFilterFactory; }
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

        public IFilter<ConsumeContext<TData>> MessageFilter
        {
            get { return _messageFilter; }
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
                return new NewOrExistingSagaPolicy<TInstance, TData>(new DefaultSagaFactory<TInstance, TData>());

            if (includesInitial)
                return new NewSagaPolicy<TInstance, TData>(new DefaultSagaFactory<TInstance, TData>());

            return new AnyExistingSagaPolicy<TInstance, TData>(_missingPipe);
        }
    }
}