---

title: ConcurrencyLimitConsumerConfigurationObserver<TConsumer>

---

# ConcurrencyLimitConsumerConfigurationObserver\<TConsumer\>

Namespace: MassTransit.Configuration

Configures a concurrency limit for a consumer, on the consumer configurator, which is constrained to
 the message types for that consumer, and only applies to the consumer prior to the consumer factory.

```csharp
public class ConcurrencyLimitConsumerConfigurationObserver<TConsumer> : IConsumerConfigurationObserver
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrencyLimitConsumerConfigurationObserver\<TConsumer\>](../masstransit-configuration/concurrencylimitconsumerconfigurationobserver-1)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)

## Properties

### **Limiter**

```csharp
public IConcurrencyLimiter Limiter { get; }
```

#### Property Value

[IConcurrencyLimiter](../masstransit-middleware/iconcurrencylimiter)<br/>

## Constructors

### **ConcurrencyLimitConsumerConfigurationObserver(IConsumerConfigurator\<TConsumer\>, Int32, String)**

```csharp
public ConcurrencyLimitConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, int concurrentMessageLimit, string id)
```

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
