namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS.Model;
    using MassTransit.Middleware;
    using Topology;


    public class SharedClientContext :
        ProxyPipeContext,
        ClientContext
    {
        readonly ClientContext _context;

        public SharedClientContext(ClientContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public ConnectionContext ConnectionContext => _context.ConnectionContext;

        public Task<TopicInfo> CreateTopic(Topology.Topic topic)
        {
            return _context.CreateTopic(topic);
        }

        public Task<QueueInfo> CreateQueue(Queue queue)
        {
            return _context.CreateQueue(queue);
        }

        public Task CreateQueueSubscription(Topology.Topic topic, Queue queue)
        {
            return _context.CreateQueueSubscription(topic, queue);
        }

        public Task DeleteTopic(Topology.Topic topic)
        {
            return _context.DeleteTopic(topic);
        }

        public Task DeleteQueue(Queue queue)
        {
            return _context.DeleteQueue(queue);
        }

        public Task<PublishRequest> CreatePublishRequest(string topicName, string body)
        {
            return _context.CreatePublishRequest(topicName, body);
        }

        public Task Publish(PublishRequest request, CancellationToken cancellationToken)
        {
            return _context.Publish(request, cancellationToken);
        }

        public Task SendMessage(string queueName, SendMessageBatchRequestEntry request, CancellationToken cancellationToken)
        {
            return _context.SendMessage(queueName, request, cancellationToken);
        }

        public Task DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken)
        {
            return _context.DeleteMessage(queueUrl, receiptHandle, cancellationToken);
        }

        public Task PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            return _context.PurgeQueue(queueName, cancellationToken);
        }

        public Task<IList<Message>> ReceiveMessages(string queueName, int messageLimit, int waitTime, CancellationToken cancellationToken)
        {
            return _context.ReceiveMessages(queueName, messageLimit, waitTime, cancellationToken);
        }

        public Task<QueueInfo> GetQueueInfo(string queueName)
        {
            return _context.GetQueueInfo(queueName);
        }

        public Task ChangeMessageVisibility(string queueUrl, string receiptHandle, int seconds)
        {
            return _context.ChangeMessageVisibility(queueUrl, receiptHandle, seconds);
        }
    }
}
