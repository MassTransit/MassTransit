namespace MassTransit.AmazonSqsTransport.Transport
{
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;


    public class AmazonSqsConnectionContextSupervisor :
        PipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        public AmazonSqsConnectionContextSupervisor(IAmazonSqsHostConfiguration configuration, IAmazonSqsHostTopology hostTopology)
            : base(new ConnectionContextFactory(configuration, hostTopology))
        {
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }
    }
}
