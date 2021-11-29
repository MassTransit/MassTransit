namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Transports;


    public class ActiveMqRegistrationBusFactory :
        TransportRegistrationBusFactory<IActiveMqReceiveEndpointConfigurator>
    {
        readonly ActiveMqBusConfiguration _busConfiguration;
        readonly Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator> _configure;

        public ActiveMqRegistrationBusFactory(Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator> configure)
            : this(new ActiveMqBusConfiguration(new ActiveMqTopologyConfiguration(ActiveMqBusFactory.MessageTopology)), configure)
        {
        }

        ActiveMqRegistrationBusFactory(ActiveMqBusConfiguration busConfiguration,
            Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var configurator = new ActiveMqBusFactoryConfigurator(_busConfiguration);

            var options = context.GetRequiredService<IOptionsMonitor<ActiveMqTransportOptions>>().Get(busName);

            configurator.Host(options.Host, options.Port, h =>
            {
                if (!string.IsNullOrWhiteSpace(options.User))
                    h.Username(options.User);
                if (!string.IsNullOrWhiteSpace(options.Pass))
                    h.Password(options.Pass);

                if (options.UseSsl)
                    h.UseSsl();
            });

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
