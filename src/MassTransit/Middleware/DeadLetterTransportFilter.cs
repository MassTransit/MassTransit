namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Transports;


    /// <summary>
    /// Moves a message received to a transport without any deserialization
    /// </summary>
    public class DeadLetterTransportFilter :
        IFilter<ReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("dead-letter");
        }

        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            if (!context.TryGetPayload(out IDeadLetterTransport transport))
                throw new TransportException(context.InputAddress, $"The {nameof(IDeadLetterTransport)} was not available on the {nameof(ReceiveContext)}.");

            await transport.Send(context, "dead-letter").ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
