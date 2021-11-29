namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class ActiveMqSendTransportProvider :
        ISendTransportProvider
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly ActiveMqReceiveEndpointContext _context;
        readonly ISessionContextSupervisor _sessionContextSupervisor;

        public ActiveMqSendTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, ActiveMqReceiveEndpointContext context)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
            _context = context;
            _sessionContextSupervisor = _context.SessionContextSupervisor;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _connectionContextSupervisor.NormalizeAddress(address);
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            return _connectionContextSupervisor.CreateSendTransport(_context, _sessionContextSupervisor, address);
        }
    }
}
