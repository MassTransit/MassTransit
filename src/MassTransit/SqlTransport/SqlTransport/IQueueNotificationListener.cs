namespace MassTransit.SqlTransport
{
    using System.Threading.Tasks;


    public interface IQueueNotificationListener
    {
        Task MessageReady(string queueName);
    }
}
