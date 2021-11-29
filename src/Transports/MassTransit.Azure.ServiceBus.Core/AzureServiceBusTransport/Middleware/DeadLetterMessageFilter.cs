namespace MassTransit.AzureServiceBusTransport.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Moves a message to the dead-letter queue, rather than the _skipped queue
    /// </summary>
    public class DeadLetterQueueFilter :
        IFilter<ReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("dead-letter-queue");
        }

        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            if (!context.TryGetPayload(out MessageLockContext lockContext))
                throw new TransportException(context.InputAddress, $"The {nameof(MessageLockContext)} was not available on the {nameof(ReceiveContext)}.");

            await lockContext.DeadLetter().ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
