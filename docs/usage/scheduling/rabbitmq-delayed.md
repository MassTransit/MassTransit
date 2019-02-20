# RabbitMQ Delayed Exchange

RabbitMQ has no native support for deferred dispatch. However, the community has developed a plugin that makes such functionality possible, enabling native support for message scheduling for RabbitMQ.

This plugin is still considered as experimental. It is supported by MassTransit, but we cannot guarantee anything more than the plugin guarantees itself. Please read the message on the plugin [project home page](1) to get full understanding of the current status and limitations.

The plugin is not included in the RabbitMQ distribution. Please follow the instructions on the [project wiki](1) to install it.

## Scheduling messages

You can use the delayed exchange to scheduled messages instead of Quartz. To enable this, you need to configure your bus properly:

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.UseDelayedExchangeMessageScheduler();

    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
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

Read more about message redelivery in [Redelivering messages](redeliver.md)

[1]: https://github.com/rabbitmq/rabbitmq-delayed-message-exchange/