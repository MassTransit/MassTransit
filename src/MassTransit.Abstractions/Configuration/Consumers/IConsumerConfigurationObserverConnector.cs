namespace MassTransit
{
    using System.ComponentModel;


    public interface IConsumerConfigurationObserverConnector
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer);
    }
}
