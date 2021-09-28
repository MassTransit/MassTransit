namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS.Model;
    using GreenPipes;
    using Topology.Entities;


    public class ScopeClientContext :
        ScopePipeContext,
        ClientContext
    {
        readonly ClientContext _context;

        public ScopeClientContext(ClientContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        ConnectionContext ClientContext.ConnectionContext => _context.ConnectionContext;

        Task<TopicInfo> ClientContext.CreateTopic(Topology.Entities.Topic topic)
        {
            return _context.CreateTopic(topic);
        }

        Task<QueueInfo> ClientContext.CreateQueue(Queue queue)
        {
            return _context.CreateQueue(queue);
        }

        Task ClientContext.CreateQueueSubscription(Topology.Entities.Topic topic, Queue queue)
        {
            return _context.CreateQueueSubscription(topic, queue);
        }

        Task ClientContext.DeleteTopic(Topology.Entities.Topic topic)
        {
            return _context.DeleteTopic(topic);
        }

        Task ClientContext.DeleteQueue(Queue queue)
        {
            return _context.DeleteQueue(queue);
        }

        Task<PublishRequest> ClientContext.CreatePublishRequest(string topicName, byte[] body)
        {
            return _context.CreatePublishRequest(topicName, body);
        }

        Task ClientContext.Publish(PublishRequest request, CancellationToken cancellationToken)
        {
            return _context.Publish(request, cancellationToken);
        }

        Task ClientContext.SendMessage(string queueName, SendMessageBatchRequestEntry request, CancellationToken cancellationToken)
        {
            return _context.SendMessage(queueName, request, cancellationToken);
        }

        Task ClientContext.DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken)
        {
            return _context.DeleteMessage(queueUrl, receiptHandle, cancellationToken);
        }

        Task ClientContext.PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            return _context.PurgeQueue(queueName, cancellationToken);
        }

        Task<IList<Message>> ClientContext.ReceiveMessages(string queueName, int messageLimit, int waitTime, CancellationToken cancellationToken)
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
