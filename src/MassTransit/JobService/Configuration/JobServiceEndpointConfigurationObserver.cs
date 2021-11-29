namespace MassTransit.Configuration
{
    using System;


    public class JobServiceEndpointConfigurationObserver :
        IEndpointConfigurationObserver
    {
        readonly Action<IReceiveEndpointConfigurator> _configureEndpoint;
        readonly JobServiceOptions _jobServiceOptions;

        public JobServiceEndpointConfigurationObserver(JobServiceOptions jobServiceOptions, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _jobServiceOptions = jobServiceOptions;
            _configureEndpoint = configureEndpoint;
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            var observer = new JobServiceConsumerConfigurationObserver(configurator, _jobServiceOptions, _configureEndpoint);
            configurator.ConnectConsumerConfigurationObserver(observer);
        }
    }
}
