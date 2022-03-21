---
prev: false
next: /usage/configuration
sidebarDepth: 0
---

# Azure Service Bus

> This tutorial will get you from zero to up and running with [Azure Service Bus](/usage/transports/azure-sb) and MassTransit. 

<iframe id="ytplayer" type="text/html" width="640" height="360"
  src="https://www.youtube.com/embed/8marp1oyY_I?autoplay=0">
</iframe>

## Prerequisites

::: tip NOTE
The following instructions assume you are starting from a completed [In-Memory Quick Start](/quick-starts/in-memory)
:::

This example requires the following:

- a functioning installation of the dotnet runtime and sdk (at least 6.0)
- an Azure account, where you have administrative control


## Setup Azure Service Bus

::: tip
To continue from this point, you must have a valid Azure subscription with an Azure Service Bus namespace. A shared access policy with _Manage_ permissions is required to use MassTransit with Azure Service Bus.
:::

1. Navigate to [Service Bus](https://portal.azure.com/#create/Microsoft.ServiceBus)
2. Create a namespace
      1. **Pricing Tier:** This _must_ be Standard or Premium
3. Create a **Shared access policy**
      1. Make sure to grant `Manage`
      2. The `Primary Connection String` will be used for the rest of the steps.

## Change the Transport to Azure Service Bus

Add the _MassTransit.Azure.ServiceBus.Core_ package to the project.

```bash
$ dotnet add package MassTransit.Azure.ServiceBus.Core
```

## Edit Program.cs

Change `UsingInMemory` to `UsingAzureServiceBus`.

```csharp {8-13}
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                // elided ...
                x.UsingAzureServiceBus((context,cfg) =>
                {
                    cfg.Host("your connection string");

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddHostedService<Worker>();
        });
```

## Run the project

```bash
$ dotnet run
```

The output should have changed to show the message consumer generating the output (again, press Control+C to exit). Notice that the bus address now starts with `sb`.

``` {11}
Building...
info: MassTransit[0]
      Configured endpoint Message, Consumer: GettingStarted.MessageConsumer
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/chris/Garbage/start/GettingStarted
info: MassTransit[0]
      Bus started: sb://your-service-bus-namespace/
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:11:10 PM -05:00
```

At this point, the service is connecting to Azure Service Bus and publishing messages which are received by the consumer.

:tada: