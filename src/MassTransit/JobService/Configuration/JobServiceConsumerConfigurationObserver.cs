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
        bool _configured;

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
                configurator.Options<JobServiceOptions>(options => options.Set(_jobServiceOptions));

                if (_configured)
                    return;

                _configureEndpoint(_configurator);

                _configured = true;
            }
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
        }
    }
}
