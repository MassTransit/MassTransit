namespace MassTransit.RabbitMqTransport.Contexts
{
    using Context;
    using GreenPipes;
    using Integration;


    public interface RabbitMqSendTransportContext :
        SendTransportContext
    {
        IPipe<ModelContext> ConfigureTopologyPipe { get; }

        string Exchange { get; }

        IModelContextSupervisor ModelContextSupervisor { get; }
    }
}
