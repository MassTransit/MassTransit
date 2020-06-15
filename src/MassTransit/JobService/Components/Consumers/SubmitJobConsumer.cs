namespace MassTransit.JobService.Components.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Courier;
    using MassTransit.Contracts.JobService;
    using Util;


    /// <summary>
    /// Handles the <see cref="SubmitJob{TJob}" /> command
    /// </summary>
    /// <typeparam name="TJob">The job type</typeparam>
    public class SubmitJobConsumer<TJob> :
        IConsumer<TJob>,
        IConsumer<SubmitJob<TJob>>
        where TJob : class
    {
        static int _publishedStartup;

        readonly JobOptions<TJob> _options;

        public SubmitJobConsumer(JobOptions<TJob> options)
        {
            _options = options;
        }

        public Task Consume(ConsumeContext<SubmitJob<TJob>> context)
        {
            return PublishJobSubmitted(context, context.Message.JobId, context.Message.Job, context.SentTime ?? DateTime.UtcNow);
        }

        public Task Consume(ConsumeContext<TJob> context)
        {
            var jobId = context.RequestId ?? NewId.NextGuid();

            return PublishJobSubmitted(context, jobId, context.Message, context.SentTime ?? DateTime.UtcNow);
        }

        async Task PublishJobSubmitted(ConsumeContext context, Guid jobId, TJob job, DateTime timestamp)
        {
            await PublishConcurrentJobLimit(context);

            await context.Publish<JobSubmitted>(new
            {
                JobId = jobId,
                JobMetadataCache<TJob>.JobTypeId,
                Timestamp = timestamp,
                Job = SerializerCache.GetObjectAsDictionary(job),
                _options.JobTimeout
            });

            if (context.RequestId.HasValue && context.ResponseAddress != null)
                await context.RespondAsync<JobSubmissionAccepted>(new {JobId = jobId});
        }

        Task PublishConcurrentJobLimit(IPublishEndpoint context)
        {
            if (Interlocked.CompareExchange(ref _publishedStartup, 1, 0) == 0)
            {
                return context.Publish<SetConcurrentJobLimit>(new
                {
                    JobMetadataCache<TJob>.JobTypeId,
                    _options.ConcurrentJobLimit,
                    Kind = ConcurrentLimitKind.Configured
                });
            }

            return TaskUtil.Completed;
        }
    }
}
