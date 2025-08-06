# Redis

[![alt NuGet](https://img.shields.io/nuget/v/MassTransit.Redis.svg "NuGet")](https://nuget.org/packages/MassTransit.Redis/)

Redis is a very popular key-value store, which is known for being very fast. To support Redis, MassTransit uses the `StackExchange.Redis` library.

::alert{type="warning"}
Redis only supports event correlation by _CorrelationId_, it does not support queries. If a saga uses expression-based correlation, a _NotImplementedByDesignException_ will be thrown.
::

::alert{type="info"}
The Redis package also supports the recent fork of Redis called [Valkey](https://github.com/valkey-io/valkey)
::

Storing a saga in Redis requires an additional interface, _ISagaVersion_, which has a _Version_ property used for optimistic concurrency. An example saga is shown below.

```csharp
public class OrderState :
    SagaStateMachineInstance,
    ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }

    public int Version { get; set; }
}
```

## Configuration

To configure Redis as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension. This will configure Redis to connect to the local Redis instance on the default port using Optimistic concurrency. This will also store the _ConnectionMultiplexer_ in the container as a single instance, which will be disposed by the container.

```csharp
services.AddMassTransit(x =>
{
    const string configurationString = "127.0.0.1";

    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .RedisRepository(configurationString);
});
```

The example below includes all the configuration options, in cases where additional settings are required.

```csharp
services.AddMassTransit(x =>
{
    const string configurationString = "127.0.0.1";

    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .RedisRepository(r =>
        {
            r.DatabaseConfiguration(configurationString);

            // Default is Optimistic
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;

            // Optional, prefix each saga instance key with the string specified
            // resulting dev:c6cfd285-80b2-4c12-bcd3-56a00d994736
            r.KeyPrefix = "dev";

            // Optional, to customize the lock key
            r.LockSuffix = "-lockage";

            // Optional, the default is 30 seconds
            r.LockTimeout = TimeSpan.FromSeconds(90);
        });;
});
```

## Concurrency

Redis supports both Optimistic (default) and Pessimistic concurrency.

In optimistic mode, the saga instance is not locked before reading, which can ultimately lead to a write conflict if the instance was updated by another message. The _Version_ property is used to compare that the update would not overwrite a previous update. It is recommended that a retry policy is configured (using `UseMessageRetry`, see the [exceptions](/documentation/concepts/exceptions#retry) documentation).

Pessimistic concurrency uses the Redis lock mechanism. During the message processing, the repository will lock the saga instance before reading it, so that any concurrent attempts to lock the same instance will wait until the current message has completed or the lock timeout expires.
