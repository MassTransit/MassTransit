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
        readonly bool _insertOnInitial;
        readonly SagaStateMachine<TInstance> _machine;
        readonly IPipe<ConsumeContext<TData>> _missingPipe;
        readonly Lazy<ISagaPolicy<TInstance, TData>> _policy;
        readonly bool _readOnly;
        readonly ISagaFactory<TInstance, TData> _sagaFactory;

        public MassTransitEventCorrelation(SagaStateMachine<TInstance> machine, Event<TData> @event, SagaFilterFactory<TInstance, TData> sagaFilterFactory,
            IFilter<ConsumeContext<TData>> messageFilter, IPipe<ConsumeContext<TData>> missingPipe, ISagaFactory<TInstance, TData> sagaFactory,
            bool insertOnInitial, bool readOnly, bool configureConsumeTopology)
        {
            Event = @event;
            FilterFactory = sagaFilterFactory;
            MessageFilter = messageFilter;
            _missingPipe = missingPipe;
            _sagaFactory = sagaFactory;
            _insertOnInitial = insertOnInitial;
            _readOnly = readOnly;
            ConfigureConsumeTopology = configureConsumeTopology;
            _machine = machine;

            _policy = new Lazy<ISagaPolicy<TInstance, TData>>(GetSagaPolicy);
        }

        public bool ConfigureConsumeTopology { get; }

        public SagaFilterFactory<TInstance, TData> FilterFactory { get; }

        public Event<TData> Event { get; }

        public Type DataType => typeof(TData);

        public IFilter<ConsumeContext<TData>> MessageFilter { get; }

        public ISagaPolicy<TInstance, TData> Policy => _policy.Value;

        public IEnumerable<ValidationResult> Validate()
        {
            if (_insertOnInitial && _readOnly)
                yield return this.Failure("ReadOnly", "ReadOnly cannot be set when InsertOnInitial is true");

            if (IncludesInitial() && _readOnly)
                yield return this.Failure("ReadOnly", "ReadOnly cannot be used for events in the initial state");
        }

        ISagaPolicy<TInstance, TData> GetSagaPolicy()
        {
            if (IncludesInitial())
                return new NewOrExistingSagaPolicy<TInstance, TData>(_sagaFactory, _insertOnInitial);

            return new AnyExistingSagaPolicy<TInstance, TData>(_missingPipe, _readOnly);
        }

        bool IncludesInitial()
        {
            return _machine.States
                .Where(state => _machine.NextEvents(state).Contains(Event))
                .Any(x => x.Name.Equals(_machine.Initial.Name));
        }
    }
}
