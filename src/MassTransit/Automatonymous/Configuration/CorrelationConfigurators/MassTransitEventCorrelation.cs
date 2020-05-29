namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.Saga.Policies;


    public class MassTransitEventCorrelation<TInstance, TData> :
        EventCorrelation<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly bool _insertOnInitial;
        readonly SagaStateMachine<TInstance> _machine;
        readonly IFilter<ConsumeContext<TData>> _messageFilter;
        readonly IPipe<ConsumeContext<TData>> _missingPipe;
        readonly Lazy<ISagaPolicy<TInstance, TData>> _policy;
        readonly ISagaFactory<TInstance, TData> _sagaFactory;

        public MassTransitEventCorrelation(SagaStateMachine<TInstance> machine, Event<TData> @event, SagaFilterFactory<TInstance, TData> sagaFilterFactory,
            IFilter<ConsumeContext<TData>> messageFilter, IPipe<ConsumeContext<TData>> missingPipe, ISagaFactory<TInstance, TData> sagaFactory,
            bool insertOnInitial)
        {
            _event = @event;
            FilterFactory = sagaFilterFactory;
            _messageFilter = messageFilter;
            _missingPipe = missingPipe;
            _sagaFactory = sagaFactory;
            _insertOnInitial = insertOnInitial;
            _machine = machine;

            _policy = new Lazy<ISagaPolicy<TInstance, TData>>(GetSagaPolicy);
        }

        public SagaFilterFactory<TInstance, TData> FilterFactory { get; }

        Event<TData> EventCorrelation<TInstance, TData>.Event => _event;

        Type EventCorrelation.DataType => typeof(TData);

        IFilter<ConsumeContext<TData>> EventCorrelation<TInstance, TData>.MessageFilter => _messageFilter;

        ISagaPolicy<TInstance, TData> EventCorrelation<TInstance, TData>.Policy => _policy.Value;

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        ISagaPolicy<TInstance, TData> GetSagaPolicy()
        {
            State[] states = _machine.States
                .Where(state => _machine.NextEvents(state).Contains(_event))
                .ToArray();

            var includesInitial = states.Any(x => x.Name.Equals(_machine.Initial.Name));

            if (includesInitial)
                return new NewOrExistingSagaPolicy<TInstance, TData>(_sagaFactory, _insertOnInitial);

            return new AnyExistingSagaPolicy<TInstance, TData>(_missingPipe);
        }
    }
}
