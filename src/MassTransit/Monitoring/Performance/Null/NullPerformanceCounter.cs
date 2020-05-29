namespace MassTransit.Monitoring.Performance.Null
{
    public class NullPerformanceCounter :
        IPerformanceCounter
    {
        public void Increment()
        {
        }

        public void IncrementBy(long val)
        {
        }

        public void Set(long val)
        {
        }

        public void Dispose()
        {
        }
    }
}
