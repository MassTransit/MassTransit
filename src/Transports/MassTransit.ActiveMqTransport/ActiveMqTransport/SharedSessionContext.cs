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

        ISession SessionContext.Session => _context.Session;
        ConnectionContext SessionContext.ConnectionContext => _context.ConnectionContext;

        Task<ITopic> SessionContext.GetTopic(Topic topic)
        {
            return _context.GetTopic(topic);
        }

        Task<IQueue> SessionContext.GetQueue(Queue queue)
        {
            return _context.GetQueue(queue);
        }

        Task<IDestination> SessionContext.GetDestination(string destinationName, DestinationType destinationType)
        {
            return _context.GetDestination(destinationName, destinationType);
        }

        Task<IMessageProducer> SessionContext.CreateMessageProducer(IDestination destination)
        {
            return _context.CreateMessageProducer(destination);
        }

        Task<IMessageConsumer> SessionContext.CreateMessageConsumer(IDestination destination, string selector, bool noLocal)
        {
            return _context.CreateMessageConsumer(destination, selector, noLocal);
        }

        public IBytesMessage CreateBytesMessage()
        {
            return _context.CreateBytesMessage();
        }

        Task SessionContext.DeleteTopic(string topicName)
        {
            return _context.DeleteTopic(topicName);
        }

        Task SessionContext.DeleteQueue(string queueName)
        {
            return _context.DeleteQueue(queueName);
        }

        public IDestination GetTemporaryDestination(string name)
        {
            return _context.GetTemporaryDestination(name);
        }
    }
}
