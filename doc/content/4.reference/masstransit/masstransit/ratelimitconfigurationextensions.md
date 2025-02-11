---

title: RateLimitConfigurationExtensions

---

# RateLimitConfigurationExtensions

Namespace: MassTransit

```csharp
public static class RateLimitConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RateLimitConfigurationExtensions](../masstransit/ratelimitconfigurationextensions)

## Methods

### **UseRateLimit\<T\>(IPipeConfigurator\<T\>, Int32, IPipeRouter)**

Specify a rate limit for message processing, so that only the specified number of messages are allowed
 per interval.

```csharp
public static void UseRateLimit<T>(IPipeConfigurator<T> configurator, int rateLimit, IPipeRouter router)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rateLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of messages allowed per interval

`router` [IPipeRouter](../masstransit-middleware/ipiperouter)<br/>
The control pipe used to adjust the rate limit dynamically

### **UseRateLimit\<T\>(IPipeConfigurator\<T\>, Int32, TimeSpan, IPipeRouter)**

Specify a rate limit for message processing, so that only the specified number of messages are allowed
 per interval.

```csharp
public static void UseRateLimit<T>(IPipeConfigurator<T> configurator, int rateLimit, TimeSpan interval, IPipeRouter router)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`rateLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of messages allowed per interval

`interval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The reset interval for each set of messages

`router` [IPipeRouter](../masstransit-middleware/ipiperouter)<br/>
The control pipe used to adjust the rate limit dynamically

### **UseRateLimit(IConsumePipeConfigurator, Int32, TimeSpan)**

Specify a rate limit for message processing, so that only the specified number of messages are allowed
 per interval.

```csharp
public static void UseRateLimit(IConsumePipeConfigurator configurator, int rateLimit, TimeSpan interval)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`rateLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of messages allowed per interval

`interval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The reset interval for each set of messages
