namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class SessionContextSupervisor :
        TransportPipeContextSupervisor<SessionContext>,
        ISessionContextSupervisor
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;
        readonly ISessionContextSupervisor _supervisor;

        public SessionContextSupervisor(IConnectionContextSupervisor supervisor)
            : base(supervisor, new SessionContextFactory(supervisor))
        {
            _connectionContextSupervisor = supervisor;
        }

        public SessionContextSupervisor(ISessionContextSupervisor supervisor)
            : base(new ScopeSessionContextFactory(supervisor))
        {
            _supervisor = supervisor;

            supervisor.AddSendAgent(this);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return _supervisor != null
                ? _supervisor.NormalizeAddress(address)
                : _connectionContextSupervisor.NormalizeAddress(address);
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            return _supervisor != null
                ? _supervisor.GetSendTransport(address)
                : _connectionContextSupervisor.CreateSendTransport(this, address);
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _supervisor != null
                ? _supervisor.GetPublishTransport<T>(publishAddress)
                : _connectionContextSupervisor.CreatePublishTransport<T>(this);
        }
    }
}
