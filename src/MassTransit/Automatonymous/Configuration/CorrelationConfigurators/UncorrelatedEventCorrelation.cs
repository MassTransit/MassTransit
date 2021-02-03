namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;


    public class UncorrelatedEventCorrelation<TInstance, TData> :
        EventCorrelation<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        public UncorrelatedEventCorrelation(Event<TData> @event)
        {
            Event = @event;
        }

        public SagaFilterFactory<TInstance, TData> FilterFactory => null;

        public Event<TData> Event { get; }

        Type EventCorrelation.DataType => typeof(TData);

        public bool ConfigureConsumeTopology => false;

        public IFilter<ConsumeContext<TData>> MessageFilter => null;

        public ISagaPolicy<TInstance, TData> Policy => null;

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Failure(Event.Name, "Correlation", "was not specified");
        }
    }
}
