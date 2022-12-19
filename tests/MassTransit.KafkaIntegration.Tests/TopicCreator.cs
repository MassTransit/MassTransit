namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;


    class TopicCreator :
        IAsyncDisposable
    {
        readonly IAdminClient _client;
        readonly List<string> _topicNames;

        public TopicCreator(ClientConfig clientConfig)
        {
            _client = new AdminClientBuilder(new AdminClientConfig(clientConfig)).Build();
            _topicNames = new List<string>();
        }

        public async Task CreateTopics(int partitions = 1, short replicas = 1, params string[] topicNames)
        {
            foreach (var topicName in topicNames)
            {
                try
                {
                    await _client.CreateTopicsAsync(new[]
                    {
                        new TopicSpecification
                        {
                            Name = topicName,
                            NumPartitions = partitions,
                            ReplicationFactor = replicas
                        }
                    });
                }
                catch (CreateTopicsException)
                {
                }
                finally
                {
                    _topicNames.Add(topicName);
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _client.DeleteTopicsAsync(_topicNames).ConfigureAwait(false);
            }
            finally
            {
                _client.Dispose();
            }
        }
    }
}
