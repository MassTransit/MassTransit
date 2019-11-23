# DiagnosticSource

MassTransit uses Microsoft's `System.Diagnostics.DiagnosticSource` to emit diagnostic events. This allows almost every metric trace provider to connect to MassTransit and monitor it.

To connect, set the current log context prior to bus configuration using:

```csharp
public static async Task Main(string[] args)
{
    var subscription = DiagnosticListener.AllListeners.Subscribe(delegate (DiagnosticListener listener)
    {
        if (listener.Name == "MassTransit")
        {
            // subscribe to the listener with your monitoring tool, etc.
        }
    });

    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
    });
}
```

That's it! Magic is done. Now you need to choose your Trace provider (for example: [Application Insights](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#create-an-application-insights-resource-1), [OpenTrace](https://github.com/opentracing-contrib/csharp-netcore)) and configure to read metrics from your `DiagnosticSource`.

`OpenTrace` subscribes to every diagnostic source under the hood it doesn't require any interaction from your side.

To enable `Application Insights`, add it to the `Startup`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((m, o) => m.IncludeDiagnosticSourceActivities.Add("Listener.Name"));

    services.AddApplicationInsightsTelemetry("InstrumentationKey");
}
```

### Additional resources

- [Activity User Guide](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md)
- [DiagnosticSource User Guide](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/DiagnosticSourceUsersGuide.md)