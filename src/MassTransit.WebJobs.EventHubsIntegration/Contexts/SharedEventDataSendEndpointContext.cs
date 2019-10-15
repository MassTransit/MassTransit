namespace MassTransit.WebJobs.EventHubsIntegration.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.EventHubs;


    public class SharedEventDataSendEndpointContext :
        ProxyPipeContext,
        EventDataSendEndpointContext
    {
        readonly EventDataSendEndpointContext _context;

        public SharedEventDataSendEndpointContext(EventDataSendEndpointContext context, CancellationToken cancellationToken)
            : base(context)
        {
            CancellationToken = cancellationToken;
            _context = context;
        }

        public override CancellationToken CancellationToken { get; }

        string EventDataSendEndpointContext.EntityPath => _context.EntityPath;

        Task EventDataSendEndpointContext.Send(EventData message)
        {
            return _context.Send(message);
        }
    }
}
