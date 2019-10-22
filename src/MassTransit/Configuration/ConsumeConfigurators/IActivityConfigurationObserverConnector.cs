namespace MassTransit.ConsumeConfigurators
{
    using GreenPipes;


    public interface IActivityConfigurationObserverConnector
    {
        /// <summary>
        /// Connect a configuration observer to the bus configurator, which is invoked as routing slip activities are configured.
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer);
    }
}
