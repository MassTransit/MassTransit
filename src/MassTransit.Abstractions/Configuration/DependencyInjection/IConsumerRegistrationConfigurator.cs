namespace MassTransit
{
    using System;


    public interface IConsumerRegistrationConfigurator<TConsumer> :
        IConsumerRegistrationConfigurator
        where TConsumer : class, IConsumer
    {
    }


    public interface IConsumerRegistrationConfigurator
    {
        void Endpoint(Action<IEndpointRegistrationConfigurator> configure);
    }
}
