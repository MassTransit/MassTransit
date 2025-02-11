---

title: InMemoryOutboxConsumerConfigurationObserver<TConsumer>

---

# InMemoryOutboxConsumerConfigurationObserver\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public class InMemoryOutboxConsumerConfigurationObserver<TConsumer> : IConsumerConfigurationObserver
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxConsumerConfigurationObserver\<TConsumer\>](../masstransit-configuration/inmemoryoutboxconsumerconfigurationobserver-1)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)

## Constructors

### **InMemoryOutboxConsumerConfigurationObserver(IRegistrationContext, IConsumerConfigurator\<TConsumer\>, Action\<IOutboxConfigurator\>)**

```csharp
public InMemoryOutboxConsumerConfigurationObserver(IRegistrationContext context, IConsumerConfigurator<TConsumer> configurator, Action<IOutboxConfigurator> configure)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **InMemoryOutboxConsumerConfigurationObserver(ISetScopedConsumeContext, IConsumerConfigurator\<TConsumer\>, Action\<IOutboxConfigurator\>)**

```csharp
public InMemoryOutboxConsumerConfigurationObserver(ISetScopedConsumeContext setter, IConsumerConfigurator<TConsumer> configurator, Action<IOutboxConfigurator> configure)
```

#### Parameters

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
