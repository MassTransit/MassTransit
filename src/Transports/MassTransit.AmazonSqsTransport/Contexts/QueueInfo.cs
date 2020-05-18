namespace MassTransit.AmazonSqsTransport.Contexts
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
        readonly IBatcher<SendMessageBatchRequestEntry> _batchSender;
        readonly IBatcher<DeleteMessageBatchRequestEntry> _batchDeleter;

        public QueueInfo(string entityName, string url, IDictionary<string, string> attributes, IAmazonSQS client, CancellationToken cancellationToken)
        {
            Attributes = attributes;
            EntityName = entityName;
            Url = url;

            Arn = attributes.TryGetValue(QueueAttributeName.QueueArn, out var queueArn)
                ? queueArn
                : throw new ArgumentException($"The queueArn was not found: {url}", nameof(attributes));

            _batchSender = new SendBatcher(client, url, cancellationToken);
            _batchDeleter = new DeleteBatcher(client, url, cancellationToken);
        }

        public string EntityName { get; }
        public string Url { get; }
        public string Arn { get; }
        public IDictionary<string, string> Attributes { get; }

        public Task Send(SendMessageBatchRequestEntry entry, CancellationToken cancellationToken)
        {
            return _batchSender.Execute(entry, cancellationToken);
        }

        public Task Delete(string receiptHandle, CancellationToken cancellationToken)
        {
            var entry = new DeleteMessageBatchRequestEntry("", receiptHandle);

            return _batchDeleter.Execute(entry, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _batchSender.DisposeAsync().ConfigureAwait(false);
            await _batchDeleter.DisposeAsync().ConfigureAwait(false);
        }
    }
}
