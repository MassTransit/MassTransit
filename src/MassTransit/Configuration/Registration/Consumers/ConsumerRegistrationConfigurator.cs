namespace MassTransit.Registration
{
    using System;
    using Definition;


    public class ConsumerRegistrationConfigurator<TConsumer> :
        IConsumerRegistrationConfigurator<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IRegistrationConfigurator _configurator;

        public ConsumerRegistrationConfigurator(IRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void Endpoint(Action<IConsumerEndpointRegistrationConfigurator> configure)
        {
            var configurator = new ConsumerEndpointRegistrationConfigurator<TConsumer>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ConsumerEndpointDefinition<TConsumer>, TConsumer>(configurator.Settings);
        }
    }
}
