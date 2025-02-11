---

title: RateLimitExtensions

---

# RateLimitExtensions

Namespace: MassTransit

```csharp
public static class RateLimitExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RateLimitExtensions](../masstransit/ratelimitextensions)

## Methods

### **SetRateLimit(IPipe\<CommandContext\>, Int32)**

```csharp
public static Task SetRateLimit(IPipe<CommandContext> pipe, int rateLimit)
```

#### Parameters

`pipe` [IPipe\<CommandContext\>](../masstransit/ipipe-1)<br/>

`rateLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
