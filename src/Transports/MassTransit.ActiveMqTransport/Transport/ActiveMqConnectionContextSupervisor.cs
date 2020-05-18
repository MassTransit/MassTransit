namespace MassTransit.ActiveMqTransport.Transport
{
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;


    public class ActiveMqConnectionContextSupervisor :
        PipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        readonly string _description;

        public ActiveMqConnectionContextSupervisor(IActiveMqHostConfiguration configuration, IActiveMqHostTopology hostTopology)
            : base(new ConnectionContextFactory(configuration, hostTopology))
        {
            _description = configuration.Description;
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
