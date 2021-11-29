namespace MassTransit.JobService
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Contracts.JobService;


    public class StartJobConsumer<TJob> :
        IConsumer<StartJob>
        where TJob : class
    {
        readonly IPipe<ConsumeContext<TJob>> _jobPipe;
        readonly IJobService _jobService;
        readonly Guid _jobTypeId;
        readonly JobOptions<TJob> _options;

        public StartJobConsumer(IJobService jobService, JobOptions<TJob> options, Guid jobTypeId, IPipe<ConsumeContext<TJob>> jobPipe)
        {
            _jobService = jobService;
            _options = options;
            _jobTypeId = jobTypeId;
            _jobPipe = jobPipe;
        }

        public async Task Consume(ConsumeContext<StartJob> context)
        {
            if (context.Message.JobTypeId != _jobTypeId)
                return;

            var job = context.GetJob<TJob>() ?? throw new SerializationException($"The job could not be deserialized: {TypeCache<TJob>.ShortName}");

            await _jobService.StartJob(context, job, _jobPipe, _options.JobTimeout);
        }
    }
}
