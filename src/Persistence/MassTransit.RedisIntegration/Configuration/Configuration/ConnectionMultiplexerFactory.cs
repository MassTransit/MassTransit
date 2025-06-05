namespace MassTransit.Configuration;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using StackExchange.Redis;


public class ConnectionMultiplexerFactory :
    IConnectionMultiplexerFactory,
    IAsyncDisposable
{
    readonly ConcurrentDictionary<string, Lazy<IConnectionMultiplexer>> _connectionMultiplexers;

    public ConnectionMultiplexerFactory()
    {
        _connectionMultiplexers = new ConcurrentDictionary<string, Lazy<IConnectionMultiplexer>>();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (Lazy<IConnectionMultiplexer> value in _connectionMultiplexers.Values)
        {
            if (value.IsValueCreated)
                await value.Value.DisposeAsync().ConfigureAwait(false);
        }
    }

    public IConnectionMultiplexer GetConnectionMultiplexer(ConfigurationOptions configuration)
    {
        IConnectionMultiplexer MultiplexerFactory(ConfigurationOptions configurationOptions)
        {
            LogContext.Debug?.Log("Creating Redis Connection Multiplexer: {Options}", configurationOptions.ToString(false));

            return ConnectionMultiplexer.Connect(configurationOptions);
        }

        Lazy<IConnectionMultiplexer> ValueFactory(ConfigurationOptions x)
        {
            return new Lazy<IConnectionMultiplexer>(() => MultiplexerFactory(x));
        }

        var key = configuration.ToString(false);

        Lazy<IConnectionMultiplexer> connection = _connectionMultiplexers.GetOrAdd(key, ValueFactory(configuration));
        try
        {
            return connection.Value;
        }
        catch (Exception)
        {
            _connectionMultiplexers.TryRemove(key, out _);

            throw;
        }
    }
}
