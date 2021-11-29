namespace MassTransit.ActiveMqTransport
{
    using System.Threading.Tasks;
    using Apache.NMS;


    public interface SessionContext :
        PipeContext
    {
        ISession Session { get; }

        ConnectionContext ConnectionContext { get; }

        Task<ITopic> GetTopic(string topicName);

        Task<IQueue> GetQueue(string queueName);

        Task<IDestination> GetDestination(string destination, DestinationType destinationType);

        Task<IMessageProducer> CreateMessageProducer(IDestination destination);

        Task<IMessageConsumer> CreateMessageConsumer(IDestination destination, string selector, bool noLocal);

        Task DeleteTopic(string topicName);

        Task DeleteQueue(string queueName);
    }
}
