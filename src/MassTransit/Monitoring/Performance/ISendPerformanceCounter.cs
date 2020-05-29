namespace MassTransit.Monitoring.Performance
{
    public interface ISendPerformanceCounter
    {
        void Sent();
        void Faulted();
    }
}
