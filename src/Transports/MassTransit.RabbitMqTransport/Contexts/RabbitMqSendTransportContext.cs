namespace MassTransit.RabbitMqTransport.Contexts
{
    using Context;
    using GreenPipes;
    using Integration;


    public interface RabbitMqSendTransportContext :
        SendTransportContext,
        IPipeContextSource<ModelContext>
    {
        IPipe<ModelContext> ConfigureTopologyPipe { get; }

        string Exchange { get; }

        IModelContextSupervisor ModelContextSupervisor { get; }
    }
}
