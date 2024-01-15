# Observability

## Monitoring

### Open Telemetry

OpenTelemetry is an open-source standard for distributed tracing, which allows you to collect and analyze data about the performance of your systems. MassTransit can be configured to use OpenTelemetry to instrument message handling, so that you can collect telemetry data about messages as they flow through your system.

By using OpenTelemetry with MassTransit, you can gain insights into the performance of your systems, which can help you to identify and troubleshoot issues, and to improve the overall performance of your application.

There is a good set of examples [opentelemetry-dotnet](https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/examples) how it can be used for different cases

#### Tracing
##### ASP.NET Core application
This example is using following packages:
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Exporter.Console`

```csharp
var builder = WebApplication.CreateBuilder(args);

void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

builder.Services.AddOpenTelemetry()
    .ConfigureResource(ConfigureResource)
    .WithTracing(b => b
        .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
        .AddConsoleExporter() // Any OTEL suportable exporter can be used here
    );
```

##### Console application
This example is using following packages:
- `OpenTelemetry`
- `OpenTelemetry.Exporter.Console`

```csharp
void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

Sdk.CreateTracerProviderBuilder()
    .ConfigureResource(ConfigureResource)
    .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
    .AddConsoleExporter() // Any OTEL suportable exporter can be used here
    .Build()
```

That's it you application will start exporting MassTransit related traces within your application

#### Metrics
##### ASP.NET Core application
This example is using following packages:
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Exporter.Console`

```csharp
var builder = WebApplication.CreateBuilder(args);

void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

builder.Services.AddOpenTelemetry()
    .ConfigureResource(ConfigureResource)
    .WithMetrics(b => b
        .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
        .AddConsoleExporter() // Any OTEL suportable exporter can be used here
    );
```

##### Console application
This example is using following packages:
- `OpenTelemetry`
- `OpenTelemetry.Exporter.Console`

```csharp
void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

Sdk.CreateTracerProviderBuilder()
    .ConfigureResource(ConfigureResource)
    .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
    .AddConsoleExporter() // Any OTEL suportable exporter can be used here
    .Build()
```

The OpenTelemetry metrics captured by MassTransit:

`Counters`

| Name                                         | Description                                 |
|:---------------------------------------------|:--------------------------------------------|
| messaging.masstransit.receive                | Number of messages received                 |
| messaging.masstransit.receive.errors         | Number of messages receive faults           |
| messaging.masstransit.consume                | Number of messages consumed                 |
| messaging.masstransit.consume.errors         | Number of message consume faults            |
| messaging.masstransit.saga                   | Number of messages processed by saga        |
| messaging.masstransit.saga.errors            | Number of message faults by saga            |
| messaging.masstransit.consume.retries        | Number of message consume retries           |
| messaging.masstransit.handler                | Number of messages handled                  |
| messaging.masstransit.handler.errors         | Number of message handler faults            |
| messaging.masstransit.outbox.delivery        | Number of messages delivered by outbox      |
| messaging.masstransit.outbox.delivery.errors | Number of message delivery faults by outbox |
| messaging.masstransit.send                   | Number of messages sent                     |
| messaging.masstransit.send.errors            | Number of message send faults               |
| messaging.masstransit.outbox.send            | Number of messages sent to outbox           |
| messaging.masstransit.outbox.send.errors     | Number of message send faults to outbox     |
| messaging.masstransit.execute                | Number of activities executed               |
| messaging.masstransit.execute.errors         | Number of activity execution faults         |
| messaging.masstransit.compensate             | Number of activities compensated            |
| messaging.masstransit.compensate.errors      | Number of activity compensation failures    |

`Gauges`

| Name                                    | Description                                             |
|:----------------------------------------|:--------------------------------------------------------|
| messaging.masstransit.receive.active    | Number of messages being received                       |
| messaging.masstransit.consume.active    | Number of consumers in progress                         |
| messaging.masstransit.execute.active    | Number of activity executions in progress               |
| messaging.masstransit.compensate.active | Number of activity compensations in progress            |
| messaging.masstransit.handler.active    | Number of handlers in progress                          |
| messaging.masstransit.saga.active       | Number of sagas in progress                             |

`Histograms`

| Name                                      | Description                                                                        |
|:------------------------------------------|:-----------------------------------------------------------------------------------|
| messaging.masstransit.receive.duration    | Elapsed time spent receiving a message, in millis                                  |
| messaging.masstransit.consume.duration    | Elapsed time spent consuming a message, in millis                                  |
| messaging.masstransit.saga.duration       | Elapsed time spent saga processing a message, in millis                            |
| messaging.masstransit.handler.duration    | Elapsed time spent handler processing a message, in millis                         |
| messaging.masstransit.delivery.durations  | Elapsed time between when the message was sent and when it was consumed, in millis |
| messaging.masstransit.execute.duration    | Elapsed time spent executing an activity, in millis                                |
| messaging.masstransit.compensate.duration | Elapsed time spent compensating an activity, in millis                             |


`Labels`

| Name                                 | Description                                         |
|:-------------------------------------|:----------------------------------------------------|
| messaging.masstransit.service        | The service name specified at bus configuration     |
| messaging.masstransit.destination    | The endpoint address                                |
| messaging.masstransit.message_type   | The message type for the metric                     |
| messaging.masstransit.consumer_type  | The consumer, saga, or activity type for the metric |
| messaging.masstransit.activity_type  | The activity name                                   |
| messaging.masstransit.argument_type  | The activity execute argument type                  |
| messaging.masstransit.log_type       | The activity compensate log type                    |
| messaging.masstransit.exception_type | The exception type for a fault metric               |
| messaging.masstransit.bus            | The bus instance                                    |
| messaging.masstransit.endpoint       | The receive endpoint                                |

Metric names and labels can be configured with `Options`:

```csharp
services.Configure<InstrumentationOptions>(options =>
{
    // Configure
});
```

### Application Insights
Azure Monitor has direct integration with Open Telemetry: 
- [OpenTelemetry overview](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-overview)
- [Enable Azure Monitor OpenTelemetry for .NET](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=net)

##### ASP.NET Core application
This example is using following packages:
- `OpenTelemetry.Extensions.Hosting`
- `Azure.Monitor.OpenTelemetry.Exporter`

```csharp
var builder = WebApplication.CreateBuilder(args);

void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

builder.Services.AddOpenTelemetry()
    .ConfigureResource(ConfigureResource)
    .WithTracing(b => b
        .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
        .AddAzureMonitorTraceExporter(
        {
            o.ConnectionString = "<Your Connection String>";
        }))
    .WithMetrics(b => b
        .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
        .AddAzureMonitorMetricExporter(o =>
        {
            o.ConnectionString = "<Your Connection String>";
        }));
```

##### Console application
This example is using following packages:
- `OpenTelemetry`
- `Azure.Monitor.OpenTelemetry.Exporter`

```csharp
void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

Sdk.CreateTracerProviderBuilder()
    .ConfigureResource(ConfigureResource)
    .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
    .AddAzureMonitorTraceExporter(
    {
        o.ConnectionString = "<Your Connection String>";
    })
    .Build();

Sdk.CreateTracerProviderBuilder()
    .ConfigureResource(ConfigureResource)
    .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
    .AddAzureMonitorMetricExporter(o =>
    {
        o.ConnectionString = "<Your Connection String>";
    })
    .Build()
```
You can also refer to the sample: [Sample-ApplicationInsights](https://github.com/MassTransit/Sample-ApplicationInsights) 

### Prometheus

::alert{type="info"}
The direct integration to Prometheus has been deprecated. Use the Open Telemetry integration instead.
::

Open Telemetry is more preferable choice of integration

#### Open Telemetry integration

This example is using following packages:
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Exporter.Prometheus`
- `OpenTelemetry.Exporter.Prometheus.AspNetCore`

```csharp
void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

builder.Services.AddOpenTelemetry()
    .ConfigureResource(ConfigureResource)
    .WithMetrics(b => b
        .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
        .AddPrometheusExporter()
    );
    
var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint(); // Map prometheus metrics endpoint
```
In case you want to migrate from direct integration to using Open Telemetry, and use previous metric names, just configure them through `Options`:
```csharp
builder.Services.Configure<InstrumentationOptions>(options =>
{
    ReceiveTotal = "mt.receive.total";
    // Configure other names by using similar approach
});
```

## Lifetime Observers

MassTransit supports several message observers allowing received, consumed, sent, and published messages to be monitored. There is a bus observer as well, so that the bus life cycle can be monitored.

::alert{type="warning"}
Observers should not be used to modify or intercept messages. To intercept messages to add headers or modify message content, create a new or use an existing middleware component.
::

### Bus

To observe bus life cycle events, create a class which implements `IBusObserver`. To configure a bus observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation.

```csharp
services.AddBusObserver<BusObserver>();
```

```csharp
services.AddBusObserver(provider => new BusObserver());
```

### Receive Endpoint

To configure a receive endpoint observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation.

```csharp
services.AddReceiveEndpointObserver<ReceiveEndpointObserver>();
```

```csharp
services.AddReceiveEndpointObserver(provider => new ReceiveEndpointObserver());
```

## Pipeline Observers

### Receive

To observe messages as they are received by the transport, create a class that implements the `IReceiveObserver` interface, and connect it to the bus as shown below.

To configure a receive observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation. When a container is not being used, the `ConnectReceiveObserver` bus method can be used instead.

```csharp
services.AddReceiveObserver<ReceiveObserver>();
```

```csharp
services.AddReceiveObserver(provider => new ReceiveObserver());
```

### Consume

If the `ReceiveContext` isn't fascinating enough for you, perhaps the actual consumption of messages might float your boat. A consume observer implements the `IConsumeObserver` interface, as shown below.

To configure a consume observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation. When a container is not being used, the `ConnectConsumeObserver` bus method can be used instead.

```csharp
services.AddConsumeObserver<ConsumeObserver>();
```

```csharp
services.AddConsumeObserver(provider => new ConsumeObserver());
```

#### Consume Message

Okay, so it's obvious that if you've read this far you want a more specific observer, one that only is called when a specific message type is consumed. We have you covered there too, as shown below.

To connect the observer, use the `ConnectConsumeMessageObserver` method before starting the bus.

> The `ConsumeMessageObserver<T>` interface may be deprecated at some point, it's sort of a legacy observer that isn't recommended.

### Send

Okay, so, incoming messages are not your thing. We get it, you're all about what goes out. It's cool. It's better to send than to receive. Or is that give? Anyway, a send observer is also available.

To configure a send observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer
creation. When a container is not being used, the `ConnectSendObserver` bus method can be used instead.

```csharp
services.AddSendObserver<SendObserver>();
```

```csharp
services.AddSendObserver(provider => new SendObserver());
```

### Publish

In addition to send, publish is also observable. Because the semantics matter, absolutely. Using the MessageId to link them up as it's unique for each message. Remember that Publish and Send are two distinct operations so if you want to observe all messages that are leaving your service, you have to connect both Publish and Send observers.

To configure a public observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer
creation. When a container is not being used, the `ConnectPublishObserver` bus method can be used instead.

```csharp
services.AddPublishObserver<PublishObserver>();
```

```csharp
services.AddPublishObserver(provider => new PublishObserver());
```

## State Machine Observers

### Event

To observe events consumed by a saga state machine, use an `IEventObserver<T>` where `T` is the saga instance type.

To configure an event observer, add it to the container using one of the methods shown below. The factory method version allows customization of the
observer creation.

```csharp
services.AddEventObserver<T, EventObserver<T>>();
```

```csharp
services.AddEventObserver<T>(provider => new EventObserver<T>());
```

### State

To observe state changes that happen in a saga state machine, use an `IStateObserver<T>` where `T` is the saga instance type.

To configure a state observer, add it to the container using one of the methods shown below. The factory method version allows customization of the
observer creation.

```csharp
services.AddStateObserver<T, StateObserver<T>>();
```

```csharp
services.AddStateObserver<T>(provider => new StateObserver<T>());
```

