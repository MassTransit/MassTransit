namespace MassTransit
{
    /// <summary>
    /// Supports connection of a message observer to the pipeline
    /// </summary>
    public interface IConsumeMessageObserverConnector
    {
        ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class;
    }


    /// <summary>
    /// Supports connection of a message observer to the pipeline
    /// </summary>
    public interface IConsumeMessageObserverConnector<out T>
        where T : class
    {
        ConnectHandle ConnectConsumeMessageObserver(IConsumeMessageObserver<T> observer);
    }
}
