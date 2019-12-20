namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Configuration
{
    public enum ConcurrencyMode
    {
        Optimistic = 0,
        Pessimistic = 1,
    }
}
