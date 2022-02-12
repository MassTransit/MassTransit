#nullable enable
namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ActiveMqPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly ActiveMqReceiveEndpointContext _context;
        readonly ISessionContextSupervisor _supervisor;

        public ActiveMqPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, ActiveMqReceiveEndpointContext context)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _context = context;
            _supervisor = context.SessionContextSupervisor;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri? publishAddress)
            where T : class
        {
            return _connectionContextSupervisor.CreatePublishTransport<T>(_context, _supervisor);
        }
    }
}
