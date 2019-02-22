# Redelivering messages

> This page may be removed in the future, for more detail on handling exceptions is available at the [Handling exceptions](usage/exception.md) page.

There are situations where a message cannot be processed, either due to an unavailable resource or a situation in which message ordering is important (you should try not to depend upon message order, but sometimes it is an easy workaround). In these situations, scheduling a message for redelivery is a powerful tool.

## Retry using scheduled redelivery

Handling exceptions (described in detail on the page linked above) includes the ability to use scheduled redelivery for longer retry delays when immediate or short delay retries fail.

## Explicit message redelivery

MassTransit makes it easy to schedule messages for redelivery. In the example below, the Quartz service is running as a separate service on the */quartz* queue.

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

It is also possible to use RabbitMQ Delayed Exchange to redeliver messages. You can even use it if the delayed exchange is not [configured](rabbitmq-delayed.md) as a default message scheduler.

You can schedule a message to be redelivered via RabbitMQ delayed exchange from a consumer by using the `Defer` extension method like this:

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


The redelivered message includes two additional message headers:

#### MT-Redelivery-Count
  The number of redelivery attempts the message has had. The first attempt is number 1.

#### MT-Scheduling-DeliveredAddress
  The address where the message was last delivered and subsequently scheduled for redelivery.
