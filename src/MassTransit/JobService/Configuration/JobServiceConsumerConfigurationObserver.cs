namespace MassTransit.JobService.Configuration
{
    using System;
    using System.Collections.Generic;
    using ConsumeConfigurators;
    using Internals.Extensions;


    public class JobServiceConsumerConfigurationObserver :
        IConsumerConfigurationObserver
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly Action<IReceiveEndpointConfigurator> _configureEndpoint;
        readonly Dictionary<Type, IConsumeConfigurator> _consumerConfigurators;
        readonly JobServiceOptions _jobServiceOptions;
        bool _endpointConfigured;

        public JobServiceConsumerConfigurationObserver(IReceiveEndpointConfigurator configurator, JobServiceOptions jobServiceOptions,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _configurator = configurator;
            _jobServiceOptions = jobServiceOptions;
            _configureEndpoint = configureEndpoint;

            _consumerConfigurators = new Dictionary<Type, IConsumeConfigurator>();
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
            if (typeof(T).HasInterface(typeof(IJobConsumer<>)))
            {
                _consumerConfigurators.Add(typeof(T), configurator);

                configurator.Options(_jobServiceOptions);

                if (_endpointConfigured)
                    return;

                _configureEndpoint(_configurator);

                _endpointConfigured = true;
            }
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            if (typeof(T).HasInterface<IJobConsumer<TMessage>>()
                && _consumerConfigurators.TryGetValue(typeof(T), out var value)
                && value is IConsumerConfigurator<T> consumerConfigurator)
            {
                var options = consumerConfigurator.Options<JobOptions<TMessage>>();

                _jobServiceOptions.JobService.RegisterJobType(_configurator, options);
            }
        }
    }
}
