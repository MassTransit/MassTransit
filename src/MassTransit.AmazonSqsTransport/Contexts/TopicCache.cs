namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;
    using GreenPipes.Caching;


    public class TopicCache
    {
        readonly IAmazonSimpleNotificationService _client;
        readonly ICache<TopicInfo> _cache;
        readonly IIndex<string, TopicInfo> _nameIndex;

        public TopicCache(IAmazonSimpleNotificationService client)
        {
            _client = client;
            _cache = new GreenCache<TopicInfo>(ClientContextCacheDefaults.GetCacheSettings());
            _nameIndex = _cache.AddIndex("entityName", x => x.EntityName);
        }

        public Task<TopicInfo> Get(Topology.Entities.Topic topic, CancellationToken cancellationToken)
        {
            return _nameIndex.Get(topic.EntityName, key => CreateMissingTopic(topic, cancellationToken));
        }

        public Task<TopicInfo> GetByName(string entityName)
        {
            return _nameIndex.Get(entityName);
        }

        public void RemoveByName(string entityName)
        {
            _nameIndex.Remove(entityName);
        }

        async Task<TopicInfo> CreateMissingTopic(Topology.Entities.Topic topic, CancellationToken cancellationToken)
        {
            var attributes = topic.TopicAttributes.ToDictionary(x => x.Key, x => x.Value.ToString());

            var request = new CreateTopicRequest(topic.EntityName)
            {
                Attributes = attributes,
                Tags = topic.TopicTags.Select(x => new Tag
                {
                    Key = x.Key,
                    Value = x.Value
                }).ToList()
            };

            var response = await _client.CreateTopicAsync(request, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessfulResponse();

            var topicArn = response.TopicArn;

            var attributesResponse = await _client.GetTopicAttributesAsync(topicArn, cancellationToken).ConfigureAwait(false);

            attributesResponse.EnsureSuccessfulResponse();

            return new TopicInfo(topic.EntityName, topicArn, attributesResponse.Attributes);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
