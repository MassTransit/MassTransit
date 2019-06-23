namespace MassTransit.WebJobs.EventHubsIntegration.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;


    public class CollectorEventDataSendEndpointContext :
        BasePipeContext,
        EventDataSendEndpointContext
    {
        readonly CancellationToken _cancellationToken;
        readonly IAsyncCollector<EventData> _collector;

        public CollectorEventDataSendEndpointContext(string path, IAsyncCollector<EventData> collector, CancellationToken cancellationToken)
        {
            _collector = collector;
            _cancellationToken = cancellationToken;
            EntityPath = path;
        }

        public string EntityPath { get; }

        public Task Send(EventData message)
        {
            return _collector.AddAsync(message, _cancellationToken);
        }
    }
}
