using System;
using MassTransit.Registration;
using MassTransit.EventStoreDbIntegration.Configurators;

namespace MassTransit.EventStoreDbIntegration.Registration
{
    public class EventStoreDbRegistrationRiderFactory :
        IRegistrationRiderFactory<IEventStoreDbRider>
    {
        readonly Action<IRiderRegistrationContext, IEventStoreDbFactoryConfigurator> _configure;

        public EventStoreDbRegistrationRiderFactory(Action<IRiderRegistrationContext, IEventStoreDbFactoryConfigurator> configure)
        {
            _configure = configure;
        }
            
        public IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
        {
            var configurator = new EventStoreDbFactoryConfigurator();

            _configure?.Invoke(context, configurator);

            return configurator.Build(context);
        }
    }
}
