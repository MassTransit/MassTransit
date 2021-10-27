# DiagnosticSource

MassTransit uses Microsoft's `System.Diagnostics.DiagnosticSource` to emit diagnostic events. This allows almost every metric trace provider to connect to MassTransit and monitor it.

To connect, set the current log context prior to bus configuration using:

```csharp
public static async Task Main(string[] args)
{
    var subscription = DiagnosticListener.AllListeners.Subscribe(new DiagnosticObserver());

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
    });
}

public class DiagnosticObserver : IObserver<DiagnosticListener>
{
    public void OnCompleted() { }

    public void OnError(Exception error) { }

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name == "MassTransit")
        {
            // subscribe to the listener with your monitoring tool, etc.
        }
    }
}
```

That's it! Magic is done. Now you need to choose your Trace provider (for example: [Application Insights](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#create-an-application-insights-resource-1), [OpenTracing](https://github.com/opentracing-contrib/csharp-netcore)) and configure to read metrics from your `DiagnosticSource`.

The `OpenTracing.Contrib.NetCore` library subscribes to every diagnostic source under the hood it doesn't require any interaction from your side,
however it neither propagates the remote context nor injects the current context to message headers, so the trace will be
limited to local operations. Also, it won't use `TraceId` and `SpanId` from the `Activity` even when you set the activity default id format to `W3C`,
those ids will be random and cannot be associated with `ActivityId`.

To enable `Application Insights`, add it to the `Startup`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((m, o) => m.IncludeDiagnosticSourceActivities.Add("Listener.Name"));

    services.AddApplicationInsightsTelemetry("InstrumentationKey");
}
```

### Available diagnostic events

You can subscribe to different types of diagnostic events produced by MassTransit. 
Below is the list of names of MassTransit diagnostic sources. Keep in mind that each
operation produces `Start` and `Stop` events using the `Activity`. So, when a message is
consumed, you get `MassTransit.Consumer.Consume.Start` event before the consumer is executed and
`MassTransit.Consumer.Consume.Stop` after the message is consumed.

#### Transport

- MassTransit.Transport.Send
- MassTransit.Transport.Receive

#### Consumer

- MassTransit.Consumer.Consume
- MassTransit.Consumer.Handle

#### Saga

- MassTransit.Saga.Send
- MassTransit.Saga.SendQuery
- MassTransit.Saga.Initiate
- MassTransit.Saga.Orchestrate
- MassTransit.Saga.Observe
- MassTransit.Saga.RaiseEvent

#### Courier

- MassTransit.Activity.Execute
- MassTransit.Activity.Compensate

### Additional resources

- [Activity User Guide](https://github.com/dotnet/runtime/blob/master/src/libraries/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md)
- [DiagnosticSource User Guide](https://github.com/dotnet/runtime/blob/master/src/libraries/System.Diagnostics.DiagnosticSource/src/DiagnosticSourceUsersGuide.md)
