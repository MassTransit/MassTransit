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
        readonly IServiceBusReceiveEndpointConfiguration _endpointConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public EventDataReceiverConfiguration(IServiceBusHostConfiguration hostConfiguration, IServiceBusReceiveEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
        }

        public IEventDataReceiver Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new ServiceBusReceiveEndpointBuilder(_hostConfiguration, _endpointConfiguration);

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
