namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    namespace SagaSpecs
    {
        using System;
        using System.Threading.Tasks;
        using AzureServiceBusTransport;
        using NUnit.Framework;


        public class JobState :
            ISaga,
            InitiatedBy<CreateJob>,
            Orchestrates<StartJob>
        {
            public Guid JobId { get; set; }
            public JobStatus JobStatus { get; set; }

            public Task Consume(ConsumeContext<CreateJob> context)
            {
                JobId = context.Message.JobId;
                JobStatus = JobStatus.Created;

                context.Respond(new JobCreated { JobId = JobId });
                return Task.CompletedTask;
            }

            public Guid CorrelationId { get; set; }

            public Task Consume(ConsumeContext<StartJob> context)
            {
                if (JobStatus != JobStatus.Created)
                    throw new InvalidOperationException("The job was not created and cannot be started");

                JobStatus = JobStatus.Running;
                context.Respond(new JobStarted { JobId = JobId });

                return Task.CompletedTask;
            }
        }


        public enum JobStatus
        {
            Created,
            Running,
            Complete
        }


        public class JobCreated
        {
            public Guid JobId { get; set; }
        }


        public class JobStarted
        {
            public Guid JobId { get; set; }
        }


        public class CreateJob :
            CorrelatedBy<Guid>
        {
            public CreateJob(Guid jobId)
            {
                CorrelationId = jobId;
                JobId = jobId;
            }

            public Guid JobId { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class StartJob :
            CorrelatedBy<Guid>
        {
            public StartJob(Guid jobId)
            {
                CorrelationId = jobId;
                JobId = jobId;
            }

            public Guid JobId { get; set; }
            public Guid CorrelationId { get; set; }
        }


        [TestFixture]
        public class Using_a_message_session_as_a_saga_repository :
            AzureServiceBusTestFixture
        {
            [Test]
            public async Task The_saga_should_be_loaded()
            {
                Response<JobCreated> created = await Bus.Request<CreateJob, JobCreated>(InputQueueAddress, new CreateJob(_jobId), TestCancellationToken,
                    TestTimeout, x =>
                    {
                        x.SetSessionId(_jobId.ToString());
                    });

                Response<JobStarted> started = await Bus.Request<StartJob, JobStarted>(InputQueueAddress, new StartJob(_jobId), TestCancellationToken,
                    TestTimeout, x =>
                    {
                        x.SetSessionId(_jobId.ToString());
                    });
            }

            public Using_a_message_session_as_a_saga_repository()
                : base("saga_input_queue_session")
            {
                _repository = MessageSessionSagaRepository.Create<JobState>();
            }

            [OneTimeSetUp]
            public void Setup()
            {
                _jobId = NewId.NextGuid();
            }

            protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
            {
                configurator.RequiresSession = true;
                configurator.EnablePartitioning = true;

                configurator.Saga(_repository);
            }

            Guid _jobId;
            readonly ISagaRepository<JobState> _repository;
        }
    }
}
