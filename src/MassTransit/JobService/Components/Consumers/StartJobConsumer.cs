namespace MassTransit.JobService.Components.Consumers
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Contracts.JobService;
    using Metadata;


    public class StartJobConsumer<TJob> :
        IConsumer<StartJob>
        where TJob : class
    {
        readonly IJobService _jobService;
        readonly JobOptions<TJob> _options;
        readonly IPipe<ConsumeContext<TJob>> _jobPipe;

        public StartJobConsumer(IJobService jobService, JobOptions<TJob> options, IPipe<ConsumeContext<TJob>> jobPipe)
        {
            _jobService = jobService;
            _options = options;
            _jobPipe = jobPipe;
        }

        public async Task Consume(ConsumeContext<StartJob> context)
        {
            var job = context.Message.GetJob<TJob>();
            if (job == null)
                throw new ArgumentNullException(nameof(context.Message.Job), $"The job could not be deserialized: {TypeMetadataCache<TJob>.ShortName}");

            await _jobService.StartJob(context, job, _jobPipe, _options.JobTimeout);
        }
    }
}
