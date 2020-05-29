namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions
{
    public interface IMessageSessionIdFormatter<in TMessage>
        where TMessage : class
    {
        string FormatSessionId(SendContext<TMessage> context);
    }
}
