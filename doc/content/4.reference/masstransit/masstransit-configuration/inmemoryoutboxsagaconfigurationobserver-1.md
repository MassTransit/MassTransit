---

title: InMemoryOutboxSagaConfigurationObserver<TSaga>

---

# InMemoryOutboxSagaConfigurationObserver\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class InMemoryOutboxSagaConfigurationObserver<TSaga> : ISagaConfigurationObserver
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxSagaConfigurationObserver\<TSaga\>](../masstransit-configuration/inmemoryoutboxsagaconfigurationobserver-1)<br/>
Implements [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver)

## Constructors

### **InMemoryOutboxSagaConfigurationObserver(IRegistrationContext, ISagaConfigurator\<TSaga\>, Action\<IOutboxConfigurator\>)**

```csharp
public InMemoryOutboxSagaConfigurationObserver(IRegistrationContext context, ISagaConfigurator<TSaga> configurator, Action<IOutboxConfigurator> configure)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **InMemoryOutboxSagaConfigurationObserver(ISetScopedConsumeContext, ISagaConfigurator\<TSaga\>, Action\<IOutboxConfigurator\>)**

```csharp
public InMemoryOutboxSagaConfigurationObserver(ISetScopedConsumeContext setter, ISagaConfigurator<TSaga> configurator, Action<IOutboxConfigurator> configure)
```

#### Parameters

`setter` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`configure` [Action\<IOutboxConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **StateMachineSagaConfigured\<TInstance\>(ISagaConfigurator\<TInstance\>, SagaStateMachine\<TInstance\>)**

```csharp
public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TInstance\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>
