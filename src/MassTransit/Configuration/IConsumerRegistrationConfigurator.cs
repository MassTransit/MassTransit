namespace MassTransit
{
    using System;
    using Registration;


    public interface IConsumerRegistrationConfigurator<TConsumer> :
        IConsumerRegistrationConfigurator
        where TConsumer : class, IConsumer
    {
    }


    public interface IConsumerRegistrationConfigurator
    {
        void Endpoint(Action<IConsumerEndpointRegistrationConfigurator> configure);
    }
}
