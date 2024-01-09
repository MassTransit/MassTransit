namespace MassTransit
{
    public interface ISqlBusTopology :
        IBusTopology
    {
        new ISqlPublishTopology PublishTopology { get; }

        new ISqlSendTopology SendTopology { get; }

        new ISqlMessagePublishTopology<T> Publish<T>()
            where T : class;

        new ISqlMessageSendTopology<T> Send<T>()
            where T : class;
    }
}
