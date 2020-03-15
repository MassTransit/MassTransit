namespace MassTransit.Definition
{
    using System;
    using Saga;


    public interface ISagaDefinition :
        IDefinition
    {
        /// <summary>
        /// The saga type
        /// </summary>
        Type SagaType { get; }

        /// <summary>
        /// Return the endpoint name for the consumer, using the specified formatter if necessary.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetEndpointName(IEndpointNameFormatter formatter);
    }


    public interface ISagaDefinition<TSaga> :
        ISagaDefinition
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Sets the endpoint definition, if available
        /// </summary>
        IEndpointDefinition<TSaga> EndpointDefinition { set; }

        /// <summary>
        /// Configure the consumer on the receive endpoint
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="sagaConfigurator">The consumer configurator</param>
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator);

        /// <summary>
        /// Called by the <see cref="SagaMessageDefinition{TSaga, T}"/> to configure any saga-level definitions, such as message partitioning.
        /// </summary>
        /// <param name="endpointConfigurator"></param>
        /// <param name="sagaMessageConfigurator"></param>
        /// <typeparam name="T"></typeparam>
        void Configure<T>(IReceiveEndpointConfigurator endpointConfigurator, ISagaMessageConfigurator<TSaga, T> sagaMessageConfigurator)
            where T : class;
    }
}
