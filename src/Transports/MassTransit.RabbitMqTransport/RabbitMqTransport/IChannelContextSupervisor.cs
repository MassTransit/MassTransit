namespace MassTransit.RabbitMqTransport
{
    using Transports;


    /// <summary>
    /// Attaches a channel context to the value
    /// </summary>
    public interface IChannelContextSupervisor :
        ITransportSupervisor<ChannelContext>
    {
    }
}
