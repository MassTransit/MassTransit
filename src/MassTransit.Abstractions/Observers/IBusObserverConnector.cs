namespace MassTransit
{
    public interface IBusObserverConnector
    {
        /// <summary>
        /// Connects a bus observer to the bus to observe lifecycle events on the bus
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectBusObserver(IBusObserver observer);
    }
}
