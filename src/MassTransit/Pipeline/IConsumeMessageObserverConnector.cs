namespace MassTransit.Pipeline
{
    using GreenPipes;


    /// <summary>
    /// Supports connection of a message observer to the pipeline
    /// </summary>
    public interface IConsumeMessageObserverConnector
    {
        ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class;
    }
}
