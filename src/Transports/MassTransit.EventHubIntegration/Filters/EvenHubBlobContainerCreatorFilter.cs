namespace MassTransit.EventHubIntegration.Filters
{
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs;
    using Context;
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
                try
                {
                    await context.BlobContainerClient.CreateIfNotExistsAsync(cancellationToken: context.CancellationToken).ConfigureAwait(false);
                    _hasBeenCreated = true;
                }
                catch (RequestFailedException exception)
                {
                    LogContext.Warning?.Log(exception, "Azure Blob Container does not exist: {Address}", context.BlobContainerClient.Uri);
                }
            }

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
