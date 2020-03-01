namespace MassTransit.ConsumeConfigurators
{
    using System.ComponentModel;
    using GreenPipes;


    public interface IActivityConfigurationObserverConnector
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer);
    }
}
