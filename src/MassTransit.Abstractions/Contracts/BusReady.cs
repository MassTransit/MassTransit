namespace MassTransit
{
    public interface BusReady
    {
        IBus Bus { get; }

        HostReady Host { get; }
    }
}
