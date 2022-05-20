namespace MassTransit
{
    using System;


    public interface IFutureDefinition<TFuture> :
        IFutureDefinition
        where TFuture : class, SagaStateMachine<FutureState>
    {
        /// <summary>Sets the endpoint definition, if available</summary>
        new IEndpointDefinition<TFuture> EndpointDefinition { set; }

        /// <summary>Configure the future on the receive endpoint</summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="sagaConfigurator">The consumer configurator</param>
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator);
    }


    public interface IFutureDefinition :
        IDefinition
    {
        Type FutureType { get; }

        IEndpointDefinition? EndpointDefinition { get; }

        /// <summary>
        /// Return the endpoint name for the future
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetEndpointName(IEndpointNameFormatter formatter);
    }
}
