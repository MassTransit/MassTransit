namespace MassTransit.AmazonSqsTransport.Transport
{
    using GreenPipes;
    using GreenPipes.Agents;


    public class ClientContextSupervisor :
        PipeContextSupervisor<ClientContext>,
        IClientContextSupervisor

    {
        public ClientContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor)
            : base(new ClientContextFactory(connectionContextSupervisor))
        {
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }
    }
}
