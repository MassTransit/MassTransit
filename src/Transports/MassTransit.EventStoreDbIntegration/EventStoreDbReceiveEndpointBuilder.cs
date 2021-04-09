using MassTransit.Builders;
using MassTransit.Configuration;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _configuration;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public EventStoreDbReceiveEndpointBuilder(
            IEventStoreDbHostConfiguration hostConfiguration,
            IBusInstance busInstance,
            IReceiveEndpointConfiguration configuration,
            ReceiveSettings receiveSettings) : base(configuration)
        {

            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _configuration = configuration;
            _receiveSettings = receiveSettings;
        }

        public IEventStoreDbReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var context = new EventStoreDbReceiveEndpointContext(_hostConfiguration, _busInstance, _configuration, _receiveSettings);

            _ = context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);
            _ = context.AddOrUpdatePayload(() => _receiveSettings, _ => _receiveSettings);

            return context;
        }
    }
}
