namespace MassTransit.EventHubIntegration.Filters
{
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;


    public class EvenHubBlobContainerCreatorFilter :
        IFilter<IEventHubProcessorContext>
    {
        public async Task Send(IEventHubProcessorContext context, IPipe<IEventHubProcessorContext> next)
        {
            await context.TryCreateContainerIfNotExists(context.CancellationToken).ConfigureAwait(false);
            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
