#nullable enable
namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class AmazonSqsPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly SqsReceiveEndpointContext _context;

        public AmazonSqsPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, SqsReceiveEndpointContext context)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _context = context;
            _clientContextSupervisor = context.ClientContextSupervisor;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri? publishAddress)
            where T : class
        {
            return _connectionContextSupervisor.CreatePublishTransport<T>(_context, _clientContextSupervisor);
        }
    }
}
