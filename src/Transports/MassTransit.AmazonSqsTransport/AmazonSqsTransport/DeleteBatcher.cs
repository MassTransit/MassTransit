namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;


    public class DeleteBatcher :
        Batcher<DeleteMessageBatchRequestEntry>
    {
        readonly CancellationToken _cancellationToken;
        readonly IAmazonSQS _client;
        readonly string _queueUrl;

        public DeleteBatcher(IAmazonSQS client, string queueUrl, CancellationToken cancellationToken)
        {
            _client = client;
            _queueUrl = queueUrl;
            _cancellationToken = cancellationToken;
        }

        protected override int AddingEntry(DeleteMessageBatchRequestEntry entry, string entryId)
        {
            entry.Id = entryId;

            return 0;
        }

        protected override async Task SendBatch(IList<BatchEntry<DeleteMessageBatchRequestEntry>> batch)
        {
            var batchRequest = new DeleteMessageBatchRequest(_queueUrl, batch.Select(x => x.Entry).ToList());

            var response = await _client.DeleteMessageBatchAsync(batchRequest, _cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            Complete(batch, response.Successful.Select(x => x.Id));

            foreach (var error in response.Failed)
                Fail(batch, error.Id, error.Code, error.Message);
        }
    }
}
