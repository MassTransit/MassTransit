namespace MassTransit.Transports.Fabric
{
    public interface IMessageFabricObserverConnector<out TContext>
        where TContext : class
    {
        ConnectHandle ConnectMessageFabricObserver(IMessageFabricObserver<TContext> observer);
    }
}
