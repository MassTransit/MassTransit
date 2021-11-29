namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using DependencyInjection;
    using MassTransit.Configuration;


    public class EventHubRegistrationRiderFactory :
        IRegistrationRiderFactory<IEventHubRider>
    {
        readonly Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> _configure;

        public EventHubRegistrationRiderFactory(Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> configure)
        {
            _configure = configure;
        }

        public IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
        {
            var configurator = new EventHubFactoryConfigurator();

            _configure?.Invoke(context, configurator);

            return configurator.Build(context);
        }
    }
}
