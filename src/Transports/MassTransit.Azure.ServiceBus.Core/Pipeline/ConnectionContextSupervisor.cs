namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Configuration;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;


    public class ConnectionContextSupervisor :
        PipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        public ConnectionContextSupervisor(IServiceBusHostConfiguration configuration)
            : base(new ConnectionContextFactory(configuration))
        {
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }
    }
}
