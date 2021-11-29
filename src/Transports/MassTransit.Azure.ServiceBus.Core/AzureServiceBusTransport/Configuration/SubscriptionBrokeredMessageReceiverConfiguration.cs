namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;


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

        public IServiceBusMessageReceiver Build()
        {
            IReadOnlyList<ValidationResult> result = Validate().ThrowIfContainsFailure($"{GetType().Name} configuration is invalid:");

            try
            {
                var builder = new ServiceBusSubscriptionEndpointBuilder(_hostConfiguration, _endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return new ServiceBusMessageReceiver(builder.CreateReceiveEndpointContext());
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the BrokeredMessageReceiver", ex);
            }
        }
    }
}
