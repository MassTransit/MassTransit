---

title: DelayedRedeliveryHandlerConfigurationObserver

---

# DelayedRedeliveryHandlerConfigurationObserver

Namespace: MassTransit.Configuration

Configures a message retry for a handler, on the handler configurator, which is constrained to
 the message types for that handler, and only applies to the handler.

```csharp
public class DelayedRedeliveryHandlerConfigurationObserver : IHandlerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelayedRedeliveryHandlerConfigurationObserver](../masstransit-configuration/delayedredeliveryhandlerconfigurationobserver)<br/>
Implements [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)

## Constructors

### **DelayedRedeliveryHandlerConfigurationObserver(Action\<IRedeliveryConfigurator\>)**

```csharp
public DelayedRedeliveryHandlerConfigurationObserver(Action<IRedeliveryConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
