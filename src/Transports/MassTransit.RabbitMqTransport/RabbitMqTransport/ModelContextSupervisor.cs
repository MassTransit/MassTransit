namespace MassTransit.RabbitMqTransport
{
    using Transports;


    public class ModelContextSupervisor :
        TransportPipeContextSupervisor<ModelContext>,
        IModelContextSupervisor
    {
        public ModelContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor)
            : base(new ModelContextFactory(connectionContextSupervisor))
        {
            connectionContextSupervisor.AddConsumeAgent(this);
        }

        public ModelContextSupervisor(IModelContextSupervisor modelContextSupervisor)
            : base(new ScopeModelContextFactory(modelContextSupervisor))
        {
            modelContextSupervisor.AddSendAgent(this);
        }
    }
}
