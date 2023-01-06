namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Logging;
    using MassTransit.Configuration;
    using MassTransit.Middleware;


    public class EventHubProcessorContext :
        BasePipeContext,
        ProcessorContext
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly EventProcessorClient _client;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;

        public EventHubProcessorContext(IHostConfiguration hostConfiguration,
            EventProcessorClient client, Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _hostConfiguration = hostConfiguration;
            _client = client;
            _partitionInitializingHandler = partitionInitializingHandler;
            _partitionClosingHandler = partitionClosingHandler;
        }

        public ILogContext LogContext => _hostConfiguration.ReceiveLogContext;

        public EventProcessorClient GetClient(ProcessorClientBuilderContext context)
        {
            _client.PartitionInitializingAsync += async args =>
            {
                await context.OnPartitionInitializing(args).ConfigureAwait(false);
                if (_partitionInitializingHandler != null)
                    await _partitionInitializingHandler(args).ConfigureAwait(false);
            };
            _client.PartitionClosingAsync += async args =>
            {
                if (_partitionClosingHandler != null)
                    await _partitionClosingHandler(args).ConfigureAwait(false);

                await context.OnPartitionClosing(args).ConfigureAwait(false);
            };
            return _client;
        }
    }
}
