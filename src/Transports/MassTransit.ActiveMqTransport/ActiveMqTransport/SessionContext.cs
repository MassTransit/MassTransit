namespace MassTransit.ActiveMqTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Topology;


    public interface SessionContext :
        PipeContext
    {
        ISession Session { get; }

        ConnectionContext ConnectionContext { get; }

        Task<ITopic> GetTopic(Topic topic);

        Task<IQueue> GetQueue(Queue queue);

        Task<IDestination> GetDestination(string destinationName, DestinationType destinationType);

        Task<IMessageConsumer> CreateMessageConsumer(IDestination destination, string selector, bool noLocal);

        Task SendAsync(IDestination destination, IMessage message, CancellationToken cancellationToken);

        IBytesMessage CreateBytesMessage(byte[] content);

        ITextMessage CreateTextMessage(string content);

        IMessage CreateMessage();

        Task DeleteTopic(string topicName);

        Task DeleteQueue(string queueName);

        IDestination GetTemporaryDestination(string name);
    }
}
