namespace MassTransit.RabbitMqTransport.Middleware
{
    using System.Threading.Tasks;


    public interface ISetPrefetchCount
    {
        Task SetPrefetchCount(ushort prefetchCount);
    }
}
