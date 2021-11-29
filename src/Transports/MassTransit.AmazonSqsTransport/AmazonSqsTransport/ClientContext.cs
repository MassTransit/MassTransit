namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS.Model;
    using Topology;


    public interface ClientContext :
        PipeContext
    {
        ConnectionContext ConnectionContext { get; }

        Task<TopicInfo> CreateTopic(Topology.Topic topic);

        Task<QueueInfo> CreateQueue(Queue queue);

        Task CreateQueueSubscription(Topology.Topic topic, Queue queue);

        Task DeleteTopic(Topology.Topic topic);

        Task DeleteQueue(Queue queue);

        Task<PublishRequest> CreatePublishRequest(string topicName, string body);

        Task Publish(PublishRequest request, CancellationToken cancellationToken = default);

        Task SendMessage(string queueName, SendMessageBatchRequestEntry request, CancellationToken cancellationToken);

        Task DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken = default);

        Task PurgeQueue(string queueName, CancellationToken cancellationToken);

        Task<IList<Message>> ReceiveMessages(string queueName, int messageLimit, int waitTime, CancellationToken cancellationToken);

        Task<QueueInfo> GetQueueInfo(string queueName);

        Task ChangeMessageVisibility(string queueUrl, string receiptHandle, int seconds);
    }
}
