namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;


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

        Task SendEndpointContext.Send(Message message)
        {
            return _context.Send(message);
        }

        Task<long> SendEndpointContext.ScheduleSend(Message message, DateTime scheduleEnqueueTimeUtc)
        {
            return _context.ScheduleSend(message, scheduleEnqueueTimeUtc);
        }

        Task SendEndpointContext.CancelScheduledSend(long sequenceNumber)
        {
            return _context.CancelScheduledSend(sequenceNumber);
        }
    }
}
