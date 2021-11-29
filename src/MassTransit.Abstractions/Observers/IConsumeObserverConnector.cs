namespace MassTransit
{
    /// <summary>
    /// Supports connection of a consume observer
    /// </summary>
    public interface IConsumeObserverConnector
    {
        ConnectHandle ConnectConsumeObserver(IConsumeObserver observer);
    }
}
