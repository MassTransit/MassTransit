namespace MassTransit.EventHubIntegration.Middleware
{
    using System.Threading.Tasks;


    public class EventHubBlobContainerFactoryFilter :
        IFilter<ProcessorContext>
    {
        bool _hasBeenCreated;

        public async Task Send(ProcessorContext context, IPipe<ProcessorContext> next)
        {
            if (!_hasBeenCreated)
                _hasBeenCreated = await context.CreateBlobIfNotExistsAsync(context.CancellationToken).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
