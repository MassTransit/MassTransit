# Azure Service Bus

```csharp
var busControl = Bus.Factory.CreateUsingAzureServiceBus(x =>
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

Azure Functions is a consumption-based compute solution that only runs code when there is work to be done. MassTransit supports Azure Service Bus and Azure Event Hubs when running as an Azure Function.

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

::: warning
Azure Functions using Azure Service Bus or Azure Event Hubs will not configure the broker topology. When using Azure Functions, the broker topology must be configured prior to function deployment.
:::

### Azure Service Bus

The bindings for using MassTransit with Azure Service Bus are shown below.

```csharp
[FunctionName("SubmitOrder")]
public static Task SubmitOrderAsync([ServiceBusTrigger("input-queue")] Message message, 
    Binder binder, ILogger logger, CancellationToken cancellationToken)
{
    var receiver = binder.CreateBrokeredMessageReceiver(logger, cancellationToken, cfg =>
    {
        cfg.InputAddress = new Uri("sb://masstransit-build.servicebus.windows.net/input-queue");

        cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
        cfg.Consumer(() => new SubmitOrderConsumer());
    });

    return receiver.Handle(message);
}
```

### Event Hub

The bindings for using MassTransit with Azure Event Hub are shown below.

```csharp
[FunctionName("AuditOrder")]
public static Task AuditOrderAsync([EventHubTrigger("input-hub")] EventData message,
    IBinder binder, ILogger logger, CancellationToken cancellationToken)
{
    var receiver = binder.CreateEventDataReceiver(logger, cancellationToken, cfg =>
    {
        cfg.InputAddress = new Uri("sb://masstransit-eventhub.servicebus.windows.net/input-hub");

        cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
        cfg.Consumer(() => new AuditOrderConsumer());
    });

    return receiver.Handle(message);
}
```

