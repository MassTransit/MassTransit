# Azure Service Bus

```csharp
Bus.Factory.CreateUsingAzureServiceBus(x =>
{
    var host = x.Host(serviceUri, h =>
    {
        h.SharedAccessSignature(s =>
        {
            s.KeyName = "keyName";
            s.SharedAccessKey = "key";
            s.TokenTimeToLive = TimeSpan.FromDays(1);
            s.TokenScope = TokenScope.Namespace;
        });
    });
});
```

## Azure Functions

Azure Functions is a consumption-based approach to compute which eliminates the need to have a service constantly running, particularly when there isn't any work to be done. Because of this, it's becoming popular for services with low utilization.

MassTransit supports Azure Service Bus and Azure Event Hub when running from an Azure Function.

> The [Sample Code](https://github.com/MassTransit/MassTransit/tree/develop/src/Samples/Sample.AzureFunctions.ServiceBus) is available
for reference as well.

The functions [host.json](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-service-bus#host-json) file needs to have messageHandlerOptions > autoComplete set to true. This is so that the message is removed from the queue once processing has completed successfully.

```
{
  "version": "2.0",
  "extensions": {
    "serviceBus": {
      "prefetchCount": 20,
      "messageHandlerOptions": {
        "autoComplete": true,
        "maxConcurrentCalls": 20,
        "maxAutoRenewDuration": "00:55:00"
      }
    }
  },
}
```

### Azure Service Bus

The bindings for using MassTransit with Azure Service Bus are shown below.

```csharp
[FunctionName("SubmitOrder")]
public static Task SubmitOrderAsync([ServiceBusTrigger("input-queue")] Message message, IBinder binder, Microsoft.Extensions.Logging.ILogger logger,
    CancellationToken cancellationToken)
{
    LogContext.ConfigureCurrentLogContext(logger);

    LogContext.Info?.Log("Creating brokered message receiver");

    var handler = Bus.Factory.CreateBrokeredMessageReceiver(binder, cfg =>
    {
        cfg.CancellationToken = cancellationToken;
        cfg.InputAddress = new Uri("sb://<namespace>.servicebus.windows.net/<input-queue-name>");

        cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
        cfg.Consumer(() => new SubmitOrderConsumer());
    });

    return handler.Handle(message);
}
```

### Event Hub

The bindings for using MassTransit with Azure Event Hub are shown below.

```csharp
[FunctionName("AuditOrder")]
public static Task AuditOrderAsync([EventHubTrigger("input-hub")] EventData message, IBinder binder, Microsoft.Extensions.Logging.ILogger logger,
    CancellationToken cancellationToken)
{
    LogContext.ConfigureCurrentLogContext(logger);

    LogContext.Info?.Log("Creating event hub receiver");

    var handler = Bus.Factory.CreateEventDataReceiver(binder, cfg =>
    {
        cfg.CancellationToken = cancellationToken;
        cfg.InputAddress = new Uri("sb://<namespace>.servicebus.windows.net/<input-hub-name>");

        cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
        cfg.Consumer(() => new AuditOrderConsumer());
    });

    return handler.Handle(message);
}
```

