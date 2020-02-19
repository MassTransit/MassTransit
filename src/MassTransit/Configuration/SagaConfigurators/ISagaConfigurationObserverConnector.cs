namespace MassTransit.SagaConfigurators
{
    using System.ComponentModel;
    using GreenPipes;


    public interface ISagaConfigurationObserverConnector
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer);
    }
}
