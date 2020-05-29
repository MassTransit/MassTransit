namespace MassTransit.RabbitMqTransport.Management
{
    using System.Threading.Tasks;


    public interface ISetPrefetchCount
    {
        Task SetPrefetchCount(ushort prefetchCount);
    }
}
