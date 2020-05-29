namespace MassTransit.Monitoring.Performance.Null
{
    public class NullCounterFactory : ICounterFactory
    {
        public IPerformanceCounter Create(CounterCategory category, string counterName, string instanceName)
        {
            return new NullPerformanceCounter();
        }
    }
}
