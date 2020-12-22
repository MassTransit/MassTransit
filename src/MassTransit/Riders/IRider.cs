namespace MassTransit.Riders
{
    public interface IRider
    {
        void Connect(IHost host);
    }
}
