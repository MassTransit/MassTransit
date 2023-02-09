namespace MassTransit.Middleware.Outbox
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IBusOutboxNotification
    {
        Task WaitForDelivery(CancellationToken cancellationToken);
        void Delivered();
    }
}
