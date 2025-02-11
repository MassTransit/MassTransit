---

title: ScheduledRedeliveryHandlerConfigurationObserver

---

# ScheduledRedeliveryHandlerConfigurationObserver

Namespace: MassTransit.Configuration

Configures a message retry for a handler, on the handler configurator, which is constrained to
 the message types for that handler, and only applies to the handler.

```csharp
public class ScheduledRedeliveryHandlerConfigurationObserver : IHandlerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduledRedeliveryHandlerConfigurationObserver](../masstransit-configuration/scheduledredeliveryhandlerconfigurationobserver)<br/>
Implements [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)

## Constructors

### **ScheduledRedeliveryHandlerConfigurationObserver(Action\<IRetryConfigurator\>)**

```csharp
public ScheduledRedeliveryHandlerConfigurationObserver(Action<IRetryConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
