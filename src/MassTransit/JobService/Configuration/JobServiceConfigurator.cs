namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Contracts.JobService;
    using JobService;
    using Middleware;


    public class JobServiceConfigurator<TReceiveEndpointConfigurator> :
        IJobServiceConfigurator,
        ISpecification
        where TReceiveEndpointConfigurator : IReceiveEndpointConfigurator
    {
        readonly IBusFactoryConfigurator<TReceiveEndpointConfigurator> _busConfigurator;
        readonly JobServiceOptions _options;
        bool _endpointsConfigured;
        ISagaRepository<JobAttemptSaga> _jobAttemptRepository;
        IReceiveEndpointConfigurator _jobAttemptSagaEndpointConfigurator;
        ISagaRepository<JobSaga> _jobRepository;
        IReceiveEndpointConfigurator _jobSagaEndpointConfigurator;
        ISagaRepository<JobTypeSaga> _jobTypeRepository;
        IReceiveEndpointConfigurator _jobTypeSagaEndpointConfigurator;

        public JobServiceConfigurator(IServiceInstanceConfigurator<TReceiveEndpointConfigurator> instanceConfigurator, JobServiceOptions options = null)
        {
            _busConfigurator = instanceConfigurator.BusConfigurator;

            _options = options != null
                ? instanceConfigurator.Options(options)
                : instanceConfigurator.Options<JobServiceOptions>();

            JobService = new JobService(instanceConfigurator, _options);

            instanceConfigurator.BusConfigurator.ConnectBusObserver(new JobServiceBusObserver(JobService));
            instanceConfigurator.AddSpecification(this);


            _options.JobService = JobService;
            _options.InstanceEndpointConfigurator = instanceConfigurator.InstanceEndpointConfigurator;

            _options.JobTypeSagaEndpointName = instanceConfigurator.EndpointNameFormatter.Saga<JobTypeSaga>();
            _options.JobStateSagaEndpointName = instanceConfigurator.EndpointNameFormatter.Saga<JobSaga>();
            _options.JobAttemptSagaEndpointName = instanceConfigurator.EndpointNameFormatter.Saga<JobAttemptSaga>();

            instanceConfigurator.ConnectEndpointConfigurationObserver(new JobServiceEndpointConfigurationObserver(_options, cfg =>
            {
                if (_jobTypeSagaEndpointConfigurator != null)
                    cfg.AddDependency(_jobTypeSagaEndpointConfigurator);
                if (_jobSagaEndpointConfigurator != null)
                    cfg.AddDependency(_jobSagaEndpointConfigurator);
                if (_jobAttemptSagaEndpointConfigurator != null)
                    cfg.AddDependency(_jobAttemptSagaEndpointConfigurator);
            }));
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

        public int SuspectJobRetryCount
        {
            set => _options.SuspectJobRetryCount = value;
        }

        public TimeSpan SuspectJobRetryDelay
        {
            set
            {
                if (value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(value), "The delay must be > TimeSpan.Zero");

                _options.SuspectJobRetryDelay = value;
            }
        }

        public int? SagaPartitionCount
        {
            set => _options.SagaPartitionCount = value;
        }

        public bool FinalizeCompleted
        {
            set => _options.FinalizeCompleted = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            ISpecification turnoutOptions = _options;

            return turnoutOptions.Validate();
        }

        public void ConfigureJobServiceEndpoints()
        {
            if (_endpointsConfigured)
                return;

            _busConfigurator.ReceiveEndpoint(_options.JobStateSagaEndpointName, e =>
            {
                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                if (_options.SagaPartitionCount.HasValue)
                {
                    var partition = new Partitioner(_options.SagaPartitionCount.Value, new Murmur3UnsafeHashGenerator());

                    e.UsePartitioner<JobSubmitted>(partition, p => p.Message.JobId);

                    e.UsePartitioner<JobSlotAllocated>(partition, p => p.Message.JobId);
                    e.UsePartitioner<JobSlotUnavailable>(partition, p => p.Message.JobId);
                    e.UsePartitioner<Fault<AllocateJobSlot>>(partition, p => p.Message.Message.JobId);

                    e.UsePartitioner<JobAttemptCreated>(partition, p => p.Message.JobId);
                    e.UsePartitioner<Fault<StartJobAttempt>>(partition, p => p.Message.Message.JobId);

                    e.UsePartitioner<JobAttemptCanceled>(partition, p => p.Message.JobId);
                    e.UsePartitioner<JobAttemptCompleted>(partition, p => p.Message.JobId);
                    e.UsePartitioner<JobAttemptFaulted>(partition, p => p.Message.JobId);
                    e.UsePartitioner<JobAttemptStarted>(partition, p => p.Message.JobId);

                    e.UsePartitioner<JobSlotWaitElapsed>(partition, p => p.Message.JobId);
                    e.UsePartitioner<JobRetryDelayElapsed>(partition, p => p.Message.JobId);
                }

                var stateMachine = new JobStateMachine(_options);
                e.StateMachineSaga(stateMachine, _jobRepository ?? new InMemorySagaRepository<JobSaga>());

                _jobSagaEndpointConfigurator = e;

                _options.JobSagaEndpointAddress = e.InputAddress;
            });

            _busConfigurator.ReceiveEndpoint(_options.JobAttemptSagaEndpointName, e =>
            {
                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                if (_options.SagaPartitionCount.HasValue)
                {
                    var partition = new Partitioner(_options.SagaPartitionCount.Value, new Murmur3UnsafeHashGenerator());

                    e.UsePartitioner<StartJobAttempt>(partition, p => p.Message.AttemptId);
                    e.UsePartitioner<Fault<StartJob>>(partition, p => p.Message.Message.AttemptId);

                    e.UsePartitioner<JobAttemptStarted>(partition, p => p.Message.AttemptId);
                    e.UsePartitioner<JobAttemptCompleted>(partition, p => p.Message.AttemptId);
                    e.UsePartitioner<JobAttemptCanceled>(partition, p => p.Message.AttemptId);
                    e.UsePartitioner<JobAttemptFaulted>(partition, p => p.Message.AttemptId);

                    e.UsePartitioner<JobAttemptStatus>(partition, p => p.Message.AttemptId);
                    e.UsePartitioner<JobStatusCheckRequested>(partition, p => p.Message.AttemptId);
                }

                var stateMachine = new JobAttemptStateMachine(_options);
                e.StateMachineSaga(stateMachine, _jobAttemptRepository ?? new InMemorySagaRepository<JobAttemptSaga>());

                _jobAttemptSagaEndpointConfigurator = e;

                _options.JobAttemptSagaEndpointAddress = e.InputAddress;
            });

            _busConfigurator.ReceiveEndpoint(_options.JobTypeSagaEndpointName, e =>
            {
                e.UseMessageRetry(r => r.Intervals(100, 200, 300, 500, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                if (_options.SagaPartitionCount.HasValue)
                {
                    var partition = new Partitioner(_options.SagaPartitionCount.Value, new Murmur3UnsafeHashGenerator());

                    e.UsePartitioner<AllocateJobSlot>(partition, p => p.Message.JobTypeId);
                    e.UsePartitioner<JobSlotReleased>(partition, p => p.Message.JobTypeId);
                    e.UsePartitioner<SetConcurrentJobLimit>(partition, p => p.Message.JobTypeId);
                }

                var stateMachine = new JobTypeStateMachine(_options);

                e.StateMachineSaga(stateMachine, _jobTypeRepository ?? new InMemorySagaRepository<JobTypeSaga>());

                _jobTypeSagaEndpointConfigurator = e;

                _options.JobTypeSagaEndpointAddress = e.InputAddress;
            });

            _endpointsConfigured = true;
        }
    }
}
