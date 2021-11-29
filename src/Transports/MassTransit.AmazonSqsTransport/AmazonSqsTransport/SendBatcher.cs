namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;


    public class SendBatcher :
        Batcher<SendMessageBatchRequestEntry>
    {
        readonly CancellationToken _cancellationToken;
        readonly IAmazonSQS _client;
        readonly string _queueUrl;

        public SendBatcher(IAmazonSQS client, string queueUrl, CancellationToken cancellationToken)
        {
            _client = client;
            _queueUrl = queueUrl;
            _cancellationToken = cancellationToken;
        }

        protected override int AddingEntry(SendMessageBatchRequestEntry entry, string entryId)
        {
            entry.Id = entryId;

            return entry.MessageBody.Length
                + entry.MessageAttributes.Where(x => x.Value.DataType == "String").Sum(x => x.Key.Length + x.Value.StringValue.Length);
        }

        protected override async Task SendBatch(IList<BatchEntry<SendMessageBatchRequestEntry>> batch)
        {
            var batchRequest = new SendMessageBatchRequest(_queueUrl, batch.Select(x => x.Entry).ToList());

            var response = await _client.SendMessageBatchAsync(batchRequest, _cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            Complete(batch, response.Successful.Select(x => x.Id));

            foreach (var error in response.Failed)
                Fail(batch, error.Id, error.Code, error.Message);
        }
    }
}
