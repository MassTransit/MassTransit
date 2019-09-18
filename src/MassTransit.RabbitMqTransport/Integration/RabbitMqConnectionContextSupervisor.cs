namespace MassTransit.RabbitMqTransport.Integration
{
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;


    public class RabbitMqConnectionContextSupervisor :
        PipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        readonly string _description;

        public RabbitMqConnectionContextSupervisor(IRabbitMqHostConfiguration configuration, IRabbitMqHostTopology hostTopology)
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
