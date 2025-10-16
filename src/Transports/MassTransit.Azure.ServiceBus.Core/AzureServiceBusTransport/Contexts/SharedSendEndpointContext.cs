namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using MassTransit.Middleware;


    public class SharedSendEndpointContext :
        ProxyPipeContext,
        SendEndpointContext
    {
        readonly SendEndpointContext _context;

        public SharedSendEndpointContext(SendEndpointContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;

            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public ConnectionContext ConnectionContext => _context.ConnectionContext;

        public string EntityPath => _context.EntityPath;

        public async Task Send(ServiceBusMessage message, CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            await _context.Send(message, tokenSource.Token).ConfigureAwait(false);
        }

        public async Task<long> ScheduleSend(ServiceBusMessage message, DateTime scheduleEnqueueTimeUtc, CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return await _context.ScheduleSend(message, scheduleEnqueueTimeUtc, tokenSource.Token).ConfigureAwait(false);
        }

        public async Task CancelScheduledSend(long sequenceNumber, CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            await _context.CancelScheduledSend(sequenceNumber, tokenSource.Token).ConfigureAwait(false);
        }
    }
}
