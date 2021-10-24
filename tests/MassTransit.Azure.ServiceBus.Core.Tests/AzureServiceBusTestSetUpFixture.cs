using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.None)]
[assembly: LevelOfParallelism(1)]


namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Azure;

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

                async Task<List<T>> ToListAsync<T>(AsyncPageable<T> pageable)
                {
                    var items = new List<T>();
                    await foreach (var item in pageable)
                    {
                        items.Add(item);
                    }

                    return items;
                }

                var pageableTopics = managementClient.GetTopicsAsync();
                var topics = await ToListAsync(pageableTopics);
                while (topics.Count > 0)
                {
                    foreach (var topic in topics)
                        await managementClient.DeleteTopicAsync(topic.Name);

                    topics = await ToListAsync(managementClient.GetTopicsAsync());
                }

                var pageableQueues = managementClient.GetQueuesAsync();
                var queues = await ToListAsync(pageableQueues);
                while (queues.Count > 0)
                {
                    foreach (var queue in queues)
                        await managementClient.DeleteQueueAsync(queue.Name);

                    queues = await ToListAsync(managementClient.GetQueuesAsync());
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
