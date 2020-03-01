namespace MassTransit.RedisIntegration
{
    using StackExchange.Redis;


    public delegate IDatabase SelectDatabase(IConnectionMultiplexer multiplexer);
}
