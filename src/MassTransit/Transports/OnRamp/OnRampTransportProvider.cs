using MassTransit.Configuration;
using System;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp
{
    public class OnRampSendTransportProvider : ISendTransportProvider
    {
        private readonly ISendTransportProvider _decoratedTransportProvider;
        private readonly IHostConfiguration _hostConfiguration;

        public OnRampSendTransportProvider(IHostConfiguration hostConfiguration, ISendTransportProvider decoratedTransportProvider)
        {
            _decoratedTransportProvider = decoratedTransportProvider;
            _hostConfiguration = hostConfiguration;
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            var context = new OnRampSendTransportContext(_hostConfiguration, address, await _decoratedTransportProvider.GetSendTransport(address));
            return new OnRampSendTransport(context);
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
            var context = new OnRampSendTransportContext(_hostConfiguration, publishAddress, await _decoratedTransportProvider.GetPublishTransport<T>(publishAddress));
            return new OnRampSendTransport(context);
        }
    }
}
