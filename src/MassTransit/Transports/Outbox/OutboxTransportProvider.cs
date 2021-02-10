using MassTransit.Configuration;
using System;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class OutboxSendTransportProvider : ISendTransportProvider
    {
        private readonly ISendTransportProvider _decoratedTransportProvider;
        private readonly IHostConfiguration _hostConfiguration;

        public OutboxSendTransportProvider(IHostConfiguration hostConfiguration, ISendTransportProvider decoratedTransportProvider)
        {
            _decoratedTransportProvider = decoratedTransportProvider;
            _hostConfiguration = hostConfiguration;
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            var context = new OutboxSendTransportContext(_hostConfiguration, address, await _decoratedTransportProvider.GetSendTransport(address));
            return new OutboxSendTransport(context);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _decoratedTransportProvider.NormalizeAddress(address);
        }
    }

    public class OutboxPublishTransportProvider : IPublishTransportProvider
    {
        private readonly IPublishTransportProvider _decoratedTransportProvider;
        private readonly IHostConfiguration _hostConfiguration;

        public OutboxPublishTransportProvider(IHostConfiguration hostConfiguration, IPublishTransportProvider decoratedTransportProvider)
        {
            _decoratedTransportProvider = decoratedTransportProvider;
            _hostConfiguration = hostConfiguration;
        }

        public async Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress) where T : class
        {
            var context = new OutboxSendTransportContext(_hostConfiguration, publishAddress, await _decoratedTransportProvider.GetPublishTransport<T>(publishAddress));
            return new OutboxSendTransport(context);
        }
    }
}
