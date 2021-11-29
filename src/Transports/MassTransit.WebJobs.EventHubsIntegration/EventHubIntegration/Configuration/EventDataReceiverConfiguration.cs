namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using AzureServiceBusTransport.Configuration;
    using MassTransit.Configuration;


    public class EventDataReceiverConfiguration :
        ReceiverConfiguration
    {
        readonly IServiceBusReceiveEndpointConfiguration _endpointConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public EventDataReceiverConfiguration(IServiceBusHostConfiguration hostConfiguration, IServiceBusReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
        }

        public IEventDataReceiver Build()
        {
            IReadOnlyList<ValidationResult> result = Validate().ThrowIfContainsFailure($"{GetType().Name} configuration is invalid:");

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
