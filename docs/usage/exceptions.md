# Exceptions

Let's face it, bad things happen. Networks partition, servers crash, remote endpoints become non-responsive. And when bad things happen, exceptions get thrown. And when exceptions get thrown, people die. Okay, maybe that's a bit dramatic, but the point is, exceptions are a fact of software development.

Fortunately, MassTransit provides a number of features to help your application recover from and deal with exceptions. But before getting into that, an understanding of what happens when a message is consumed is needed.

Take, for example, a consumer that simply throws an exception.

```cs
public class SubmitOrderConsumer :
    IConsumer<SubmitOrder>
{
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        throw new Exception("Very bad things happened");
    }
}
```

When a message is delivered to the consumer, the consumer throws an exception. With a default bus configuration, the exception is caught by middleware in the transport (the `ErrorTransportFilter` to be exact), and the message is moved to an *_error* queue (prefixed by the receive endpoint queue name). The exception details are stored as headers with the message for analysis and to assist in troubleshooting the exception.

::: tip Video
Learn about the error queue in [this short video](https://youtu.be/3TMKUu7c4lc).
:::

> In addition to moving the message to an error queue, MassTransit also produces a `Fault<T>` event. See below for more details on _faults_.

## Retry

Some exceptions may be caused by a transient condition, such as a database deadlock, a busy web service, or some similar type of situation which usually clears up on a second attempt. With these exception types, it is often desirable to retry the message delivery to the consumer, allowing the consumer to try the operation again.

```cs
public class SubmitOrderConsumer :
    IConsumer<SubmitOrder>
{
    ISessionFactory _sessionFactory;

    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        using(var session = _sessionFactory.OpenSession())
        using(var transaction = session.BeginTransaction())
        {
            var customer = session.Get<Customer>(context.Message.CustomerId);

            // continue with order processing

            transaction.Commit();
        }
    }
}
```

With this consumer, an `ADOException` can be thrown, say there is a deadlock or the SQL server is unavailable. In this case, the operation should be retried before moving the message to the error queue. This can be configured on the receive endpoint or the consumer. Shown below is a retry policy which attempts to deliver the message to a consumer five times before throwing the exception back up the pipeline.

```cs
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((context,cfg) =>
    {
        cfg.UseMessageRetry(r => r.Immediate(5));

        cfg.ConfigureEndpoints(context);
    });
});
```

The `UseMessageRetry` method is an extension method that configures a middleware filter, in this case the `RetryFilter`. There are a variety of retry policies available, which are detailed in the [section below](#retry-configuration).

::: tip 
In this example, the <i>UseMessageRetry</i> is at the bus level, and will be configured on every receive endpoint. Additional retry filters can be added at the bus and consumer level, providing flexibility in how different consumers, messages, etc. are retried.
:::

To configure retry on a manually configured receive endpoint:

```cs
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("submit-order", e =>
        {
            e.UseMessageRetry(r => r.Immediate(5));

            e.ConfigureConsumer<SubmitOrderConsumer>(context);
        });
    });
});
```

MassTransit retry filters execute in memory and maintain a _lock_ on the message. As such, they should only be used to handle short, transient error conditions. Setting a retry interval of an hour would fall into the category of _bad things_. To retry messages after longer waits, look at the next section on redelivering messages.

## Retry Configuration

::: tip Video
Learn how to configure message retry in [this short video](https://youtu.be/pKxf6Ii-3ow).
:::

When configuring message retry, there are several retry policies available, including:

| Policy | Description |
| :-- | :-- |
| None          | No retry
| Immediate     | Retry immediately, up to the retry limit
| Interval      | Retry after a fixed delay, up to the retry limit
| Intervals     | Retry after a delay, for each interval specified
| Exponential   | Retry after an exponentially increasing delay, up to the retry limit
| Incremental   | Retry after a steadily increasing delay, up to the retry limit

Each policy has configuration settings which specifies the expected behavior.

### Exception Filters

Sometimes you do not want to always retry, but instead only retry when some specific exception is thrown and fault for all other exceptions. To implement this, you can use an exception filter. Specify exception types using either the `Handle` or `Ignore` method. A filter can have either _Handle_ or _Ignore_ statements, combining them has unpredictable effects.

Both methods have two signatures:

1. Generic version `Handle<T>` and `Ignore<T>` where `T` must be derivate of  `System.Exception`. With no argument, all exceptions of specified type will be either handled or ignored. You can also specify a function argument that will filter exceptions further based on other parameters.

1. Non-generic version that needs one or more exception types as parameters. No further filtering is possible if this version is used.

You can use multiple calls to these methods to specify filters for multiple exception types:

```cs
e.UseMessageRetry(r => 
{
    r.Handle<ArgumentNullException>();
    r.Ignore(typeof(InvalidOperationException), typeof(InvalidCastException));
    r.Ignore<ArgumentException>(t => t.ParamName == "orderTotal");
});
```

You can also specify multiple retry policies for a single endpoint:

```cs
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("submit-order", e =>
        {
            e.UseMessageRetry(r => 
            {
                r.Immediate(5);
                r.Handle<DataException>(x => x.Message.Contains("SQL"));
            });

            e.ConfigureConsumer<SubmitOrderConsumer>(context, c => c.UseMessageRetry(r => 
            {
                r.Interval(10, TimeSpan.FromMilliseconds(200));
                r.Ignore<ArgumentNullException>();
                r.Ignore<DataException>(x => x.Message.Contains("SQL"));
            }));
        });
    });
});
```

In the above example, if the consumer throws an `ArgumentNullException` it won't be retried (because it would obvious fail again, most likely). If a `DataException` is thrown matching the filter expression, it wouldn't be handled by the second retry filter, but would be handled by the first retry filter.

## Redelivery

Some errors take a while to resolve, say a remote service is down or a SQL server has crashed. In these situations, it's best to dust off and nuke the site from orbit - at a much later time obviously. Redelivery is a form of retry (some refer to it as _second-level retry_) where the message is removed from the queue and then redelivered to the queue at a future time.

::: tip NOTE
In some frameworks, message redelivery is also referred to as second-level retry.
:::

To use delayed redelivery, ensure the transport is properly configured. RabbitMQ required a delayed-exchange plug-in, and ActiveMQ (non-Artemis) requires the scheduler to be enabled via the XML configuration.

```cs
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
        cfg.UseMessageRetry(r => r.Immediate(5));

        cfg.ConfigureEndpoints(context);
    });
});
```

Now, if the initial 5 immediate retries fail (the database is really, really down), the message will retry an additional three times after 5, 15, and 30 minutes. This could mean a total of 15 retry attempts (on top of the initial 4 attempts prior to the retry/redelivery filters taking control).

::: tip Scheduled Redelivery
MassTransit also supports scheduled redelivery using the `UseScheduledRedelivery` configuration method. Scheduled redelivery requires the use of a message scheduler, which can be configured to use the message transport or Quartz.NET/Hangfire. The configuration is similar, just ensure the scheduler is properly configured.
:::

## Outbox

If the consumer publishes events or sends messages (using `ConsumeContext`, which is provided via the `Consume` method on the consumer) and subsequently throws an exception, it isn't likely that those messages should still be published or sent. MassTransit provides an outbox to buffer those messages until the consumer completes successfully. If an exception is thrown, the buffered messages are discarded.

To configure the outbox with redelivery and retry:

```cs
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
        cfg.UseMessageRetry(r => r.Immediate(5));
        cfg.UseInMemoryOutbox();

        cfg.ConfigureEndpoints(context);
    });
});
```

### Configuring for a consumer or saga

If there are multiple consumers (or saga) on the same endpoint (which could potentially get you on the _naughty list_), and the retry/redelivery behavior should only apply to a specific consumer or saga, the same configuration can be applied specifically to the consumer or saga.

To configure a specific consumer.

```cs
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("submit-order", e =>
        {
            e.ConfigureConsumer<SubmitOrderConsumer>(context, c =>
            {
                c.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
                c.UseMessageRetry(r => r.Immediate(5));
                c.UseInMemoryOutbox();
            });
        });
    });
});
```

Sagas are configured in the same way, using the saga configurator.

## Faults

As shown above, MassTransit delivers messages to consumers by calling the _Consume_ method. When a message consumer throws an exception instead of returning normally, a `Fault<T>` is produced, which may be published or sent depending upon the context.

A `Fault<T>` is a generic message contract including the original message that caused the consumer to fail, as well as the `ExceptionInfo`, `HostInfo`, and the time of the exception.

```cs
public interface Fault<T>
    where T : class
{
    Guid FaultId { get; }
    Guid? FaultedMessageId { get; }
    DateTime Timestamp { get; }
    ExceptionInfo[] Exceptions { get; }
    HostInfo Host { get; }
    T Message { get; }
}
```

If the message headers specify a `FaultAddress`, the fault is sent directly to that address. If the _FaultAddress_ is not present, but a `ResponseAddress` is specified, the fault is sent to the response address. Otherwise, the fault is published, allowing any subscribed consumers to receive it.

### Consuming Faults

Developers may want to do something with faults, such as updating an operational dashboard. To observe faults separate of the consumer that caused the fault to be produced, a consumer can consume fault messages the same as any other message.

```csharp
public class DashboardFaultConsumer :
    IConsumer<Fault<SubmitOrder>>
{
    public async Task Consume(ConsumeContext<Fault<SubmitOrder>> context)
    {
        // update the dashboard
    }
}
```

Faults can also be observed by state machines when specified as an event:

```csharp
Event(() => SubmitOrderFaulted, x => x
    .CorrelateById(m => m.Message.Message.OrderId) // Fault<T> includes the original message
    .SelectId(m => m.Message.Message.OrderId));

public Event<Fault<SubmitOrder>> SubmitOrderFaulted { get; private set; }
```

## Error Pipe

By default, MassTransit will move faulted messages to the *_error* queue. This behavior can be customized for each receive endpoint.

To discard faulted messages so that they are _not_ moved to the *_error* queue:

```cs
cfg.ReceiveEndpoint("input-queue", ec =>
{
    ec.DiscardFaultedMessages();
});
```

Beyond that built-in customization, the individual filters can be added/configured as well. Shown below are the default filters, as an example.

> This is by default, do _NOT_ configure this unless you have a reason to change the behavior.

```cs
cfg.ReceiveEndpoint("input-queue", ec =>
{
    ec.ConfigureError(x =>
    {
        x.UseFilter(new GenerateFaultFilter());
        x.UseFilter(new ErrorTransportFilter());
    });
});
```

## Dead-Letter Pipe

By default, MassTransit will move skipped messages to the *_skipped* queue. This behavior can be customized for each receive endpoint.

> Skipped messages are messages that are read from the receive endpoint queue that do not have a matching handler, consumer, saga, etc. configured. For instance, receiving a _SubmitOrder_ message on a receive endpoint that only has a consumer for the _UpdateOrder_ message would cause that _SubmitOrder_ message to end up in the *_skipped* queue.

To discard skipped messages so they are _not_ moved to the *_skipped* queue:

```cs
cfg.ReceiveEndpoint("input-queue", ec =>
{
    ec.DiscardSkippedMessages();
});
```

Beyond that built-in customization, the individual filters can be added/configured as well. Shown below are the default filters, as an example.

> This is by default, do _NOT_ configure this unless you have a reason to change the behavior.

```cs
cfg.ReceiveEndpoint("input-queue", ec =>
{
    ec.ConfigureDeadLetter(x =>
    {
        x.UseFilter(new DeadLetterTransportFilter());
    });
});
```



