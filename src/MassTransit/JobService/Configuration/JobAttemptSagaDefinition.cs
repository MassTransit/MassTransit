namespace MassTransit.Configuration
{
    using System;
    using Contracts.JobService;
    using JobService;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Middleware;


    public class JobAttemptSagaDefinition :
        SagaDefinition<JobAttemptSaga>
    {
        readonly JobSagaOptions _options;
        readonly JobSagaSettingsConfigurator _setOptions;

        public JobAttemptSagaDefinition(IOptions<JobSagaOptions> options)
        {
            _options = options.Value;
            _setOptions = _options;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator configurator, ISagaConfigurator<JobAttemptSaga> sagaConfigurator,
            IRegistrationContext context)
        {
            configurator.UseMessageRetry(r => r.Exponential(20, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1)));

            configurator.UseMessageScope(context);

            configurator.UseInMemoryOutbox(context);

            if (_options.ConcurrentMessageLimit.HasValue)
            {
                configurator.ConcurrentMessageLimit = _options.ConcurrentMessageLimit;

                var partition = new Partitioner(_options.ConcurrentMessageLimit.Value, new Murmur3UnsafeHashGenerator());

                configurator.UsePartitioner<StartJobAttempt>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<FinalizeJobAttempt>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<CancelJobAttempt>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<Fault<StartJob>>(partition, p => p.Message.Message.JobId);

                configurator.UsePartitioner<JobAttemptStarted>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobAttemptCompleted>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobAttemptCanceled>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobAttemptFaulted>(partition, p => p.Message.JobId);

                configurator.UsePartitioner<JobAttemptStatus>(partition, p => p.Message.JobId);
                configurator.UsePartitioner<JobStatusCheckRequested>(partition, p => p.Message.JobId ?? p.Message.AttemptId);
            }

            sagaConfigurator.UseFilter(new PayloadFilter<SagaConsumeContext<JobAttemptSaga>, JobSagaSettings>(_options));

            _setOptions.JobAttemptSagaEndpointAddress = configurator.InputAddress;

            if (context.GetRequiredService<IContainerSelector>().TryGetRegistration(context, typeof(JobService), out IJobServiceRegistration registration))
                registration.AddReceiveEndpointDependency(configurator);
        }
    }
}
