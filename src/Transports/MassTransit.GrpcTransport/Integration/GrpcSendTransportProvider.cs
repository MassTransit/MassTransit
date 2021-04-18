namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using Transports;


    public class GrpcSendTransportProvider :
        ISendTransportProvider
    {
        readonly GrpcReceiveEndpointContext _context;
        readonly IGrpcHostConfiguration _hostConfiguration;

        public GrpcSendTransportProvider(IGrpcHostConfiguration hostConfiguration, GrpcReceiveEndpointContext context)
        {
            _hostConfiguration = hostConfiguration;
            _context = context;
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            return _hostConfiguration.TransportProvider.GetSendTransport(_context, address);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _hostConfiguration.TransportProvider.NormalizeAddress(address);
        }
    }
}