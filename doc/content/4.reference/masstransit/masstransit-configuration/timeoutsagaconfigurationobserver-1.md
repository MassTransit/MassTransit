---

title: TimeoutSagaConfigurationObserver<TSaga>

---

# TimeoutSagaConfigurationObserver\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class TimeoutSagaConfigurationObserver<TSaga> : ISagaConfigurationObserver
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeoutSagaConfigurationObserver\<TSaga\>](../masstransit-configuration/timeoutsagaconfigurationobserver-1)<br/>
Implements [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver)

## Constructors

### **TimeoutSagaConfigurationObserver(ISagaConfigurator\<TSaga\>, Action\<ITimeoutConfigurator\>)**

```csharp
public TimeoutSagaConfigurationObserver(ISagaConfigurator<TSaga> configurator, Action<ITimeoutConfigurator> configure)
```

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`configure` [Action\<ITimeoutConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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
