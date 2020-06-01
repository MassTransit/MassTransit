namespace MassTransit.EventHubIntegration.Registration
{
    using System;
    using Configurators;
    using MassTransit.Registration;


    public class EventHubRegistrationRiderFactory<TContainerContext> :
        IRegistrationRiderFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly Action<IRiderRegistrationContext<TContainerContext>, IEventHubFactoryConfigurator> _configure;

        public EventHubRegistrationRiderFactory(Action<IRiderRegistrationContext<TContainerContext>, IEventHubFactoryConfigurator> configure)
        {
            _configure = configure;
        }

        public IBusInstanceSpecification CreateRider(IRiderRegistrationContext<TContainerContext> context)
        {
            var factoryConfigurator = new EventHubFactoryConfigurator();

            context.UseHealthCheck(factoryConfigurator);

            _configure?.Invoke(context, factoryConfigurator);

            return factoryConfigurator.Build();
        }
    }
}
