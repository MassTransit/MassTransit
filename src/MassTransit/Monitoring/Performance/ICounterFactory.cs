namespace MassTransit.Monitoring.Performance
{
    public interface ICounterFactory
    {
        IPerformanceCounter Create(CounterCategory category, string counterName, string instanceName);
    }
}
