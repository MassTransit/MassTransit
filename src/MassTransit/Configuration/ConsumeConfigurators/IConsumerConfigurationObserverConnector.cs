namespace MassTransit.ConsumeConfigurators
{
    using System.ComponentModel;
    using GreenPipes;


    public interface IConsumerConfigurationObserverConnector
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer);
    }
}
