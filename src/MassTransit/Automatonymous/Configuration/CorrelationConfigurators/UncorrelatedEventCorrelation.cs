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

        public SagaFilterFactory<TInstance, TData> FilterFactory { get; } = null;

        public Event<TData> Event { get; }

        Type EventCorrelation.DataType => typeof(TData);

        IFilter<ConsumeContext<TData>> EventCorrelation<TInstance, TData>.MessageFilter { get; } = null;

        ISagaPolicy<TInstance, TData> EventCorrelation<TInstance, TData>.Policy { get; } = null;

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Failure(Event.Name, "Correlation", "was not specified");
        }
    }
}
