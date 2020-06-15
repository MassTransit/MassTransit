namespace MassTransit.JobService.Components.Consumers
{
    using System.Threading.Tasks;
    using MassTransit.Contracts.JobService;


    public class CancelJobConsumer<TJob> :
        IConsumer<CancelJob>
        where TJob : class
    {
        readonly IJobService _jobService;

        public CancelJobConsumer(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task Consume(ConsumeContext<CancelJob> context)
        {
            if (_jobService.TryGetJob(context.Message.JobId, out var handle))
                await handle.Cancel();
        }
    }
}
