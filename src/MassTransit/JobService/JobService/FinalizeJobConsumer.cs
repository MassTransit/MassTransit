namespace MassTransit.JobService
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using Messages;


    public class FinalizeJobConsumer<TJob> :
        IConsumer<FaultJob>,
        IConsumer<CompleteJob>
        where TJob : class
    {
        readonly string _jobConsumerTypeName;
        readonly Guid _jobTypeId;

        public FinalizeJobConsumer(Guid jobTypeId, string jobConsumerTypeName)
        {
            _jobTypeId = jobTypeId;
            _jobConsumerTypeName = jobConsumerTypeName;
        }

        public Task Consume(ConsumeContext<CompleteJob> context)
        {
            if (context.Message.JobTypeId != _jobTypeId)
                return Task.CompletedTask;

            var job = context.GetJob<TJob>() ?? throw new SerializationException($"The job could not be deserialized: {TypeCache<TJob>.ShortName}");

            return context.Publish<JobCompleted<TJob>>(new JobCompletedEvent<TJob>
            {
                JobId = context.Message.JobId,
                Timestamp = context.Message.Timestamp,
                Duration = context.Message.Duration,
                Job = job,
                Result = context.Message.Result,
            });
        }

        public Task Consume(ConsumeContext<FaultJob> context)
        {
            var message = context.Message;
            if (message.JobTypeId != _jobTypeId)
                return Task.CompletedTask;

            var job = context.GetJob<TJob>() ?? throw new SerializationException($"The job could not be deserialized: {TypeCache<TJob>.ShortName}");

            var jobContext = new FaultJobContext<TJob>(context, job);

            return jobContext.NotifyFaulted(message.Duration ?? TimeSpan.Zero, _jobConsumerTypeName, new ExceptionInfoException(message.Exceptions));
        }
    }
}
