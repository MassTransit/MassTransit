namespace MassTransit.Configuration
{
    using System;
    using JobService;


    public class JobServiceEndpointConfigurationObserver :
        IEndpointConfigurationObserver
    {
        readonly Action<IReceiveEndpointConfigurator> _configureEndpoint;
        readonly JobServiceSettings _settings;

        public JobServiceEndpointConfigurationObserver(JobServiceSettings settings, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _settings = settings;
            _configureEndpoint = configureEndpoint;
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            configurator.ConnectConsumerConfigurationObserver(new JobServiceConsumerConfigurationObserver(configurator, _settings, _configureEndpoint));
        }
    }
}
