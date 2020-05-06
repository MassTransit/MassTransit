namespace MassTransit.Monitoring.Health
{
    public interface IBusHealth
    {
        string Name { get; }

        HealthResult CheckHealth();
    }
}
