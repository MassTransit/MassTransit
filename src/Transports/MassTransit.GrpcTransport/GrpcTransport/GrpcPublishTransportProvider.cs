#nullable enable
namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class GrpcPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IGrpcTransportProvider _transportProvider;
        readonly ReceiveEndpointContext _context;

        public GrpcPublishTransportProvider(IGrpcTransportProvider transportProvider, ReceiveEndpointContext context)
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
