# Prometheus Metrics

[MassTransit.Prometheus](https://www.nuget.org/packages/MassTransit.Prometheus)

MassTransit supports Prometheus metric capture, which provides useful observability into the bus, endpoints, consumers, and messages.

> The `prometheus-net` library is used as the Prometheus client since it is mentioned on the Prometheus client list.

### Configuration

To configure the bus to capture metrics, add the `UsePrometheusMetrics()` method to your bus configuration.

```cs
public void ConfigureServices(IServiceCollection services)
{
    // this registration is simplified
    services.AddMassTransit(x =>
    {

        x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.UsePrometheusMetrics(serviceName: "order_service");
        }));
    });
}
```

#### Publishing Metrics

Metrics must be available on an HTTP endpoint, by default it is `/metrics`. Add the ASP.NET Core integration package to get this endpoint out-of-the-box.

::: tip
To publish metrics so that they can be scraped by Prometheus, an ASP.NET Core Generic Host is recommended, with the Kestrel HTTP server configured.
:::

```
dotnet add package prometheus-net.AspNetCore
``` 

And then, add the metric server to the application builder.

```cs
public void Configure(IApplicationBuilder app)
{
    app.UseMetricServer();
}
```

> For more details, see the [Prometheus-Net Documentation](https://github.com/prometheus-net/prometheus-net#aspnet-core-exporter-middleware).

### Metrics Captured

The metrics captured by MassTransit are listed below.

| Name       | Description |
|:-----------|:------------|
| mt_receive_total | Total number of messages received
| mt_receive_fault_total | Total number of messages receive faults
| mt_receive_duration_seconds | Elapsed time spent receiving messages, in seconds
| mt_receive_in_progress | Number of messages being received
| mt_consume_total | Total number of messages consumed
| mt_consume_fault_total | Total number of message consume faults
| mt_consume_retry_total | Total number of message consume retries
| mt_consume_duration_seconds | Elapsed time spent consuming a message, in seconds
| mt_delivery_duration_seconds | Elapsed time between when the message was sent and when it was consumed, in seconds.
| mt_publish_total | Total number of messages published
| mt_publish_fault_total | Total number of message publish faults
| mt_send_total | Total number of messages sent
| mt_send_fault_total | Total number of message send faults
| mt_bus | Number of bus instances
| mt_endpoint | Number of receive endpoint instances
| mt_consumer_in_progress | Number of consumers in progress
| mt_handler_in_progress | Number of handlers in progress
| mt_saga_in_progress | Number of sagas in progress
| mt_activity_execute_in_progress | Number of activity executions in progress
| mt_activity_compensate_in_progress | Number of activity compensations in progress
| mt_activity_execute_total | Total number of activities executed
| mt_activity_execute_fault_total | Total number of activity executions faults
| mt_activity_execute_duration_seconds | Elapsed time spent executing an activity, in seconds
| mt_activity_compensate_total | Total number of activities compensated
| mt_activity_compensate_failure_total | Total number of activity compensation failures
| mt_activity_compensate_duration_seconds | Elapsed time spent compensating an activity, in seconds


### Labels

For the metrics above, labels are specified where appropriate.

| Name       | Description |
|:-----------|:------------|
| service_name | The service name specified at bus configuration
| endpoint_address | The endpoint address
| message_type | The message type for the metric
| consumer_type | The consumer, saga, or activity type for the metric
| activity_name | The activity name
| argument_type | The activity execute argument type
| log_type | The activity compensate log type
| exception_type | The exception type for a fault metric

