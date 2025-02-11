---

title: ConcurrencyLimitConfigurationExtensions

---

# ConcurrencyLimitConfigurationExtensions

Namespace: MassTransit

```csharp
public static class ConcurrencyLimitConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrencyLimitConfigurationExtensions](../masstransit/concurrencylimitconfigurationextensions)

## Methods

### **UseConcurrencyLimit\<T\>(IPipeConfigurator\<T\>, Int32, IPipeRouter)**

Specify a concurrency limit for tasks executing through the filter. No more than the specified
 number of tasks will be allowed to execute concurrently.

```csharp
public static void UseConcurrencyLimit<T>(IPipeConfigurator<T> configurator, int concurrencyLimit, IPipeRouter router)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrency limit for the subsequent filters in the pipeline

`router` [IPipeRouter](../masstransit-middleware/ipiperouter)<br/>
A control pipe to support runtime adjustment

### **UseConcurrencyLimit(IConsumePipeConfigurator, Int32)**

Limits the number of concurrent messages consumed on the receive endpoint, regardless of message type.

```csharp
public static void UseConcurrencyLimit(IConsumePipeConfigurator configurator, int concurrentMessageLimit)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrency limit for the subsequent filters in the pipeline

### **UseConcurrencyLimit(IConsumePipeConfigurator, Int32, IReceiveEndpointConfigurator, String)**

Limits the number of concurrent messages consumed on the receive endpoint, regardless of message type.

```csharp
public static void UseConcurrencyLimit(IConsumePipeConfigurator configurator, int concurrentMessageLimit, IReceiveEndpointConfigurator managementEndpointConfigurator, string id)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`concurrentMessageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The concurrency limit for the subsequent filters in the pipeline

`managementEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
A management endpoint configurator to support runtime adjustment

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
An identifier for the concurrency limit to allow selective adjustment
