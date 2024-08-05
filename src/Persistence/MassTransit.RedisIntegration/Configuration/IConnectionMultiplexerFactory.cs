namespace MassTransit;

using StackExchange.Redis;


public interface IConnectionMultiplexerFactory
{
    IConnectionMultiplexer GetConnectionMultiplexer(ConfigurationOptions configuration);
}
