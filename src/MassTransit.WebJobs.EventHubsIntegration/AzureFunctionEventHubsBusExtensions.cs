namespace MassTransit
{
    using System;
    using System.Threading;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Configuration;
    using Azure.ServiceBus.Core.Configurators;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using WebJobs.EventHubsIntegration;
    using WebJobs.EventHubsIntegration.Configuration;


    public static class AzureFunctionEventHubsBusExtensions
    {
        public static IEventDataReceiver CreateEventDataReceiver(this IBusFactorySelector selector, IBinder binder, ILogger logger,
            Action<IReceiverConfigurator> configure)
        {
            return CreateEventDataReceiver(binder, logger, CancellationToken.None, configure);
        }

        public static IEventDataReceiver CreateEventDataReceiver(this IBinder binder, ILogger logger, CancellationToken cancellationToken,
            Action<IReceiverConfigurator> configure)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);

            var receiveEndpointConfiguration = busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration("unspecified");

            var configurator = new WebJobEventDataReceiverSpecification(binder, logger, receiveEndpointConfiguration, cancellationToken);

            configure(configurator);

            return configurator.Build();
        }
    }
}
