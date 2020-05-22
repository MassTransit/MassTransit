namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using Configurators;
    using Context;
    using Microsoft.Extensions.Logging;
    using Registration;


    public class RabbitMqRegistrationBusFactory<TContainerContext> :
        IRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly RabbitMqBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext<TContainerContext>, IRabbitMqBusFactoryConfigurator> _configure;

        public RabbitMqRegistrationBusFactory(Action<IRegistrationContext<TContainerContext>, IRabbitMqBusFactoryConfigurator> configure)
        {
            _configure = configure;

            var topologyConfiguration = new RabbitMqTopologyConfiguration(RabbitMqBusFactory.MessageTopology);
            _busConfiguration = new RabbitMqBusConfiguration(topologyConfiguration);
        }

        public IBusInstance CreateBus(IRegistrationContext<TContainerContext> context)
        {
            var configurator = new RabbitMqBusFactoryConfigurator(_busConfiguration);

            var loggerFactory = context.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);

            context.UseHealthCheck(configurator);

            _configure?.Invoke(context, configurator);

            var busControl = configurator.Build();

            return new RabbitMqBusInstance(busControl, _busConfiguration.HostConfiguration);
        }
    }
}
