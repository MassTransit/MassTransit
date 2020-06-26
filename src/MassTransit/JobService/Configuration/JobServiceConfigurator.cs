namespace MassTransit.JobService.Configuration
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Components.StateMachines;
    using Conductor;
    using GreenPipes;
    using Saga;


    public class JobServiceConfigurator<TReceiveEndpointConfigurator> :
        IJobServiceConfigurator,
        ISpecification
        where TReceiveEndpointConfigurator : IReceiveEndpointConfigurator
    {
        readonly IServiceInstanceConfigurator<TReceiveEndpointConfigurator> _instanceConfigurator;
        readonly JobServiceOptions _options;
        bool _endpointsConfigured;
        ISagaRepository<JobAttemptSaga> _jobAttemptRepository;
        IReceiveEndpointConfigurator _jobAttemptSagaEndpointConfigurator;
        ISagaRepository<JobSaga> _jobRepository;
        IReceiveEndpointConfigurator _jobSagaEndpointConfigurator;
        ISagaRepository<JobTypeSaga> _jobTypeRepository;
        IReceiveEndpointConfigurator _jobTypeSagaEndpointConfigurator;

        public JobServiceConfigurator(IServiceInstanceConfigurator<TReceiveEndpointConfigurator> instanceConfigurator)
        {
            _instanceConfigurator = instanceConfigurator;

            JobService = new JobService(_instanceConfigurator.InstanceAddress);

            instanceConfigurator.ConnectBusObserver(new JobServiceBusObserver(JobService));
            instanceConfigurator.AddSpecification(this);

            _options = _instanceConfigurator.Options<JobServiceOptions>(options =>
            {
                options.JobService = JobService;

                options.JobTypeSagaEndpointName = _instanceConfigurator.EndpointNameFormatter.Saga<JobTypeSaga>();
                options.JobStateSagaEndpointName = _instanceConfigurator.EndpointNameFormatter.Saga<JobSaga>();
                options.JobAttemptSagaEndpointName = _instanceConfigurator.EndpointNameFormatter.Saga<JobAttemptSaga>();
            });
        }

        IJobService JobService { get; }

        public ISagaRepository<JobTypeSaga> Repository
        {
            set => _jobTypeRepository = value;
        }

        public ISagaRepository<JobSaga> JobRepository
        {
            set => _jobRepository = value;
        }

        public ISagaRepository<JobAttemptSaga> JobAttemptRepository
        {
            set => _jobAttemptRepository = value;
        }

        public string JobServiceStateEndpointName
        {
            set => _options.JobTypeSagaEndpointName = value;
        }

        public string JobServiceJobStateEndpointName
        {
            set => _options.JobStateSagaEndpointName = value;
        }

        public string JobServiceJobAttemptStateEndpointName
        {
            set => _options.JobAttemptSagaEndpointName = value;
        }

        public TimeSpan SlotRequestTimeout
        {
            set => _options.SlotRequestTimeout = value;
        }

        public TimeSpan SlotWaitTime
        {
            set => _options.SlotWaitTime = value;
        }

        public TimeSpan StatusCheckInterval
        {
            set => _options.StatusCheckInterval = value;
        }

        public TimeSpan StartJobTimeout
        {
            set => _options.StartJobTimeout = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            ISpecification turnoutOptions = _options;

            return turnoutOptions.Validate();
        }

        public void ApplyJobServiceOptions(JobServiceOptions jobServiceOptions)
        {
            _options.Set(jobServiceOptions);
        }

        public void ConfigureJobServiceEndpoints()
        {
            if (_endpointsConfigured)
                return;

            _instanceConfigurator.ReceiveEndpoint(_options.JobStateSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobStateMachine(_options);

                e.StateMachineSaga(stateMachine, _jobRepository ?? new InMemorySagaRepository<JobSaga>());

                _jobSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ReceiveEndpoint(_options.JobAttemptSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobAttemptStateMachine(_options);

                e.StateMachineSaga(stateMachine, _jobAttemptRepository ?? new InMemorySagaRepository<JobAttemptSaga>());

                _jobAttemptSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ReceiveEndpoint(_options.JobTypeSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 200, 300, 500, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobTypeStateMachine();

                e.StateMachineSaga(stateMachine, _jobTypeRepository ?? new InMemorySagaRepository<JobTypeSaga>());

                _jobTypeSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ConnectEndpointConfigurationObserver(new JobServiceEndpointConfigurationObserver(_options, cfg =>
            {
                cfg.AddDependency(_instanceConfigurator);
                cfg.AddDependency(_jobTypeSagaEndpointConfigurator);
                cfg.AddDependency(_jobSagaEndpointConfigurator);
                cfg.AddDependency(_jobAttemptSagaEndpointConfigurator);
            }));

            _endpointsConfigured = true;
        }
    }
}
