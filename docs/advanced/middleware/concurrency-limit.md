# Concurrency Limit

By specifying a concurrent message limit, MassTransit limits the number of messages delivered to a consumer at the same time. At the same time, since a consumer factory is used to create consumers, it also limits the number of concurrent consumers that exist at the same time.

::: tip
The concurrent message limit applies to the total of all message types consumed by the consumer.
:::

::: tip
The `ConcurrentMessageLimit` is not initialized by default, and does not need to be specified. If no limit is specified, which is the default, it will equal the PrefetchCount.
:::

### Consumer

To add a concurrent message limit to a consumer:

```csharp
cfg.ReceiveEndpoint("submit-order", e =>
{
    e.Consumer<SubmitOrderConsumer>(cc =>
    {
        cc.UseConcurrentMessageLimit(2);
    });
});
```

### Saga

To add a concurrent message limit to a saga:

```csharp
cfg.ReceiveEndpoint("order-status", e =>
{
    e.Saga<OrderStatusSaga>(cc =>
    {
        cc.UseConcurrentMessageLimit(2);
    });
});
```

## Dynamically adjusting the concurrent message limit

The concurrent message limit can be dynamically adjusted using a management endpoint.

To add a concurrent message limit to a consumer, and support dynamic adjustment:

```csharp
var management = cfg.ManagementEndpoint();

cfg.ReceiveEndpoint("order-status", e =>
{
    e.Saga<OrderStatusSaga>(cc =>
    {
        cc.UseConcurrentMessageLimit(2, management, "order-status");
    });
});
```

To adjust the concurrent message limit:

```csharp
var client = bus.CreateRequestClient<SetConcurrencyLimit>();
var response = await client.GetResponse<ConcurrencyLimitUpdated>(new
{
    Id = "order-status",
    ConcurrencyLimit = 4,
    Timestamp = DateTime.UtcNow
});
```

## Legacy Concurrency Limit

The concurrency limit support built into GreenPipes supports any `PipeContext`, which means it can be used anywhere a filter can be added. It is still supported, however, the new syntax above is recommended.

To use the Green Pipes concurrency limit:

```csharp
cfg.ReceiveEndpoint("submit-order", e =>
{
    e.UseConcurrencyLimit(4);

    e.Consumer<SubmitOrderConsumer>();
});
```

Used this way, a single filter will be applied to each message type.


