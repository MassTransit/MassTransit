namespace MassTransit.EventHubIntegration.Registration
{
    using System;
    using Configurators;
    using MassTransit.Registration;
    using Transport;


    public class EventHubRegistrationRiderFactory<TContainerContext> :
        RegistrationRiderFactory<TContainerContext, IEventHubRider>
        where TContainerContext : class
    {
        readonly Action<IRiderRegistrationContext<TContainerContext>, IEventHubFactoryConfigurator> _configure;

        public EventHubRegistrationRiderFactory(Action<IRiderRegistrationContext<TContainerContext>, IEventHubFactoryConfigurator> configure)
        {
            _configure = configure;
        }

        public override IBusInstanceSpecification CreateRider(IRiderRegistrationContext<TContainerContext> context)
        {
            var configurator = new EventHubFactoryConfigurator();

            ConfigureRider(configurator, context);

            _configure?.Invoke(context, configurator);

            return configurator.Build();
        }
    }
}
