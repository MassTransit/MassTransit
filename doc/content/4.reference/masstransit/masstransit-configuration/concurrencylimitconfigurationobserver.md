---

title: ConcurrencyLimitConfigurationObserver

---

# ConcurrencyLimitConfigurationObserver

Namespace: MassTransit.Configuration

Adds a concurrency limit filter for each message type configured on the consume pipe

```csharp
public class ConcurrencyLimitConfigurationObserver : ConfigurationObserver, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, IMessageConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IMessageConfigurationObserver\>](../../masstransit-abstractions/masstransit-util/connectable-1) → [ConfigurationObserver](../masstransit-configuration/configurationobserver) → [ConcurrencyLimitConfigurationObserver](../masstransit-configuration/concurrencylimitconfigurationobserver)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [IMessageConfigurationObserver](../../masstransit-abstractions/masstransit/imessageconfigurationobserver)

## Properties

### **Limiter**

```csharp
public IConcurrencyLimiter Limiter { get; }
```

#### Property Value

[IConcurrencyLimiter](../masstransit-middleware/iconcurrencylimiter)<br/>

### **Connected**

```csharp
public IMessageConfigurationObserver[] Connected { get; }
```

#### Property Value

[IMessageConfigurationObserver[]](../../masstransit-abstractions/masstransit/imessageconfigurationobserver)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ConcurrencyLimitConfigurationObserver(IConsumePipeConfigurator, Int32, String)**

```csharp
public ConcurrencyLimitConfigurationObserver(IConsumePipeConfigurator configurator, int concurrentMessageLimit, string id)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **MessageConfigured\<TMessage\>(IConsumePipeConfigurator)**

```csharp
public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

### **Method4()**

```csharp
public void Method4()
```

### **Method5()**

```csharp
public void Method5()
```

### **Method6()**

```csharp
public void Method6()
```
