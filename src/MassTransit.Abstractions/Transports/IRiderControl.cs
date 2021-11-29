namespace MassTransit.Transports
{
    using System.Threading;


    public interface IRiderControl :
        IRider
    {
        RiderHandle Start(CancellationToken cancellationToken = default);
    }
}
