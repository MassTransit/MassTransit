namespace MassTransit.RabbitMqTransport.Integration
{
    using GreenPipes;
    using GreenPipes.Agents;


    public class ModelContextSupervisor :
        PipeContextSupervisor<ModelContext>,
        IModelContextSupervisor

    {
        public ModelContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor)
            : base(new ModelContextFactory(connectionContextSupervisor))
        {
        }

        public ModelContextSupervisor(IPipeContextFactory<ModelContext> contextFactory)
            : base(contextFactory)
        {
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }
    }
}
