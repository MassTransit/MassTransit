using MassTransit.Configuration;
using MassTransit.Transports.Outbox;

namespace MassTransit.Context
{
    public abstract class BaseReceiveEndpointOutboxTransportContext : BaseReceiveEndpointContext
    {
        private readonly IHostConfiguration _hostConfiguration;

        public BaseReceiveEndpointOutboxTransportContext(IHostConfiguration hostConfiguration, IReceiveEndpointConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
        }

        protected override IPublishTransportProvider CreateDecoratedPublishTransportProvider()
        {
            var transport = CreatePublishTransportProvider();

            if (_hostConfiguration.UseOutbox)
            {
                return new OutboxPublishTransportProvider(_hostConfiguration, transport);
            }
            else
            {
                return transport;
            }
        }

        protected override ISendTransportProvider CreateDecoratedSendTransportProvider()
        {
            var transport = CreateSendTransportProvider();

            if (_hostConfiguration.UseOutbox)
            {
                return new OnRampSendTransportProvider(_hostConfiguration, transport);
            }
            else
            {
                return transport;
            }
        }
    }
}
