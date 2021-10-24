using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.None)]
[assembly: LevelOfParallelism(1)]


namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;

    [SetUpFixture]
    public class AzureServiceBusTestSetUpFixture
    {
        [OneTimeSetUp]
        public async Task Before_any()
        {
            await CleanupNamespace();
        }

        async Task CleanupNamespace()
        {
            try
            {
                var managementClient = Configuration.GetManagementClient();

                var pageableTopics = managementClient.GetTopicsAsync();
                var topics = await pageableTopics.ToList();
                while (topics.Count > 0)
                {
                    foreach (var topic in topics)
                        await managementClient.DeleteTopicAsync(topic.Name);

                    topics = await managementClient.GetTopicsAsync().ToList();
                }

                var pageableQueues = managementClient.GetQueuesAsync();
                var queues = await pageableQueues.ToList();
                while (queues.Count > 0)
                {
                    foreach (var queue in queues)
                        await managementClient.DeleteQueueAsync(queue.Name);

                    queues = await managementClient.GetQueuesAsync().ToList();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
