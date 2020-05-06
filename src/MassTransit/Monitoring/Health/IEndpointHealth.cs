namespace MassTransit.Monitoring.Health
{
    public interface IEndpointHealth
    {
        HealthResult CheckHealth();
    }
}
