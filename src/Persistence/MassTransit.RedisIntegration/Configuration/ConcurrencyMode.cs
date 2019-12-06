namespace MassTransit.RedisIntegration
{
    public enum ConcurrencyMode
    {
        Optimistic = 0,
        Pessimistic = 1,
    }
}
