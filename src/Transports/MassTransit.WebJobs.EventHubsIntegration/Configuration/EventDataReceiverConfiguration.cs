namespace MassTransit.WebJobs.EventHubsIntegration.Configuration
{
    using System;
    using Azure.ServiceBus.Core.Builders;
    using Azure.ServiceBus.Core.Configuration;
    using Configurators;
    using MassTransit.Configuration;
    using Transport;


    public class EventDataReceiverConfiguration :
        ReceiverConfiguration
    {
        readonly IServiceBusBusConfiguration _busConfiguration;
        readonly IServiceBusReceiveEndpointConfiguration _endpointConfiguration;

        public EventDataReceiverConfiguration(IServiceBusBusConfiguration busConfiguration,
            IServiceBusReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _busConfiguration = busConfiguration;
            _endpointConfiguration = endpointConfiguration;
        }

        public IEventDataReceiver Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new ServiceBusReceiveEndpointBuilder(_busConfiguration.HostConfiguration.Proxy, _endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return new EventDataReceiver(builder.CreateReceiveEndpointContext());
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the EventDataReceiver", ex);
            }
        }
    }
}
