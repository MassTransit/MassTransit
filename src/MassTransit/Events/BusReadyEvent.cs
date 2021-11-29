namespace MassTransit.Events
{
    public class BusReadyEvent :
        BusReady
    {
        public BusReadyEvent(HostReady host, IBus bus)
        {
            Host = host;
            Bus = bus;
        }

        public IBus Bus { get; }

        public HostReady Host { get; }
    }
}
