namespace MassTransit.Riders
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IRider
    {
        void Connect(IHost host);
    }


    public interface RiderHandle
    {
        Task Stop(CancellationToken cancellationToken = default);
    }
}
