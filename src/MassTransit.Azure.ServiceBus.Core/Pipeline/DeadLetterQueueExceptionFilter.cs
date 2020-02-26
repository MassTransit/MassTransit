namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Moves a faulted message to the dead-letter queue, rather than the _skipped queue
    /// </summary>
    public class DeadLetterQueueExceptionFilter :
        IFilter<ExceptionReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("dead-letter-queue");
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            if (!context.TryGetPayload(out MessageLockContext lockContext))
                throw new TransportException(context.InputAddress, $"The {nameof(MessageLockContext)} was not available on the {nameof(ReceiveContext)}.");

            await lockContext.DeadLetter(context.Exception).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
