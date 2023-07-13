namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Internals;
    using JobService;
    using Transports;


    public class JobServiceConsumerConfigurationObserver :
        IConsumerConfigurationObserver
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly Action<IReceiveEndpointConfigurator> _configureEndpoint;
        readonly Dictionary<Type, IConsumeConfigurator> _consumerConfigurators;
        readonly JobServiceSettings _settings;
        bool _endpointConfigured;

        public JobServiceConsumerConfigurationObserver(IReceiveEndpointConfigurator configurator, JobServiceSettings settings,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _configurator = configurator;
            _configureEndpoint = configureEndpoint;

            _settings = settings;

            _consumerConfigurators = new Dictionary<Type, IConsumeConfigurator>();
        }

        public void ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
            where T : class
        {
            if (typeof(T).HasInterface(typeof(IJobConsumer<>)))
            {
                _consumerConfigurators.Add(typeof(T), configurator);

                configurator.Options(_settings);

                if (_endpointConfigured)
                    return;

                _configureEndpoint(_configurator);

                _endpointConfigured = true;
            }
        }

        public void ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
            where T : class
            where TMessage : class
        {
            if (typeof(T).HasInterface<IJobConsumer<TMessage>>()
                && _consumerConfigurators.TryGetValue(typeof(T), out var value)
                && value is IConsumerConfigurator<T> consumerConfigurator)
            {
                var options = consumerConfigurator.Options<JobOptions<TMessage>>();

                var jobTypeId = JobMetadataCache<T, TMessage>.GenerateJobTypeId(_configurator.InputAddress.GetEndpointName());
                var jobTypeName = JobMetadataCache<T, TMessage>.GenerateJobTypeName(_configurator.InputAddress.GetEndpointName());

                _settings.JobService.RegisterJobType(_configurator, options, jobTypeId, jobTypeName);
            }
        }
    }
}
