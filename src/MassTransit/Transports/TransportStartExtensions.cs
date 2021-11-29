namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;


    public static class TransportStartExtensions
    {
        public static async Task OnTransportStartup<T>(this ReceiveEndpointContext context, ISupervisor<T> supervisor, CancellationToken cancellationToken)
            where T : class, PipeContext
        {
            // Nothing connected to the pipe, so signal early we are available
            if (!context.ReceivePipe.Connected.IsCompleted)
            {
                var pipe = new WaitForConnectionPipe<T>(context, cancellationToken);

                await supervisor.Send(pipe, cancellationToken).ConfigureAwait(false);
            }

            await context.Dependencies.OrCanceled(cancellationToken).ConfigureAwait(false);
        }


        class WaitForConnectionPipe<T> :
            IPipe<T>
            where T : class, PipeContext
        {
            readonly ReceiveEndpointContext _context;
            readonly CancellationToken _stopping;

            public WaitForConnectionPipe(ReceiveEndpointContext context, CancellationToken stopping)
            {
                _context = context;
                _stopping = stopping;
            }

            public async Task Send(T context)
            {
                await _context.TransportObservers.NotifyReady(_context.InputAddress, false).ConfigureAwait(false);

                try
                {
                    await _context.ReceivePipe.Connected.OrCanceled(_stopping).ConfigureAwait(false);
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == _stopping)
                {
                    await _context.TransportObservers.NotifyCompleted(_context.InputAddress, Metrics.None).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class Metrics :
            DeliveryMetrics
        {
            public static readonly DeliveryMetrics None = new Metrics();

            public long DeliveryCount => 0;
            public int ConcurrentDeliveryCount => 0;
        }
    }
}
