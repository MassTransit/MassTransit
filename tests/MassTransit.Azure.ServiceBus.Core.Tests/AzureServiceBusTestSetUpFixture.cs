using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.None)]
[assembly: LevelOfParallelism(1)]


namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus.Management;


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

                IList<TopicDescription> topics = await managementClient.GetTopicsAsync();
                while (topics.Count > 0)
                {
                    foreach (var topic in topics)
                        await managementClient.DeleteTopicAsync(topic.Path);

                    topics = await managementClient.GetTopicsAsync();
                }

                IList<QueueDescription> queues = await managementClient.GetQueuesAsync();
                while (queues.Count > 0)
                {
                    foreach (var queue in queues)
                        await managementClient.DeleteQueueAsync(queue.Path);

                    queues = await managementClient.GetQueuesAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
