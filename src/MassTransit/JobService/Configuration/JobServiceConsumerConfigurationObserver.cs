namespace MassTransit.JobService.Configuration
{
    using System;
    using ConsumeConfigurators;
    using Internals.Extensions;


    public class JobServiceConsumerConfigurationObserver :
        IConsumerConfigurationObserver
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly Action<IReceiveEndpointConfigurator> _configureEndpoint;
        readonly JobServiceOptions _jobServiceOptions;
        bool _endpointConfigured;

        public JobServiceConsumerConfigurationObserver(IReceiveEndpointConfigurator configurator, JobServiceOptions jobServiceOptions,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _configurator = configurator;
            _jobServiceOptions = jobServiceOptions;
            _configureEndpoint = configureEndpoint;
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
            if (typeof(T).HasInterface(typeof(IJobConsumer<>)))
            {
                configurator.Options(_jobServiceOptions);

                if (_endpointConfigured)
                    return;

                _configureEndpoint(_configurator);

                _endpointConfigured = true;
            }
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
        }
    }
}
