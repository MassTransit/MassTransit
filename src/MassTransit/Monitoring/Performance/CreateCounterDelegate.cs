namespace MassTransit.Monitoring.Performance
{
    public delegate IPerformanceCounter CreateCounterDelegate(string name, string instanceName);
}
