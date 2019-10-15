namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService.Model;
    using Amazon.SQS.Model;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Topic = Topology.Entities.Topic;
    using Queue = Topology.Entities.Queue;


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

        ConnectionContext ClientContext.ConnectionContext => _context.ConnectionContext;

        Task<string> ClientContext.CreateTopic(Topic topic)
        {
            return _context.CreateTopic(topic);
        }

        Task<string> ClientContext.CreateQueue(Queue queue)
        {
            return _context.CreateQueue(queue);
        }

        Task ClientContext.CreateQueueSubscription(Topic topic, Queue queue)
        {
            return _context.CreateQueueSubscription(topic, queue);
        }

        Task ClientContext.DeleteTopic(Topic topic)
        {
            return _context.DeleteTopic(topic);
        }

        Task ClientContext.DeleteQueue(Queue queue)
        {
            return _context.DeleteQueue(queue);
        }

        Task ClientContext.BasicConsume(ReceiveSettings receiveSettings, IBasicConsumer consumer)
        {
            return _context.BasicConsume(receiveSettings, consumer);
        }

        PublishRequest ClientContext.CreatePublishRequest(string topicName, byte[] body)
        {
            return _context.CreatePublishRequest(topicName, body);
        }

        Task ClientContext.Publish(PublishRequest request, CancellationToken cancellationToken)
        {
            return _context.Publish(request, cancellationToken);
        }

        Task ClientContext.DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken)
        {
            return _context.DeleteMessage(queueUrl, receiptHandle, cancellationToken);
        }

        Task ClientContext.PurgeQueue(string queueName, CancellationToken cancellationToken)
        {
            return _context.PurgeQueue(queueName, cancellationToken);
        }

        SendMessageRequest ClientContext.CreateSendRequest(string queueName, byte[] body)
        {
            return _context.CreateSendRequest(queueName, body);
        }

        Task ClientContext.SendMessage(SendMessageRequest request, CancellationToken cancellationToken)
        {
            return _context.SendMessage(request, cancellationToken);
        }
    }
}
