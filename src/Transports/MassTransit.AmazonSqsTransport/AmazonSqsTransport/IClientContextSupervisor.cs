namespace MassTransit.AmazonSqsTransport
{
    using Transports;


    /// <summary>
    /// Creates and caches a model on the connection
    /// </summary>
    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
    }
}
