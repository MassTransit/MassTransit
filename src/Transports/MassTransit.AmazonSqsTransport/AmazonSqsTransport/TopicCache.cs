namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;
    using Caching;


    public class TopicCache
    {
        readonly ICache<TopicInfo> _cache;
        readonly CancellationToken _cancellationToken;
        readonly IAmazonSimpleNotificationService _client;
        readonly IDictionary<string, TopicInfo> _durableTopics;
        readonly Lazy<Task> _loadExistingTopics;
        readonly IIndex<string, TopicInfo> _nameIndex;
        bool _topicsLoaded;

        public TopicCache(IAmazonSimpleNotificationService client, CancellationToken cancellationToken)
        {
            _client = client;
            _cancellationToken = cancellationToken;
            _cache = new GreenCache<TopicInfo>(ClientContextCacheDefaults.GetCacheSettings());
            _nameIndex = _cache.AddIndex("entityName", x => x.EntityName);

            _loadExistingTopics = new Lazy<Task>(() => LoadExistingTopics(cancellationToken));

            _durableTopics = new Dictionary<string, TopicInfo>();
        }

        public async Task<TopicInfo> Get(Topology.Topic topic)
        {
            lock (_durableTopics)
            {
                if (_durableTopics.TryGetValue(topic.EntityName, out var queueInfo))
                    return queueInfo;
            }

            if (!_topicsLoaded)
                await _loadExistingTopics.Value.ConfigureAwait(false);

            return await _nameIndex.Get(topic.EntityName, key => CreateMissingTopic(topic)).ConfigureAwait(false);
        }

        public async Task<TopicInfo> GetByName(string entityName)
        {
            lock (_durableTopics)
            {
                if (_durableTopics.TryGetValue(entityName, out var topicInfo))
                    return topicInfo;
            }

            if (!_topicsLoaded)
                await _loadExistingTopics.Value.ConfigureAwait(false);

            return await _nameIndex.Get(entityName).ConfigureAwait(false);
        }

        public void RemoveByName(string entityName)
        {
            lock (_durableTopics)
                _durableTopics.Remove(entityName);

            _nameIndex.Remove(entityName);
        }

        async Task<TopicInfo> CreateMissingTopic(Topology.Topic topic)
        {
            Dictionary<string, string> attributes = topic.TopicAttributes.ToDictionary(x => x.Key, x => x.Value.ToString());

            // CreateTopicRequest is idempotent except for case when existing topic attributes are not matching the attributes provided in the request
            var request = new CreateTopicRequest(topic.EntityName)
            {
                Tags = topic.TopicTags.Select(x => new Tag
                {
                    Key = x.Key,
                    Value = x.Value
                }).ToList()
            };

            var createResponse = await _client.CreateTopicAsync(request, _cancellationToken).ConfigureAwait(false);

            createResponse.EnsureSuccessfulResponse();

            var attributesResponse = await _client.GetTopicAttributesAsync(createResponse.TopicArn, _cancellationToken).ConfigureAwait(false);

            attributesResponse.EnsureSuccessfulResponse();

            foreach (var attribute in attributes)
            {
                if (!attributesResponse.Attributes.ContainsKey(attribute.Key) || attributesResponse.Attributes[attribute.Key] != attribute.Value)
                {
                    var setTopicAttributesResponse = await _client.SetTopicAttributesAsync(createResponse.TopicArn, attribute.Key, attribute.Value, _cancellationToken).ConfigureAwait(false);

                    setTopicAttributesResponse.EnsureSuccessfulResponse();
                }
            }

            var missingTopic = new TopicInfo(topic.EntityName, createResponse.TopicArn);

            if (topic.Durable && topic.AutoDelete == false)
            {
                lock (_durableTopics)
                    _durableTopics[missingTopic.EntityName] = missingTopic;
            }

            return missingTopic;
        }

        async Task LoadExistingTopics(CancellationToken token)
        {
            var cursor = string.Empty;
            do
            {
                var request = new ListTopicsRequest { NextToken = cursor };

                var response = await _client.ListTopicsAsync(request, token).ConfigureAwait(false);

                foreach (var topic in response.Topics)
                {
                    var index = topic.TopicArn.LastIndexOf(":", StringComparison.OrdinalIgnoreCase);
                    if (index < 0)
                        continue;

                    var topicName = topic.TopicArn.Substring(index + 1);

                    await _nameIndex.Get(topicName, async key => new TopicInfo(topicName, topic.TopicArn)).ConfigureAwait(false);
                }

                cursor = response.NextToken;
            }
            while (!string.IsNullOrEmpty(cursor) && !token.IsCancellationRequested);

            _topicsLoaded = true;
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
