namespace MassTransit.AmazonSqsTransport.Transport
{
    using GreenPipes.Agents;


    /// <summary>
    /// Creates and caches a model on the connection
    /// </summary>
    public interface IClientContextSupervisor :
        ISupervisor<ClientContext>
    {
    }
}
