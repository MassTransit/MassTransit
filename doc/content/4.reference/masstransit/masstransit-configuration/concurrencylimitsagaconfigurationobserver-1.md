---

title: ConcurrencyLimitSagaConfigurationObserver<TSaga>

---

# ConcurrencyLimitSagaConfigurationObserver\<TSaga\>

Namespace: MassTransit.Configuration

Configures a concurrency limit for a consumer, on the consumer configurator, which is constrained to
 the message types for that consumer, and only applies to the consumer prior to the consumer factory.

```csharp
public class ConcurrencyLimitSagaConfigurationObserver<TSaga> : ISagaConfigurationObserver
```

#### Type Parameters

`TSaga`<br/>
The consumer type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrencyLimitSagaConfigurationObserver\<TSaga\>](../masstransit-configuration/concurrencylimitsagaconfigurationobserver-1)<br/>
Implements [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver)

## Properties

### **Limiter**

```csharp
public IConcurrencyLimiter Limiter { get; }
```

#### Property Value

[IConcurrencyLimiter](../masstransit-middleware/iconcurrencylimiter)<br/>

## Constructors

### **ConcurrencyLimitSagaConfigurationObserver(ISagaConfigurator\<TSaga\>, Int32, String)**

```csharp
public ConcurrencyLimitSagaConfigurationObserver(ISagaConfigurator<TSaga> configurator, int concurrentMessageLimit, string id)
```

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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
