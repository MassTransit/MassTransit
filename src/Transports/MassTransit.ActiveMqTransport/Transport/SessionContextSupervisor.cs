namespace MassTransit.ActiveMqTransport.Transport
{
    using GreenPipes;
    using GreenPipes.Agents;


    public class SessionContextSupervisor :
        PipeContextSupervisor<SessionContext>,
        ISessionContextSupervisor

    {
        public SessionContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor)
            : base(new SessionContextFactory(connectionContextSupervisor))
        {
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }
    }
}
