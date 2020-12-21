namespace MassTransit.EventHubIntegration.Filters
{
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;


    public class EvenHubBlobContainerCreatorFilter :
        IFilter<IEventHubProcessorContext>
    {
        bool _hasBeenCreated;

        public async Task Send(IEventHubProcessorContext context, IPipe<IEventHubProcessorContext> next)
        {
            if (!_hasBeenCreated)
            {
                _hasBeenCreated = await context.CreateBlobIfNotExistsAsync(context.CancellationToken).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
