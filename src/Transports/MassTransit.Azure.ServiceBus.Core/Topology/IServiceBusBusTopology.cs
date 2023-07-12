namespace MassTransit
{
    public interface IServiceBusBusTopology :
        IBusTopology
    {
        new IServiceBusPublishTopology PublishTopology { get; }

        new IServiceBusSendTopology SendTopology { get; }

        new IServiceBusMessagePublishTopology<T> Publish<T>()
            where T : class;

        new IServiceBusMessageSendTopology<T> Send<T>()
            where T : class;
    }
}
