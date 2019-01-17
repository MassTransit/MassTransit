## Diagnostic Source
When Microsoft release new version of `System.Diagnostics.DiagnosticSource` it gives us an ability to connect almost every Metric Trace provider to our application. MassTransit has built-in support with .NET Standard package version.
To enable it you just need to create `DiagnosticSource` and add a filter
```csharp
public static async Task Main(string[] args)
{
    var source = new DiagnosticListener("Listener.Name");
    var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
    {
        //your configuration
        cfg.UseDiagnosticsActivity(source);
    });
}
```
That's it! Magic is done. Now you need to choose your Trace provider (for example: [Application Insights](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-create-new-resource#create-an-application-insights-resource-1), [OpenTrace](https://github.com/opentracing-contrib/csharp-netcore)) and configure to read metrics from your `DiagnosticSource`.

`OpenTrace` subscribes to every diagnostic source under the hood it doesn't require any interaction from your side.

To enable `Application Insights` using .NET Core application you should enable it on your `Startup`:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((m, o) => m.IncludeDiagnosticSourceActivities.Add("Listener.Name"));
    services.AddApplicationInsightsTelemetry("InstrumentationKey");
    //your configuration
}
```

### Additional resources
- [Actvity User Guide](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md)
- [DiagnosticSource User Guide](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/DiagnosticSourceUsersGuide.md)