namespace MassTransit.EventHubIntegration.Registration
{
    using System;
    using Configurators;
    using MassTransit.Registration;
    using Transport;


    public class EventHubRegistrationRiderFactory :
        RegistrationRiderFactory<IEventHubRider>
    {
        readonly Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> _configure;

        public EventHubRegistrationRiderFactory(Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> configure)
        {
            _configure = configure;
        }

        public override IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
        {
            var configurator = new EventHubFactoryConfigurator();

            ConfigureRider(configurator, context);

            _configure?.Invoke(context, configurator);

            return configurator.Build();
        }
    }
}
