# Azure Functions

Azure Functions is a consumption-based compute solution that only runs code when there is work to be done. MassTransit supports Azure Service Bus and Azure Event Hubs when running as an Azure Function.

> The [Sample Code](https://github.com/MassTransit/Sample-AzureFunction) is available for reference as well, which is based on the 8.0.0 version of MassTransit.

The functions [host.json](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-service-bus-trigger?tabs=csharp) file needs to have messageHandlerOptions > autoComplete set to true. If this isn't set to true, MassTransit will _try_ to set it to true for you. This is so that the message is acknowledged by the Azure Functions runtime, which removes it from the queue once processing has completed successfully.

```json
{
  "version": "2.0",
  "logging": {
    "applicationInsights": {
      "samplingSettings": {
        "isEnabled": true
      }
    },
    "logLevel": {
      "MassTransit": "Debug",
      "Sample.AzureFunctions.ServiceBus": "Information"
    }
  },
  "extensions": {
    "serviceBus": {
      "prefetchCount": 32,
      "messageHandlerOptions": {
        "autoComplete": true,
        "maxConcurrentCalls": 32,
        "maxAutoRenewDuration": "00:30:00"
      }
    },
    "eventHub": {
      "maxBatchSize": 64,
      "prefetchCount": 256,
      "batchCheckpointFrequency": 1
    }
  }
}
```

This settings for _prefetchCount_, _maxConcurrentCalls_, and _maxAutoRenewDuration_ are the most important – these will directly affect the performance of an Azure Function.

The function should include a Startup class, which is called on startup by the Azure Functions framework. The example below configures MassTransit, registers the consumers for use, and adds a scoped type for an Azure function class.

```cs
using MassTransit;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Sample.AzureFunctions.ServiceBus.Startup))]

namespace Sample.AzureFunctions.ServiceBus
{
    public class Startup :
        FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddScoped<SubmitOrderFunctions>() // add your functions as scoped
                .AddMassTransitForAzureFunctions(cfg =>
                {
                    cfg.AddConsumersFromNamespaceContaining<ConsumerNamespace>();
                });
        }
    }
}

```

::: tip
Azure Functions using Azure Service Bus or Azure Event Hubs require the queue, subscription, topic, or event hub to exist prior to starting the function service. If the messaging entity does not exist, the function will not be bound, and messages or events will not be delivered. Service Bus messages sent or published by MassTransit running inside an Azure Function will, however, create the appropriate topics and/or queues as needed.
:::

### Azure Service Bus

The bindings for using MassTransit with Azure Service Bus are shown below.

```csharp
using MassTransit.WebJobs.ServiceBusIntegration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;


public class SubmitOrderFunctions
{
    const string SubmitOrderQueueName = "input-queue";
    readonly IMessageReceiver _receiver;

    public SubmitOrderFunctions(IMessageReceiver receiver)
    {
        _receiver = receiver;
    }

    [FunctionName("SubmitOrder")]
    public Task SubmitOrderAsync([ServiceBusTrigger(SubmitOrderQueueName)]
        Message message, CancellationToken cancellationToken)
    {
        return _receiver.HandleConsumer<SubmitOrderConsumer>(SubmitOrderQueueName, message, cancellationToken);
    }
}
```

In the example above, the _HandleConsumer_ method is used to configure a specific consumer on the message receiver.

> Message receivers are cached by _entityName_. Once a message receiver has been used, the configuration cannot be changed. 

To configure the consumer pipeline, such as to add `UseMessageRetry` middleware, use a `ConsumerDefinition` for the consumer type.

### Event Hub

The bindings for using MassTransit with Azure Event Hub are shown below. In addition to the event hub name, the _Connection_ must also be specified.

> At least I think so, the documentation isn't great and I only found this approach when digging through the extension source code.

```csharp
using MassTransit.WebJobs.EventHubsIntegration;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;

public class AuditOrderFunctions
{
    const string AuditOrderEventHubName = "input-hub";
    readonly IEventReceiver _receiver;

    public AuditOrderFunctions(IEventReceiver receiver)
    {
        _receiver = receiver;
    }

    [FunctionName("AuditOrder")]
    public Task AuditOrderAsync([EventHubTrigger(AuditOrderEventHubName, Connection = "AzureWebJobsEventHub")]
        EventData message, CancellationToken cancellationToken)
    {
        return _receiver.HandleConsumer<AuditOrderConsumer>(AuditOrderEventHubName, message, cancellationToken);
    }
}
```

::: warning
With this refresh of the Azure Function code, it is no longer possible to send messages to other event hubs. Messages published or sent are done so using Azure Service Bus.
:::

### Testing Locally

To test locally, a settings files must be created. Connections strings for the various services, along with the Application Insights connection string, can be configured.

```js
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureWebJobsStorage": "",
    "AzureWebJobsServiceBus": "",
    "AzureWebJobsEventHub": "",
    "FUNCTIONS_EXTENSION_VERSION": "~4",
    "APPINSIGHTS_INSTRUMENTATIONKEY": "",
    "APPLICATIONINSIGHTS_CONNECTION_STRING": "InstrumentationKey="
  }
}

````

