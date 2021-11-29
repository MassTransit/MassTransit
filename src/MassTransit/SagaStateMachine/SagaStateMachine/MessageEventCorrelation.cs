namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Saga;


    public class MessageEventCorrelation<TSaga, TMessage> :
        EventCorrelation<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly bool _insertOnInitial;
        readonly SagaStateMachine<TSaga> _machine;
        readonly IPipe<ConsumeContext<TMessage>> _missingPipe;
        readonly Lazy<ISagaPolicy<TSaga, TMessage>> _policy;
        readonly bool _readOnly;
        readonly ISagaFactory<TSaga, TMessage> _sagaFactory;

        public MessageEventCorrelation(SagaStateMachine<TSaga> machine, Event<TMessage> @event, SagaFilterFactory<TSaga, TMessage> sagaFilterFactory,
            IFilter<ConsumeContext<TMessage>> messageFilter, IPipe<ConsumeContext<TMessage>> missingPipe, ISagaFactory<TSaga, TMessage> sagaFactory,
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

            _policy = new Lazy<ISagaPolicy<TSaga, TMessage>>(GetSagaPolicy);
        }

        public bool ConfigureConsumeTopology { get; }

        public SagaFilterFactory<TSaga, TMessage> FilterFactory { get; }

        public Event<TMessage> Event { get; }

        public Type DataType => typeof(TMessage);

        public IFilter<ConsumeContext<TMessage>> MessageFilter { get; }

        public ISagaPolicy<TSaga, TMessage> Policy => _policy.Value;

        public IEnumerable<ValidationResult> Validate()
        {
            if (_insertOnInitial && _readOnly)
                yield return this.Failure("ReadOnly", "ReadOnly cannot be set when InsertOnInitial is true");

            if (IncludesInitial() && _readOnly)
                yield return this.Failure("ReadOnly", "ReadOnly cannot be used for events in the initial state");
        }

        ISagaPolicy<TSaga, TMessage> GetSagaPolicy()
        {
            if (IncludesInitial())
                return new NewOrExistingSagaPolicy<TSaga, TMessage>(_sagaFactory, _insertOnInitial);

            return new AnyExistingSagaPolicy<TSaga, TMessage>(_missingPipe, _readOnly);
        }

        bool IncludesInitial()
        {
            return _machine.States
                .Where(state => _machine.NextEvents(state).Contains(Event))
                .Any(x => x.Name.Equals(_machine.Initial.Name));
        }
    }
}
