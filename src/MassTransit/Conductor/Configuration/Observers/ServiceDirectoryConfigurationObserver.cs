namespace MassTransit.Conductor.Observers
{
    using Directory;
    using EndpointConfigurators;


    public class ServiceDirectoryConfigurationObserver :
        IEndpointConfigurationObserver
    {
        readonly IBusFactoryConfigurator _configurator;
        readonly ServiceDirectoryConfigurator _directoryConfigurator;

        public ServiceDirectoryConfigurationObserver(IBusFactoryConfigurator configurator, ServiceDirectoryConfigurator directoryConfigurator)
        {
            _configurator = configurator;
            _directoryConfigurator = directoryConfigurator;
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            var observer = new ServiceDirectoryEndpointConfigurationObserver(_configurator, _directoryConfigurator, configurator);

            _directoryConfigurator.AddConfigurationHandle(configurator.ConnectConsumerConfigurationObserver(observer));
            _directoryConfigurator.AddConfigurationHandle(configurator.ConnectHandlerConfigurationObserver(observer));
            _directoryConfigurator.AddConfigurationHandle(configurator.ConnectSagaConfigurationObserver(observer));
            _directoryConfigurator.AddConfigurationHandle(configurator.ConnectActivityConfigurationObserver(observer));
        }
    }
}
