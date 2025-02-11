---

title: RedeliverRequestStateMachineSpecification

---

# RedeliverRequestStateMachineSpecification

Namespace: MassTransit.Configuration

```csharp
public class RedeliverRequestStateMachineSpecification : IRequestStateMachineMissingInstanceConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RedeliverRequestStateMachineSpecification](../masstransit-configuration/redeliverrequeststatemachinespecification)<br/>
Implements [IRequestStateMachineMissingInstanceConfigurator](../masstransit/irequeststatemachinemissinginstanceconfigurator)

## Constructors

### **RedeliverRequestStateMachineSpecification(Action\<IMissingInstanceRedeliveryConfigurator\>)**

```csharp
public RedeliverRequestStateMachineSpecification(Action<IMissingInstanceRedeliveryConfigurator> configure)
```

#### Parameters

`configure` [Action\<IMissingInstanceRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **Apply\<TInstance, TMessage\>(IMissingInstanceConfigurator\<TInstance, TMessage\>)**

```csharp
public IPipe<ConsumeContext<TMessage>> Apply<TInstance, TMessage>(IMissingInstanceConfigurator<TInstance, TMessage> configurator)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IMissingInstanceConfigurator\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/imissinginstanceconfigurator-2)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
