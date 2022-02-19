namespace MassTransit.Transports.Fabric
{
    public interface IMessageQueue<in TContext, T> :
        IMessageSink<T>
        where TContext : class
        where T : class
    {
        string Name { get; }

        TopologyHandle ConnectMessageReceiver(TContext nodeContext, IMessageReceiver<T> receiver);
    }
}
