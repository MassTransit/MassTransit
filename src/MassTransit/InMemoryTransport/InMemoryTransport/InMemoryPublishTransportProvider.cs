#nullable enable
namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class InMemoryPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly ReceiveEndpointContext _context;
        readonly IInMemoryTransportProvider _transportProvider;

        public InMemoryPublishTransportProvider(IInMemoryTransportProvider transportProvider, ReceiveEndpointContext context)
        {
            _transportProvider = transportProvider;
            _context = context;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri? publishAddress)
            where T : class
        {
            return _transportProvider.CreatePublishTransport<T>(_context, publishAddress);
        }
    }
}
