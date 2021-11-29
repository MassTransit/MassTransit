namespace MassTransit.AmazonSqsTransport
{
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
        readonly IIndex<string, TopicInfo> _nameIndex;

        public TopicCache(IAmazonSimpleNotificationService client, CancellationToken cancellationToken)
        {
            _client = client;
            _cancellationToken = cancellationToken;
            _cache = new GreenCache<TopicInfo>(ClientContextCacheDefaults.GetCacheSettings());
            _nameIndex = _cache.AddIndex("entityName", x => x.EntityName);

            _durableTopics = new Dictionary<string, TopicInfo>();
        }

        public Task<TopicInfo> Get(Topology.Topic topic)
        {
            lock (_durableTopics)
            {
                if (_durableTopics.TryGetValue(topic.EntityName, out var queueInfo))
                    return Task.FromResult(queueInfo);
            }

            return _nameIndex.Get(topic.EntityName, async key =>
            {
                try
                {
                    return await GetExistingTopic(topic.EntityName).ConfigureAwait(false);
                }
                catch (AmazonSqsTransportException e) when (e.Message.Equals($"Topic {topic.EntityName} not found."))
                {
                    return await CreateMissingTopic(topic).ConfigureAwait(false);
                }
            });
        }

        public Task<TopicInfo> GetByName(string entityName)
        {
            lock (_durableTopics)
            {
                if (_durableTopics.TryGetValue(entityName, out var queueInfo))
                    return Task.FromResult(queueInfo);
            }

            return _nameIndex.Get(entityName, key => GetExistingTopic(key));
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

            var request = new CreateTopicRequest(topic.EntityName)
            {
                Attributes = attributes,
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

            var missingTopic = new TopicInfo(topic.EntityName, createResponse.TopicArn, attributesResponse.Attributes);

            if (topic.Durable && topic.AutoDelete == false)
            {
                lock (_durableTopics)
                    _durableTopics[missingTopic.EntityName] = missingTopic;
            }

            return missingTopic;
        }

        async Task<TopicInfo> GetExistingTopic(string topicName)
        {
            var topic = await _client.FindTopicAsync(topicName).ConfigureAwait(false);

            if (topic == null)
                throw new AmazonSqsTransportException($"Topic {topicName} not found.");

            var attributesResponse = await _client.GetTopicAttributesAsync(topic.TopicArn, _cancellationToken).ConfigureAwait(false);

            attributesResponse.EnsureSuccessfulResponse();

            return new TopicInfo(topicName, topic.TopicArn, attributesResponse.Attributes);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
