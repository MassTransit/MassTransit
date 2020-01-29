# Redis

> Package: [MassTransit.Redis](https://www.nuget.org/packages/MassTransit.Redis)

Redis is a very popular key-value store, which is known for being very fast. To support Redis, MassTransit uses the `StackExchange.Redis` library.

::: warning
Redis only supports event correlation by _CorrelationId_, it does not support queries. If a saga uses expression-based correlation, a _NotImplementedByDesignException_ will be thrown.
:::

To use Redis, an additional interface and property are required on saga instances.

```cs {10}
public class OrderState :
    SagaStateMachineInstance,
    IVersionedSaga
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }

    public int Version { get; set; }
}
```

### Container Integration

To configure Redis as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension. This will configure Redis to connect to the local Redis instance on the default port using Optimistic concurrency. This will also store the _ConnectionMultiplexer_ in the container as a single instance, which will be disposed by the container.

```cs {6}
container.AddMassTransit(cfg =>
{
    const string configurationString = "127.0.0.1";

    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .RedisRepository(configurationString);
});
```

All configuration options are shown below, in cases where additional settings are required.

```cs
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .RedisRepository(r =>
        {
            // Default is Optimistic
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;

            // Connect to Redis on the local host using the default port
            r.DatabaseConfiguration("127.0.0.1");

            // Optional, prefix each saga instance key with the string specified
            // resulting dev:c6cfd285-80b2-4c12-bcd3-56a00d994736
            r.KeyPrefix = "dev";

            // Optional, to customize the lock key
            r.LockSuffix = "-lockage";

            // Optional, the default is 30 seconds
            r.LockTimeout = TimeSpan.FromMinutes(1);
        });
});
```

The container extension will register the saga repository in the container. For more details on container configuration, review the [container configuration](/usage/containers/) section of the documentation.

### Concurrency

Redis supports both Optimistic (default) and Pessimistic concurrency.

In optimistic mode, the saga instance is not locked before reading, which can ultimately lead to a write conflict if the instance was updated by another message. The _Version_ property is used to compare that the update would not overwrite a previous update. It is recommended that a retry policy is configured (using `UseMessageRetry`, see the [exceptions](/usage/exceptions.md#retry) documentation).

Pessimistic concurrency uses the Redis lock mechanism. During the message processing, the repository will lock the saga instance before reading it, so that any concurrent attempts to lock the same instance will wait until the current message has completed or the lock timeout expires.
