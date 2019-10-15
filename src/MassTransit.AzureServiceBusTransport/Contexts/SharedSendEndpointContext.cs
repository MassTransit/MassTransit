namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


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

        string SendEndpointContext.EntityPath => _context.EntityPath;

        Task SendEndpointContext.Send(BrokeredMessage message)
        {
            return _context.Send(message);
        }

        Task<long> SendEndpointContext.ScheduleSend(BrokeredMessage message, DateTime scheduleEnqueueTimeUtc)
        {
            return _context.ScheduleSend(message, scheduleEnqueueTimeUtc);
        }

        Task SendEndpointContext.CancelScheduledSend(long sequenceNumber)
        {
            return _context.CancelScheduledSend(sequenceNumber);
        }
    }
}
