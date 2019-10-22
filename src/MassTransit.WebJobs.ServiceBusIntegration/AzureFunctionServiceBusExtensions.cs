namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Configuration;
    using Azure.ServiceBus.Core.Transport;
    using Configuration;
    using Microsoft.Azure.WebJobs;


    public static class AzureFunctionServiceBusExtensions
    {
        public static IBrokeredMessageReceiver CreateBrokeredMessageReceiver(this IBusFactorySelector selector, IBinder binder,
            Action<IWebJobReceiverConfigurator> configure)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);

            var endpointConfiguration = new BrokeredMessageReceiverServiceBusEndpointConfiguration(busConfiguration);

            var configurator = new WebJobBrokeredMessageReceiverSpecification(binder, endpointConfiguration);

            configure(configurator);

            return configurator.Build();
        }
    }
}
