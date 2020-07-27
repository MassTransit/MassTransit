namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Builders;
    using MassTransit.Configuration;
    using MassTransit.Configurators;
    using Transport;


    public class QueueBrokeredMessageReceiverConfiguration :
        ReceiverConfiguration
    {
        readonly IServiceBusReceiveEndpointConfiguration _endpointConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public QueueBrokeredMessageReceiverConfiguration(IServiceBusHostConfiguration hostConfiguration,
            IServiceBusReceiveEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
        }

        public IBrokeredMessageReceiver Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new ServiceBusReceiveEndpointBuilder(_hostConfiguration, _endpointConfiguration);

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
