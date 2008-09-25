namespace MassTransit.Host
{
    public interface IStartUpTime
    {
        void RequestMoreTime(int milliseconds);
    }
}