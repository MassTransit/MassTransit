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

    public IConnectionMultiplexer GetConnectionMultiplexer(string configuration)
    {
        IConnectionMultiplexer MultiplexerFactory(string configurationString)
        {
            LogContext.Debug?.Log("Creating Redis Connection Multiplexer: {Options}", ConfigurationOptions.Parse(configurationString).ToString(false));
            return ConnectionMultiplexer.Connect(configurationString);
        }

        Lazy<IConnectionMultiplexer> ValueFactory(string x)
        {
            return new Lazy<IConnectionMultiplexer>(() => MultiplexerFactory(x));
        }

        return _connectionMultiplexers.GetOrAdd(configuration, ValueFactory).Value;
    }
}
