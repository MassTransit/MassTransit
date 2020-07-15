namespace MassTransit.JobService.Configuration
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Components.StateMachines;
    using Conductor;
    using GreenPipes;
    using MassTransit.Contracts.JobService;
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
                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobStateMachine(_options);

                e.StateMachineSaga(stateMachine, _jobRepository ?? new InMemorySagaRepository<JobSaga>(), x =>
                {
                    if (_options.SagaPartitionCount.HasValue)
                    {
                        var partition = e.CreatePartitioner(_options.SagaPartitionCount.Value);

                        x.Message<JobSlotAllocated>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<JobSlotUnavailable>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<Fault<AllocateJobSlot>>(m => m.UsePartitioner(partition, p => p.Message.Message.JobId.ToByteArray()));
                        x.Message<JobAttemptCreated>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<Fault<StartJobAttempt>>(m => m.UsePartitioner(partition, p => p.Message.Message.JobId.ToByteArray()));
                        x.Message<JobSubmitted>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<JobAttemptStarted>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<JobAttemptCompleted>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<JobAttemptCanceled>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<JobAttemptFaulted>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<JobSlotWaitElapsed>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                        x.Message<JobRetryDelayElapsed>(m => m.UsePartitioner(partition, p => p.Message.JobId.ToByteArray()));
                    }
                });


                _jobSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ReceiveEndpoint(_options.JobAttemptSagaEndpointName, e =>
            {
                e.UseMessageRetry(r => r.Intervals(100, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobAttemptStateMachine(_options);

                e.StateMachineSaga(stateMachine, _jobAttemptRepository ?? new InMemorySagaRepository<JobAttemptSaga>(), x =>
                {
                    if (_options.SagaPartitionCount.HasValue)
                    {
                        var partition = e.CreatePartitioner(_options.SagaPartitionCount.Value);

                        x.Message<StartJobAttempt>(m => m.UsePartitioner(partition, p => p.Message.AttemptId.ToByteArray()));
                        x.Message<Fault<StartJob>>(m => m.UsePartitioner(partition, p => p.Message.Message.AttemptId.ToByteArray()));
                        x.Message<JobAttemptStarted>(m => m.UsePartitioner(partition, p => p.Message.AttemptId.ToByteArray()));
                        x.Message<JobAttemptCompleted>(m => m.UsePartitioner(partition, p => p.Message.AttemptId.ToByteArray()));
                        x.Message<JobAttemptCanceled>(m => m.UsePartitioner(partition, p => p.Message.AttemptId.ToByteArray()));
                        x.Message<JobAttemptFaulted>(m => m.UsePartitioner(partition, p => p.Message.AttemptId.ToByteArray()));
                        x.Message<JobStatusCheckRequested>(m => m.UsePartitioner(partition, p => p.Message.AttemptId.ToByteArray()));
                    }
                });

                _jobAttemptSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ReceiveEndpoint(_options.JobTypeSagaEndpointName, e =>
            {
                e.UseMessageRetry(r => r.Intervals(100, 200, 300, 500, 1000, 2000, 5000));
                e.UseInMemoryOutbox();

                var stateMachine = new JobTypeStateMachine();

                e.StateMachineSaga(stateMachine, _jobTypeRepository ?? new InMemorySagaRepository<JobTypeSaga>(), x =>
                {
                    if (_options.SagaPartitionCount.HasValue)
                    {
                        var partition = e.CreatePartitioner(_options.SagaPartitionCount.Value);

                        x.Message<AllocateJobSlot>(m => m.UsePartitioner(partition, p => p.Message.JobTypeId.ToByteArray()));
                        x.Message<JobSlotReleased>(m => m.UsePartitioner(partition, p => p.Message.JobTypeId.ToByteArray()));
                        x.Message<SetConcurrentJobLimit>(m => m.UsePartitioner(partition, p => p.Message.JobTypeId.ToByteArray()));
                    }
                });

                _jobTypeSagaEndpointConfigurator = e;
            });

            _instanceConfigurator.ConnectEndpointConfigurationObserver(new JobServiceEndpointConfigurationObserver(_options, cfg =>
            {
                cfg.AddDependency(_jobTypeSagaEndpointConfigurator);
                cfg.AddDependency(_jobSagaEndpointConfigurator);
                cfg.AddDependency(_jobAttemptSagaEndpointConfigurator);
            }));

            _endpointsConfigured = true;
        }
    }
}
