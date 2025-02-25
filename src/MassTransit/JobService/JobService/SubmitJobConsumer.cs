#nullable enable
namespace MassTransit.JobService
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using Messages;
    using Scheduling;


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
            if (context.Message.Schedule != null)
            {
                if (string.IsNullOrWhiteSpace(context.Message.Schedule.CronExpression) && !context.Message.Schedule.Start.HasValue)
                    throw new RecurringJobException("A valid cron expression or start date is required");

                if (!string.IsNullOrWhiteSpace(context.Message.Schedule.CronExpression))
                    CronExpression.ValidateExpression(context.Message.Schedule.CronExpression!);
            }

            return PublishJobSubmitted(context, context.Message.JobId, context.Message.Job, context.SentTime ?? DateTime.UtcNow, context.Message.Schedule,
                context.Message.Properties);
        }

        public Task Consume(ConsumeContext<TJob> context)
        {
            var jobId = context.RequestId ?? NewId.NextGuid();

            return PublishJobSubmitted(context, jobId, context.Message, context.SentTime ?? DateTime.UtcNow, null, null);
        }

        async Task PublishJobSubmitted(ConsumeContext context, Guid jobId, TJob job, DateTime timestamp, RecurringJobSchedule? schedule,
            Dictionary<string, object>? jobProperties)
        {
            await context.Publish<JobSubmitted>(new JobSubmittedEvent
            {
                JobId = jobId,
                JobTypeId = _jobTypeId,
                Timestamp = timestamp,
                Job = context.ToDictionary(job),
                JobProperties = jobProperties,
                JobTimeout = _options.JobTimeout,
                Schedule = schedule
            });

            if (context.RequestId.HasValue && context.ResponseAddress != null)
                await context.RespondAsync<JobSubmissionAccepted>(new JobSubmissionAcceptedResponse { JobId = jobId });
        }
    }
}
