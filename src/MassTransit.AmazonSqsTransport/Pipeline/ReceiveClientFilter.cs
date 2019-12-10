namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Creates a receiving model context using the connection
    /// </summary>
    public class ReceiveClientFilter :
        IFilter<ConnectionContext>
    {
        readonly IPipe<ClientContext> _pipe;

        public ReceiveClientFilter(IPipe<ClientContext> pipe)
        {
            _pipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("receiveModel");

            _pipe.Probe(scope);
        }

        async Task IFilter<ConnectionContext>.Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            var clientContext = context.CreateClientContext(context.CancellationToken);

            try
            {
                await _pipe.Send(clientContext).ConfigureAwait(false);
            }
            finally
            {
                await clientContext.DisposeAsync(CancellationToken.None).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
