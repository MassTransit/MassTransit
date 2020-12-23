namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Builders;
    using MassTransit.Configuration;
    using MassTransit.Configurators;
    using Transport;


    public class SubscriptionBrokeredMessageReceiverConfiguration :
        ReceiverConfiguration
    {
        readonly IServiceBusSubscriptionEndpointConfiguration _endpointConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public SubscriptionBrokeredMessageReceiverConfiguration(IServiceBusHostConfiguration hostConfiguration,
            IServiceBusSubscriptionEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
        }

        public IBrokeredMessageReceiver Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new ServiceBusSubscriptionEndpointBuilder(_hostConfiguration, _endpointConfiguration);

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
