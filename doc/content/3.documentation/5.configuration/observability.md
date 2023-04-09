# Observability

## Monitoring

### Open Telemetry

OpenTelemetry is an open-source standard for distributed tracing, which allows you to collect and analyze data about the performance of your systems. MassTransit can be configured to use OpenTelemetry to instrument message handling, so that you can collect telemetry data about messages as they flow through your system.

By using OpenTelemetry with MassTransit, you can gain insights into the performance of your systems, which can help you to identify and troubleshoot issues, and to improve the overall performance of your application.

#### RabbitMQ metrics instrumentation

To use Open Telemetry instrumentation you need to add configuration in `Program.cs`:
```csharp
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseInstrumentation();
    });
});
```

The OpenTelemetry metrics captured by MassTransit are listed below.

`Counters`

| Name                                    | Description                                                                          |
|:----------------------------------------|:-------------------------------------------------------------------------------------|
| messaging_masstransit_receive                       | Number of messages received                                                    |
| messaging_masstransit_receive_errors                  | Number of messages receive faults                                              |
| messaging_masstransit_consume            | Number of messages consumed                                    |
| messaging_masstransit_consume_errors                  | Number of message consume faults                                                   |
| messaging_masstransit_consume_retries                        | Number of message consume retries                                                   |
| messaging_masstransit_publish                 | Number of messages published                                               |
| messaging_masstransit_publish_errors                 | Number of message publish faults                                              |
| messaging_masstransit_send            | Number of messages sent                                   |
| messaging_masstransit_send_errors            | Number of message send faults |
| messaging_masstransit_execute                        | Number of activities executed                                                   |
| messaging_masstransit_execute_errors                  | Number of activity execution faults                                               |
| messaging_masstransit_compensate                           | Number of activities compensated                                                        |
| messaging_masstransit_compensate_errors                     | Number of activity compensation failures                                                  |

`Gauges`

| Name                                    | Description                                                                          |
|:----------------------------------------|:-------------------------------------------------------------------------------------|
| messaging_masstransit_receive_active                       | Number of messages being received                                                    |
| messaging_masstransit_consume_active                  | Number of consumers in progress                                              |
| messaging_masstransit_execute_active             | Number of activity executions in progress                                    |
| messaging_masstransit_compensate_active                  | Number of activity compensations in progress                                                    |
| messaging_masstransit_handler_active                        | Number of handlers in progress                                                    |
| messaging_masstransit_saga_active                  | Number of sagas in progress                                               |

`Histograms`

| Name                                    | Description                                                                          |
|:----------------------------------------|:-------------------------------------------------------------------------------------|
| messaging_masstransit_receive_duration                      | Elapsed time spent receiving a message, in seconds                                                    |
| messaging_masstransit_consume_duration                  | Elapsed time spent consuming a message, in seconds                                              |
| messaging_masstransit_delivery_durations             | Elapsed time between when the message was sent and when it was consumed, in seconds                                    |
| messaging_masstransit_execute_duration                  | Elapsed time spent executing an activity, in seconds                                                    |
| messaging_masstransit_compensate_duration                        | Elapsed time spent compensating an activity, in seconds                                                    |

### Application Insights

Application Insights (part of Azure Monitor) is able to capture and record metrics from MassTransit. It can also be configured as a log sink for logging.

[Create an Application Insights resource](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#create-an-application-insights-resource-1)

[Copy the instrumentation key](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#copy-the-instrumentation-key)

To configure an application to use Application Insights with MassTransit:

> Requires NuGets `MassTransit`, `Microsoft.ApplicationInsights.DependencyCollector`
>
> (for logging, add `Microsoft.Extensions.Logging.ApplicationInsights`)

```csharp
using System;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;

namespace Example
{
    public class MyMessageConsumerConsumer : 
        MassTransit.IConsumer<MyMessage>
    {
        public async Task Consume(ConsumeContext<MyMessage> context)
        {
             await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");
        }
    }

    // Message Definition
    public class MyMessage
    {
        public string Value { get; set; }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var module = new DependencyTrackingTelemetryModule();
            module.IncludeDiagnosticSourceActivities.Add("MassTransit");

            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = "<your instrumentation key>";
            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            var telemetryClient = new TelemetryClient(configuration);
            module.Initialize(configuration);

            var loggerOptions = new ApplicationInsightsLoggerOptions();

            var applicationInsightsLoggerProvider = new ApplicationInsightsLoggerProvider(Options.Create(configuration),
                Options.Create(loggerOptions));

            ILoggerFactory factory = new LoggerFactory();
            factory.AddProvider(applicationInsightsLoggerProvider);

            LogContext.ConfigureCurrentLogContext(factory);

            var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ReceiveEndpoint("my_queue", ec =>
                {
                    ec.Consumer<MyMessageConsumer>();
                });
            });

            using(busControl.StartAsync())
            {
                await busControl.Publish(new MyMessage{Value = "Hello, World."});

                await Task.Run(() => Console.ReadLine());
            }

            module.Dispose();

            telemetryClient.Flush();
            await Task.Delay(5000);

            configuration.Dispose();
        }
    }
}
```

### Prometheus

[![alt NuGet](https://img.shields.io/nuget/v/MassTransit.Prometheus.svg "NuGet")](https://nuget.org/packages/MassTransit.Prometheus/)

MassTransit supports Prometheus metric capture, which provides useful observability into the bus, endpoints, consumers, and messages.

> The `prometheus-net` library is used as the Prometheus client since it is mentioned on the Prometheus client list.

#### Installation

```bash
$ dotnet add package prometheus-net.AspNetCore
$ dotnet add package MassTransit.Prometheus
```

#### Configuration

To configure the bus to capture metrics, add the `UsePrometheusMetrics()` method to your bus configuration.

```csharp
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UsePrometheusMetrics(serviceName: "order_service");
    });
});
```

To then mount the metrics to `/metrics` go to your `Program.cs` and add

```csharp
app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
});
```

> For more details, see the [Prometheus-Net Documentation](https://github.com/prometheus-net/prometheus-net#aspnet-core-exporter-middleware).

#### Metrics Captured

The metrics captured by MassTransit are listed below.

| Name                                    | Description                                                                          |
|:----------------------------------------|:-------------------------------------------------------------------------------------|
| mt_receive_total                        | Total number of messages received                                                    |
| mt_receive_fault_total                  | Total number of messages receive faults                                              |
| mt_receive_duration_seconds             | Elapsed time spent receiving messages, in seconds                                    |
| mt_receive_in_progress                  | Number of messages being received                                                    |
| mt_consume_total                        | Total number of messages consumed                                                    |
| mt_consume_fault_total                  | Total number of message consume faults                                               |
| mt_consume_retry_total                  | Total number of message consume retries                                              |
| mt_consume_duration_seconds             | Elapsed time spent consuming a message, in seconds                                   |
| mt_delivery_duration_seconds            | Elapsed time between when the message was sent and when it was consumed, in seconds. |
| mt_publish_total                        | Total number of messages published                                                   |
| mt_publish_fault_total                  | Total number of message publish faults                                               |
| mt_send_total                           | Total number of messages sent                                                        |
| mt_send_fault_total                     | Total number of message send faults                                                  |
| mt_bus                                  | Number of bus instances                                                              |
| mt_endpoint                             | Number of receive endpoint instances                                                 |
| mt_consumer_in_progress                 | Number of consumers in progress                                                      |
| mt_handler_in_progress                  | Number of handlers in progress                                                       |
| mt_saga_in_progress                     | Number of sagas in progress                                                          |
| mt_activity_execute_in_progress         | Number of activity executions in progress                                            |
| mt_activity_compensate_in_progress      | Number of activity compensations in progress                                         |
| mt_activity_execute_total               | Total number of activities executed                                                  |
| mt_activity_execute_fault_total         | Total number of activity executions faults                                           |
| mt_activity_execute_duration_seconds    | Elapsed time spent executing an activity, in seconds                                 |
| mt_activity_compensate_total            | Total number of activities compensated                                               |
| mt_activity_compensate_failure_total    | Total number of activity compensation failures                                       |
| mt_activity_compensate_duration_seconds | Elapsed time spent compensating an activity, in seconds                              |

#### Labels

For the metrics above, labels are specified where appropriate.

| Name             | Description                                         |
|:-----------------|:----------------------------------------------------|
| service_name     | The service name specified at bus configuration     |
| endpoint_address | The endpoint address                                |
| message_type     | The message type for the metric                     |
| consumer_type    | The consumer, saga, or activity type for the metric |
| activity_name    | The activity name                                   |
| argument_type    | The activity execute argument type                  |
| log_type         | The activity compensate log type                    |
| exception_type   | The exception type for a fault metric               |

#### Example Docker Compose

```yaml
version: "3.7"

services:
  prometheus:
    image: prom/prometheus
    ports:
     - "9090:9090"
```

**Example MassTransit Prometheus Config File**

> You can use the domain `host.docker.internal` to access process running on the host machine.

```yaml
global:
  scrape_interval: 10s

scrape_configs:
  - job_name: masstransit
    tls_config:
      insecure_skip_verify: true
    scheme: https
    static_configs:
      - targets:
        - 'host.docker.internal:5001'
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

