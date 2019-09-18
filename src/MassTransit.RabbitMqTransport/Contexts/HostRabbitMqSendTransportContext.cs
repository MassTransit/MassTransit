namespace MassTransit.RabbitMqTransport.Contexts
{
    using Context;
    using GreenPipes;
    using Integration;


    public class HostRabbitMqSendTransportContext :
        BaseSendTransportContext,
        RabbitMqSendTransportContext
    {
        public HostRabbitMqSendTransportContext(IModelContextSupervisor modelContextSupervisor, IPipe<ModelContext> configureTopologyPipe, string exchange,
            ILogContext logContext)
            : base(logContext)
        {
            ModelContextSupervisor = modelContextSupervisor;
            ConfigureTopologyPipe = configureTopologyPipe;
            Exchange = exchange;
        }

        public IPipe<ModelContext> ConfigureTopologyPipe { get; }
        public string Exchange { get; }
        public IModelContextSupervisor ModelContextSupervisor { get; }
    }
}
