namespace MassTransit.RabbitMqTransport
{
    using Transports;


    /// <summary>
    /// Attaches a model context to the value
    /// </summary>
    public interface IModelContextSupervisor :
        ITransportSupervisor<ModelContext>
    {
    }
}
