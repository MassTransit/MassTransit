namespace MassTransit.RabbitMqTransport.Integration
{
    using Transports;


    public class ModelContextSupervisor :
        TransportPipeContextSupervisor<ModelContext>,
        IModelContextSupervisor
    {
        public ModelContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor)
            : base(connectionContextSupervisor, new ModelContextFactory(connectionContextSupervisor))
        {
        }

        public ModelContextSupervisor(IModelContextSupervisor modelContextSupervisor)
            : base(modelContextSupervisor, new ScopeModelContextFactory(modelContextSupervisor))
        {
        }
    }
}
