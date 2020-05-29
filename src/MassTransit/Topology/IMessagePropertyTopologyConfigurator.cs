namespace MassTransit.Topology
{
    public interface IMessagePropertyTopologyConfigurator<in TMessage, in TProperty> :
        IMessagePropertyTopologyConfigurator<TMessage>,
        IMessagePropertyTopology<TMessage, TProperty>
        where TMessage : class
    {
        new bool IsCorrelationId { set; }
    }


    public interface IMessagePropertyTopologyConfigurator<in TMessage>
        where TMessage : class
    {
    }
}
