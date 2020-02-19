namespace MassTransit.ConsumeConfigurators
{
    using System.ComponentModel;
    using GreenPipes;


    public interface IHandlerConfigurationObserverConnector
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer);
    }
}
