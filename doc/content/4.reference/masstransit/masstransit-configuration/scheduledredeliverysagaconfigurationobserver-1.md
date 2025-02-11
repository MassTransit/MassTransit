---

title: ScheduledRedeliverySagaConfigurationObserver<TSaga>

---

# ScheduledRedeliverySagaConfigurationObserver\<TSaga\>

Namespace: MassTransit.Configuration

Configures scheduled message redelivery for a saga, on the saga configurator, which is constrained to
 the message types for that saga, and only applies to the saga prior to the saga repository.

```csharp
public class ScheduledRedeliverySagaConfigurationObserver<TSaga> : ISagaConfigurationObserver
```

#### Type Parameters

`TSaga`<br/>
The saga type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduledRedeliverySagaConfigurationObserver\<TSaga\>](../masstransit-configuration/scheduledredeliverysagaconfigurationobserver-1)<br/>
Implements [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver)

## Constructors

### **ScheduledRedeliverySagaConfigurationObserver(ISagaConfigurator\<TSaga\>, Action\<IRetryConfigurator\>)**

```csharp
public ScheduledRedeliverySagaConfigurationObserver(ISagaConfigurator<TSaga> configurator, Action<IRetryConfigurator> configure)
```

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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
