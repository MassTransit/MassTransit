namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;


    public static class TransportStartExtensions
    {
        public static async Task OnTransportStartup<T>(this ReceiveEndpointContext context, ISupervisor<T> supervisor, CancellationToken cancellationToken)
            where T : class, PipeContext
        {
            // nothing is connected to the pipe, so signal early that we are available
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
                await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(_context.InputAddress, false)).ConfigureAwait(false);

                try
                {
                    await _context.ReceivePipe.Connected.OrCanceled(_stopping).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
