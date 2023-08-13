namespace MassTransit.ActiveMqTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using MassTransit.Middleware;
    using Topology;


    public class SharedSessionContext :
        ProxyPipeContext,
        SessionContext
    {
        readonly SessionContext _context;

        public SharedSessionContext(SessionContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public ISession Session => _context.Session;
        public ConnectionContext ConnectionContext => _context.ConnectionContext;

        public Task<ITopic> GetTopic(Topic topic)
        {
            return _context.GetTopic(topic);
        }

        public Task<IQueue> GetQueue(Queue queue)
        {
            return _context.GetQueue(queue);
        }

        public Task<IDestination> GetDestination(string destinationName, DestinationType destinationType)
        {
            return _context.GetDestination(destinationName, destinationType);
        }

        public Task<IMessageConsumer> CreateMessageConsumer(IDestination destination, string selector, bool noLocal)
        {
            return _context.CreateMessageConsumer(destination, selector, noLocal);
        }

        public Task SendAsync(IDestination destination, IMessage message, CancellationToken cancellationToken)
        {
            return _context.SendAsync(destination, message, cancellationToken);
        }

        public IBytesMessage CreateBytesMessage(byte[] content)
        {
            return _context.CreateBytesMessage(content);
        }

        public Task DeleteTopic(string topicName)
        {
            return _context.DeleteTopic(topicName);
        }

        public Task DeleteQueue(string queueName)
        {
            return _context.DeleteQueue(queueName);
        }

        public IDestination GetTemporaryDestination(string name)
        {
            return _context.GetTemporaryDestination(name);
        }

        public ITextMessage CreateTextMessage(string content)
        {
            return _context.CreateTextMessage(content);
        }

        public IMessage CreateMessage()
        {
            return _context.CreateMessage();
        }
    }
}
