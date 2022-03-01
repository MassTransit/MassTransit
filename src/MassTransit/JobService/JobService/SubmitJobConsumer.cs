namespace MassTransit.JobService
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;


    /// <summary>
    /// Handles the <see cref="SubmitJob{TJob}" /> command
    /// </summary>
    /// <typeparam name="TJob">The job type</typeparam>
    public class SubmitJobConsumer<TJob> :
        IConsumer<TJob>,
        IConsumer<SubmitJob<TJob>>
        where TJob : class
    {
        readonly Guid _jobTypeId;
        readonly JobOptions<TJob> _options;

        public SubmitJobConsumer(JobOptions<TJob> options, Guid jobTypeId)
        {
            _options = options;
            _jobTypeId = jobTypeId;
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
            await context.Publish<JobSubmitted>(new
            {
                JobId = jobId,
                JobTypeId = _jobTypeId,
                Timestamp = timestamp,
                Job = context.ToDictionary(job),
                _options.JobTimeout
            });

            if (context.RequestId.HasValue && context.ResponseAddress != null)
                await context.RespondAsync<JobSubmissionAccepted>(new { JobId = jobId });
        }
    }
}
