namespace MassTransit.RabbitMqTransport.Integration
{
    using GreenPipes.Agents;


    /// <summary>
    /// Attaches a model context to the value
    /// </summary>
    public interface IModelContextSupervisor :
        ISupervisor<ModelContext>
    {
    }
}
