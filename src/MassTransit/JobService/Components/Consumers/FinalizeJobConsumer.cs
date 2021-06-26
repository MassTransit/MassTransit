namespace MassTransit.JobService.Components.Consumers
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using Metadata;


    public class FinalizeJobConsumer<TJob> :
        IConsumer<FaultJob>,
        IConsumer<CompleteJob>
        where TJob : class
    {
        readonly string _jobConsumerTypeName;
        readonly IJobService _jobService;
        readonly Guid _jobTypeId;
        readonly JobOptions<TJob> _options;

        public FinalizeJobConsumer(IJobService jobService, JobOptions<TJob> options, Guid jobTypeId, string jobConsumerTypeName)
        {
            _jobService = jobService;
            _options = options;
            _jobTypeId = jobTypeId;
            _jobConsumerTypeName = jobConsumerTypeName;
        }

        public Task Consume(ConsumeContext<CompleteJob> context)
        {
            if (context.Message.JobTypeId != _jobTypeId)
                return Task.CompletedTask;

            var completeJob = context.Message;

            var job = completeJob.GetJob<TJob>();
            if (job == null)
                throw new ArgumentNullException(nameof(completeJob.Job), $"The job could not be deserialized: {TypeMetadataCache<TJob>.ShortName}");

            return context.Publish<JobCompleted<TJob>>(context.Message);
        }

        public Task Consume(ConsumeContext<FaultJob> context)
        {
            if (context.Message.JobTypeId != _jobTypeId)
                return Task.CompletedTask;

            var faultJob = context.Message;

            var job = faultJob.GetJob<TJob>();
            if (job == null)
                throw new ArgumentNullException(nameof(faultJob.Job), $"The job could not be deserialized: {TypeMetadataCache<TJob>.ShortName}");

            using var jobContext = new ConsumeJobContext<TJob>(context, _jobService.InstanceAddress, faultJob.JobId, faultJob.AttemptId, faultJob.RetryAttempt,
                job, _options.JobTimeout);

            return jobContext.NotifyFaulted(faultJob.Duration ?? TimeSpan.Zero, _jobConsumerTypeName, new JobFaultedException(faultJob.Exceptions));
        }


        class JobFaultedException :
            MassTransitException
        {
            readonly ExceptionInfo _exceptionInfo;

            public JobFaultedException(ExceptionInfo exceptionInfo)
                : base(exceptionInfo.Message, exceptionInfo.InnerException != null ? new JobFaultedException(exceptionInfo.InnerException) : default)
            {
                _exceptionInfo = exceptionInfo;
                if (_exceptionInfo.Data != null)
                    Data = _exceptionInfo.Data.ToDictionary(x => x.Key);
            }

            public override string StackTrace => _exceptionInfo.StackTrace;
            public override string Source => _exceptionInfo.Source;
            public override IDictionary Data { get; }
        }
    }
}
