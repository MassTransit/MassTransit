namespace MassTransit.AmazonSqsTransport;

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

    Task<TopicInfo> CreateTopic(Topology.Topic topic, CancellationToken cancellationToken);

    Task<QueueInfo> CreateQueue(Queue queue, CancellationToken cancellationToken);

    Task<bool> CreateQueueSubscription(Topology.Topic topic, Queue queue, CancellationToken cancellationToken);

    Task DeleteTopic(Topology.Topic topic, CancellationToken cancellationToken);

    Task DeleteQueue(Queue queue, CancellationToken cancellationToken);

    Task Publish(string topicName, PublishBatchRequestEntry request, CancellationToken cancellationToken);

    Task SendMessage(string queueName, SendMessageBatchRequestEntry request, CancellationToken cancellationToken);

    Task DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken);

    Task PurgeQueue(string queueName, CancellationToken cancellationToken);

    Task<IList<Message>> ReceiveMessages(string queueName, int messageLimit, int waitTime, CancellationToken cancellationToken);

    Task<QueueInfo> GetQueueInfo(string queueName, CancellationToken cancellationToken);

    Task ChangeMessageVisibility(string queueUrl, string receiptHandle, int seconds, CancellationToken cancellationToken);
}
