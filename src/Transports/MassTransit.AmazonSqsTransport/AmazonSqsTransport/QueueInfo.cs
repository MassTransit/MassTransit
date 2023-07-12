namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;


    public class QueueInfo :
        IAsyncDisposable
    {
        readonly Lazy<IBatcher<DeleteMessageBatchRequestEntry>> _batchDeleter;
        readonly Lazy<IBatcher<SendMessageBatchRequestEntry>> _batchSender;
        bool _disposed;

        public QueueInfo(string entityName, string url, IDictionary<string, string> attributes, IAmazonSQS client, CancellationToken cancellationToken)
        {
            Attributes = attributes;
            EntityName = entityName;
            Url = url;

            Arn = attributes.TryGetValue(QueueAttributeName.QueueArn, out var queueArn)
                ? queueArn
                : throw new ArgumentException($"The queueArn was not found: {url}", nameof(attributes));

            _batchSender = new Lazy<IBatcher<SendMessageBatchRequestEntry>>(() => new SendBatcher(client, url, cancellationToken));
            _batchDeleter = new Lazy<IBatcher<DeleteMessageBatchRequestEntry>>(() => new DeleteBatcher(client, url, cancellationToken));

            SubscriptionArns = new List<string>();
        }

        public string EntityName { get; }
        public string Url { get; }
        public string Arn { get; }
        public IDictionary<string, string> Attributes { get; }
        public IList<string> SubscriptionArns { get; }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_batchSender.IsValueCreated)
                await _batchSender.Value.DisposeAsync().ConfigureAwait(false);
            if (_batchDeleter.IsValueCreated)
                await _batchDeleter.Value.DisposeAsync().ConfigureAwait(false);
        }

        public Task Send(SendMessageBatchRequestEntry entry, CancellationToken cancellationToken)
        {
            return _batchSender.Value.Execute(entry, cancellationToken);
        }

        public Task Delete(string receiptHandle, CancellationToken cancellationToken)
        {
            var entry = new DeleteMessageBatchRequestEntry("", receiptHandle);

            return _batchDeleter.Value.Execute(entry, cancellationToken);
        }
    }
}
