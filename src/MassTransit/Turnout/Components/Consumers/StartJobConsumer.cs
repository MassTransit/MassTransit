namespace MassTransit.Turnout.Components.Consumers
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Contracts.Turnout;
    using Metadata;


    public class StartJobConsumer<TJob> :
        IConsumer<StartJob>
        where TJob : class
    {
        readonly IJobFactory<TJob> _jobFactory;
        readonly IJobService _jobService;
        readonly TurnoutJobOptions<TJob> _options;

        public StartJobConsumer(IJobService jobService, IJobFactory<TJob> jobFactory, TurnoutJobOptions<TJob> options)
        {
            _jobService = jobService;
            _jobFactory = jobFactory;
            _options = options;
        }

        public async Task Consume(ConsumeContext<StartJob> context)
        {
            var message = context.Message;

            var job = message.GetJob<TJob>();
            if (job == null)
                throw new ArgumentNullException(nameof(context.Message.Job), $"The job could not be deserialized: {TypeMetadataCache<TJob>.ShortName}");

            await _jobService.StartJob(context, job, _jobFactory, _options.JobTimeout);
        }
    }
}
