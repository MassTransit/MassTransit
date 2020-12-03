namespace MassTransit.RabbitMqTransport.Integration
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
