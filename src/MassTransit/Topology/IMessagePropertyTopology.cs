namespace MassTransit.Topology
{
    public interface IMessagePropertyTopology<in TMessage, in TProperty>
        where TMessage : class
    {
        bool IsCorrelationId { get; }
    }
}
