namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Builders;
    using MassTransit.Configuration;
    using MassTransit.Configurators;
    using Transport;


    public class BrokeredMessageReceiverConfiguration :
        ReceiverConfiguration
    {
        readonly IServiceBusBusConfiguration _busConfiguration;
        readonly IServiceBusReceiveEndpointConfiguration _endpointConfiguration;

        public BrokeredMessageReceiverConfiguration(IServiceBusBusConfiguration busConfiguration,
            IServiceBusReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _busConfiguration = busConfiguration;
            _endpointConfiguration = endpointConfiguration;
        }

        public IBrokeredMessageReceiver Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new ServiceBusReceiveEndpointBuilder(_busConfiguration.HostConfiguration.Proxy, _endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return new BrokeredMessageReceiver(builder.CreateReceiveEndpointContext());
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the BrokeredMessageReceiver", ex);
            }
        }
    }
}
