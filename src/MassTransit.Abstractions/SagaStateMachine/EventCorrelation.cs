namespace MassTransit
{
    using System;


    public interface EventCorrelation :
        ISpecification
    {
        /// <summary>
        /// The data type for the event
        /// </summary>
        Type DataType { get; }

        bool ConfigureConsumeTopology { get; }
    }


    public interface EventCorrelation<TInstance, TData> :
        EventCorrelation
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        Event<TData> Event { get; }

        /// <summary>
        /// Returns the saga policy for the event correlation
        /// </summary>
        /// <value></value>
        ISagaPolicy<TInstance, TData> Policy { get; }

        /// <summary>
        /// The filter factory creates the filter when requested by the connector
        /// </summary>
        SagaFilterFactory<TInstance, TData> FilterFactory { get; }

        /// <summary>
        /// The message filter which extracts the correlationId from the message
        /// </summary>
        IFilter<ConsumeContext<TData>> MessageFilter { get; }
    }
}
