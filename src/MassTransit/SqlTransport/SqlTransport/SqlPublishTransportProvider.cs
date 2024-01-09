#nullable enable
namespace MassTransit.SqlTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class SqlPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly SqlReceiveEndpointContext _context;

        public SqlPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, SqlReceiveEndpointContext context)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _context = context;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri? publishAddress)
            where T : class
        {
            return _connectionContextSupervisor.CreatePublishTransport<T>(_context, publishAddress);
        }
    }
}
