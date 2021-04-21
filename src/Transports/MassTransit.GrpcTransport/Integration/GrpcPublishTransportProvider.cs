namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using Transports;


    public class GrpcPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly GrpcReceiveEndpointContext _context;
        readonly IGrpcHostConfiguration _hostConfiguration;

        public GrpcPublishTransportProvider(IGrpcHostConfiguration hostConfiguration, GrpcReceiveEndpointContext context)
        {
            _hostConfiguration = hostConfiguration;
            _context = context;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _hostConfiguration.TransportProvider.GetPublishTransport<T>(_context, publishAddress);
        }
    }
}