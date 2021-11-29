namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class InMemorySendTransportProvider :
        ISendTransportProvider
    {
        readonly ReceiveEndpointContext _context;
        readonly IInMemoryTransportProvider _transportProvider;

        public InMemorySendTransportProvider(IInMemoryTransportProvider transportProvider, ReceiveEndpointContext context)
        {
            _transportProvider = transportProvider;
            _context = context;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _transportProvider.NormalizeAddress(address);
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            return _transportProvider.CreateSendTransport(_context, address);
        }
    }
}
