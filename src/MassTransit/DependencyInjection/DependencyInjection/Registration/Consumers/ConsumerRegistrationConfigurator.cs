namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public class ConsumerRegistrationConfigurator<TConsumer> :
        IConsumerRegistrationConfigurator<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IRegistrationConfigurator _configurator;

        public ConsumerRegistrationConfigurator(IRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            var configurator = new EndpointRegistrationConfigurator<TConsumer>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ConsumerEndpointDefinition<TConsumer>, TConsumer>(configurator.Settings);
        }
    }
}
