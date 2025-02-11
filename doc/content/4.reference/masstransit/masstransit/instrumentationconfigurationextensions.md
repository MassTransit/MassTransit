---

title: InstrumentationConfigurationExtensions

---

# InstrumentationConfigurationExtensions

Namespace: MassTransit

```csharp
public static class InstrumentationConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstrumentationConfigurationExtensions](../masstransit/instrumentationconfigurationextensions)

## Methods

### **UseInstrumentation(IBusFactoryConfigurator, Action\<InstrumentationOptions\>, String)**

Enables instrumentation using the built-in .NET Meter class, which can be collected by OpenTelemetry.
 See https://docs.microsoft.com/en-us/dotnet/core/diagnostics/metrics for details.

```csharp
public static void UseInstrumentation(IBusFactoryConfigurator configurator, Action<InstrumentationOptions> configureOptions, string serviceName)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`configureOptions` [Action\<InstrumentationOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`serviceName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The service name for metrics reporting, defaults to the current process main module filename
