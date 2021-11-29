namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Transports;


    /// <summary>
    /// In the case of an exception, the message is moved to the destination transport. If the receive had not yet been
    /// faulted, a fault is generated.
    /// </summary>
    public class ErrorTransportFilter :
        IFilter<ExceptionReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("moveFault");
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            if (!context.TryGetPayload(out IErrorTransport transport))
                throw new TransportException(context.InputAddress, $"The {nameof(IErrorTransport)} was not available on the {nameof(ReceiveContext)}.");

            await transport.Send(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
