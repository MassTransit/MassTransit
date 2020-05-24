namespace MassTransit.KafkaIntegration.Configuration.Configurators
{
    using System;
    using MassTransit.Configuration;
    using MassTransit.Configurators;
    using Registration;
    using Registration.Attachments;
    using Transport;


    public class KafkaReceiverConfiguration<TKey, TValue> :
        ReceiverConfiguration
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;

        public KafkaReceiverConfiguration(IBusInstance busInstance, IReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;
        }

        public IKafkaReceiver<TKey, TValue> Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new BusAttachmentReceiveEndpointBuilder(_busInstance, _endpointConfiguration);

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
