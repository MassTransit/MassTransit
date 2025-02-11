---

title: RateLimitFilter<TContext>

---

# RateLimitFilter\<TContext\>

Namespace: MassTransit.Middleware

Limits the number of calls through the filter to a specified count per time interval
 specified.

```csharp
public class RateLimitFilter<TContext> : IFilter<TContext>, IProbeSite, IPipe<CommandContext<SetRateLimit>>, IDisposable
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RateLimitFilter\<TContext\>](../masstransit-middleware/ratelimitfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipe\<CommandContext\<SetRateLimit\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **RateLimitFilter(Int32, TimeSpan)**

```csharp
public RateLimitFilter(int rateLimit, TimeSpan interval)
```

#### Parameters

`rateLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`interval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(CommandContext\<SetRateLimit\>)**

```csharp
public Task Send(CommandContext<SetRateLimit> context)
```

#### Parameters

`context` [CommandContext\<SetRateLimit\>](../../masstransit-abstractions/masstransit-contracts/commandcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
