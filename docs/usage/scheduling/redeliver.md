# Redelivering messages

There are situations where a message cannot be processed, either due to an unavailable resource or a situation
in which message ordering is important (you should try not to depend upon message order, but sometimes it is an
easy workaround). In these situations, scheduling a message for redelivery is a powerful tool.

> Sometimes this behavior is referred to as a *Second Level Retry*.

## Explicitly specifying redelivery

MassTransit makes it easy to schedule messages for redelivery. In the example below, the Quartz service is running
as a separate service on the */quartz* queue.

```csharp
public class UpdateCustomerAddressConsumer :
    IConsumer<UpdateCustomerAddress>
{
    public async Task Consume(ConsumeContext<ScheduleNotification> context)
    {
        try
        {
            // try to update the database
        }
        catch (CustomerNotFoundException exception)
        {
            // schedule redelivery in one minute
            context.Redeliver(TimeSpan.FromMinutes(1));
        }
    }
}
```

To enable the `Redeliver` method, the Quartz endpoint must be setup on the bus.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));
});
```

### Redelivery with RabbitMQ

It is also possible to use RabbitMQ Delayed Exchange to redeliver messages.
You can even use it if the delayed exchange is not [configured](rabbitmq-delayed.md)
as a default message scheduler.

You can schedule a message to be redelivered via RabbitMQ delayed exchange
from a consumer by using the `Defer` extension method like this:

```csharp
public async Task Consume(ConsumeContext<ScheduleNotification> context)
{
    try
    {
        // try to update the database
    }
    catch (CustomerNotFoundException exception)
    {
        // schedule redelivery in one minute
        context.Defer(TimeSpan.FromMinutes(1));
    }
}
```

## Combining retries and redelivery

MassTransit allows you to combine first-level retries (see [Retries](../retries.md))
and second-level retries (Redelivery). In this case you do not have to explicitly
instruct MassTransit to redeliver the message, instead you configure your consumers
in a way that they apply extended retry policy to certain messages.

At this moment, you have to configure this per handler or per consumer and message type.

To use the default scheduler, the consumer pipeline needs to be configured like this:

```csharp
cfg.ReceiveEndpoint(host, "endpoint_queue", e =>
{
    e.Consumer<MyConsumerClass>(c =>
    {
        c.Message<MyMessage>(x => x.UseScheduledRedelivery(
            Retry.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10))));
    });
});
```

and to apply such configuration to handler delegates:

```csharp
cfg.ReceiveEndpoint(host, "endpoint_queue", e =>
{
    e.Handler<MyMessage>(c =>
    {
        // handling the message
    }, x => x.UseScheduledRedelivery(
        Retry.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10))));
});
```

`UseScheduledRedelivery` methods accepts a retry policy or a retry policy configuration delegate
as the only parameter.

Again, it is also possible to use RabbitMQ delayed exchange instead of the default scheduler.
In such case, instead of using `UseScheduledRedelivery` you need to use `UseDelayedRedelivery`.

The redelivered message includes two additional message headers:

#### MT-Redelivery-Count
  The number of redelivery attempts the message has had. The first attempt is number 1.

#### MT-Scheduling-DeliveredAddress
  The address where the message was last delivered and subsequently scheduled for redelivery.
