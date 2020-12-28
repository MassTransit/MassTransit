namespace MassTransit.Monitoring.Health
{
    public interface IEndpointHealth
    {
        EndpointHealthResult CheckHealth();
    }
}
