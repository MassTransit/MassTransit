namespace MassTransit.RabbitMqTransport
{
    using Transports;


    public interface RabbitMqSendTransportContext :
        SendTransportContext,
        IPipeContextSource<ModelContext>
    {
        IPipe<ModelContext> ConfigureTopologyPipe { get; }
        IPipe<ModelContext> DelayConfigureTopologyPipe { get; }

        string Exchange { get; }
        string DelayExchange { get; }

        IModelContextSupervisor ModelContextSupervisor { get; }
    }
}
