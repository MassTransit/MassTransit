# Configuring Retry

Errors happen, and when they do it's important to handle those errors gracefully. In a distributed system, transient errors can occur for a variety of reasons, such a network glitches, service restarts, etc. To handle those types of errors, MassTransit has the ability to retry messages.

To configure message retry on a receive endpoint:

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("input-queue", e =>
    {
        e.UseMessageRetry(r => r.Interval(3, 1000));

        e.Consumer<MyConsumer>();
    });
});
```

> The order matters, so make sure the `UseMessageRetry` statement is before any consumer statements.

In the above configuration, if `MyConsumer` throws an exception while processing a message, the message will be delivered to a new consumer again after one second, and that will repeat up to three times before generating a fault and moving the message to the error queue.

## Adding an outbox

If the consumer publishes events or sends messages (using `ConsumeContext`, which is provided via the `Consume` method on the consumer) and subsequently throws an exception, it isn't likely that those messages should still be published or sent. MassTransit provides an outbox to buffer those messages until the consumer completes successfully. If an exception is thrown, the buffered messages are discarded.

To configure the outbox:

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("input-queue", e =>
    {
        e.UseMessageRetry(r => r.Interval(3, 1000));
        e.UseInMemoryOutbox();

        e.Consumer<MyConsumer>();
    });
});
```

> Once again, order matters!

## Supported retry policies

Several retry policies are supported, including:

* None
* Immediate
* Intervals
* Exponential
* Incremental

Each policy has configuration settings which specifies the expected behavior.

## Retry filters

Sometimes you do not want to always retry, but instead only retry when some specific exception is thrown and fault for all other exceptions.

To implement this, you can use an exception filter.

### Available filters

* Handle - only process specified exceptions
* Ignore - ignore specified exceptions

Filters can be combined to narrow down a set of exception types that you need to apply the retry policy to.

Both filters exist in two version:

1. Generic version `Handle<T>` and `Ignore<T>` where `T` must be derivate of  `System.Exception`. With no argument, all exceptions of specified type will be either handled or ignored. You can also specify a function argument that will filter exceptions further based on other parameters.

1. Non-generic version that needs one or more exception types as parameters. No further filtering is possible if this version is used.

You can use multiple calls to these methods to specify filters for multiple exception types:

```csharp
e.UseMessageRetry(r => 
{
    r.Handle<ArgumentNullException>();
    r.Ignore(typeof(InvalidOperationException), typeof(InvalidCastException));
    r.Ignore<ArgumentException>(t => t.ParamName == "orderTotal");
});
```

You can also specify multiple retry policies for a single endpoint:

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("input-queue", e =>
    {
        e.UseMessageRetry(r => 
        {
            r.Immediate(5);
            r.Handle<DataException>(x => x.Message.Contains("SQL"));
        });
        e.Consumer<MyConsumer>(c => c.UseMessageRetry(r => 
            {
                r.Interval(10, TimeSpan.FromMilliseconds(200));
                r.Ignore<ArgumentNullException>();
                r.Ignore<DataException>(x => x.Message.Contains("SQL"));
            });
        );
    });
});
```

In the above example, if the consumer throws an `ArgumentNullException` it won't be retried (because it would obvious fail again, most likely). If a `DataException` is thrown matching the filter expression, it wouldn't be handled by the second retry filter, but would be handled by the first retry filter.

## Previous Retry Configurations

There are previous configuration of retry that are still supported. However, it's encouraged that the above methods be used to configure message-level retry.

## Retry policies

The retry policy can be specified at the bus level, as well as the endpoint and consumer level. The policy closest to the exception is the policy that is used.

To configure the default retry policy for the entire bus.

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.UseRetry(r => r.None());
});
```

To configure the retry policy for a receive endpoint.

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("inbound", ep =>
    {
        ep.UseRetry(r => r.Immediate(5));
    });
});
```

To configure the retry policy for a specific consumer on an endpoint.

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("inbound", ep =>
    {
        ep.Consumer<MyConsumer>(cc =>
        {
            cc.UseRetry(r => r.Interval(10, TimeSpan.FromMilliseconds(200)));
        });
    });
});
```

