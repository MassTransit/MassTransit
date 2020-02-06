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

        public ModelContextSupervisor(IModelContextSupervisor modelContextSupervisor)
            : base(new ScopeModelContextFactory(modelContextSupervisor))
        {
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }
    }
}
