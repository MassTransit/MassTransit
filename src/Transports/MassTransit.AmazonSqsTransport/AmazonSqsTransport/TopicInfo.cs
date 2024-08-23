namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;


    public class TopicInfo :
        IAsyncDisposable
    {
        readonly Lazy<IBatcher<PublishBatchRequestEntry>> _batchPublisher;
        bool _disposed;

        public TopicInfo(string entityName, string arn, IAmazonSimpleNotificationService client, CancellationToken cancellationToken, bool existing)
        {
            EntityName = entityName;
            Arn = arn;
            Existing = existing;

            _batchPublisher = new Lazy<IBatcher<PublishBatchRequestEntry>>(() => new PublishBatcher(client, arn, cancellationToken));
        }

        public string EntityName { get; }
        public string Arn { get; }
        public bool Existing { get; }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_batchPublisher.IsValueCreated)
                await _batchPublisher.Value.DisposeAsync().ConfigureAwait(false);
        }

        public Task Publish(PublishBatchRequestEntry entry, CancellationToken cancellationToken)
        {
            return _batchPublisher.Value.Execute(entry, cancellationToken);
        }
    }
}
