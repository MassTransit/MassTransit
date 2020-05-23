namespace MassTransit.KafkaIntegration.Configuration.Configurators
{
    using System;
    using MassTransit.Configuration;
    using MassTransit.Configurators;
    using Transport;


    public class KafkaReceiverConfiguration<TKey, TValue> :
        ReceiverConfiguration
        where TValue : class
    {
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly IHostConfiguration _hostConfiguration;

        public KafkaReceiverConfiguration(IHostConfiguration hostConfiguration, IReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
        }

        public IKafkaReceiver<TKey, TValue> Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new ServiceBusReceiveEndpointBuilder(_hostConfiguration, _endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return new KafkaReceiver<TKey, TValue>(builder.CreateReceiveEndpointContext());
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the EventDataReceiver", ex);
            }
        }
    }
}
