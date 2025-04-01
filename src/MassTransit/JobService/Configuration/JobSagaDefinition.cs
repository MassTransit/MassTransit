namespace MassTransit.Configuration
{
    using System;
    using Contracts.JobService;
    using JobService;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Middleware;


    public class JobSagaDefinition :
        SagaDefinition<JobSaga>
    {
        readonly JobSagaOptions _options;
        readonly JobSagaSettingsConfigurator _setOptions;

        public JobSagaDefinition(IOptions<JobSagaOptions> options)
        {
            _options = options.Value;
            _setOptions = _options;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator configurator, ISagaConfigurator<JobSaga> sagaConfigurator,
            IRegistrationContext context)
        {
            configurator.UseMessageRetry(r => r.Exponential(20, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1)));

            configurator.UseMessageScope(context);

            configurator.UseInMemoryOutbox(context);

            if (_options.ConcurrentMessageLimit.HasValue)
            {
                configurator.ConcurrentMessageLimit = _options.ConcurrentMessageLimit;

                var partition = new Partitioner(_options.ConcurrentMessageLimit.Value, new Murmur3UnsafeHashGenerator());

                configurator.UsePartitioner<JobSubmitted>(partition, p => p.Message.JobId);

                configurator.UsePartitioner<JobSlotAllocated>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobSlotUnavailable>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<Fault<AllocateJobSlot>>(partition, p => p.Message.Message.JobId);

                configurator.UsePartitioner<Fault<StartJobAttempt>>(partition, p => p.Message.Message.JobId);

                configurator.UsePartitioner<JobAttemptCanceled>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobAttemptCompleted>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobAttemptFaulted>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobAttemptStarted>(partition, p => p.Message.JobId);

                configurator.UsePartitioner<GetJobState>(partition, p => p.Message.JobId);

                configurator.UsePartitioner<JobCompleted>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<CancelJob>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<RetryJob>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<RunJob>(partition, p => p.Message.JobId);

                configurator.UsePartitioner<SetJobProgress>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<SaveJobState>(partition, p => p.Message.JobId);

                configurator.UsePartitioner<JobSlotWaitElapsed>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobRetryDelayElapsed>(partition, p => p.Message.JobId);
            }

            sagaConfigurator.UseFilter(new PayloadFilter<SagaConsumeContext<JobSaga>, JobSagaSettings>(_options));

            _setOptions.JobSagaEndpointAddress = configurator.InputAddress;

            if (context.GetRequiredService<IContainerSelector>().TryGetRegistration(context, typeof(JobService), out IJobServiceRegistration registration))
                registration.AddReceiveEndpointDependency(configurator);
        }
    }
}
