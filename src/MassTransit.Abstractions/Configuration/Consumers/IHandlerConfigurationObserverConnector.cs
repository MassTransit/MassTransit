namespace MassTransit
{
    using System.ComponentModel;


    public interface IHandlerConfigurationObserverConnector
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer);
    }
}
