namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;


    public class UncorrelatedEventCorrelation<TSaga, TMessage> :
        EventCorrelation<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        public UncorrelatedEventCorrelation(Event<TMessage> @event)
        {
            Event = @event;
        }

        public SagaFilterFactory<TSaga, TMessage> FilterFactory => null;

        public Event<TMessage> Event { get; }

        Type EventCorrelation.DataType => typeof(TMessage);

        public bool ConfigureConsumeTopology => false;

        public IFilter<ConsumeContext<TMessage>> MessageFilter => null;

        public ISagaPolicy<TSaga, TMessage> Policy => null;

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Failure(Event.Name, "Correlation", "was not specified");
        }
    }
}
