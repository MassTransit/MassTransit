namespace MassTransit.Riders
{
    using System.Threading;


    public interface IRiderControl :
        IRider
    {
        RiderHandle Start(CancellationToken cancellationToken = default);
    }
}
