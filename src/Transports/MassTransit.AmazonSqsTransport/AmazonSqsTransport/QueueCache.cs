namespace MassTransit.AmazonSqsTransport;

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
    static readonly List<string> AllAttributes = [QueueAttributeName.All];

    readonly ICache<string, QueueInfo, ITimeToLiveCacheValue<QueueInfo>> _cache;
    readonly IAmazonSQS _client;
    readonly IDictionary<string, QueueInfo> _durableQueues;

    public QueueCache(IAmazonSQS client)
    {
        _client = client;

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

    public Task<QueueInfo> Get(Queue queue, CancellationToken cancellationToken)
    {
        lock (_durableQueues)
        {
            if (_durableQueues.TryGetValue(queue.EntityName, out var queueInfo))
                return Task.FromResult(queueInfo);
        }

        return _cache.GetOrAdd(queue.EntityName, async _ =>
        {
            try
            {
                return await GetExistingQueue(queue.EntityName, cancellationToken).ConfigureAwait(false);
            }
            catch (QueueDoesNotExistException)
            {
                return await CreateMissingQueue(queue, cancellationToken).ConfigureAwait(false);
            }
        });
    }

    public Task<QueueInfo> GetByName(string entityName, CancellationToken cancellationToken)
    {
        lock (_durableQueues)
        {
            if (_durableQueues.TryGetValue(entityName, out var queueInfo))
                return Task.FromResult(queueInfo);
        }

        return _cache.GetOrAdd(entityName, queueName => GetExistingQueue(queueName, cancellationToken));
    }

    public Task<bool> RemoveByName(string entityName)
    {
        lock (_durableQueues)
            _durableQueues.Remove(entityName);

        return _cache.Remove(entityName);
    }

    async Task<QueueInfo> CreateMissingQueue(Queue queue, CancellationToken cancellationToken)
    {
        Dictionary<string, string> attributes = queue.QueueAttributes.ToDictionary(x => x.Key, x => x.Value.ToString()!);

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

        var createResponse = await _client.CreateQueueAsync(request, cancellationToken).ConfigureAwait(false);

        createResponse.EnsureSuccessfulResponse();

        var attributesResponse = await _client.GetQueueAttributesAsync(createResponse.QueueUrl, AllAttributes, cancellationToken).ConfigureAwait(false);

        attributesResponse.EnsureSuccessfulResponse();

        var missingQueue = new QueueInfo(queue.EntityName, createResponse.QueueUrl, attributesResponse.Attributes ?? new Dictionary<string, string>(),
            _client, cancellationToken, false);

        if (queue is { Durable: true, AutoDelete: false })
        {
            lock (_durableQueues)
                _durableQueues[missingQueue.EntityName] = missingQueue;
        }

        return missingQueue;
    }

    async Task<QueueInfo> GetExistingQueue(string queueName, CancellationToken cancellationToken)
    {
        var urlResponse = await _client.GetQueueUrlAsync(queueName, cancellationToken).ConfigureAwait(false);

        urlResponse.EnsureSuccessfulResponse();

        var attributesResponse = await _client.GetQueueAttributesAsync(urlResponse.QueueUrl, AllAttributes, cancellationToken).ConfigureAwait(false);

        attributesResponse.EnsureSuccessfulResponse();

        return new QueueInfo(queueName, urlResponse.QueueUrl, attributesResponse.Attributes ?? new Dictionary<string, string>(), _client,
            cancellationToken, true);
    }
}
