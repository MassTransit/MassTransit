using MassTransit.Builders;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IBusInstance _busInstance;
        readonly CheckpointStoreFactory _checkpointStoreFactory;
        readonly IReceiveEndpointConfiguration _configuration;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public EventStoreDbReceiveEndpointBuilder(
            IEventStoreDbHostConfiguration hostConfiguration,
            IBusInstance busInstance,
            IReceiveEndpointConfiguration configuration,
            ReceiveSettings receiveSettings,
            IHeadersDeserializer headersDeserializer,
            CheckpointStoreFactory checkpointStoreFactory)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _configuration = configuration;
            _receiveSettings = receiveSettings;
            _headersDeserializer = headersDeserializer;
            _checkpointStoreFactory = checkpointStoreFactory;
        }

        public IEventStoreDbReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var context = new EventStoreDbReceiveEndpointContext(_hostConfiguration, _busInstance, _configuration, _receiveSettings,
                _headersDeserializer, _checkpointStoreFactory);

            _ = context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);
            _ = context.AddOrUpdatePayload(() => _receiveSettings, _ => _receiveSettings);

            return context;
        }
    }
}
