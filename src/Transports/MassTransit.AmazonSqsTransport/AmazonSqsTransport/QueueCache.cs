namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Internals.Caching;
    using Topology;


    public class QueueCache :
        IAsyncDisposable
    {
        static readonly List<string> AllAttributes = new List<string> { QueueAttributeName.All };

        readonly ICache<string, QueueInfo, ITimeToLiveCacheValue<QueueInfo>> _cache;
        readonly CancellationToken _cancellationToken;
        readonly IAmazonSQS _client;
        readonly IDictionary<string, QueueInfo> _durableQueues;

        public QueueCache(IAmazonSQS client, CancellationToken cancellationToken)
        {
            _client = client;
            _cancellationToken = cancellationToken;

            _cache = ClientContextCacheDefaults.CreateCache<string, QueueInfo>();

            _durableQueues = new Dictionary<string, QueueInfo>();
        }

        public async ValueTask DisposeAsync()
        {
            QueueInfo[] queueInfos;
            lock (_durableQueues)
            {
                queueInfos = _durableQueues.Values.ToArray();

                _durableQueues.Clear();
            }

            foreach (var queueInfo in queueInfos)
                await queueInfo.DisposeAsync().ConfigureAwait(false);

            await _cache.Clear().ConfigureAwait(false);
        }

        public Task<QueueInfo> Get(Queue queue)
        {
            lock (_durableQueues)
            {
                if (_durableQueues.TryGetValue(queue.EntityName, out var queueInfo))
                    return Task.FromResult(queueInfo);
            }

            return _cache.GetOrAdd(queue.EntityName, async key =>
            {
                try
                {
                    return await GetExistingQueue(queue.EntityName).ConfigureAwait(false);
                }
                catch (QueueDoesNotExistException)
                {
                    return await CreateMissingQueue(queue).ConfigureAwait(false);
                }
            });
        }

        public Task<QueueInfo> GetByName(string entityName)
        {
            lock (_durableQueues)
            {
                if (_durableQueues.TryGetValue(entityName, out var queueInfo))
                    return Task.FromResult(queueInfo);
            }

            return _cache.GetOrAdd(entityName, queueName => GetExistingQueue(queueName));
        }

        public Task<bool> RemoveByName(string entityName)
        {
            lock (_durableQueues)
                _durableQueues.Remove(entityName);

            return _cache.Remove(entityName);
        }

        async Task<QueueInfo> CreateMissingQueue(Queue queue)
        {
            Dictionary<string, string> attributes = queue.QueueAttributes.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (AmazonSqsEndpointAddress.IsFifo(queue.EntityName) && !attributes.ContainsKey(QueueAttributeName.FifoQueue))
            {
                LogContext.Warning?.Log("Using '.fifo' suffix without 'FifoQueue' attribute might cause unexpected behavior.");

                attributes[QueueAttributeName.FifoQueue] = "true";
            }

            var request = new CreateQueueRequest(queue.EntityName)
            {
                Attributes = attributes,
                Tags = queue.QueueTags.ToDictionary(x => x.Key, x => x.Value)
            };

            var createResponse = await _client.CreateQueueAsync(request, _cancellationToken).ConfigureAwait(false);

            createResponse.EnsureSuccessfulResponse();

            var attributesResponse = await _client.GetQueueAttributesAsync(createResponse.QueueUrl, AllAttributes, _cancellationToken).ConfigureAwait(false);

            attributesResponse.EnsureSuccessfulResponse();

            var missingQueue = new QueueInfo(queue.EntityName, createResponse.QueueUrl, attributesResponse.Attributes, _client, _cancellationToken, false);

            if (queue.Durable && queue.AutoDelete == false)
            {
                lock (_durableQueues)
                    _durableQueues[missingQueue.EntityName] = missingQueue;
            }

            return missingQueue;
        }

        async Task<QueueInfo> GetExistingQueue(string queueName)
        {
            var urlResponse = await _client.GetQueueUrlAsync(queueName, _cancellationToken).ConfigureAwait(false);

            urlResponse.EnsureSuccessfulResponse();

            var attributesResponse = await _client.GetQueueAttributesAsync(urlResponse.QueueUrl, AllAttributes, _cancellationToken).ConfigureAwait(false);

            attributesResponse.EnsureSuccessfulResponse();

            return new QueueInfo(queueName, urlResponse.QueueUrl, attributesResponse.Attributes, _client, _cancellationToken, true);
        }
    }
}
