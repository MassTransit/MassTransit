namespace MassTransit.Turnout.Components.Consumers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Courier;
    using MassTransit.Contracts.Turnout;
    using Metadata;


    /// <summary>
    /// Handles the <see cref="SubmitJob{TJob}"/> command
    /// </summary>
    /// <typeparam name="TJob">The job type</typeparam>
    public class SubmitJobConsumer<TJob> :
        IConsumer<TJob>,
        IConsumer<SubmitJob<TJob>>
        where TJob : class
    {
        static readonly Lazy<Guid> _jobTypeId = new Lazy<Guid>(() => GenerateJobTypeId());
        static int _publishedStartup;

        readonly TurnoutJobOptions<TJob> _options;

        public SubmitJobConsumer(TurnoutJobOptions<TJob> options)
        {
            _options = options;
        }

        public Task Consume(ConsumeContext<SubmitJob<TJob>> context)
        {
            return PublishJobSubmitted(context, context.Message.JobId, context.Message.Job, context.SentTime ?? DateTime.UtcNow);
        }

        public Task Consume(ConsumeContext<TJob> context)
        {
            return PublishJobSubmitted(context, NewId.NextGuid(), context.Message, context.SentTime ?? DateTime.UtcNow);
        }

        async Task PublishJobSubmitted(ConsumeContext context, Guid jobId, TJob job, DateTime timestamp)
        {
            if (Interlocked.CompareExchange(ref _publishedStartup, 1, 0) == 0)
            {
                await context.Publish<SetConcurrentJobLimit>(new
                {
                    JobTypeId = _jobTypeId.Value,
                    _options.ConcurrentJobLimit,
                    Kind = JobLimitKind.Configured
                });
            }

            await context.Publish<JobSubmitted>(new
            {
                JobId = jobId,
                JobTypeId = _jobTypeId.Value,
                Timestamp = timestamp,
                Job = SerializerCache.GetObjectAsDictionary(job),
                _options.JobTimeout
            });

            if (context.RequestId.HasValue && context.ResponseAddress != null)
                await context.RespondAsync<JobSubmissionAccepted>(new {JobId = jobId});
        }

        static Guid GenerateJobTypeId()
        {
            var shortName = TypeMetadataCache<TJob>.ShortName;

            using var hasher = MD5.Create();

            byte[] data = hasher.ComputeHash(Encoding.UTF8.GetBytes(shortName));

            return new Guid(data);
        }
    }
}
