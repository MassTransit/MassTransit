namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;


    public class PublishBatcher :
        Batcher<PublishBatchRequestEntry>
    {
        readonly CancellationToken _cancellationToken;
        readonly IAmazonSimpleNotificationService _client;
        readonly string _topicArn;

        public PublishBatcher(IAmazonSimpleNotificationService client, string topicArn, CancellationToken cancellationToken)
            : base(PublishBatchSettings.GetBatchSettings())
        {
            _client = client;
            _topicArn = topicArn;
            _cancellationToken = cancellationToken;
        }

        protected override int CalculateEntryLength(PublishBatchRequestEntry entry, string entryId)
        {
            entry.Id = entryId;

            return entry.Message.Length
                + entry.MessageAttributes.Where(x => x.Value.DataType == "String").Sum(x => x.Key.Length + x.Value.StringValue.Length);
        }

        protected override async Task SendBatch(IList<BatchEntry<PublishBatchRequestEntry>> batch)
        {
            var batchRequest = new PublishBatchRequest
            {
                TopicArn = _topicArn,
                PublishBatchRequestEntries = batch.Select(x => x.Entry).ToList()
            };

            var response = await _client.PublishBatchAsync(batchRequest, _cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            Complete(batch, response.Successful.Select(x => x.Id));

            foreach (var error in response.Failed)
                Fail(batch, error.Id, error.Code, error.Message);
        }
    }
}
