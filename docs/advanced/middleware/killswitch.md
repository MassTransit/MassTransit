# Kill Switch

A Kill Switch is used to prevent failing consumers from moving all the messages from the input queue to the error queue. By monitoring message consumption and tracking message successes and failures, a Kill Switch stops the receive endpoint when a trip threshold has been reached.

Typically, consumer exceptions are transient issues and suspending consumption until a later time when the transient issue may have been resolved.

::: tip
A Kill Switch is the messaging analog of a Circuit Breaker, and operates in a similar manner. However, instead of inducing failure to reduce pressure on a backing service, the kill switch stops consuming messages instead thereby reducing pressure on backing services.

> Read Martin Fowler's description of the pattern [here](http://martinfowler.com/bliki/CircuitBreaker.html).
:::

### Configuration

A Kill Switch can be configured on an individual receive endpoint or all receive endpoints on the bus. To configure a kill switch on all receive endpoints, add the _UseKillSwitch_ method as shown.

```cs
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.UseKillSwitch(options => options
        .SetActivationThreshold(10)
        .SetTripThreshold(0.15)
        .SetRestartTimeout(m: 1));

    cfg.ReceiveEndpoint("some-queue", e =>
    {
        e.Consumer<SomeConsumer>();
    });
});
```

In the above example, the kill switch will activate after _10_ messages have been consumed. If the ratio of failures/attempts exceeds _15%_, the kill switch will trip and stop the receive endpoint. After _1_ minute, the receive endpoint will be restarted. Once restarted, if exceptions are still observed, the receive endpoint will be stopped again for _1_ minute.

To configure the kill switch on a receive endpoint, the syntax is the same as shown.

```cs
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ReceiveEndpoint("some-queue", e =>
    {
        e.UseKillSwitch(options => options
            .SetActivationThreshold(10)
            .SetTripThreshold(0.15)
            .SetRestartTimeout(m: 1));

        e.Consumer<SomeConsumer>();
    });
});
```

### Options

| Option                       | Description                                               |
| ---------------------------- | --------------------------------------------------------- |
| `TrackingPeriod`       | The time window for tracking exceptions |
| `TripThreshold`        | The percentage of failed messages that triggers the kill switch. Should be 0-100, but seriously like 5-10. |
| `ActivationThreshold`  | The number of messages that must be consumed before the kill switch activates. |
| `RestartTimeout`       | The wait time before restarting the receive endpoint   |
| `ExceptionFilter`      | By default, all exceptions are tracked. An exception filter can be configured to only track specific exceptions. |

