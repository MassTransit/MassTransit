namespace MassTransit.JobService.Configuration
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Components.StateMachines;
    using Conductor.Configuration;
    using ConsumeConfigurators;
    using GreenPipes;
    using Saga;


    public class JobServiceConfigurator<TReceiveEndpointConfigurator> :
        IJobServiceConfigurator,
        IReceiveEndpointSpecification
        where TReceiveEndpointConfigurator : IReceiveEndpointConfigurator
    {
        readonly IServiceInstanceConfigurator<TReceiveEndpointConfigurator> _instanceConfigurator;
        readonly OptionsSet _optionsSet;
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

            ConsumerConvention.Register<JobConsumerConvention>();

            _optionsSet = new OptionsSet();

            instanceConfigurator.ConnectBusObserver(new JobServiceBusObserver(JobService));
            instanceConfigurator.AddSpecification(this);

            _optionsSet.Configure<JobServiceOptions>(options =>
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
            set { _optionsSet.Configure<JobServiceOptions>(options => options.JobTypeSagaEndpointName = value); }
        }

        public string JobServiceJobStateEndpointName
        {
            set { _optionsSet.Configure<JobServiceOptions>(options => options.JobStateSagaEndpointName = value); }
        }

        public string JobServiceJobAttemptStateEndpointName
        {
            set { _optionsSet.Configure<JobServiceOptions>(options => options.JobAttemptSagaEndpointName = value); }
        }

        public TimeSpan JobSlotWaitTime
        {
            set { _optionsSet.Configure<JobServiceOptions>(options => options.SlotWaitTime = value); }
        }

        public TimeSpan JobStatusCheckInterval
        {
            set { _optionsSet.Configure<JobServiceOptions>(options => options.StatusCheckInterval = value); }
        }

        public IEnumerable<ValidationResult> Validate()
        {
            ISpecification turnoutOptions = _optionsSet.Configure<JobServiceOptions>();

            return turnoutOptions.Validate();
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
        }

        public void ConfigureJobServiceEndpoints()
        {
            if (_endpointsConfigured)
                return;

            var options = _optionsSet.Configure<JobServiceOptions>();

            _instanceConfigurator.ReceiveEndpoint(options.JobStateSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobStateMachine(options);

                e.StateMachineSaga(stateMachine, _jobRepository ?? new InMemorySagaRepository<JobSaga>());

                _jobSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ReceiveEndpoint(options.JobAttemptSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobAttemptStateMachine(options);

                e.StateMachineSaga(stateMachine, _jobAttemptRepository ?? new InMemorySagaRepository<JobAttemptSaga>());

                _jobAttemptSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ReceiveEndpoint(options.JobTypeSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 200, 300, 500, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobTypeStateMachine();

                e.StateMachineSaga(stateMachine, _jobTypeRepository ?? new InMemorySagaRepository<JobTypeSaga>());

                _jobTypeSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ConnectEndpointConfigurationObserver(new JobServiceEndpointConfigurationObserver(_optionsSet.Configure<JobServiceOptions>(),
                cfg =>
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
