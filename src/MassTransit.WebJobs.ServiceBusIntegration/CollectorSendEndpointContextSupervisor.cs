namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.ServiceBus.Core.Contexts;
    using Azure.ServiceBus.Core.Pipeline;
    using GreenPipes;
    using GreenPipes.Agents;


    public class CollectorSendEndpointContextSupervisor :
        Supervisor,
        ISendEndpointContextSupervisor
    {
        readonly SendEndpointContext _context;

        public CollectorSendEndpointContextSupervisor(SendEndpointContext context)
        {
            _context = context;
        }

        public Task Send(IPipe<SendEndpointContext> pipe, CancellationToken cancellationToken = default)
        {
            var sharedContext = new SharedSendEndpointContext(_context, cancellationToken);

            return pipe.Send(sharedContext);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("binderSource");
        }
    }
}
