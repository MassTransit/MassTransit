---

title: TimeoutHandlerConfigurationObserver

---

# TimeoutHandlerConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class TimeoutHandlerConfigurationObserver : IHandlerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeoutHandlerConfigurationObserver](../masstransit-configuration/timeouthandlerconfigurationobserver)<br/>
Implements [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)

## Constructors

### **TimeoutHandlerConfigurationObserver(Action\<ITimeoutConfigurator\>)**

```csharp
public TimeoutHandlerConfigurationObserver(Action<ITimeoutConfigurator> configure)
```

#### Parameters

`configure` [Action\<ITimeoutConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
