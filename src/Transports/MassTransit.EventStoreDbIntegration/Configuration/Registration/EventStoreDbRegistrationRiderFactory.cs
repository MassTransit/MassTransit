using System;
using MassTransit.Registration;
using MassTransit.EventStoreDbIntegration.Configurators;

namespace MassTransit.EventStoreDbIntegration.Registration
{
    public class EventStoreDbRegistrationRiderFactory :
        IRegistrationRiderFactory<IEventStoreDbRider>
    {
        readonly IContainerRegistrar _containerRegistrar;
        readonly Action<IRiderRegistrationContext, IEventStoreDbFactoryConfigurator> _configure;

        public EventStoreDbRegistrationRiderFactory(IContainerRegistrar containerRegistrar,
            Action<IRiderRegistrationContext, IEventStoreDbFactoryConfigurator> configure)
        {
            _containerRegistrar = containerRegistrar;
            _configure = configure;
        }
            
        public IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
        {
            var configurator = new EventStoreDbFactoryConfigurator(_containerRegistrar);

            _configure?.Invoke(context, configurator);

            return configurator.Build(context);
        }
    }
}
