namespace MassTransit.ActiveMqTransport
{
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

        Task<IMessageProducer> CreateMessageProducer(IDestination destination);

        Task<IMessageConsumer> CreateMessageConsumer(IDestination destination, string selector, bool noLocal);

        IBytesMessage CreateBytesMessage();

        Task DeleteTopic(string topicName);

        Task DeleteQueue(string queueName);

        IDestination GetTemporaryDestination(string name);
    }
}
