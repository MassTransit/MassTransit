namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class GrpcSendTransportProvider :
        ISendTransportProvider
    {
        readonly ReceiveEndpointContext _context;
        readonly IGrpcTransportProvider _transportProvider;

        public GrpcSendTransportProvider(IGrpcTransportProvider transportProvider, ReceiveEndpointContext context)
        {
            _transportProvider = transportProvider;
            _context = context;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _transportProvider.NormalizeAddress(address);
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return _transportProvider.CreateSendTransport(_context, address);
        }
    }
}
