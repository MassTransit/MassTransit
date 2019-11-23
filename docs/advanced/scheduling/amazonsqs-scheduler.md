# Amazon SQS Scheduler

Amazon SQS includes a _DelaySeconds_ property, which can be used to defer message delivery. MassTransit can use this feature to provide _scheduled_ message delivery.

### Scheduling messages

To enable the Amazon SQS message scheduler:

```cs {3}
var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
{
    cfg.UseAmazonSqsMessageScheduler();

    cfg.Host("region", hc =>
    {
        hc.Credentials(...);
    });
}
```

::: warning
Unscheduling messages is not supported using Amazon SQS delayed delivery.
:::

### Redelivery

Amazon SQS delayed delivery can be used with the `UseScheduledRedelivery` feature, which is explained in the [exceptions](/usage/exceptions.md#redelivery) section.
