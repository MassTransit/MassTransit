namespace MassTransit.Transports.InMemory.Fabric
{
    using System.Threading.Tasks;


    public interface IMessageSink<T>
        where T : class
    {
        Task Deliver(DeliveryContext<T> context);
    }
}