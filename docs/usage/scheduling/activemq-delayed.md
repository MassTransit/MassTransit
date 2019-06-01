# ActiveMQ Delay and Schedule Message Delivery

ActiveMQ has a native support for deferred dispatch. It is supported by MassTransit, but we cannot guarantee anything more than the feature guarantees itself. Please read the message on the plugin [project home page](1) to get full understanding of the current status and limitations.

## Scheduling messages

You can use the delayed exchange to scheduled messages instead of Quartz. To enable this, you need to configure your bus properly:

```csharp
var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
{
    cfg.UseDelayedExchangeMessageScheduler();

    var host = cfg.Host(new Uri("activemq://localhost/"), h =>
    {
        h.Username("admin");
        h.Password("admin");
    });
}
```

**Important:** there is no support for unscheduling messages that are scheduled with the delayed exchange.

## Redelivery

You also can use the delayed exchange to implement message redelivery (second-level retries). All you need to do is to call the `context.Defer(TimeSpan delay)` method in your consumer.

```csharp
public async Task Consume(ConsumeContext<MyMessage> context)
{
    try
    {
        // process message
    }
    catch (MyTemporaryException e)
    {
        // we will try again later
        context.Defer(TimeSpan.FromSeconds(10));
    }
}
```

[1]: https://activemq.apache.org/delay-and-schedule-message-delivery
