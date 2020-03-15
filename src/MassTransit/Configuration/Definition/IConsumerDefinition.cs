namespace MassTransit.Definition
{
    using System;
    using ConsumeConfigurators;


    public interface IConsumerDefinition :
        IDefinition
    {
        /// <summary>
        /// The consumer type
        /// </summary>
        Type ConsumerType { get; }

        /// <summary>
        /// Return the endpoint name for the consumer, using the specified formatter if necessary.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetEndpointName(IEndpointNameFormatter formatter);
    }


    public interface IConsumerDefinition<TConsumer> :
        IConsumerDefinition
        where TConsumer : class, IConsumer
    {
        /// <summary>
        /// Sets the endpoint definition, if available
        /// </summary>
        IEndpointDefinition<TConsumer> EndpointDefinition { set; }

        /// <summary>
        /// Configure the consumer on the receive endpoint
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="consumerConfigurator">The consumer configurator</param>
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator);
    }
}
