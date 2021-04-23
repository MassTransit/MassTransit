namespace MassTransit.GrpcTransport.Fabric
{
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IMessageSink<T> :
        IProbeSite
        where T : class
    {
        Task Deliver(DeliveryContext<T> context);
    }
}
