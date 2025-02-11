---

title: InMemoryOutboxHandlerConfigurationObserver

---

# InMemoryOutboxHandlerConfigurationObserver

Namespace: MassTransit.Configuration

Configures a message retry for a handler, on the handler configurator, which is constrained to
 the message types for that handler, and only applies to the handler.

```csharp
public class InMemoryOutboxHandlerConfigurationObserver : IHandlerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxHandlerConfigurationObserver](../masstransit-configuration/inmemoryoutboxhandlerconfigurationobserver)<br/>
Implements [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)

## Constructors

### **InMemoryOutboxHandlerConfigurationObserver(IRegistrationContext, Action\<IOutboxConfigurator\>)**

```csharp
public InMemoryOutboxHandlerConfigurationObserver(IRegistrationContext context, Action<IOutboxConfigurator> configure)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **InMemoryOutboxHandlerConfigurationObserver(ISetScopedConsumeContext, Action\<IOutboxConfigurator\>)**

```csharp
public InMemoryOutboxHandlerConfigurationObserver(ISetScopedConsumeContext setter, Action<IOutboxConfigurator> configure)
```

#### Parameters

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
