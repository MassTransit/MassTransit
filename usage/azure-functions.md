# Using MassTransit with Azure Functions

Azure Functions is a consumption-based approach to compute which eliminates the need to have a service constantly running, particularly when there isn't any work to be done. Because of this, it's becoming popular for services with low utilization.

MassTransit supports Azure Service Bus and Azure Event Hub when running from an Azure Function.

> The [Sample Code](https://github.com/MassTransit/MassTransit/tree/develop/src/Samples/Sample.AzureFunctions.ServiceBus) is available
for reference as well.

## Azure Service Bus

The bindings for using MassTransit with Azure Service Bus are shown below.

```csharp
[FunctionName("SubmitOrder")]
public static Task SubmitOrderAsync([ServiceBusTrigger("<input-queue-name>", AccessRights.Manage)]
    BrokeredMessage message, IBinder binder,
    TraceWriter traceWriter, CancellationToken cancellationToken)
{
    var handler = Bus.Factory.CreateBrokeredMessageReceiver(binder, cfg =>
    {
        cfg.CancellationToken = cancellationToken;
        cfg.SetLog(traceWriter);
        cfg.InputAddress = new Uri("sb://<namespace>.servicebus.windows.net/<input-queue-name>");

        cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
        cfg.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(cfg.Log));
    });

    return handler.Handle(message);
}
```

## Event Hub

The bindings for using MassTransit with Azure Event Hub are shown below.

```csharp
[FunctionName("AuditOrder")]
public static Task AuditOrderAsync([EventHubTrigger("<input-hub-name>")] EventData message, IBinder binder,
    TraceWriter traceWriter, CancellationToken cancellationToken)
{
    traceWriter.Info("Creating EventHub receiver");

    var handler = Bus.Factory.CreateEventDataReceiver(binder, cfg =>
    {
        cfg.CancellationToken = cancellationToken;
        cfg.SetLog(traceWriter);
        cfg.InputAddress = new Uri("sb://<namespace>.servicebus.windows.net/<input-hub-name>");

        cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
        cfg.Consumer<AuditOrderConsumer>(() => new AuditOrderConsumer(cfg.Log));
    });

    return handler.Handle(message);
}
```
