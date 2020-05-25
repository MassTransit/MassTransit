namespace MassTransit.Turnout.Components.Consumers
{
    using System.Threading.Tasks;
    using MassTransit.Contracts.Turnout;


    public class CancelJobConsumer<TJob> :
        IConsumer<CancelJob>
        where TJob : class
    {
        readonly IJobRegistry _jobRegistry;

        public CancelJobConsumer(IJobRegistry jobRegistry)
        {
            _jobRegistry = jobRegistry;
        }

        public async Task Consume(ConsumeContext<CancelJob> context)
        {
            if (_jobRegistry.TryGetJob(context.Message.JobId, out var handle))
            {
                await handle.Cancel();
            }
        }
    }
}
