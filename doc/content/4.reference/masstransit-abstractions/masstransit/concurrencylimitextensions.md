---

title: ConcurrencyLimitExtensions

---

# ConcurrencyLimitExtensions

Namespace: MassTransit

```csharp
public static class ConcurrencyLimitExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrencyLimitExtensions](../masstransit/concurrencylimitextensions)

## Methods

### **SetConcurrencyLimit(IPipe\<CommandContext\>, Int32)**

Set the concurrency limit of the filter

```csharp
public static Task SetConcurrencyLimit(IPipe<CommandContext> pipe, int concurrencyLimit)
```

#### Parameters

`pipe` [IPipe\<CommandContext\>](../masstransit/ipipe-1)<br/>

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
