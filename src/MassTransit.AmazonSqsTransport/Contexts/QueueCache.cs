namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Context;
    using GreenPipes.Caching;
    using Topology.Entities;


    public class QueueCache
    {
        readonly IAmazonSQS _client;
        readonly ICache<QueueInfo> _cache;
        readonly IIndex<string, QueueInfo> _nameIndex;

        public QueueCache(IAmazonSQS client)
        {
            _client = client;
            _cache = new GreenCache<QueueInfo>(ClientContextCacheDefaults.GetCacheSettings());
            _nameIndex = _cache.AddIndex("entityName", x => x.EntityName);
        }

        public Task<QueueInfo> Get(Queue queue, CancellationToken cancellationToken)
        {
            return _nameIndex.Get(queue.EntityName, key => CreateMissingQueue(queue, cancellationToken));
        }

        public Task<QueueInfo> GetByName(string entityName)
        {
            return _nameIndex.Get(entityName);
        }

        public void RemoveByName(string entityName)
        {
            _nameIndex.Remove(entityName);
        }

        async Task<QueueInfo> CreateMissingQueue(Queue queue, CancellationToken cancellationToken)
        {
            var attributes = queue.QueueAttributes.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (queue.EntityName.EndsWith(".fifo", true, CultureInfo.InvariantCulture) && !attributes.ContainsKey(QueueAttributeName.FifoQueue))
            {
                LogContext.Warning?.Log("Using '.fifo' suffix without 'FifoQueue' attribute might cause unexpected behavior.");

                attributes[QueueAttributeName.FifoQueue] = "true";
            }

            var request = new CreateQueueRequest(queue.EntityName)
            {
                Attributes = attributes,
                Tags = queue.QueueTags.ToDictionary(x => x.Key, x => x.Value)
            };

            var response = await _client.CreateQueueAsync(request, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            var queueUrl = response.QueueUrl;

            var queueAttributes = await _client.GetAttributesAsync(queueUrl).ConfigureAwait(false);

            return new QueueInfo(queue.EntityName, queueUrl, queueAttributes);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
