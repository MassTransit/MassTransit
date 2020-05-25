namespace MassTransit.Turnout.Configuration
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Components.Consumers;
    using Components.StateMachines;
    using Conductor.Configuration;
    using Configurators;
    using ConsumeConfigurators;
    using GreenPipes;
    using Saga;


    public class TurnoutConfigurator<TReceiveEndpointConfigurator> :
        ITurnoutConfigurator<TReceiveEndpointConfigurator>,
        IReceiveEndpointSpecification
        where TReceiveEndpointConfigurator : IReceiveEndpointConfigurator
    {
        readonly IServiceInstanceConfigurator<TReceiveEndpointConfigurator> _instanceConfigurator;
        readonly OptionsSet _optionsSet;
        bool _endpointsConfigured;
        ISagaRepository<TurnoutJobAttemptState> _jobAttemptRepository;
        ISagaRepository<TurnoutJobState> _jobRepository;
        ISagaRepository<TurnoutJobTypeState> _jobTypeRepository;
        IReceiveEndpointConfigurator _turnoutJobAttemptStateEndpointConfigurator;
        IReceiveEndpointConfigurator _turnoutJobStateEndpointConfigurator;
        IReceiveEndpointConfigurator _turnoutStateEndpointConfigurator;

        public TurnoutConfigurator(IServiceInstanceConfigurator<TReceiveEndpointConfigurator> instanceConfigurator)
        {
            _instanceConfigurator = instanceConfigurator;

            JobRegistry = new JobRegistry();

            JobService = new JobService(JobRegistry, _instanceConfigurator.InstanceAddress);

            _optionsSet = new OptionsSet();

            instanceConfigurator.ConnectBusObserver(new JobServiceBusObserver(JobService));
            instanceConfigurator.AddSpecification(this);

            _optionsSet.Configure<TurnoutOptions>(options =>
            {
                options.JobTypeSagaEndpointName = _instanceConfigurator.EndpointNameFormatter.Saga<TurnoutJobTypeState>();
                options.JobStateSagaEndpointName = _instanceConfigurator.EndpointNameFormatter.Saga<TurnoutJobState>();
                options.JobAttemptSagaEndpointName = _instanceConfigurator.EndpointNameFormatter.Saga<TurnoutJobAttemptState>();
            });
        }

        IJobRegistry JobRegistry { get; }
        IJobService JobService { get; }

        public ISagaRepository<TurnoutJobTypeState> Repository
        {
            set => _jobTypeRepository = value;
        }

        public ISagaRepository<TurnoutJobState> JobRepository
        {
            set => _jobRepository = value;
        }

        public ISagaRepository<TurnoutJobAttemptState> JobAttemptRepository
        {
            set => _jobAttemptRepository = value;
        }

        public string TurnoutStateEndpointName
        {
            set { _optionsSet.Configure<TurnoutOptions>(options => options.JobTypeSagaEndpointName = value); }
        }

        public string TurnoutJobStateEndpointName
        {
            set { _optionsSet.Configure<TurnoutOptions>(options => options.JobStateSagaEndpointName = value); }
        }

        public string TurnoutJobAttemptStateEndpointName
        {
            set { _optionsSet.Configure<TurnoutOptions>(options => options.JobAttemptSagaEndpointName = value); }
        }

        public TimeSpan JobSlotWaitTime
        {
            set { _optionsSet.Configure<TurnoutOptions>(options => options.JobSlotWaitTime = value); }
        }

        public TimeSpan JobStatusCheckInterval
        {
            set { _optionsSet.Configure<TurnoutOptions>(options => options.JobStatusCheckInterval = value); }
        }

        public void Job<T>(Action<ITurnoutJobConfigurator<T>> configure)
            where T : class
        {
            var endpointName = _instanceConfigurator.EndpointNameFormatter.Message<T>();

            var jobConfigurator = new TurnoutJobConfigurator<T>();

            configure?.Invoke(jobConfigurator);

            BusConfigurationResult.CompileResults(jobConfigurator.Validate(), "The turnout job was not properly configured:");

            var options = _optionsSet.Configure<TurnoutJobOptions<T>>(x =>
            {
                x.ConcurrentJobLimit = jobConfigurator.ConcurrentJobLimit;
                x.JobTimeout = jobConfigurator.JobTimeout;
            });

            ConfigureTurnoutEndpoints();

            _instanceConfigurator.ReceiveEndpoint(endpointName, cfg =>
            {
                cfg.AddDependency(_instanceConfigurator);
                cfg.AddDependency(_turnoutStateEndpointConfigurator);
                cfg.AddDependency(_turnoutJobStateEndpointConfigurator);
                cfg.AddDependency(_turnoutJobAttemptStateEndpointConfigurator);

                cfg.Consumer(() => new SubmitJobConsumer<T>(options));
                cfg.Consumer(() => new StartJobConsumer<T>(JobService, jobConfigurator.JobFactory, options));
                cfg.Consumer(() => new CancelJobConsumer<T>(JobRegistry));
            });
        }

        void ConfigureTurnoutEndpoints()
        {
            if (_endpointsConfigured)
                return;

            var options = _optionsSet.Configure<TurnoutOptions>();

            _instanceConfigurator.ReceiveEndpoint(options.JobStateSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new TurnoutJobStateMachine(options);

                e.StateMachineSaga(stateMachine, _jobRepository ?? new InMemorySagaRepository<TurnoutJobState>());

                _turnoutJobStateEndpointConfigurator = e;
            });

            _instanceConfigurator.ReceiveEndpoint(options.JobAttemptSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new TurnoutJobAttemptStateMachine(options);

                e.StateMachineSaga(stateMachine, _jobAttemptRepository ?? new InMemorySagaRepository<TurnoutJobAttemptState>());

                _turnoutJobAttemptStateEndpointConfigurator = e;
            });

            _instanceConfigurator.ReceiveEndpoint(options.JobTypeSagaEndpointName, e =>
            {
                e.AddDependency(_instanceConfigurator);

                e.UseMessageRetry(r => r.Intervals(100, 200, 300, 500, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new TurnoutJobTypeStateMachine();

                e.StateMachineSaga(stateMachine, _jobTypeRepository ?? new InMemorySagaRepository<TurnoutJobTypeState>());

                _turnoutStateEndpointConfigurator = e;
            });

            _endpointsConfigured = true;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            ISpecification turnoutOptions = _optionsSet.Configure<TurnoutOptions>();

            return turnoutOptions.Validate();
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
        }
    }
}
