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
            CancellationToken = cancellationToken;
            _context = context;
        }

        public override CancellationToken CancellationToken { get; }

        public ConnectionContext ConnectionContext => _context.ConnectionContext;

        string SendEndpointContext.EntityPath => _context.EntityPath;

        Task SendEndpointContext.Send(ServiceBusMessage message)
        {
            return _context.Send(message);
        }

        Task<long> SendEndpointContext.ScheduleSend(ServiceBusMessage message, DateTime scheduleEnqueueTimeUtc)
        {
            return _context.ScheduleSend(message, scheduleEnqueueTimeUtc);
        }

        Task SendEndpointContext.CancelScheduledSend(long sequenceNumber)
        {
            return _context.CancelScheduledSend(sequenceNumber);
        }
    }
}
