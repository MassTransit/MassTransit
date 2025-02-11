---

title: ConcurrencyLimitHandlerConfigurationObserver

---

# ConcurrencyLimitHandlerConfigurationObserver

Namespace: MassTransit.Configuration

Configures a concurrency limit for a handler, on the handler configurator, which is constrained to
 the message type for that handler, and only applies to the handler.

```csharp
public class ConcurrencyLimitHandlerConfigurationObserver : IHandlerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrencyLimitHandlerConfigurationObserver](../masstransit-configuration/concurrencylimithandlerconfigurationobserver)<br/>
Implements [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)

## Properties

### **Limiter**

```csharp
public IConcurrencyLimiter Limiter { get; }
```

#### Property Value

[IConcurrencyLimiter](../masstransit-middleware/iconcurrencylimiter)<br/>

## Constructors

### **ConcurrencyLimitHandlerConfigurationObserver(Int32, String)**

```csharp
public ConcurrencyLimitHandlerConfigurationObserver(int concurrentMessageLimit, string id)
```

#### Parameters

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **HandlerConfigured\<T\>(IHandlerConfigurator\<T\>)**

```csharp
public void HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<T\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>
