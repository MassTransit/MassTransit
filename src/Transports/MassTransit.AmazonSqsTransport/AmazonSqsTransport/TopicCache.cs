namespace MassTransit.AmazonSqsTransport;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Internals;
using Internals.Caching;


public class TopicCache :
    IAsyncDisposable
{
    readonly ICache<string, TopicInfo, ITimeToLiveCacheValue<TopicInfo>> _cache;
    readonly CancellationToken _cancellationToken;
    readonly IAmazonSimpleNotificationService _client;
    readonly IDictionary<string, TopicInfo> _durableTopics;
    Lazy<Task> _loadExistingTopics;
    bool _topicsLoaded;

    public TopicCache(IAmazonSimpleNotificationService client, CancellationToken cancellationToken)
    {
        _client = client;
        _cancellationToken = cancellationToken;

        _cache = ClientContextCacheDefaults.CreateCache<string, TopicInfo>();

        _loadExistingTopics = ResetLoadExistingTopics();

        _durableTopics = new Dictionary<string, TopicInfo>();
    }

    public async ValueTask DisposeAsync()
    {
        TopicInfo[] topicInfos;
        lock (_durableTopics)
        {
            topicInfos = _durableTopics.Values.ToArray();

            _durableTopics.Clear();
        }

        foreach (var topicInfo in topicInfos)
            await topicInfo.DisposeAsync().ConfigureAwait(false);

        await _cache.Clear().ConfigureAwait(false);
    }

    public async Task<TopicInfo> Get(Topology.Topic topic, CancellationToken cancellationToken)
    {
        if (!_topicsLoaded)
            await LoadExistingTopics().OrCanceled(cancellationToken).ConfigureAwait(false);

        lock (_durableTopics)
        {
            if (_durableTopics.TryGetValue(topic.EntityName, out var topicInfo))
                return topicInfo;
        }

        return await _cache.GetOrAdd(topic.EntityName, _ => CreateMissingTopic(topic, cancellationToken)).ConfigureAwait(false);
    }

    public async Task<TopicInfo> GetByName(string entityName, CancellationToken cancellationToken)
    {
        if (!_topicsLoaded)
            await LoadExistingTopics().OrCanceled(cancellationToken).ConfigureAwait(false);

        lock (_durableTopics)
        {
            if (_durableTopics.TryGetValue(entityName, out var topicInfo))
                return topicInfo;
        }

        return await _cache.Get(entityName).ConfigureAwait(false);
    }

    public Task<bool> RemoveByName(string entityName)
    {
        lock (_durableTopics)
            _durableTopics.Remove(entityName);

        return _cache.Remove(entityName);
    }

    async Task<TopicInfo> CreateMissingTopic(Topology.Topic topic, CancellationToken cancellationToken)
    {
        var request = new CreateTopicRequest(topic.EntityName)
        {
            Attributes = topic.TopicAttributes.ToDictionary(x => x.Key, x => x.Value.ToString()),
            Tags = topic.TopicTags.Select(x => new Tag
            {
                Key = x.Key,
                Value = x.Value
            }).ToList()
        };

        var createResponse = await _client.CreateTopicAsync(request, cancellationToken).ConfigureAwait(false);

        createResponse.EnsureSuccessfulResponse();

        var attributesResponse = await _client.GetTopicAttributesAsync(createResponse.TopicArn, cancellationToken).ConfigureAwait(false);

        attributesResponse.EnsureSuccessfulResponse();

        var missingTopic = new TopicInfo(topic.EntityName, createResponse.TopicArn, _client, cancellationToken, false);

        if (topic is { Durable: true, AutoDelete: false })
        {
            lock (_durableTopics)
                _durableTopics[missingTopic.EntityName] = missingTopic;
        }

        return missingTopic;
    }

    Lazy<Task> ResetLoadExistingTopics()
    {
        return _loadExistingTopics = new Lazy<Task>(() => LoadExistingTopicsLazy(_cancellationToken));
    }

    Task LoadExistingTopics()
    {
        var result = _loadExistingTopics.Value;
        if (result.IsFaulted || result.IsCanceled)
        {
            lock (this)
            {
                result = _loadExistingTopics.Value;
                if (result.IsFaulted || result.IsCanceled)
                    result = ResetLoadExistingTopics().Value;
            }
        }

        return result;
    }

    async Task LoadExistingTopicsLazy(CancellationToken cancellationToken)
    {
        var cursor = string.Empty;
        do
        {
            var request = new ListTopicsRequest { NextToken = cursor };

            var response = await _client.ListTopicsAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.Topics != null)
            {
                foreach (var topic in response.Topics)
                {
                    var index = topic.TopicArn.LastIndexOf(":", StringComparison.OrdinalIgnoreCase);
                    if (index < 0)
                        continue;

                    var topicName = topic.TopicArn.Substring(index + 1);

                    await _cache.GetOrAdd(topicName, async _ =>
                    {
                        var topicInfo = new TopicInfo(topicName, topic.TopicArn, _client, _cancellationToken, true);

                        lock (_durableTopics)
                            _durableTopics[topicInfo.EntityName] = topicInfo;

                        return topicInfo;
                    }).ConfigureAwait(false);
                }
            }

            cursor = response.NextToken;
        }
        while (!string.IsNullOrEmpty(cursor) && !cancellationToken.IsCancellationRequested);

        _topicsLoaded = true;
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
