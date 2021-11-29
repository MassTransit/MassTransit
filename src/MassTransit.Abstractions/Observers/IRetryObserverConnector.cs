namespace MassTransit
{
    public interface IRetryObserverConnector
    {
        /// <summary>
        /// Connect an observer to the filter and/or pipe
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectRetryObserver(IRetryObserver observer);
    }
}
